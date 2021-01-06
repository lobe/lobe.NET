using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace lobe
{
    public class OnnxImageClassifier : ImageClassifier
    {
        private InferenceSession _session;
        private Signature _signature;

        public OnnxImageClassifier()
        {
            RegisterForDisposal(() =>
            {
                _session?.Dispose();
            });
        }

        public override ClassificationResults Classify(float[] input)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("No model loaded");
            }

            var shape = _signature.GetInputShape("Image");
            var name = _signature.GetInputId("Image");

            var predictionKey =  _signature.GetOutputId("Prediction");
            var confidencesKey = _signature.GetOutputId("Confidences"); 

            var labels = _signature.Classes.ToArray();
            var data = new DenseTensor<float>(input, shape);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(name, data)
            };

            using var results = _session.Run(inputs);

            var lookup = results.ToDictionary(r => r.Name);

            if (!lookup.TryGetValue(predictionKey, out var predictionResult))
            {
                throw new InvalidOperationException($"Cannot find output {predictionKey}");
            }

            if (!lookup.TryGetValue(confidencesKey, out var confidencesResults))
            {
                throw new InvalidOperationException($"Cannot find output {confidencesKey}");
            }

            var confidences = (confidencesResults.Value as DenseTensor<float>).Buffer.Span;
            var prediction = (predictionResult.Value as DenseTensor<string>).Buffer.Span[0];

            var classifications =  new List<Classification>();
            var maxConfidence = 0.0;
            Classification max = null;
            for (var i = 0; i < confidences.Length; i++)
            {
                var classification = new Classification(labels[i], confidences[i]);
                
                classifications.Add(classification);
                if (classification.Confidence > maxConfidence)
                {
                    maxConfidence = classification.Confidence; 
                    max = classification;
                }
            }

            return new ClassificationResults(max, classifications.OrderByDescending(c => c.Confidence ).ToList());
        }

        public override void InitialiseFromSignature(Signature signature, string modelFileName = null)
        {
            _signature = signature;
            _session?.Dispose();
            var modelFile = _signature.GetModelFile(modelFileName);
            if (!modelFile.Exists)
            {
                throw new FileNotFoundException("Cannot find model file", modelFile.FullName);
            }
           
            _session = new InferenceSession(modelFile.FullName, new SessionOptions
            {
                LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR
            });
        }
    }
}

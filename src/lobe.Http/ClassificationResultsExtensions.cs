using System.Collections.Generic;
using System.Text.Json;

namespace lobe.Http;

public static class ClassificationResultsExtensions
{
    public static ClassificationResults ToClassificationResults(this string json)
    {
        var root = JsonDocument.Parse(json).RootElement;
        return root.ToClassificationResults();
    }

    public static ClassificationResults ToClassificationResults(this JsonElement json)
    {
        var root = json;
        var predictions = root.GetProperty("predictions");

        Classification maxClassification = null;
        var max = 0.0;
        var classifications = new List<Classification>();
        foreach (var prediction in predictions.EnumerateArray())
        {
            var label = prediction.GetProperty("label").GetString();
            var confidence = prediction.GetProperty("confidence").GetDouble();
            var classification = new Classification(label, confidence);
            if (classification.Confidence > max)
            {
                max = classification.Confidence;
                maxClassification = classification;
            }
            classifications.Add(classification);
        }

        var classificationResults =
            new ClassificationResults(
                maxClassification,
                classifications);


        return classificationResults;
    }
}
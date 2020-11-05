using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using lobe.ImageSharp;


namespace lobe.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var signatureFilePath = args[0];
            var modelFile = args[1];
            var modelFormat = args[2];
            var imageToClassify = args[3];

            ImageClassifier.Register("onnx", () => new OnnxImageClassifier());

            using var classifier = ImageClassifier.CreateFromSignatureFile(
                new FileInfo(signatureFilePath),
                modelFile,
                modelFormat);

            if (File.Exists(imageToClassify))
            {
                var results = classifier.Classify(Image
                    .Load(imageToClassify).CloneAs<Rgb24>());

                Console.WriteLine(results.Prediction.Label);
            }
            else if (Directory.Exists(imageToClassify))
            {
                var files  = Directory.GetFiles(imageToClassify);

                foreach (var file in files)
                {
                    var results = classifier.Classify(Image
                        .Load(file).CloneAs<Rgb24>());

                    Console.WriteLine(results.Prediction.Label);
                }
            }
        }
    }
}

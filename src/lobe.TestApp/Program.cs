using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using lobe.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using lobe.ImageSharp;


namespace lobe.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = CreateParser();

            parser.Invoke(args);
          
        }

        private static Parser CreateParser()
        {
            var rootCommand = new RootCommand();

            var signatureFileOption = new Option<FileInfo>("--signature-file");

            var imageFileOption = new Option<FileInfo>("--image-file");

            var imageFolderOption = new Option<FileInfo>("--image-folder");

            var predictionEndpointOption = new Option<Uri>("--prediction-endpoint");

            rootCommand.AddOption(signatureFileOption);
            rootCommand.AddOption(imageFileOption);
            rootCommand.AddOption(imageFolderOption);
            rootCommand.AddOption(predictionEndpointOption);

            rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo, DirectoryInfo, Uri>(
                (signatureFile, imageFile, imageFolder, predictionEndpoint) =>
                {
                    var images = GatherImages(imageFile, imageFolder);

                    if (signatureFile != null)
                    {
                        ImageClassifier.Register("onnx", () => new OnnxImageClassifier());

                        using var classifier = ImageClassifier.CreateFromSignatureFile(
                            new FileInfo(signatureFile.FullName));

                        foreach (var file in images)
                        {
                            var results = Classify(file, classifier.Classify);

                            Console.WriteLine(results.Prediction.Label);
                        }

                        return 0;
                    }
                     
                    if (predictionEndpoint != null)
                    {
                        var classifier = new LobeClient(predictionEndpoint);
                       
                        foreach (var file in images)
                        {
                            var results = Classify(file, classifier.Classify);

                            Console.WriteLine(results.Prediction.Label);
                        }

                        return 0;
                    }

                    return 1;
                });

            return new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

            static ClassificationResults Classify(FileInfo image, ClassifierDelegate<Rgb24> classifier)
            {
                var source = Image.Load<Rgb24>(image.FullName);
                return classifier(source);
            }
        }

        private static FileInfo[] GatherImages(FileInfo imageFile, DirectoryInfo imageFolder)
        {
            return imageFile != null ? new[] {imageFile} : imageFolder.GetFiles();
        }
    }
}

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


namespace lobe.TestApp;

class Program
{
    static void Main(string[] args)
    {
        var parser = CreateParser();

        Environment.Exit(parser.Invoke(args));

    }

    private static Parser CreateParser()
    {
        var rootCommand = new RootCommand
        {
            Description = @"Classify images using onnx model or directly the http endpoint from the lobe app."
        };

        var signatureFileOption = new Option<FileInfo>("--signature-file", "signature file for model loading.");

        var imageFileOption = new Option<FileInfo>("--image-file", "image file to classify.");

        var imageFolderOption = new Option<FileInfo>("--image-folder", "folder that contain images to classify.");

        var predictionEndpointOption = new Option<Uri>("--prediction-endpoint", "prediction endpoint from lobe app.");

        rootCommand.AddOption(signatureFileOption);
        rootCommand.AddOption(imageFileOption);
        rootCommand.AddOption(imageFolderOption);
        rootCommand.AddOption(predictionEndpointOption);

        rootCommand.Handler = CommandHandler.Create<FileInfo, FileInfo, DirectoryInfo, Uri>(
            (signatureFile, imageFile, imageFolder, predictionEndpoint) =>
            {
                var images = GatherImages(imageFile, imageFolder);

                if (signatureFile is null && predictionEndpoint is null)
                {
                    throw new InvalidOperationException("Must use a signature file or prediction endpoint.");
                }

                if (imageFile is null && imageFolder is null)
                {
                    throw new InvalidOperationException("Must use a image file or image folder.");
                }

                if (signatureFile != null)
                {
                    if (!signatureFile.Exists)
                    {
                        throw new InvalidOperationException(
                            $"Signature file {signatureFile.FullName} does not exist.");
                    }

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
        if (imageFile != null && !imageFile.Exists)
        {
            throw new InvalidOperationException(
                $"image file {imageFile.FullName} does not exist.");
        }

        if (imageFolder != null && !imageFolder.Exists)
        {
            throw new InvalidOperationException(
                $"image folder {imageFolder.FullName} does not exist.");
        }

        return imageFile != null ? new[] { imageFile } : imageFolder.GetFiles();
    }
}
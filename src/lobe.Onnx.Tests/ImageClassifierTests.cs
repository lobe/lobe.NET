using System;
using System.IO;

using lobe.ImageSharp;

using FluentAssertions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace lobe.Onnx.Tests;

public class ImageClassifierTests
{
    public ImageClassifierTests()
    {
        ImageClassifier.Register("onnx", () => new OnnxImageClassifier());
    }

    [Fact]
    public void can_initialise_from_signature()
    {
        var classifier = new OnnxImageClassifier();
        Action initialization = () =>
            classifier.InitialiseFromSignatureFile(
                new FileInfo(@"../../../../../models/Bakugan/signature.json"));

        initialization.Should()
            .NotThrow();
    }

    [Fact]
    public void can_from_signature_via_factory()
    {
        Action initialization = () =>
            ImageClassifier.CreateFromSignatureFile(
                new FileInfo(@"../../../../../models/Bakugan/signature.json"),
                "model.onnx", "onnx");

        initialization.Should()
            .NotThrow();
    }

    [Theory]
    [InlineData("../../../../../models/Bakugan/test set/hidorous.jpg", "Aquos Hydorous (Battle Planet)")]
    [InlineData("../../../../../models/Bakugan/test set/lupitheon.jpg", "Aurelus Lupitheon Ultra (Battle Planet)")]
    public void can_classify_image(string imageFilePath, string expectedLabel)
    {
        using var classifier = ImageClassifier.CreateFromSignatureFile(
            new FileInfo(@"../../../../../models/Bakugan/signature.json"));

        var results = classifier.Classify(Image.Load(imageFilePath).CloneAs<Rgb24>());

        results.Prediction.Label.Should()
            .Be(expectedLabel);
    }
}
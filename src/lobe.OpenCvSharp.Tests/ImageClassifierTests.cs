using System;
using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace lobe.OpenCvSharp.Tests
{
    public class ImageClassifierTests
    {
        [Fact]
        public void cropping_to_square_image_with_black_padding()
        {
            using var image = new Image<Rgb24>(128, 256, Color.White);

            // we put a black square in the middle
            image.Mutate(context =>
            {
                context.Invert(new Rectangle(48, 112, 32, 32));
            });

            using var stream = new MemoryStream();
            image.Save(stream, new BmpEncoder());

            stream.TryGetBuffer(out ArraySegment<byte> buffer);
            var source = Mat.FromImageData(buffer);
            using var square = source.CropToSquare();
            using var _ = new AssertionScope();

            square.Width.Should().Be(128);
            square.Height.Should().Be(128);
            square.Get<Vec3b>(0, 0).Should().Be(new Vec3b(255,255,255));
            square.Get<Vec3b>(64, 64).Should().Be(new Vec3b(0, 0, 0));
            square.Get<Vec3b>(127, 127).Should().Be(new Vec3b(255, 255, 255));
        }

        [Fact]
        public void transform_image_according_to_signature()
        {
            var signature = Signature.FromFile(new FileInfo("./signature.json"));
            using var image = new Image<Rgb24>(1024, 1024, Color.White);
            using var stream = new MemoryStream();
            image.Save(stream, new BmpEncoder());
            stream.TryGetBuffer(out ArraySegment<byte> buffer);
            var source = Mat.FromImageData(buffer);

            var shape = signature.GetInputShape("Image");
            var flatData = source.ToFlatArrayMatchingInputShape(signature, "Image");
            var width = shape[1];
            var height = shape[2];
            var pixelSize = shape[3];

            using var _ = new AssertionScope();
            flatData.Length.Should().Be(width * height * pixelSize);
            flatData.Should().AllBeEquivalentTo(1f);
        }
    }
}

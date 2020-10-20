using System.IO;
using System.Linq;

using FluentAssertions;
using FluentAssertions.Execution;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Xunit;

namespace lobe.ImageSharp.Tests
{
    public class ImageProcessingTests
    {
        [Fact]
        public void square_image_with_black_padding()
        {
            using var source = new Image<Rgba32>(128, 256, Color.White);
            using var square = source.ToSquare();

            using var _ = new AssertionScope();

            square.Width.Should().Be(256);
            square.Height.Should().Be(256);
            square[0, 0].Rgb.Should().Be(Color.Black.ToPixel<Rgb24>());
            square[172, 255].Rgb.Should().Be(Color.White.ToPixel<Rgb24>());
            square[255, 255].Rgb.Should().Be(Color.Black.ToPixel<Rgb24>());
        }

        [Fact]
        public void cropping_to_square_image_with_black_padding()
        {
            using var source = new Image<Rgba32>(128, 256, Color.White);

            // we put a black square in the middle
            source.Mutate(context =>
            {
                context.Invert(new Rectangle(48, 112, 32, 32));
            });


            using var square = source.CropToSquare();
            using var _ = new AssertionScope();

            square.Width.Should().Be(128);
            square.Height.Should().Be(128);
            square[0, 0].Rgb.Should().Be(Color.White.ToPixel<Rgb24>());
            square[64, 64].Rgb.Should().Be(Color.Black.ToPixel<Rgb24>());
            square[127, 127].Rgb.Should().Be(Color.White.ToPixel<Rgb24>());
        }

        [Fact]
        public void transform_image_according_to_signature()
        {
            var signature = Signature.FromFile(new FileInfo("./signature.json"));
            using var source = new Image<Rgb24>(1024, 1024, Color.White);

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

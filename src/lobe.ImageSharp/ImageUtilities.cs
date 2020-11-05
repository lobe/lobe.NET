using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace lobe.ImageSharp
{
    public static class ImageUtilities
    {
        /// <summary>
        /// Creates a squared image with black padding mathcing the max between Width and Height of the source image.
        /// </summary>
        /// <typeparam name="TPixel">The Pixel type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>A squared image</returns>
        public static Image<TPixel> ToSquare<TPixel>(this Image<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Width == source.Height)
            {
                return source;
            }

            var width = source.Width;
            var height = source.Height;

            var squareSize = Math.Max(width, height);

            var squared = new Image<TPixel>(squareSize, squareSize, Color.Black.ToPixel<TPixel>());

            squared.Mutate(context =>
            {
                context.DrawImage(source, new Point((squareSize - width) / 2, (squareSize - height) / 2), 1.0f);
            });

            return squared;
        }

        /// <summary>
        /// Crops the image to a square mathcing the min between Width and Height of the source image.
        /// </summary>
        /// <typeparam name="TPixel">The Pixel type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>A squared image</returns>
        public static Image<TPixel> CropToSquare<TPixel>(this Image<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(source.Width == source.Height)
            {
                return source;
            }

            var width = source.Width;
            var height = source.Height;

            var squareSize = Math.Min(width, height);

            var squared = source.Clone(context => context.Crop(new Rectangle((width - squareSize) / 2, (height - squareSize) / 2, squareSize, squareSize)));
            return squared;
        }

        public static float[] ToFlatArrayMatchingInputShape<TPixel>(this Image<TPixel> source, Signature signature,
            string inputLabel) where TPixel : unmanaged, IPixel<TPixel>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (signature == null)
            {
                throw new ArgumentNullException(nameof(signature));
            }

            if (signature.GetInputType(inputLabel) != "float32")
            {
                throw new InvalidOperationException($"Input {inputLabel} is not a float type");
            }

            var shape = signature.GetInputShape(inputLabel);
            var width = shape[1];
            var height = shape[2];
            var pixelSize = shape[3];
            var dataSize = width * height * pixelSize;

            if (dataSize <= 0)
            {
                throw new InvalidOperationException($"Shape {shape} is invalid.");
            }
            var conformed = new float[dataSize];
            using var resized = source.CropToSquare();
            
            
            resized.Mutate(context =>
            {
                context.Resize(width, height);
            });

            for (var y = 0; y < resized.Height; y++)
            {
                var row = resized.GetPixelRowSpan(y);
                var offset = y * resized.Width;
                for (var x = 0; x < resized.Width; x++)
                {
                    var vector = row[x].ToVector4();
                    var dst = (x + offset) * pixelSize;
                    conformed[dst] = vector.X;
                    conformed[dst + 1] = vector.Y;
                    conformed[dst + 2] = vector.Z;
                }
            }
            return conformed;
        }
    }
}
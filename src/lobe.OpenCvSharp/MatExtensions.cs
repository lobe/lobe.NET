using System;

using OpenCvSharp;

namespace lobe.OpenCvSharp
{
    public static class MatExtensions
    {

        public static Mat CropToSquare(this Mat source)
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

            var squareSize = Math.Min(width, height);

            var squared = new Mat(source, new Rect((width - squareSize) / 2, (height - squareSize) / 2, squareSize, squareSize));
            return squared;
        }

        /// <summary>
        /// Converts the image to a flat float array with normalized values. <remarks>The source image must be converted to RGB color space.</remarks>
        /// </summary>
        /// <param name="source">Source image in RGB color space</param>
        /// <param name="signature">Signature file</param>
        /// <param name="inputLabel">Input label</param>
        /// <returns>A flat array with normalized values</returns>
        public static float[] ToFlatArrayMatchingInputShape(this Mat source, Signature signature,
            string inputLabel)
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
            using var resized = source.CropToSquare().Resize(new Size(width, height));

            for (var y = 0; y < resized.Height; y++)
            {
                var offset = y * resized.Width;
                for (var x = 0; x < resized.Width; x++)
                {
                    var dst = (x + offset) * pixelSize;
                    var (r, g, b) = resized.Get<Vec3b>(y,x);
                    conformed[dst] = r / 255.0f;
                    conformed[dst + 1] = g / 255.0f;
                    conformed[dst + 2] = b / 255.0f;
                }
            }
            return conformed;
        }
    }
}
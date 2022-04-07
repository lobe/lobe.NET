using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace lobe.ImageSharp;

public static class ImageUtilities
{
    /// <summary>
    /// Creates a squared image with black padding matching the max between Width and Height of the source image.
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
    /// Crops the image to a square matching the min between Width and Height of the source image.
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

    /// <summary>
    /// Converts the image to a flat float array with normalized values. <remarks>The source image must be converted to RGB color space.</remarks>
    /// </summary>
    /// <param name="source">Source image in RGB color space</param>
    /// <param name="signature">Signature file</param>
    /// <param name="inputLabel">Input label</param>
    /// <returns>A flat array with normalized values</returns>
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

        resized.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                var offset = y * accessor.Width;
                for (var x = 0; x < accessor.Width; x++)
                {
                    var vector = row[x].ToVector4();
                    var dst = (x + offset) * pixelSize;
                    conformed[dst] = vector.X;
                    conformed[dst + 1] = vector.Y;
                    conformed[dst + 2] = vector.Z;
                }
            }
        });

           
        return conformed;
    }
        
    /// <summary>
    /// Extracts the center part of the image
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <param name="asSquare">If true the focused area is a square.</param>
    /// <returns>The image portion.</returns>
    public static Image Focus(this Image source, bool asSquare = true)
    {
        var rect = CreateFocusRectangle(source, asSquare);
        return source.Clone(c => c.Crop(rect));
    }

    /// <summary>
    /// Extracts the center part of the image
    /// </summary>
    /// <typeparam name="TPixel">The Pixel type.</typeparam>
    /// <param name="source">The source image.</param>
    /// <param name="asSquare">If true the focused area is a square.</param>
    /// <returns>The image portion.</returns>
    public static Image<TPixel> Focus<TPixel>(this Image<TPixel> source, bool asSquare = true) where TPixel : unmanaged, IPixel<TPixel>
    {
        var rect = CreateFocusRectangle(source, asSquare);
        return source.Clone(c => c.Crop(rect));
    }

    /// <summary>
    /// Create a resized copy of the image.
    /// </summary>
    /// <param name="source">Source.</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height.</param>
    /// <returns>The image.</returns>
    public static Image CloneAndResize(this Image source, int width, int height)
    {
        return source.Clone(c => c.Resize(width, height));
    }

    /// <summary>
    /// Create a resized copy of the image.
    /// </summary>
    /// <typeparam name="TPixel">The Pixel type.</typeparam>
    /// <param name="source">Source.</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height.</param>
    /// <returns>The image.</returns>
    public static Image<TPixel> CloneAndResize<TPixel>(this Image<TPixel> source, int width, int height) where TPixel : unmanaged, IPixel<TPixel>
    {
        return source.Clone(c => c.Resize(width, height));
    }

    private static Rectangle CreateFocusRectangle(IImageInfo source, bool asSquare)
    {
        var width = source.Width / 2;
        var height = source.Height / 2;
        if (asSquare)
        {
            width = height = Math.Min(height, width);
        }
        var x = (source.Width - width) / 2;
        var y = (source.Height - height) / 2;
        var rect = new Rectangle(x, y, width, height);
        return rect;
    }
}
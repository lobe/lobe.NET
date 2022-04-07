using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace lobe.ImageSharp;

public static class ImageClassifierExtensions
{
    public static ClassificationResults Classify<TPixel>(this ImageClassifier classifier,
        Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
    {
        return classifier.Classify(image.ToFlatArrayMatchingInputShape(classifier.Signature, "Image"));
    }
}
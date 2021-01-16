using OpenCvSharp;

namespace lobe.OpenCvSharp
{
    public static class ImageClassifierExtensions
    {
        public static ClassificationResults Classify(this ImageClassifier classifier,
            Mat image)
        {
            return classifier.Classify(image.ToFlatArrayMatchingInputShape(classifier.Signature, "Image"));
        }
    }
}

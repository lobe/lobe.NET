using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace lobe.ImageSharp
{
    public delegate ClassificationResults ClassifierDelegate<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>;
}
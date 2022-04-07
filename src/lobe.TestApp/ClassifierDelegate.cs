using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace lobe.TestApp;

public delegate ClassificationResults ClassifierDelegate<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>;
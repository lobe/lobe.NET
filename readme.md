# .NET libraries for Lobe

A .NET library to run inference on exported Lobe models.

## How to get started

### Export the model from Lobe app

* Export a TensorFlow model from a project you have in the Lobe applciation
* Install the [tf2onnx](https://github.com/onnx/tensorflow-onnx) tool
* Convert the TensorFlow model to ONNX following using the command ```python -m tf2onnx.convert --saved-model path/that/contains/saved_model/ --output model.onnx```


### Use the model in your own .NET application

Install the follwing packages
* ```lobe.Onnx``` to import the [Onnx](https://github.com/Microsoft/onnxruntime) based implementation of the image classifier.
*  ```lobe.ImageSharp``` to get image manipulation utilities 
*  ```Microsoft.ML.OnnxRuntime``` to get he native onnx runtimes

This code creates a simple command line app that loads a model and classifies an image file
```cs
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using lobe.ImageSharp;

namespace lobe.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var signatureFilePath = args[0];
            var imageToClassify = args[1];

            ImageClassifier.Register("onnx", () => new OnnxImageClassifier());
            using var classifier = ImageClassifier.CreateFromSignatureFile(
                new FileInfo(signatureFilePath));

            var results = classifier.Classify(Image
                .Load(imageToClassify).CloneAs<Rgb24>());
            Console.WriteLine(results.Classification.Label);
        }
    }
}
```
The code
```cs
ImageClassifier.Register("onnx", () => new OnnxImageClassifier());
```

Registers the ```OnnxImageClassifier``` against the format ```onnx```

Then a classifier can be built from a signature file

```cs
using var classifier = ImageClassifier.CreateFromSignatureFile(new FileInfo(signatureFilePath));

```

!! At this moment you need to override the model file path and the model format to `onnx` or you can override the values of `format` and `filename` in the `signature.json` to look like this
```json
{
    ...
    "format": "onnx",
    ...
    "filename": "model.onnx"
}
```
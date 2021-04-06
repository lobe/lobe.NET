# .NET libraries for Lobe

A .NET library to run inference on exported Lobe models.

## How to get started

### Export the model from Lobe app

* Export  your model as ONNX format

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

### Use the lobe app directly

For rapid iterations you can test your model by taking advantage of the http endpoint that the lobe app exposes. First, open the app and then the model you would like to use. Next, go to the export menu and select the api option, get the url from there (it should look like ```http://localhost:38100/predict/bdff75cc-ee54-46cf-a290-f9095ef78516"```).

In your .NET App make sure you have installed the following nuget packages
* ```lobe.Http```
* ```lobe.ImageSharp```

Then import the namespaces
```csharp
using System.IO;
using SixLabors.ImageSharp;

using lobe.Http;

```

And finally create the client, connect to the endpoint and now use it classify the image
```csharp
var client = new LobeClient();
client.UseUri(new Uri("http://localhost:38100/predict/bdff75cc-ee54-46cf-a290-f9095ef78516"));

var result = client.Classify(picture.CloneAs<Rgb24>());

```

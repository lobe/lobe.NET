using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace lobe.Http
{
    public class LobeClient
    {
        private Uri _predictionEndpoint;
        private HttpClient _client;


        public LobeClient()
        {

        }

        public LobeClient(Uri predictionEndpoint)
        {
            UseUri(predictionEndpoint);
        }
        public void UseUri(Uri predictionEndpoint)
        {
            _predictionEndpoint = predictionEndpoint;
            _client = new HttpClient();
        }



        public ClassificationResults Classify<TPixel>(Image<TPixel> frame) where TPixel : unmanaged, IPixel<TPixel>
        {
            ClassificationResults results = null;


            if (_predictionEndpoint != null)
            {
                results = UseService(frame);
            }

            return results;

        }

        private ClassificationResults UseService(Image frame)
        {
            var image = frame.CloneAs<Rgb24>();
            using var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            stream.Flush();
            var data = stream.ToArray();
            var imageSource = $"{Convert.ToBase64String(data)}";

            var content = new StringContent($"{{ \"inputs\": {{ \"Image\":  \"{imageSource}\" }} }}", Encoding.UTF8,
                "application/json");
            var response = _client.PostAsync(_predictionEndpoint, content).Result;
            var body = response.Content.ReadAsStringAsync().Result;

            var classification = JObject.Parse(body);

            var classifications = classification.SelectToken("outputs.Labels").Values<JArray>()
                .Select(ja => new Classification(ja[0].Value<string>(), ja[1].Value<double>())).ToArray();


            var classificationResults =
                new ClassificationResults(
                    classifications.First(c => c.Label == classification.SelectToken("outputs.Prediction[0]").Value<string>()),
                    classifications);


            return classificationResults;
        }
    }
}

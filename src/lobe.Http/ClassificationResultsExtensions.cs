using System.Linq;
using Newtonsoft.Json.Linq;

namespace lobe.Http
{
    public static class ClassificationResultsExtensions
    {
        public static ClassificationResults ToClassificationResults(this string json)
        {
            var classification = JObject.Parse(json);

            var classifications = classification.SelectToken("outputs.Labels").Values<JArray>()
                .Select(ja => new Classification(Extensions.Value<string>(ja[0]), Extensions.Value<double>(ja[1]))).ToArray();


            var classificationResults =
                new ClassificationResults(
                    classifications.First(c => c.Label == classification.SelectToken("outputs.Prediction[0]").Value<string>()),
                    classifications);


            return classificationResults;
        }
    }
}
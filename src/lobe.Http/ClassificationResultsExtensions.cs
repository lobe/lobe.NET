using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace lobe.Http
{
    public static class ClassificationResultsExtensions
    {
        public static ClassificationResults ToClassificationResults(this string json)
        {
            var root = JsonDocument.Parse(json).RootElement;
            var outputs = root.EnumerateObject().Single(p => p.Name == "outputs").Value;
            var labels = outputs.GetProperty("Labels");
            var prediction = outputs.GetProperty("Prediction").EnumerateArray().FirstOrDefault().GetString()?? string.Empty;

            var classifications = new List<Classification>();
            foreach (var label in labels.EnumerateArray())
            {
                var parts = label.EnumerateArray().ToArray();
                var classification = new Classification(parts[0].GetString(), parts[1].GetDouble());
                classifications.Add(classification);
            }

            var classificationResults =
                new ClassificationResults(
                    classifications.First(c => c.Label == prediction),
                    classifications);


            return classificationResults;
        }
    }
}
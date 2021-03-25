using System.Linq;
using FluentAssertions;
using Xunit;

namespace lobe.Http.Tests
{
    public class ResultParsingTests 
    {
        [Fact]
        public void can_deserialize()
        {
            var src = @"{
    ""predictions"": [
        {
            ""label"": ""Aurelus Lupitheon Ultra (Battle Planet)"",
            ""confidence"": 1
        },
        {
            ""label"": ""Aquos Hydorous (Battle Planet)"",
            ""confidence"": 0
        },
        {
            ""label"": ""Aquos Trox Ultra (Battle Planet)"",
            ""confidence"": 0
        },
        {
            ""label"": ""Aurelus Pyravian Ultra (Battle Planet)"",
            ""confidence"": 0
        },
        {
            ""label"": ""Darkus Hydorous (Battle Planet)"",
            ""confidence"": 0
        }
    ]
}";

            var result = src.ToClassificationResults();

            result.Should().NotBeNull();
            result.Prediction.Label.Should().Be("Aurelus Lupitheon Ultra (Battle Planet)");
            result.Classifications.Select(c => c.Label).Should().BeEquivalentTo(

                "Aurelus Lupitheon Ultra (Battle Planet)", "Aquos Hydorous (Battle Planet)", "Aquos Trox Ultra (Battle Planet)", "Aurelus Pyravian Ultra (Battle Planet)", "Darkus Hydorous (Battle Planet)"
            );
        }
    }
}

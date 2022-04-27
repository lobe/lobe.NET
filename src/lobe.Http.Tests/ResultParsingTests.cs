using System.Linq;
using FluentAssertions;
using Xunit;

namespace lobe.Http.Tests;

public class ResultParsingTests 
{
    [Fact]
    public void can_deserialize()
    {
        var src = @"{""predictions"":[{""label"":""Chiaro"",""confidence"":0.999998927116394},{""label"":""Bianco Leggero"",""confidence"":0.0000011116593441329314},{""label"":""Mexico"",""confidence"":8.473352575144588e-10},{""label"":""Hazelino Muffin"",""confidence"":1.7132049945356442e-10},{""label"":""No Capsule"",""confidence"":3.861575295638353e-11}]}";

        var result = src.ToClassificationResults();

        result.Should().NotBeNull();
        result.Prediction.Label.Should().Be("Chiaro");
        result.Classifications.Select(c => c.Label).Should().BeEquivalentTo(

            "Chiaro", "Bianco Leggero", "Mexico", "Hazelino Muffin", "No Capsule"
        );
    }
}
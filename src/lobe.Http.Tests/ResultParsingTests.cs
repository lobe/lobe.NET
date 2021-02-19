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
    ""outputs"": {
        ""Labels"": [
            [
                ""Dragonoid Pyrus"",
                2.9784330557447447e-10
            ],
            [
                ""Fangzor Aquos"",
                5.896686134168605e-13
            ],
            [
                ""Gorthion Haos"",
                4.2895406671128455e-10
            ],
            [
                ""Gorthion Ventus"",
                1.5067771160522222e-16
            ],
            [
                ""Howlkor Ultra Haos"",
                4.38083805343048e-11
            ],
            [
                ""Hydorous Ultra Pyrus "",
                1.7968190378235462e-15
            ],
            [
                ""Hydorous X Thryno Ultra Pyrus Aurelus"",
                2.984291711527476e-07
            ],
            [
                ""Lupitheon Ultra Aurelus"",
                0.8843237161636353
            ],
            [
                ""Nillius Haos"",
                1.99084368782132e-12
            ],
            [
                ""No Bakugan"",
                2.932698862423422e-06
            ],
            [
                ""Pegatrix Aurelus"",
                5.779965807188592e-13
            ],
            [
                ""Pegratrix X Goreen Ultra Haos / Aurelus"",
                8.831047892954302e-09
            ],
            [
                ""Phaedrus Pyrus"",
                0.031188247725367546
            ],
            [
                ""Pyravian Ultra Aurelus"",
                0.08448425680398941
            ],
            [
                ""Serpenteze Aquos"",
                4.100685763175349e-11
            ],
            [
                ""Thryno Pyrus"",
                5.200028567742265e-07
            ],
            [
                ""Vicerox Ultra Haos"",
                7.508459409777402e-10
            ]
        ],
        ""Prediction"": [
            ""Lupitheon Ultra Aurelus""
        ]
    }
}";

            var result = src.ToClassificationResults();

            result.Should().NotBeNull();
            result.Prediction.Label.Should().Be("Lupitheon Ultra Aurelus");
        }
    }
}

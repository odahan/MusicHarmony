using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Primitives;

public sealed class PitchClassRulesTests
{
    [Theory]
    [InlineData(-25, 11)]
    [InlineData(-24, 0)]
    [InlineData(-13, 11)]
    [InlineData(-12, 0)]
    [InlineData(-1, 11)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(11, 11)]
    [InlineData(12, 0)]
    [InlineData(13, 1)]
    [InlineData(24, 0)]
    [InlineData(25, 1)]
    [InlineData(int.MinValue, 4)]
    [InlineData(int.MaxValue, 7)]
    public void Pitch_class_is_normalized_to_twelve_values(int input, int expected)
    {
        var pitchClass = PitchClass.FromChromaticIndex(input);

        Assert.Equal(expected, pitchClass.Value);
        Assert.InRange(pitchClass.Value, 0, 11);
    }

    [Fact]
    public void Upward_distance_is_exhaustive_and_normalized()
    {
        for (var source = 0; source < 12; source++)
        {
            for (var target = 0; target < 12; target++)
            {
                var actual = PitchClass
                    .FromChromaticIndex(source)
                    .DistanceUpTo(PitchClass.FromChromaticIndex(target));
                var expected = ((target - source) % 12 + 12) % 12;

                Assert.Equal(expected, actual);
                Assert.InRange(actual, 0, 11);
            }
        }
    }
}

using Enaxos.MusicTheory.Intervals;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Intervals;

public sealed class IntervalBetweenRulesTests
{
    [Theory]
    [InlineData("C4", "E4", IntervalDirection.Ascending, 3, 4, IntervalQualityKind.Major)]
    [InlineData("C#4", "Db4", IntervalDirection.Ascending, 2, 0, IntervalQualityKind.Diminished)]
    [InlineData("C#4", "C4", IntervalDirection.Ascending, 1, -1, IntervalQualityKind.Diminished)]
    [InlineData("C4", "E5", IntervalDirection.Ascending, 10, 16, IntervalQualityKind.Major)]
    [InlineData("C5", "E4", IntervalDirection.Descending, 6, 8, IntervalQualityKind.Minor)]
    [InlineData("C#4", "Cb4", IntervalDirection.Descending, 1, 2, IntervalQualityKind.Augmented)]
    public void Written_interval_uses_both_diatonic_and_chromatic_distance(
        string from,
        string to,
        IntervalDirection direction,
        int expectedNumber,
        int expectedSemitones,
        IntervalQualityKind expectedQuality)
    {
        var interval = Interval.Between(Note.Parse(from), Note.Parse(to), direction);

        Assert.Equal(expectedNumber, interval.Number);
        Assert.Equal(expectedSemitones, interval.Semitones);
        Assert.Equal(expectedQuality, interval.Quality.Kind);
    }

    [Theory]
    [InlineData("C4", "B3", IntervalDirection.Ascending)]
    [InlineData("C4", "D4", IntervalDirection.Descending)]
    public void Target_must_lie_in_the_requested_diatonic_direction(
        string from,
        string to,
        IntervalDirection direction)
    {
        Assert.Throws<ArgumentException>(() =>
            Interval.Between(Note.Parse(from), Note.Parse(to), direction));
    }

    [Fact]
    public void Undefined_direction_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Interval.Between(Note.Parse("C4"), Note.Parse("D4"), (IntervalDirection)99));
    }
}

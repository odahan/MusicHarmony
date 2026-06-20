using Enaxos.MusicTheory.Intervals;

namespace Enaxos.MusicTheory.Tests.Intervals;

public sealed class IntervalInversionRulesTests
{
    [Fact]
    public void Every_representative_simple_interval_inverts_to_nine_and_twelve()
    {
        for (var number = 1; number <= 8; number++)
        {
            var reference = ReferenceSemitones(number);
            for (var offset = -3; offset <= 3; offset++)
            {
                var interval = Interval.FromDistances(number, reference + offset);
                var inverse = interval.InvertSimple();

                Assert.Equal(9, interval.Number + inverse.Number);
                Assert.Equal(12, interval.Semitones + inverse.Semitones);
                Assert.Equal(interval, inverse.InvertSimple());
            }
        }
    }

    [Theory]
    [InlineData(IntervalQualityKind.Major, IntervalQualityKind.Minor)]
    [InlineData(IntervalQualityKind.Minor, IntervalQualityKind.Major)]
    [InlineData(IntervalQualityKind.Perfect, IntervalQualityKind.Perfect)]
    [InlineData(IntervalQualityKind.Augmented, IntervalQualityKind.Diminished)]
    [InlineData(IntervalQualityKind.Diminished, IntervalQualityKind.Augmented)]
    public void Inversion_maps_quality_families(
        IntervalQualityKind sourceKind,
        IntervalQualityKind expectedKind)
    {
        var source = sourceKind switch
        {
            IntervalQualityKind.Major => Interval.Create(
                3,
                new IntervalQuality(IntervalQualityKind.Major)),
            IntervalQualityKind.Minor => Interval.Create(
                3,
                new IntervalQuality(IntervalQualityKind.Minor)),
            IntervalQualityKind.Perfect => Interval.Create(
                4,
                new IntervalQuality(IntervalQualityKind.Perfect)),
            IntervalQualityKind.Augmented => Interval.Create(
                4,
                new IntervalQuality(IntervalQualityKind.Augmented, 1)),
            IntervalQualityKind.Diminished => Interval.Create(
                5,
                new IntervalQuality(IntervalQualityKind.Diminished, 1)),
            _ => throw new InvalidOperationException(),
        };

        Assert.Equal(expectedKind, source.InvertSimple().Quality.Kind);
    }

    [Fact]
    public void Compound_interval_cannot_be_inverted_as_simple()
    {
        var compound = Interval.Create(
            9,
            new IntervalQuality(IntervalQualityKind.Major));

        Assert.Throws<InvalidOperationException>(() => compound.InvertSimple());
    }

    private static int ReferenceSemitones(int number) => number switch
    {
        1 => 0,
        2 => 2,
        3 => 4,
        4 => 5,
        5 => 7,
        6 => 9,
        7 => 11,
        8 => 12,
        _ => throw new ArgumentOutOfRangeException(nameof(number)),
    };
}

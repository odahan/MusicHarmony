using Enaxos.MusicTheory.Intervals;

namespace Enaxos.MusicTheory.Tests.Intervals;

public sealed class IntervalConstructionRulesTests
{
    public static TheoryData<int, IntervalQualityKind, int, int> SpecificIntervals => new()
    {
        { 1, IntervalQualityKind.Diminished, 1, -1 },
        { 1, IntervalQualityKind.Perfect, 0, 0 },
        { 1, IntervalQualityKind.Augmented, 1, 1 },
        { 2, IntervalQualityKind.Diminished, 1, 0 },
        { 2, IntervalQualityKind.Minor, 0, 1 },
        { 2, IntervalQualityKind.Major, 0, 2 },
        { 2, IntervalQualityKind.Augmented, 1, 3 },
        { 3, IntervalQualityKind.Minor, 0, 3 },
        { 3, IntervalQualityKind.Major, 0, 4 },
        { 4, IntervalQualityKind.Perfect, 0, 5 },
        { 4, IntervalQualityKind.Augmented, 2, 7 },
        { 5, IntervalQualityKind.Diminished, 2, 5 },
        { 5, IntervalQualityKind.Perfect, 0, 7 },
        { 6, IntervalQualityKind.Minor, 0, 8 },
        { 6, IntervalQualityKind.Major, 0, 9 },
        { 7, IntervalQualityKind.Minor, 0, 10 },
        { 7, IntervalQualityKind.Major, 0, 11 },
        { 8, IntervalQualityKind.Perfect, 0, 12 },
        { 9, IntervalQualityKind.Major, 0, 14 },
        { 10, IntervalQualityKind.Minor, 0, 15 },
        { 10, IntervalQualityKind.Major, 0, 16 },
        { 15, IntervalQualityKind.Perfect, 0, 24 },
    };

    [Theory]
    [MemberData(nameof(SpecificIntervals))]
    public void Quality_and_number_determine_the_chromatic_distance(
        int number,
        IntervalQualityKind kind,
        int degree,
        int expectedSemitones)
    {
        var interval = Interval.Create(number, new IntervalQuality(kind, degree));

        Assert.Equal(number, interval.Number);
        Assert.Equal(expectedSemitones, interval.Semitones);
        Assert.Equal(kind, interval.Quality.Kind);
        Assert.Equal(degree, interval.Quality.Degree);
        Assert.Equal(number > 8, interval.IsCompound);
    }

    [Fact]
    public void Default_interval_is_a_valid_perfect_unison()
    {
        var interval = default(Interval);

        Assert.Equal(1, interval.Number);
        Assert.Equal(0, interval.Semitones);
        Assert.Equal(IntervalQualityKind.Perfect, interval.Quality.Kind);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Interval_number_must_be_positive(int number)
    {
        var quality = new IntervalQuality(IntervalQualityKind.Perfect);

        Assert.Throws<ArgumentOutOfRangeException>(() => Interval.Create(number, quality));
        Assert.Throws<ArgumentOutOfRangeException>(() => Interval.FromDistances(number, 0));
    }

    [Theory]
    [InlineData(IntervalQualityKind.Augmented, 0)]
    [InlineData(IntervalQualityKind.Augmented, -1)]
    [InlineData(IntervalQualityKind.Diminished, 0)]
    [InlineData(IntervalQualityKind.Diminished, -1)]
    [InlineData(IntervalQualityKind.Major, 1)]
    [InlineData(IntervalQualityKind.Minor, 1)]
    [InlineData(IntervalQualityKind.Perfect, 1)]
    public void Quality_degree_is_validated(IntervalQualityKind kind, int degree)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new IntervalQuality(kind, degree));
    }

    [Fact]
    public void Undefined_quality_kind_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new IntervalQuality((IntervalQualityKind)99));
    }

    [Theory]
    [InlineData(1, IntervalQualityKind.Major)]
    [InlineData(4, IntervalQualityKind.Minor)]
    [InlineData(5, IntervalQualityKind.Major)]
    [InlineData(2, IntervalQualityKind.Perfect)]
    [InlineData(3, IntervalQualityKind.Perfect)]
    [InlineData(6, IntervalQualityKind.Perfect)]
    [InlineData(7, IntervalQualityKind.Perfect)]
    public void Quality_family_must_match_the_interval_number(
        int number,
        IntervalQualityKind kind)
    {
        Assert.Throws<ArgumentException>(() =>
            Interval.Create(number, new IntervalQuality(kind)));
    }

    [Fact]
    public void Common_intervals_have_the_normative_distances()
    {
        var expected = new (Interval Interval, int Number, int Semitones)[]
        {
            (CommonIntervals.PerfectUnison, 1, 0),
            (CommonIntervals.MinorSecond, 2, 1),
            (CommonIntervals.MajorSecond, 2, 2),
            (CommonIntervals.MinorThird, 3, 3),
            (CommonIntervals.MajorThird, 3, 4),
            (CommonIntervals.PerfectFourth, 4, 5),
            (CommonIntervals.AugmentedFourth, 4, 6),
            (CommonIntervals.DiminishedFifth, 5, 6),
            (CommonIntervals.PerfectFifth, 5, 7),
            (CommonIntervals.MinorSixth, 6, 8),
            (CommonIntervals.MajorSixth, 6, 9),
            (CommonIntervals.MinorSeventh, 7, 10),
            (CommonIntervals.MajorSeventh, 7, 11),
            (CommonIntervals.PerfectOctave, 8, 12),
        };

        foreach (var item in expected)
        {
            Assert.Equal(item.Number, item.Interval.Number);
            Assert.Equal(item.Semitones, item.Interval.Semitones);
        }
    }
}

using Enaxos.MusicTheory.Intervals;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Intervals;

public sealed class TranspositionRulesTests
{
    [Theory]
    [InlineData("C4", 3, IntervalQualityKind.Major, 0, "E4")]
    [InlineData("C#4", 3, IntervalQualityKind.Major, 0, "E#4")]
    [InlineData("Db4", 3, IntervalQualityKind.Major, 0, "F4")]
    [InlineData("B#4", 2, IntervalQualityKind.Major, 0, "C##5")]
    [InlineData("C4", 10, IntervalQualityKind.Major, 0, "E5")]
    [InlineData("C#4", 1, IntervalQualityKind.Diminished, 1, "C4")]
    public void Ascending_transposition_preserves_required_spelling(
        string source,
        int number,
        IntervalQualityKind quality,
        int qualityDegree,
        string expected)
    {
        var interval = Interval.Create(number, new IntervalQuality(quality, qualityDegree));

        Assert.Equal(expected, Transposition.Transpose(Note.Parse(source), interval).ToString());
    }

    [Theory]
    [InlineData("C4", 3, IntervalQualityKind.Major, 0, "Ab3")]
    [InlineData("C#4", 2, IntervalQualityKind.Major, 0, "B3")]
    [InlineData("F4", 4, IntervalQualityKind.Augmented, 1, "Cb4")]
    [InlineData("C4", 10, IntervalQualityKind.Major, 0, "Ab2")]
    [InlineData("C4", 1, IntervalQualityKind.Diminished, 1, "C#4")]
    public void Descending_transposition_preserves_required_spelling(
        string source,
        int number,
        IntervalQualityKind quality,
        int qualityDegree,
        string expected)
    {
        var interval = Interval.Create(number, new IntervalQuality(quality, qualityDegree));

        var result = Transposition.Transpose(
            Note.Parse(source),
            interval,
            IntervalDirection.Descending);

        Assert.Equal(expected, result.ToString());
    }

    [Theory]
    [InlineData("C#", 3, IntervalQualityKind.Major, 0, IntervalDirection.Ascending, "E#")]
    [InlineData("B#", 2, IntervalQualityKind.Major, 0, IntervalDirection.Ascending, "C##")]
    [InlineData("C", 3, IntervalQualityKind.Major, 0, IntervalDirection.Descending, "Ab")]
    [InlineData("F", 4, IntervalQualityKind.Augmented, 1, IntervalDirection.Descending, "Cb")]
    public void Pitch_without_octave_preserves_diatonic_and_chromatic_distance(
        string source,
        int number,
        IntervalQualityKind quality,
        int qualityDegree,
        IntervalDirection direction,
        string expected)
    {
        var interval = Interval.Create(number, new IntervalQuality(quality, qualityDegree));

        var result = Transposition.Transpose(SpelledPitch.Parse(source), interval, direction);

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void Note_round_trip_restores_exact_writing_exhaustively()
    {
        var intervals = RepresentativeIntervals();

        foreach (var letter in Enum.GetValues<NoteLetter>())
        {
            for (var accidental = -2; accidental <= 2; accidental++)
            {
                for (var octave = -2; octave <= 6; octave++)
                {
                    var source = new Note(
                        new SpelledPitch(letter, Accidental.FromSemitones(accidental)),
                        octave);

                    foreach (var interval in intervals)
                    {
                        AssertRoundTrip(source, interval, IntervalDirection.Ascending);
                        AssertRoundTrip(source, interval, IntervalDirection.Descending);
                    }
                }
            }
        }
    }

    [Fact]
    public void Pitch_round_trip_restores_exact_writing_exhaustively()
    {
        var intervals = RepresentativeIntervals();

        foreach (var letter in Enum.GetValues<NoteLetter>())
        {
            for (var accidental = -2; accidental <= 2; accidental++)
            {
                var source = new SpelledPitch(
                    letter,
                    Accidental.FromSemitones(accidental));

                foreach (var interval in intervals)
                {
                    var upward = Transposition.Transpose(source, interval);
                    var restoredFromUp = Transposition.Transpose(
                        upward,
                        interval,
                        IntervalDirection.Descending);
                    Assert.Equal(source, restoredFromUp);

                    var downward = Transposition.Transpose(
                        source,
                        interval,
                        IntervalDirection.Descending);
                    var restoredFromDown = Transposition.Transpose(downward, interval);
                    Assert.Equal(source, restoredFromDown);
                }
            }
        }
    }

    [Fact]
    public void Undefined_direction_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Transposition.Transpose(
                Note.Parse("C4"),
                CommonIntervals.MajorSecond,
                (IntervalDirection)99));
    }

    private static void AssertRoundTrip(
        Note source,
        Interval interval,
        IntervalDirection firstDirection)
    {
        var opposite = firstDirection == IntervalDirection.Ascending
            ? IntervalDirection.Descending
            : IntervalDirection.Ascending;
        var transposed = Transposition.Transpose(source, interval, firstDirection);
        var measured = Interval.Between(source, transposed, firstDirection);
        var restored = Transposition.Transpose(transposed, interval, opposite);

        Assert.Equal(interval, measured);
        Assert.Equal(source, restored);
    }

    private static Interval[] RepresentativeIntervals() =>
    [
        CommonIntervals.PerfectUnison,
        CommonIntervals.MinorSecond,
        CommonIntervals.MajorSecond,
        CommonIntervals.MinorThird,
        CommonIntervals.MajorThird,
        CommonIntervals.PerfectFourth,
        CommonIntervals.AugmentedFourth,
        CommonIntervals.DiminishedFifth,
        CommonIntervals.PerfectFifth,
        CommonIntervals.MinorSixth,
        CommonIntervals.MajorSixth,
        CommonIntervals.MinorSeventh,
        CommonIntervals.MajorSeventh,
        CommonIntervals.PerfectOctave,
        Interval.Create(1, new IntervalQuality(IntervalQualityKind.Diminished, 1)),
        Interval.Create(10, new IntervalQuality(IntervalQualityKind.Major)),
    ];
}

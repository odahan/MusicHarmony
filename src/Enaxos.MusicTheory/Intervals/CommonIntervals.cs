namespace Enaxos.MusicTheory.Intervals;

/// <summary>Provides reusable canonical instances of the most common simple intervals.</summary>
public static class CommonIntervals
{
    /// <summary>Gets a perfect unison (0 semitones).</summary>
    public static Interval PerfectUnison { get; } =
        Interval.Create(1, new IntervalQuality(IntervalQualityKind.Perfect));

    /// <summary>Gets a minor second (1 semitone).</summary>
    public static Interval MinorSecond { get; } =
        Interval.Create(2, new IntervalQuality(IntervalQualityKind.Minor));

    /// <summary>Gets a major second (2 semitones).</summary>
    public static Interval MajorSecond { get; } =
        Interval.Create(2, new IntervalQuality(IntervalQualityKind.Major));

    /// <summary>Gets a minor third (3 semitones).</summary>
    public static Interval MinorThird { get; } =
        Interval.Create(3, new IntervalQuality(IntervalQualityKind.Minor));

    /// <summary>Gets a major third (4 semitones).</summary>
    public static Interval MajorThird { get; } =
        Interval.Create(3, new IntervalQuality(IntervalQualityKind.Major));

    /// <summary>Gets a perfect fourth (5 semitones).</summary>
    public static Interval PerfectFourth { get; } =
        Interval.Create(4, new IntervalQuality(IntervalQualityKind.Perfect));

    /// <summary>Gets an augmented fourth (6 semitones).</summary>
    public static Interval AugmentedFourth { get; } =
        Interval.Create(4, new IntervalQuality(IntervalQualityKind.Augmented, 1));

    /// <summary>Gets a diminished fifth (6 semitones).</summary>
    public static Interval DiminishedFifth { get; } =
        Interval.Create(5, new IntervalQuality(IntervalQualityKind.Diminished, 1));

    /// <summary>Gets a perfect fifth (7 semitones).</summary>
    public static Interval PerfectFifth { get; } =
        Interval.Create(5, new IntervalQuality(IntervalQualityKind.Perfect));

    /// <summary>Gets a minor sixth (8 semitones).</summary>
    public static Interval MinorSixth { get; } =
        Interval.Create(6, new IntervalQuality(IntervalQualityKind.Minor));

    /// <summary>Gets a major sixth (9 semitones).</summary>
    public static Interval MajorSixth { get; } =
        Interval.Create(6, new IntervalQuality(IntervalQualityKind.Major));

    /// <summary>Gets a minor seventh (10 semitones).</summary>
    public static Interval MinorSeventh { get; } =
        Interval.Create(7, new IntervalQuality(IntervalQualityKind.Minor));

    /// <summary>Gets a major seventh (11 semitones).</summary>
    public static Interval MajorSeventh { get; } =
        Interval.Create(7, new IntervalQuality(IntervalQualityKind.Major));

    /// <summary>Gets a perfect octave (12 semitones).</summary>
    public static Interval PerfectOctave { get; } =
        Interval.Create(8, new IntervalQuality(IntervalQualityKind.Perfect));
}

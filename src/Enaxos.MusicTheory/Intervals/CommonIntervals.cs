namespace Enaxos.MusicTheory.Intervals;

public static class CommonIntervals
{
    public static Interval PerfectUnison { get; } =
        Interval.Create(1, new IntervalQuality(IntervalQualityKind.Perfect));

    public static Interval MinorSecond { get; } =
        Interval.Create(2, new IntervalQuality(IntervalQualityKind.Minor));

    public static Interval MajorSecond { get; } =
        Interval.Create(2, new IntervalQuality(IntervalQualityKind.Major));

    public static Interval MinorThird { get; } =
        Interval.Create(3, new IntervalQuality(IntervalQualityKind.Minor));

    public static Interval MajorThird { get; } =
        Interval.Create(3, new IntervalQuality(IntervalQualityKind.Major));

    public static Interval PerfectFourth { get; } =
        Interval.Create(4, new IntervalQuality(IntervalQualityKind.Perfect));

    public static Interval AugmentedFourth { get; } =
        Interval.Create(4, new IntervalQuality(IntervalQualityKind.Augmented, 1));

    public static Interval DiminishedFifth { get; } =
        Interval.Create(5, new IntervalQuality(IntervalQualityKind.Diminished, 1));

    public static Interval PerfectFifth { get; } =
        Interval.Create(5, new IntervalQuality(IntervalQualityKind.Perfect));

    public static Interval MinorSixth { get; } =
        Interval.Create(6, new IntervalQuality(IntervalQualityKind.Minor));

    public static Interval MajorSixth { get; } =
        Interval.Create(6, new IntervalQuality(IntervalQualityKind.Major));

    public static Interval MinorSeventh { get; } =
        Interval.Create(7, new IntervalQuality(IntervalQualityKind.Minor));

    public static Interval MajorSeventh { get; } =
        Interval.Create(7, new IntervalQuality(IntervalQualityKind.Major));

    public static Interval PerfectOctave { get; } =
        Interval.Create(8, new IntervalQuality(IntervalQualityKind.Perfect));
}

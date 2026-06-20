namespace Enaxos.MusicTheory.Intervals;

public readonly record struct IntervalQuality
{
    public IntervalQuality(IntervalQualityKind kind, int degree = 0)
    {
        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        var expectedDegree = kind is IntervalQualityKind.Augmented or IntervalQualityKind.Diminished
            ? degree >= 1
            : degree == 0;

        if (!expectedDegree)
        {
            throw new ArgumentOutOfRangeException(
                nameof(degree),
                "Augmented and diminished qualities require a positive degree; other qualities require zero.");
        }

        Kind = kind;
        Degree = degree;
    }

    public IntervalQualityKind Kind { get; }

    public int Degree { get; }

    internal static IntervalQuality Diminished(int degree) =>
        new(IntervalQualityKind.Diminished, degree);

    internal static IntervalQuality Minor { get; } = new(IntervalQualityKind.Minor);

    internal static IntervalQuality Perfect { get; } = new(IntervalQualityKind.Perfect);

    internal static IntervalQuality Major { get; } = new(IntervalQualityKind.Major);

    internal static IntervalQuality Augmented(int degree) =>
        new(IntervalQualityKind.Augmented, degree);

    internal void EnsureValid(string parameterName)
    {
        if (!Enum.IsDefined(Kind) ||
            (Kind is IntervalQualityKind.Augmented or IntervalQualityKind.Diminished
                ? Degree < 1
                : Degree != 0))
        {
            throw new ArgumentException("The interval quality is invalid.", parameterName);
        }
    }
}

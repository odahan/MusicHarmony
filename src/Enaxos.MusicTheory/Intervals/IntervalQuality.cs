namespace Enaxos.MusicTheory.Intervals;

/// <summary>Represents an interval quality, including multiply augmented or diminished degrees.</summary>
public readonly record struct IntervalQuality
{
    /// <summary>Creates and validates an interval quality.</summary>
    /// <param name="kind">The base quality family.</param>
    /// <param name="degree">The augmentation or diminution degree; zero for major, minor, or perfect.</param>
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

    /// <summary>Gets the base quality family.</summary>
    public IntervalQualityKind Kind { get; }

    /// <summary>Gets the positive degree for augmented or diminished qualities, otherwise zero.</summary>
    public int Degree { get; }

    /// <summary>Creates an internally derived diminished quality.</summary>
    internal static IntervalQuality Diminished(int degree) =>
        new(IntervalQualityKind.Diminished, degree);

    /// <summary>Gets the validated minor quality.</summary>
    internal static IntervalQuality Minor { get; } = new(IntervalQualityKind.Minor);

    /// <summary>Gets the validated perfect quality.</summary>
    internal static IntervalQuality Perfect { get; } = new(IntervalQualityKind.Perfect);

    /// <summary>Gets the validated major quality.</summary>
    internal static IntervalQuality Major { get; } = new(IntervalQualityKind.Major);

    /// <summary>Creates an internally derived augmented quality.</summary>
    internal static IntervalQuality Augmented(int degree) =>
        new(IntervalQualityKind.Augmented, degree);

    /// <summary>
    /// Revalidates a value that may have been produced through the default struct state or deserialization.
    /// </summary>
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

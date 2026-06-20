using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Intervals;

/// <summary>Represents a directed-independent musical interval by diatonic number and chromatic distance.</summary>
/// <remarks>
/// The diatonic number is inclusive: a unison is one and an octave is eight. Semitones are
/// stored exactly and may be negative for valid spellings such as a diminished unison.
/// </remarks>
public readonly record struct Interval
{
    /// <summary>The greatest diatonic number treated as a simple interval.</summary>
    private const int SimpleIntervalLimit = 8;

    // A zero-based backing value makes default(Interval) expose the valid number one.
    private readonly int _zeroBasedNumber;

    private Interval(int number, int semitones)
    {
        _zeroBasedNumber = number - 1;
        Semitones = semitones;
    }

    /// <summary>Gets the inclusive one-based diatonic interval number.</summary>
    public int Number => _zeroBasedNumber + 1;

    /// <summary>Gets the exact chromatic distance in semitones.</summary>
    public int Semitones { get; }

    /// <summary>Gets the quality derived from the number and chromatic distance.</summary>
    public IntervalQuality Quality => DetermineQuality(Number, Semitones);

    /// <summary>Gets whether the interval is larger than an octave by diatonic number.</summary>
    public bool IsCompound => Number > SimpleIntervalLimit;

    /// <summary>Creates an interval from a diatonic number and a compatible quality.</summary>
    /// <param name="number">The inclusive one-based diatonic number.</param>
    /// <param name="quality">A quality valid for the number's perfect or major class.</param>
    /// <returns>The corresponding interval.</returns>
    public static Interval Create(int number, IntervalQuality quality)
    {
        ValidateNumber(number);
        quality.EnsureValid(nameof(quality));

        // Perfect-class and major-class intervals use different diminished baselines:
        // diminished is one step below perfect, but two steps below major.
        var perfectClass = IsPerfectClass(number);
        long adjustment = (perfectClass, quality.Kind) switch
        {
            (true, IntervalQualityKind.Perfect) => 0,
            (true, IntervalQualityKind.Augmented) => quality.Degree,
            (true, IntervalQualityKind.Diminished) => -quality.Degree,
            (false, IntervalQualityKind.Major) => 0,
            (false, IntervalQualityKind.Minor) => -1,
            (false, IntervalQualityKind.Augmented) => quality.Degree,
            (false, IntervalQualityKind.Diminished) => -(long)quality.Degree - 1,
            _ => throw new ArgumentException(
                "The quality is not valid for this interval number.",
                nameof(quality)),
        };

        var semitones = ReferenceSemitones(number) + adjustment;
        if (semitones is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "The resulting chromatic distance must fit in a 32-bit signed integer.");
        }

        return new Interval(number, (int)semitones);
    }

    /// <summary>Creates an interval from exact diatonic and chromatic distances.</summary>
    /// <param name="diatonicNumber">The inclusive one-based diatonic number.</param>
    /// <param name="semitones">The exact chromatic distance, including negative values.</param>
    public static Interval FromDistances(int diatonicNumber, int semitones)
    {
        ValidateNumber(diatonicNumber);
        _ = DetermineQuality(diatonicNumber, semitones);
        return new Interval(diatonicNumber, semitones);
    }

    /// <summary>Measures the interval between two spelled notes in a requested direction.</summary>
    /// <remarks>The target must lie in the requested diatonic direction; spelling determines the interval number.</remarks>
    public static Interval Between(
        Note from,
        Note to,
        IntervalDirection direction = IntervalDirection.Ascending)
    {
        var fromDiatonic = DiatonicPosition(from);
        var toDiatonic = DiatonicPosition(to);

        var diatonicSteps = direction switch
        {
            IntervalDirection.Ascending => toDiatonic - fromDiatonic,
            IntervalDirection.Descending => fromDiatonic - toDiatonic,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        if (diatonicSteps < 0)
        {
            throw new ArgumentException(
                "The target note does not lie in the requested diatonic direction.",
                nameof(to));
        }

        if (diatonicSteps >= int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(to),
                "The diatonic interval number must fit in a 32-bit signed integer.");
        }

        long chromaticDistance = direction switch
        {
            IntervalDirection.Ascending => (long)to.AbsoluteSemitone - from.AbsoluteSemitone,
            IntervalDirection.Descending => (long)from.AbsoluteSemitone - to.AbsoluteSemitone,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        if (chromaticDistance is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(to),
                "The chromatic distance must fit in a 32-bit signed integer.");
        }

        return FromDistances((int)diatonicSteps + 1, (int)chromaticDistance);
    }

    /// <summary>Inverts a simple interval so the diatonic numbers sum to nine and semitones sum to twelve.</summary>
    /// <returns>The complementary simple interval.</returns>
    /// <exception cref="InvalidOperationException">The interval is compound or the result cannot be represented.</exception>
    public Interval InvertSimple()
    {
        if (IsCompound)
        {
            throw new InvalidOperationException("Only simple intervals can be inverted.");
        }

        var invertedSemitones = 12L - Semitones;
        if (invertedSemitones is < int.MinValue or > int.MaxValue)
        {
            throw new InvalidOperationException(
                "The inverted chromatic distance cannot be represented.");
        }

        return FromDistances(9 - Number, (int)invertedSemitones);
    }

    /// <summary>Enforces the inclusive positive numbering convention.</summary>
    private static void ValidateNumber(int number)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "An interval number must be at least one.");
        }
    }

    /// <summary>Derives a quality by comparing the exact distance with the major/perfect reference.</summary>
    private static IntervalQuality DetermineQuality(int number, int semitones)
    {
        var offset = (long)semitones - ReferenceSemitones(number);

        if (IsPerfectClass(number))
        {
            if (offset == 0)
            {
                return IntervalQuality.Perfect;
            }

            return offset > 0
                ? IntervalQuality.Augmented(ToQualityDegree(offset, number))
                : IntervalQuality.Diminished(ToQualityDegree(-offset, number));
        }

        if (offset == 0)
        {
            return IntervalQuality.Major;
        }

        if (offset == -1)
        {
            return IntervalQuality.Minor;
        }

        return offset > 0
            ? IntervalQuality.Augmented(ToQualityDegree(offset, number))
            : IntervalQuality.Diminished(ToQualityDegree(-offset - 1, number));
    }

    /// <summary>Safely narrows a computed augmentation or diminution degree.</summary>
    private static int ToQualityDegree(long degree, int number)
    {
        if (degree is < 1 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "The interval quality degree must fit in a 32-bit signed integer.");
        }

        return (int)degree;
    }

    /// <summary>Tests the repeating unison/fourth/fifth perfect-class pattern.</summary>
    private static bool IsPerfectClass(int number)
    {
        var simpleNumber = ((number - 1) % 7) + 1;
        return simpleNumber is 1 or 4 or 5;
    }

    /// <summary>Gets the major or perfect chromatic reference distance for any compound number.</summary>
    private static long ReferenceSemitones(int number)
    {
        ValidateNumber(number);

        var zeroBasedNumber = number - 1;
        var octaves = zeroBasedNumber / 7;
        var simpleNumber = (zeroBasedNumber % 7) + 1;
        var simpleSemitones = simpleNumber switch
        {
            1 => 0,
            2 => 2,
            3 => 4,
            4 => 5,
            5 => 7,
            6 => 9,
            7 => 11,
            _ => throw new InvalidOperationException("Unexpected interval number."),
        };

        return (12L * octaves) + simpleSemitones;
    }

    /// <summary>Projects a spelled note onto an unbounded seven-letter coordinate.</summary>
    private static long DiatonicPosition(Note note) =>
        (7L * note.Octave) + (int)note.Pitch.Letter;
}

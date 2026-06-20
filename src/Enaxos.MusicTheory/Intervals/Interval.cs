using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Intervals;

public readonly record struct Interval
{
    private const int SimpleIntervalLimit = 8;
    private readonly int _zeroBasedNumber;

    private Interval(int number, int semitones)
    {
        _zeroBasedNumber = number - 1;
        Semitones = semitones;
    }

    public int Number => _zeroBasedNumber + 1;

    public int Semitones { get; }

    public IntervalQuality Quality => DetermineQuality(Number, Semitones);

    public bool IsCompound => Number > SimpleIntervalLimit;

    public static Interval Create(int number, IntervalQuality quality)
    {
        ValidateNumber(number);
        quality.EnsureValid(nameof(quality));

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

    public static Interval FromDistances(int diatonicNumber, int semitones)
    {
        ValidateNumber(diatonicNumber);
        _ = DetermineQuality(diatonicNumber, semitones);
        return new Interval(diatonicNumber, semitones);
    }

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

    private static void ValidateNumber(int number)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "An interval number must be at least one.");
        }
    }

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

    private static bool IsPerfectClass(int number)
    {
        var simpleNumber = ((number - 1) % 7) + 1;
        return simpleNumber is 1 or 4 or 5;
    }

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

    private static long DiatonicPosition(Note note) =>
        (7L * note.Octave) + (int)note.Pitch.Letter;
}

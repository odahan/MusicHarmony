using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Intervals;

/// <summary>Applies exact intervals while preserving the required diatonic target letter.</summary>
public static class Transposition
{
    /// <summary>Transposes an octave-independent spelling by an interval.</summary>
    /// <remarks>
    /// The target letter comes from the diatonic distance; the accidental is then calculated
    /// so the target also satisfies the exact chromatic distance.
    /// </remarks>
    public static SpelledPitch Transpose(
        SpelledPitch source,
        Interval interval,
        IntervalDirection direction = IntervalDirection.Ascending)
    {
        var sign = DirectionSign(direction);
        var diatonicTarget = (long)(int)source.Letter + (sign * (interval.Number - 1L));
        var (targetOctave, targetLetterIndex) = DivideDiatonicPosition(diatonicTarget);
        var targetLetter = (NoteLetter)targetLetterIndex;

        var sourceChromatic =
            (long)NaturalSemitone(source.Letter) + source.Accidental.Semitones;
        var targetChromatic = sourceChromatic + (sign * (long)interval.Semitones);
        var targetNatural = (12L * targetOctave) + NaturalSemitone(targetLetter);
        var accidental = targetChromatic - targetNatural;

        return new SpelledPitch(
            targetLetter,
            Accidental.FromSemitones(ToAccidental(accidental)));
    }

    /// <summary>Transposes a spelled note, preserving both diatonic spelling and absolute chromatic distance.</summary>
    /// <exception cref="ArgumentOutOfRangeException">A resulting coordinate cannot fit in the public integer representation.</exception>
    public static Note Transpose(
        Note source,
        Interval interval,
        IntervalDirection direction = IntervalDirection.Ascending)
    {
        var sign = DirectionSign(direction);
        var sourceDiatonic = (7L * source.Octave) + (int)source.Pitch.Letter;
        var targetDiatonic = sourceDiatonic + (sign * (interval.Number - 1L));
        var (targetOctave, targetLetterIndex) = DivideDiatonicPosition(targetDiatonic);

        if (targetOctave is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(interval),
                "The resulting octave must fit in a 32-bit signed integer.");
        }

        var targetAbsolute =
            (long)source.AbsoluteSemitone + (sign * (long)interval.Semitones);
        if (targetAbsolute is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(interval),
                "The resulting absolute semitone must fit in a 32-bit signed integer.");
        }

        var targetLetter = (NoteLetter)targetLetterIndex;
        var targetNatural = (12L * targetOctave) + NaturalSemitone(targetLetter);
        var accidental = targetAbsolute - targetNatural;
        var targetPitch = new SpelledPitch(
            targetLetter,
            Accidental.FromSemitones(ToAccidental(accidental)));

        return new Note(targetPitch, (int)targetOctave);
    }

    /// <summary>Converts the domain direction into the sign used by coordinate arithmetic.</summary>
    private static int DirectionSign(IntervalDirection direction) => direction switch
    {
        IntervalDirection.Ascending => 1,
        IntervalDirection.Descending => -1,
        _ => throw new ArgumentOutOfRangeException(nameof(direction)),
    };

    /// <summary>
    /// Divides an unbounded letter coordinate using floor division so descending values retain
    /// a non-negative letter index and the correct lower octave.
    /// </summary>
    private static (long Quotient, int Remainder) DivideDiatonicPosition(long value)
    {
        var quotient = Math.DivRem(value, 7, out var remainder);
        if (remainder < 0)
        {
            quotient--;
            remainder += 7;
        }

        return (quotient, (int)remainder);
    }

    /// <summary>Safely narrows an accidental computed with overflow-resistant arithmetic.</summary>
    private static int ToAccidental(long semitones)
    {
        if (semitones is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(semitones),
                "The resulting accidental must fit in a 32-bit signed integer.");
        }

        return (int)semitones;
    }

    /// <summary>Returns the natural chromatic offset needed to solve for the target accidental.</summary>
    private static int NaturalSemitone(NoteLetter letter) => letter switch
    {
        NoteLetter.C => 0,
        NoteLetter.D => 2,
        NoteLetter.E => 4,
        NoteLetter.F => 5,
        NoteLetter.G => 7,
        NoteLetter.A => 9,
        NoteLetter.B => 11,
        _ => throw new ArgumentOutOfRangeException(nameof(letter)),
    };
}

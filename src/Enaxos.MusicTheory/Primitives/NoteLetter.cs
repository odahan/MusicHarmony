namespace Enaxos.MusicTheory.Primitives;

/// <summary>Identifies one of the seven diatonic note letters.</summary>
public enum NoteLetter
{
    /// <summary>The letter C.</summary>
    C,
    /// <summary>The letter D.</summary>
    D,
    /// <summary>The letter E.</summary>
    E,
    /// <summary>The letter F.</summary>
    F,
    /// <summary>The letter G.</summary>
    G,
    /// <summary>The letter A.</summary>
    A,
    /// <summary>The letter B.</summary>
    B,
}

/// <summary>Provides invariant chromatic and textual data for validated note letters.</summary>
internal static class NoteLetterInfo
{
    /// <summary>Returns the letter's natural semitone position within a C-based octave.</summary>
    internal static int NaturalSemitone(NoteLetter letter) => letter switch
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

    /// <summary>Returns the culture-independent Latin name of a note letter.</summary>
    internal static char InvariantName(NoteLetter letter) => letter switch
    {
        NoteLetter.C => 'C',
        NoteLetter.D => 'D',
        NoteLetter.E => 'E',
        NoteLetter.F => 'F',
        NoteLetter.G => 'G',
        NoteLetter.A => 'A',
        NoteLetter.B => 'B',
        _ => throw new ArgumentOutOfRangeException(nameof(letter)),
    };
}

namespace Enaxos.MusicTheory.Primitives;

public enum NoteLetter
{
    C,
    D,
    E,
    F,
    G,
    A,
    B,
}

internal static class NoteLetterInfo
{
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

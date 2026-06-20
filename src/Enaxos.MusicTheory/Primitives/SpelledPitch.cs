namespace Enaxos.MusicTheory.Primitives;

/// <summary>Represents a pitch spelling as a diatonic letter plus an accidental, without an octave.</summary>
/// <remarks>Equality preserves spelling; use <see cref="IsEnharmonicWith"/> to compare sounding pitch classes.</remarks>
public readonly record struct SpelledPitch
{
    /// <summary>Creates a spelled pitch.</summary>
    /// <param name="letter">The diatonic letter.</param>
    /// <param name="accidental">The chromatic alteration applied to that letter.</param>
    public SpelledPitch(NoteLetter letter, Accidental accidental)
    {
        _ = NoteLetterInfo.NaturalSemitone(letter);
        Letter = letter;
        Accidental = accidental;
    }

    /// <summary>Gets the written diatonic letter.</summary>
    public NoteLetter Letter { get; }

    /// <summary>Gets the written accidental.</summary>
    public Accidental Accidental { get; }

    /// <summary>Gets the octave-independent sounding pitch class produced by the spelling.</summary>
    public PitchClass PitchClass => PitchClass.FromChromaticIndex(
        (long)NoteLetterInfo.NaturalSemitone(Letter) + Accidental.Semitones);

    /// <summary>Determines whether another spelling denotes the same pitch class.</summary>
    public bool IsEnharmonicWith(SpelledPitch other) => PitchClass == other.PitchClass;

    /// <summary>Parses a complete invariant spelling such as <c>C#</c>, <c>Eb</c>, or <c>F𝄪</c>.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="text"/> is invalid.</exception>
    public static SpelledPitch Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!TryParse(text.AsSpan(), out var result))
        {
            throw new FormatException("The value is not a valid spelled pitch.");
        }

        return result;
    }

    /// <summary>Attempts to parse a complete invariant pitch spelling.</summary>
    /// <returns><see langword="true"/> when the entire input is valid.</returns>
    public static bool TryParse(ReadOnlySpan<char> text, out SpelledPitch result)
    {
        result = default;

        if (text.IsEmpty || !TryParseLetter(text[0], out var letter) ||
            !AccidentalParser.TryParseExact(text[1..], out var accidental))
        {
            return false;
        }

        result = new SpelledPitch(letter, accidental);
        return true;
    }

    /// <summary>Returns the invariant ASCII spelling.</summary>
    public override string ToString() => string.Concat(
        NoteLetterInfo.InvariantName(Letter),
        Accidental.ToString());

    /// <summary>Maps an ASCII letter to the domain enum while accepting either case.</summary>
    private static bool TryParseLetter(char value, out NoteLetter letter)
    {
        letter = char.ToUpperInvariant(value) switch
        {
            'C' => NoteLetter.C,
            'D' => NoteLetter.D,
            'E' => NoteLetter.E,
            'F' => NoteLetter.F,
            'G' => NoteLetter.G,
            'A' => NoteLetter.A,
            'B' => NoteLetter.B,
            _ => default,
        };

        return value is 'A' or 'a' or 'B' or 'b' or 'C' or 'c' or 'D' or 'd'
            or 'E' or 'e' or 'F' or 'f' or 'G' or 'g';
    }
}

namespace Enaxos.MusicTheory.Primitives;

public readonly record struct SpelledPitch
{
    public SpelledPitch(NoteLetter letter, Accidental accidental)
    {
        _ = NoteLetterInfo.NaturalSemitone(letter);
        Letter = letter;
        Accidental = accidental;
    }

    public NoteLetter Letter { get; }

    public Accidental Accidental { get; }

    public PitchClass PitchClass => PitchClass.FromChromaticIndex(
        (long)NoteLetterInfo.NaturalSemitone(Letter) + Accidental.Semitones);

    public bool IsEnharmonicWith(SpelledPitch other) => PitchClass == other.PitchClass;

    public static SpelledPitch Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!TryParse(text.AsSpan(), out var result))
        {
            throw new FormatException("The value is not a valid spelled pitch.");
        }

        return result;
    }

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

    public override string ToString() => string.Concat(
        NoteLetterInfo.InvariantName(Letter),
        Accidental.ToString());

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

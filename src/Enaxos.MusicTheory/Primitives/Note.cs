using System.Globalization;

namespace Enaxos.MusicTheory.Primitives;

public readonly record struct Note
{
    public Note(SpelledPitch pitch, int octave)
    {
        var absoluteSemitone =
            (12L * octave) +
            NoteLetterInfo.NaturalSemitone(pitch.Letter) +
            pitch.Accidental.Semitones;

        if (absoluteSemitone is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(octave),
                "The resulting absolute semitone must fit in a 32-bit signed integer.");
        }

        Pitch = pitch;
        Octave = octave;
        AbsoluteSemitone = (int)absoluteSemitone;
    }

    public SpelledPitch Pitch { get; }

    public int Octave { get; }

    public int AbsoluteSemitone { get; }

    public bool IsEnharmonicWith(Note other) => AbsoluteSemitone == other.AbsoluteSemitone;

    public static Note Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!TryParse(text.AsSpan(), out var result))
        {
            throw new FormatException("The value is not a valid note.");
        }

        return result;
    }

    public static bool TryParse(ReadOnlySpan<char> text, out Note result)
    {
        result = default;

        if (text.Length < 2 || !TryParseLetter(text[0], out var letter) ||
            !AccidentalParser.TryReadPrefix(text[1..], out var accidental, out var consumed))
        {
            return false;
        }

        var octaveText = text[(1 + consumed)..];
        if (octaveText.IsEmpty ||
            !int.TryParse(
                octaveText,
                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var octave))
        {
            return false;
        }

        var pitch = new SpelledPitch(letter, accidental);
        var absoluteSemitone =
            (12L * octave) +
            NoteLetterInfo.NaturalSemitone(letter) +
            accidental.Semitones;

        if (absoluteSemitone is < int.MinValue or > int.MaxValue)
        {
            return false;
        }

        result = new Note(pitch, octave);
        return true;
    }

    public override string ToString() => string.Concat(
        Pitch.ToString(),
        Octave.ToString(CultureInfo.InvariantCulture));

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

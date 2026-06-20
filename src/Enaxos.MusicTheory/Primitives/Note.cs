using System.Globalization;

namespace Enaxos.MusicTheory.Primitives;

/// <summary>Represents a spelled pitch placed in a scientific-pitch-notation octave.</summary>
/// <remarks>
/// The octave belongs to the written note. Enharmonic spellings can therefore have different
/// octave numbers at a B/C boundary while still sharing the same absolute semitone.
/// </remarks>
public readonly record struct Note
{
    /// <summary>Creates a note from its written pitch and octave.</summary>
    /// <param name="pitch">The pitch spelling, including its accidental.</param>
    /// <param name="octave">The scientific-pitch-notation octave.</param>
    /// <exception cref="ArgumentOutOfRangeException">The computed absolute semitone cannot be represented.</exception>
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

    /// <summary>Gets the written pitch component.</summary>
    public SpelledPitch Pitch { get; }

    /// <summary>Gets the scientific-pitch-notation octave attached to the written pitch.</summary>
    public int Octave { get; }

    /// <summary>
    /// Gets the signed absolute semitone using the project convention where <c>C0</c> is zero.
    /// </summary>
    public int AbsoluteSemitone { get; }

    /// <summary>Determines whether another note denotes the same absolute sounding pitch.</summary>
    /// <param name="other">The note to compare without considering spelling.</param>
    /// <returns><see langword="true"/> when both notes have the same absolute semitone.</returns>
    public bool IsEnharmonicWith(Note other) => AbsoluteSemitone == other.AbsoluteSemitone;

    /// <summary>Parses an invariant note spelling such as <c>C#4</c>, <c>Db-1</c>, or <c>F♯3</c>.</summary>
    /// <param name="text">The complete note text.</param>
    /// <returns>The parsed note.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException"><paramref name="text"/> is not a valid note.</exception>
    public static Note Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!TryParse(text.AsSpan(), out var result))
        {
            throw new FormatException("The value is not a valid note.");
        }

        return result;
    }

    /// <summary>Attempts to parse a complete invariant note spelling without throwing for invalid syntax.</summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="result">Receives the parsed note on success.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
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

    /// <summary>Returns the invariant ASCII spelling followed by its octave.</summary>
    public override string ToString() => string.Concat(
        Pitch.ToString(),
        Octave.ToString(CultureInfo.InvariantCulture));

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

namespace Enaxos.MusicTheory.Primitives;

/// <summary>Centralizes parsing of the ASCII and Unicode accidental forms accepted by the public parsers.</summary>
internal static class AccidentalParser
{
    /// <summary>Parses an accidental only when the entire input is consumed.</summary>
    internal static bool TryParseExact(ReadOnlySpan<char> text, out Accidental accidental)
    {
        if (!TryReadPrefix(text, out accidental, out var consumed) || consumed != text.Length)
        {
            accidental = default;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Reads an optional accidental prefix and reports its UTF-16 length so a following octave can be parsed.
    /// </summary>
    internal static bool TryReadPrefix(
        ReadOnlySpan<char> text,
        out Accidental accidental,
        out int consumed)
    {
        accidental = Accidental.Natural;
        consumed = 0;

        if (text.IsEmpty || IsOctaveStart(text[0]))
        {
            return true;
        }

        if (text.StartsWith("##", StringComparison.Ordinal) ||
            text.StartsWith("♯♯", StringComparison.Ordinal))
        {
            accidental = Accidental.DoubleSharp;
            consumed = 2;
            return true;
        }

        if (text.StartsWith("bb", StringComparison.Ordinal) ||
            text.StartsWith("♭♭", StringComparison.Ordinal))
        {
            accidental = Accidental.DoubleFlat;
            consumed = 2;
            return true;
        }

        // Musical double-accidental symbols are surrogate pairs, hence the string length
        // rather than a hard-coded single-character count.
        if (text.StartsWith("𝄪", StringComparison.Ordinal))
        {
            accidental = Accidental.DoubleSharp;
            consumed = "𝄪".Length;
            return true;
        }

        if (text.StartsWith("𝄫", StringComparison.Ordinal))
        {
            accidental = Accidental.DoubleFlat;
            consumed = "𝄫".Length;
            return true;
        }

        switch (text[0])
        {
            case '#':
            case '♯':
                accidental = Accidental.Sharp;
                consumed = 1;
                return true;
            case 'b':
            case '♭':
                accidental = Accidental.Flat;
                consumed = 1;
                return true;
            case 'n':
            case '♮':
                accidental = Accidental.Natural;
                consumed = 1;
                return true;
            default:
                return false;
        }
    }

    /// <summary>Determines whether a suffix begins directly with a signed or unsigned octave.</summary>
    private static bool IsOctaveStart(char value) =>
        value is '+' or '-' || char.IsAsciiDigit(value);
}

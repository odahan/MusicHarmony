namespace Enaxos.MusicTheory.Primitives;

internal static class AccidentalParser
{
    internal static bool TryParseExact(ReadOnlySpan<char> text, out Accidental accidental)
    {
        if (!TryReadPrefix(text, out accidental, out var consumed) || consumed != text.Length)
        {
            accidental = default;
            return false;
        }

        return true;
    }

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

    private static bool IsOctaveStart(char value) =>
        value is '+' or '-' || char.IsAsciiDigit(value);
}

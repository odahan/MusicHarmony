using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Primitives;

public sealed class ParsingRulesTests
{
    [Theory]
    [InlineData("C", NoteLetter.C, 0)]
    [InlineData("c", NoteLetter.C, 0)]
    [InlineData("C#", NoteLetter.C, 1)]
    [InlineData("D♭", NoteLetter.D, -1)]
    [InlineData("F##", NoteLetter.F, 2)]
    [InlineData("F♯♯", NoteLetter.F, 2)]
    [InlineData("F𝄪", NoteLetter.F, 2)]
    [InlineData("Abb", NoteLetter.A, -2)]
    [InlineData("A♭♭", NoteLetter.A, -2)]
    [InlineData("A𝄫", NoteLetter.A, -2)]
    [InlineData("B♮", NoteLetter.B, 0)]
    public void Spelled_pitch_parser_accepts_supported_ASCII_and_Unicode(
        string text,
        NoteLetter letter,
        int accidental)
    {
        Assert.True(SpelledPitch.TryParse(text, out var result));
        Assert.Equal(letter, result.Letter);
        Assert.Equal(accidental, result.Accidental.Semitones);
        Assert.Equal(result, SpelledPitch.Parse(text));
    }

    [Theory]
    [InlineData("C#4", NoteLetter.C, 1, 4)]
    [InlineData("Db4", NoteLetter.D, -1, 4)]
    [InlineData("E♭-1", NoteLetter.E, -1, -1)]
    [InlineData("F𝄪3", NoteLetter.F, 2, 3)]
    [InlineData("G𝄫+2", NoteLetter.G, -2, 2)]
    [InlineData("an0", NoteLetter.A, 0, 0)]
    public void Note_parser_accepts_supported_ASCII_and_Unicode(
        string text,
        NoteLetter letter,
        int accidental,
        int octave)
    {
        Assert.True(Note.TryParse(text, out var result));
        Assert.Equal(letter, result.Pitch.Letter);
        Assert.Equal(accidental, result.Pitch.Accidental.Semitones);
        Assert.Equal(octave, result.Octave);
        Assert.Equal(result, Note.Parse(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData("H")]
    [InlineData("C4")]
    [InlineData("C###")]
    [InlineData("Cbbb")]
    [InlineData("C #")]
    [InlineData("Csharp")]
    public void Invalid_spelled_pitch_is_rejected_without_exception(string text)
    {
        var exception = Record.Exception(() =>
            Assert.False(SpelledPitch.TryParse(text, out _)));

        Assert.Null(exception);
        Assert.Throws<FormatException>(() => SpelledPitch.Parse(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData("C")]
    [InlineData("H4")]
    [InlineData("C#")]
    [InlineData("C###4")]
    [InlineData("C4.0")]
    [InlineData("C 4")]
    [InlineData("C4 ")]
    [InlineData("C2147483647")]
    public void Invalid_note_is_rejected_without_exception(string text)
    {
        var exception = Record.Exception(() =>
            Assert.False(Note.TryParse(text, out _)));

        Assert.Null(exception);
        Assert.Throws<FormatException>(() => Note.Parse(text));
    }

    [Theory]
    [InlineData("C#", "C#")]
    [InlineData("Db", "Db")]
    [InlineData("F𝄪", "F##")]
    public void Spelled_pitch_ToString_is_invariant_ASCII(string input, string expected)
    {
        Assert.Equal(expected, SpelledPitch.Parse(input).ToString());
    }

    [Theory]
    [InlineData("C#4", "C#4")]
    [InlineData("Db-1", "Db-1")]
    [InlineData("F𝄪3", "F##3")]
    public void Note_ToString_is_invariant_ASCII(string input, string expected)
    {
        Assert.Equal(expected, Note.Parse(input).ToString());
    }
}

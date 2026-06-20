using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Tests.Tonality;

public sealed class TonalityRulesTests
{
    [Fact]
    public void All_fifteen_conventional_signatures_have_one_accidental_type_and_order()
    {
        NoteLetter[] sharps = [NoteLetter.F, NoteLetter.C, NoteLetter.G, NoteLetter.D, NoteLetter.A, NoteLetter.E, NoteLetter.B];
        NoteLetter[] flats = [NoteLetter.B, NoteLetter.E, NoteLetter.A, NoteLetter.D, NoteLetter.G, NoteLetter.C, NoteLetter.F];
        for (var fifths = -7; fifths <= 7; fifths++)
        {
            var signature = KeySignature.FromFifths(fifths);
            Assert.Equal(fifths, signature.Fifths);
            Assert.Equal(Math.Abs(fifths), signature.AccidentalCount);
            Assert.Equal((fifths < 0 ? flats : sharps).Take(Math.Abs(fifths)), signature.AlteredLetters);
            Assert.Equal(Math.Sign(fifths), signature.Accidental.Semitones);
        }
    }

    [Theory]
    [InlineData("Cb major", -7)]
    [InlineData("C major", 0)]
    [InlineData("C# major", 7)]
    [InlineData("Ab minor", -7)]
    [InlineData("A minor", 0)]
    [InlineData("A# minor", 7)]
    public void Conventional_keys_map_to_signed_fifths(string text, int expected)
    {
        Assert.Equal(expected, KeySignature.For(MusicalKey.Parse(text)).Fifths);
    }

    [Theory]
    [InlineData("Eb major", "C minor")]
    [InlineData("C major", "A minor")]
    [InlineData("F# major", "D# minor")]
    [InlineData("C minor", "Eb major")]
    public void Relative_keys_share_exactly_the_same_signature(string source, string expected)
    {
        var key = MusicalKey.Parse(source); var relative = KeyRelationships.RelativeOf(key);
        Assert.Equal(expected, relative.ToString());
        Assert.Equal(KeySignature.For(key), KeySignature.For(relative));
    }

    [Fact]
    public void Parallel_key_keeps_spelling_and_changes_mode()
    {
        Assert.Equal(MusicalKey.Minor(SpelledPitch.Parse("F#")), KeyRelationships.ParallelOf(MusicalKey.Parse("F# major")));
    }

    [Fact]
    public void Scale_degree_is_always_one_through_seven()
    {
        Assert.Equal(1, default(ScaleDegreeNumber).Value);
        Assert.Throws<ArgumentOutOfRangeException>(() => new ScaleDegreeNumber(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ScaleDegreeNumber(8));
    }
}

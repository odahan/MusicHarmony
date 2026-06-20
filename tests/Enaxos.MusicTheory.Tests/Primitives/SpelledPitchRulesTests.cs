using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Primitives;

public sealed class SpelledPitchRulesTests
{
    public static TheoryData<NoteLetter, int> NaturalPositions => new()
    {
        { NoteLetter.C, 0 },
        { NoteLetter.D, 2 },
        { NoteLetter.E, 4 },
        { NoteLetter.F, 5 },
        { NoteLetter.G, 7 },
        { NoteLetter.A, 9 },
        { NoteLetter.B, 11 },
    };

    [Theory]
    [MemberData(nameof(NaturalPositions))]
    public void Natural_letters_have_the_normative_pitch_classes(
        NoteLetter letter,
        int expected)
    {
        var pitch = new SpelledPitch(letter, Accidental.Natural);

        Assert.Equal(expected, pitch.PitchClass.Value);
    }

    [Fact]
    public void Writing_equality_is_distinct_from_enharmonic_equivalence()
    {
        var cSharp = new SpelledPitch(NoteLetter.C, Accidental.Sharp);
        var dFlat = new SpelledPitch(NoteLetter.D, Accidental.Flat);

        Assert.NotEqual(cSharp, dFlat);
        Assert.True(cSharp.IsEnharmonicWith(dFlat));
    }

    [Fact]
    public void Equal_values_have_equal_hash_codes()
    {
        var first = new SpelledPitch(NoteLetter.F, Accidental.DoubleSharp);
        var second = new SpelledPitch(NoteLetter.F, Accidental.FromSemitones(2));

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Extreme_accidentals_are_normalized_without_integer_overflow()
    {
        var high = new SpelledPitch(
            NoteLetter.B,
            Accidental.FromSemitones(int.MaxValue));
        var low = new SpelledPitch(
            NoteLetter.C,
            Accidental.FromSemitones(int.MinValue));

        Assert.InRange(high.PitchClass.Value, 0, 11);
        Assert.InRange(low.PitchClass.Value, 0, 11);
    }

    [Fact]
    public void Undefined_note_letter_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SpelledPitch((NoteLetter)99, Accidental.Natural));
    }
}

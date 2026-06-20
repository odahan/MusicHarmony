using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Primitives;

public sealed class NoteRulesTests
{
    [Theory]
    [InlineData("C0", 0)]
    [InlineData("C4", 48)]
    [InlineData("A4", 57)]
    [InlineData("B#4", 60)]
    [InlineData("Cb4", 47)]
    [InlineData("C-1", -12)]
    public void Absolute_semitone_follows_the_core_C0_convention(
        string text,
        int expected)
    {
        Assert.Equal(expected, Note.Parse(text).AbsoluteSemitone);
    }

    [Fact]
    public void Enharmonic_note_equivalence_includes_the_octave()
    {
        Assert.True(Note.Parse("C#4").IsEnharmonicWith(Note.Parse("Db4")));
        Assert.True(Note.Parse("B#4").IsEnharmonicWith(Note.Parse("C5")));
        Assert.False(Note.Parse("C#4").IsEnharmonicWith(Note.Parse("Db5")));
    }

    [Fact]
    public void Written_note_equality_preserves_spelling()
    {
        Assert.NotEqual(Note.Parse("C#4"), Note.Parse("Db4"));
    }

    [Fact]
    public void Equal_notes_have_equal_hash_codes()
    {
        var first = Note.Parse("F##3");
        var second = new Note(
            new SpelledPitch(NoteLetter.F, Accidental.DoubleSharp),
            3);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Note_contains_no_rhythm_or_MIDI_state()
    {
        var publicProperties = typeof(Note)
            .GetProperties()
            .Select(property => property.Name)
            .OrderBy(name => name)
            .ToArray();

        Assert.Equal(
            [nameof(Note.AbsoluteSemitone), nameof(Note.Octave), nameof(Note.Pitch)],
            publicProperties);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Octaves_that_overflow_absolute_semitone_are_rejected(int octave)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Note(new SpelledPitch(NoteLetter.C, Accidental.Natural), octave));
    }
}

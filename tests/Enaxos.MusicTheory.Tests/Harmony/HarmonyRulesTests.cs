using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Tests.Harmony;

public sealed class HarmonyRulesTests
{
    [Theory]
    [InlineData("Db", "Db F Ab")]
    [InlineData("C#", "C# E# G#")]
    public void Major_chord_preserves_formula_spelling(string root, string expected)
    {
        var chord = Chord.Create(SpelledPitch.Parse(root), StandardChords.Major);
        Assert.Equal(expected.Split(' '), chord.Pitches.Select(pitch => pitch.ToString()));
        Assert.Equal(root, chord.Symbol.Root.ToString());
    }

    [Fact]
    public void Standard_definitions_start_with_unique_root_degree()
    {
        foreach (var property in typeof(StandardChords).GetProperties()
            .Where(property => property.PropertyType == typeof(ChordDefinition)))
        {
            var definition = Assert.IsType<ChordDefinition>(property.GetValue(null));
            Assert.Equal(1, definition.Degrees[0].Number);
            Assert.Equal(definition.Degrees.Count, definition.Degrees.Select(degree => degree.Number).Distinct().Count());
        }
    }

    [Fact]
    public void Standard_chord_catalog_is_read_only_and_contains_public_definitions()
    {
        Assert.Contains(StandardChords.Major, StandardChords.All);
        Assert.Contains(StandardChords.DiminishedSeventh, StandardChords.All);
        var mutableView = Assert.IsAssignableFrom<ICollection<ChordDefinition>>(StandardChords.All);

        Assert.Throws<NotSupportedException>(() => mutableView.Add(StandardChords.Minor));
    }

    [Theory]
    [InlineData("C", "C")]
    [InlineData("Cm", "Cm")]
    [InlineData("Csus2", "Csus2")]
    [InlineData("Csus", "Csus4")]
    [InlineData("Csus4", "Csus4")]
    [InlineData("C6", "C6")]
    [InlineData("Cm6", "Cm6")]
    [InlineData("F#7", "F#7")]
    [InlineData("Bbmaj7", "Bbmaj7")]
    [InlineData("C9", "C9")]
    [InlineData("Cmaj9", "Cmaj9")]
    [InlineData("Cm9", "Cm9")]
    [InlineData("C11", "C11")]
    [InlineData("Cmaj11", "Cmaj11")]
    [InlineData("Cm11", "Cm11")]
    [InlineData("C13", "C13")]
    [InlineData("Cmaj13", "Cmaj13")]
    [InlineData("Cm13", "Cm13")]
    [InlineData("Dø7", "Dø7")]
    [InlineData("G°7", "G°7")]
    public void Chord_symbols_parse_and_round_trip(string input, string expected)
    {
        Assert.True(ChordSymbol.TryParse(input, out var symbol));
        Assert.Equal(expected, symbol.ToString());
    }

    [Fact]
    public void Transformation_provenance_is_exact_and_cumulative()
    {
        var original = Chord.Create(SpelledPitch.Parse("C"), StandardChords.Major);
        var result = DerivedChord.From(original).Transpose(12).Transpose(2);
        Assert.Same(original, result.OriginalChord);
        Assert.Equal("D", result.CurrentChord.Root.ToString());
        Assert.Equal(14, result.SemitoneDeltaFromOriginal);
    }

    [Fact]
    public void Inversion_replaces_number_and_preserves_transposition()
    {
        var original = Chord.Create(SpelledPitch.Parse("C"), StandardChords.Major);
        var result = ChordRealization.CreateRootPosition(original, 4).Transpose(14).Invert(1).Invert(2);
        Assert.Same(original, result.OriginalChord);
        Assert.Equal(14, result.SemitoneDeltaFromOriginal);
        Assert.Equal(2, result.InversionNumber);
        Assert.Equal("A5", result.Bass.ToString());
        Assert.Equal(["A5", "D6", "F#6"], result.Notes.Select(note => note.ToString()));
    }

    [Fact]
    public void Voicing_is_defensively_copied_and_rejects_outside_tones()
    {
        var chord = Chord.Create(SpelledPitch.Parse("C"), StandardChords.Major);
        var notes = new List<Note> { Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G4") };
        var realization = ChordRealization.Create(chord, notes);
        notes[0] = Note.Parse("D4");
        Assert.Equal("C4", realization.Bass.ToString());
        Assert.Throws<ArgumentException>(() => ChordRealization.Create(chord, [Note.Parse("C4"), Note.Parse("D4")]));
    }

    [Fact]
    public void Harmonic_function_requires_an_explicit_compatible_key()
    {
        var chord = Chord.Create(SpelledPitch.Parse("B"), StandardChords.Diminished);
        var function = HarmonicFunctions.Analyze(chord, MusicalKey.Parse("C major"));
        Assert.Equal(7, function.Degree.Value);
        Assert.Equal(HarmonicChordQuality.Diminished, function.Quality);
        Assert.False(HarmonicFunctions.TryAnalyze(chord, MusicalKey.Parse("Db major"), out _));
    }
}

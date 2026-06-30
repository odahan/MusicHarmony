using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tests.Scales;

public sealed class ScaleRelationsAndStructureRulesTests
{
    [Fact]
    public void Common_notes_preserve_first_scale_order_and_spelling()
    {
        var cMajor = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);
        var gMajor = Scale.Create(SpelledPitch.Parse("G"), StandardScales.Major);
        var dMajor = Scale.Create(SpelledPitch.Parse("D"), StandardScales.Major);

        var common = ScaleRelations.GetCommonNotes(cMajor, gMajor, dMajor);

        Assert.Equal(["D", "E", "G", "A", "B"],
            common.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Common_notes_can_compare_pitch_classes_or_exact_spellings()
    {
        var sharp = new[] { SpelledPitch.Parse("C#") };
        var flat = new[] { SpelledPitch.Parse("Db") };

        var enharmonic = ScaleRelations.GetCommonNotes(
            PitchMatchMode.PitchClass,
            sharp,
            flat);
        var exact = ScaleRelations.GetCommonNotes(
            PitchMatchMode.ExactSpelling,
            sharp,
            flat);

        Assert.Equal("C#", Assert.Single(enharmonic).ToString());
        Assert.Empty(exact);
    }

    [Fact]
    public void Common_notes_results_are_read_only_snapshots()
    {
        var first = new[] { SpelledPitch.Parse("C"), SpelledPitch.Parse("E") };
        var second = new[] { SpelledPitch.Parse("C"), SpelledPitch.Parse("G") };

        var common = ScaleRelations.GetCommonNotes(first, second);
        var mutableView = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(common);

        Assert.Throws<NotSupportedException>(() => mutableView.Add(SpelledPitch.Parse("D")));
    }

    [Theory]
    [InlineData("scale.major", "WWHWWWH", new[] { 2, 2, 1, 2, 2, 2, 1 })]
    [InlineData("mode.major.2", "WHWWWHW", new[] { 2, 1, 2, 2, 2, 1, 2 })]
    [InlineData("scale.minor.harmonic", "WHWWH3H", new[] { 2, 1, 2, 2, 1, 3, 1 })]
    [InlineData("scale.pentatonic.major", "WW3W3", new[] { 2, 2, 3, 2, 3 })]
    public void Standard_scale_structures_use_stable_ids(
        string id,
        string expectedPattern,
        int[] expectedSteps)
    {
        var structure = ScaleStructures.GetScaleStruct(id);

        Assert.Equal(expectedSteps, structure.SemitoneSteps);
        Assert.Equal(expectedPattern, structure.CompactPattern);
    }

    [Fact]
    public void Realized_scale_and_pitch_list_structures_match_formula_order()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);
        var fromScale = ScaleStructures.GetScaleStruct(scale);
        var fromPitches = ScaleStructures.GetScaleStruct(scale.Pitches);
        var fromNotes = ScaleStructures.GetScaleStruct(
            scale.Pitches.Select(pitch => new Note(pitch, 4)));

        Assert.Equal("WWHWWWH", fromScale.CompactPattern);
        Assert.Equal(fromScale.SemitoneSteps, fromPitches.SemitoneSteps);
        Assert.Equal(fromScale.SemitoneSteps, fromNotes.SemitoneSteps);
    }

    [Fact]
    public void Scale_structure_rejects_unknown_ids_and_unordered_pitch_cycles()
    {
        Assert.Throws<ArgumentException>(() => ScaleStructures.GetScaleStruct("scale.unknown"));
        Assert.Throws<ArgumentException>(() => ScaleStructures.GetScaleStruct(
            [SpelledPitch.Parse("C"), SpelledPitch.Parse("B"), SpelledPitch.Parse("D")]));
    }
}

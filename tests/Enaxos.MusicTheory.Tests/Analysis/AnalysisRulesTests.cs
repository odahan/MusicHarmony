using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Analysis;

public sealed class AnalysisRulesTests
{
    [Fact]
    public void Exact_chord_match_is_first_and_reports_inversion()
    {
        var result = ChordRecognizer.Recognize([Note.Parse("E4"), Note.Parse("G4"), Note.Parse("C5")]);
        Assert.NotEmpty(result);
        Assert.Equal(StandardChords.Major, result[0].Chord.Definition);
        Assert.Equal("C", result[0].Chord.Root.ToString());
        Assert.Equal(1, result[0].InversionNumber);
        Assert.Empty(result[0].MissingPitches); Assert.Empty(result[0].AddedPitches);
    }

    [Fact]
    public void Omission_and_addition_require_explicit_options_and_rank_below_exact()
    {
        Assert.Empty(ChordRecognizer.Recognize([Note.Parse("C4"), Note.Parse("E4")]));
        var omitted = ChordRecognizer.Recognize([Note.Parse("C4"), Note.Parse("E4")], new ChordRecognitionOptions { AllowMissingTones = true });
        Assert.Contains(omitted, item => item.Chord.Definition.Equals(StandardChords.Major) && item.MissingPitches.Count == 1);
        var added = ChordRecognizer.Recognize([Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G4"), Note.Parse("D5")], new ChordRecognitionOptions { AllowAddedTones = true });
        Assert.Contains(added, item => item.Chord.Definition.Equals(StandardChords.Major) && item.AddedPitches.Count == 1);
    }

    [Fact]
    public void Scale_probabilities_are_normalized_and_order_is_deterministic()
    {
        var notes = new[] { Note.Parse("C4"), Note.Parse("E4"), Note.Parse("G4") };
        var first = ScaleRecognizer.FindCandidates(notes);
        var second = ScaleRecognizer.FindCandidates(notes);
        Assert.NotEmpty(first);
        Assert.Equal(1d, first.Sum(item => item.RelativeProbability), 12);
        Assert.Equal(first.Select(item => (item.Scale.Tonic, item.Scale.Definition.Id)), second.Select(item => (item.Scale.Tonic, item.Scale.Definition.Id)));
        Assert.All(first, item => Assert.Empty(item.OutsidePitches));
        Assert.All(first, item => Assert.NotEmpty(item.ScoreFactors));
    }

    [Fact]
    public void Non_strict_search_reports_outside_notes()
    {
        var options = new ScaleRecognitionOptions { StrictMembership = false, MaximumResults = 8 };
        var result = ScaleRecognizer.FindCandidates(
            [Note.Parse("C4"), Note.Parse("C#4"), Note.Parse("D4"), Note.Parse("D#4"),
             Note.Parse("E4"), Note.Parse("F4"), Note.Parse("F#4"), Note.Parse("G4")],
            options);
        Assert.Contains(result, item => item.OutsidePitches.Count > 0);
    }

    [Fact]
    public void Chord_root_evidence_is_explicit_in_scale_score()
    {
        var chord = Chord.Create(SpelledPitch.Parse("C"), StandardChords.Major);
        var result = ScaleRecognizer.FindCandidates(chord);
        Assert.Contains(result, item => item.ScoreFactors["chordRoot"] > 0);
    }
}

using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Analysis;

public sealed class ChordRecognitionRulesTests
{
    [Theory]
    [MemberData(nameof(StandardChordRealizations))]
    public void Every_standard_chord_definition_is_recognized_from_exact_realization(
        ChordDefinition definition,
        string expectedSymbol,
        string[] notes)
    {
        var result = ChordRecognizer.Recognize(
            Notes(notes),
            new ChordRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(definition, candidate.Chord.Definition);
        Assert.Equal(expectedSymbol, candidate.Chord.Symbol.ToString());
        Assert.Equal(0, candidate.InversionNumber);
        Assert.Empty(candidate.MissingPitches);
        Assert.Empty(candidate.AddedPitches);
        Assert.Equal(1d, candidate.Confidence);
    }

    [Fact]
    public void Bass_detection_uses_lowest_absolute_note_after_collapsing_duplicate_spellings()
    {
        var result = ChordRecognizer.Recognize(
            Notes("G5", "C5", "E4", "C6", "E5"),
            new ChordRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(StandardChords.Major, candidate.Chord.Definition);
        Assert.Equal("C", candidate.Chord.Root.ToString());
        Assert.Equal(1, candidate.InversionNumber);
        Assert.Empty(candidate.AddedPitches);
    }

    [Fact]
    public void Enharmonic_equivalence_can_be_disabled_for_chord_matching()
    {
        var notes = Notes("C4", "E4", "Ab4");

        var defaultTop = ChordRecognizer.Recognize(notes)[0];
        Assert.Equal(StandardChords.Augmented, defaultTop.Chord.Definition);
        Assert.Equal("C", defaultTop.Chord.Root.ToString());

        var strict = ChordRecognizer.Recognize(
            notes,
            new ChordRecognitionOptions { AllowEnharmonicEquivalence = false });

        Assert.NotEmpty(strict);
        Assert.Equal(StandardChords.Augmented, strict[0].Chord.Definition);
        Assert.Equal("Ab", strict[0].Chord.Root.ToString());
        Assert.DoesNotContain(
            strict,
            candidate => candidate.Chord.Definition.Equals(StandardChords.Augmented) &&
                candidate.Chord.Root.ToString() == "C");
    }

    [Fact]
    public void Chord_overload_uses_the_canonical_root_position_realization()
    {
        var chord = Chord.Create(SpelledPitch.Parse("F#"), StandardChords.MajorSeventh);

        var result = ChordRecognizer.Recognize(
            chord,
            new ChordRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(chord, candidate.Chord);
        Assert.Equal(0, candidate.InversionNumber);
        Assert.Empty(candidate.MissingPitches);
        Assert.Empty(candidate.AddedPitches);
    }

    [Fact]
    public void Try_recognize_best_returns_the_first_ranked_candidate_from_recognize()
    {
        var notes = Notes("G5", "C5", "E4", "C6", "E5");

        var recognized = ChordRecognizer.Recognize(notes);
        var success = ChordRecognizer.TryRecognizeBest(notes, out var candidate);

        Assert.True(success);
        Assert.NotNull(candidate);
        Assert.Equal(recognized[0].Chord, candidate.Chord);
        Assert.Equal(recognized[0].InversionNumber, candidate.InversionNumber);
        Assert.Equal(recognized[0].Score, candidate.Score);
    }

    [Fact]
    public void Try_recognize_best_returns_false_when_no_candidate_matches()
    {
        var success = ChordRecognizer.TryRecognizeBest(Notes("C4"), out var candidate);

        Assert.False(success);
        Assert.Null(candidate);
    }

    [Fact]
    public void Recognition_validates_input_and_result_limit()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ChordRecognizer.Recognize((IEnumerable<Note>)null!));
        Assert.Throws<ArgumentException>(() =>
            ChordRecognizer.Recognize(Array.Empty<Note>()));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ChordRecognizer.Recognize(
                Notes("C4"),
                new ChordRecognitionOptions { MaximumResults = 0 }));
    }

    [Fact]
    public void Candidate_missing_and_added_evidence_are_read_only_snapshots()
    {
        var result = ChordRecognizer.Recognize(
            Notes("C4", "E4", "D5"),
            new ChordRecognitionOptions
            {
                AllowAddedTones = true,
                AllowMissingTones = true,
            });
        var candidate = result.First(item =>
            item.Chord.Definition.Equals(StandardChords.Major) &&
            item.Chord.Root.ToString() == "C");

        Assert.Equal(["G"], candidate.MissingPitches.Select(pitch => pitch.ToString()));
        Assert.Equal(["D"], candidate.AddedPitches.Select(pitch => pitch.ToString()));

        var missing = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.MissingPitches);
        var added = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.AddedPitches);
        Assert.Throws<NotSupportedException>(() => missing.Add(SpelledPitch.Parse("G")));
        Assert.Throws<NotSupportedException>(() => added.Add(SpelledPitch.Parse("D")));
    }

    public static TheoryData<ChordDefinition, string, string[]> StandardChordRealizations => new()
    {
        { StandardChords.Major, "C", ["C4", "E4", "G4"] },
        { StandardChords.Minor, "Cm", ["C4", "Eb4", "G4"] },
        { StandardChords.Diminished, "C°", ["C4", "Eb4", "Gb4"] },
        { StandardChords.Augmented, "C+", ["C4", "E4", "G#4"] },
        { StandardChords.DominantSeventh, "C7", ["C4", "E4", "G4", "Bb4"] },
        { StandardChords.MajorSeventh, "Cmaj7", ["C4", "E4", "G4", "B4"] },
        { StandardChords.MinorSeventh, "Cm7", ["C4", "Eb4", "G4", "Bb4"] },
        { StandardChords.HalfDiminishedSeventh, "Cø7", ["C4", "Eb4", "Gb4", "Bb4"] },
        { StandardChords.DiminishedSeventh, "C°7", ["C4", "Eb4", "Gb4", "Bbb4"] },
    };

    private static Note[] Notes(params string[] names) =>
        names.Select(Note.Parse).ToArray();
}

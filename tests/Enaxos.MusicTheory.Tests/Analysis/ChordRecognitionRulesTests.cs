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
    public void Recognized_pitches_keep_observed_spellings_for_enharmonic_matches()
    {
        var result = ChordRecognizer.Recognize(
            Notes("A#4", "D5", "F5"),
            new ChordRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(StandardChords.Major, candidate.Chord.Definition);
        Assert.Equal("A#", candidate.Chord.Root.ToString());
        Assert.Equal(["A#", "D", "F"], candidate.RecognizedPitches.Select(pitch => pitch.ToString()));
        Assert.Equal(["A#", "C##", "E#"], candidate.Chord.Pitches.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Inversions_are_ranked_from_the_lowest_absolute_note()
    {
        var result = ChordRecognizer.Recognize(
            Notes("E4", "G4", "C5"),
            new ChordRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(StandardChords.Major, candidate.Chord.Definition);
        Assert.Equal("C", candidate.Chord.Root.ToString());
        Assert.Equal(1, candidate.InversionNumber);
    }

    [Fact]
    public void Sixth_eleventh_and_thirteenth_chords_are_part_of_standard_recognition()
    {
        Assert.Equal(StandardChords.MajorSixth, ChordRecognizer.Recognize(Notes("C4", "E4", "G4", "A4"), new ChordRecognitionOptions { MaximumResults = 1 })[0].Chord.Definition);
        Assert.Equal(StandardChords.MajorNinth, ChordRecognizer.Recognize(Notes("C4", "E4", "G4", "B4", "D5"), new ChordRecognitionOptions { MaximumResults = 1 })[0].Chord.Definition);
        Assert.Equal(StandardChords.DominantEleventh, ChordRecognizer.Recognize(Notes("C4", "E4", "G4", "Bb4", "D5", "F5"), new ChordRecognitionOptions { MaximumResults = 1 })[0].Chord.Definition);
        Assert.Equal(StandardChords.MajorEleventh, ChordRecognizer.Recognize(Notes("C4", "E4", "G4", "B4", "D5", "F5"), new ChordRecognitionOptions { MaximumResults = 1 })[0].Chord.Definition);
        Assert.Equal(StandardChords.DominantThirteenth, ChordRecognizer.Recognize(Notes("C4", "E4", "G4", "Bb4", "D5", "F5", "A5"), new ChordRecognitionOptions { MaximumResults = 1 })[0].Chord.Definition);
    }

    [Fact]
    public void Dominant_extensions_allow_common_omitted_inner_tones()
    {
        var eleventh = ChordRecognizer.Recognize(
            Notes("C4", "E4", "Bb4", "F5"),
            new ChordRecognitionOptions { MaximumResults = 1 })[0];
        var thirteenth = ChordRecognizer.Recognize(
            Notes("C4", "E4", "Bb4", "A5"),
            new ChordRecognitionOptions { MaximumResults = 1 })[0];

        Assert.Equal(StandardChords.DominantEleventh, eleventh.Chord.Definition);
        Assert.Equal(["G", "D"], eleventh.MissingPitches.Select(pitch => pitch.ToString()));
        Assert.Equal(["C", "E", "Bb", "F"], eleventh.RecognizedPitches.Select(pitch => pitch.ToString()));
        Assert.Equal(StandardChords.DominantThirteenth, thirteenth.Chord.Definition);
        Assert.Equal(["G", "D", "F"], thirteenth.MissingPitches.Select(pitch => pitch.ToString()));
        Assert.Equal(["C", "E", "Bb", "A"], thirteenth.RecognizedPitches.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Recognized_bass_is_available_for_sparse_extended_inversions()
    {
        var result = ChordRecognizer.Recognize(
            Notes("A3", "C4", "E4", "Bb4"),
            new ChordRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(StandardChords.DominantThirteenth, candidate.Chord.Definition);
        Assert.Equal(6, candidate.InversionNumber);
        Assert.Equal("A", candidate.BassPitch?.ToString());
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

        var recognized = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.RecognizedPitches);
        var missing = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.MissingPitches);
        var added = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.AddedPitches);
        Assert.Throws<NotSupportedException>(() => recognized.Add(SpelledPitch.Parse("C")));
        Assert.Throws<NotSupportedException>(() => missing.Add(SpelledPitch.Parse("G")));
        Assert.Throws<NotSupportedException>(() => added.Add(SpelledPitch.Parse("D")));
    }

    public static TheoryData<ChordDefinition, string, string[]> StandardChordRealizations => new()
    {
        { StandardChords.Major, "C", ["C4", "E4", "G4"] },
        { StandardChords.Minor, "Cm", ["C4", "Eb4", "G4"] },
        { StandardChords.Diminished, "C°", ["C4", "Eb4", "Gb4"] },
        { StandardChords.Augmented, "C+", ["C4", "E4", "G#4"] },
        { StandardChords.SuspendedSecond, "Csus2", ["C4", "D4", "G4"] },
        { StandardChords.SuspendedFourth, "Csus4", ["C4", "F4", "G4"] },
        { StandardChords.MajorSixth, "C6", ["C4", "E4", "G4", "A4"] },
        { StandardChords.MinorSixth, "Cm6", ["C4", "Eb4", "G4", "A4"] },
        { StandardChords.DominantSeventh, "C7", ["C4", "E4", "G4", "Bb4"] },
        { StandardChords.MajorSeventh, "Cmaj7", ["C4", "E4", "G4", "B4"] },
        { StandardChords.MinorSeventh, "Cm7", ["C4", "Eb4", "G4", "Bb4"] },
        { StandardChords.HalfDiminishedSeventh, "Cø7", ["C4", "Eb4", "Gb4", "Bb4"] },
        { StandardChords.DiminishedSeventh, "C°7", ["C4", "Eb4", "Gb4", "Bbb4"] },
        { StandardChords.DominantNinth, "C9", ["C4", "E4", "G4", "Bb4", "D5"] },
        { StandardChords.MajorNinth, "Cmaj9", ["C4", "E4", "G4", "B4", "D5"] },
        { StandardChords.MinorNinth, "Cm9", ["C4", "Eb4", "G4", "Bb4", "D5"] },
        { StandardChords.DominantEleventh, "C11", ["C4", "E4", "G4", "Bb4", "D5", "F5"] },
        { StandardChords.MajorEleventh, "Cmaj11", ["C4", "E4", "G4", "B4", "D5", "F5"] },
        { StandardChords.MinorEleventh, "Cm11", ["C4", "Eb4", "G4", "Bb4", "D5", "F5"] },
        { StandardChords.DominantThirteenth, "C13", ["C4", "E4", "G4", "Bb4", "D5", "F5", "A5"] },
        { StandardChords.MajorThirteenth, "Cmaj13", ["C4", "E4", "G4", "B4", "D5", "F5", "A5"] },
        { StandardChords.MinorThirteenth, "Cm13", ["C4", "Eb4", "G4", "Bb4", "D5", "F5", "A5"] },
    };

    private static Note[] Notes(params string[] names) =>
        names.Select(Note.Parse).ToArray();
}

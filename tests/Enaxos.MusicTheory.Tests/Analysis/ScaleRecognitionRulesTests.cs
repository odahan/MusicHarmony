using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tests.Analysis;

public sealed class ScaleRecognitionRulesTests
{
    [Fact]
    public void Complete_major_collection_is_recognized_with_complete_evidence()
    {
        var result = ScaleRecognizer.FindCandidates(
            Notes("C4", "D4", "E4", "F4", "G4", "A4", "B4"),
            new ScaleRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal("C", candidate.Scale.Tonic.ToString());
        Assert.Equal(StandardScales.Ionian, candidate.Scale.Definition);
        Assert.Equal(["C", "D", "E", "F", "G", "A", "B"],
            candidate.Scale.Pitches.Select(pitch => pitch.ToString()));
        Assert.Equal(7, candidate.MatchedPitches.Count);
        Assert.Empty(candidate.MissingPitches);
        Assert.Empty(candidate.OutsidePitches);
        Assert.Equal(1d, candidate.RelativeProbability);
        Assert.Equal(candidate.Score, candidate.ScoreFactors.Values.Sum(), 12);
    }

    [Fact]
    public void Custom_catalog_limits_search_and_finds_the_expected_minor_collection()
    {
        var result = ScaleRecognizer.FindCandidates(
            Notes("A4", "B4", "C5", "D5", "E5", "F5", "G5"),
            new ScaleRecognitionOptions
            {
                Catalog = [StandardScales.NaturalMinor],
                MaximumResults = 4,
            });

        var candidate = Assert.Single(result);
        Assert.Equal("A", candidate.Scale.Tonic.ToString());
        Assert.Equal(StandardScales.NaturalMinor, candidate.Scale.Definition);
        Assert.Empty(candidate.MissingPitches);
        Assert.Empty(candidate.OutsidePitches);
    }

    [Fact]
    public void Strict_membership_rejects_observed_pitch_sets_no_scale_can_contain()
    {
        var chromatic = Notes(
            "C4", "C#4", "D4", "D#4", "E4", "F4",
            "F#4", "G4", "G#4", "A4", "A#4", "B4");

        Assert.Empty(ScaleRecognizer.FindCandidates(chromatic));

        var relaxed = ScaleRecognizer.FindCandidates(
            chromatic,
            new ScaleRecognitionOptions
            {
                StrictMembership = false,
                MaximumResults = 1,
            });

        var candidate = Assert.Single(relaxed);
        Assert.NotEmpty(candidate.OutsidePitches);
    }

    [Fact]
    public void Recognition_collapses_repeated_octaves_to_distinct_pitch_spellings()
    {
        var result = ScaleRecognizer.FindCandidates(
            Notes("C4", "E4", "G4", "C5", "E5"),
            new ScaleRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal(["C", "E", "G"],
            candidate.MatchedPitches.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Recognition_can_include_pentatonic_candidates_when_requested()
    {
        var notes = Notes("C4", "D4", "E4", "G4", "A4");

        var standard = ScaleRecognizer.FindCandidates(
            notes,
            new ScaleRecognitionOptions { MaximumResults = 32 });
        var withPentatonics = ScaleRecognizer.FindCandidates(
            notes,
            new ScaleRecognitionOptions
            {
                IncludePentatonicCandidates = true,
                MaximumResults = 32,
            });

        Assert.DoesNotContain(standard, candidate => candidate.Scale.Definition == StandardScales.MajorPentatonic);
        var pentatonic = Assert.Single(withPentatonics, candidate =>
            candidate.Scale.Tonic.ToString() == "C" &&
            candidate.Scale.Definition == StandardScales.MajorPentatonic);
        Assert.Empty(pentatonic.MissingPitches);
        Assert.Empty(pentatonic.OutsidePitches);
    }

    [Fact]
    public void Recognition_can_include_exotic_candidates_when_requested()
    {
        var notes = Notes("C4", "D4", "Eb4", "E4", "G4", "A4");
        var majorBlues = ExoticScales.BluesAndBebop.Single(definition => definition.Id == "scale.exotic.blues.major");

        var standard = ScaleRecognizer.FindCandidates(
            notes,
            new ScaleRecognitionOptions { MaximumResults = 32 });
        var withExotics = ScaleRecognizer.FindCandidates(
            notes,
            new ScaleRecognitionOptions
            {
                IncludeExoticCandidates = true,
                MaximumResults = 32,
            });

        Assert.DoesNotContain(standard, candidate => candidate.Scale.Definition == majorBlues);
        var candidate = Assert.Single(withExotics, candidate =>
            candidate.Scale.Tonic.ToString() == "C" &&
            candidate.Scale.Definition == majorBlues);
        Assert.Empty(candidate.MissingPitches);
        Assert.Empty(candidate.OutsidePitches);
    }

    [Fact]
    public void Chord_overload_uses_chord_root_as_tonic_evidence()
    {
        var chord = Chord.Create(SpelledPitch.Parse("C"), StandardChords.Major);

        var result = ScaleRecognizer.FindCandidates(
            chord,
            new ScaleRecognitionOptions
            {
                Catalog = [StandardScales.Major],
                MaximumResults = 1,
            });

        var candidate = Assert.Single(result);
        Assert.Equal("C", candidate.Scale.Tonic.ToString());
        Assert.Equal(StandardScales.Major, candidate.Scale.Definition);
        Assert.True(candidate.ScoreFactors["chordRoot"] > 0);
    }

    [Fact]
    public void Note_overload_uses_lowest_note_as_tonic_evidence()
    {
        var result = ScaleRecognizer.FindCandidates(
            Notes("D4", "E4", "F4", "G4", "A4", "B4", "C5"),
            new ScaleRecognitionOptions { MaximumResults = 1 });

        var candidate = Assert.Single(result);
        Assert.Equal("D", candidate.Scale.Tonic.ToString());
        Assert.Equal(StandardScales.Dorian, candidate.Scale.Definition);
        Assert.True(candidate.ScoreFactors["bassTonic"] > 0);
    }

    [Fact]
    public void Recognition_validates_notes_chords_catalog_and_required_options()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ScaleRecognizer.FindCandidates((IEnumerable<Note>)null!));
        Assert.Throws<ArgumentException>(() =>
            ScaleRecognizer.FindCandidates(Array.Empty<Note>()));
        Assert.Throws<ArgumentNullException>(() =>
            ScaleRecognizer.FindCandidates((Chord)null!));
        Assert.Throws<ArgumentException>(() =>
            ScaleRecognizer.FindCandidates(
                Notes("C4"),
                new ScaleRecognitionOptions { Catalog = [] }));
        Assert.Throws<ArgumentNullException>(() =>
            ScaleRecognizer.FindCandidates(
                Notes("C4"),
                new ScaleRecognitionOptions { Weights = null! }));
    }

    [Theory]
    [MemberData(nameof(InvalidNumericOptions))]
    public void Recognition_validates_numeric_options(ScaleRecognitionOptions options)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ScaleRecognizer.FindCandidates(Notes("C4"), options));
    }

    [Fact]
    public void Candidate_evidence_and_score_factors_are_read_only_snapshots()
    {
        var result = ScaleRecognizer.FindCandidates(
            Notes("C4", "C#4", "E4"),
            new ScaleRecognitionOptions
            {
                StrictMembership = false,
                MaximumResults = 1,
            });
        var candidate = Assert.Single(result);

        var matched = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.MatchedPitches);
        var missing = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.MissingPitches);
        var outside = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(candidate.OutsidePitches);
        var factors = Assert.IsAssignableFrom<IDictionary<string, double>>(candidate.ScoreFactors);

        Assert.Throws<NotSupportedException>(() => matched.Add(SpelledPitch.Parse("C")));
        Assert.Throws<NotSupportedException>(() => missing.Add(SpelledPitch.Parse("D")));
        Assert.Throws<NotSupportedException>(() => outside.Add(SpelledPitch.Parse("C#")));
        Assert.Throws<NotSupportedException>(() => factors.Add("custom", 1d));
    }

    public static TheoryData<ScaleRecognitionOptions> InvalidNumericOptions => new()
    {
        new ScaleRecognitionOptions { MaximumResults = 0 },
        new ScaleRecognitionOptions { ProbabilityTemperature = 0d },
        new ScaleRecognitionOptions { ProbabilityTemperature = double.NaN },
        new ScaleRecognitionOptions
        {
            Weights = new ScaleRecognitionWeights { Membership = -1d },
        },
        new ScaleRecognitionOptions
        {
            Weights = new ScaleRecognitionWeights { Coverage = double.PositiveInfinity },
        },
        new ScaleRecognitionOptions
        {
            Weights = new ScaleRecognitionWeights { BassTonicEvidence = -1d },
        },
    };

    private static Note[] Notes(params string[] names) =>
        names.Select(Note.Parse).ToArray();
}

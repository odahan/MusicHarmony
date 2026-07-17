using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tests.Scales;

public sealed class ScaleConstructionRulesTests
{
    [Theory]
    [InlineData("F#", "F# G# A# B C# D# E#")]
    [InlineData("Db", "Db Eb F Gb Ab Bb C")]
    [InlineData("C#", "C# D# E# F# G# A# B#")]
    [InlineData("Cb", "Cb Db Eb Fb Gb Ab Bb")]
    public void Major_scale_preserves_the_expected_written_letters(
        string tonic,
        string expected)
    {
        var scale = Scale.Create(SpelledPitch.Parse(tonic), StandardScales.Major);

        Assert.Equal(expected.Split(' '), scale.Pitches.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Every_heptatonic_standard_scale_uses_each_letter_once()
    {
        var definitions = new[]
        {
            StandardScales.Major,
            StandardScales.NaturalMinor,
            StandardScales.HarmonicMinor,
            StandardScales.MelodicMinorAscending,
        };

        foreach (var definition in definitions)
        {
            foreach (var letter in Enum.GetValues<NoteLetter>())
            {
                for (var accidental = -2; accidental <= 2; accidental++)
                {
                    var tonic = new SpelledPitch(
                        letter,
                        Accidental.FromSemitones(accidental));
                    var scale = Scale.Create(tonic, definition);

                    Assert.Equal(7, scale.Pitches.Count);
                    Assert.Equal(7, scale.Pitches.Select(pitch => pitch.Letter).Distinct().Count());
                    Assert.Equal(tonic, scale.Pitches[0]);
                }
            }
        }
    }

    [Fact]
    public void Scale_identity_does_not_include_a_repeated_final_tonic()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);

        Assert.Equal(7, scale.Pitches.Count);
        Assert.Equal("B", scale.Pitches[^1].ToString());
    }

    [Fact]
    public void Degree_uses_formula_numbers_including_sparse_formulas()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.MajorPentatonic);

        Assert.Equal("C", scale.Degree(1).ToString());
        Assert.Equal("E", scale.Degree(3).ToString());
        Assert.Equal("A", scale.Degree(6).ToString());
        Assert.Throws<ArgumentOutOfRangeException>(() => scale.Degree(4));
        Assert.Throws<ArgumentOutOfRangeException>(() => scale.Degree(0));
    }

    [Fact]
    public void Scale_definitions_can_include_chromatic_variants_of_the_same_degree()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), ExoticScales.BluesAndBebop.First(definition => definition.Id == "scale.exotic.blues.major"));

        Assert.Equal(["C", "D", "Eb", "E", "G", "A"], scale.Pitches.Select(pitch => pitch.ToString()));
        Assert.Equal("Eb", scale.Degree(3).ToString());
    }

    [Theory]
    [InlineData("C", "scale.exotic.diminished.half-whole", "C Db Eb E Gb G A Bb")]
    [InlineData("C", "scale.exotic.diminished.whole-half", "C D Eb F Gb Ab A B")]
    [InlineData("D", "scale.exotic.diminished.half-whole", "D Eb F F# Ab A B C")]
    [InlineData("D", "scale.exotic.diminished.whole-half", "D E F G Ab Bb B C#")]
    public void Octatonic_scales_preserve_the_requested_construction_root(
        string tonic,
        string definitionId,
        string expected)
    {
        var definition = ExoticScales.SymmetricAndJazz.Single(scale => scale.Id == definitionId);
        var scale = Scale.Create(SpelledPitch.Parse(tonic), definition);

        Assert.Equal(tonic, scale.Tonic.ToString());
        Assert.Equal(expected.Split(' '), scale.Pitches.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Octatonic_aliases_preserve_historical_diminished_definitions()
    {
        Assert.Same(ExoticScales.DiminishedHalfWhole, ExoticScales.OctatonicHalfWhole);
        Assert.Same(ExoticScales.DiminishedWholeHalf, ExoticScales.OctatonicWholeHalf);
        Assert.Contains(ExoticScales.OctatonicHalfWhole, ExoticScales.SymmetricAndJazz);
        Assert.Contains(ExoticScales.OctatonicWholeHalf, ExoticScales.SymmetricAndJazz);
    }

    [Fact]
    public void Degree_occurrences_expose_duplicate_formula_degree_numbers()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), ExoticScales.OctatonicHalfWhole);
        var thirds = scale.Degrees(3);

        Assert.Equal(["Eb", "E"], thirds.Select(pitch => pitch.ToString()));
        Assert.Equal("Eb", scale.Degree(3).ToString());
        var mutableView = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(thirds);
        Assert.Throws<NotSupportedException>(() => mutableView.Add(SpelledPitch.Parse("E")));
    }

    [Fact]
    public void Scale_aggregate_has_structural_equality_and_stable_hash_code()
    {
        var equivalentDefinition = new ScaleDefinition(
            StandardScales.Major.Id,
            StandardScales.Major.Degrees);
        var first = Scale.Create(SpelledPitch.Parse("Eb"), StandardScales.Major);
        var second = Scale.Create(SpelledPitch.Parse("Eb"), equivalentDefinition);
        var other = Scale.Create(SpelledPitch.Parse("E"), equivalentDefinition);

        Assert.Equal(first, second);
        Assert.True(first == second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, other);
    }

    [Fact]
    public void Pitches_are_exposed_as_a_read_only_collection()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);
        var mutableView = Assert.IsAssignableFrom<ICollection<SpelledPitch>>(scale.Pitches);

        Assert.Throws<NotSupportedException>(() => mutableView.Add(SpelledPitch.Parse("C")));
    }
}

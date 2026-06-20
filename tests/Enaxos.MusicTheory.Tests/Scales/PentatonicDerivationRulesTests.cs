using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tests.Scales;

public sealed class PentatonicDerivationRulesTests
{
    [Fact]
    public void Major_scale_derives_the_standard_major_pentatonic()
    {
        var source = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);

        var derivation = PentatonicScales.FromScale(source);

        Assert.Same(source, derivation.Source);
        Assert.Same(StandardScales.MajorPentatonic, derivation.Result.Definition);
        Assert.Equal([1, 2, 3, 5, 6], derivation.SelectedDegrees);
        Assert.Equal(["C", "D", "E", "G", "A"],
            derivation.Result.Pitches.Select(pitch => pitch.ToString()));
        Assert.Equal(PentatonicDerivationStrategy.StandardMajorOrMinor, derivation.Strategy);
    }

    [Fact]
    public void Natural_minor_scale_derives_the_standard_minor_pentatonic()
    {
        var source = Scale.Create(SpelledPitch.Parse("A"), StandardScales.NaturalMinor);

        var derivation = PentatonicScales.FromScale(source);

        Assert.Same(StandardScales.MinorPentatonic, derivation.Result.Definition);
        Assert.Equal([1, 3, 4, 5, 7], derivation.SelectedDegrees);
        Assert.Equal(["A", "C", "D", "E", "G"],
            derivation.Result.Pitches.Select(pitch => pitch.ToString()));
    }

    [Fact]
    public void Incompatible_scale_fails_explicitly()
    {
        var source = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Locrian);

        Assert.False(PentatonicScales.TryFromScale(source, out var result));
        Assert.Null(result);
        Assert.Throws<InvalidOperationException>(() => PentatonicScales.FromScale(source));
    }

    [Fact]
    public void Ambiguous_scale_with_both_standard_subsets_fails_explicitly()
    {
        var definition = new ScaleDefinition(
            "scale.ambiguous",
            [
                new FormulaDegree(1),
                new FormulaDegree(2),
                new FormulaDegree(3, -1),
                new FormulaDegree(4, -1),
                new FormulaDegree(5, -2),
                new FormulaDegree(6, -2),
                new FormulaDegree(7, -2),
                new FormulaDegree(8, -2),
            ]);
        var source = Scale.Create(SpelledPitch.Parse("C"), definition);

        Assert.False(PentatonicScales.TryFromScale(source, out _));
        Assert.Throws<InvalidOperationException>(() => PentatonicScales.FromScale(source));
    }

    [Fact]
    public void Explicit_selection_extracts_five_source_positions_and_preserves_provenance()
    {
        var source = Scale.Create(SpelledPitch.Parse("D"), StandardScales.Dorian);
        var positions = new[] { 1, 2, 4, 5, 7 };

        var derivation = PentatonicScales.FromScale(
            source,
            PentatonicDerivationStrategy.SelectSourceDegrees,
            positions);
        positions[1] = 7;

        Assert.Same(source, derivation.Source);
        Assert.Equal(PentatonicDerivationStrategy.SelectSourceDegrees, derivation.Strategy);
        Assert.Equal([1, 2, 4, 5, 7], derivation.SelectedDegrees);
        Assert.Equal(["D", "E", "G", "A", "C"],
            derivation.Result.Pitches.Select(pitch => pitch.ToString()));
        Assert.Equal("scale.pentatonic.derived", derivation.Result.Definition.Id);

        var mutableView = Assert.IsAssignableFrom<ICollection<int>>(derivation.SelectedDegrees);
        Assert.Throws<NotSupportedException>(() => mutableView.Add(6));
    }

    [Theory]
    [MemberData(nameof(InvalidSelections))]
    public void Invalid_explicit_selection_returns_false_and_FromScale_throws(int[]? positions)
    {
        var source = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);

        Assert.False(PentatonicScales.TryFromScale(
            source,
            out var result,
            PentatonicDerivationStrategy.SelectSourceDegrees,
            positions));
        Assert.Null(result);
        Assert.Throws<ArgumentException>(() => PentatonicScales.FromScale(
            source,
            PentatonicDerivationStrategy.SelectSourceDegrees,
            positions));
    }

    [Fact]
    public void Source_positions_are_rejected_for_the_standard_strategy()
    {
        var source = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);
        int[] positions = [1, 2, 3, 5, 6];

        Assert.False(PentatonicScales.TryFromScale(source, out _, sourceDegrees: positions));
        Assert.Throws<ArgumentException>(() =>
            PentatonicScales.FromScale(source, sourceDegrees: positions));
    }

    [Fact]
    public void Undefined_strategy_is_rejected()
    {
        var source = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PentatonicScales.TryFromScale(source, out _, (PentatonicDerivationStrategy)99));
    }

    public static TheoryData<int[]?> InvalidSelections => new()
    {
        null,
        Array.Empty<int>(),
        new[] { 1, 2, 3, 5 },
        new[] { 1, 2, 3, 5, 6, 7 },
        new[] { 2, 3, 4, 5, 6 },
        new[] { 1, 2, 2, 5, 6 },
        new[] { 1, 3, 2, 5, 6 },
        new[] { 1, 2, 3, 5, 8 },
    };
}

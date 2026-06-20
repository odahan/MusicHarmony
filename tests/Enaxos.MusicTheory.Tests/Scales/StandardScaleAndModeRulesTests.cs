using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tests.Scales;

public sealed class StandardScaleAndModeRulesTests
{
    private static readonly int[] MajorReference = [0, 2, 4, 5, 7, 9, 11];

    [Theory]
    [MemberData(nameof(StandardDefinitions))]
    public void Standard_definition_has_stable_id_and_formula(
        ScaleDefinition definition,
        string expectedId,
        FormulaDegree[] expectedDegrees)
    {
        Assert.Equal(expectedId, definition.Id);
        Assert.Equal(expectedDegrees, definition.Degrees);
    }

    [Fact]
    public void Standard_catalog_has_seven_modes_per_family_and_no_duplicate_all_entries()
    {
        var catalog = ModeCatalog.Standard;

        Assert.Equal(7, catalog.MajorModes.Count);
        Assert.Equal(7, catalog.NaturalMinorModes.Count);
        Assert.Equal(7, catalog.HarmonicMinorModes.Count);
        Assert.Equal(7, catalog.MelodicMinorModes.Count);
        Assert.Equal(21, catalog.All.Count);
        Assert.Equal(21, catalog.All.Select(mode => mode.Id).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Natural_minor_modes_are_relative_views_of_the_major_modal_collection()
    {
        var catalog = ModeCatalog.Standard;
        var expected = new[]
        {
            StandardScales.Aeolian,
            StandardScales.Locrian,
            StandardScales.Ionian,
            StandardScales.Dorian,
            StandardScales.Phrygian,
            StandardScales.Lydian,
            StandardScales.Mixolydian,
        };

        Assert.Equal(expected, catalog.NaturalMinorModes);
    }

    [Theory]
    [MemberData(nameof(RotatedFamilies))]
    public void Minor_family_modes_are_exact_rotations(
        IReadOnlyList<ScaleDefinition> modes,
        int[] sourceOffsets,
        string idPrefix)
    {
        Assert.Equal(7, modes.Count);

        for (var rotation = 0; rotation < modes.Count; rotation++)
        {
            Assert.Equal($"{idPrefix}.{rotation + 1}", modes[rotation].Id);
            Assert.Equal(
                ExpectedRotation(sourceOffsets, rotation),
                ChromaticOffsets(modes[rotation]));
        }
    }

    [Fact]
    public void Every_catalog_definition_obeys_scale_invariants()
    {
        foreach (var definition in ModeCatalog.Standard.All)
        {
            Assert.Equal(new FormulaDegree(1), definition.Degrees[0]);
            Assert.Equal(7, definition.Degrees.Count);
            Assert.Equal(
                definition.Degrees.Count,
                definition.Degrees.Select(degree => degree.Number).Distinct().Count());
            Assert.True(definition.Degrees.Zip(definition.Degrees.Skip(1))
                .All(pair => pair.First.Number < pair.Second.Number));
        }
    }

    public static TheoryData<ScaleDefinition, string, FormulaDegree[]> StandardDefinitions => new()
    {
        { StandardScales.Major, "scale.major", Degrees((1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0)) },
        { StandardScales.NaturalMinor, "scale.minor.natural", Degrees((1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1)) },
        { StandardScales.HarmonicMinor, "scale.minor.harmonic", Degrees((1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, 0)) },
        { StandardScales.MelodicMinorAscending, "scale.minor.melodic.ascending", Degrees((1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, 0)) },
        { StandardScales.Ionian, "mode.major.1", Degrees((1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0)) },
        { StandardScales.Dorian, "mode.major.2", Degrees((1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, -1)) },
        { StandardScales.Phrygian, "mode.major.3", Degrees((1, 0), (2, -1), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1)) },
        { StandardScales.Lydian, "mode.major.4", Degrees((1, 0), (2, 0), (3, 0), (4, 1), (5, 0), (6, 0), (7, 0)) },
        { StandardScales.Mixolydian, "mode.major.5", Degrees((1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, -1)) },
        { StandardScales.Aeolian, "mode.major.6", Degrees((1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1)) },
        { StandardScales.Locrian, "mode.major.7", Degrees((1, 0), (2, -1), (3, -1), (4, 0), (5, -1), (6, -1), (7, -1)) },
        { StandardScales.MajorPentatonic, "scale.pentatonic.major", Degrees((1, 0), (2, 0), (3, 0), (5, 0), (6, 0)) },
        { StandardScales.MinorPentatonic, "scale.pentatonic.minor", Degrees((1, 0), (3, -1), (4, 0), (5, 0), (7, -1)) },
    };

    public static TheoryData<IReadOnlyList<ScaleDefinition>, int[], string> RotatedFamilies => new()
    {
        { ModeCatalog.Standard.HarmonicMinorModes, new[] { 0, 2, 3, 5, 7, 8, 11 }, "mode.harmonic-minor" },
        { ModeCatalog.Standard.MelodicMinorModes, new[] { 0, 2, 3, 5, 7, 9, 11 }, "mode.melodic-minor" },
    };

    private static FormulaDegree[] Degrees(params (int Number, int Alteration)[] values) =>
        values.Select(value => new FormulaDegree(value.Number, value.Alteration)).ToArray();

    private static int[] ChromaticOffsets(ScaleDefinition definition) =>
        definition.Degrees
            .Select((degree, index) => MajorReference[index] + degree.Alteration)
            .ToArray();

    private static int[] ExpectedRotation(IReadOnlyList<int> source, int rotation)
    {
        var result = new int[source.Count];
        var root = source[rotation];

        for (var index = 0; index < source.Count; index++)
        {
            var value = source[(rotation + index) % source.Count] - root;
            result[index] = value < 0 ? value + 12 : value;
        }

        return result;
    }
}

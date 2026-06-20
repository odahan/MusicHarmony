using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

public static class StandardScales
{
    public static ScaleDefinition Major { get; } = Definition(
        "scale.major",
        (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0));

    public static ScaleDefinition NaturalMinor { get; } = Definition(
        "scale.minor.natural",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1));

    public static ScaleDefinition HarmonicMinor { get; } = Definition(
        "scale.minor.harmonic",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, 0));

    public static ScaleDefinition MelodicMinorAscending { get; } = Definition(
        "scale.minor.melodic.ascending",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, 0));

    public static ScaleDefinition Ionian { get; } = Definition(
        "mode.major.1",
        (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0));

    public static ScaleDefinition Dorian { get; } = Definition(
        "mode.major.2",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, -1));

    public static ScaleDefinition Phrygian { get; } = Definition(
        "mode.major.3",
        (1, 0), (2, -1), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1));

    public static ScaleDefinition Lydian { get; } = Definition(
        "mode.major.4",
        (1, 0), (2, 0), (3, 0), (4, 1), (5, 0), (6, 0), (7, 0));

    public static ScaleDefinition Mixolydian { get; } = Definition(
        "mode.major.5",
        (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, -1));

    public static ScaleDefinition Aeolian { get; } = Definition(
        "mode.major.6",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1));

    public static ScaleDefinition Locrian { get; } = Definition(
        "mode.major.7",
        (1, 0), (2, -1), (3, -1), (4, 0), (5, -1), (6, -1), (7, -1));

    public static ScaleDefinition MajorPentatonic { get; } = Definition(
        "scale.pentatonic.major",
        (1, 0), (2, 0), (3, 0), (5, 0), (6, 0));

    public static ScaleDefinition MinorPentatonic { get; } = Definition(
        "scale.pentatonic.minor",
        (1, 0), (3, -1), (4, 0), (5, 0), (7, -1));

    private static ScaleDefinition Definition(
        string id,
        params (int Number, int Alteration)[] degrees) =>
        new(
            id,
            degrees.Select(degree => new FormulaDegree(degree.Number, degree.Alteration)));
}

using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Provides canonical immutable definitions for the standard scales and major modes.</summary>
public static class StandardScales
{
    /// <summary>Gets the seven-degree major scale.</summary>
    public static ScaleDefinition Major { get; } = Definition(
        "scale.major",
        (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0));

    /// <summary>Gets the natural minor scale.</summary>
    public static ScaleDefinition NaturalMinor { get; } = Definition(
        "scale.minor.natural",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1));

    /// <summary>Gets the harmonic minor scale.</summary>
    public static ScaleDefinition HarmonicMinor { get; } = Definition(
        "scale.minor.harmonic",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, 0));

    /// <summary>Gets the ascending melodic minor scale.</summary>
    public static ScaleDefinition MelodicMinorAscending { get; } = Definition(
        "scale.minor.melodic.ascending",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, 0));

    /// <summary>Gets Ionian, the first mode of the major collection.</summary>
    public static ScaleDefinition Ionian { get; } = Definition(
        "mode.major.1",
        (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0));

    /// <summary>Gets Dorian, the second mode of the major collection.</summary>
    public static ScaleDefinition Dorian { get; } = Definition(
        "mode.major.2",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, -1));

    /// <summary>Gets Phrygian, the third mode of the major collection.</summary>
    public static ScaleDefinition Phrygian { get; } = Definition(
        "mode.major.3",
        (1, 0), (2, -1), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1));

    /// <summary>Gets Lydian, the fourth mode of the major collection.</summary>
    public static ScaleDefinition Lydian { get; } = Definition(
        "mode.major.4",
        (1, 0), (2, 0), (3, 0), (4, 1), (5, 0), (6, 0), (7, 0));

    /// <summary>Gets Mixolydian, the fifth mode of the major collection.</summary>
    public static ScaleDefinition Mixolydian { get; } = Definition(
        "mode.major.5",
        (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, -1));

    /// <summary>Gets Aeolian, the sixth mode of the major collection and the natural minor mode.</summary>
    public static ScaleDefinition Aeolian { get; } = Definition(
        "mode.major.6",
        (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, -1), (7, -1));

    /// <summary>Gets Locrian, the seventh mode of the major collection.</summary>
    public static ScaleDefinition Locrian { get; } = Definition(
        "mode.major.7",
        (1, 0), (2, -1), (3, -1), (4, 0), (5, -1), (6, -1), (7, -1));

    /// <summary>Gets the standard major pentatonic scale (degrees 1, 2, 3, 5, and 6).</summary>
    public static ScaleDefinition MajorPentatonic { get; } = Definition(
        "scale.pentatonic.major",
        (1, 0), (2, 0), (3, 0), (5, 0), (6, 0));

    /// <summary>Gets the standard minor pentatonic scale (degrees 1, flat 3, 4, 5, and flat 7).</summary>
    public static ScaleDefinition MinorPentatonic { get; } = Definition(
        "scale.pentatonic.minor",
        (1, 0), (3, -1), (4, 0), (5, 0), (7, -1));

    /// <summary>Builds a canonical definition from compact number/alteration table data.</summary>
    private static ScaleDefinition Definition(
        string id,
        params (int Number, int Alteration)[] degrees) =>
        new(
            id,
            degrees.Select(degree => new FormulaDegree(degree.Number, degree.Alteration)));
}

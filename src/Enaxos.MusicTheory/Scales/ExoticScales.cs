using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Provides curated non-core scale definitions that are useful in jazz, blues, and modal color work.</summary>
public static class ExoticScales
{
    private static readonly ReadOnlyCollection<ScaleDefinition> SymmetricAndJazzDefinitions = Array.AsReadOnly(
    [
        Definition("scale.exotic.whole-tone", (1, 0), (2, 0), (3, 0), (4, 1), (5, 1), (7, -1)),
        Definition("scale.exotic.diminished.whole-half", (1, 0), (2, 0), (3, -1), (4, 0), (5, -1), (6, -1), (6, 0), (7, 0)),
        Definition("scale.exotic.diminished.half-whole", (1, 0), (2, -1), (3, -1), (3, 0), (5, -1), (5, 0), (6, 0), (7, -1)),
        Definition("scale.exotic.augmented", (1, 0), (2, 1), (3, 0), (5, 0), (6, -1), (7, 0)),
    ]);

    private static readonly ReadOnlyCollection<ScaleDefinition> BluesAndBebopDefinitions = Array.AsReadOnly(
    [
        Definition("scale.exotic.blues.minor", (1, 0), (3, -1), (4, 0), (4, 1), (5, 0), (7, -1)),
        Definition("scale.exotic.blues.major", (1, 0), (2, 0), (3, -1), (3, 0), (5, 0), (6, 0)),
        Definition("scale.exotic.bebop.dominant", (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, -1), (7, 0)),
        Definition("scale.exotic.bebop.major", (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, -1), (6, 0), (7, 0)),
        Definition("scale.exotic.bebop.dorian", (1, 0), (2, 0), (3, -1), (4, 0), (5, 0), (6, 0), (7, -1), (7, 0)),
    ]);

    private static readonly ReadOnlyCollection<ScaleDefinition> RareMajorMinorDefinitions = Array.AsReadOnly(
    [
        Definition("scale.exotic.harmonic-major", (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, -1), (7, 0)),
        Definition("scale.exotic.double-harmonic-major", (1, 0), (2, -1), (3, 0), (4, 0), (5, 0), (6, -1), (7, 0)),
        Definition("scale.exotic.hungarian-minor", (1, 0), (2, 0), (3, -1), (4, 1), (5, 0), (6, -1), (7, 0)),
        Definition("scale.exotic.ukrainian-dorian", (1, 0), (2, 0), (3, -1), (4, 1), (5, 0), (6, 0), (7, -1)),
        Definition("scale.exotic.neapolitan-major", (1, 0), (2, -1), (3, -1), (4, 0), (5, 0), (6, 0), (7, 0)),
        Definition("scale.exotic.neapolitan-minor", (1, 0), (2, -1), (3, -1), (4, 0), (5, 0), (6, -1), (7, 0)),
    ]);

    private static readonly ReadOnlyCollection<ScaleDefinition> WesternizedOrientalDefinitions = Array.AsReadOnly(
    [
        Definition("scale.exotic.persian", (1, 0), (2, -1), (3, 0), (4, 0), (5, -1), (6, -1), (7, 0)),
        Definition("scale.exotic.arabian", (1, 0), (2, 0), (3, 0), (4, 0), (5, -1), (6, -1), (7, -1)),
        Definition("scale.exotic.spanish-phrygian", (1, 0), (2, -1), (3, 0), (4, 0), (5, 0), (6, -1), (7, -1)),
        Definition("scale.exotic.oriental", (1, 0), (2, -1), (3, 0), (4, 0), (5, -1), (6, 0), (7, -1)),
        Definition("scale.exotic.egyptian", (1, 0), (2, 0), (4, 0), (5, 0), (7, -1)),
    ]);

    private static readonly ReadOnlyCollection<ScaleDefinition> JapaneseDefinitions = Array.AsReadOnly(
    [
        Definition("scale.exotic.japanese.hirajoshi", (1, 0), (2, 0), (3, -1), (5, 0), (6, -1)),
        Definition("scale.exotic.japanese.insen", (1, 0), (2, -1), (4, 0), (5, 0), (7, -1)),
        Definition("scale.exotic.japanese.iwato", (1, 0), (2, -1), (4, 0), (5, -1), (7, -1)),
        Definition("scale.exotic.japanese.yo", (1, 0), (2, 0), (4, 0), (5, 0), (6, 0)),
        Definition("scale.exotic.japanese.kumoi", (1, 0), (2, 0), (3, -1), (5, 0), (6, 0)),
    ]);

    private static readonly ReadOnlyCollection<ScaleDefinition> AllDefinitions = Array.AsReadOnly(
        SymmetricAndJazzDefinitions
            .Concat(BluesAndBebopDefinitions)
            .Concat(RareMajorMinorDefinitions)
            .Concat(WesternizedOrientalDefinitions)
            .Concat(JapaneseDefinitions)
            .ToArray());

    /// <summary>Gets symmetric and jazz color scales.</summary>
    public static IReadOnlyList<ScaleDefinition> SymmetricAndJazz => SymmetricAndJazzDefinitions;

    /// <summary>Gets common blues and bebop scales.</summary>
    public static IReadOnlyList<ScaleDefinition> BluesAndBebop => BluesAndBebopDefinitions;

    /// <summary>Gets less common major and minor family scales.</summary>
    public static IReadOnlyList<ScaleDefinition> RareMajorMinor => RareMajorMinorDefinitions;

    /// <summary>Gets westernized oriental color scales.</summary>
    public static IReadOnlyList<ScaleDefinition> WesternizedOriental => WesternizedOrientalDefinitions;

    /// <summary>Gets curated Japanese pentatonic color scales.</summary>
    public static IReadOnlyList<ScaleDefinition> Japanese => JapaneseDefinitions;

    /// <summary>Gets every curated exotic scale definition.</summary>
    public static IReadOnlyList<ScaleDefinition> All => AllDefinitions;

    private static ScaleDefinition Definition(
        string id,
        params (int Number, int Alteration)[] degrees) =>
        new(
            id,
            degrees.Select(degree => new FormulaDegree(degree.Number, degree.Alteration)));
}

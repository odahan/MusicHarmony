using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Provides canonical definitions for the standard triads and seventh chords.</summary>
public static class StandardChords
{
    /// <summary>Gets the major triad.</summary>
    public static ChordDefinition Major { get; } = Create("chord.major", (1, 0), (3, 0), (5, 0));
    /// <summary>Gets the minor triad.</summary>
    public static ChordDefinition Minor { get; } = Create("chord.minor", (1, 0), (3, -1), (5, 0));
    /// <summary>Gets the diminished triad.</summary>
    public static ChordDefinition Diminished { get; } = Create("chord.diminished", (1, 0), (3, -1), (5, -1));
    /// <summary>Gets the augmented triad.</summary>
    public static ChordDefinition Augmented { get; } = Create("chord.augmented", (1, 0), (3, 0), (5, 1));
    /// <summary>Gets the dominant seventh chord.</summary>
    public static ChordDefinition DominantSeventh { get; } = Create("chord.dominant7", (1, 0), (3, 0), (5, 0), (7, -1));
    /// <summary>Gets the major seventh chord.</summary>
    public static ChordDefinition MajorSeventh { get; } = Create("chord.major7", (1, 0), (3, 0), (5, 0), (7, 0));
    /// <summary>Gets the minor seventh chord.</summary>
    public static ChordDefinition MinorSeventh { get; } = Create("chord.minor7", (1, 0), (3, -1), (5, 0), (7, -1));
    /// <summary>Gets the half-diminished seventh chord.</summary>
    public static ChordDefinition HalfDiminishedSeventh { get; } = Create("chord.half-diminished7", (1, 0), (3, -1), (5, -1), (7, -1));
    /// <summary>Gets the fully diminished seventh chord.</summary>
    public static ChordDefinition DiminishedSeventh { get; } = Create("chord.diminished7", (1, 0), (3, -1), (5, -1), (7, -2));

    /// <summary>Gets the deterministic catalog searched by chord recognition.</summary>
    internal static IReadOnlyList<ChordDefinition> All { get; } =
    [Major, Minor, Diminished, Augmented, DominantSeventh, MajorSeventh, MinorSeventh, HalfDiminishedSeventh, DiminishedSeventh];

    /// <summary>Builds a canonical definition from compact number/alteration table data.</summary>
    private static ChordDefinition Create(string id, params (int Number, int Alteration)[] degrees) =>
        new(id, degrees.Select(value => new FormulaDegree(value.Number, value.Alteration)));
}

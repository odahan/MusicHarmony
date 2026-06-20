using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Harmony;

public static class StandardChords
{
    public static ChordDefinition Major { get; } = Create("chord.major", (1, 0), (3, 0), (5, 0));
    public static ChordDefinition Minor { get; } = Create("chord.minor", (1, 0), (3, -1), (5, 0));
    public static ChordDefinition Diminished { get; } = Create("chord.diminished", (1, 0), (3, -1), (5, -1));
    public static ChordDefinition Augmented { get; } = Create("chord.augmented", (1, 0), (3, 0), (5, 1));
    public static ChordDefinition DominantSeventh { get; } = Create("chord.dominant7", (1, 0), (3, 0), (5, 0), (7, -1));
    public static ChordDefinition MajorSeventh { get; } = Create("chord.major7", (1, 0), (3, 0), (5, 0), (7, 0));
    public static ChordDefinition MinorSeventh { get; } = Create("chord.minor7", (1, 0), (3, -1), (5, 0), (7, -1));
    public static ChordDefinition HalfDiminishedSeventh { get; } = Create("chord.half-diminished7", (1, 0), (3, -1), (5, -1), (7, -1));
    public static ChordDefinition DiminishedSeventh { get; } = Create("chord.diminished7", (1, 0), (3, -1), (5, -1), (7, -2));

    internal static IReadOnlyList<ChordDefinition> All { get; } =
    [Major, Minor, Diminished, Augmented, DominantSeventh, MajorSeventh, MinorSeventh, HalfDiminishedSeventh, DiminishedSeventh];

    private static ChordDefinition Create(string id, params (int Number, int Alteration)[] degrees) =>
        new(id, degrees.Select(value => new FormulaDegree(value.Number, value.Alteration)));
}

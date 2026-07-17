using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Provides canonical definitions for the standard triads, suspended chords, sixth chords, seventh chords, and common extensions.</summary>
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
    /// <summary>Gets the suspended second chord.</summary>
    public static ChordDefinition SuspendedSecond { get; } = Create("chord.sus2", (1, 0), (2, 0), (5, 0));
    /// <summary>Gets the suspended fourth chord.</summary>
    public static ChordDefinition SuspendedFourth { get; } = Create("chord.sus4", (1, 0), (4, 0), (5, 0));
    /// <summary>Gets the major sixth chord.</summary>
    public static ChordDefinition MajorSixth { get; } = Create("chord.major6", (1, 0), (3, 0), (5, 0), (6, 0));
    /// <summary>Gets the minor sixth chord.</summary>
    public static ChordDefinition MinorSixth { get; } = Create("chord.minor6", (1, 0), (3, -1), (5, 0), (6, 0));
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
    /// <summary>Gets the dominant ninth chord.</summary>
    public static ChordDefinition DominantNinth { get; } = Create("chord.dominant9", (1, 0), (3, 0), (5, 0), (7, -1), (9, 0));
    /// <summary>Gets the major ninth chord.</summary>
    public static ChordDefinition MajorNinth { get; } = Create("chord.major9", (1, 0), (3, 0), (5, 0), (7, 0), (9, 0));
    /// <summary>Gets the minor ninth chord.</summary>
    public static ChordDefinition MinorNinth { get; } = Create("chord.minor9", (1, 0), (3, -1), (5, 0), (7, -1), (9, 0));
    /// <summary>Gets the dominant eleventh chord.</summary>
    public static ChordDefinition DominantEleventh { get; } = Create("chord.dominant11", (1, 0), (3, 0), (5, 0), (7, -1), (9, 0), (11, 0));
    /// <summary>Gets the major eleventh chord.</summary>
    public static ChordDefinition MajorEleventh { get; } = Create("chord.major11", (1, 0), (3, 0), (5, 0), (7, 0), (9, 0), (11, 0));
    /// <summary>Gets the minor eleventh chord.</summary>
    public static ChordDefinition MinorEleventh { get; } = Create("chord.minor11", (1, 0), (3, -1), (5, 0), (7, -1), (9, 0), (11, 0));
    /// <summary>Gets the dominant thirteenth chord.</summary>
    public static ChordDefinition DominantThirteenth { get; } = Create("chord.dominant13", (1, 0), (3, 0), (5, 0), (7, -1), (9, 0), (11, 0), (13, 0));
    /// <summary>Gets the major thirteenth chord.</summary>
    public static ChordDefinition MajorThirteenth { get; } = Create("chord.major13", (1, 0), (3, 0), (5, 0), (7, 0), (9, 0), (11, 0), (13, 0));
    /// <summary>Gets the minor thirteenth chord.</summary>
    public static ChordDefinition MinorThirteenth { get; } = Create("chord.minor13", (1, 0), (3, -1), (5, 0), (7, -1), (9, 0), (11, 0), (13, 0));

    /// <summary>Gets the deterministic standard catalog searched by chord recognition and scale-containment queries.</summary>
    public static IReadOnlyList<ChordDefinition> All { get; } =
    [
        Major,
        Minor,
        Diminished,
        Augmented,
        SuspendedSecond,
        SuspendedFourth,
        MajorSixth,
        MinorSixth,
        DominantSeventh,
        MajorSeventh,
        MinorSeventh,
        HalfDiminishedSeventh,
        DiminishedSeventh,
        DominantNinth,
        MajorNinth,
        MinorNinth,
        DominantEleventh,
        MajorEleventh,
        MinorEleventh,
        DominantThirteenth,
        MajorThirteenth,
        MinorThirteenth,
    ];

    /// <summary>Builds a canonical definition from compact number/alteration table data.</summary>
    private static ChordDefinition Create(string id, params (int Number, int Alteration)[] degrees) =>
        new(id, degrees.Select(value => new FormulaDegree(value.Number, value.Alteration)));
}

namespace Enaxos.MusicTheory.Analysis;

/// <summary>Configures chord matching, tolerated discrepancies, and result truncation.</summary>
public sealed record ChordRecognitionOptions
{
    /// <summary>Gets whether pitches with different spellings may match by pitch class.</summary>
    public bool AllowEnharmonicEquivalence { get; init; } = true;

    /// <summary>Gets whether candidates may omit expected chord tones.</summary>
    public bool AllowMissingTones { get; init; }

    /// <summary>Gets whether observed pitches outside a candidate chord are tolerated.</summary>
    public bool AllowAddedTones { get; init; }

    /// <summary>Gets the maximum number of ranked candidates returned.</summary>
    public int MaximumResults { get; init; } = 16;
}

namespace Enaxos.MusicTheory.Analysis;

public sealed record ChordRecognitionOptions
{
    public bool AllowEnharmonicEquivalence { get; init; } = true;
    public bool AllowMissingTones { get; init; }
    public bool AllowAddedTones { get; init; }
    public int MaximumResults { get; init; } = 16;
}

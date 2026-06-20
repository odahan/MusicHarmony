namespace Enaxos.MusicTheory.Analysis;

public sealed record ScaleRecognitionWeights
{
    public double Membership { get; init; } = 1.0;
    public double Coverage { get; init; } = 1.0;
    public double OutsideNotePenalty { get; init; } = 2.0;
    public double SpellingConsistency { get; init; } = 0.5;
    public double TonicEvidence { get; init; } = 0.5;
    public double ChordRootEvidence { get; init; } = 0.75;
}

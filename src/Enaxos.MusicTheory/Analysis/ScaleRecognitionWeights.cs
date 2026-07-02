namespace Enaxos.MusicTheory.Analysis;

/// <summary>Defines non-negative coefficients for each independently reported scale-recognition factor.</summary>
public sealed record ScaleRecognitionWeights
{
    /// <summary>Gets the weight of the fraction of observed pitches explained by the scale.</summary>
    public double Membership { get; init; } = 1.0;

    /// <summary>Gets the weight of the fraction of scale pitches present in the observation.</summary>
    public double Coverage { get; init; } = 1.0;

    /// <summary>Gets the penalty applied per observed pitch outside the scale.</summary>
    public double OutsideNotePenalty { get; init; } = 2.0;

    /// <summary>Gets the weight of exact spelling matches among observed pitches.</summary>
    public double SpellingConsistency { get; init; } = 0.5;

    /// <summary>Gets the bonus awarded when the candidate tonic is observed.</summary>
    public double TonicEvidence { get; init; } = 0.5;

    /// <summary>Gets the bonus awarded when the candidate tonic equals the lowest observed note.</summary>
    public double BassTonicEvidence { get; init; } = 0.75;

    /// <summary>Gets the bonus awarded when a supplied chord root equals the candidate tonic.</summary>
    public double ChordRootEvidence { get; init; } = 0.75;
}

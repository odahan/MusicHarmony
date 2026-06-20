using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Analysis;

/// <summary>Configures the scale catalog, score model, filtering, and probability conversion.</summary>
public sealed record ScaleRecognitionOptions
{
    /// <summary>Gets a custom catalog, or null to search all standard modes.</summary>
    public IReadOnlyList<ScaleDefinition>? Catalog { get; init; }

    /// <summary>Gets the non-negative coefficients used to calculate candidate scores.</summary>
    public ScaleRecognitionWeights Weights { get; init; } = new();

    /// <summary>Gets whether candidates containing any outside observed pitch are rejected.</summary>
    public bool StrictMembership { get; init; } = true;

    /// <summary>Gets the maximum number of candidates retained before probabilities are normalized.</summary>
    public int MaximumResults { get; init; } = 32;

    /// <summary>
    /// Gets the positive softmax temperature; lower values concentrate probability on top scores.
    /// </summary>
    public double ProbabilityTemperature { get; init; } = 1.0;
}

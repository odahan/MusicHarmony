using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Analysis;

public sealed record ScaleRecognitionOptions
{
    public IReadOnlyList<ScaleDefinition>? Catalog { get; init; }
    public ScaleRecognitionWeights Weights { get; init; } = new();
    public bool StrictMembership { get; init; } = true;
    public int MaximumResults { get; init; } = 32;
    public double ProbabilityTemperature { get; init; } = 1.0;
}

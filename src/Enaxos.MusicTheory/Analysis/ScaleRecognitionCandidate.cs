using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Analysis;

/// <summary>Describes one scale candidate, its pitch comparison, and its transparent score breakdown.</summary>
public sealed class ScaleRecognitionCandidate
{
    internal ScaleRecognitionCandidate(Scale scale, double score, double probability, SpelledPitch[] matched, SpelledPitch[] missing, SpelledPitch[] outside, IDictionary<string, double> factors)
    {
        Scale = scale; Score = score; RelativeProbability = probability;
        MatchedPitches = Array.AsReadOnly(matched); MissingPitches = Array.AsReadOnly(missing);
        OutsidePitches = Array.AsReadOnly(outside);
        ScoreFactors = new ReadOnlyDictionary<string, double>(new Dictionary<string, double>(factors, StringComparer.Ordinal));
    }

    /// <summary>Gets the realized candidate scale.</summary>
    public Scale Scale { get; }

    /// <summary>Gets the sum of the named weighted score factors.</summary>
    public double Score { get; }

    /// <summary>Gets this candidate's softmax probability relative to the returned result set.</summary>
    public double RelativeProbability { get; }

    /// <summary>Gets observed pitches matched enharmonically by the scale.</summary>
    public IReadOnlyList<SpelledPitch> MatchedPitches { get; }

    /// <summary>Gets scale pitches absent from the observation.</summary>
    public IReadOnlyList<SpelledPitch> MissingPitches { get; }

    /// <summary>Gets observed pitches that do not belong to the scale.</summary>
    public IReadOnlyList<SpelledPitch> OutsidePitches { get; }

    /// <summary>Gets the immutable named contributions whose sum is <see cref="Score"/>.</summary>
    public IReadOnlyDictionary<string, double> ScoreFactors { get; }
}

using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Analysis;

public sealed class ScaleRecognitionCandidate
{
    internal ScaleRecognitionCandidate(Scale scale, double score, double probability, SpelledPitch[] matched, SpelledPitch[] missing, SpelledPitch[] outside, IDictionary<string, double> factors)
    {
        Scale = scale; Score = score; RelativeProbability = probability;
        MatchedPitches = Array.AsReadOnly(matched); MissingPitches = Array.AsReadOnly(missing);
        OutsidePitches = Array.AsReadOnly(outside);
        ScoreFactors = new ReadOnlyDictionary<string, double>(new Dictionary<string, double>(factors, StringComparer.Ordinal));
    }

    public Scale Scale { get; }
    public double Score { get; }
    public double RelativeProbability { get; }
    public IReadOnlyList<SpelledPitch> MatchedPitches { get; }
    public IReadOnlyList<SpelledPitch> MissingPitches { get; }
    public IReadOnlyList<SpelledPitch> OutsidePitches { get; }
    public IReadOnlyDictionary<string, double> ScoreFactors { get; }
}

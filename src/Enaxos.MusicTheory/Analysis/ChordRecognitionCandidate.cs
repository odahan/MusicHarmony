using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Analysis;

public sealed class ChordRecognitionCandidate
{
    private readonly ReadOnlyCollection<SpelledPitch> _missing;
    private readonly ReadOnlyCollection<SpelledPitch> _added;

    internal ChordRecognitionCandidate(Chord chord, int? inversion, SpelledPitch[] missing, SpelledPitch[] added, double score, double confidence)
    {
        Chord = chord; InversionNumber = inversion; _missing = Array.AsReadOnly(missing);
        _added = Array.AsReadOnly(added); Score = score; Confidence = confidence;
    }

    public Chord Chord { get; }
    public int? InversionNumber { get; }
    public IReadOnlyList<SpelledPitch> MissingPitches => _missing;
    public IReadOnlyList<SpelledPitch> AddedPitches => _added;
    public double Score { get; }
    public double Confidence { get; }
}

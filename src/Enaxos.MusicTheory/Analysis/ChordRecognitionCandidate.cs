using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Analysis;

/// <summary>Describes one recognized chord candidate and the evidence used to rank it.</summary>
public sealed class ChordRecognitionCandidate
{
    /// <summary>Expected candidate pitches absent from the observation.</summary>
    private readonly ReadOnlyCollection<SpelledPitch> _missing;

    /// <summary>Observed pitches not explained by the candidate.</summary>
    private readonly ReadOnlyCollection<SpelledPitch> _added;

    internal ChordRecognitionCandidate(Chord chord, int? inversion, SpelledPitch[] missing, SpelledPitch[] added, double score, double confidence)
    {
        Chord = chord; InversionNumber = inversion; _missing = Array.AsReadOnly(missing);
        _added = Array.AsReadOnly(added); Score = score; Confidence = confidence;
    }

    /// <summary>Gets the candidate chord.</summary>
    public Chord Chord { get; }

    /// <summary>Gets the zero-based bass chord-tone index, or null when the bass is not a chord tone.</summary>
    public int? InversionNumber { get; }

    /// <summary>Gets candidate chord tones absent from the input.</summary>
    public IReadOnlyList<SpelledPitch> MissingPitches => _missing;

    /// <summary>Gets input pitches not explained by the candidate chord.</summary>
    public IReadOnlyList<SpelledPitch> AddedPitches => _added;

    /// <summary>Gets the deterministic heuristic ranking score; larger values are better.</summary>
    public double Score { get; }

    /// <summary>Gets the score normalized to the inclusive range 0 through 1.</summary>
    public double Confidence { get; }
}

using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Analysis;

public static class ScaleRecognizer
{
    public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(IEnumerable<Note> notes, ScaleRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(notes);
        var observed = notes.Select(note => note.Pitch).Distinct().ToArray();
        return Find(observed, null, options);
    }

    public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(Chord chord, ScaleRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chord);
        return Find(chord.Pitches.Distinct().ToArray(), chord.Root, options);
    }

    private static IReadOnlyList<ScaleRecognitionCandidate> Find(SpelledPitch[] observed, SpelledPitch? chordRoot, ScaleRecognitionOptions? options)
    {
        if (observed.Length == 0) throw new ArgumentException("At least one pitch is required.", nameof(observed));
        options ??= new ScaleRecognitionOptions();
        if (options.MaximumResults < 1 || !double.IsFinite(options.ProbabilityTemperature) || options.ProbabilityTemperature <= 0)
            throw new ArgumentOutOfRangeException(nameof(options));
        ArgumentNullException.ThrowIfNull(options.Weights);
        ValidateWeights(options.Weights);
        var catalog = (options.Catalog ?? ModeCatalog.Standard.All).ToArray();
        if (catalog.Length == 0) throw new ArgumentException("The scale catalog cannot be empty.", nameof(options));

        var raw = new List<(Scale Scale, double Score, SpelledPitch[] Matched, SpelledPitch[] Missing, SpelledPitch[] Outside, Dictionary<string, double> Factors)>();
        for (var tonicValue = 0; tonicValue < 12; tonicValue++)
            foreach (var definition in catalog)
            {
                var tonic = EnharmonicSpelling.For(PitchClass.FromChromaticIndex(tonicValue), EnharmonicPreference.PreferSharps);
                var scale = Scale.Create(tonic, definition);
                var matched = observed.Where(pitch => scale.Pitches.Any(value => value.PitchClass == pitch.PitchClass)).ToArray();
                var outside = observed.Where(pitch => !scale.Pitches.Any(value => value.PitchClass == pitch.PitchClass)).ToArray();
                if (options.StrictMembership && outside.Length > 0) continue;
                var missing = scale.Pitches.Where(pitch => !observed.Any(value => value.PitchClass == pitch.PitchClass)).ToArray();
                var exact = observed.Count(pitch => scale.Pitches.Contains(pitch));
                var factors = new Dictionary<string, double>(StringComparer.Ordinal)
                {
                    ["membership"] = options.Weights.Membership * matched.Length / observed.Length,
                    ["coverage"] = options.Weights.Coverage * matched.Length / scale.Pitches.Count,
                    ["outside"] = -options.Weights.OutsideNotePenalty * outside.Length,
                    ["spelling"] = options.Weights.SpellingConsistency * exact / observed.Length,
                    ["tonic"] = observed.Any(pitch => pitch.PitchClass == tonic.PitchClass) ? options.Weights.TonicEvidence : 0,
                    ["chordRoot"] = chordRoot is { } root && root.PitchClass == tonic.PitchClass ? options.Weights.ChordRootEvidence : 0,
                };
                raw.Add((scale, factors.Values.Sum(), matched, missing, outside, factors));
            }

        var selected = raw.OrderByDescending(item => item.Score)
            .ThenBy(item => item.Scale.Tonic.PitchClass.Value)
            .ThenBy(item => item.Scale.Definition.Id, StringComparer.Ordinal)
            .Take(options.MaximumResults).ToArray();
        if (selected.Length == 0) return [];
        var max = selected.Max(item => item.Score);
        var exponentials = selected.Select(item => Math.Exp((item.Score - max) / options.ProbabilityTemperature)).ToArray();
        var total = exponentials.Sum();
        return selected.Select((item, index) => new ScaleRecognitionCandidate(
            item.Scale, item.Score, exponentials[index] / total, item.Matched, item.Missing, item.Outside, item.Factors)).ToArray();
    }

    private static void ValidateWeights(ScaleRecognitionWeights weights)
    {
        var values = new[]
        {
            weights.Membership, weights.Coverage, weights.OutsideNotePenalty,
            weights.SpellingConsistency, weights.TonicEvidence, weights.ChordRootEvidence,
        };
        if (values.Any(value => !double.IsFinite(value) || value < 0))
        {
            throw new ArgumentOutOfRangeException(nameof(weights));
        }
    }
}

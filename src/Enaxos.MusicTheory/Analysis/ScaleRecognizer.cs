using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Analysis;

/// <summary>Ranks realized scales against observed pitches using a configurable transparent score model.</summary>
public static class ScaleRecognizer
{
    /// <summary>Raw candidate data retained until equivalent symmetric collections are collapsed.</summary>
    private sealed record RawCandidate(
        Scale Scale,
        double Score,
        SpelledPitch[] Matched,
        SpelledPitch[] Missing,
        SpelledPitch[] Outside,
        Dictionary<string, double> Factors);

    /// <summary>Finds scale candidates for absolute notes without assuming a chord root.</summary>
    public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(IEnumerable<Note> notes, ScaleRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(notes);
        var materialized = notes.ToArray();
        var observed = materialized.Select(note => note.Pitch).Distinct().ToArray();
        var bass = materialized.OrderBy(note => note.AbsoluteSemitone).FirstOrDefault().Pitch;
        return Find(observed, bass, null, options);
    }

    /// <summary>Finds scale candidates for chord tones and uses the chord root as additional tonic evidence.</summary>
    public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(Chord chord, ScaleRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chord);
        return Find(chord.Pitches.Distinct().ToArray(), null, chord.Root, options);
    }

    /// <summary>Runs the shared scoring pipeline over every tonic and catalog definition.</summary>
    private static IReadOnlyList<ScaleRecognitionCandidate> Find(
        SpelledPitch[] observed,
        SpelledPitch? bassPitch,
        SpelledPitch? chordRoot,
        ScaleRecognitionOptions? options)
    {
        if (observed.Length == 0) throw new ArgumentException("At least one pitch is required.", nameof(observed));
        options ??= new ScaleRecognitionOptions();
        if (options.MaximumResults < 1 || !double.IsFinite(options.ProbabilityTemperature) || options.ProbabilityTemperature <= 0)
            throw new ArgumentOutOfRangeException(nameof(options));
        ArgumentNullException.ThrowIfNull(options.Weights);
        ValidateWeights(options.Weights);
        var catalog = ResolveCatalog(options).ToArray();
        if (catalog.Length == 0) throw new ArgumentException("The scale catalog cannot be empty.", nameof(options));

        // Keep the individual factors with each raw candidate so callers can inspect
        // exactly how the final score was assembled.
        var raw = new List<RawCandidate>();
        for (var tonicValue = 0; tonicValue < 12; tonicValue++)
            foreach (var definition in catalog)
            {
                var tonic = EnharmonicSpelling.For(PitchClass.FromChromaticIndex(tonicValue), EnharmonicPreference.PreferSharps);
                var scale = Scale.Create(tonic, definition);
                var isOctatonic = IsOctatonic(scale);
                var matched = observed.Where(pitch => scale.Pitches.Any(value => value.PitchClass == pitch.PitchClass)).ToArray();
                var outside = observed.Where(pitch => !scale.Pitches.Any(value => value.PitchClass == pitch.PitchClass)).ToArray();
                if (options.StrictMembership && outside.Length > 0) continue;
                var missing = scale.Pitches.Where(pitch => !observed.Any(value => value.PitchClass == pitch.PitchClass)).ToArray();
                var exact = observed.Count(pitch => scale.Pitches.Contains(pitch));
                var hasRoot = observed.Any(pitch => pitch.PitchClass == tonic.PitchClass);
                var bassMatchesRoot = bassPitch is { } bass && bass.PitchClass == tonic.PitchClass;
                var chordRootMatchesRoot = chordRoot is { } root && root.PitchClass == tonic.PitchClass;
                var factors = new Dictionary<string, double>(StringComparer.Ordinal)
                {
                    ["membership"] = options.Weights.Membership * matched.Length / observed.Length,
                    ["coverage"] = options.Weights.Coverage * matched.Length / scale.Pitches.Count,
                    ["outside"] = -options.Weights.OutsideNotePenalty * outside.Length,
                    ["spelling"] = options.Weights.SpellingConsistency * exact / observed.Length,
                    ["tonic"] = !isOctatonic && hasRoot ? options.Weights.TonicEvidence : 0,
                    ["constructionRoot"] = isOctatonic && hasRoot ? options.Weights.TonicEvidence : 0,
                    ["bassTonic"] = !isOctatonic && bassMatchesRoot ? options.Weights.BassTonicEvidence : 0,
                    ["bassConstructionRoot"] = isOctatonic && bassMatchesRoot ? options.Weights.BassTonicEvidence : 0,
                    ["chordRoot"] = !isOctatonic && chordRootMatchesRoot ? options.Weights.ChordRootEvidence : 0,
                    ["chordConstructionRoot"] = isOctatonic && chordRootMatchesRoot ? options.Weights.ChordRootEvidence : 0,
                };
                raw.Add(new RawCandidate(scale, factors.Values.Sum(), matched, missing, outside, factors));
            }

        // Limit candidates before softmax so reported probabilities sum to one across
        // exactly the result set visible to the caller.
        var selected = CollapseEquivalentOctatonicCandidates(raw)
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.Scale.Tonic.PitchClass.Value)
            .ThenBy(item => item.Scale.Definition.Id, StringComparer.Ordinal)
            .Take(options.MaximumResults).ToArray();
        if (selected.Length == 0) return [];
        // Subtracting the maximum score is the standard numerically stable softmax form.
        var max = selected.Max(item => item.Score);
        var exponentials = selected.Select(item => Math.Exp((item.Score - max) / options.ProbabilityTemperature)).ToArray();
        var total = exponentials.Sum();
        return selected.Select((item, index) => new ScaleRecognitionCandidate(
            item.Scale, item.Score, exponentials[index] / total, item.Matched, item.Missing, item.Outside, item.Factors)).ToArray();
    }

    /// <summary>Keeps one representative for each equivalent octatonic pitch-class collection.</summary>
    private static IEnumerable<RawCandidate> CollapseEquivalentOctatonicCandidates(IEnumerable<RawCandidate> candidates)
    {
        var octatonicGroups = new Dictionary<string, List<RawCandidate>>(StringComparer.Ordinal);

        foreach (var candidate in candidates)
        {
            if (!IsOctatonic(candidate.Scale))
            {
                yield return candidate;
                continue;
            }

            var key = PitchClassSetKey(candidate.Scale);
            if (!octatonicGroups.TryGetValue(key, out var group))
            {
                group = [];
                octatonicGroups.Add(key, group);
            }

            group.Add(candidate);
        }

        foreach (var group in octatonicGroups.Values)
        {
            yield return group
                .OrderByDescending(item => item.Score)
                .ThenByDescending(item => item.Factors["bassConstructionRoot"])
                .ThenByDescending(item => item.Factors["constructionRoot"])
                .ThenBy(item => item.Scale.Tonic.PitchClass.Value)
                .ThenBy(item => item.Scale.Definition.Id, StringComparer.Ordinal)
                .First();
        }
    }

    /// <summary>Detects eight-note scales built by alternating half and whole steps.</summary>
    private static bool IsOctatonic(Scale scale)
    {
        if (scale.Pitches.Count != 8)
        {
            return false;
        }

        var firstStep = scale.Pitches[0].PitchClass.DistanceUpTo(scale.Pitches[1].PitchClass);
        if (firstStep is not (1 or 2))
        {
            return false;
        }

        for (var index = 0; index < scale.Pitches.Count; index++)
        {
            var next = (index + 1) % scale.Pitches.Count;
            var step = scale.Pitches[index].PitchClass.DistanceUpTo(scale.Pitches[next].PitchClass);
            var expected = index % 2 == 0 ? firstStep : 3 - firstStep;
            if (step != expected)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Builds a deterministic key from the pitch classes contained by a scale.</summary>
    private static string PitchClassSetKey(Scale scale) =>
        string.Join(",", scale.Pitches
            .Select(pitch => pitch.PitchClass.Value)
            .Order()
            .Distinct());

    /// <summary>Returns the explicit catalog or the requested built-in recognition catalog.</summary>
    private static IReadOnlyList<ScaleDefinition> ResolveCatalog(ScaleRecognitionOptions options)
    {
        if (options.Catalog is not null)
        {
            return options.Catalog;
        }

        return (options.IncludePentatonicCandidates, options.IncludeExoticCandidates) switch
        {
            (true, true) => ModeCatalog.Standard.AllWithPentatonicAndExoticScales,
            (true, false) => ModeCatalog.Standard.AllWithPentatonicScales,
            (false, true) => ModeCatalog.Standard.AllWithExoticScales,
            _ => ModeCatalog.Standard.All,
        };
    }

    /// <summary>Rejects negative, infinite, and NaN coefficients before scoring.</summary>
    private static void ValidateWeights(ScaleRecognitionWeights weights)
    {
        var values = new[]
        {
            weights.Membership, weights.Coverage, weights.OutsideNotePenalty,
            weights.SpellingConsistency, weights.TonicEvidence, weights.BassTonicEvidence,
            weights.ChordRootEvidence,
        };
        if (values.Any(value => !double.IsFinite(value) || value < 0))
        {
            throw new ArgumentOutOfRangeException(nameof(weights));
        }
    }
}

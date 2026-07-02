using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Enaxos.MusicTheory.Analysis;

/// <summary>Ranks standard chord definitions against observed absolute notes.</summary>
public static class ChordRecognizer
{
    /// <summary>Recognizes chord candidates from notes, using the lowest note as inversion evidence.</summary>
    public static IReadOnlyList<ChordRecognitionCandidate> Recognize(IEnumerable<Note> notes, ChordRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(notes);
        var copy = notes.ToArray();
        if (copy.Length == 0) throw new ArgumentException("At least one note is required.", nameof(notes));
        options ??= new ChordRecognitionOptions();
        if (options.MaximumResults < 1) throw new ArgumentOutOfRangeException(nameof(options));

        // Recognition works on distinct spellings, while the original absolute notes
        // remain necessary to identify the true bass independently of input order.
        var observed = copy.Select(note => note.Pitch).Distinct().ToArray();
        var bass = copy.MinBy(note => note.AbsoluteSemitone).Pitch;
        var candidates = new List<ChordRecognitionCandidate>();

        foreach (var root in observed)
            foreach (var definition in StandardChords.All)
            {
                var chord = Chord.Create(root, definition);
                bool Match(SpelledPitch left, SpelledPitch right) => options.AllowEnharmonicEquivalence
                    ? left.PitchClass == right.PitchClass : left == right;
                var missing = chord.Pitches.Where(expected => !observed.Any(value => Match(expected, value))).ToArray();
                var added = observed.Where(value => !chord.Pitches.Any(expected => Match(expected, value))).ToArray();
                var implicitOmission = IsImplicitExtendedChordOmission(chord, observed, missing);
                if ((missing.Length > 0 && !options.AllowMissingTones && !implicitOmission) ||
                    (added.Length > 0 && !options.AllowAddedTones)) continue;

                var recognized = chord.Pitches
                    .Select(expected => ResolveRecognizedPitch(expected, observed, options.AllowEnharmonicEquivalence))
                    .Where(pitch => pitch.HasValue)
                    .Select(pitch => pitch!.Value)
                    .ToArray();
                var exactSpellings = chord.Pitches.Count(expected => observed.Contains(expected));
                var inversion = Array.FindIndex(chord.Pitches.ToArray(), pitch => pitch.PitchClass == bass.PitchClass);
                var bassPitch = inversion < 0
                    ? null
                    : ResolveRecognizedPitch(chord.Pitches[inversion], observed, options.AllowEnharmonicEquivalence);
                // Missing structural tones carry a larger penalty than additions. Exact
                // spelling and root position are deterministic tie-breaking evidence.
                var missingPenalty = implicitOmission ? 5d : 20d;
                var score = 100d - (missingPenalty * missing.Length) - (10d * added.Length) + exactSpellings + (inversion == 0 ? 2d : 0d);
                candidates.Add(new ChordRecognitionCandidate(chord, inversion < 0 ? null : inversion, bassPitch, recognized, missing, added, score, Math.Clamp(score / 105d, 0d, 1d)));
            }

        // Stable secondary ordering makes results reproducible when heuristic scores tie.
        return candidates.OrderByDescending(candidate => candidate.Score)
            .ThenByDescending(candidate => candidate.RecognizedPitches.Count)
            .ThenBy(candidate => candidate.MissingPitches.Count)
            .ThenBy(candidate => candidate.Chord.Root.Letter)
            .ThenBy(candidate => candidate.Chord.Root.Accidental.Semitones)
            .ThenBy(candidate => candidate.Chord.Definition.Id, StringComparer.Ordinal)
            .Take(options.MaximumResults).ToArray();
    }

    /// <summary>Attempts to return the highest-ranked chord candidate recognized from notes.</summary>
    public static bool TryRecognizeBest(
        IEnumerable<Note> notes,
        [NotNullWhen(true)] out ChordRecognitionCandidate? candidate,
        ChordRecognitionOptions? options = null)
    {
        var candidates = Recognize(notes, options);
        candidate = candidates.Count > 0 ? candidates[0] : null;
        return candidate is not null;
    }

    /// <summary>Recognizes candidates from a chord's canonical close root-position realization.</summary>
    public static IReadOnlyList<ChordRecognitionCandidate> Recognize(Chord chord, ChordRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chord);
        return Recognize(ChordRealization.CreateRootPosition(chord, 4).Notes, options);
    }

    private static SpelledPitch? ResolveRecognizedPitch(
        SpelledPitch expected,
        IReadOnlyList<SpelledPitch> observed,
        bool allowEnharmonicEquivalence)
    {
        foreach (var pitch in observed)
        {
            if (pitch == expected)
            {
                return pitch;
            }
        }

        if (!allowEnharmonicEquivalence)
        {
            return null;
        }

        foreach (var pitch in observed)
        {
            if (pitch.PitchClass == expected.PitchClass)
            {
                return pitch;
            }
        }

        return null;
    }

    private static bool IsImplicitExtendedChordOmission(
        Chord chord,
        IReadOnlyList<SpelledPitch> observed,
        IReadOnlyList<SpelledPitch> missing)
    {
        if (missing.Count == 0 || observed.Count < 4)
        {
            return false;
        }

        var degrees = chord.Definition.Degrees;
        var optionalDegreeNumbers = chord.Definition.Id switch
        {
            "chord.dominant11" or "chord.major11" or "chord.minor11" => new HashSet<int> { 5, 9 },
            "chord.dominant13" or "chord.major13" or "chord.minor13" => new HashSet<int> { 5, 9, 11 },
            _ => null,
        };
        if (optionalDegreeNumbers is null)
        {
            return false;
        }

        for (var index = 0; index < chord.Pitches.Count; index++)
        {
            if (!missing.Contains(chord.Pitches[index]))
            {
                continue;
            }

            if (!optionalDegreeNumbers.Contains(degrees[index].Number))
            {
                return false;
            }
        }

        return true;
    }
}

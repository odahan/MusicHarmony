using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Analysis;

public static class ChordRecognizer
{
    public static IReadOnlyList<ChordRecognitionCandidate> Recognize(IEnumerable<Note> notes, ChordRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(notes);
        var copy = notes.ToArray();
        if (copy.Length == 0) throw new ArgumentException("At least one note is required.", nameof(notes));
        options ??= new ChordRecognitionOptions();
        if (options.MaximumResults < 1) throw new ArgumentOutOfRangeException(nameof(options));

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
                if ((missing.Length > 0 && !options.AllowMissingTones) || (added.Length > 0 && !options.AllowAddedTones)) continue;

                var exactSpellings = chord.Pitches.Count(expected => observed.Contains(expected));
                var inversion = Array.FindIndex(chord.Pitches.ToArray(), pitch => pitch.PitchClass == bass.PitchClass);
                var score = 100d - (20d * missing.Length) - (10d * added.Length) + exactSpellings + (inversion == 0 ? 2d : 0d);
                candidates.Add(new ChordRecognitionCandidate(chord, inversion < 0 ? null : inversion, missing, added, score, Math.Clamp(score / 105d, 0d, 1d)));
            }

        return candidates.OrderByDescending(candidate => candidate.Score)
            .ThenBy(candidate => candidate.Chord.Root.Letter)
            .ThenBy(candidate => candidate.Chord.Root.Accidental.Semitones)
            .ThenBy(candidate => candidate.Chord.Definition.Id, StringComparer.Ordinal)
            .Take(options.MaximumResults).ToArray();
    }

    public static IReadOnlyList<ChordRecognitionCandidate> Recognize(Chord chord, ChordRecognitionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chord);
        return Recognize(ChordRealization.CreateRootPosition(chord, 4).Notes, options);
    }
}

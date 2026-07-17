using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Builds basic scale-degree chords from realized scales.</summary>
public static class ScaleHarmony
{
    /// <summary>The standard triad formulas that receive conventional chord definitions and Roman quality.</summary>
    private static readonly ChordDefinition[] StandardTriads =
    [
        StandardChords.Major,
        StandardChords.Minor,
        StandardChords.Diminished,
        StandardChords.Augmented,
    ];

    /// <summary>Returns one three-note chord per degree by stacking alternate tones of a scale.</summary>
    public static IReadOnlyList<ScaleChord> GetDiatonicTriads(Scale scale)
    {
        ArgumentNullException.ThrowIfNull(scale);

        var count = scale.Pitches.Count;
        if (count is < 3 or > 12)
        {
            throw new ArgumentException("Only scales with three to twelve tones are supported.", nameof(scale));
        }

        var result = new ScaleChord[count];
        for (var index = 0; index < count; index++)
        {
            var tones = new[]
            {
                scale.Pitches[index],
                scale.Pitches[(index + 2) % count],
                scale.Pitches[(index + 4) % count],
            };
            var definition = CreateDefinition(index + 1, tones);
            var chord = Chord.Create(tones[0], definition);
            var quality = QualityOf(definition);
            result[index] = new ScaleChord(
                scale,
                new ScaleDegreeNumber(index + 1),
                chord,
                quality);
        }

        return Array.AsReadOnly(result);
    }

    /// <summary>
    /// Returns standard chords whose pitch classes are fully contained in the scale.
    /// </summary>
    /// <remarks>
    /// This method is collection-based and does not infer harmonic function or
    /// Roman numerals. Roots are tried in scale order, and chord definitions are
    /// tried in <see cref="StandardChords.All"/> order.
    /// </remarks>
    public static IReadOnlyList<Chord> GetContainedStandardChords(Scale scale) =>
        GetContainedChords(scale, StandardChords.All);

    /// <summary>
    /// Returns chords from the supplied catalog whose pitch classes are fully contained in the scale.
    /// </summary>
    /// <remarks>
    /// This method compares pitch classes rather than stacked scale degrees.
    /// It is intended for collections such as octatonic scales where many
    /// standard triads and seventh chords are available without implying tonal
    /// function. Results are immutable and deterministic.
    /// </remarks>
    public static IReadOnlyList<Chord> GetContainedChords(Scale scale, IEnumerable<ChordDefinition> catalog)
    {
        ArgumentNullException.ThrowIfNull(scale);
        ArgumentNullException.ThrowIfNull(catalog);

        var definitions = catalog.ToArray();
        if (definitions.Length == 0)
        {
            throw new ArgumentException("The chord catalog cannot be empty.", nameof(catalog));
        }

        if (definitions.Any(definition => definition is null))
        {
            throw new ArgumentException("The chord catalog cannot contain null definitions.", nameof(catalog));
        }

        var scalePitchClasses = scale.Pitches
            .Select(pitch => pitch.PitchClass.Value)
            .ToHashSet();
        var result = new List<Chord>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var root in scale.Pitches)
        {
            foreach (var definition in definitions)
            {
                var chord = Chord.Create(root, definition);
                if (!chord.Pitches.All(pitch => scalePitchClasses.Contains(pitch.PitchClass.Value)))
                {
                    continue;
                }

                var key = string.Concat(root.PitchClass.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), "|", definition.Id);
                if (seen.Add(key))
                {
                    result.Add(chord);
                }
            }
        }

        return Array.AsReadOnly(result.ToArray());
    }

    /// <summary>Creates a standard triad definition when possible, otherwise a stable scale-derived formula.</summary>
    private static ChordDefinition CreateDefinition(int scaleDegree, IReadOnlyList<SpelledPitch> tones)
    {
        var degrees = tones
            .Select(tone => DegreeFromRoot(tones[0], tone))
            .ToArray();

        foreach (var standard in StandardTriads)
        {
            if (degrees.SequenceEqual(standard.Degrees))
            {
                return standard;
            }
        }

        return new ChordDefinition($"chord.scale-degree.{scaleDegree}", degrees);
    }

    /// <summary>Expresses a target tone as a root-relative formula degree with chromatic alteration.</summary>
    private static FormulaDegree DegreeFromRoot(SpelledPitch root, SpelledPitch target)
    {
        var letterDistance = (int)target.Letter - (int)root.Letter;
        if (letterDistance < 0)
        {
            letterDistance += 7;
        }

        var number = letterDistance + 1;
        var semitoneDistance = root.PitchClass.DistanceUpTo(target.PitchClass);
        var alteration = semitoneDistance - FormulaDegreeSemitones.ReferenceSemitones(number);
        return new FormulaDegree(number, alteration);
    }

    /// <summary>Maps standard triad definitions to the quality categories used by Roman numerals.</summary>
    private static HarmonicChordQuality QualityOf(ChordDefinition definition)
    {
        if (definition.Equals(StandardChords.Major))
        {
            return HarmonicChordQuality.Major;
        }

        if (definition.Equals(StandardChords.Minor))
        {
            return HarmonicChordQuality.Minor;
        }

        if (definition.Equals(StandardChords.Diminished))
        {
            return HarmonicChordQuality.Diminished;
        }

        if (definition.Equals(StandardChords.Augmented))
        {
            return HarmonicChordQuality.Augmented;
        }

        return HarmonicChordQuality.Other;
    }
}

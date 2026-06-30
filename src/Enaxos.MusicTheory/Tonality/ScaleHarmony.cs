using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Builds basic scale-degree chords from pentatonic and heptatonic realized scales.</summary>
public static class ScaleHarmony
{
    /// <summary>The supported scale sizes for basic degree-chord generation.</summary>
    private static readonly int[] SupportedToneCounts = [5, 7];
    /// <summary>The standard triad formulas that receive conventional chord definitions and Roman quality.</summary>
    private static readonly ChordDefinition[] StandardTriads =
    [
        StandardChords.Major,
        StandardChords.Minor,
        StandardChords.Diminished,
        StandardChords.Augmented,
    ];

    /// <summary>Returns one three-note chord per degree by stacking alternate tones of a pentatonic or heptatonic scale.</summary>
    public static IReadOnlyList<ScaleChord> GetDiatonicTriads(Scale scale)
    {
        ArgumentNullException.ThrowIfNull(scale);

        var count = scale.Pitches.Count;
        if (!SupportedToneCounts.Contains(count))
        {
            throw new ArgumentException("Only pentatonic and heptatonic scales are supported.", nameof(scale));
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

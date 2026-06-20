using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Interprets chord roots and standard qualities in the context of a major or natural-minor key.</summary>
public static class HarmonicFunctions
{
    /// <summary>Analyzes a chord or throws when its root is outside the supplied key.</summary>
    public static HarmonicFunction Analyze(Chord chord, MusicalKey key)
    {
        if (!TryAnalyze(chord, key, out var result))
            throw new InvalidOperationException("The chord root is not a degree of the supplied key.");
        return result;
    }

    /// <summary>Attempts to assign a diatonic harmonic function to a chord.</summary>
    /// <remarks>Enharmonic root membership is accepted; the current chord model does not carry an inversion.</remarks>
    public static bool TryAnalyze(Chord chord, MusicalKey key, out HarmonicFunction result)
    {
        ArgumentNullException.ThrowIfNull(chord);
        var definition = key.Mode == KeyMode.Major ? StandardScales.Major : StandardScales.NaturalMinor;
        var scale = Scale.Create(key.Tonic, definition);
        var degree = -1;
        for (var index = 0; index < scale.Pitches.Count; index++)
            if (scale.Pitches[index].PitchClass == chord.Root.PitchClass) { degree = index + 1; break; }

        if (degree < 1) { result = default; return false; }
        result = new HarmonicFunction(new ScaleDegreeNumber(degree), Quality(chord.Definition), 0);
        return true;
    }

    /// <summary>Collapses supported chord definitions into the quality categories used by Roman-numeral formatting.</summary>
    private static HarmonicChordQuality Quality(ChordDefinition definition)
    {
        if (definition.Equals(StandardChords.Major) || definition.Equals(StandardChords.MajorSeventh) || definition.Equals(StandardChords.DominantSeventh)) return HarmonicChordQuality.Major;
        if (definition.Equals(StandardChords.Minor) || definition.Equals(StandardChords.MinorSeventh)) return HarmonicChordQuality.Minor;
        if (definition.Equals(StandardChords.Diminished) || definition.Equals(StandardChords.DiminishedSeventh)) return HarmonicChordQuality.Diminished;
        if (definition.Equals(StandardChords.HalfDiminishedSeventh)) return HarmonicChordQuality.HalfDiminished;
        if (definition.Equals(StandardChords.Augmented)) return HarmonicChordQuality.Augmented;
        return HarmonicChordQuality.Other;
    }
}

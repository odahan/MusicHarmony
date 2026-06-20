namespace Enaxos.MusicTheory.Tonality;

/// <summary>Classifies the chord quality attached to a harmonic scale-degree interpretation.</summary>
public enum HarmonicChordQuality
{
    /// <summary>A major triadic quality, including supported major and dominant sevenths.</summary>
    Major,
    /// <summary>A minor triadic quality, including the supported minor seventh.</summary>
    Minor,
    /// <summary>A diminished triadic quality, including the fully diminished seventh.</summary>
    Diminished,
    /// <summary>A half-diminished seventh quality.</summary>
    HalfDiminished,
    /// <summary>An augmented triadic quality.</summary>
    Augmented,
    /// <summary>A chord definition not mapped to a standard harmonic quality.</summary>
    Other,
}

namespace Enaxos.MusicTheory.Intervals;

/// <summary>Identifies the base quality family of a musical interval.</summary>
public enum IntervalQualityKind
{
    /// <summary>A quality below the minor or perfect reference.</summary>
    Diminished,
    /// <summary>The minor quality used by seconds, thirds, sixths, and sevenths.</summary>
    Minor,
    /// <summary>The perfect quality used by unisons, fourths, fifths, and octaves.</summary>
    Perfect,
    /// <summary>The major quality used by seconds, thirds, sixths, and sevenths.</summary>
    Major,
    /// <summary>A quality above the major or perfect reference.</summary>
    Augmented,
}

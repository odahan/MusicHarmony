namespace Enaxos.MusicTheory.Scales;

/// <summary>Classifies a scale definition by its originating collection.</summary>
public enum ScaleFamily
{
    /// <summary>The major diatonic collection and its modes.</summary>
    Major,
    /// <summary>The harmonic-minor collection and its modes.</summary>
    HarmonicMinor,
    /// <summary>The ascending melodic-minor collection and its modes.</summary>
    MelodicMinor,
    /// <summary>A five-note pentatonic collection.</summary>
    Pentatonic,
    /// <summary>A caller-defined collection outside the standard catalogs.</summary>
    Custom,
}

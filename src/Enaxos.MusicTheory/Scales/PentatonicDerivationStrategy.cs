namespace Enaxos.MusicTheory.Scales;

/// <summary>Specifies how a five-note scale is derived from a source scale.</summary>
public enum PentatonicDerivationStrategy
{
    /// <summary>Selects the unique standard major or minor pentatonic subset found in the source.</summary>
    StandardMajorOrMinor,
    /// <summary>Uses five explicitly supplied one-based positions from the source scale.</summary>
    SelectSourceDegrees,
}

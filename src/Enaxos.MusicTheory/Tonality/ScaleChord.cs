using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Describes one basic chord obtained by stacking alternate degrees of a supported scale.</summary>
public sealed class ScaleChord
{
    internal ScaleChord(
        Scale sourceScale,
        ScaleDegreeNumber degree,
        Chord chord,
        HarmonicChordQuality quality)
    {
        SourceScale = sourceScale;
        Degree = degree;
        Chord = chord;
        Quality = quality;
        Function = new HarmonicFunction(degree, quality, 0);
    }

    /// <summary>Gets the realized scale from which the chord was derived.</summary>
    public Scale SourceScale { get; }

    /// <summary>Gets the one-based scale degree used as the chord root.</summary>
    public ScaleDegreeNumber Degree { get; }

    /// <summary>Gets the realized chord with its root and chord-tone spellings.</summary>
    public Chord Chord { get; }

    /// <summary>Gets the normalized quality used for Roman-numeral formatting.</summary>
    public HarmonicChordQuality Quality { get; }

    /// <summary>Gets the harmonic-function value that can be formatted as a Roman numeral.</summary>
    public HarmonicFunction Function { get; }
}

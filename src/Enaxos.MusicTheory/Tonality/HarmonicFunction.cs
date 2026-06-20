namespace Enaxos.MusicTheory.Tonality;

/// <summary>Describes a chord's scale-degree, quality, and inversion interpretation within a key.</summary>
public readonly record struct HarmonicFunction
{
    /// <summary>Creates a harmonic-function value.</summary>
    public HarmonicFunction(ScaleDegreeNumber degree, HarmonicChordQuality quality, int inversionNumber)
    {
        if (!Enum.IsDefined(quality)) throw new ArgumentOutOfRangeException(nameof(quality));
        if (inversionNumber < 0) throw new ArgumentOutOfRangeException(nameof(inversionNumber));
        Degree = degree;
        Quality = quality;
        InversionNumber = inversionNumber;
    }

    /// <summary>Gets the diatonic scale degree of the chord root.</summary>
    public ScaleDegreeNumber Degree { get; }

    /// <summary>Gets the normalized harmonic chord quality.</summary>
    public HarmonicChordQuality Quality { get; }

    /// <summary>Gets the zero-based inversion number, where zero is root position.</summary>
    public int InversionNumber { get; }
}

namespace Enaxos.MusicTheory.Tonality;

public readonly record struct HarmonicFunction
{
    public HarmonicFunction(ScaleDegreeNumber degree, HarmonicChordQuality quality, int inversionNumber)
    {
        if (!Enum.IsDefined(quality)) throw new ArgumentOutOfRangeException(nameof(quality));
        if (inversionNumber < 0) throw new ArgumentOutOfRangeException(nameof(inversionNumber));
        Degree = degree;
        Quality = quality;
        InversionNumber = inversionNumber;
    }

    public ScaleDegreeNumber Degree { get; }
    public HarmonicChordQuality Quality { get; }
    public int InversionNumber { get; }
}

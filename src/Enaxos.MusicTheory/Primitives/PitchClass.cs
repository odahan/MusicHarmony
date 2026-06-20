namespace Enaxos.MusicTheory.Primitives;

public readonly record struct PitchClass
{
    private const int PitchClassCount = 12;

    private PitchClass(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static PitchClass FromChromaticIndex(int value) =>
        new(Normalize(value));

    public int DistanceUpTo(PitchClass target) =>
        Normalize(target.Value - Value);

    internal static PitchClass FromChromaticIndex(long value) =>
        new(Normalize(value));

    private static int Normalize(long value)
    {
        var remainder = value % PitchClassCount;
        return (int)(remainder < 0 ? remainder + PitchClassCount : remainder);
    }

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

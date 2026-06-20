namespace Enaxos.MusicTheory.Primitives;

/// <summary>Represents an octave-independent chromatic position normalized to the range 0 through 11.</summary>
public readonly record struct PitchClass
{
    /// <summary>The number of pitch classes in twelve-tone chromatic arithmetic.</summary>
    private const int PitchClassCount = 12;

    private PitchClass(int value)
    {
        Value = value;
    }

    /// <summary>Gets the normalized chromatic index, where C is 0 and B is 11.</summary>
    public int Value { get; }

    /// <summary>Normalizes any signed chromatic index to a pitch class.</summary>
    /// <param name="value">A possibly negative or octave-displaced chromatic index.</param>
    /// <returns>The corresponding normalized pitch class.</returns>
    public static PitchClass FromChromaticIndex(int value) =>
        new(Normalize(value));

    /// <summary>Gets the ascending modular distance to another pitch class.</summary>
    /// <param name="target">The destination pitch class.</param>
    /// <returns>A distance from 0 through 11 semitones.</returns>
    public int DistanceUpTo(PitchClass target) =>
        Normalize(target.Value - Value);

    /// <summary>Normalizes a wide intermediate without risking integer overflow.</summary>
    internal static PitchClass FromChromaticIndex(long value) =>
        new(Normalize(value));

    /// <summary>Implements mathematical modulo so negative inputs still produce a non-negative pitch class.</summary>
    private static int Normalize(long value)
    {
        var remainder = value % PitchClassCount;
        return (int)(remainder < 0 ? remainder + PitchClassCount : remainder);
    }

    /// <summary>Returns the invariant decimal chromatic index.</summary>
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

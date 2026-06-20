namespace Enaxos.MusicTheory.Tonality;

/// <summary>Represents a validated one-based scale degree in the diatonic range 1 through 7.</summary>
public readonly record struct ScaleDegreeNumber
{
    // The zero-based backing field makes default(ScaleDegreeNumber) represent degree one.
    private readonly int _zeroBasedValue;

    /// <summary>Creates a diatonic scale-degree number.</summary>
    /// <param name="value">A value from 1 through 7.</param>
    public ScaleDegreeNumber(int value)
    {
        if (value is < 1 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        _zeroBasedValue = value - 1;
    }

    /// <summary>Gets the one-based degree value.</summary>
    public int Value => _zeroBasedValue + 1;
}

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Represents a validated one-based scale degree number.</summary>
public readonly record struct ScaleDegreeNumber
{
    private const int MaximumSupportedValue = 12;
    // The zero-based backing field makes default(ScaleDegreeNumber) represent degree one.
    private readonly int _zeroBasedValue;

    /// <summary>Creates a scale-degree number.</summary>
    /// <param name="value">A value from 1 through 12.</param>
    public ScaleDegreeNumber(int value)
    {
        if (value is < 1 or > MaximumSupportedValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        _zeroBasedValue = value - 1;
    }

    /// <summary>Gets the one-based degree value.</summary>
    public int Value => _zeroBasedValue + 1;
}

namespace Enaxos.MusicTheory.Tonality;

public readonly record struct ScaleDegreeNumber
{
    private readonly int _zeroBasedValue;

    public ScaleDegreeNumber(int value)
    {
        if (value is < 1 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        _zeroBasedValue = value - 1;
    }

    public int Value => _zeroBasedValue + 1;
}

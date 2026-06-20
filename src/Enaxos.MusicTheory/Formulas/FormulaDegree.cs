namespace Enaxos.MusicTheory.Formulas;

public readonly record struct FormulaDegree
{
    private readonly int _zeroBasedNumber;

    public FormulaDegree(int number, int alteration = 0)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "A formula degree number must be at least one.");
        }

        _zeroBasedNumber = number - 1;
        Alteration = alteration;
    }

    public int Number => _zeroBasedNumber + 1;

    public int Alteration { get; }
}

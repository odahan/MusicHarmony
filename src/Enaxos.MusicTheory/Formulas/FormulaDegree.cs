namespace Enaxos.MusicTheory.Formulas;

/// <summary>Describes one tonic-relative degree of a scale or chord formula.</summary>
public readonly record struct FormulaDegree
{
    // Storing number - 1 makes the default struct value represent degree one rather than an invalid degree zero.
    private readonly int _zeroBasedNumber;

    /// <summary>Creates a formula degree.</summary>
    /// <param name="number">The one-based diatonic degree number; compound degrees are allowed.</param>
    /// <param name="alteration">The signed semitone alteration from the major/perfect reference degree.</param>
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

    /// <summary>Gets the one-based diatonic degree number.</summary>
    public int Number => _zeroBasedNumber + 1;

    /// <summary>Gets the signed chromatic alteration from the reference degree.</summary>
    public int Alteration { get; }
}

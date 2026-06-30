namespace Enaxos.MusicTheory.Formulas;

/// <summary>Provides chromatic reference distances for tonic-relative formula degrees.</summary>
internal static class FormulaDegreeSemitones
{
    /// <summary>The major/perfect reference offsets for simple degrees one through seven.</summary>
    private static readonly int[] SimpleReferenceSemitones = [0, 2, 4, 5, 7, 9, 11];

    /// <summary>Returns the unaltered major/perfect chromatic distance for a formula degree number.</summary>
    internal static int ReferenceSemitones(int number)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        var zeroBased = number - 1;
        return checked((zeroBased / 7 * 12) + SimpleReferenceSemitones[zeroBased % 7]);
    }

    /// <summary>Returns the chromatic distance produced by a formula degree and its alteration.</summary>
    internal static int Offset(FormulaDegree degree) =>
        checked(ReferenceSemitones(degree.Number) + degree.Alteration);
}

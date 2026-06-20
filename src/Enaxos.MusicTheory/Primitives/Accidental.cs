using System.Globalization;

namespace Enaxos.MusicTheory.Primitives;

/// <summary>
/// Represents a written pitch alteration as an unrestricted signed semitone offset.
/// </summary>
/// <remarks>
/// Equality is based on the semitone count. Values outside the conventional double-flat
/// through double-sharp range are supported because the domain model permits mathematical
/// spellings such as <c>(+3)</c>.
/// </remarks>
public readonly record struct Accidental
{
    private Accidental(int semitones)
    {
        Semitones = semitones;
    }

    /// <summary>Gets the signed chromatic alteration in semitones.</summary>
    public int Semitones { get; }

    /// <summary>Gets the conventional double-flat accidental (-2 semitones).</summary>
    public static Accidental DoubleFlat { get; } = new(-2);

    /// <summary>Gets the conventional flat accidental (-1 semitone).</summary>
    public static Accidental Flat { get; } = new(-1);

    /// <summary>Gets the natural accidental (no chromatic alteration).</summary>
    public static Accidental Natural { get; } = new(0);

    /// <summary>Gets the conventional sharp accidental (+1 semitone).</summary>
    public static Accidental Sharp { get; } = new(1);

    /// <summary>Gets the conventional double-sharp accidental (+2 semitones).</summary>
    public static Accidental DoubleSharp { get; } = new(2);

    /// <summary>Creates an accidental from any signed semitone alteration.</summary>
    /// <param name="semitones">The number of semitones by which the natural letter is altered.</param>
    /// <returns>An accidental carrying the exact supplied alteration.</returns>
    public static Accidental FromSemitones(int semitones) => new(semitones);

    /// <summary>Returns the invariant ASCII representation of the accidental.</summary>
    /// <returns>An empty string for natural, conventional symbols up to double accidentals,
    /// or a parenthesized signed number for larger alterations.</returns>
    public override string ToString() => Semitones switch
    {
        -2 => "bb",
        -1 => "b",
        0 => string.Empty,
        1 => "#",
        2 => "##",
        > 0 => $"(+{Semitones.ToString(CultureInfo.InvariantCulture)})",
        _ => $"({Semitones.ToString(CultureInfo.InvariantCulture)})",
    };
}

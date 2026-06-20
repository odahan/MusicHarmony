using System.Globalization;

namespace Enaxos.MusicTheory.Primitives;

public readonly record struct Accidental
{
    private Accidental(int semitones)
    {
        Semitones = semitones;
    }

    public int Semitones { get; }

    public static Accidental DoubleFlat { get; } = new(-2);

    public static Accidental Flat { get; } = new(-1);

    public static Accidental Natural { get; } = new(0);

    public static Accidental Sharp { get; } = new(1);

    public static Accidental DoubleSharp { get; } = new(2);

    public static Accidental FromSemitones(int semitones) => new(semitones);

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

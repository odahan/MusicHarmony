using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tonality;

public sealed class KeySignature : IEquatable<KeySignature>
{
    private static readonly NoteLetter[] SharpOrder =
        [NoteLetter.F, NoteLetter.C, NoteLetter.G, NoteLetter.D, NoteLetter.A, NoteLetter.E, NoteLetter.B];
    private static readonly NoteLetter[] FlatOrder =
        [NoteLetter.B, NoteLetter.E, NoteLetter.A, NoteLetter.D, NoteLetter.G, NoteLetter.C, NoteLetter.F];

    private readonly ReadOnlyCollection<NoteLetter> _alteredLetters;

    private KeySignature(int fifths)
    {
        Fifths = fifths;
        AccidentalCount = Math.Abs(fifths);
        Accidental = fifths switch
        {
            > 0 => Accidental.Sharp,
            < 0 => Accidental.Flat,
            _ => Accidental.Natural,
        };
        var order = fifths >= 0 ? SharpOrder : FlatOrder;
        _alteredLetters = Array.AsReadOnly(order.Take(AccidentalCount).ToArray());
    }

    public int Fifths { get; }

    public int AccidentalCount { get; }

    public Accidental Accidental { get; }

    public IReadOnlyList<NoteLetter> AlteredLetters => _alteredLetters;

    public static KeySignature FromFifths(int fifths)
    {
        if (fifths is < -7 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(fifths));
        }

        return new KeySignature(fifths);
    }

    public static KeySignature For(MusicalKey key)
    {
        var fifths = key.Mode == KeyMode.Major
            ? MajorFifths(key.Tonic)
            : MinorFifths(key.Tonic);
        return FromFifths(fifths);
    }

    public bool Equals(KeySignature? other) => other is not null && Fifths == other.Fifths;

    public override bool Equals(object? obj) => Equals(obj as KeySignature);

    public override int GetHashCode() => Fifths;

    private static int MajorFifths(SpelledPitch tonic) =>
        (tonic.Letter, tonic.Accidental.Semitones) switch
        {
            (NoteLetter.C, -1) => -7,
            (NoteLetter.G, -1) => -6,
            (NoteLetter.D, -1) => -5,
            (NoteLetter.A, -1) => -4,
            (NoteLetter.E, -1) => -3,
            (NoteLetter.B, -1) => -2,
            (NoteLetter.F, 0) => -1,
            (NoteLetter.C, 0) => 0,
            (NoteLetter.G, 0) => 1,
            (NoteLetter.D, 0) => 2,
            (NoteLetter.A, 0) => 3,
            (NoteLetter.E, 0) => 4,
            (NoteLetter.B, 0) => 5,
            (NoteLetter.F, 1) => 6,
            (NoteLetter.C, 1) => 7,
            _ => throw new ArgumentException("The major key has no conventional -7..+7 signature.", nameof(tonic)),
        };

    private static int MinorFifths(SpelledPitch tonic) =>
        (tonic.Letter, tonic.Accidental.Semitones) switch
        {
            (NoteLetter.A, -1) => -7,
            (NoteLetter.E, -1) => -6,
            (NoteLetter.B, -1) => -5,
            (NoteLetter.F, 0) => -4,
            (NoteLetter.C, 0) => -3,
            (NoteLetter.G, 0) => -2,
            (NoteLetter.D, 0) => -1,
            (NoteLetter.A, 0) => 0,
            (NoteLetter.E, 0) => 1,
            (NoteLetter.B, 0) => 2,
            (NoteLetter.F, 1) => 3,
            (NoteLetter.C, 1) => 4,
            (NoteLetter.G, 1) => 5,
            (NoteLetter.D, 1) => 6,
            (NoteLetter.A, 1) => 7,
            _ => throw new ArgumentException("The minor key has no conventional -7..+7 signature.", nameof(tonic)),
        };
}

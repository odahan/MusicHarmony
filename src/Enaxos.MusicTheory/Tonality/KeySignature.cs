using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Represents a conventional key signature from seven flats through seven sharps.</summary>
public sealed class KeySignature : IEquatable<KeySignature>
{
    /// <summary>The conventional order in which sharps are added.</summary>
    private static readonly NoteLetter[] SharpOrder =
        [NoteLetter.F, NoteLetter.C, NoteLetter.G, NoteLetter.D, NoteLetter.A, NoteLetter.E, NoteLetter.B];
    /// <summary>The conventional order in which flats are added.</summary>
    private static readonly NoteLetter[] FlatOrder =
        [NoteLetter.B, NoteLetter.E, NoteLetter.A, NoteLetter.D, NoteLetter.G, NoteLetter.C, NoteLetter.F];

    /// <summary>The immutable prefix of the appropriate accidental order.</summary>
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

    /// <summary>Gets the signed circle-of-fifths coordinate: negative for flats and positive for sharps.</summary>
    public int Fifths { get; }

    /// <summary>Gets the number of altered letters in the signature.</summary>
    public int AccidentalCount { get; }

    /// <summary>Gets the common accidental applied by the signature, or natural for an empty signature.</summary>
    public Accidental Accidental { get; }

    /// <summary>Gets altered letters in conventional accumulation order.</summary>
    public IReadOnlyList<NoteLetter> AlteredLetters => _alteredLetters;

    /// <summary>Creates a signature from a signed fifths coordinate in the range -7 through +7.</summary>
    public static KeySignature FromFifths(int fifths)
    {
        if (fifths is < -7 or > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(fifths));
        }

        return new KeySignature(fifths);
    }

    /// <summary>Gets the conventional signature for a major or minor key spelling.</summary>
    /// <exception cref="ArgumentException">The spelling has no conventional signature in the supported range.</exception>
    public static KeySignature For(MusicalKey key)
    {
        var fifths = key.Mode == KeyMode.Major
            ? MajorFifths(key.Tonic)
            : MinorFifths(key.Tonic);
        return FromFifths(fifths);
    }

    /// <summary>Compares signatures by their signed fifths coordinate.</summary>
    public bool Equals(KeySignature? other) => other is not null && Fifths == other.Fifths;

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as KeySignature);

    /// <inheritdoc />
    public override int GetHashCode() => Fifths;

    /// <summary>Maps a conventional major tonic spelling to its signed fifths coordinate.</summary>
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

    /// <summary>Maps a conventional minor tonic spelling to its signed fifths coordinate.</summary>
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

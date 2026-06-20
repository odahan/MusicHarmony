using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Tracks a current chord together with its original chord and cumulative chromatic displacement.</summary>
public sealed class DerivedChord : IEquatable<DerivedChord>
{
    internal DerivedChord(Chord original, Chord current, int delta)
    {
        OriginalChord = original;
        CurrentChord = current;
        SemitoneDeltaFromOriginal = delta;
    }

    /// <summary>Gets the immutable origin of the transformation chain.</summary>
    public Chord OriginalChord { get; }

    /// <summary>Gets the chord at the current point in the transformation chain.</summary>
    public Chord CurrentChord { get; }

    /// <summary>Gets the cumulative signed semitone displacement from the original chord.</summary>
    public int SemitoneDeltaFromOriginal { get; }

    /// <summary>Starts a derivation chain with zero displacement.</summary>
    public static DerivedChord From(Chord chord)
    {
        ArgumentNullException.ThrowIfNull(chord);
        return new DerivedChord(chord, chord, 0);
    }

    /// <summary>Returns the next derived chord while preserving the original provenance.</summary>
    public DerivedChord Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        var next = CurrentChord.Transpose(semitones, preference).CurrentChord;
        return new DerivedChord(OriginalChord, next, checked(SemitoneDeltaFromOriginal + semitones));
    }

    /// <summary>Compares the original, current, and cumulative transformation state.</summary>
    public bool Equals(DerivedChord? other) => other is not null &&
        OriginalChord.Equals(other.OriginalChord) && CurrentChord.Equals(other.CurrentChord) &&
        SemitoneDeltaFromOriginal == other.SemitoneDeltaFromOriginal;
    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as DerivedChord);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(OriginalChord, CurrentChord, SemitoneDeltaFromOriginal);
}

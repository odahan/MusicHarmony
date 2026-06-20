using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

public sealed class DerivedChord : IEquatable<DerivedChord>
{
    internal DerivedChord(Chord original, Chord current, int delta)
    {
        OriginalChord = original;
        CurrentChord = current;
        SemitoneDeltaFromOriginal = delta;
    }

    public Chord OriginalChord { get; }
    public Chord CurrentChord { get; }
    public int SemitoneDeltaFromOriginal { get; }

    public static DerivedChord From(Chord chord)
    {
        ArgumentNullException.ThrowIfNull(chord);
        return new DerivedChord(chord, chord, 0);
    }

    public DerivedChord Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        var next = CurrentChord.Transpose(semitones, preference).CurrentChord;
        return new DerivedChord(OriginalChord, next, checked(SemitoneDeltaFromOriginal + semitones));
    }

    public bool Equals(DerivedChord? other) => other is not null &&
        OriginalChord.Equals(other.OriginalChord) && CurrentChord.Equals(other.CurrentChord) &&
        SemitoneDeltaFromOriginal == other.SemitoneDeltaFromOriginal;
    public override bool Equals(object? obj) => Equals(obj as DerivedChord);
    public override int GetHashCode() => HashCode.Combine(OriginalChord, CurrentChord, SemitoneDeltaFromOriginal);
}

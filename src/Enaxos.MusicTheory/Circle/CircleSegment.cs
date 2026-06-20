using System.Collections.ObjectModel;

namespace Enaxos.MusicTheory.Circle;

/// <summary>Represents one of twelve pitch-class positions on the circle of fifths.</summary>
public sealed class CircleSegment : IEquatable<CircleSegment>
{
    /// <summary>All conventional enharmonic key spellings assigned to this segment.</summary>
    private readonly ReadOnlyCollection<CircleKeyPair> _spellings;
    internal CircleSegment(int index, CircleKeyPair[] spellings, CircleKeyPair primary)
    {
        Index = index; _spellings = Array.AsReadOnly(spellings); Primary = primary;
    }
    /// <summary>Gets the zero-based clockwise index, with C major at zero.</summary>
    public int Index { get; }

    /// <summary>Gets every conventional key spelling represented at this pitch-class position.</summary>
    public IReadOnlyList<CircleKeyPair> Spellings => _spellings;

    /// <summary>Gets the spelling selected by the circle's enharmonic preference.</summary>
    public CircleKeyPair Primary { get; }

    /// <summary>Compares the index, available spellings, and selected primary spelling.</summary>
    public bool Equals(CircleSegment? other) => other is not null && Index == other.Index && _spellings.SequenceEqual(other._spellings) && Primary == other.Primary;

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as CircleSegment);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Index, Primary);
}

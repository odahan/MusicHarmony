using System.Collections.ObjectModel;

namespace Enaxos.MusicTheory.Circle;

public sealed class CircleSegment : IEquatable<CircleSegment>
{
    private readonly ReadOnlyCollection<CircleKeyPair> _spellings;
    internal CircleSegment(int index, CircleKeyPair[] spellings, CircleKeyPair primary)
    {
        Index = index; _spellings = Array.AsReadOnly(spellings); Primary = primary;
    }
    public int Index { get; }
    public IReadOnlyList<CircleKeyPair> Spellings => _spellings;
    public CircleKeyPair Primary { get; }
    public bool Equals(CircleSegment? other) => other is not null && Index == other.Index && _spellings.SequenceEqual(other._spellings) && Primary == other.Primary;
    public override bool Equals(object? obj) => Equals(obj as CircleSegment);
    public override int GetHashCode() => HashCode.Combine(Index, Primary);
}

using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Harmony;

public sealed class ChordDefinition : IEquatable<ChordDefinition>
{
    private readonly ReadOnlyCollection<FormulaDegree> _degrees;

    public ChordDefinition(string id, IEnumerable<FormulaDegree> degrees)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(degrees);
        var copy = degrees.ToArray();

        if (copy.Length == 0 || copy[0] != new FormulaDegree(1))
        {
            throw new ArgumentException("A chord definition must begin with natural degree one.", nameof(degrees));
        }

        for (var index = 1; index < copy.Length; index++)
        {
            if (copy[index].Number <= copy[index - 1].Number)
            {
                throw new ArgumentException("Chord degrees must be unique and increasing.", nameof(degrees));
            }
        }

        Id = id;
        _degrees = Array.AsReadOnly(copy);
    }

    public string Id { get; }

    public IReadOnlyList<FormulaDegree> Degrees => _degrees;

    public bool Equals(ChordDefinition? other) => other is not null &&
        string.Equals(Id, other.Id, StringComparison.Ordinal) &&
        _degrees.SequenceEqual(other._degrees);

    public override bool Equals(object? obj) => Equals(obj as ChordDefinition);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id, StringComparer.Ordinal);
        foreach (var degree in _degrees) hash.Add(degree);
        return hash.ToHashCode();
    }
}

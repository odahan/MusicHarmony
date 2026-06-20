using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Defines an immutable root-relative chord formula and stable identifier.</summary>
public sealed class ChordDefinition : IEquatable<ChordDefinition>
{
    /// <summary>The immutable validated formula exposed through <see cref="Degrees"/>.</summary>
    private readonly ReadOnlyCollection<FormulaDegree> _degrees;

    /// <summary>Creates a chord definition.</summary>
    /// <param name="id">A stable culture-independent identifier.</param>
    /// <param name="degrees">Strictly increasing degrees beginning with an unaltered root.</param>
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

    /// <summary>Gets the stable definition identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the immutable ordered chord formula.</summary>
    public IReadOnlyList<FormulaDegree> Degrees => _degrees;

    /// <summary>Compares the identifier and complete degree formula.</summary>
    public bool Equals(ChordDefinition? other) => other is not null &&
        string.Equals(Id, other.Id, StringComparison.Ordinal) &&
        _degrees.SequenceEqual(other._degrees);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ChordDefinition);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id, StringComparer.Ordinal);
        foreach (var degree in _degrees) hash.Add(degree);
        return hash.ToHashCode();
    }
}

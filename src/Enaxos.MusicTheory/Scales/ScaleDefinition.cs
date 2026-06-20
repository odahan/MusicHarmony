using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Defines an immutable tonic-relative scale formula and its stable identifier.</summary>
public sealed class ScaleDefinition : IEquatable<ScaleDefinition>
{
    /// <summary>The private immutable snapshot exposed through <see cref="Degrees"/>.</summary>
    private readonly ReadOnlyCollection<FormulaDegree> _degrees;

    /// <summary>Creates and validates a scale definition.</summary>
    /// <param name="id">A stable ordinal identifier, not a localized display name.</param>
    /// <param name="degrees">Strictly increasing degrees beginning with an unaltered tonic.</param>
    public ScaleDefinition(string id, IEnumerable<FormulaDegree> degrees)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(degrees);

        var copy = degrees.ToArray();
        ValidateDegrees(copy);

        Id = id;
        _degrees = Array.AsReadOnly(copy);
    }

    /// <summary>Gets the stable, culture-independent definition identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the immutable ordered tonic-relative formula.</summary>
    public IReadOnlyList<FormulaDegree> Degrees => _degrees;

    /// <summary>Compares identifiers and every formula degree using value semantics.</summary>
    public bool Equals(ScaleDefinition? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is not null &&
            string.Equals(Id, other.Id, StringComparison.Ordinal) &&
            _degrees.SequenceEqual(other._degrees);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ScaleDefinition);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id, StringComparer.Ordinal);
        foreach (var degree in _degrees)
        {
            hash.Add(degree);
        }

        return hash.ToHashCode();
    }

    /// <summary>Determines whether two definitions have identical value content.</summary>
    public static bool operator ==(ScaleDefinition? left, ScaleDefinition? right) =>
        Equals(left, right);

    /// <summary>Determines whether two definitions differ in identifier or formula.</summary>
    public static bool operator !=(ScaleDefinition? left, ScaleDefinition? right) =>
        !Equals(left, right);

    /// <summary>Enforces the normalized scale-formula invariants required by realization and lookup.</summary>
    private static void ValidateDegrees(IReadOnlyList<FormulaDegree> degrees)
    {
        if (degrees.Count == 0)
        {
            throw new ArgumentException("A scale definition must contain at least one degree.", nameof(degrees));
        }

        if (degrees[0] != new FormulaDegree(1))
        {
            throw new ArgumentException(
                "A scale definition must begin with the unaltered first degree.",
                nameof(degrees));
        }

        for (var index = 1; index < degrees.Count; index++)
        {
            if (degrees[index].Number <= degrees[index - 1].Number)
            {
                throw new ArgumentException(
                    "Scale degree numbers must be unique and strictly increasing.",
                    nameof(degrees));
            }
        }
    }
}

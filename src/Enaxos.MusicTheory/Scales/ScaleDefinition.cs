using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

public sealed class ScaleDefinition : IEquatable<ScaleDefinition>
{
    private readonly ReadOnlyCollection<FormulaDegree> _degrees;

    public ScaleDefinition(string id, IEnumerable<FormulaDegree> degrees)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(degrees);

        var copy = degrees.ToArray();
        ValidateDegrees(copy);

        Id = id;
        _degrees = Array.AsReadOnly(copy);
    }

    public string Id { get; }

    public IReadOnlyList<FormulaDegree> Degrees => _degrees;

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

    public override bool Equals(object? obj) => Equals(obj as ScaleDefinition);

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

    public static bool operator ==(ScaleDefinition? left, ScaleDefinition? right) =>
        Equals(left, right);

    public static bool operator !=(ScaleDefinition? left, ScaleDefinition? right) =>
        !Equals(left, right);

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

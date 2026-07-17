using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Intervals;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Represents an immutable realization of a scale definition on a spelled tonic.</summary>
/// <remarks>
/// For symmetric collections such as octatonic scales, <see cref="Tonic"/>
/// records the construction root or starting pitch chosen by the caller. It
/// does not by itself assert a functional tonal center.
/// </remarks>
public sealed class Scale : IEquatable<Scale>
{
    /// <summary>The immutable realized spellings in formula order.</summary>
    private readonly ReadOnlyCollection<SpelledPitch> _pitches;

    private Scale(
        SpelledPitch tonic,
        ScaleDefinition definition,
        SpelledPitch[] pitches)
    {
        Tonic = tonic;
        Definition = definition;
        _pitches = Array.AsReadOnly(pitches);
    }

    /// <summary>Gets the tonic spelling used to realize the scale.</summary>
    public SpelledPitch Tonic { get; }

    /// <summary>Gets the formula from which the scale was realized.</summary>
    public ScaleDefinition Definition { get; }

    /// <summary>Gets the immutable realized pitch spellings in formula order.</summary>
    public IReadOnlyList<SpelledPitch> Pitches => _pitches;

    /// <summary>Gets a realized pitch by its formula degree number.</summary>
    /// <remarks>
    /// The argument is a musical degree number, not a zero-based collection
    /// index. When a formula contains multiple chromatic variants of the same
    /// degree number, this method returns the first occurrence for backward
    /// compatibility. Use <see cref="Degrees(int)"/> to inspect every
    /// occurrence explicitly.
    /// </remarks>
    public SpelledPitch Degree(int number)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        for (var index = 0; index < Definition.Degrees.Count; index++)
        {
            if (Definition.Degrees[index].Number == number)
            {
                return _pitches[index];
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(number),
            "The requested degree is not present in this scale definition.");
    }

    /// <summary>Gets all realized pitches whose formula degree has the requested number.</summary>
    /// <remarks>The returned collection is immutable and follows formula order.</remarks>
    public IReadOnlyList<SpelledPitch> Degrees(int number)
    {
        if (number < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        var matches = new List<SpelledPitch>();
        for (var index = 0; index < Definition.Degrees.Count; index++)
        {
            if (Definition.Degrees[index].Number == number)
            {
                matches.Add(_pitches[index]);
            }
        }

        if (matches.Count == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "The requested degree is not present in this scale definition.");
        }

        return Array.AsReadOnly(matches.ToArray());
    }

    /// <summary>Realizes every degree of a definition above a tonic while preserving diatonic spelling.</summary>
    public static Scale Create(SpelledPitch tonic, ScaleDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var pitches = new SpelledPitch[definition.Degrees.Count];
        for (var index = 0; index < definition.Degrees.Count; index++)
        {
            var degree = definition.Degrees[index];
            pitches[index] = Transposition.Transpose(tonic, ToInterval(degree));
        }

        return new Scale(tonic, definition, pitches);
    }

    /// <summary>Compares tonic, definition, and realized spellings using value semantics.</summary>
    public bool Equals(Scale? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other is not null &&
            Tonic == other.Tonic &&
            Definition.Equals(other.Definition) &&
            _pitches.SequenceEqual(other._pitches);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Scale);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Tonic);
        hash.Add(Definition);
        foreach (var pitch in _pitches)
        {
            hash.Add(pitch);
        }

        return hash.ToHashCode();
    }

    /// <summary>Determines whether two scales have equal value content.</summary>
    public static bool operator ==(Scale? left, Scale? right) => Equals(left, right);

    /// <summary>Determines whether two scales differ.</summary>
    public static bool operator !=(Scale? left, Scale? right) => !Equals(left, right);

    /// <summary>Converts a tonic-relative degree into the exact interval needed for transposition.</summary>
    private static Interval ToInterval(FormulaDegree degree)
    {
        var simpleNumber = ((degree.Number - 1) % 7) + 1;
        var referenceQuality = simpleNumber is 1 or 4 or 5
            ? new IntervalQuality(IntervalQualityKind.Perfect)
            : new IntervalQuality(IntervalQualityKind.Major);
        var reference = Interval.Create(degree.Number, referenceQuality);
        var semitones = (long)reference.Semitones + degree.Alteration;

        if (semitones is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentException(
                "A formula degree produces an unrepresentable chromatic distance.",
                nameof(degree));
        }

        return Interval.FromDistances(degree.Number, (int)semitones);
    }
}

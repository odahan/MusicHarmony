using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Intervals;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Scales;

public sealed class Scale : IEquatable<Scale>
{
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

    public SpelledPitch Tonic { get; }

    public ScaleDefinition Definition { get; }

    public IReadOnlyList<SpelledPitch> Pitches => _pitches;

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

    public override bool Equals(object? obj) => Equals(obj as Scale);

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

    public static bool operator ==(Scale? left, Scale? right) => Equals(left, right);

    public static bool operator !=(Scale? left, Scale? right) => !Equals(left, right);

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

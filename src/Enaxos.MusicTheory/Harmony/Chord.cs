using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Intervals;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

public sealed class Chord : IEquatable<Chord>
{
    private readonly ReadOnlyCollection<SpelledPitch> _pitches;

    private Chord(SpelledPitch root, ChordDefinition definition, SpelledPitch[] pitches)
    {
        Root = root;
        Definition = definition;
        Symbol = new ChordSymbol(root, definition.Id);
        _pitches = Array.AsReadOnly(pitches);
    }

    public SpelledPitch Root { get; }
    public ChordDefinition Definition { get; }
    public ChordSymbol Symbol { get; }
    public IReadOnlyList<SpelledPitch> Pitches => _pitches;

    public static Chord Create(SpelledPitch root, ChordDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var pitches = definition.Degrees.Select(degree =>
        {
            var simple = ((degree.Number - 1) % 7) + 1;
            var quality = simple is 1 or 4 or 5
                ? new IntervalQuality(IntervalQualityKind.Perfect)
                : new IntervalQuality(IntervalQualityKind.Major);
            var reference = Interval.Create(degree.Number, quality);
            return Transposition.Transpose(root, Interval.FromDistances(degree.Number, checked(reference.Semitones + degree.Alteration)));
        }).ToArray();
        return new Chord(root, definition, pitches);
    }

    public DerivedChord Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        var pitchClass = PitchClass.FromChromaticIndex((long)Root.PitchClass.Value + semitones);
        var root = EnharmonicSpelling.For(pitchClass, preference);
        return new DerivedChord(this, Create(root, Definition), semitones);
    }

    public bool Equals(Chord? other) => other is not null && Root == other.Root && Definition.Equals(other.Definition);
    public override bool Equals(object? obj) => Equals(obj as Chord);
    public override int GetHashCode() => HashCode.Combine(Root, Definition);
}

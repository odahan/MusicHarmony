using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Intervals;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Represents an immutable root-position pitch spelling of a chord definition.</summary>
public sealed class Chord : IEquatable<Chord>
{
    /// <summary>The immutable realized chord tones in formula order.</summary>
    private readonly ReadOnlyCollection<SpelledPitch> _pitches;

    private Chord(SpelledPitch root, ChordDefinition definition, SpelledPitch[] pitches)
    {
        Root = root;
        Definition = definition;
        Symbol = new ChordSymbol(root, definition.Id);
        _pitches = Array.AsReadOnly(pitches);
    }

    /// <summary>Gets the spelled chord root.</summary>
    public SpelledPitch Root { get; }

    /// <summary>Gets the formula used to realize the chord.</summary>
    public ChordDefinition Definition { get; }

    /// <summary>Gets the normalized lead-sheet symbol.</summary>
    public ChordSymbol Symbol { get; }

    /// <summary>Gets immutable chord-tone spellings in formula order.</summary>
    public IReadOnlyList<SpelledPitch> Pitches => _pitches;

    /// <summary>Realizes a chord formula on a spelled root.</summary>
    /// <remarks>Each target letter is determined diatonically before its accidental is solved chromatically.</remarks>
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

    /// <summary>Chromatically transposes the chord and records its derivation from this chord.</summary>
    /// <param name="semitones">The signed chromatic displacement.</param>
    /// <param name="preference">The spelling policy for the new root.</param>
    public DerivedChord Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        var pitchClass = PitchClass.FromChromaticIndex((long)Root.PitchClass.Value + semitones);
        var root = EnharmonicSpelling.For(pitchClass, preference);
        return new DerivedChord(this, Create(root, Definition), semitones);
    }

    /// <summary>Compares the written root and definition; realized pitches follow deterministically.</summary>
    public bool Equals(Chord? other) => other is not null && Root == other.Root && Definition.Equals(other.Definition);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Chord);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Root, Definition);
}

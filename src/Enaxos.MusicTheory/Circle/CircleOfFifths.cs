using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Circle;

public sealed class CircleOfFifths
{
    private readonly ReadOnlyCollection<CircleSegment> _segments;
    private CircleOfFifths(CircleSegment[] segments) => _segments = Array.AsReadOnly(segments);

    public static CircleOfFifths Standard { get; } = Create();
    public IReadOnlyList<CircleSegment> Segments => _segments;
    public CircleSegment this[int index] => _segments[Modulo12(index)];

    public static CircleOfFifths Create(EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        if (!Enum.IsDefined(preference)) throw new ArgumentOutOfRangeException(nameof(preference));
        var data = new (SpelledPitch Major, int Fifths)[][]
        {
            [(Pitch(NoteLetter.C), 0)], [(Pitch(NoteLetter.G), 1)],
            [(Pitch(NoteLetter.D), 2)], [(Pitch(NoteLetter.A), 3)],
            [(Pitch(NoteLetter.E), 4)],
            [(Pitch(NoteLetter.B), 5), (Pitch(NoteLetter.C, -1), -7)],
            [(Pitch(NoteLetter.F, 1), 6), (Pitch(NoteLetter.G, -1), -6)],
            [(Pitch(NoteLetter.C, 1), 7), (Pitch(NoteLetter.D, -1), -5)],
            [(Pitch(NoteLetter.A, -1), -4)], [(Pitch(NoteLetter.E, -1), -3)],
            [(Pitch(NoteLetter.B, -1), -2)], [(Pitch(NoteLetter.F), -1)],
        };
        var segments = new CircleSegment[12];
        for (var index = 0; index < data.Length; index++)
        {
            var spellings = data[index].Select(item =>
            {
                var major = MusicalKey.Major(item.Major);
                return new CircleKeyPair(major, KeyRelationships.RelativeOf(major), KeySignature.FromFifths(item.Fifths));
            }).ToArray();
            var primary = preference switch
            {
                EnharmonicPreference.PreferFlats => spellings.FirstOrDefault(item => item.Signature.Fifths < 0) ?? spellings[0],
                EnharmonicPreference.PreferSharps => spellings.FirstOrDefault(item => item.Signature.Fifths >= 0) ?? spellings[0],
                _ => spellings.OrderBy(item => item.Signature.AccidentalCount).ThenByDescending(item => item.Signature.Fifths).First(),
            };
            segments[index] = new CircleSegment(index, spellings, primary);
        }
        return new CircleOfFifths(segments);
    }

    public CircleSegment Find(MusicalKey key) => TryFind(key, out var segment)
        ? segment! : throw new KeyNotFoundException("The key is not represented by the conventional circle.");

    public bool TryFind(MusicalKey key, out CircleSegment? segment)
    {
        segment = _segments.FirstOrDefault(item => item.Spellings.Any(pair => pair.Major == key || pair.RelativeMinor == key));
        return segment is not null;
    }

    public CircleSegment Move(CircleSegment from, CircleDirection direction, int steps = 1)
    {
        ArgumentNullException.ThrowIfNull(from);
        if (!Enum.IsDefined(direction)) throw new ArgumentOutOfRangeException(nameof(direction));
        if (steps < 0) throw new ArgumentOutOfRangeException(nameof(steps));
        return this[from.Index + (direction == CircleDirection.Clockwise ? steps : -steps)];
    }

    public CircleNeighbors GetNeighbors(CircleSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        return new CircleNeighbors(this[segment.Index - 1], this[segment.Index], this[segment.Index + 1]);
    }

    public CircleDistance GetDistance(CircleSegment from, CircleSegment to)
    {
        ArgumentNullException.ThrowIfNull(from); ArgumentNullException.ThrowIfNull(to);
        var clockwise = Modulo12(to.Index - from.Index);
        return new CircleDistance(clockwise, Modulo12(from.Index - to.Index));
    }

    private static int Modulo12(int value) { var result = value % 12; return result < 0 ? result + 12 : result; }

    private static SpelledPitch Pitch(NoteLetter letter, int accidental = 0) =>
        new(letter, Accidental.FromSemitones(accidental));
}

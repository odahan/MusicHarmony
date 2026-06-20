using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

public sealed class ChordRealization : IEquatable<ChordRealization>
{
    private readonly ReadOnlyCollection<Note> _notes;

    private ChordRealization(DerivedChord derivation, Note[] notes, int inversionNumber)
    {
        Derivation = derivation;
        _notes = Array.AsReadOnly(notes);
        InversionNumber = inversionNumber;
    }

    public DerivedChord Derivation { get; }
    public Chord OriginalChord => Derivation.OriginalChord;
    public Chord CurrentChord => Derivation.CurrentChord;
    public IReadOnlyList<Note> Notes => _notes;
    public int SemitoneDeltaFromOriginal => Derivation.SemitoneDeltaFromOriginal;
    public int InversionNumber { get; }
    public Note Bass => _notes[0];

    public static ChordRealization CreateRootPosition(Chord chord, int bassOctave) =>
        CreateRootPosition(DerivedChord.From(chord), bassOctave);

    public static ChordRealization CreateRootPosition(DerivedChord chord, int bassOctave)
    {
        ArgumentNullException.ThrowIfNull(chord);
        var notes = ClosePosition(chord.CurrentChord, bassOctave);
        return new ChordRealization(chord, notes, 0);
    }

    public static ChordRealization Create(Chord chord, IEnumerable<Note> notes, bool validate = true)
    {
        ArgumentNullException.ThrowIfNull(chord);
        ArgumentNullException.ThrowIfNull(notes);
        var copy = notes.ToArray();
        if (copy.Length == 0) throw new ArgumentException("A realization requires at least one note.", nameof(notes));
        if (!copy.Zip(copy.Skip(1)).All(pair => pair.First.AbsoluteSemitone <= pair.Second.AbsoluteSemitone))
            throw new ArgumentException("Realization notes must be ordered from bass to treble.", nameof(notes));

        if (validate && copy.Any(note => !chord.Pitches.Any(pitch => pitch.PitchClass == note.Pitch.PitchClass)))
            throw new ArgumentException("A realization contains a pitch outside the chord.", nameof(notes));

        var inversion = FindDegree(chord, copy[0].Pitch);
        if (validate && inversion < 0) throw new ArgumentException("The bass is not a chord tone.", nameof(notes));
        return new ChordRealization(DerivedChord.From(chord), copy, Math.Max(0, inversion));
    }

    public ChordRealization Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        var next = Derivation.Transpose(semitones, preference);
        var notes = new Note[_notes.Count];
        for (var index = 0; index < notes.Length; index++)
        {
            var degree = FindDegree(CurrentChord, _notes[index].Pitch);
            if (degree < 0) degree = 0;
            var pitch = next.CurrentChord.Pitches[degree];
            var targetAbsolute = checked(_notes[index].AbsoluteSemitone + semitones);
            var offset = targetAbsolute - pitch.Accidental.Semitones - NaturalSemitone(pitch.Letter);
            if (offset % 12 != 0) throw new InvalidOperationException("Transposed spelling is inconsistent.");
            notes[index] = new Note(pitch, offset / 12);
        }

        return new ChordRealization(next, notes, InversionNumber);
    }

    public ChordRealization Invert(int inversionNumber)
    {
        var toneCount = CurrentChord.Pitches.Count;
        if (inversionNumber < 0 || inversionNumber >= toneCount)
            throw new ArgumentOutOfRangeException(nameof(inversionNumber));

        var notes = _notes.ToList();
        for (var index = 0; index < InversionNumber && notes.Count > 1; index++)
        {
            var note = notes[^1];
            notes.RemoveAt(notes.Count - 1);
            do note = new Note(note.Pitch, note.Octave - 1); while (notes.Count > 0 && note.AbsoluteSemitone > notes[0].AbsoluteSemitone);
            notes.Insert(0, note);
        }

        for (var index = 0; index < inversionNumber && notes.Count > 1; index++)
        {
            var note = notes[0];
            notes.RemoveAt(0);
            do note = new Note(note.Pitch, note.Octave + 1); while (note.AbsoluteSemitone <= notes[^1].AbsoluteSemitone);
            notes.Add(note);
        }

        return new ChordRealization(Derivation, notes.ToArray(), inversionNumber);
    }

    public bool Equals(ChordRealization? other) => other is not null &&
        Derivation.Equals(other.Derivation) && InversionNumber == other.InversionNumber && _notes.SequenceEqual(other._notes);
    public override bool Equals(object? obj) => Equals(obj as ChordRealization);
    public override int GetHashCode()
    {
        var hash = new HashCode(); hash.Add(Derivation); hash.Add(InversionNumber);
        foreach (var note in _notes) hash.Add(note); return hash.ToHashCode();
    }

    private static Note[] ClosePosition(Chord chord, int bassOctave)
    {
        var result = new Note[chord.Pitches.Count];
        result[0] = new Note(chord.Pitches[0], bassOctave);
        for (var index = 1; index < result.Length; index++)
        {
            var octave = bassOctave;
            var note = new Note(chord.Pitches[index], octave);
            while (note.AbsoluteSemitone <= result[index - 1].AbsoluteSemitone)
                note = new Note(chord.Pitches[index], ++octave);
            result[index] = note;
        }
        return result;
    }

    private static int FindDegree(Chord chord, SpelledPitch pitch)
    {
        for (var index = 0; index < chord.Pitches.Count; index++)
            if (chord.Pitches[index].PitchClass == pitch.PitchClass) return index;
        return -1;
    }

    private static int NaturalSemitone(NoteLetter letter) => letter switch
    {
        NoteLetter.C => 0,
        NoteLetter.D => 2,
        NoteLetter.E => 4,
        NoteLetter.F => 5,
        NoteLetter.G => 7,
        NoteLetter.A => 9,
        NoteLetter.B => 11,
        _ => throw new ArgumentOutOfRangeException(nameof(letter)),
    };
}

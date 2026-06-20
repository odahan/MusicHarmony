using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Places a derived chord into ordered absolute notes and records its inversion.</summary>
public sealed class ChordRealization : IEquatable<ChordRealization>
{
    /// <summary>The immutable notes ordered from bass to treble.</summary>
    private readonly ReadOnlyCollection<Note> _notes;

    private ChordRealization(DerivedChord derivation, Note[] notes, int inversionNumber)
    {
        Derivation = derivation;
        _notes = Array.AsReadOnly(notes);
        InversionNumber = inversionNumber;
    }

    /// <summary>Gets the chord transformation provenance.</summary>
    public DerivedChord Derivation { get; }

    /// <summary>Gets the chord at the start of the transformation chain.</summary>
    public Chord OriginalChord => Derivation.OriginalChord;

    /// <summary>Gets the chord represented by the current notes.</summary>
    public Chord CurrentChord => Derivation.CurrentChord;

    /// <summary>Gets immutable notes ordered from bass to treble.</summary>
    public IReadOnlyList<Note> Notes => _notes;

    /// <summary>Gets the cumulative chromatic displacement from the original chord.</summary>
    public int SemitoneDeltaFromOriginal => Derivation.SemitoneDeltaFromOriginal;

    /// <summary>Gets the zero-based chord-tone index placed in the bass.</summary>
    public int InversionNumber { get; }

    /// <summary>Gets the lowest note of the realization.</summary>
    public Note Bass => _notes[0];

    /// <summary>Creates a close root-position realization of a chord at a specified bass octave.</summary>
    public static ChordRealization CreateRootPosition(Chord chord, int bassOctave) =>
        CreateRootPosition(DerivedChord.From(chord), bassOctave);

    /// <summary>Creates a close root-position realization while retaining derivation provenance.</summary>
    public static ChordRealization CreateRootPosition(DerivedChord chord, int bassOctave)
    {
        ArgumentNullException.ThrowIfNull(chord);
        var notes = ClosePosition(chord.CurrentChord, bassOctave);
        return new ChordRealization(chord, notes, 0);
    }

    /// <summary>Creates a realization from explicitly voiced notes ordered bass to treble.</summary>
    /// <param name="chord">The chord whose tones the realization represents.</param>
    /// <param name="notes">The absolute notes in non-decreasing bass-to-treble order.</param>
    /// <param name="validate">When true, rejects non-chord tones and a bass outside the chord.</param>
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

    /// <summary>Transposes every voice by an exact chromatic displacement and respells it from the derived chord.</summary>
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

    /// <summary>Moves voices by octaves to produce the requested inversion without changing pitch classes.</summary>
    public ChordRealization Invert(int inversionNumber)
    {
        var toneCount = CurrentChord.Pitches.Count;
        if (inversionNumber < 0 || inversionNumber >= toneCount)
            throw new ArgumentOutOfRangeException(nameof(inversionNumber));

        var notes = _notes.ToList();
        // First undo the current inversion back to root-position ordering by moving
        // the current upper voices below the bass.
        for (var index = 0; index < InversionNumber && notes.Count > 1; index++)
        {
            var note = notes[^1];
            notes.RemoveAt(notes.Count - 1);
            do note = new Note(note.Pitch, note.Octave - 1); while (notes.Count > 0 && note.AbsoluteSemitone > notes[0].AbsoluteSemitone);
            notes.Insert(0, note);
        }

        // Then build the requested inversion by moving successive bass voices above
        // the current treble, preserving a strictly ascending realization.
        for (var index = 0; index < inversionNumber && notes.Count > 1; index++)
        {
            var note = notes[0];
            notes.RemoveAt(0);
            do note = new Note(note.Pitch, note.Octave + 1); while (note.AbsoluteSemitone <= notes[^1].AbsoluteSemitone);
            notes.Add(note);
        }

        return new ChordRealization(Derivation, notes.ToArray(), inversionNumber);
    }

    /// <summary>Compares derivation, inversion, and the complete ordered voicing.</summary>
    public bool Equals(ChordRealization? other) => other is not null &&
        Derivation.Equals(other.Derivation) && InversionNumber == other.InversionNumber && _notes.SequenceEqual(other._notes);
    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as ChordRealization);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode(); hash.Add(Derivation); hash.Add(InversionNumber);
        foreach (var note in _notes) hash.Add(note); return hash.ToHashCode();
    }

    /// <summary>Stacks each formula tone in the nearest octave strictly above the previous tone.</summary>
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

    /// <summary>Finds a chord-tone index by enharmonic pitch class, or -1 for a non-chord tone.</summary>
    private static int FindDegree(Chord chord, SpelledPitch pitch)
    {
        for (var index = 0; index < chord.Pitches.Count; index++)
            if (chord.Pitches[index].PitchClass == pitch.PitchClass) return index;
        return -1;
    }

    /// <summary>Returns a natural letter offset used to recover the octave after chromatic transposition.</summary>
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

using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Midi;

/// <summary>Converts between domain notes and MIDI 1.0 note numbers using the explicit C4 = 60 convention.</summary>
public static class MidiNote
{
    /// <summary>Converts a representable note to a MIDI number from 0 through 127.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The note lies outside the MIDI range.</exception>
    public static int ToNumber(Note note)
    {
        if (!TryToNumber(note, out var number)) throw new ArgumentOutOfRangeException(nameof(note), "The note lies outside MIDI 0..127.");
        return number;
    }

    /// <summary>Attempts to convert a note to a MIDI number without throwing for range failure.</summary>
    public static bool TryToNumber(Note note, out int number)
    {
        // Domain absolute semitone zero is C0, while MIDI note zero is C-1.
        var candidate = (long)note.AbsoluteSemitone + 12;
        if (candidate is < 0 or > 127) { number = default; return false; }
        number = (int)candidate; return true;
    }

    /// <summary>Converts a MIDI number to a conventionally spelled domain note.</summary>
    /// <param name="number">A MIDI note number from 0 through 127.</param>
    /// <param name="preference">The spelling policy for chromatic pitch classes.</param>
    public static Note FromNumber(int number, EnharmonicPreference preference = EnharmonicPreference.PreferSharps)
    {
        if (number is < 0 or > 127) throw new ArgumentOutOfRangeException(nameof(number));
        var absolute = number - 12;
        var pitch = EnharmonicSpelling.For(PitchClass.FromChromaticIndex(absolute), preference);
        // Recover the spelling-aware octave after choosing an enharmonic pitch. Subtracting
        // both the natural letter offset and accidental keeps B#/Cb-style boundaries correct.
        var natural = pitch.Letter switch { NoteLetter.C => 0, NoteLetter.D => 2, NoteLetter.E => 4, NoteLetter.F => 5, NoteLetter.G => 7, NoteLetter.A => 9, NoteLetter.B => 11, _ => throw new InvalidOperationException() };
        return new Note(pitch, (absolute - natural - pitch.Accidental.Semitones) / 12);
    }
}

using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Midi;

public static class MidiNote
{
    public static int ToNumber(Note note)
    {
        if (!TryToNumber(note, out var number)) throw new ArgumentOutOfRangeException(nameof(note), "The note lies outside MIDI 0..127.");
        return number;
    }

    public static bool TryToNumber(Note note, out int number)
    {
        var candidate = (long)note.AbsoluteSemitone + 12;
        if (candidate is < 0 or > 127) { number = default; return false; }
        number = (int)candidate; return true;
    }

    public static Note FromNumber(int number, EnharmonicPreference preference = EnharmonicPreference.PreferSharps)
    {
        if (number is < 0 or > 127) throw new ArgumentOutOfRangeException(nameof(number));
        var absolute = number - 12;
        var pitch = EnharmonicSpelling.For(PitchClass.FromChromaticIndex(absolute), preference);
        var natural = pitch.Letter switch { NoteLetter.C => 0, NoteLetter.D => 2, NoteLetter.E => 4, NoteLetter.F => 5, NoteLetter.G => 7, NoteLetter.A => 9, NoteLetter.B => 11, _ => throw new InvalidOperationException() };
        return new Note(pitch, (absolute - natural - pitch.Accidental.Semitones) / 12);
    }
}

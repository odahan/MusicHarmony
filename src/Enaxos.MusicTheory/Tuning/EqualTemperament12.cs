using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tuning;

/// <summary>Implements twelve-tone equal temperament relative to a configurable reference pitch.</summary>
public sealed class EqualTemperament12 : ITuningSystem
{
    /// <summary>Creates a twelve-tone equal-temperament tuning.</summary>
    /// <param name="referenceNote">The reference note, or null for A4.</param>
    /// <param name="referenceFrequency">The positive reference frequency in hertz, defaulting to 440.</param>
    public EqualTemperament12(Note? referenceNote = null, double referenceFrequency = 440.0)
    {
        if (!double.IsFinite(referenceFrequency) || referenceFrequency <= 0)
            throw new ArgumentOutOfRangeException(nameof(referenceFrequency));
        ReferenceNote = referenceNote ?? Note.Parse("A4");
        ReferenceFrequency = referenceFrequency;
    }
    /// <summary>Gets the note from which all frequency ratios are measured.</summary>
    public Note ReferenceNote { get; }

    /// <summary>Gets the frequency assigned to <see cref="ReferenceNote"/>.</summary>
    public double ReferenceFrequency { get; }

    /// <summary>Gets a note's frequency using a ratio of 2^(semitone distance / 12).</summary>
    public double GetFrequency(Note note) => ReferenceFrequency * Math.Pow(2d, (note.AbsoluteSemitone - ReferenceNote.AbsoluteSemitone) / 12d);

    /// <summary>Gets the nearest note to a measured frequency in this tuning.</summary>
    /// <param name="frequency">A positive measured frequency in hertz.</param>
    /// <param name="preference">The enharmonic spelling policy for the returned note.</param>
    /// <returns>The nearest note, its tuned frequency, and the measured deviation in cents.</returns>
    public FrequencyMatch GetNearestNote(double frequency, EnharmonicPreference preference = EnharmonicPreference.PreferSharps)
    {
        if (!double.IsFinite(frequency) || frequency <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(frequency));
        }

        var exactSemitoneDistance = 12d * Math.Log2(frequency / ReferenceFrequency);
        var absoluteSemitone = ReferenceNote.AbsoluteSemitone +
            (int)Math.Round(exactSemitoneDistance, MidpointRounding.AwayFromZero);
        var note = CreateNoteFromAbsoluteSemitone(absoluteSemitone, preference);
        var noteFrequency = GetFrequency(note);
        var centsDeviation = 1200d * Math.Log2(frequency / noteFrequency);
        return new FrequencyMatch(note, noteFrequency, centsDeviation);
    }

    private static Note CreateNoteFromAbsoluteSemitone(int absoluteSemitone, EnharmonicPreference preference)
    {
        var pitch = EnharmonicSpelling.For(PitchClass.FromChromaticIndex(absoluteSemitone), preference);
        var natural = NoteLetterInfo.NaturalSemitone(pitch.Letter);
        return new Note(pitch, (absoluteSemitone - natural - pitch.Accidental.Semitones) / 12);
    }
}

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
}

using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tuning;

public sealed class EqualTemperament12 : ITuningSystem
{
    public EqualTemperament12(Note? referenceNote = null, double referenceFrequency = 440.0)
    {
        if (!double.IsFinite(referenceFrequency) || referenceFrequency <= 0)
            throw new ArgumentOutOfRangeException(nameof(referenceFrequency));
        ReferenceNote = referenceNote ?? Note.Parse("A4");
        ReferenceFrequency = referenceFrequency;
    }
    public Note ReferenceNote { get; }
    public double ReferenceFrequency { get; }
    public double GetFrequency(Note note) => ReferenceFrequency * Math.Pow(2d, (note.AbsoluteSemitone - ReferenceNote.AbsoluteSemitone) / 12d);
}

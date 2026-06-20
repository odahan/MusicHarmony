using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tuning;

/// <summary>Defines a strategy that maps absolute musical notes to frequencies in hertz.</summary>
public interface ITuningSystem
{
    /// <summary>Gets the frequency assigned to a note.</summary>
    /// <param name="note">The absolute spelled note to evaluate.</param>
    /// <returns>A positive frequency in hertz.</returns>
    double GetFrequency(Note note);
}

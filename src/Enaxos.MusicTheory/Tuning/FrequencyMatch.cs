using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tuning;

/// <summary>Represents the nearest equal-temperament note for a measured frequency.</summary>
/// <param name="Note">The nearest note in the selected spelling preference.</param>
/// <param name="Frequency">The exact frequency assigned to <paramref name="Note"/> by the tuning system.</param>
/// <param name="CentsDeviation">The deviation of the measured frequency from <paramref name="Frequency"/>, in cents.</param>
public sealed record FrequencyMatch(Note Note, double Frequency, double CentsDeviation);

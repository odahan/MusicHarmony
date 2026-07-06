namespace Enaxos.MusicTheory.Tuning;

/// <summary>Groups instrument ranges by musical family.</summary>
/// <param name="GroupName">The family display name.</param>
/// <param name="Ranges">The ranges in this family.</param>
public sealed record InstrumentFrequencyRangeGroup(
    string GroupName,
    IReadOnlyList<InstrumentFrequencyRange> Ranges);

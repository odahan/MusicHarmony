using System.Collections.ObjectModel;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Records both the result and the provenance of a pentatonic derivation.</summary>
public sealed class PentatonicDerivation
{
    /// <summary>The immutable one-based source positions retained in the result.</summary>
    private readonly ReadOnlyCollection<int> _selectedDegrees;

    internal PentatonicDerivation(
        Scale source,
        Scale result,
        PentatonicDerivationStrategy strategy,
        int[] selectedDegrees)
    {
        Source = source;
        Result = result;
        Strategy = strategy;
        _selectedDegrees = Array.AsReadOnly(selectedDegrees);
    }

    /// <summary>Gets the original scale from which pitches were selected.</summary>
    public Scale Source { get; }

    /// <summary>Gets the derived five-note scale.</summary>
    public Scale Result { get; }

    /// <summary>Gets the strategy that produced the result.</summary>
    public PentatonicDerivationStrategy Strategy { get; }

    /// <summary>Gets the one-based positions retained from the source scale.</summary>
    public IReadOnlyList<int> SelectedDegrees => _selectedDegrees;
}

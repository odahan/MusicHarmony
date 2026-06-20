using System.Collections.ObjectModel;

namespace Enaxos.MusicTheory.Scales;

public sealed class PentatonicDerivation
{
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

    public Scale Source { get; }

    public Scale Result { get; }

    public PentatonicDerivationStrategy Strategy { get; }

    public IReadOnlyList<int> SelectedDegrees => _selectedDegrees;
}

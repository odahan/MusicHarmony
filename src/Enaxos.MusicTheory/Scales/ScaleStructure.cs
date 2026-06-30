using System.Collections.ObjectModel;
using System.Globalization;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Represents the cyclic semitone pattern of a scale across one octave.</summary>
public sealed class ScaleStructure
{
    /// <summary>The immutable semitone distances exposed through <see cref="SemitoneSteps"/>.</summary>
    private readonly ReadOnlyCollection<int> _semitoneSteps;
    /// <summary>The immutable symbolic distances exposed through <see cref="StepSymbols"/>.</summary>
    private readonly ReadOnlyCollection<string> _stepSymbols;

    internal ScaleStructure(IEnumerable<int> semitoneSteps)
    {
        ArgumentNullException.ThrowIfNull(semitoneSteps);

        var steps = semitoneSteps.ToArray();
        if (steps.Length == 0 || steps.Any(step => step <= 0))
        {
            throw new ArgumentException("A scale structure must contain positive semitone steps.", nameof(semitoneSteps));
        }

        var symbols = steps.Select(SymbolFor).ToArray();
        _semitoneSteps = Array.AsReadOnly(steps);
        _stepSymbols = Array.AsReadOnly(symbols);
        CompactPattern = string.Concat(symbols);
    }

    /// <summary>Gets the ascending semitone distance from each scale tone to the next, including octave closure.</summary>
    public IReadOnlyList<int> SemitoneSteps => _semitoneSteps;

    /// <summary>Gets compact symbols for each step, using W for two semitones and H for one semitone.</summary>
    public IReadOnlyList<string> StepSymbols => _stepSymbols;

    /// <summary>Gets the concatenated step-symbol pattern, such as WWHWWWH for a major scale.</summary>
    public string CompactPattern { get; }

    /// <summary>Returns the compact pattern for diagnostic display.</summary>
    public override string ToString() => CompactPattern;

    /// <summary>Maps common whole and half steps to conventional symbols and leaves wider gaps numeric.</summary>
    private static string SymbolFor(int semitones) => semitones switch
    {
        1 => "H",
        2 => "W",
        _ => semitones.ToString(CultureInfo.InvariantCulture),
    };
}

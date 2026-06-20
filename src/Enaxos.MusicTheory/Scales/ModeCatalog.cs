using System.Collections.ObjectModel;
using System.Globalization;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

public sealed class ModeCatalog
{
    private static readonly int[] MajorOffsets = [0, 2, 4, 5, 7, 9, 11];
    private static readonly int[] HarmonicMinorOffsets = [0, 2, 3, 5, 7, 8, 11];
    private static readonly int[] MelodicMinorOffsets = [0, 2, 3, 5, 7, 9, 11];

    private readonly ReadOnlyCollection<ScaleDefinition> _majorModes;
    private readonly ReadOnlyCollection<ScaleDefinition> _naturalMinorModes;
    private readonly ReadOnlyCollection<ScaleDefinition> _harmonicMinorModes;
    private readonly ReadOnlyCollection<ScaleDefinition> _melodicMinorModes;
    private readonly ReadOnlyCollection<ScaleDefinition> _all;

    private ModeCatalog()
    {
        var major = new[]
        {
            StandardScales.Ionian,
            StandardScales.Dorian,
            StandardScales.Phrygian,
            StandardScales.Lydian,
            StandardScales.Mixolydian,
            StandardScales.Aeolian,
            StandardScales.Locrian,
        };
        var naturalMinor = new[]
        {
            StandardScales.Aeolian,
            StandardScales.Locrian,
            StandardScales.Ionian,
            StandardScales.Dorian,
            StandardScales.Phrygian,
            StandardScales.Lydian,
            StandardScales.Mixolydian,
        };
        var harmonicMinor = CreateRotations("mode.harmonic-minor", HarmonicMinorOffsets);
        var melodicMinor = CreateRotations("mode.melodic-minor", MelodicMinorOffsets);

        _majorModes = Array.AsReadOnly(major);
        _naturalMinorModes = Array.AsReadOnly(naturalMinor);
        _harmonicMinorModes = Array.AsReadOnly(harmonicMinor);
        _melodicMinorModes = Array.AsReadOnly(melodicMinor);
        _all = Array.AsReadOnly(major.Concat(harmonicMinor).Concat(melodicMinor).ToArray());
    }

    public static ModeCatalog Standard { get; } = new();

    public IReadOnlyList<ScaleDefinition> MajorModes => _majorModes;

    public IReadOnlyList<ScaleDefinition> NaturalMinorModes => _naturalMinorModes;

    public IReadOnlyList<ScaleDefinition> HarmonicMinorModes => _harmonicMinorModes;

    public IReadOnlyList<ScaleDefinition> MelodicMinorModes => _melodicMinorModes;

    public IReadOnlyList<ScaleDefinition> All => _all;

    private static ScaleDefinition[] CreateRotations(string idPrefix, IReadOnlyList<int> offsets)
    {
        var definitions = new ScaleDefinition[offsets.Count];

        for (var rotation = 0; rotation < offsets.Count; rotation++)
        {
            var degrees = new FormulaDegree[offsets.Count];
            var rootOffset = offsets[rotation];

            for (var index = 0; index < offsets.Count; index++)
            {
                var sourceIndex = (rotation + index) % offsets.Count;
                var rotatedOffset = offsets[sourceIndex] - rootOffset;
                if (rotatedOffset < 0)
                {
                    rotatedOffset += 12;
                }

                degrees[index] = new FormulaDegree(
                    index + 1,
                    rotatedOffset - MajorOffsets[index]);
            }

            var id = string.Concat(
                idPrefix,
                ".",
                (rotation + 1).ToString(CultureInfo.InvariantCulture));
            definitions[rotation] = new ScaleDefinition(id, degrees);
        }

        return definitions;
    }
}

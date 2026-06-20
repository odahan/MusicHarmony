using System.Collections.ObjectModel;
using System.Globalization;
using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Provides immutable catalogs of rotations for the supported seven-note parent collections.</summary>
public sealed class ModeCatalog
{
    /// <summary>Chromatic offsets of the major reference used to express every rotation as altered degrees.</summary>
    private static readonly int[] MajorOffsets = [0, 2, 4, 5, 7, 9, 11];
    /// <summary>Chromatic offsets of the harmonic-minor parent collection.</summary>
    private static readonly int[] HarmonicMinorOffsets = [0, 2, 3, 5, 7, 8, 11];
    /// <summary>Chromatic offsets of the ascending melodic-minor parent collection.</summary>
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

    /// <summary>Gets the shared catalog of standard modes.</summary>
    public static ModeCatalog Standard { get; } = new();

    /// <summary>Gets the seven rotations of the major collection in modal order.</summary>
    public IReadOnlyList<ScaleDefinition> MajorModes => _majorModes;

    /// <summary>Gets the natural-minor rotations starting from Aeolian.</summary>
    public IReadOnlyList<ScaleDefinition> NaturalMinorModes => _naturalMinorModes;

    /// <summary>Gets the seven generated rotations of harmonic minor.</summary>
    public IReadOnlyList<ScaleDefinition> HarmonicMinorModes => _harmonicMinorModes;

    /// <summary>Gets the seven generated rotations of ascending melodic minor.</summary>
    public IReadOnlyList<ScaleDefinition> MelodicMinorModes => _melodicMinorModes;

    /// <summary>Gets each distinct catalog definition used by recognition.</summary>
    public IReadOnlyList<ScaleDefinition> All => _all;

    /// <summary>Rotates a parent pitch-class collection and converts each rotation to altered major degrees.</summary>
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

using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Calculates octave-normalized step structures for standard definitions and realized scales.</summary>
public static class ScaleStructures
{
    /// <summary>The supported standard definitions addressable by stable public identifiers.</summary>
    private static readonly IReadOnlyList<ScaleDefinition> StandardDefinitions =
        new[]
        {
            StandardScales.Major,
            StandardScales.NaturalMinor,
            StandardScales.HarmonicMinor,
            StandardScales.MelodicMinorAscending,
            StandardScales.MajorPentatonic,
            StandardScales.MinorPentatonic,
        }
        .Concat(ModeCatalog.Standard.All)
        .GroupBy(definition => definition.Id, StringComparer.Ordinal)
        .Select(group => group.First())
        .ToArray();

    /// <summary>Returns the step structure for a standard scale or mode identified by its stable id.</summary>
    public static ScaleStructure GetScaleStruct(string standardScaleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(standardScaleId);

        var definition = StandardDefinitions.FirstOrDefault(value =>
            string.Equals(value.Id, standardScaleId, StringComparison.Ordinal));
        if (definition is null)
        {
            throw new ArgumentException("The value is not a supported standard scale identifier.", nameof(standardScaleId));
        }

        return GetScaleStruct(definition);
    }

    /// <summary>Returns the step structure encoded by a scale definition.</summary>
    public static ScaleStructure GetScaleStruct(ScaleDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var offsets = definition.Degrees
            .Select(FormulaDegreeSemitones.Offset)
            .ToArray();
        return FromChromaticOffsets(offsets, nameof(definition));
    }

    /// <summary>Returns the step structure of a realized scale in formula order.</summary>
    public static ScaleStructure GetScaleStruct(Scale scale)
    {
        ArgumentNullException.ThrowIfNull(scale);
        return GetScaleStruct(scale.Pitches);
    }

    /// <summary>Returns the step structure of an ordered pitch collection across one octave.</summary>
    public static ScaleStructure GetScaleStruct(IEnumerable<SpelledPitch> pitches)
    {
        ArgumentNullException.ThrowIfNull(pitches);

        var copy = pitches.ToArray();
        if (copy.Length < 2)
        {
            throw new ArgumentException("At least two pitches are required to calculate a scale structure.", nameof(pitches));
        }

        return FromPitchCycle(copy, nameof(pitches));
    }

    /// <summary>Returns the step structure of an ordered note collection, ignoring octaves after preserving pitch order.</summary>
    public static ScaleStructure GetScaleStruct(IEnumerable<Note> notes)
    {
        ArgumentNullException.ThrowIfNull(notes);
        return GetScaleStruct(notes.Select(note => note.Pitch));
    }

    /// <summary>Converts tonic-relative chromatic offsets into successive octave-closing steps.</summary>
    private static ScaleStructure FromChromaticOffsets(IReadOnlyList<int> offsets, string parameterName)
    {
        if (offsets.Count < 2 || offsets[0] != 0 || offsets[^1] >= 12)
        {
            throw new ArgumentException(
                "Scale offsets must begin at zero and contain at least two tones within one octave.",
                parameterName);
        }

        var steps = new int[offsets.Count];
        for (var index = 0; index < offsets.Count - 1; index++)
        {
            steps[index] = offsets[index + 1] - offsets[index];
            if (steps[index] <= 0)
            {
                throw new ArgumentException(
                    "Scale offsets must be strictly ascending within one octave.",
                    parameterName);
            }
        }

        steps[^1] = 12 - offsets[^1];
        return new ScaleStructure(steps);
    }

    /// <summary>Calculates adjacent pitch-class distances and verifies that the sequence closes one octave exactly once.</summary>
    private static ScaleStructure FromPitchCycle(IReadOnlyList<SpelledPitch> pitches, string parameterName)
    {
        var steps = new int[pitches.Count];
        var total = 0;

        for (var index = 0; index < pitches.Count; index++)
        {
            var nextIndex = (index + 1) % pitches.Count;
            var distance = pitches[index].PitchClass.DistanceUpTo(pitches[nextIndex].PitchClass);
            if (distance == 0)
            {
                throw new ArgumentException("A scale structure cannot contain duplicate pitch classes.", parameterName);
            }

            steps[index] = distance;
            total += distance;
        }

        if (total != 12)
        {
            throw new ArgumentException("Pitches must be ordered as a single ascending cycle within one octave.", parameterName);
        }

        return new ScaleStructure(steps);
    }
}

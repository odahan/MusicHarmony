using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Scales;

/// <summary>Provides set-style relationships between realized scales or pitch collections.</summary>
public static class ScaleRelations
{
    /// <summary>Returns pitches from the first scale that are present in every supplied scale by pitch class.</summary>
    public static IReadOnlyList<SpelledPitch> GetCommonNotes(
        Scale first,
        Scale second,
        params Scale[] others) =>
        GetCommonNotes(PitchMatchMode.PitchClass, first, second, others);

    /// <summary>Returns pitches from the first scale that are present in every supplied scale using the requested match mode.</summary>
    public static IReadOnlyList<SpelledPitch> GetCommonNotes(
        PitchMatchMode matchMode,
        Scale first,
        Scale second,
        params Scale[] others)
    {
        ValidateMatchMode(matchMode);
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(others);

        var remaining = new IEnumerable<SpelledPitch>[others.Length + 1];
        remaining[0] = second.Pitches;
        for (var index = 0; index < others.Length; index++)
        {
            ArgumentNullException.ThrowIfNull(others[index]);
            remaining[index + 1] = others[index].Pitches;
        }

        return FindCommon(matchMode, first.Pitches, remaining);
    }

    /// <summary>Returns pitches from the first collection that are present in every supplied collection by pitch class.</summary>
    public static IReadOnlyList<SpelledPitch> GetCommonNotes(
        IEnumerable<SpelledPitch> first,
        IEnumerable<SpelledPitch> second,
        params IEnumerable<SpelledPitch>[] others) =>
        GetCommonNotes(PitchMatchMode.PitchClass, first, second, others);

    /// <summary>Returns pitches from the first collection that are present in every supplied collection using the requested match mode.</summary>
    public static IReadOnlyList<SpelledPitch> GetCommonNotes(
        PitchMatchMode matchMode,
        IEnumerable<SpelledPitch> first,
        IEnumerable<SpelledPitch> second,
        params IEnumerable<SpelledPitch>[] others)
    {
        ValidateMatchMode(matchMode);
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(others);

        var remaining = new IEnumerable<SpelledPitch>[others.Length + 1];
        remaining[0] = second;
        for (var index = 0; index < others.Length; index++)
        {
            ArgumentNullException.ThrowIfNull(others[index]);
            remaining[index + 1] = others[index];
        }

        return FindCommon(matchMode, first, remaining);
    }

    /// <summary>Computes the intersection while keeping the order and spelling of the first collection.</summary>
    private static IReadOnlyList<SpelledPitch> FindCommon(
        PitchMatchMode matchMode,
        IEnumerable<SpelledPitch> first,
        IReadOnlyList<IEnumerable<SpelledPitch>> remaining)
    {
        var source = first.ToArray();
        var others = remaining.Select(values => values.ToArray()).ToArray();

        return matchMode switch
        {
            PitchMatchMode.PitchClass => FindCommonPitchClasses(source, others),
            PitchMatchMode.ExactSpelling => FindCommonSpellings(source, others),
            _ => throw new ArgumentOutOfRangeException(nameof(matchMode)),
        };
    }

    /// <summary>Computes a distinct enharmonic intersection keyed by normalized pitch class.</summary>
    private static IReadOnlyList<SpelledPitch> FindCommonPitchClasses(
        IReadOnlyList<SpelledPitch> source,
        IReadOnlyList<SpelledPitch[]> others)
    {
        var lookup = others
            .Select(values => values.Select(pitch => pitch.PitchClass).ToHashSet())
            .ToArray();
        var seen = new HashSet<PitchClass>();
        var result = new List<SpelledPitch>();

        foreach (var pitch in source)
        {
            if (seen.Add(pitch.PitchClass) &&
                lookup.All(values => values.Contains(pitch.PitchClass)))
            {
                result.Add(pitch);
            }
        }

        return Array.AsReadOnly(result.ToArray());
    }

    /// <summary>Computes a distinct intersection keyed by exact written spelling.</summary>
    private static IReadOnlyList<SpelledPitch> FindCommonSpellings(
        IReadOnlyList<SpelledPitch> source,
        IReadOnlyList<SpelledPitch[]> others)
    {
        var lookup = others.Select(values => values.ToHashSet()).ToArray();
        var seen = new HashSet<SpelledPitch>();
        var result = new List<SpelledPitch>();

        foreach (var pitch in source)
        {
            if (seen.Add(pitch) &&
                lookup.All(values => values.Contains(pitch)))
            {
                result.Add(pitch);
            }
        }

        return Array.AsReadOnly(result.ToArray());
    }

    /// <summary>Rejects undefined enum values before dispatching to a comparison strategy.</summary>
    private static void ValidateMatchMode(PitchMatchMode matchMode)
    {
        if (!Enum.IsDefined(matchMode))
        {
            throw new ArgumentOutOfRangeException(nameof(matchMode));
        }
    }
}

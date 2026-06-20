using Enaxos.MusicTheory.Formulas;

namespace Enaxos.MusicTheory.Scales;

public static class PentatonicScales
{
    private const int PentatonicDegreeCount = 5;

    public static PentatonicDerivation FromScale(
        Scale source,
        PentatonicDerivationStrategy strategy =
            PentatonicDerivationStrategy.StandardMajorOrMinor,
        IReadOnlyList<int>? sourceDegrees = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ValidateStrategy(strategy);

        if (TryCreate(source, strategy, sourceDegrees, out var result))
        {
            return result!;
        }

        if (strategy == PentatonicDerivationStrategy.SelectSourceDegrees)
        {
            throw new ArgumentException(
                "Exactly five distinct, increasing source positions beginning with the tonic are required.",
                nameof(sourceDegrees));
        }

        if (sourceDegrees is not null)
        {
            throw new ArgumentException(
                "Source degrees are only valid with SelectSourceDegrees.",
                nameof(sourceDegrees));
        }

        throw new InvalidOperationException(
            "The source scale does not contain exactly one standard major or minor pentatonic scale.");
    }

    public static bool TryFromScale(
        Scale source,
        out PentatonicDerivation? result,
        PentatonicDerivationStrategy strategy =
            PentatonicDerivationStrategy.StandardMajorOrMinor,
        IReadOnlyList<int>? sourceDegrees = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ValidateStrategy(strategy);
        return TryCreate(source, strategy, sourceDegrees, out result);
    }

    private static bool TryCreate(
        Scale source,
        PentatonicDerivationStrategy strategy,
        IReadOnlyList<int>? sourceDegrees,
        out PentatonicDerivation? result)
    {
        result = null;

        return strategy switch
        {
            PentatonicDerivationStrategy.StandardMajorOrMinor =>
                TryCreateStandard(source, sourceDegrees, out result),
            PentatonicDerivationStrategy.SelectSourceDegrees =>
                TryCreateSelection(source, sourceDegrees, out result),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy)),
        };
    }

    private static bool TryCreateStandard(
        Scale source,
        IReadOnlyList<int>? sourceDegrees,
        out PentatonicDerivation? result)
    {
        result = null;
        if (sourceDegrees is not null)
        {
            return false;
        }

        var major = Scale.Create(source.Tonic, StandardScales.MajorPentatonic);
        var minor = Scale.Create(source.Tonic, StandardScales.MinorPentatonic);
        var majorMatches = TryFindSourcePositions(source, major, out var majorPositions);
        var minorMatches = TryFindSourcePositions(source, minor, out var minorPositions);

        if (majorMatches == minorMatches)
        {
            return false;
        }

        var selected = majorMatches ? major : minor;
        var positions = majorMatches ? majorPositions : minorPositions;
        result = new PentatonicDerivation(
            source,
            selected,
            PentatonicDerivationStrategy.StandardMajorOrMinor,
            positions);
        return true;
    }

    private static bool TryCreateSelection(
        Scale source,
        IReadOnlyList<int>? sourceDegrees,
        out PentatonicDerivation? result)
    {
        result = null;
        if (!IsValidSelection(source, sourceDegrees))
        {
            return false;
        }

        var positions = sourceDegrees!.ToArray();
        var degrees = positions
            .Select(position => source.Definition.Degrees[position - 1])
            .ToArray();
        var definition = new ScaleDefinition("scale.pentatonic.derived", degrees);
        var derived = Scale.Create(source.Tonic, definition);
        result = new PentatonicDerivation(
            source,
            derived,
            PentatonicDerivationStrategy.SelectSourceDegrees,
            positions);
        return true;
    }

    private static bool TryFindSourcePositions(
        Scale source,
        Scale candidate,
        out int[] positions)
    {
        positions = new int[PentatonicDegreeCount];
        var used = new HashSet<int>();

        for (var candidateIndex = 0; candidateIndex < candidate.Pitches.Count; candidateIndex++)
        {
            var found = false;
            for (var sourceIndex = 0; sourceIndex < source.Pitches.Count; sourceIndex++)
            {
                if (!used.Contains(sourceIndex) &&
                    candidate.Pitches[candidateIndex].IsEnharmonicWith(source.Pitches[sourceIndex]))
                {
                    positions[candidateIndex] = sourceIndex + 1;
                    used.Add(sourceIndex);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                positions = [];
                return false;
            }
        }

        return true;
    }

    private static bool IsValidSelection(Scale source, IReadOnlyList<int>? sourceDegrees)
    {
        if (sourceDegrees is null ||
            sourceDegrees.Count != PentatonicDegreeCount ||
            sourceDegrees[0] != 1)
        {
            return false;
        }

        for (var index = 0; index < sourceDegrees.Count; index++)
        {
            if (sourceDegrees[index] < 1 || sourceDegrees[index] > source.Pitches.Count)
            {
                return false;
            }

            if (index > 0 && sourceDegrees[index] <= sourceDegrees[index - 1])
            {
                return false;
            }
        }

        return true;
    }

    private static void ValidateStrategy(PentatonicDerivationStrategy strategy)
    {
        if (!Enum.IsDefined(strategy))
        {
            throw new ArgumentOutOfRangeException(nameof(strategy));
        }
    }
}

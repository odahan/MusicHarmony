using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Circle;

namespace Enaxos.MusicTheory.Geometry;

/// <summary>Builds immutable, renderer-independent geometry for a twelve-segment circle of fifths.</summary>
public sealed class CircleGeometry
{
    /// <summary>The immutable geometry in the same index order as the logical circle.</summary>
    private readonly ReadOnlyCollection<CircleGeometrySegment> _segments;
    private CircleGeometry(CircleGeometrySegment[] segments) => _segments = Array.AsReadOnly(segments);
    /// <summary>Gets the immutable twelve-segment geometry.</summary>
    public IReadOnlyList<CircleGeometrySegment> Segments => _segments;

    /// <summary>Creates sector angles and normalized label anchors for a logical circle.</summary>
    public static CircleGeometry Create(CircleOfFifths circle, CircleGeometryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(circle); options ??= new CircleGeometryOptions();
        if (!double.IsFinite(options.StartAngleDegrees) || !Enum.IsDefined(options.Direction)) throw new ArgumentOutOfRangeException(nameof(options));
        if (!double.IsFinite(options.MajorLabelRadius) || options.MajorLabelRadius is < 0 or > 1 ||
            !double.IsFinite(options.MinorLabelRadius) || options.MinorLabelRadius is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(options));
        // Twelve equal sectors cover 360 degrees; the sign alone controls progression.
        var sweep = options.Direction == RotationDirection.Clockwise ? 30d : -30d;
        var result = new CircleGeometrySegment[12];
        for (var index = 0; index < result.Length; index++)
        {
            var center = options.StartAngleDegrees + (index * sweep);
            var start = center - (sweep / 2d);
            result[index] = new CircleGeometrySegment(circle[index], start, center, sweep,
                Point(center, options.MajorLabelRadius), Point(center, options.MinorLabelRadius));
        }
        return new CircleGeometry(result);
    }

    /// <summary>Converts polar degrees and normalized radius to a Cartesian label anchor.</summary>
    private static UnitPoint Point(double degrees, double radius)
    {
        var radians = degrees * Math.PI / 180d;
        return new UnitPoint(Math.Cos(radians) * radius, Math.Sin(radians) * radius);
    }
}

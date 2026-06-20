using System.Collections.ObjectModel;
using Enaxos.MusicTheory.Circle;

namespace Enaxos.MusicTheory.Geometry;

public sealed class CircleGeometry
{
    private readonly ReadOnlyCollection<CircleGeometrySegment> _segments;
    private CircleGeometry(CircleGeometrySegment[] segments) => _segments = Array.AsReadOnly(segments);
    public IReadOnlyList<CircleGeometrySegment> Segments => _segments;

    public static CircleGeometry Create(CircleOfFifths circle, CircleGeometryOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(circle); options ??= new CircleGeometryOptions();
        if (!double.IsFinite(options.StartAngleDegrees) || !Enum.IsDefined(options.Direction)) throw new ArgumentOutOfRangeException(nameof(options));
        if (!double.IsFinite(options.MajorLabelRadius) || options.MajorLabelRadius is < 0 or > 1 ||
            !double.IsFinite(options.MinorLabelRadius) || options.MinorLabelRadius is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(options));
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

    private static UnitPoint Point(double degrees, double radius)
    {
        var radians = degrees * Math.PI / 180d;
        return new UnitPoint(Math.Cos(radians) * radius, Math.Sin(radians) * radius);
    }
}

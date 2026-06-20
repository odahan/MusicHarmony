using Enaxos.MusicTheory.Circle;

namespace Enaxos.MusicTheory.Geometry;

public sealed record CircleGeometrySegment
{
    internal CircleGeometrySegment(CircleSegment segment, double start, double center, double sweep, UnitPoint major, UnitPoint minor)
    { Segment = segment; StartAngleDegrees = start; CenterAngleDegrees = center; SweepAngleDegrees = sweep; MajorLabelAnchor = major; MinorLabelAnchor = minor; }
    public CircleSegment Segment { get; }
    public double StartAngleDegrees { get; }
    public double CenterAngleDegrees { get; }
    public double SweepAngleDegrees { get; }
    public UnitPoint MajorLabelAnchor { get; }
    public UnitPoint MinorLabelAnchor { get; }
}

using Enaxos.MusicTheory.Circle;

namespace Enaxos.MusicTheory.Geometry;

/// <summary>Contains renderer-independent angular and label-anchor geometry for one circle segment.</summary>
public sealed record CircleGeometrySegment
{
    internal CircleGeometrySegment(CircleSegment segment, double start, double center, double sweep, UnitPoint major, UnitPoint minor)
    { Segment = segment; StartAngleDegrees = start; CenterAngleDegrees = center; SweepAngleDegrees = sweep; MajorLabelAnchor = major; MinorLabelAnchor = minor; }
    /// <summary>Gets the logical circle segment described by this geometry.</summary>
    public CircleSegment Segment { get; }

    /// <summary>Gets the leading sector boundary angle in degrees.</summary>
    public double StartAngleDegrees { get; }

    /// <summary>Gets the sector center angle in degrees.</summary>
    public double CenterAngleDegrees { get; }

    /// <summary>Gets the signed angular sweep; its sign encodes rotation direction.</summary>
    public double SweepAngleDegrees { get; }

    /// <summary>Gets the normalized major-key label anchor.</summary>
    public UnitPoint MajorLabelAnchor { get; }

    /// <summary>Gets the normalized relative-minor label anchor.</summary>
    public UnitPoint MinorLabelAnchor { get; }
}

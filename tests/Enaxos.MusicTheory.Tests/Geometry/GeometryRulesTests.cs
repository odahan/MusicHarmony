using Enaxos.MusicTheory.Circle;
using Enaxos.MusicTheory.Geometry;

namespace Enaxos.MusicTheory.Tests.Geometry;

public sealed class GeometryRulesTests
{
    [Fact]
    public void Geometry_has_twelve_contiguous_thirty_degree_sectors()
    {
        var geometry = CircleGeometry.Create(CircleOfFifths.Standard);
        Assert.Equal(12, geometry.Segments.Count);
        Assert.All(geometry.Segments, item => Assert.Equal(30d, Math.Abs(item.SweepAngleDegrees)));
        Assert.Equal(-90d, geometry.Segments[0].CenterAngleDegrees);
        for (var index = 1; index < 12; index++)
            Assert.Equal(geometry.Segments[index - 1].StartAngleDegrees + geometry.Segments[index - 1].SweepAngleDegrees, geometry.Segments[index].StartAngleDegrees);
        Assert.Equal(360d, geometry.Segments.Sum(item => Math.Abs(item.SweepAngleDegrees)));
    }

    [Fact]
    public void Anchors_use_normalized_configured_radii()
    {
        var geometry = CircleGeometry.Create(CircleOfFifths.Standard);
        foreach (var item in geometry.Segments)
        {
            Assert.Equal(0.78, Math.Sqrt((item.MajorLabelAnchor.X * item.MajorLabelAnchor.X) + (item.MajorLabelAnchor.Y * item.MajorLabelAnchor.Y)), 12);
            Assert.Equal(0.52, Math.Sqrt((item.MinorLabelAnchor.X * item.MinorLabelAnchor.X) + (item.MinorLabelAnchor.Y * item.MinorLabelAnchor.Y)), 12);
            Assert.InRange(item.MajorLabelAnchor.X, -1, 1); Assert.InRange(item.MajorLabelAnchor.Y, -1, 1);
        }
    }

    [Fact]
    public void Counterclockwise_geometry_uses_negative_sweep()
    {
        var geometry = CircleGeometry.Create(CircleOfFifths.Standard, new CircleGeometryOptions { Direction = RotationDirection.CounterClockwise });
        Assert.All(geometry.Segments, item => Assert.Equal(-30d, item.SweepAngleDegrees));
    }
}

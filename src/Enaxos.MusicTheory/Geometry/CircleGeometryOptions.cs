namespace Enaxos.MusicTheory.Geometry;

public sealed record CircleGeometryOptions
{
    public double StartAngleDegrees { get; init; } = -90.0;
    public RotationDirection Direction { get; init; } = RotationDirection.Clockwise;
    public double MajorLabelRadius { get; init; } = 0.78;
    public double MinorLabelRadius { get; init; } = 0.52;
}

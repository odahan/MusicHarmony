namespace Enaxos.MusicTheory.Geometry;

/// <summary>Configures angular orientation and normalized label radii for circle geometry.</summary>
public sealed record CircleGeometryOptions
{
    /// <summary>Gets the center angle of segment zero in degrees; -90 places C at the top.</summary>
    public double StartAngleDegrees { get; init; } = -90.0;

    /// <summary>Gets the direction in which subsequent segment centers progress.</summary>
    public RotationDirection Direction { get; init; } = RotationDirection.Clockwise;

    /// <summary>Gets the normalized radius of major-key label anchors.</summary>
    public double MajorLabelRadius { get; init; } = 0.78;

    /// <summary>Gets the normalized radius of relative-minor label anchors.</summary>
    public double MinorLabelRadius { get; init; } = 0.52;
}

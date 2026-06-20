namespace Enaxos.MusicTheory.Geometry;

/// <summary>Represents a Cartesian point relative to a unit-radius circle centered at the origin.</summary>
/// <param name="X">The horizontal coordinate, normally in the range -1 through 1.</param>
/// <param name="Y">The vertical coordinate, normally in the range -1 through 1.</param>
public readonly record struct UnitPoint(double X, double Y);

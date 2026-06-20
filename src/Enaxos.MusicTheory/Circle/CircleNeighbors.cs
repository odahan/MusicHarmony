namespace Enaxos.MusicTheory.Circle;

/// <summary>Groups a circle segment with its immediate subdominant and dominant neighbors.</summary>
/// <param name="Subdominant">The previous counterclockwise segment.</param>
/// <param name="Current">The requested segment.</param>
/// <param name="Dominant">The next clockwise segment.</param>
public readonly record struct CircleNeighbors(CircleSegment Subdominant, CircleSegment Current, CircleSegment Dominant);

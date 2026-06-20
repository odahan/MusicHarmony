namespace Enaxos.MusicTheory.Circle;

/// <summary>Contains directional step counts between two circle segments.</summary>
/// <param name="ClockwiseSteps">The normalized clockwise distance from 0 through 11.</param>
/// <param name="CounterClockwiseSteps">The normalized counterclockwise distance from 0 through 11.</param>
public readonly record struct CircleDistance(int ClockwiseSteps, int CounterClockwiseSteps)
{
    /// <summary>Gets the length of a shortest path in either direction.</summary>
    public int MinimumSteps => Math.Min(ClockwiseSteps, CounterClockwiseSteps);

    /// <summary>Gets whether clockwise and counterclockwise paths are equally short.</summary>
    public bool HasTwoShortestPaths => ClockwiseSteps == CounterClockwiseSteps;
}

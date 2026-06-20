namespace Enaxos.MusicTheory.Circle;

public readonly record struct CircleDistance(int ClockwiseSteps, int CounterClockwiseSteps)
{
    public int MinimumSteps => Math.Min(ClockwiseSteps, CounterClockwiseSteps);
    public bool HasTwoShortestPaths => ClockwiseSteps == CounterClockwiseSteps;
}

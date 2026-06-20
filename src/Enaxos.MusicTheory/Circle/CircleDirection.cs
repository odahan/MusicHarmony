namespace Enaxos.MusicTheory.Circle;

/// <summary>Specifies movement around the logical circle of fifths.</summary>
public enum CircleDirection
{
    /// <summary>Moves toward the dominant, adding one sharp or removing one flat.</summary>
    Clockwise,

    /// <summary>Moves toward the subdominant, adding one flat or removing one sharp.</summary>
    CounterClockwise,
}

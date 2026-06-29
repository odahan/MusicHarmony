using Enaxos.MusicTheory.Circle;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Tests.Circle;

public sealed class CircleLookupAndNavigationRulesTests
{
    [Fact]
    public void Every_major_and_relative_minor_spelling_can_be_found()
    {
        var circle = CircleOfFifths.Standard;

        foreach (var segment in circle.Segments)
        {
            foreach (var pair in segment.Spellings)
            {
                Assert.Same(segment, circle.Find(pair.Major));
                Assert.Same(segment, circle.Find(pair.RelativeMinor));
                Assert.True(circle.TryFind(pair.Major, out var byMajor));
                Assert.True(circle.TryFind(pair.RelativeMinor, out var byMinor));
                Assert.Same(segment, byMajor);
                Assert.Same(segment, byMinor);
            }
        }
    }

    [Fact]
    public void Unsupported_key_spellings_are_rejected_without_false_match()
    {
        var circle = CircleOfFifths.Standard;
        var unsupported = MusicalKey.Parse("G# major");

        Assert.False(circle.TryFind(unsupported, out var segment));
        Assert.Null(segment);
        Assert.Throws<KeyNotFoundException>(() => circle.Find(unsupported));
    }

    [Fact]
    public void Move_wraps_clockwise_and_counterclockwise()
    {
        var circle = CircleOfFifths.Standard;

        Assert.Same(circle[2], circle.Move(circle[11], CircleDirection.Clockwise, 3));
        Assert.Same(circle[10], circle.Move(circle[1], CircleDirection.CounterClockwise, 3));
        Assert.Same(circle[4], circle.Move(circle[4], CircleDirection.Clockwise, 0));
    }

    [Fact]
    public void Move_validates_direction_steps_and_source()
    {
        var circle = CircleOfFifths.Standard;

        Assert.Throws<ArgumentNullException>(() =>
            circle.Move(null!, CircleDirection.Clockwise));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            circle.Move(circle[0], (CircleDirection)99));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            circle.Move(circle[0], CircleDirection.Clockwise, -1));
    }

    [Fact]
    public void Distances_are_complementary_and_replayable_as_moves()
    {
        var circle = CircleOfFifths.Standard;

        foreach (var from in circle.Segments)
        {
            foreach (var to in circle.Segments)
            {
                var distance = circle.GetDistance(from, to);

                Assert.InRange(distance.ClockwiseSteps, 0, 11);
                Assert.InRange(distance.CounterClockwiseSteps, 0, 11);
                Assert.Same(to, circle.Move(from, CircleDirection.Clockwise, distance.ClockwiseSteps));
                Assert.Same(to, circle.Move(from, CircleDirection.CounterClockwise, distance.CounterClockwiseSteps));
                Assert.Equal(
                    from == to ? 0 : 12,
                    distance.ClockwiseSteps + distance.CounterClockwiseSteps);
            }
        }
    }
}

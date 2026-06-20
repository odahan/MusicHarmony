using Enaxos.MusicTheory.Circle;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Tests.Circle;

public sealed class CircleRulesTests
{
    [Fact]
    public void Standard_circle_has_twelve_ordered_segments_and_fifths()
    {
        var circle = CircleOfFifths.Standard;
        Assert.Equal(12, circle.Segments.Count);
        Assert.Equal(Enumerable.Range(0, 12), circle.Segments.Select(segment => segment.Index));
        for (var index = 0; index < 12; index++)
        {
            var current = circle[index].Primary.Major.Tonic.PitchClass;
            var next = circle[index + 1].Primary.Major.Tonic.PitchClass;
            Assert.Equal(7, current.DistanceUpTo(next));
        }
    }

    [Fact]
    public void Three_positions_preserve_both_conventional_spellings()
    {
        var circle = CircleOfFifths.Standard;
        Assert.Equal(["B", "Cb"], circle[5].Spellings.Select(item => item.Major.Tonic.ToString()));
        Assert.Equal(["F#", "Gb"], circle[6].Spellings.Select(item => item.Major.Tonic.ToString()));
        Assert.Equal(["C#", "Db"], circle[7].Spellings.Select(item => item.Major.Tonic.ToString()));
    }

    [Fact]
    public void Preference_changes_primary_but_never_removes_spellings()
    {
        var sharps = CircleOfFifths.Create(EnharmonicPreference.PreferSharps);
        var flats = CircleOfFifths.Create(EnharmonicPreference.PreferFlats);
        Assert.Equal("F#", sharps[6].Primary.Major.Tonic.ToString());
        Assert.Equal("Gb", flats[6].Primary.Major.Tonic.ToString());
        Assert.Equal(2, sharps[6].Spellings.Count); Assert.Equal(2, flats[6].Spellings.Count);
    }

    [Fact]
    public void Eb_neighbors_and_relative_match_normative_examples()
    {
        var circle = CircleOfFifths.Standard;
        var eb = circle.Find(MusicalKey.Parse("Eb major")); var neighbors = circle.GetNeighbors(eb);
        Assert.Equal("Bb major", neighbors.Dominant.Primary.Major.ToString());
        Assert.Equal("Ab major", neighbors.Subdominant.Primary.Major.ToString());
        Assert.Equal("C minor", eb.Primary.RelativeMinor.ToString());
    }

    [Fact]
    public void Distance_keeps_both_paths_and_normalizes_indexes()
    {
        var circle = CircleOfFifths.Standard;
        var distance = circle.GetDistance(circle[0], circle[6]);
        Assert.Equal(6, distance.ClockwiseSteps); Assert.Equal(6, distance.CounterClockwiseSteps);
        Assert.True(distance.HasTwoShortestPaths); Assert.Same(circle[11], circle[-1]); Assert.Same(circle[1], circle[13]);
    }
}

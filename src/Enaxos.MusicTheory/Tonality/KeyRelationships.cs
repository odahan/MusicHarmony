using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tonality;

public static class KeyRelationships
{
    public static MusicalKey RelativeOf(MusicalKey key)
    {
        if (key.Mode == KeyMode.Major)
        {
            var scale = Scale.Create(key.Tonic, StandardScales.Major);
            return MusicalKey.Minor(scale.Degree(6));
        }

        var minor = Scale.Create(key.Tonic, StandardScales.NaturalMinor);
        return MusicalKey.Major(minor.Degree(3));
    }

    public static MusicalKey ParallelOf(MusicalKey key) => new(
        key.Tonic,
        key.Mode == KeyMode.Major ? KeyMode.Minor : KeyMode.Major);
}

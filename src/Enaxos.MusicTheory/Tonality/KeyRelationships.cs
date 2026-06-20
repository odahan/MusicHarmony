using Enaxos.MusicTheory.Scales;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Computes standard relative and parallel relationships between major and minor keys.</summary>
public static class KeyRelationships
{
    /// <summary>Gets the key sharing the same signature in the opposite mode.</summary>
    /// <remarks>Major maps to its sixth degree; natural minor maps to its third degree.</remarks>
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

    /// <summary>Gets the opposite mode on the same written tonic.</summary>
    public static MusicalKey ParallelOf(MusicalKey key) => new(
        key.Tonic,
        key.Mode == KeyMode.Major ? KeyMode.Minor : KeyMode.Major);
}

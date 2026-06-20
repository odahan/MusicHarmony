using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tonality;

/// <summary>Identifies a tonal key by its tonic spelling and major/minor mode.</summary>
public readonly record struct MusicalKey
{
    /// <summary>Creates a musical key.</summary>
    /// <param name="tonic">The written tonic spelling.</param>
    /// <param name="mode">The major or minor key mode.</param>
    public MusicalKey(SpelledPitch tonic, KeyMode mode)
    {
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        Tonic = tonic;
        Mode = mode;
    }

    /// <summary>Gets the tonic spelling.</summary>
    public SpelledPitch Tonic { get; }

    /// <summary>Gets the major or minor mode.</summary>
    public KeyMode Mode { get; }

    /// <summary>Creates a major key on the supplied tonic.</summary>
    public static MusicalKey Major(SpelledPitch tonic) => new(tonic, KeyMode.Major);

    /// <summary>Creates a minor key on the supplied tonic.</summary>
    public static MusicalKey Minor(SpelledPitch tonic) => new(tonic, KeyMode.Minor);

    /// <summary>
    /// Parses a key using a <c>major</c>/<c>minor</c> suffix, a compact <c>m</c> suffix,
    /// or no suffix for major.
    /// </summary>
    public static MusicalKey Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var value = text.Trim();

        if (value.EndsWith(" major", StringComparison.OrdinalIgnoreCase))
        {
            return Major(SpelledPitch.Parse(value[..^6].TrimEnd()));
        }

        if (value.EndsWith(" minor", StringComparison.OrdinalIgnoreCase))
        {
            return Minor(SpelledPitch.Parse(value[..^6].TrimEnd()));
        }

        if (value.EndsWith('m') && value.Length > 1)
        {
            return Minor(SpelledPitch.Parse(value[..^1]));
        }

        return Major(SpelledPitch.Parse(value));
    }

    /// <summary>Returns the invariant tonic followed by <c>major</c> or <c>minor</c>.</summary>
    public override string ToString() => string.Concat(
        Tonic.ToString(),
        Mode == KeyMode.Major ? " major" : " minor");
}

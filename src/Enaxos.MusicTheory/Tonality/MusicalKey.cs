using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tonality;

public readonly record struct MusicalKey
{
    public MusicalKey(SpelledPitch tonic, KeyMode mode)
    {
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        Tonic = tonic;
        Mode = mode;
    }

    public SpelledPitch Tonic { get; }

    public KeyMode Mode { get; }

    public static MusicalKey Major(SpelledPitch tonic) => new(tonic, KeyMode.Major);

    public static MusicalKey Minor(SpelledPitch tonic) => new(tonic, KeyMode.Minor);

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

    public override string ToString() => string.Concat(
        Tonic.ToString(),
        Mode == KeyMode.Major ? " major" : " minor");
}

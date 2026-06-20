using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

public readonly record struct ChordSymbol
{
    public ChordSymbol(SpelledPitch root, string definitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);
        Root = root;
        DefinitionId = definitionId;
    }

    public SpelledPitch Root { get; }

    public string DefinitionId { get; }

    public static ChordSymbol Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (!TryParse(text.AsSpan(), out var result))
        {
            throw new FormatException("The value is not a supported chord symbol.");
        }

        return result;
    }

    public static bool TryParse(ReadOnlySpan<char> text, out ChordSymbol result)
    {
        result = default;
        for (var length = Math.Min(3, text.Length); length >= 1; length--)
        {
            if (!SpelledPitch.TryParse(text[..length], out var root)) continue;
            var id = SuffixId(text[length..]);
            if (id is null) continue;
            result = new ChordSymbol(root, id);
            return true;
        }

        return false;
    }

    public override string ToString() => string.Concat(Root.ToString(), CanonicalSuffix(DefinitionId));

    private static string? SuffixId(ReadOnlySpan<char> suffix)
    {
        if (suffix.IsEmpty) return StandardChords.Major.Id;
        if (suffix.SequenceEqual("m")) return StandardChords.Minor.Id;
        if (suffix.SequenceEqual("dim") || suffix.SequenceEqual("°")) return StandardChords.Diminished.Id;
        if (suffix.SequenceEqual("aug") || suffix.SequenceEqual("+")) return StandardChords.Augmented.Id;
        if (suffix.SequenceEqual("7")) return StandardChords.DominantSeventh.Id;
        if (suffix.SequenceEqual("maj7")) return StandardChords.MajorSeventh.Id;
        if (suffix.SequenceEqual("m7")) return StandardChords.MinorSeventh.Id;
        if (suffix.SequenceEqual("ø7") || suffix.SequenceEqual("m7b5")) return StandardChords.HalfDiminishedSeventh.Id;
        if (suffix.SequenceEqual("dim7") || suffix.SequenceEqual("°7")) return StandardChords.DiminishedSeventh.Id;
        return null;
    }

    private static string CanonicalSuffix(string id) => id switch
    {
        "chord.major" => "",
        "chord.minor" => "m",
        "chord.diminished" => "°",
        "chord.augmented" => "+",
        "chord.dominant7" => "7",
        "chord.major7" => "maj7",
        "chord.minor7" => "m7",
        "chord.half-diminished7" => "ø7",
        "chord.diminished7" => "°7",
        _ => string.Concat("[", id, "]"),
    };
}

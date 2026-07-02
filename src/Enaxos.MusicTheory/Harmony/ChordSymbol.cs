using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Harmony;

/// <summary>Represents a lead-sheet chord symbol as a spelled root and a stable definition identifier.</summary>
public readonly record struct ChordSymbol
{
    /// <summary>Creates a chord symbol from normalized components.</summary>
    public ChordSymbol(SpelledPitch root, string definitionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(definitionId);
        Root = root;
        DefinitionId = definitionId;
    }

    /// <summary>Gets the written chord root.</summary>
    public SpelledPitch Root { get; }

    /// <summary>Gets the stable chord-definition identifier encoded by the suffix.</summary>
    public string DefinitionId { get; }

    /// <summary>Parses one of the supported lead-sheet chord symbols.</summary>
    public static ChordSymbol Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (!TryParse(text.AsSpan(), out var result))
        {
            throw new FormatException("The value is not a supported chord symbol.");
        }

        return result;
    }

    /// <summary>Attempts to parse a complete supported lead-sheet chord symbol.</summary>
    public static bool TryParse(ReadOnlySpan<char> text, out ChordSymbol result)
    {
        result = default;
        // Try the longest plausible root first so a multi-character accidental is not
        // incorrectly consumed as part of the chord-quality suffix.
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

    /// <summary>Returns the root followed by the canonical suffix for its definition.</summary>
    public override string ToString() => string.Concat(Root.ToString(), CanonicalSuffix(DefinitionId));

    /// <summary>Maps every accepted suffix alias to a stable definition identifier.</summary>
    private static string? SuffixId(ReadOnlySpan<char> suffix)
    {
        if (suffix.IsEmpty) return StandardChords.Major.Id;
        if (suffix.SequenceEqual("m")) return StandardChords.Minor.Id;
        if (suffix.SequenceEqual("dim") || suffix.SequenceEqual("°")) return StandardChords.Diminished.Id;
        if (suffix.SequenceEqual("aug") || suffix.SequenceEqual("+")) return StandardChords.Augmented.Id;
        if (suffix.SequenceEqual("sus2")) return StandardChords.SuspendedSecond.Id;
        if (suffix.SequenceEqual("sus4") || suffix.SequenceEqual("sus")) return StandardChords.SuspendedFourth.Id;
        if (suffix.SequenceEqual("6")) return StandardChords.MajorSixth.Id;
        if (suffix.SequenceEqual("m6")) return StandardChords.MinorSixth.Id;
        if (suffix.SequenceEqual("7")) return StandardChords.DominantSeventh.Id;
        if (suffix.SequenceEqual("maj7")) return StandardChords.MajorSeventh.Id;
        if (suffix.SequenceEqual("m7")) return StandardChords.MinorSeventh.Id;
        if (suffix.SequenceEqual("ø7") || suffix.SequenceEqual("m7b5")) return StandardChords.HalfDiminishedSeventh.Id;
        if (suffix.SequenceEqual("dim7") || suffix.SequenceEqual("°7")) return StandardChords.DiminishedSeventh.Id;
        if (suffix.SequenceEqual("9")) return StandardChords.DominantNinth.Id;
        if (suffix.SequenceEqual("maj9")) return StandardChords.MajorNinth.Id;
        if (suffix.SequenceEqual("m9")) return StandardChords.MinorNinth.Id;
        if (suffix.SequenceEqual("11")) return StandardChords.DominantEleventh.Id;
        if (suffix.SequenceEqual("maj11")) return StandardChords.MajorEleventh.Id;
        if (suffix.SequenceEqual("m11")) return StandardChords.MinorEleventh.Id;
        if (suffix.SequenceEqual("13")) return StandardChords.DominantThirteenth.Id;
        if (suffix.SequenceEqual("maj13")) return StandardChords.MajorThirteenth.Id;
        if (suffix.SequenceEqual("m13")) return StandardChords.MinorThirteenth.Id;
        return null;
    }

    /// <summary>Maps a definition identifier to the preferred display suffix.</summary>
    private static string CanonicalSuffix(string id) => id switch
    {
        "chord.major" => "",
        "chord.minor" => "m",
        "chord.diminished" => "°",
        "chord.augmented" => "+",
        "chord.sus2" => "sus2",
        "chord.sus4" => "sus4",
        "chord.major6" => "6",
        "chord.minor6" => "m6",
        "chord.dominant7" => "7",
        "chord.major7" => "maj7",
        "chord.minor7" => "m7",
        "chord.half-diminished7" => "ø7",
        "chord.diminished7" => "°7",
        "chord.dominant9" => "9",
        "chord.major9" => "maj9",
        "chord.minor9" => "m9",
        "chord.dominant11" => "11",
        "chord.major11" => "maj11",
        "chord.minor11" => "m11",
        "chord.dominant13" => "13",
        "chord.major13" => "maj13",
        "chord.minor13" => "m13",
        _ => string.Concat("[", id, "]"),
    };
}

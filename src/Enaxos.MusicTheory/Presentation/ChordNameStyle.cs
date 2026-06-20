namespace Enaxos.MusicTheory.Presentation;

/// <summary>Specifies whether a chord is displayed with a lead-sheet suffix or a full quality name.</summary>
public enum ChordNameStyle
{
    /// <summary>Uses canonical abbreviations such as <c>m7</c> and <c>maj7</c>.</summary>
    StandardAbbreviation,

    /// <summary>Uses a localized full chord-quality name.</summary>
    Full,
}

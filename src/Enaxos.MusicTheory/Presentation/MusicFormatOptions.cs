namespace Enaxos.MusicTheory.Presentation;

/// <summary>Overrides presentation settings for one formatting operation.</summary>
public sealed record MusicFormatOptions
{
    /// <summary>Gets a per-call terminology override, or null to capture the global default.</summary>
    public MusicTerminology? TerminologyOverride { get; init; }

    /// <summary>Gets the accidental glyph style used by the call.</summary>
    public AccidentalGlyphStyle Accidentals { get; init; } = AccidentalGlyphStyle.Unicode;

    /// <summary>Gets the numeric culture, or null for invariant formatting.</summary>
    public IFormatProvider? Culture { get; init; }
}

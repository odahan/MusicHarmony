namespace Enaxos.MusicTheory.Presentation;

public sealed record MusicFormatOptions
{
    public MusicTerminology? TerminologyOverride { get; init; }
    public AccidentalGlyphStyle Accidentals { get; init; } = AccidentalGlyphStyle.Unicode;
    public IFormatProvider? Culture { get; init; }
}

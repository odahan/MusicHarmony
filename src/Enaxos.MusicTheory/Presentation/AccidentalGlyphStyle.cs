namespace Enaxos.MusicTheory.Presentation;

/// <summary>Specifies which glyph family is used to display accidentals.</summary>
public enum AccidentalGlyphStyle
{
    /// <summary>Uses dedicated musical Unicode symbols where available.</summary>
    Unicode,

    /// <summary>Uses portable ASCII sequences such as <c>#</c>, <c>b</c>, and <c>##</c>.</summary>
    Ascii,
}

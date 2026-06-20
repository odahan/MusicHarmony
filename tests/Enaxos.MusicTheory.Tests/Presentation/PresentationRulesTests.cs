using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Presentation;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Tests.Presentation;

public sealed class PresentationRulesTests
{
    [Fact]
    public void French_and_American_chord_examples_are_exact()
    {
        var chord = Chord.Create(SpelledPitch.Parse("C"), StandardChords.MinorSeventh);
        var french = new MusicFormatOptions { TerminologyOverride = MusicTerminology.French };
        var american = new MusicFormatOptions { TerminologyOverride = MusicTerminology.American, Accidentals = AccidentalGlyphStyle.Ascii };
        Assert.Equal("Do m7", MusicFormatter.Format(chord, options: french));
        Assert.Equal("C minor seventh", MusicFormatter.Format(chord, ChordNameStyle.Full, american));
        Assert.Equal("Do Mi♭ Sol Si♭", MusicFormatter.FormatChordPitches(chord, french));
    }

    [Fact]
    public void Local_override_never_changes_global_default()
    {
        MusicDisplayDefaults.Terminology = MusicTerminology.French;
        var pitch = SpelledPitch.Parse("Db");
        Assert.Equal("Ré♭", MusicFormatter.Format(pitch));
        Assert.Equal("Db", MusicFormatter.Format(pitch, new MusicFormatOptions { TerminologyOverride = MusicTerminology.American, Accidentals = AccidentalGlyphStyle.Ascii }));
        Assert.Equal(MusicTerminology.French, MusicDisplayDefaults.Terminology);
    }

    [Theory]
    [InlineData(1, HarmonicChordQuality.Major, "I")]
    [InlineData(2, HarmonicChordQuality.Minor, "ii")]
    [InlineData(7, HarmonicChordQuality.Diminished, "vii°")]
    [InlineData(7, HarmonicChordQuality.HalfDiminished, "viiø")]
    [InlineData(3, HarmonicChordQuality.Augmented, "III+")]
    public void Roman_numerals_follow_quality_conventions(int degree, HarmonicChordQuality quality, string expected)
    {
        Assert.Equal(expected, MusicFormatter.Format(new HarmonicFunction(new ScaleDegreeNumber(degree), quality, 0)));
    }

    [Fact]
    public void Major_mode_names_follow_both_terminologies()
    {
        Assert.Equal("mode de sol", MusicFormatter.Format(StandardScales.Mixolydian, new MusicFormatOptions { TerminologyOverride = MusicTerminology.French }));
        Assert.Equal("Mixolydian", MusicFormatter.Format(StandardScales.Mixolydian, new MusicFormatOptions { TerminologyOverride = MusicTerminology.American }));
    }
}

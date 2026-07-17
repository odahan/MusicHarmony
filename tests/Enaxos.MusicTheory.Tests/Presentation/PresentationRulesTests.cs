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
    public void Extended_chord_names_and_observed_pitches_are_formatted()
    {
        var american = new MusicFormatOptions { TerminologyOverride = MusicTerminology.American, Accidentals = AccidentalGlyphStyle.Ascii };

        Assert.Equal("C6", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.MajorSixth), options: american));
        Assert.Equal("Csus2", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.SuspendedSecond), options: american));
        Assert.Equal("C suspended fourth", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.SuspendedFourth), ChordNameStyle.Full, american));
        Assert.Equal("Cmaj9", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.MajorNinth), options: american));
        Assert.Equal("C dominant eleventh", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.DominantEleventh), ChordNameStyle.Full, american));
        Assert.Equal("C major eleventh", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.MajorEleventh), ChordNameStyle.Full, american));
        Assert.Equal("C13", MusicFormatter.Format(Chord.Create(SpelledPitch.Parse("C"), StandardChords.DominantThirteenth), options: american));
        Assert.Equal("A# D F", MusicFormatter.FormatPitches([SpelledPitch.Parse("A#"), SpelledPitch.Parse("D"), SpelledPitch.Parse("F")], american));
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

    [Fact]
    public void Exotic_scale_names_are_formatted()
    {
        var american = new MusicFormatOptions { TerminologyOverride = MusicTerminology.American };
        var french = new MusicFormatOptions { TerminologyOverride = MusicTerminology.French };

        Assert.Equal("major blues", MusicFormatter.Format(
            ExoticScales.BluesAndBebop.Single(definition => definition.Id == "scale.exotic.blues.major"),
            american));
        Assert.Equal("octatonic half-whole", MusicFormatter.Format(ExoticScales.OctatonicHalfWhole, american));
        Assert.Equal("octatonique ton-demi-ton", MusicFormatter.Format(ExoticScales.OctatonicWholeHalf, french));
        Assert.Equal("Hirajoshi", MusicFormatter.Format(
            ExoticScales.Japanese.Single(definition => definition.Id == "scale.exotic.japanese.hirajoshi"),
            american));
    }

    [Fact]
    public void Derived_chord_names_use_recognition_or_return_no_name()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.MajorPentatonic);
        var chords = ScaleHarmony.GetDiatonicTriads(scale);
        var american = new MusicFormatOptions { TerminologyOverride = MusicTerminology.American, Accidentals = AccidentalGlyphStyle.Ascii };

        Assert.True(MusicFormatter.TryFormatChordName(chords[0].Chord, out var firstName, options: american));
        Assert.Equal("Am/C", firstName);

        Assert.True(MusicFormatter.TryFormatChordName(chords[1].Chord, out var secondName, options: american));
        Assert.Equal("Csus2/D", secondName);
    }

    [Fact]
    public void Roman_numerals_are_only_formatted_for_heptatonic_scale_chords()
    {
        var heptatonic = ScaleHarmony.GetDiatonicTriads(Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major))[0];
        var pentatonic = ScaleHarmony.GetDiatonicTriads(Scale.Create(SpelledPitch.Parse("C"), StandardScales.MajorPentatonic))[0];

        Assert.True(MusicFormatter.TryFormatRomanNumeral(heptatonic, out var romanNumeral));
        Assert.Equal("I", romanNumeral);

        Assert.False(MusicFormatter.TryFormatRomanNumeral(pentatonic, out var pentatonicRomanNumeral));
        Assert.Equal(string.Empty, pentatonicRomanNumeral);
    }
}

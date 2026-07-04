using Enaxos.MusicTheory.Formulas;
using Enaxos.MusicTheory.Presentation;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Tests.Tonality;

public sealed class ScaleHarmonyRulesTests
{
    [Fact]
    public void Major_scale_triads_have_expected_notes_and_roman_numerals()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);

        var chords = ScaleHarmony.GetDiatonicTriads(scale);

        Assert.Equal(7, chords.Count);
        Assert.Equal(
            ["C E G", "D F A", "E G B", "F A C", "G B D", "A C E", "B D F"],
            chords.Select(chord => string.Join(" ", chord.Chord.Pitches)));
        Assert.Equal(
            ["I", "ii", "iii", "IV", "V", "vi", "vii°"],
            chords.Select(chord => MusicFormatter.Format(chord.Function)));
    }

    [Fact]
    public void Harmonic_minor_scale_triads_include_augmented_and_diminished_qualities()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.HarmonicMinor);

        var chords = ScaleHarmony.GetDiatonicTriads(scale);

        Assert.Equal(
            ["i", "ii°", "III+", "iv", "V", "VI", "vii°"],
            chords.Select(chord => MusicFormatter.Format(chord.Function)));
        Assert.Equal(
            ["C Eb G", "D F Ab", "Eb G B", "F Ab C", "G B D", "Ab C Eb", "B D F"],
            chords.Select(chord => string.Join(" ", chord.Chord.Pitches)));
    }

    [Fact]
    public void Pentatonic_scales_return_one_basic_three_note_chord_per_degree()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.MajorPentatonic);

        var chords = ScaleHarmony.GetDiatonicTriads(scale);

        Assert.Equal(5, chords.Count);
        Assert.Equal(
            ["C E A", "D G C", "E A D", "G C E", "A D G"],
            chords.Select(chord => string.Join(" ", chord.Chord.Pitches)));
        Assert.All(chords, chord => Assert.Equal(HarmonicChordQuality.Other, chord.Quality));
        Assert.Equal(
            ["I", "II", "III", "IV", "V"],
            chords.Select(chord => MusicFormatter.Format(chord.Function)));
    }

    [Fact]
    public void Scale_harmony_generates_chords_for_hexatonic_sources()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), ExoticScales.BluesAndBebop.First(definition => definition.Id == "scale.exotic.blues.major"));

        var chords = ScaleHarmony.GetDiatonicTriads(scale);

        Assert.Equal(6, chords.Count);
        Assert.Equal(
            ["C Eb G", "D E A", "Eb G C", "E A D", "G C Eb", "A D E"],
            chords.Select(chord => string.Join(" ", chord.Chord.Pitches)));
    }

    [Fact]
    public void Scale_harmony_generates_chords_for_octatonic_sources()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), ExoticScales.SymmetricAndJazz.First(definition => definition.Id == "scale.exotic.diminished.half-whole"));

        var chords = ScaleHarmony.GetDiatonicTriads(scale);

        Assert.Equal(8, chords.Count);
        Assert.Equal(
            ["C Eb Gb", "Db E G", "Eb Gb A", "E G Bb", "Gb A C", "G Bb Db", "A C Eb", "Bb Db E"],
            chords.Select(chord => string.Join(" ", chord.Chord.Pitches)));
    }

    [Fact]
    public void Scale_harmony_results_are_read_only_snapshots()
    {
        var scale = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);
        var chords = ScaleHarmony.GetDiatonicTriads(scale);
        var mutableView = Assert.IsAssignableFrom<ICollection<ScaleChord>>(chords);

        Assert.Throws<NotSupportedException>(() => mutableView.Add(chords[0]));
    }
}

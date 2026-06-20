using System.Globalization;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Presentation;

public static class MusicFormatter
{
    public static string Format(SpelledPitch pitch, MusicFormatOptions? options = null)
    { var settings = Capture(options); return Pitch(pitch, settings); }

    public static string Format(Note note, MusicFormatOptions? options = null)
    { var settings = Capture(options); return string.Concat(Pitch(note.Pitch, settings), note.Octave.ToString(settings.Culture)); }

    public static string Format(MusicalKey key, MusicFormatOptions? options = null)
    {
        var settings = Capture(options); var mode = settings.Terminology == MusicTerminology.French
            ? key.Mode == KeyMode.Major ? " majeur" : " mineur"
            : key.Mode == KeyMode.Major ? " major" : " minor";
        return string.Concat(Pitch(key.Tonic, settings), mode);
    }

    public static string Format(Chord chord, ChordNameStyle style = ChordNameStyle.StandardAbbreviation, MusicFormatOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chord); if (!Enum.IsDefined(style)) throw new ArgumentOutOfRangeException(nameof(style));
        var settings = Capture(options); var root = Pitch(chord.Root, settings);
        if (style == ChordNameStyle.StandardAbbreviation)
        {
            var suffix = Abbreviation(chord.Definition.Id);
            return settings.Terminology == MusicTerminology.French && suffix.Length > 0
                ? string.Concat(root, " ", suffix) : string.Concat(root, suffix);
        }
        return string.Concat(root, " ", FullChordName(chord.Definition.Id, settings.Terminology));
    }

    public static string FormatChordPitches(Chord chord, MusicFormatOptions? options = null)
    { ArgumentNullException.ThrowIfNull(chord); var settings = Capture(options); return string.Join(" ", chord.Pitches.Select(pitch => Pitch(pitch, settings))); }

    public static string Format(ScaleDefinition definition, MusicFormatOptions? options = null)
    { ArgumentNullException.ThrowIfNull(definition); var settings = Capture(options); return ScaleName(definition.Id, settings.Terminology); }

    public static string Format(ScaleDegreeNumber degree, DegreeDisplayStyle style = DegreeDisplayStyle.Arabic, MusicFormatOptions? options = null)
    { if (!Enum.IsDefined(style)) throw new ArgumentOutOfRangeException(nameof(style)); var settings = Capture(options); return style == DegreeDisplayStyle.Arabic ? degree.Value.ToString(settings.Culture) : Roman(degree.Value, true); }

    public static string Format(HarmonicFunction function, MusicFormatOptions? options = null)
    {
        _ = Capture(options); var upper = function.Quality is HarmonicChordQuality.Major or HarmonicChordQuality.Augmented or HarmonicChordQuality.Other;
        var suffix = function.Quality switch { HarmonicChordQuality.Diminished => "°", HarmonicChordQuality.HalfDiminished => "ø", HarmonicChordQuality.Augmented => "+", _ => "" };
        return string.Concat(Roman(function.Degree.Value, upper), suffix);
    }

    private static Settings Capture(MusicFormatOptions? options)
    {
        var terminology = options?.TerminologyOverride ?? MusicDisplayDefaults.Terminology;
        var glyphs = options?.Accidentals ?? AccidentalGlyphStyle.Unicode;
        if (!Enum.IsDefined(terminology) || !Enum.IsDefined(glyphs)) throw new ArgumentOutOfRangeException(nameof(options));
        return new Settings(terminology, glyphs, options?.Culture ?? CultureInfo.InvariantCulture);
    }

    private static string Pitch(SpelledPitch pitch, Settings settings)
    {
        var names = settings.Terminology == MusicTerminology.French
            ? new[] { "Do", "Ré", "Mi", "Fa", "Sol", "La", "Si" }
            : new[] { "C", "D", "E", "F", "G", "A", "B" };
        return string.Concat(names[(int)pitch.Letter], AccidentalText(pitch.Accidental.Semitones, settings.Glyphs));
    }

    private static string AccidentalText(int value, AccidentalGlyphStyle style) => (value, style) switch
    {
        (0, _) => "",
        (1, AccidentalGlyphStyle.Unicode) => "♯",
        (-1, AccidentalGlyphStyle.Unicode) => "♭",
        (2, AccidentalGlyphStyle.Unicode) => "𝄪",
        (-2, AccidentalGlyphStyle.Unicode) => "𝄫",
        (1, _) => "#",
        (-1, _) => "b",
        (2, _) => "##",
        (-2, _) => "bb",
        _ => string.Concat("[", value.ToString(CultureInfo.InvariantCulture), "]"),
    };

    private static string Abbreviation(string id) => id switch
    { "chord.major" => "", "chord.minor" => "m", "chord.diminished" => "°", "chord.augmented" => "+", "chord.dominant7" => "7", "chord.major7" => "maj7", "chord.minor7" => "m7", "chord.half-diminished7" => "ø7", "chord.diminished7" => "°7", _ => string.Concat("[", id, "]") };

    private static string FullChordName(string id, MusicTerminology terminology)
    {
        var french = id switch { "chord.major" => "majeur", "chord.minor" => "mineur", "chord.diminished" => "diminué", "chord.augmented" => "augmenté", "chord.dominant7" => "septième de dominante", "chord.major7" => "majeur septième", "chord.minor7" => "mineur septième", "chord.half-diminished7" => "semi-diminué septième", "chord.diminished7" => "diminué septième", _ => id };
        var american = id switch { "chord.major" => "major", "chord.minor" => "minor", "chord.diminished" => "diminished", "chord.augmented" => "augmented", "chord.dominant7" => "dominant seventh", "chord.major7" => "major seventh", "chord.minor7" => "minor seventh", "chord.half-diminished7" => "half-diminished seventh", "chord.diminished7" => "diminished seventh", _ => id };
        return terminology == MusicTerminology.French ? french : american;
    }

    private static string ScaleName(string id, MusicTerminology terminology)
    {
        if (id.StartsWith("mode.major.", StringComparison.Ordinal) && int.TryParse(id.AsSpan(11), out var number))
        {
            string[] fr = ["mode de do", "mode de ré", "mode de mi", "mode de fa", "mode de sol", "mode de la", "mode de si"];
            string[] en = ["Ionian", "Dorian", "Phrygian", "Lydian", "Mixolydian", "Aeolian", "Locrian"];
            if (number is >= 1 and <= 7) return terminology == MusicTerminology.French ? fr[number - 1] : en[number - 1];
        }
        return id switch
        {
            "scale.major" => terminology == MusicTerminology.French ? "majeure" : "major",
            "scale.minor.natural" => terminology == MusicTerminology.French ? "mineure naturelle" : "natural minor",
            "scale.minor.harmonic" => terminology == MusicTerminology.French ? "mineure harmonique" : "harmonic minor",
            "scale.minor.melodic.ascending" => terminology == MusicTerminology.French ? "mineure mélodique ascendante" : "ascending melodic minor",
            "scale.pentatonic.major" => terminology == MusicTerminology.French ? "pentatonique majeure" : "major pentatonic",
            "scale.pentatonic.minor" => terminology == MusicTerminology.French ? "pentatonique mineure" : "minor pentatonic",
            _ => id,
        };
    }

    private static string Roman(int value, bool upper)
    { string[] values = ["I", "II", "III", "IV", "V", "VI", "VII"]; var result = values[value - 1]; return upper ? result : result.ToLowerInvariant(); }
    private readonly record struct Settings(MusicTerminology Terminology, AccidentalGlyphStyle Glyphs, IFormatProvider Culture);
}

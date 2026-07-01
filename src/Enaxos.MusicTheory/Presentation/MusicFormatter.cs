using System.Globalization;
using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;
using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Presentation;

/// <summary>Formats domain values without changing their invariant identity or stored spelling.</summary>
public static class MusicFormatter
{
    /// <summary>Formats a spelled pitch using localized note names and configured accidentals.</summary>
    public static string Format(SpelledPitch pitch, MusicFormatOptions? options = null)
    { var settings = Capture(options); return Pitch(pitch, settings); }

    /// <summary>Formats a note as a localized pitch followed by its scientific octave.</summary>
    public static string Format(Note note, MusicFormatOptions? options = null)
    { var settings = Capture(options); return string.Concat(Pitch(note.Pitch, settings), note.Octave.ToString(settings.Culture)); }

    /// <summary>Formats a key tonic and localized major/minor mode name.</summary>
    public static string Format(MusicalKey key, MusicFormatOptions? options = null)
    {
        var settings = Capture(options); var mode = settings.Terminology == MusicTerminology.French
            ? key.Mode == KeyMode.Major ? " majeur" : " mineur"
            : key.Mode == KeyMode.Major ? " major" : " minor";
        return string.Concat(Pitch(key.Tonic, settings), mode);
    }

    /// <summary>Formats a chord using either a canonical suffix or a localized full quality name.</summary>
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

    /// <summary>Attempts to format a useful chord name, using recognition when the chord definition is not directly named.</summary>
    public static bool TryFormatChordName(
        Chord chord,
        out string name,
        ChordNameStyle style = ChordNameStyle.StandardAbbreviation,
        MusicFormatOptions? options = null,
        ChordRecognitionOptions? recognitionOptions = null)
    {
        ArgumentNullException.ThrowIfNull(chord); if (!Enum.IsDefined(style)) throw new ArgumentOutOfRangeException(nameof(style));
        var settings = Capture(options);
        if (TryFormatKnownChord(chord, style, settings, out name))
        {
            return true;
        }

        var candidates = ChordRecognizer.Recognize(
            chord,
            recognitionOptions ?? new ChordRecognitionOptions { MaximumResults = 1 });
        var candidate = candidates.FirstOrDefault();
        if (candidate is null || !TryFormatKnownChord(candidate.Chord, style, settings, out var candidateName))
        {
            name = string.Empty;
            return false;
        }

        name = candidate.InversionNumber is > 0 && candidate.InversionNumber < candidate.Chord.Pitches.Count
            ? string.Concat(candidateName, "/", Pitch(candidate.Chord.Pitches[candidate.InversionNumber.Value], settings))
            : candidateName;
        return true;
    }

    /// <summary>Formats chord tones as a space-separated sequence in formula order.</summary>
    public static string FormatChordPitches(Chord chord, MusicFormatOptions? options = null)
    { ArgumentNullException.ThrowIfNull(chord); var settings = Capture(options); return string.Join(" ", chord.Pitches.Select(pitch => Pitch(pitch, settings))); }

    /// <summary>Formats a known scale definition as a localized display name.</summary>
    public static string Format(ScaleDefinition definition, MusicFormatOptions? options = null)
    { ArgumentNullException.ThrowIfNull(definition); var settings = Capture(options); return ScaleName(definition.Id, settings.Terminology); }

    /// <summary>Formats a scale degree using Arabic or Roman numerals.</summary>
    public static string Format(ScaleDegreeNumber degree, DegreeDisplayStyle style = DegreeDisplayStyle.Arabic, MusicFormatOptions? options = null)
    { if (!Enum.IsDefined(style)) throw new ArgumentOutOfRangeException(nameof(style)); var settings = Capture(options); return style == DegreeDisplayStyle.Arabic ? degree.Value.ToString(settings.Culture) : Roman(degree.Value, true); }

    /// <summary>Formats a harmonic function as a quality-sensitive Roman numeral.</summary>
    public static string Format(HarmonicFunction function, MusicFormatOptions? options = null)
    {
        _ = Capture(options); var upper = function.Quality is HarmonicChordQuality.Major or HarmonicChordQuality.Augmented or HarmonicChordQuality.Other;
        var suffix = function.Quality switch { HarmonicChordQuality.Diminished => "°", HarmonicChordQuality.HalfDiminished => "ø", HarmonicChordQuality.Augmented => "+", _ => "" };
        return string.Concat(Roman(function.Degree.Value, upper), suffix);
    }

    /// <summary>Attempts to format a scale chord Roman numeral when its source scale supports that analysis.</summary>
    public static bool TryFormatRomanNumeral(ScaleChord chord, out string romanNumeral, MusicFormatOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(chord);
        if (chord.SourceScale.Pitches.Count != 7)
        {
            romanNumeral = string.Empty;
            return false;
        }

        romanNumeral = Format(chord.Function, options);
        return true;
    }

    /// <summary>
    /// Captures mutable global defaults once so a single formatting operation remains
    /// internally consistent even if another thread changes the defaults concurrently.
    /// </summary>
    private static Settings Capture(MusicFormatOptions? options)
    {
        var terminology = options?.TerminologyOverride ?? MusicDisplayDefaults.Terminology;
        var glyphs = options?.Accidentals ?? AccidentalGlyphStyle.Unicode;
        if (!Enum.IsDefined(terminology) || !Enum.IsDefined(glyphs)) throw new ArgumentOutOfRangeException(nameof(options));
        return new Settings(terminology, glyphs, options?.Culture ?? CultureInfo.InvariantCulture);
    }

    /// <summary>Formats a letter and accidental using already captured settings.</summary>
    private static string Pitch(SpelledPitch pitch, Settings settings)
    {
        var names = settings.Terminology == MusicTerminology.French
            ? new[] { "Do", "Ré", "Mi", "Fa", "Sol", "La", "Si" }
            : new[] { "C", "D", "E", "F", "G", "A", "B" };
        return string.Concat(names[(int)pitch.Letter], AccidentalText(pitch.Accidental.Semitones, settings.Glyphs));
    }

    /// <summary>Maps conventional accidentals to glyphs and preserves arbitrary alterations numerically.</summary>
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

    /// <summary>Maps supported chord identifiers to canonical lead-sheet suffixes.</summary>
    private static string Abbreviation(string id) => id switch
    { "chord.major" => "", "chord.minor" => "m", "chord.diminished" => "°", "chord.augmented" => "+", "chord.dominant7" => "7", "chord.major7" => "maj7", "chord.minor7" => "m7", "chord.half-diminished7" => "ø7", "chord.diminished7" => "°7", _ => string.Concat("[", id, "]") };

    /// <summary>Formats only chord definitions that have a conventional display name.</summary>
    private static bool TryFormatKnownChord(Chord chord, ChordNameStyle style, Settings settings, out string name)
    {
        var root = Pitch(chord.Root, settings);
        if (style == ChordNameStyle.StandardAbbreviation)
        {
            var suffix = KnownAbbreviation(chord.Definition.Id);
            if (suffix is null)
            {
                name = string.Empty;
                return false;
            }

            name = settings.Terminology == MusicTerminology.French && suffix.Length > 0
                ? string.Concat(root, " ", suffix) : string.Concat(root, suffix);
            return true;
        }

        var fullName = KnownFullChordName(chord.Definition.Id, settings.Terminology);
        if (fullName is null)
        {
            name = string.Empty;
            return false;
        }

        name = string.Concat(root, " ", fullName);
        return true;
    }

    /// <summary>Maps supported chord identifiers to canonical lead-sheet suffixes, or null when unnamed.</summary>
    private static string? KnownAbbreviation(string id) => id switch
    { "chord.major" => "", "chord.minor" => "m", "chord.diminished" => "°", "chord.augmented" => "+", "chord.dominant7" => "7", "chord.major7" => "maj7", "chord.minor7" => "m7", "chord.half-diminished7" => "ø7", "chord.diminished7" => "°7", _ => null };

    /// <summary>Maps supported chord identifiers to localized full quality names.</summary>
    private static string FullChordName(string id, MusicTerminology terminology)
    {
        var french = id switch { "chord.major" => "majeur", "chord.minor" => "mineur", "chord.diminished" => "diminué", "chord.augmented" => "augmenté", "chord.dominant7" => "septième de dominante", "chord.major7" => "majeur septième", "chord.minor7" => "mineur septième", "chord.half-diminished7" => "semi-diminué septième", "chord.diminished7" => "diminué septième", _ => id };
        var american = id switch { "chord.major" => "major", "chord.minor" => "minor", "chord.diminished" => "diminished", "chord.augmented" => "augmented", "chord.dominant7" => "dominant seventh", "chord.major7" => "major seventh", "chord.minor7" => "minor seventh", "chord.half-diminished7" => "half-diminished seventh", "chord.diminished7" => "diminished seventh", _ => id };
        return terminology == MusicTerminology.French ? french : american;
    }

    /// <summary>Maps supported chord identifiers to localized full quality names, or null when unnamed.</summary>
    private static string? KnownFullChordName(string id, MusicTerminology terminology)
    {
        var french = id switch { "chord.major" => "majeur", "chord.minor" => "mineur", "chord.diminished" => "diminué", "chord.augmented" => "augmenté", "chord.dominant7" => "septième de dominante", "chord.major7" => "majeur septième", "chord.minor7" => "mineur septième", "chord.half-diminished7" => "semi-diminué septième", "chord.diminished7" => "diminué septième", _ => null };
        var american = id switch { "chord.major" => "major", "chord.minor" => "minor", "chord.diminished" => "diminished", "chord.augmented" => "augmented", "chord.dominant7" => "dominant seventh", "chord.major7" => "major seventh", "chord.minor7" => "minor seventh", "chord.half-diminished7" => "half-diminished seventh", "chord.diminished7" => "diminished seventh", _ => null };
        return terminology == MusicTerminology.French ? french : american;
    }

    /// <summary>Maps supported scale identifiers to localized names while preserving unknown identifiers.</summary>
    private static string ScaleName(string id, MusicTerminology terminology)
    {
        if (id.StartsWith("mode.major.", StringComparison.Ordinal) && int.TryParse(id.AsSpan(11), out var number))
        {
            string[] fr = ["mode de do", "mode de ré", "mode de mi", "mode de fa", "mode de sol", "mode de la", "mode de si"];
            string[] en = ["Ionian", "Dorian", "Phrygian", "Lydian", "Mixolydian", "Aeolian", "Locrian"];
            if (number is >= 1 and <= 7) return terminology == MusicTerminology.French ? fr[number - 1] : en[number - 1];
        }
        if (id.StartsWith("mode.harmonic-minor.", StringComparison.Ordinal) && int.TryParse(id.AsSpan(20), out var harmonicMinorMode))
        {
            if (harmonicMinorMode is >= 1 and <= 7)
            {
                return terminology == MusicTerminology.French
                    ? string.Concat("mode mineur harmonique ", harmonicMinorMode.ToString(CultureInfo.InvariantCulture))
                    : string.Concat("Harmonic minor mode ", harmonicMinorMode.ToString(CultureInfo.InvariantCulture));
            }
        }
        if (id.StartsWith("mode.melodic-minor.", StringComparison.Ordinal) && int.TryParse(id.AsSpan(19), out var melodicMinorMode))
        {
            if (melodicMinorMode is >= 1 and <= 7)
            {
                return terminology == MusicTerminology.French
                    ? string.Concat("mode mineur mélodique ", melodicMinorMode.ToString(CultureInfo.InvariantCulture))
                    : string.Concat("Melodic minor mode ", melodicMinorMode.ToString(CultureInfo.InvariantCulture));
            }
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

    /// <summary>Formats a validated diatonic value as an upper- or lowercase Roman numeral.</summary>
    private static string Roman(int value, bool upper)
    { string[] values = ["I", "II", "III", "IV", "V", "VI", "VII"]; var result = values[value - 1]; return upper ? result : result.ToLowerInvariant(); }
    /// <summary>Immutable snapshot of all settings used during one formatting operation.</summary>
    private readonly record struct Settings(MusicTerminology Terminology, AccidentalGlyphStyle Glyphs, IFormatProvider Culture);
}

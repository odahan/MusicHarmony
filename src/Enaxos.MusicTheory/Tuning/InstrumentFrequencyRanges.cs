using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tuning;

/// <summary>Provides documented, display-oriented frequency ranges for common instruments.</summary>
public static class InstrumentFrequencyRanges
{
    /// <summary>Gets the default range groups for a broad 20 Hz to 20 kHz spectrum view.</summary>
    public static IReadOnlyList<InstrumentFrequencyRangeGroup> DefaultGroups { get; } =
    [
        new(
            "Vocals",
            [
                Range("Male voice", "C2", "F#5", 8000d),
                Range("Female voice", "F3", "Eb6", 10000d),
            ]),
        new(
            "Percussion",
            [
                new InstrumentFrequencyRange("Kick drum", 50d, 120d, 5000d),
                Range("Timpani", "D2", "C4", 2500d),
                new InstrumentFrequencyRange("Snare", 220d, 391d, 10000d),
                new InstrumentFrequencyRange("Cymbals", 300d, 1000d, 18000d),
            ]),
        new(
            "Brass",
            [
                Range("Tuba", "G0", "C5", 2500d),
                Range("French horn", "B1", "F5", 5000d),
                Range("Bass trombone", "Bb1", "Bb4", 8000d),
                Range("Tenor trombone", "E2", "F5", 8000d),
                Range("Trumpet", "E3", "C6", 9000d),
            ]),
        new(
            "Woodwinds",
            [
                Range("Bassoon", "Bb1", "Eb5", 7000d),
                Range("Tenor sax", "Ab2", "F5", 10000d),
                Range("Alto sax", "Db3", "Bb5", 10000d),
                Range("Clarinet", "D3", "Bb6", 9000d),
                Range("Oboe", "Bb3", "A6", 10000d),
                Range("Flute", "C4", "D7", 9000d),
                Range("Piccolo", "D5", "C8", 12000d),
            ]),
        new(
            "Strings",
            [
                Range("Double bass", "E1", "D5", 5000d),
                Range("Cello", "C2", "C6", 7000d),
                Range("Viola", "C3", "E6", 6000d),
                Range("Violin", "G3", "A7", 16000d),
                Range("Guitar", "E2", "E5", 5000d),
                Range("Harp", "Cb1", "G#7", 6000d),
            ]),
        new(
            "Keys",
            [
                Range("88-key piano", "A0", "C8", 12000d),
                Range("Pipe organ", "C0", "C8", 16000d),
            ]),
    ];

    private static InstrumentFrequencyRange Range(
        string instrumentName,
        string fundamentalLowNote,
        string fundamentalHighNote,
        double harmonicHighFrequency) =>
        new(
            instrumentName,
            Note.Parse(fundamentalLowNote),
            Note.Parse(fundamentalHighNote),
            harmonicHighFrequency);
}

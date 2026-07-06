using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tuning;

/// <summary>Describes an instrument's fundamental range and indicative upper harmonic energy.</summary>
public sealed record InstrumentFrequencyRange
{
    /// <summary>Creates a frequency-only range for instruments without a stable fundamental pitch map.</summary>
    public InstrumentFrequencyRange(
        string instrumentName,
        double fundamentalLowFrequency,
        double fundamentalHighFrequency,
        double harmonicHighFrequency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(instrumentName);
        ValidateFrequency(fundamentalLowFrequency, nameof(fundamentalLowFrequency));
        ValidateFrequency(fundamentalHighFrequency, nameof(fundamentalHighFrequency));
        ValidateFrequency(harmonicHighFrequency, nameof(harmonicHighFrequency));

        InstrumentName = instrumentName;
        FundamentalLowFrequency = fundamentalLowFrequency;
        FundamentalHighFrequency = Math.Max(fundamentalLowFrequency, fundamentalHighFrequency);
        HarmonicHighFrequency = Math.Max(FundamentalHighFrequency, harmonicHighFrequency);
    }

    /// <summary>Creates a range whose fundamentals are defined by notes in the supplied tuning.</summary>
    public InstrumentFrequencyRange(
        string instrumentName,
        Note fundamentalLowNote,
        Note fundamentalHighNote,
        double harmonicHighFrequency,
        ITuningSystem? tuningSystem = null)
        : this(
            instrumentName,
            (tuningSystem ?? DefaultTuning).GetFrequency(fundamentalLowNote),
            (tuningSystem ?? DefaultTuning).GetFrequency(fundamentalHighNote),
            harmonicHighFrequency)
    {
        FundamentalLowNote = fundamentalLowNote;
        FundamentalHighNote = fundamentalHighNote;
    }

    /// <summary>Gets the instrument display name.</summary>
    public string InstrumentName { get; }

    /// <summary>Gets the lowest sounding note when the fundamental range is pitch-defined.</summary>
    public Note? FundamentalLowNote { get; }

    /// <summary>Gets the highest sounding note when the fundamental range is pitch-defined.</summary>
    public Note? FundamentalHighNote { get; }

    /// <summary>Gets the lowest fundamental frequency in hertz.</summary>
    public double FundamentalLowFrequency { get; }

    /// <summary>Gets the highest fundamental frequency in hertz.</summary>
    public double FundamentalHighFrequency { get; }

    /// <summary>Gets an indicative upper frequency for audible harmonic content.</summary>
    public double HarmonicHighFrequency { get; }

    private static EqualTemperament12 DefaultTuning { get; } = new();

    private static void ValidateFrequency(double value, string parameterName)
    {
        if (!double.IsFinite(value) || value <= 0d)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Frequency must be positive and finite.");
        }
    }
}

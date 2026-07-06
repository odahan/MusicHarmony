using Enaxos.MusicTheory.Midi;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tuning;

namespace Enaxos.MusicTheory.Tests.Adapters;

public sealed class TuningAndMidiRulesTests
{
    [Fact]
    public void Default_tuning_maps_A4_to_440_hertz()
    {
        var tuning = new EqualTemperament12();
        Assert.Equal(440d, tuning.GetFrequency(Note.Parse("A4")), 12);
        Assert.Equal(880d, tuning.GetFrequency(Note.Parse("A5")), 12);
        Assert.Equal(220d, tuning.GetFrequency(Note.Parse("A3")), 12);
    }

    [Fact]
    public void Tuning_finds_nearest_note_and_cents_deviation()
    {
        var tuning = new EqualTemperament12();

        var match = tuning.GetNearestNote(445d);

        Assert.Equal("A4", match.Note.ToString());
        Assert.Equal(440d, match.Frequency, 12);
        Assert.InRange(match.CentsDeviation, 19.56d, 19.57d);
    }

    [Fact]
    public void Instrument_frequency_ranges_expose_documented_sounding_ranges()
    {
        var ranges = InstrumentFrequencyRanges.DefaultGroups.SelectMany(group => group.Ranges);

        var violin = ranges.Single(range => range.InstrumentName == "Violin");
        Assert.Equal("G3", violin.FundamentalLowNote?.ToString());
        Assert.Equal("A7", violin.FundamentalHighNote?.ToString());
        Assert.Equal(195.997717990875d, violin.FundamentalLowFrequency, 12);
        Assert.Equal(3520d, violin.FundamentalHighFrequency, 12);

        var piano = ranges.Single(range => range.InstrumentName == "88-key piano");
        Assert.Equal("A0", piano.FundamentalLowNote?.ToString());
        Assert.Equal("C8", piano.FundamentalHighNote?.ToString());
        Assert.Equal(27.5d, piano.FundamentalLowFrequency, 12);
        Assert.Equal(4186.009044809578d, piano.FundamentalHighFrequency, 12);
    }

    [Fact]
    public void Midi_adapter_uses_C4_as_60_and_round_trips_all_values()
    {
        Assert.Equal(60, MidiNote.ToNumber(Note.Parse("C4")));
        for (var number = 0; number <= 127; number++)
        {
            var note = MidiNote.FromNumber(number);
            Assert.Equal(number, MidiNote.ToNumber(note));
        }
    }

    [Fact]
    public void Midi_spelling_preference_is_explicit()
    {
        Assert.Equal("C#4", MidiNote.FromNumber(61, EnharmonicPreference.PreferSharps).ToString());
        Assert.Equal("Db4", MidiNote.FromNumber(61, EnharmonicPreference.PreferFlats).ToString());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(128)]
    public void Midi_range_is_enforced(int number)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MidiNote.FromNumber(number));
    }
}

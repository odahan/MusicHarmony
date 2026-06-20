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

using Enaxos.MusicTheory.Primitives;

namespace Enaxos.MusicTheory.Tests.Primitives;

public sealed class AccidentalRulesTests
{
    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(-12)]
    [InlineData(12)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Accidental_preserves_its_exact_signed_displacement(int semitones)
    {
        Assert.Equal(semitones, Accidental.FromSemitones(semitones).Semitones);
    }

    [Fact]
    public void Named_accidentals_have_the_normative_displacements()
    {
        Assert.Equal(-2, Accidental.DoubleFlat.Semitones);
        Assert.Equal(-1, Accidental.Flat.Semitones);
        Assert.Equal(0, Accidental.Natural.Semitones);
        Assert.Equal(1, Accidental.Sharp.Semitones);
        Assert.Equal(2, Accidental.DoubleSharp.Semitones);
    }
}

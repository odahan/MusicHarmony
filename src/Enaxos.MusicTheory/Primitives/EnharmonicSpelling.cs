namespace Enaxos.MusicTheory.Primitives;

internal static class EnharmonicSpelling
{
    internal static SpelledPitch For(PitchClass pitchClass, EnharmonicPreference preference)
    {
        if (!Enum.IsDefined(preference))
        {
            throw new ArgumentOutOfRangeException(nameof(preference));
        }

        var sharps = new[]
        {
            new SpelledPitch(NoteLetter.C, Accidental.Natural),
            new SpelledPitch(NoteLetter.C, Accidental.Sharp),
            new SpelledPitch(NoteLetter.D, Accidental.Natural),
            new SpelledPitch(NoteLetter.D, Accidental.Sharp),
            new SpelledPitch(NoteLetter.E, Accidental.Natural),
            new SpelledPitch(NoteLetter.F, Accidental.Natural),
            new SpelledPitch(NoteLetter.F, Accidental.Sharp),
            new SpelledPitch(NoteLetter.G, Accidental.Natural),
            new SpelledPitch(NoteLetter.G, Accidental.Sharp),
            new SpelledPitch(NoteLetter.A, Accidental.Natural),
            new SpelledPitch(NoteLetter.A, Accidental.Sharp),
            new SpelledPitch(NoteLetter.B, Accidental.Natural),
        };
        var flats = new[]
        {
            sharps[0], new SpelledPitch(NoteLetter.D, Accidental.Flat), sharps[2],
            new SpelledPitch(NoteLetter.E, Accidental.Flat), sharps[4], sharps[5],
            new SpelledPitch(NoteLetter.G, Accidental.Flat), sharps[7],
            new SpelledPitch(NoteLetter.A, Accidental.Flat), sharps[9],
            new SpelledPitch(NoteLetter.B, Accidental.Flat), sharps[11],
        };
        return (preference == EnharmonicPreference.PreferFlats ? flats : sharps)[pitchClass.Value];
    }
}

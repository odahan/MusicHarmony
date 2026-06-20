namespace Enaxos.MusicTheory.Primitives;

/// <summary>Maps normalized pitch classes to deterministic conventional spellings.</summary>
internal static class EnharmonicSpelling
{
    /// <summary>Returns the canonical spelling selected by the requested accidental preference.</summary>
    internal static SpelledPitch For(PitchClass pitchClass, EnharmonicPreference preference)
    {
        if (!Enum.IsDefined(preference))
        {
            throw new ArgumentOutOfRangeException(nameof(preference));
        }

        // The natural notes are shared by both tables. FewestAccidentals intentionally
        // falls back to the sharp table for the enharmonic ties on black keys.
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

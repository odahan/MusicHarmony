using System.Collections.ObjectModel;

namespace Enaxos.MusicTheory.Primitives;

/// <summary>Exposes conventional spellings for normalized pitch classes.</summary>
public static class PitchClassSpellings
{
    /// <summary>Gets natural spellings in diatonic order from C through B.</summary>
    public static IReadOnlyList<SpelledPitch> NaturalPitches { get; } =
        Array.AsReadOnly(
        [
            new SpelledPitch(NoteLetter.C, Accidental.Natural),
            new SpelledPitch(NoteLetter.D, Accidental.Natural),
            new SpelledPitch(NoteLetter.E, Accidental.Natural),
            new SpelledPitch(NoteLetter.F, Accidental.Natural),
            new SpelledPitch(NoteLetter.G, Accidental.Natural),
            new SpelledPitch(NoteLetter.A, Accidental.Natural),
            new SpelledPitch(NoteLetter.B, Accidental.Natural),
        ]);

    /// <summary>Returns the conventional spelling selected by the requested accidental preference.</summary>
    public static SpelledPitch For(
        PitchClass pitchClass,
        EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals) =>
        EnharmonicSpelling.For(pitchClass, preference);

    /// <summary>Returns one spelling for every chromatic pitch class from C through B.</summary>
    public static IReadOnlyList<SpelledPitch> Chromatic(
        EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)
    {
        if (!Enum.IsDefined(preference))
        {
            throw new ArgumentOutOfRangeException(nameof(preference));
        }

        var pitches = Enumerable
            .Range(0, PitchClass.Count)
            .Select(index => For(PitchClass.FromChromaticIndex(index), preference))
            .ToArray();

        return Array.AsReadOnly(pitches);
    }
}

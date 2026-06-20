namespace Enaxos.MusicTheory.Primitives;

/// <summary>Specifies how a pitch class should be converted to a written pitch when no spelling is supplied.</summary>
public enum EnharmonicPreference
{
    /// <summary>Uses the canonical spelling with the smallest accidental.</summary>
    FewestAccidentals,

    /// <summary>Uses sharps for black-key pitch classes.</summary>
    PreferSharps,

    /// <summary>Uses flats for black-key pitch classes.</summary>
    PreferFlats,
}

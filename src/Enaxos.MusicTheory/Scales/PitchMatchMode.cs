namespace Enaxos.MusicTheory.Scales;

/// <summary>Controls whether pitch collection comparisons preserve spelling or use enharmonic equivalence.</summary>
public enum PitchMatchMode
{
    /// <summary>Compares normalized pitch classes, so enharmonic spellings such as C# and Db match.</summary>
    PitchClass,
    /// <summary>Compares the exact written pitch spelling, including letter and accidental.</summary>
    ExactSpelling,
}

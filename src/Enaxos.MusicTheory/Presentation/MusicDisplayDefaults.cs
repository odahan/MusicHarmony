namespace Enaxos.MusicTheory.Presentation;

/// <summary>Stores thread-safe process-wide defaults used when a formatter call has no override.</summary>
public static class MusicDisplayDefaults
{
    // The enum is stored as an integer because Volatile has direct atomic overloads for int.
    private static int _terminology = (int)MusicTerminology.French;

    /// <summary>Gets or atomically sets the process-wide default terminology.</summary>
    public static MusicTerminology Terminology
    {
        get => (MusicTerminology)Volatile.Read(ref _terminology);
        set
        {
            if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Volatile.Write(ref _terminology, (int)value);
        }
    }
}

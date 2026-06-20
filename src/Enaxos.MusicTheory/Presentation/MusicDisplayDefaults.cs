namespace Enaxos.MusicTheory.Presentation;

public static class MusicDisplayDefaults
{
    private static int _terminology = (int)MusicTerminology.French;
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

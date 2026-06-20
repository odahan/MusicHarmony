using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Circle;

public sealed record CircleKeyPair
{
    public CircleKeyPair(MusicalKey major, MusicalKey relativeMinor, KeySignature signature)
    {
        Major = major; RelativeMinor = relativeMinor; Signature = signature;
        if (KeySignature.For(major).Fifths != signature.Fifths || KeySignature.For(relativeMinor).Fifths != signature.Fifths)
            throw new ArgumentException("Major and relative minor must share the supplied signature.");
    }
    public MusicalKey Major { get; }
    public MusicalKey RelativeMinor { get; }
    public KeySignature Signature { get; }
}

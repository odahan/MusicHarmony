using Enaxos.MusicTheory.Tonality;

namespace Enaxos.MusicTheory.Circle;

/// <summary>Groups a major key, its relative minor, and their shared key signature.</summary>
public sealed record CircleKeyPair
{
    /// <summary>Creates a validated relative-key pair.</summary>
    public CircleKeyPair(MusicalKey major, MusicalKey relativeMinor, KeySignature signature)
    {
        Major = major; RelativeMinor = relativeMinor; Signature = signature;
        if (KeySignature.For(major).Fifths != signature.Fifths || KeySignature.For(relativeMinor).Fifths != signature.Fifths)
            throw new ArgumentException("Major and relative minor must share the supplied signature.");
    }
    /// <summary>Gets the major key represented by this spelling.</summary>
    public MusicalKey Major { get; }

    /// <summary>Gets the relative minor sharing the signature.</summary>
    public MusicalKey RelativeMinor { get; }

    /// <summary>Gets the key signature shared by both keys.</summary>
    public KeySignature Signature { get; }
}

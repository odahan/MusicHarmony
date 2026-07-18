# Enaxos MusicTheory

[Version française](README.FR.MD)

An independent .NET 8 library for representing and analyzing music theory
concepts while preserving note spelling.

## NuGet package

`Enaxos.MusicTheory` is available from [NuGet.org](https://www.nuget.org/packages/Enaxos.MusicTheory).

```powershell
dotnet add package Enaxos.MusicTheory --version 1.2.5
```

The package can also be installed through the [NuGet Package Manager](https://www.nuget.org/packages/Enaxos.MusicTheory) or referenced directly:

```xml
<PackageReference Include="Enaxos.MusicTheory" Version="1.2.5" />
```

This project is a clean-room rewrite. Its behavior is defined by the
[domain specification](docs/DOMAIN_SPECIFICATION.md) and the
[public API proposal](docs/PUBLIC_API_PROPOSAL.md). The process and independent
references used by the implementation are recorded in
[PROVENANCE.md](PROVENANCE.md).

## Features

- notes, pitches, accidentals, and enharmonic equivalence;
- intervals and transpositions that preserve diatonic spelling;
- scale, mode, and pentatonic construction;
- keys, key signatures, and harmonic functions;
- chord construction, inversions, and chord or scale recognition;
- the circle of fifths and presentation geometry;
- twelve-tone equal temperament and MIDI conversion;
- French or American music formatting.

## Requirements

- .NET 8 SDK;
- the SDK version selected by the repository is specified in `global.json`.

## Build and Test

```powershell
dotnet build Enaxos.MusicTheory.sln --configuration Release
dotnet test Enaxos.MusicTheory.sln --no-build --configuration Release
```

## Examples

### Construct a scale and a chord

```csharp
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;

var cMajor = Scale.Create(SpelledPitch.Parse("C"), StandardScales.Major);
var dDorian = Scale.Create(SpelledPitch.Parse("D"), StandardScales.Dorian);
var cMajorSeventh = Chord.Create(SpelledPitch.Parse("C"), StandardChords.MajorSeventh);

Console.WriteLine(string.Join(", ", cMajor.Pitches));         // C, D, E, F, G, A, B
Console.WriteLine(string.Join(", ", dDorian.Pitches));         // D, E, F, G, A, B, C
Console.WriteLine(string.Join(", ", cMajorSeventh.Pitches));   // C, E, G, B
```

### Recognize a chord and its inversion

The recognizer returns ranked candidates, preserves the observed spellings when
possible, and uses the lowest note as inversion evidence.

```csharp
using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Primitives;

var notes = new[] { Note.Parse("E4"), Note.Parse("G4"), Note.Parse("C5") };

if (ChordRecognizer.TryRecognizeBest(notes, out var candidate))
{
    Console.WriteLine(candidate.Chord.Symbol);     // C
    Console.WriteLine(candidate.InversionNumber);  // 1 (first inversion)
    Console.WriteLine(candidate.Confidence);       // 1
}
```

`ChordRecognizer.Recognize` supports all standard chord definitions: major,
minor, diminished and augmented triads; `sus2` and `sus4`; major and minor
sixths; dominant, major, minor, half-diminished, and diminished sevenths; and
dominant, major, and minor ninths, elevenths, and thirteenths. Use
`ChordRecognitionOptions` to allow missing or added tones, limit the results,
or disable enharmonic matching.

### Recognize a scale

Scale candidates expose their matched, missing, and outside pitches, score, and
relative probability. By default, recognition searches the major modes and the
seven modes of harmonic and ascending melodic minor. Pentatonic and exotic
catalogs are opt-in.

```csharp
using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Primitives;

var notes = new[]
{
    Note.Parse("C4"), Note.Parse("D4"), Note.Parse("E4"),
    Note.Parse("G4"), Note.Parse("A4"),
};

var candidates = ScaleRecognizer.FindCandidates(
    notes,
    new ScaleRecognitionOptions
    {
        IncludePentatonicCandidates = true,
        MaximumResults = 3,
    });

var best = candidates[0];
Console.WriteLine($"{best.Scale.Tonic}: {best.Scale.Definition.Id}");
// C: scale.pentatonic.major
Console.WriteLine(best.RelativeProbability);
```

For the complete built-in search space, also set
`IncludeExoticCandidates = true`. A custom `Catalog` can restrict recognition
to exactly the scale definitions required by an application.

### MIDI and tuning

```csharp
using Enaxos.MusicTheory.Midi;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tuning;

var concertA = Note.Parse("A4");
var frequency = new EqualTemperament12().GetFrequency(concertA); // 440 Hz
var midiNumber = MidiNote.ToNumber(concertA);                    // 69
```

## Built-in scale catalog

Every definition below can be constructed with `Scale.Create(tonic, definition)`.
The default recognition catalog contains the 21 modal definitions marked
**default**. Add `IncludePentatonicCandidates` and/or
`IncludeExoticCandidates` to extend it; together, they make all 48 definitions
below searchable.

| Family | Scale definitions | Recognition |
| --- | --- | --- |
| Major modes | Ionian (major), Dorian, Phrygian, Lydian, Mixolydian, Aeolian (natural minor), Locrian | **default** |
| Harmonic minor | Harmonic minor, and its modes 2 through 7 (`mode.harmonic-minor.1` to `.7`) | **default** |
| Ascending melodic minor | Ascending melodic minor, and its modes 2 through 7 (`mode.melodic-minor.1` to `.7`) | **default** |
| Pentatonic | Major pentatonic, minor pentatonic | `IncludePentatonicCandidates` |
| Symmetric and jazz | Whole tone, octatonic whole-half, octatonic half-whole, augmented | `IncludeExoticCandidates` |
| Blues and bebop | Minor blues, major blues, dominant bebop, major bebop, Dorian bebop | `IncludeExoticCandidates` |
| Rare major/minor families | Harmonic major, double harmonic major, Hungarian minor, Ukrainian Dorian, Neapolitan major, Neapolitan minor | `IncludeExoticCandidates` |
| Westernized oriental | Persian, Arabian, Spanish Phrygian, Oriental, Egyptian | `IncludeExoticCandidates` |
| Japanese pentatonic | Hirajoshi, Insen, Iwato, Yo, Kumoi | `IncludeExoticCandidates` |

`StandardScales` also exposes the equivalent named definitions `Major`,
`NaturalMinor`, `HarmonicMinor`, and `MelodicMinorAscending`; the recognition
catalog uses the modal identifiers shown above so that a candidate's modal
interpretation remains explicit. `ExoticScales.All` and `ModeCatalog.Standard`
provide the corresponding programmatic catalogs.

## Repository Structure

- `src/Enaxos.MusicTheory`: library with no third-party runtime dependencies;
- `tests/Enaxos.MusicTheory.Tests`: behavior tests;
- `docs/`: normative project documents.

## License

This repository is public, but its code is not open source. Private use is
permitted without restriction. Commercial use and any modification require the
author's prior written permission. Redistribution also requires permission.

See [LICENSE.MD](LICENSE.MD) for the complete terms.

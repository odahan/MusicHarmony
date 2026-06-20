# Enaxos MusicTheory

[Version française](README.FR.MD)

An independent .NET 8 library for representing and analyzing music theory
concepts while preserving note spelling.

This project is a clean-room rewrite. Its behavior is defined by the
[domain specification](docs/DOMAIN_SPECIFICATION.md) and the
[public API proposal](docs/PUBLIC_API_PROPOSAL.md). The process and independent
references used by the implementation are recorded in
[PROVENANCE.md](PROVENANCE.md).

## Features

- notes, pitches, accidentals, and enharmonic equivalence;
- intervals and transpositions that preserve diatonic spelling;
- scales, modes, and pentatonic scales;
- keys, key signatures, and harmonic functions;
- chords, inversions, and chord or scale recognition;
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

## Example

```csharp
using Enaxos.MusicTheory.Harmony;
using Enaxos.MusicTheory.Midi;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Scales;
using Enaxos.MusicTheory.Tuning;

var tonic = SpelledPitch.Parse("C");
var scale = Scale.Create(tonic, StandardScales.Major);
var chord = Chord.Create(tonic, StandardChords.MajorSeventh);

var concertA = Note.Parse("A4");
var frequency = new EqualTemperament12().GetFrequency(concertA); // 440 Hz
var midiNumber = MidiNote.ToNumber(concertA);                    // 69

Console.WriteLine(string.Join(", ", scale.Pitches));
Console.WriteLine(string.Join(", ", chord.Pitches));
```

## Repository Structure

- `src/Enaxos.MusicTheory`: library with no third-party runtime dependencies;
- `tests/Enaxos.MusicTheory.Tests`: behavior tests;
- `docs/`: normative project documents.

## License

This repository is public, but its code is not open source. Private use is
permitted without restriction. Commercial use and any modification require the
author's prior written permission. Redistribution also requires permission.

See [LICENSE.MD](LICENSE.MD) for the complete terms.

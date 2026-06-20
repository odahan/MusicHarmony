# Enaxos MusicTheory

[English version](README.MD)

Bibliothèque .NET 8 indépendante pour représenter et analyser des notions de
théorie musicale tout en conservant l’orthographe des notes.

Version actuelle : **1.0**.

Le projet est une réécriture *clean-room*. Son comportement est défini par les
[spécifications du domaine](docs/DOMAIN_SPECIFICATION.md) et la
[proposition d’API publique](docs/PUBLIC_API_PROPOSAL.md). La démarche et les
références indépendantes utilisées sont consignées dans
[PROVENANCE.md](PROVENANCE.md).

## Fonctionnalités

- notes, hauteurs, altérations et équivalences enharmoniques ;
- intervalles et transpositions respectant l’orthographe diatonique ;
- gammes, modes et pentatoniques ;
- tonalités, armures et fonctions harmoniques ;
- accords, renversements et reconnaissance d’accords ou de gammes ;
- cercle des quintes et géométrie de présentation ;
- tempérament égal à douze sons et conversion MIDI ;
- formatage musical français ou américain.

## Prérequis

- SDK .NET 8 ;
- la version du SDK retenue par le dépôt est indiquée dans `global.json`.

## Compiler et tester

```powershell
dotnet build Enaxos.MusicTheory.sln --configuration Release
dotnet test Enaxos.MusicTheory.sln --no-build --configuration Release
```

## Exemple

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

## Structure du dépôt

- `src/Enaxos.MusicTheory` : bibliothèque sans dépendance d’exécution tierce ;
- `tests/Enaxos.MusicTheory.Tests` : tests de comportement ;
- `docs/` : documents normatifs du projet.

## Licence

Ce dépôt est public mais son code n’est pas open source. L’utilisation privée
est autorisée sans restriction. Toute utilisation commerciale et toute
modification nécessitent l’autorisation préalable et écrite de l’auteur. La
redistribution est également soumise à autorisation.

Consultez les conditions complètes dans [LICENSE.FR.MD](LICENSE.FR.MD).

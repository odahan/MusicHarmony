# Enaxos MusicTheory

[English version](README.MD)

Bibliothèque .NET 8 indépendante pour représenter et analyser des notions de
théorie musicale tout en conservant l’orthographe des notes.

## Paquet NuGet

`Enaxos.MusicTheory` est disponible sur [NuGet.org](https://www.nuget.org/packages/Enaxos.MusicTheory).

```powershell
dotnet add package Enaxos.MusicTheory --version 1.2.5
```

Le paquet peut aussi être installé depuis le [gestionnaire de paquets NuGet](https://www.nuget.org/packages/Enaxos.MusicTheory) ou référencé directement :

```xml
<PackageReference Include="Enaxos.MusicTheory" Version="1.2.5" />
```

Le projet est une réécriture *clean-room*. Son comportement est défini par les
[spécifications du domaine](docs/DOMAIN_SPECIFICATION.FR.md) et la
[proposition d’API publique](docs/PUBLIC_API_PROPOSAL.FR.md). La démarche et les
références indépendantes utilisées sont consignées dans
[PROVENANCE.md](PROVENANCE.md).

## Fonctionnalités

- notes, hauteurs, altérations et équivalences enharmoniques ;
- intervalles et transpositions respectant l’orthographe diatonique ;
- construction de gammes, de modes et de pentatoniques ;
- tonalités, armures et fonctions harmoniques ;
- construction d’accords, renversements et reconnaissance d’accords ou de gammes ;
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

## Exemples

### Construire une gamme et un accord

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

### Reconnaître un accord et son renversement

Le reconnaisseur renvoie des candidats classés, préserve les orthographes
observées quand cela est possible et utilise la note la plus grave comme indice
de renversement.

```csharp
using Enaxos.MusicTheory.Analysis;
using Enaxos.MusicTheory.Primitives;

var notes = new[] { Note.Parse("E4"), Note.Parse("G4"), Note.Parse("C5") };

if (ChordRecognizer.TryRecognizeBest(notes, out var candidate))
{
    Console.WriteLine(candidate.Chord.Symbol);     // C
    Console.WriteLine(candidate.InversionNumber);  // 1 (premier renversement)
    Console.WriteLine(candidate.Confidence);       // 1
}
```

`ChordRecognizer.Recognize` reconnaît toutes les définitions d’accords
standards : triades majeure, mineure, diminuée et augmentée ; `sus2` et `sus4` ;
sixtes majeure et mineure ; septièmes de dominante, majeure, mineure,
semi-diminuée et diminuée ; ainsi que neuvièmes, onzièmes et treizièmes de
dominante, majeures et mineures. Utilisez `ChordRecognitionOptions` pour
autoriser des notes absentes ou ajoutées, limiter les résultats ou désactiver
la correspondance enharmonique.

### Reconnaître une gamme

Les candidats de gamme exposent les hauteurs correspondantes, absentes et hors
gamme, ainsi que leur score et leur probabilité relative. Par défaut, la
reconnaissance parcourt les modes majeurs et les sept modes des gammes mineures
harmonique et mélodique ascendante. Les catalogues pentatoniques et exotiques
sont optionnels.

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

Pour parcourir tout l’espace de recherche intégré, définissez également
`IncludeExoticCandidates = true`. Un `Catalog` personnalisé permet de
restreindre la reconnaissance aux définitions de gammes requises par une
application.

### MIDI et tempérament

```csharp
using Enaxos.MusicTheory.Midi;
using Enaxos.MusicTheory.Primitives;
using Enaxos.MusicTheory.Tuning;

var concertA = Note.Parse("A4");
var frequency = new EqualTemperament12().GetFrequency(concertA); // 440 Hz
var midiNumber = MidiNote.ToNumber(concertA);                    // 69
```

## Catalogue de gammes intégré

Chaque définition ci-dessous peut être construite avec
`Scale.Create(tonic, definition)`. Le catalogue de reconnaissance par défaut
contient les 21 définitions modales marquées **par défaut**. Ajoutez
`IncludePentatonicCandidates` et/ou `IncludeExoticCandidates` pour l’étendre ;
ensemble, ces options rendent les 48 définitions ci-dessous recherchables.

| Famille | Définitions de gammes | Reconnaissance |
| --- | --- | --- |
| Modes majeurs | Ionien (majeur), dorien, phrygien, lydien, mixolydien, éolien (mineur naturel), locrien | **par défaut** |
| Mineure harmonique | Mineure harmonique et ses modes 2 à 7 (`mode.harmonic-minor.1` à `.7`) | **par défaut** |
| Mineure mélodique ascendante | Mineure mélodique ascendante et ses modes 2 à 7 (`mode.melodic-minor.1` à `.7`) | **par défaut** |
| Pentatoniques | Pentatonique majeure, pentatonique mineure | `IncludePentatonicCandidates` |
| Symétriques et jazz | Par tons entiers, octatonique ton-demi-ton, octatonique demi-ton-ton, augmentée | `IncludeExoticCandidates` |
| Blues et bebop | Blues mineure, blues majeure, bebop dominante, bebop majeure, bebop dorienne | `IncludeExoticCandidates` |
| Familles majeure/mineure rares | Majeure harmonique, majeure double harmonique, mineure hongroise, dorienne ukrainienne, majeure napolitaine, mineure napolitaine | `IncludeExoticCandidates` |
| Orientales occidentalisées | Persane, arabe, phrygienne espagnole, orientale, égyptienne | `IncludeExoticCandidates` |
| Pentatoniques japonaises | Hirajoshi, Insen, Iwato, Yo, Kumoi | `IncludeExoticCandidates` |

`StandardScales` expose aussi les définitions nommées équivalentes `Major`,
`NaturalMinor`, `HarmonicMinor` et `MelodicMinorAscending` ; le catalogue de
reconnaissance utilise les identifiants modaux indiqués ci-dessus afin de rendre
explicite l’interprétation modale d’un candidat. `ExoticScales.All` et
`ModeCatalog.Standard` fournissent les catalogues programmatiques correspondants.

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

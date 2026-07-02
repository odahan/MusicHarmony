# Enaxos Music Theory — Proposition d'API publique .NET 8

Statut : proposition initiale  
Assembly proposé : `Enaxos.MusicTheory`  
Framework : `net8.0`

Cette API privilégie les valeurs immuables, les calculs purs, les erreurs explicites et les noms du domaine. Les signatures ci-dessous constituent un contrat de conception ; elles ne sont pas encore une implémentation.

## 1. Organisation

```text
Enaxos.MusicTheory
├── Primitives       lettres, altérations, hauteurs, notes
├── Intervals        intervalles et transposition
├── Formulas         degrés relatifs partagés
├── Scales           formules et gammes
├── Tonality         tonalités et armures
├── Harmony          accords, voicings et renversements
├── Analysis         reconnaissance d'accords et de gammes
├── Tuning           fréquences et tempéraments
├── Midi             conversions MIDI explicites
├── Circle           cercle des quintes et relations
├── Presentation     formatage sans localisation globale
└── Geometry         géométrie normalisée du cercle
```

Le package n'a aucune dépendance envers un framework UI. Les applications WPF, WinUI, MAUI, Avalonia, Blazor ou autres adaptent les coordonnées et libellés à leurs propres types.

## 2. Principes d'API

- `nullable` activé.
- Types de valeur immuables pour les primitives.
- Types `sealed` immuables pour les agrégats contenant des collections.
- `IReadOnlyList<T>` en sortie ; aucune collection mutable exposée.
- Tout `IEnumerable<T>` reçu est copié défensivement avant validation.
- Les agrégats avec collections utilisent une égalité explicite ; ils ne dépendent pas de l'égalité par référence d'une liste produite par défaut par `record`.
- Pas de constructeur à partir d'une chaîne : `Parse` et `TryParse` sont explicites.
- Pas de booléen ambigu comme `preferSharp`; utilisation d'énumérations ou de stratégies.
- Aucun état global n'influence les calculs. Seule la terminologie d'affichage par défaut est globale, atomique et surchargeable localement.
- Les opérations qui ont besoin d'un contexte le reçoivent en paramètre.
- Les services de formatage, d'accordage et MIDI restent séparés du modèle harmonique.

## 3. Primitives

```csharp
namespace Enaxos.MusicTheory.Primitives;

public enum NoteLetter
{
    C, D, E, F, G, A, B
}

public enum EnharmonicPreference
{
    FewestAccidentals,
    PreferSharps,
    PreferFlats
}

public readonly record struct Accidental
{
    public int Semitones { get; }

    public static Accidental DoubleFlat { get; }
    public static Accidental Flat { get; }
    public static Accidental Natural { get; }
    public static Accidental Sharp { get; }
    public static Accidental DoubleSharp { get; }

    public static Accidental FromSemitones(int semitones);
}

public readonly record struct PitchClass
{
    public int Value { get; }

    public static PitchClass FromChromaticIndex(int value);
    public int DistanceUpTo(PitchClass target);
}

public readonly record struct SpelledPitch
{
    public NoteLetter Letter { get; }
    public Accidental Accidental { get; }
    public PitchClass PitchClass { get; }

    public SpelledPitch(NoteLetter letter, Accidental accidental);

    public bool IsEnharmonicWith(SpelledPitch other);

    public static SpelledPitch Parse(string text);
    public static bool TryParse(
        ReadOnlySpan<char> text,
        out SpelledPitch result);
}

public readonly record struct Note
{
    public SpelledPitch Pitch { get; }
    public int Octave { get; }
    public int AbsoluteSemitone { get; }

    public Note(SpelledPitch pitch, int octave);

    public bool IsEnharmonicWith(Note other);

    public static Note Parse(string text);
    public static bool TryParse(
        ReadOnlySpan<char> text,
        out Note result);
}
```

Usage :

```csharp
var cSharp = SpelledPitch.Parse("C#");
var dFlat = SpelledPitch.Parse("D♭");

bool sameWriting = cSharp == dFlat;                 // false
bool sameSound = cSharp.IsEnharmonicWith(dFlat);   // true en 12-TET
```

## 4. Intervalles et transposition

Le stockage canonique `(Number, Semitones)` permet de représenter les qualités multiples sans multiplier les cas particuliers. `Quality` est calculée.

```csharp
namespace Enaxos.MusicTheory.Intervals;

public enum IntervalDirection
{
    Ascending,
    Descending
}

public enum IntervalQualityKind
{
    Diminished,
    Minor,
    Perfect,
    Major,
    Augmented
}

public readonly record struct IntervalQuality
{
    public IntervalQualityKind Kind { get; }

    // 1 pour augmenté/diminué, 2 pour doublement augmenté/diminué, etc.
    public int Degree { get; }
}

public readonly record struct Interval
{
    public int Number { get; }
    public int Semitones { get; }
    public IntervalQuality Quality { get; }
    public bool IsCompound { get; }

    public static Interval Create(
        int number,
        IntervalQuality quality);

    public static Interval FromDistances(
        int diatonicNumber,
        int semitones);

    public static Interval Between(
        Note from,
        Note to,
        IntervalDirection direction = IntervalDirection.Ascending);

    public Interval InvertSimple();
}

public static class CommonIntervals
{
    public static Interval PerfectUnison { get; }
    public static Interval MinorSecond { get; }
    public static Interval MajorSecond { get; }
    public static Interval MinorThird { get; }
    public static Interval MajorThird { get; }
    public static Interval PerfectFourth { get; }
    public static Interval AugmentedFourth { get; }
    public static Interval DiminishedFifth { get; }
    public static Interval PerfectFifth { get; }
    public static Interval MinorSixth { get; }
    public static Interval MajorSixth { get; }
    public static Interval MinorSeventh { get; }
    public static Interval MajorSeventh { get; }
    public static Interval PerfectOctave { get; }
}

public static class Transposition
{
    public static SpelledPitch Transpose(
        SpelledPitch source,
        Interval interval,
        IntervalDirection direction = IntervalDirection.Ascending);

    public static Note Transpose(
        Note source,
        Interval interval,
        IntervalDirection direction = IntervalDirection.Ascending);
}
```

Usage :

```csharp
var result = Transposition.Transpose(
    Note.Parse("C#4"),
    CommonIntervals.MajorThird);

// E#4, sans simplification enharmonique implicite.
```

## 5. Formules, gammes et modes

```csharp
namespace Enaxos.MusicTheory.Formulas;

public readonly record struct FormulaDegree
{
    public int Number { get; }
    public int Alteration { get; }

    public FormulaDegree(int number, int alteration = 0);
}
```

`FormulaDegree` est partagé par les gammes et les accords afin d'éviter une dépendance de l'harmonie envers le module des gammes.

```csharp
namespace Enaxos.MusicTheory.Scales;

public sealed class ScaleDefinition : IEquatable<ScaleDefinition>
{
    public string Id { get; }
    public IReadOnlyList<FormulaDegree> Degrees { get; }

    public ScaleDefinition(
        string id,
        IEnumerable<FormulaDegree> degrees);
}

public sealed class Scale : IEquatable<Scale>
{
    public SpelledPitch Tonic { get; }
    public ScaleDefinition Definition { get; }
    public IReadOnlyList<SpelledPitch> Pitches { get; }

    public SpelledPitch Degree(int number);

    public static Scale Create(
        SpelledPitch tonic,
        ScaleDefinition definition);
}

public static class StandardScales
{
    public static ScaleDefinition Major { get; }
    public static ScaleDefinition NaturalMinor { get; }
    public static ScaleDefinition HarmonicMinor { get; }
    public static ScaleDefinition MelodicMinorAscending { get; }
    public static ScaleDefinition Ionian { get; }
    public static ScaleDefinition Dorian { get; }
    public static ScaleDefinition Phrygian { get; }
    public static ScaleDefinition Lydian { get; }
    public static ScaleDefinition Mixolydian { get; }
    public static ScaleDefinition Aeolian { get; }
    public static ScaleDefinition Locrian { get; }
    public static ScaleDefinition MajorPentatonic { get; }
    public static ScaleDefinition MinorPentatonic { get; }
}

public enum ScaleFamily
{
    Major,
    HarmonicMinor,
    MelodicMinor,
    Pentatonic,
    Custom
}

public sealed class ModeCatalog
{
    public static ModeCatalog Standard { get; }

    public IReadOnlyList<ScaleDefinition> MajorModes { get; }
    public IReadOnlyList<ScaleDefinition> NaturalMinorModes { get; }
    public IReadOnlyList<ScaleDefinition> HarmonicMinorModes { get; }
    public IReadOnlyList<ScaleDefinition> MelodicMinorModes { get; }
    public IReadOnlyList<ScaleDefinition> All { get; }
}
```

Usage :

```csharp
var scale = Scale.Create(
    SpelledPitch.Parse("F#"),
    StandardScales.Major);

// F# G# A# B C# D# E#
```

Les identifiants comme `scale.major` sont stables et non localisés. Un formateur transforme ensuite cet identifiant en libellé français, anglais ou autre.

Exemples de libellés d'une même définition :

```text
mode.major.5 -> "mode de sol" en français
mode.major.5 -> "Mixolydian" en américain
```

Les rotations des gammes mineures harmonique et mélodique possèdent également des identifiants stables. Le catalogue de présentation peut fournir plusieurs alias lorsque la terminologie n'est pas universelle.

### 5.1 Dérivation pentatonique

```csharp
public enum PentatonicDerivationStrategy
{
    StandardMajorOrMinor,
    SelectSourceDegrees
}

public sealed class PentatonicDerivation
{
    public Scale Source { get; }
    public Scale Result { get; }
    public PentatonicDerivationStrategy Strategy { get; }
    public IReadOnlyList<int> SelectedDegrees { get; }
}

public static class PentatonicScales
{
    public static PentatonicDerivation FromScale(
        Scale source,
        PentatonicDerivationStrategy strategy =
            PentatonicDerivationStrategy.StandardMajorOrMinor,
        IReadOnlyList<int>? sourceDegrees = null);

    public static bool TryFromScale(
        Scale source,
        out PentatonicDerivation? result,
        PentatonicDerivationStrategy strategy =
            PentatonicDerivationStrategy.StandardMajorOrMinor,
        IReadOnlyList<int>? sourceDegrees = null);
}
```

La stratégie standard retourne `1 2 3 5 6` pour une gamme majeure compatible et `1 ♭3 4 5 ♭7` pour une gamme mineure compatible. Elle échoue si la gamme source ne contient pas les cinq sons requis.

## 6. Tonalités et armures

```csharp
namespace Enaxos.MusicTheory.Tonality;

public enum KeyMode
{
    Major,
    Minor
}

public readonly record struct MusicalKey
{
    public SpelledPitch Tonic { get; }
    public KeyMode Mode { get; }

    public MusicalKey(SpelledPitch tonic, KeyMode mode);

    public static MusicalKey Major(SpelledPitch tonic);
    public static MusicalKey Minor(SpelledPitch tonic);
    public static MusicalKey Parse(string text);
}

public sealed class KeySignature : IEquatable<KeySignature>
{
    // -7..+7 : bémols négatifs, dièses positifs.
    public int Fifths { get; }
    public int AccidentalCount { get; }
    public Accidental Accidental { get; }
    public IReadOnlyList<NoteLetter> AlteredLetters { get; }

    public static KeySignature FromFifths(int fifths);
    public static KeySignature For(MusicalKey key);
}

public static class KeyRelationships
{
    public static MusicalKey RelativeOf(MusicalKey key);
    public static MusicalKey ParallelOf(MusicalKey key);
}

public readonly record struct ScaleDegreeNumber
{
    public int Value { get; }

    public ScaleDegreeNumber(int value); // 1..7
}

public enum HarmonicChordQuality
{
    Major,
    Minor,
    Diminished,
    HalfDiminished,
    Augmented,
    Other
}

public readonly record struct HarmonicFunction
{
    public ScaleDegreeNumber Degree { get; }
    public HarmonicChordQuality Quality { get; }
    public int InversionNumber { get; }
}

public static class HarmonicFunctions
{
    public static HarmonicFunction Analyze(
        Chord chord,
        MusicalKey key);

    public static bool TryAnalyze(
        Chord chord,
        MusicalKey key,
        out HarmonicFunction result);
}
```

`HarmonicFunction` est toujours contextuelle. Le formateur produit par défaut `I`, `ii`, `vii°`, `viiø` ou `III+` selon la qualité.

## 7. Accords, voicings et renversements

```csharp
namespace Enaxos.MusicTheory.Harmony;

public sealed class ChordDefinition : IEquatable<ChordDefinition>
{
    public string Id { get; }
    public IReadOnlyList<FormulaDegree> Degrees { get; }

    public ChordDefinition(
        string id,
        IEnumerable<FormulaDegree> degrees);
}

public sealed class Chord : IEquatable<Chord>
{
    public SpelledPitch Root { get; }
    public ChordDefinition Definition { get; }
    public ChordSymbol Symbol { get; }
    public IReadOnlyList<SpelledPitch> Pitches { get; }

    public static Chord Create(
        SpelledPitch root,
        ChordDefinition definition);

    public DerivedChord Transpose(
        int semitones,
        EnharmonicPreference preference =
            EnharmonicPreference.FewestAccidentals);
}

public readonly record struct ChordSymbol
{
    public SpelledPitch Root { get; }
    public string DefinitionId { get; }

    public static ChordSymbol Parse(string text);
    public static bool TryParse(
        ReadOnlySpan<char> text,
        out ChordSymbol result);
}

public sealed class DerivedChord : IEquatable<DerivedChord>
{
    public Chord OriginalChord { get; }
    public Chord CurrentChord { get; }
    public int SemitoneDeltaFromOriginal { get; }

    public static DerivedChord From(Chord chord);

    public DerivedChord Transpose(
        int semitones,
        EnharmonicPreference preference =
            EnharmonicPreference.FewestAccidentals);
}

public sealed class ChordRealization : IEquatable<ChordRealization>
{
    public DerivedChord Derivation { get; }
    public Chord OriginalChord { get; }
    public Chord CurrentChord { get; }
    public IReadOnlyList<Note> Notes { get; }
    public int SemitoneDeltaFromOriginal { get; }
    public int InversionNumber { get; }
    public Note Bass { get; }

    public static ChordRealization CreateRootPosition(
        Chord chord,
        int bassOctave);

    public static ChordRealization CreateRootPosition(
        DerivedChord chord,
        int bassOctave);

    public static ChordRealization Create(
        Chord chord,
        IEnumerable<Note> notes,
        bool validate = true);

    public ChordRealization Transpose(
        int semitones,
        EnharmonicPreference preference =
            EnharmonicPreference.FewestAccidentals);

    public ChordRealization Invert(int inversionNumber);
}

public static class StandardChords
{
    public static ChordDefinition Major { get; }
    public static ChordDefinition Minor { get; }
    public static ChordDefinition Diminished { get; }
    public static ChordDefinition Augmented { get; }
    public static ChordDefinition DominantSeventh { get; }
    public static ChordDefinition MajorSeventh { get; }
    public static ChordDefinition MinorSeventh { get; }
    public static ChordDefinition HalfDiminishedSeventh { get; }
    public static ChordDefinition DiminishedSeventh { get; }
}
```

Usage :

```csharp
var chord = Chord.Create(
    SpelledPitch.Parse("Db"),
    StandardChords.Major);

// Db F Ab
```

Le nom est accessible sous forme structurelle par `Symbol`; les chaînes localisées sont produites par `MusicFormatter`. La provenance des transformations est portée par `ChordRealization` :

```csharp
var original = Chord.Create(
    SpelledPitch.Parse("C"),
    StandardChords.Major);

var result = ChordRealization
    .CreateRootPosition(original, bassOctave: 4)
    .Transpose(+14)
    .Invert(2);

result.OriginalChord == original;             // true
result.SemitoneDeltaFromOriginal == 14;       // true, pas 2
result.InversionNumber == 2;                  // true
```

## 7.1 Reconnaissance d'accords

```csharp
namespace Enaxos.MusicTheory.Analysis;

public sealed record ChordRecognitionOptions
{
    public bool AllowEnharmonicEquivalence { get; init; } = true;
    public bool AllowMissingTones { get; init; }
    public bool AllowAddedTones { get; init; }
    public int MaximumResults { get; init; } = 16;
}

public sealed class ChordRecognitionCandidate
{
    public Chord Chord { get; }
    public int? InversionNumber { get; }
    public IReadOnlyList<SpelledPitch> MissingPitches { get; }
    public IReadOnlyList<SpelledPitch> AddedPitches { get; }
    public double Score { get; }
    public double Confidence { get; }
}

public static class ChordRecognizer
{
    public static IReadOnlyList<ChordRecognitionCandidate> Recognize(
        IEnumerable<Note> notes,
        ChordRecognitionOptions? options = null);

    public static bool TryRecognizeBest(
        IEnumerable<Note> notes,
        out ChordRecognitionCandidate? candidate,
        ChordRecognitionOptions? options = null);

    public static IReadOnlyList<ChordRecognitionCandidate> Recognize(
        Chord chord,
        ChordRecognitionOptions? options = null);
}
```

`TryRecognizeBest` est un raccourci explicite vers le premier candidat classé par `Recognize`. Il ne transforme pas une liste de notes en `Chord` directement, afin de conserver l'ambiguïté musicale dans le type `ChordRecognitionCandidate`.

Le formateur transforme ensuite chaque candidat en nom abrégé ou complet, français ou américain. La reconnaissance ne mélange donc pas analyse et localisation.

## 7.2 Recherche de gammes compatibles

```csharp
public sealed record ScaleRecognitionWeights
{
    public double Membership { get; init; } = 1.0;
    public double Coverage { get; init; } = 1.0;
    public double OutsideNotePenalty { get; init; } = 2.0;
    public double SpellingConsistency { get; init; } = 0.5;
    public double TonicEvidence { get; init; } = 0.5;
    public double ChordRootEvidence { get; init; } = 0.75;
}

public sealed record ScaleRecognitionOptions
{
    public IReadOnlyList<ScaleDefinition>? Catalog { get; init; }
    public ScaleRecognitionWeights Weights { get; init; } = new();
    public bool StrictMembership { get; init; } = true;
    public int MaximumResults { get; init; } = 32;
    public double ProbabilityTemperature { get; init; } = 1.0;
}

public sealed class ScaleRecognitionCandidate
{
    public Scale Scale { get; }
    public double Score { get; }

    // Normalisée sur l'ensemble retourné ; ce n'est pas une probabilité
    // statistique calibrée depuis un corpus.
    public double RelativeProbability { get; }

    public IReadOnlyList<SpelledPitch> MatchedPitches { get; }
    public IReadOnlyList<SpelledPitch> MissingPitches { get; }
    public IReadOnlyList<SpelledPitch> OutsidePitches { get; }
    public IReadOnlyDictionary<string, double> ScoreFactors { get; }
}

public static class ScaleRecognizer
{
    public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(
        IEnumerable<Note> notes,
        ScaleRecognitionOptions? options = null);

    public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(
        Chord chord,
        ScaleRecognitionOptions? options = null);
}
```

Les résultats sont triés par `Score` décroissant, puis par armure, tonique et identifiant de mode afin que l'ordre reste déterministe.

`RelativeProbability` est calculée par normalisation softmax des scores retournés. `ProbabilityTemperature` contrôle l'écart entre les candidats et doit être strictement positif.

## 8. Accordage et MIDI

```csharp
namespace Enaxos.MusicTheory.Tuning;

public interface ITuningSystem
{
    double GetFrequency(Note note);
}

public sealed class EqualTemperament12 : ITuningSystem
{
    public Note ReferenceNote { get; }
    public double ReferenceFrequency { get; }

    public EqualTemperament12(
        Note? referenceNote = null,
        double referenceFrequency = 440.0);

    public double GetFrequency(Note note);
}
```

```csharp
namespace Enaxos.MusicTheory.Midi;

public static class MidiNote
{
    public static int ToNumber(Note note);
    public static bool TryToNumber(Note note, out int number);

    public static Note FromNumber(
        int number,
        EnharmonicPreference preference = EnharmonicPreference.PreferSharps);
}
```

`MidiNote.FromNumber` doit recevoir une préférence, car un numéro MIDI ne contient aucune information d'orthographe.

## 9. Cercle des quintes

### 9.1 Données harmoniques

```csharp
namespace Enaxos.MusicTheory.Circle;

public enum CircleDirection
{
    Clockwise,
    CounterClockwise
}

public sealed record CircleKeyPair
{
    public MusicalKey Major { get; }
    public MusicalKey RelativeMinor { get; }
    public KeySignature Signature { get; }
}

public sealed class CircleSegment : IEquatable<CircleSegment>
{
    public int Index { get; }
    public IReadOnlyList<CircleKeyPair> Spellings { get; }
    public CircleKeyPair Primary { get; }
}

public readonly record struct CircleNeighbors(
    CircleSegment Subdominant,
    CircleSegment Current,
    CircleSegment Dominant);

public readonly record struct CircleDistance(
    int ClockwiseSteps,
    int CounterClockwiseSteps)
{
    public int MinimumSteps { get; }
    public bool HasTwoShortestPaths { get; }
}

public sealed class CircleOfFifths
{
    public static CircleOfFifths Standard { get; }

    public IReadOnlyList<CircleSegment> Segments { get; }

    public static CircleOfFifths Create(
        EnharmonicPreference preference =
            EnharmonicPreference.FewestAccidentals);

    public CircleSegment this[int index] { get; }

    public CircleSegment Find(MusicalKey key);
    public bool TryFind(MusicalKey key, out CircleSegment? segment);

    public CircleSegment Move(
        CircleSegment from,
        CircleDirection direction,
        int steps = 1);

    public CircleNeighbors GetNeighbors(CircleSegment segment);

    public CircleDistance GetDistance(
        CircleSegment from,
        CircleSegment to);
}
```

`CircleDistance` conserve les deux chemins. Deux positions opposées ont six pas dans chaque direction ; l'API ne choisit donc pas arbitrairement un signe.

Usage pour les relations musicales :

```csharp
var circle = CircleOfFifths.Standard;
var eb = circle.Find(MusicalKey.Major(SpelledPitch.Parse("Eb")));
var neighbors = circle.GetNeighbors(eb);

MusicalKey dominant = neighbors.Dominant.Primary.Major;       // Bb majeur
MusicalKey subdominant = neighbors.Subdominant.Primary.Major; // Ab majeur
MusicalKey relative = eb.Primary.RelativeMinor;               // C mineur
```

Les trois graphies doubles conventionnelles restent accessibles :

```text
index 5 : B / Cb
index 6 : F# / Gb
index 7 : C# / Db
```

La stratégie ne supprime aucune graphie ; elle choisit uniquement `Primary`.

### 9.2 Géométrie UI indépendante

```csharp
namespace Enaxos.MusicTheory.Geometry;

public enum RotationDirection
{
    Clockwise,
    CounterClockwise
}

public readonly record struct UnitPoint(double X, double Y);

public sealed record CircleGeometryOptions
{
    // C majeur en haut par défaut.
    public double StartAngleDegrees { get; init; } = -90.0;
    public RotationDirection Direction { get; init; } =
        RotationDirection.Clockwise;

    public double MajorLabelRadius { get; init; } = 0.78;
    public double MinorLabelRadius { get; init; } = 0.52;
}

public sealed record CircleGeometrySegment
{
    public CircleSegment Segment { get; }
    public double StartAngleDegrees { get; }
    public double CenterAngleDegrees { get; }
    public double SweepAngleDegrees { get; }
    public UnitPoint MajorLabelAnchor { get; }
    public UnitPoint MinorLabelAnchor { get; }
}

public sealed class CircleGeometry
{
    public IReadOnlyList<CircleGeometrySegment> Segments { get; }

    public static CircleGeometry Create(
        CircleOfFifths circle,
        CircleGeometryOptions? options = null);
}
```

Exemple de rendu générique :

```csharp
var circle = CircleOfFifths.Create(
    EnharmonicPreference.FewestAccidentals);

var geometry = CircleGeometry.Create(circle);

foreach (var item in geometry.Segments)
{
    DrawSector(
        item.StartAngleDegrees,
        item.SweepAngleDegrees,
        isSelected: item.Segment == selected);

    DrawMajorLabel(
        item.Segment.Primary.Major,
        item.MajorLabelAnchor);

    DrawMinorLabel(
        item.Segment.Primary.RelativeMinor,
        item.MinorLabelAnchor);
}
```

Pour convertir un point normalisé vers un contrôle de largeur `w` et hauteur `h` :

```text
x = centerX + point.X × radiusPixels
y = centerY + point.Y × radiusPixels
```

Le hit-testing peut être ajouté ultérieurement par :

```csharp
bool TryHitTest(
    UnitPoint point,
    out CircleGeometrySegment? segment);
```

## 10. Présentation et localisation

```csharp
namespace Enaxos.MusicTheory.Presentation;

public enum MusicTerminology
{
    French,
    American
}

public enum AccidentalGlyphStyle
{
    Unicode,
    Ascii
}

public enum ChordNameStyle
{
    StandardAbbreviation,
    Full
}

public enum DegreeDisplayStyle
{
    Arabic,
    Roman
}

public static class MusicDisplayDefaults
{
    // Accès atomique. N'influence aucun calcul du domaine.
    public static MusicTerminology Terminology { get; set; }
}

public sealed record MusicFormatOptions
{
    // null signifie : capturer MusicDisplayDefaults.Terminology au début
    // de l'appel de formatage.
    public MusicTerminology? TerminologyOverride { get; init; }

    public AccidentalGlyphStyle Accidentals { get; init; } =
        AccidentalGlyphStyle.Unicode;

    public IFormatProvider? Culture { get; init; }
}

public static class MusicFormatter
{
    public static string Format(
        SpelledPitch pitch,
        MusicFormatOptions? options = null);

    public static string Format(
        Note note,
        MusicFormatOptions? options = null);

    public static string Format(
        MusicalKey key,
        MusicFormatOptions? options = null);

    public static string Format(
        Chord chord,
        ChordNameStyle style = ChordNameStyle.StandardAbbreviation,
        MusicFormatOptions? options = null);

    public static string FormatChordPitches(
        Chord chord,
        MusicFormatOptions? options = null);

    public static string Format(
        ScaleDefinition definition,
        MusicFormatOptions? options = null);

    public static string Format(
        ScaleDegreeNumber degree,
        DegreeDisplayStyle style = DegreeDisplayStyle.Arabic,
        MusicFormatOptions? options = null);

    public static string Format(
        HarmonicFunction function,
        MusicFormatOptions? options = null);
}
```

Les noms de gammes et d'accords utilisent des identifiants de ressources comme `scale.major` et `chord.dominant7`. Leur traduction appartient à l'application ou à un package de ressources optionnel.

Le défaut et l'override local peuvent être utilisés simultanément :

```csharp
MusicDisplayDefaults.Terminology = MusicTerminology.French;

string french = MusicFormatter.Format(chord); // "Do m7"

string american = MusicFormatter.Format(
    chord,
    ChordNameStyle.StandardAbbreviation,
    new MusicFormatOptions
    {
        TerminologyOverride = MusicTerminology.American
    }); // "Cm7"

// Le défaut global reste French.
```

Exemple de reconnaissance suivie du nommage :

```csharp
var candidates = ChordRecognizer.Recognize(notes);

string? bestFullName = candidates.Count == 0
    ? null
    : MusicFormatter.Format(
        candidates[0].Chord,
        ChordNameStyle.Full,
        new MusicFormatOptions
        {
            TerminologyOverride = MusicTerminology.French
        });
```

## 11. Erreurs

Erreurs de programmation :

- `ArgumentOutOfRangeException` pour un index, un rayon ou une fréquence invalide ;
- `ArgumentException` pour une définition incohérente ;
- `InvalidOperationException` uniquement lorsqu'un état valide ne permet pas l'opération demandée.

Entrées utilisateur :

- `Parse` lève `FormatException` avec une erreur concise ;
- `TryParse` retourne `false` et ne lève pas d'exception ;
- une API détaillée optionnelle peut retourner un `MusicParseError` avec code et position.

Il n'existe aucun `catch` global transformant une erreur en valeur vide, `0` ou `null` silencieux.

## 12. Compatibilité et versionnement

- Cette API ne cherche pas la compatibilité avec `Enaxos.Harmony.Core`.
- Aucun type `Py*`, aucune méthode d'extension générale et aucun alias historique n'est ajouté.
- La première version stable doit être publiée seulement après gel des invariants et revue de l'API.
- Toute modification de l'égalité, de l'orthographe ou de la formule d'un type standard est considérée comme un changement incompatible.
- Les définitions standard possèdent des identifiants versionnés et testés.

## 13. Ordre d'implémentation recommandé

1. `NoteLetter`, `Accidental`, `PitchClass`, `SpelledPitch`, `Note`.
2. `Interval`, qualité, calcul entre notes et transposition.
3. `FormulaDegree`, familles de modes, gammes et pentatoniques.
4. Armures, degrés et relations relative/parallèle.
5. Accords, réalisations, provenance des transformations et fonctions harmoniques.
6. Reconnaissance d'accords et classement des gammes candidates.
7. Cercle des quintes et relations.
8. Géométrie UI, terminologies française/américaine et formatage.
9. Accordage et adaptateur MIDI.

Chaque étape doit être accompagnée de tests des invariants correspondants avant la suivante.

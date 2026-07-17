# Enaxos.MusicTheory 1.2.5 — Full Public API

This document describes the complete public API exposed by `Enaxos.MusicTheory` version **1.2.5** (`net8.0`).

## Conventions

- Musical spelling is significant unless a member explicitly states that comparison is enharmonic.
- Collection properties are immutable views unless stated otherwise.
- Degree and interval numbers are one-based; collection indexes and inversion numbers are zero-based.
- All invariant parsers use English letter names. Localized output is handled by the presentation API.
- Optional parameter defaults are shown in signatures.

## Namespaces

- [`Enaxos.MusicTheory.Analysis`](#enaxosmusictheoryanalysis)
- [`Enaxos.MusicTheory.Circle`](#enaxosmusictheorycircle)
- [`Enaxos.MusicTheory.Formulas`](#enaxosmusictheoryformulas)
- [`Enaxos.MusicTheory.Geometry`](#enaxosmusictheorygeometry)
- [`Enaxos.MusicTheory.Harmony`](#enaxosmusictheoryharmony)
- [`Enaxos.MusicTheory.Intervals`](#enaxosmusictheoryintervals)
- [`Enaxos.MusicTheory.Midi`](#enaxosmusictheorymidi)
- [`Enaxos.MusicTheory.Presentation`](#enaxosmusictheorypresentation)
- [`Enaxos.MusicTheory.Primitives`](#enaxosmusictheoryprimitives)
- [`Enaxos.MusicTheory.Scales`](#enaxosmusictheoryscales)
- [`Enaxos.MusicTheory.Tonality`](#enaxosmusictheorytonality)
- [`Enaxos.MusicTheory.Tuning`](#enaxosmusictheorytuning)

## `Enaxos.MusicTheory.Analysis`

### `ChordRecognitionCandidate`

Describes one recognized chord candidate and the evidence used to rank it.

**Declaration**

```csharp
public sealed class ChordRecognitionCandidate
```

#### Properties

- `public IReadOnlyList<SpelledPitch> AddedPitches { get; }`
  Gets input pitches not explained by the candidate chord.

- `public Chord Chord { get; }`
  Gets the candidate chord.

- `public double Confidence { get; }`
  Gets the score normalized to the inclusive range 0 through 1.

- `public int? InversionNumber { get; }`
  Gets the zero-based bass chord-tone index, or null when the bass is not a chord tone.

- `public IReadOnlyList<SpelledPitch> MissingPitches { get; }`
  Gets candidate chord tones absent from the input.

- `public double Score { get; }`
  Gets the deterministic heuristic ranking score; larger values are better.

### `ChordRecognitionOptions`

Configures chord matching, tolerated discrepancies, and result truncation.

**Declaration**

```csharp
public sealed class ChordRecognitionOptions
```

#### Constructors

- `public ChordRecognitionOptions()`
  Creates a new ChordRecognitionOptions instance.

#### Properties

- `public bool AllowAddedTones { get; init; }`
  Gets whether observed pitches outside a candidate chord are tolerated.

- `public bool AllowEnharmonicEquivalence { get; init; }`
  Gets whether pitches with different spellings may match by pitch class.

- `public bool AllowMissingTones { get; init; }`
  Gets whether candidates may omit expected chord tones.

- `public int MaximumResults { get; init; }`
  Gets the maximum number of ranked candidates returned.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(ChordRecognitionOptions other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(ChordRecognitionOptions left, ChordRecognitionOptions right)`
  Determines whether two values are equal.

- `public static bool operator !=(ChordRecognitionOptions left, ChordRecognitionOptions right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `ChordRecognizer`

Ranks standard chord definitions against observed absolute notes.

**Declaration**

```csharp
public static class ChordRecognizer
```

#### Methods

- `public static IReadOnlyList<ChordRecognitionCandidate> Recognize(IEnumerable<Note> notes, ChordRecognitionOptions? options = null)`
  Recognizes chord candidates from notes, using the lowest note as inversion evidence.

- `public static bool TryRecognizeBest(IEnumerable<Note> notes, out ChordRecognitionCandidate? candidate, ChordRecognitionOptions? options = null)`
  Attempts to return the highest-ranked chord candidate recognized from notes.

- `public static IReadOnlyList<ChordRecognitionCandidate> Recognize(Chord chord, ChordRecognitionOptions? options = null)`
  Recognizes candidates from a chord's canonical close root-position realization.

### `ScaleRecognitionCandidate`

Describes one scale candidate, its pitch comparison, and its transparent score breakdown.

**Declaration**

```csharp
public sealed class ScaleRecognitionCandidate
```

#### Properties

- `public IReadOnlyList<SpelledPitch> MatchedPitches { get; }`
  Gets observed pitches matched enharmonically by the scale.

- `public IReadOnlyList<SpelledPitch> MissingPitches { get; }`
  Gets scale pitches absent from the observation.

- `public IReadOnlyList<SpelledPitch> OutsidePitches { get; }`
  Gets observed pitches that do not belong to the scale.

- `public double RelativeProbability { get; }`
  Gets this candidate's softmax probability relative to the returned result set.

- `public Scale Scale { get; }`
  Gets the realized candidate scale.

- `public double Score { get; }`
  Gets the sum of the named weighted score factors.

- `public IReadOnlyDictionary<string, double> ScoreFactors { get; }`
  Gets the immutable named contributions whose sum is `Score`.

### `ScaleRecognitionOptions`

Configures the scale catalog, score model, filtering, and probability conversion.

**Declaration**

```csharp
public sealed class ScaleRecognitionOptions
```

#### Constructors

- `public ScaleRecognitionOptions()`
  Creates a new ScaleRecognitionOptions instance.

#### Properties

- `public IReadOnlyList<ScaleDefinition>? Catalog { get; init; }`
  Gets a custom catalog, or null to use the standard catalog plus any enabled optional catalogs.

- `public bool IncludeExoticCandidates { get; init; }`
  Gets whether the default catalog also searches curated exotic scale definitions.

- `public bool IncludePentatonicCandidates { get; init; }`
  Gets whether the default catalog also searches the standard major and minor pentatonic scales.

- `public int MaximumResults { get; init; }`
  Gets the maximum number of candidates retained before probabilities are normalized.

- `public double ProbabilityTemperature { get; init; }`
  Gets the positive softmax temperature; lower values concentrate probability on top scores.

- `public bool StrictMembership { get; init; }`
  Gets whether candidates containing any outside observed pitch are rejected.

- `public ScaleRecognitionWeights Weights { get; init; }`
  Gets the non-negative coefficients used to calculate candidate scores.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(ScaleRecognitionOptions other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(ScaleRecognitionOptions left, ScaleRecognitionOptions right)`
  Determines whether two values are equal.

- `public static bool operator !=(ScaleRecognitionOptions left, ScaleRecognitionOptions right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `ScaleRecognitionWeights`

Defines non-negative coefficients for each independently reported scale-recognition factor.

**Declaration**

```csharp
public sealed class ScaleRecognitionWeights
```

#### Constructors

- `public ScaleRecognitionWeights()`
  Creates a new ScaleRecognitionWeights instance.

#### Properties

- `public double ChordRootEvidence { get; init; }`
  Gets the bonus awarded when a supplied chord root equals the candidate tonic.

- `public double Coverage { get; init; }`
  Gets the weight of the fraction of scale pitches present in the observation.

- `public double Membership { get; init; }`
  Gets the weight of the fraction of observed pitches explained by the scale.

- `public double OutsideNotePenalty { get; init; }`
  Gets the penalty applied per observed pitch outside the scale.

- `public double SpellingConsistency { get; init; }`
  Gets the weight of exact spelling matches among observed pitches.

- `public double TonicEvidence { get; init; }`
  Gets the bonus awarded when the candidate tonic is observed.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(ScaleRecognitionWeights other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(ScaleRecognitionWeights left, ScaleRecognitionWeights right)`
  Determines whether two values are equal.

- `public static bool operator !=(ScaleRecognitionWeights left, ScaleRecognitionWeights right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `ScaleRecognizer`

Ranks realized scales against observed pitches using a configurable transparent score model.

**Declaration**

```csharp
public static class ScaleRecognizer
```

#### Methods

- `public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(IEnumerable<Note> notes, ScaleRecognitionOptions? options = null)`
  Finds scale candidates for absolute notes without assuming a chord root.

- `public static IReadOnlyList<ScaleRecognitionCandidate> FindCandidates(Chord chord, ScaleRecognitionOptions? options = null)`
  Finds scale candidates for chord tones and uses the chord root as additional tonic evidence.

## `Enaxos.MusicTheory.Circle`

### `CircleDirection`

Specifies movement around the logical circle of fifths.

**Declaration**

```csharp
public enum CircleDirection
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Clockwise` | 0 | Moves toward the dominant, adding one sharp or removing one flat. |
| `CounterClockwise` | 1 | Moves toward the subdominant, adding one flat or removing one sharp. |

### `CircleDistance`

Contains directional step counts between two circle segments.

**Declaration**

```csharp
public struct CircleDistance
```

#### Constructors

- `public CircleDistance(int ClockwiseSteps, int CounterClockwiseSteps)`
  Contains directional step counts between two circle segments.
  - `ClockwiseSteps`: The normalized clockwise distance from 0 through 11.
  - `CounterClockwiseSteps`: The normalized counterclockwise distance from 0 through 11.

#### Properties

- `public int ClockwiseSteps { get; init; }`
  The normalized clockwise distance from 0 through 11.

- `public int CounterClockwiseSteps { get; init; }`
  The normalized counterclockwise distance from 0 through 11.

- `public bool HasTwoShortestPaths { get; }`
  Gets whether clockwise and counterclockwise paths are equally short.

- `public int MinimumSteps { get; }`
  Gets the length of a shortest path in either direction.

#### Methods

- `public void Deconstruct(out int ClockwiseSteps, out int CounterClockwiseSteps)`
  Deconstructs the value into its positional components.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(CircleDistance other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(CircleDistance left, CircleDistance right)`
  Determines whether two values are equal.

- `public static bool operator !=(CircleDistance left, CircleDistance right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `CircleKeyPair`

Groups a major key, its relative minor, and their shared key signature.

**Declaration**

```csharp
public sealed class CircleKeyPair
```

#### Constructors

- `public CircleKeyPair(MusicalKey major, MusicalKey relativeMinor, KeySignature signature)`
  Creates a validated relative-key pair.

#### Properties

- `public MusicalKey Major { get; }`
  Gets the major key represented by this spelling.

- `public MusicalKey RelativeMinor { get; }`
  Gets the relative minor sharing the signature.

- `public KeySignature Signature { get; }`
  Gets the key signature shared by both keys.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(CircleKeyPair other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(CircleKeyPair left, CircleKeyPair right)`
  Determines whether two values are equal.

- `public static bool operator !=(CircleKeyPair left, CircleKeyPair right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `CircleNeighbors`

Groups a circle segment with its immediate subdominant and dominant neighbors.

**Declaration**

```csharp
public struct CircleNeighbors
```

#### Constructors

- `public CircleNeighbors(CircleSegment Subdominant, CircleSegment Current, CircleSegment Dominant)`
  Groups a circle segment with its immediate subdominant and dominant neighbors.
  - `Subdominant`: The previous counterclockwise segment.
  - `Current`: The requested segment.
  - `Dominant`: The next clockwise segment.

#### Properties

- `public CircleSegment Current { get; init; }`
  The requested segment.

- `public CircleSegment Dominant { get; init; }`
  The next clockwise segment.

- `public CircleSegment Subdominant { get; init; }`
  The previous counterclockwise segment.

#### Methods

- `public void Deconstruct(out CircleSegment Subdominant, out CircleSegment Current, out CircleSegment Dominant)`
  Deconstructs the value into its positional components.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(CircleNeighbors other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(CircleNeighbors left, CircleNeighbors right)`
  Determines whether two values are equal.

- `public static bool operator !=(CircleNeighbors left, CircleNeighbors right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `CircleOfFifths`

Provides a twelve-segment logical circle of fifths with conventional enharmonic spellings.

**Declaration**

```csharp
public sealed class CircleOfFifths
```

#### Properties

- `public CircleSegment this[int index] { get; }`
  Gets a segment by a wrapping index; negative and out-of-range values are normalized.

- `public IReadOnlyList<CircleSegment> Segments { get; }`
  Gets the immutable clockwise sequence of twelve segments.

- `public static CircleOfFifths Standard { get; }`
  Gets the shared circle using the fewest-accidentals primary-spelling policy.

#### Methods

- `public static CircleOfFifths Create(EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)`
  Creates a circle and selects primary spellings according to an enharmonic preference.

- `public CircleSegment Find(MusicalKey key)`
  Finds the segment containing a major or relative-minor key spelling.

- `public CircleDistance GetDistance(CircleSegment from, CircleSegment to)`
  Measures both normalized directional distances between two segments.

- `public CircleNeighbors GetNeighbors(CircleSegment segment)`
  Gets the immediate subdominant and dominant neighbors of a segment.

- `public CircleSegment Move(CircleSegment from, CircleDirection direction, int steps = 1)`
  Moves a non-negative number of wrapping steps from a segment.

- `public bool TryFind(MusicalKey key, out CircleSegment? segment)`
  Attempts to find the segment containing a major or relative-minor key spelling.

### `CircleSegment`

Represents one of twelve pitch-class positions on the circle of fifths.

**Declaration**

```csharp
public sealed class CircleSegment
```

#### Properties

- `public int Index { get; }`
  Gets the zero-based clockwise index, with C major at zero.

- `public CircleKeyPair Primary { get; }`
  Gets the spelling selected by the circle's enharmonic preference.

- `public IReadOnlyList<CircleKeyPair> Spellings { get; }`
  Gets every conventional key spelling represented at this pitch-class position.

#### Methods

- `public bool Equals(CircleSegment? other)`
  Determines whether this value equals another value.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

## `Enaxos.MusicTheory.Formulas`

### `FormulaDegree`

Describes one tonic-relative degree of a scale or chord formula.

**Declaration**

```csharp
public struct FormulaDegree
```

#### Constructors

- `public FormulaDegree(int number, int alteration = 0)`
  Creates a formula degree.
  - `number`: The one-based diatonic degree number; compound degrees are allowed.
  - `alteration`: The signed semitone alteration from the major/perfect reference degree.

#### Properties

- `public int Alteration { get; }`
  Gets the signed chromatic alteration from the reference degree.

- `public int Number { get; }`
  Gets the one-based diatonic degree number.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(FormulaDegree other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(FormulaDegree left, FormulaDegree right)`
  Determines whether two values are equal.

- `public static bool operator !=(FormulaDegree left, FormulaDegree right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

## `Enaxos.MusicTheory.Geometry`

### `CircleGeometry`

Builds immutable, renderer-independent geometry for a twelve-segment circle of fifths.

**Declaration**

```csharp
public sealed class CircleGeometry
```

#### Properties

- `public IReadOnlyList<CircleGeometrySegment> Segments { get; }`
  Gets the immutable twelve-segment geometry.

#### Methods

- `public static CircleGeometry Create(CircleOfFifths circle, CircleGeometryOptions? options = null)`
  Creates sector angles and normalized label anchors for a logical circle.

### `CircleGeometryOptions`

Configures angular orientation and normalized label radii for circle geometry.

**Declaration**

```csharp
public sealed class CircleGeometryOptions
```

#### Constructors

- `public CircleGeometryOptions()`
  Creates a new CircleGeometryOptions instance.

#### Properties

- `public RotationDirection Direction { get; init; }`
  Gets the direction in which subsequent segment centers progress.

- `public double MajorLabelRadius { get; init; }`
  Gets the normalized radius of major-key label anchors.

- `public double MinorLabelRadius { get; init; }`
  Gets the normalized radius of relative-minor label anchors.

- `public double StartAngleDegrees { get; init; }`
  Gets the center angle of segment zero in degrees; -90 places C at the top.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(CircleGeometryOptions other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(CircleGeometryOptions left, CircleGeometryOptions right)`
  Determines whether two values are equal.

- `public static bool operator !=(CircleGeometryOptions left, CircleGeometryOptions right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `CircleGeometrySegment`

Contains renderer-independent angular and label-anchor geometry for one circle segment.

**Declaration**

```csharp
public sealed class CircleGeometrySegment
```

#### Properties

- `public double CenterAngleDegrees { get; }`
  Gets the sector center angle in degrees.

- `public UnitPoint MajorLabelAnchor { get; }`
  Gets the normalized major-key label anchor.

- `public UnitPoint MinorLabelAnchor { get; }`
  Gets the normalized relative-minor label anchor.

- `public CircleSegment Segment { get; }`
  Gets the logical circle segment described by this geometry.

- `public double StartAngleDegrees { get; }`
  Gets the leading sector boundary angle in degrees.

- `public double SweepAngleDegrees { get; }`
  Gets the signed angular sweep; its sign encodes rotation direction.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(CircleGeometrySegment other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(CircleGeometrySegment left, CircleGeometrySegment right)`
  Determines whether two values are equal.

- `public static bool operator !=(CircleGeometrySegment left, CircleGeometrySegment right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `RotationDirection`

Specifies the visual angular progression of circle sectors.

**Declaration**

```csharp
public enum RotationDirection
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Clockwise` | 0 | Angles increase clockwise in the configured screen-oriented coordinate convention. |
| `CounterClockwise` | 1 | Angles decrease counterclockwise. |

### `UnitPoint`

Represents a Cartesian point relative to a unit-radius circle centered at the origin.

**Declaration**

```csharp
public struct UnitPoint
```

#### Constructors

- `public UnitPoint(double X, double Y)`
  Represents a Cartesian point relative to a unit-radius circle centered at the origin.
  - `X`: The horizontal coordinate, normally in the range -1 through 1.
  - `Y`: The vertical coordinate, normally in the range -1 through 1.

#### Properties

- `public double X { get; init; }`
  The horizontal coordinate, normally in the range -1 through 1.

- `public double Y { get; init; }`
  The vertical coordinate, normally in the range -1 through 1.

#### Methods

- `public void Deconstruct(out double X, out double Y)`
  Deconstructs the value into its positional components.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(UnitPoint other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(UnitPoint left, UnitPoint right)`
  Determines whether two values are equal.

- `public static bool operator !=(UnitPoint left, UnitPoint right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

## `Enaxos.MusicTheory.Harmony`

### `Chord`

Represents an immutable root-position pitch spelling of a chord definition.

**Declaration**

```csharp
public sealed class Chord
```

#### Properties

- `public ChordDefinition Definition { get; }`
  Gets the formula used to realize the chord.

- `public IReadOnlyList<SpelledPitch> Pitches { get; }`
  Gets immutable chord-tone spellings in formula order.

- `public SpelledPitch Root { get; }`
  Gets the spelled chord root.

- `public ChordSymbol Symbol { get; }`
  Gets the normalized lead-sheet symbol.

#### Methods

- `public static Chord Create(SpelledPitch root, ChordDefinition definition)`
  Realizes a chord formula on a spelled root.

- `public bool Equals(Chord? other)`
  Compares the written root and definition; realized pitches follow deterministically.

- `public bool Equals(object obj)`
  Compares the written root and definition; realized pitches follow deterministically.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public DerivedChord Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)`
  Chromatically transposes the chord and records its derivation from this chord.
  - `semitones`: The signed chromatic displacement.
  - `preference`: The spelling policy for the new root.

### `ChordDefinition`

Defines an immutable root-relative chord formula and stable identifier.

**Declaration**

```csharp
public sealed class ChordDefinition
```

#### Constructors

- `public ChordDefinition(string id, IEnumerable<FormulaDegree> degrees)`
  Creates a chord definition.
  - `id`: A stable culture-independent identifier.
  - `degrees`: Strictly increasing degrees beginning with an unaltered root.

#### Properties

- `public IReadOnlyList<FormulaDegree> Degrees { get; }`
  Gets the immutable ordered chord formula.

- `public string Id { get; }`
  Gets the stable definition identifier.

#### Methods

- `public bool Equals(ChordDefinition? other)`
  Compares the identifier and complete degree formula.

- `public bool Equals(object obj)`
  Compares the identifier and complete degree formula.

- `public int GetHashCode()`
  Returns a hash code for this value.

### `ChordRealization`

Places a derived chord into ordered absolute notes and records its inversion.

**Declaration**

```csharp
public sealed class ChordRealization
```

#### Properties

- `public Note Bass { get; }`
  Gets the lowest note of the realization.

- `public Chord CurrentChord { get; }`
  Gets the chord represented by the current notes.

- `public DerivedChord Derivation { get; }`
  Gets the chord transformation provenance.

- `public int InversionNumber { get; }`
  Gets the zero-based chord-tone index placed in the bass.

- `public IReadOnlyList<Note> Notes { get; }`
  Gets immutable notes ordered from bass to treble.

- `public Chord OriginalChord { get; }`
  Gets the chord at the start of the transformation chain.

- `public int SemitoneDeltaFromOriginal { get; }`
  Gets the cumulative chromatic displacement from the original chord.

#### Methods

- `public static ChordRealization Create(Chord chord, IEnumerable<Note> notes, bool validate = true)`
  Creates a realization from explicitly voiced notes ordered bass to treble.
  - `chord`: The chord whose tones the realization represents.
  - `notes`: The absolute notes in non-decreasing bass-to-treble order.
  - `validate`: When true, rejects non-chord tones and a bass outside the chord.

- `public static ChordRealization CreateRootPosition(Chord chord, int bassOctave)`
  Creates a close root-position realization while retaining derivation provenance.

- `public static ChordRealization CreateRootPosition(DerivedChord chord, int bassOctave)`
  Creates a close root-position realization while retaining derivation provenance.

- `public bool Equals(ChordRealization? other)`
  Compares derivation, inversion, and the complete ordered voicing.

- `public bool Equals(object obj)`
  Compares derivation, inversion, and the complete ordered voicing.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public ChordRealization Invert(int inversionNumber)`
  Moves voices by octaves to produce the requested inversion without changing pitch classes.

- `public ChordRealization Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)`
  Transposes every voice by an exact chromatic displacement and respells it from the derived chord.

### `ChordSymbol`

Represents a lead-sheet chord symbol as a spelled root and a stable definition identifier.

**Declaration**

```csharp
public struct ChordSymbol
```

#### Constructors

- `public ChordSymbol(SpelledPitch root, string definitionId)`
  Creates a chord symbol from normalized components.

#### Properties

- `public string DefinitionId { get; }`
  Gets the stable chord-definition identifier encoded by the suffix.

- `public SpelledPitch Root { get; }`
  Gets the written chord root.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(ChordSymbol other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(ChordSymbol left, ChordSymbol right)`
  Determines whether two values are equal.

- `public static bool operator !=(ChordSymbol left, ChordSymbol right)`
  Determines whether two values are different.

- `public static ChordSymbol Parse(string text)`
  Parses one of the supported lead-sheet chord symbols.

- `public string ToString()`
  Returns the root followed by the canonical suffix for its definition.

- `public static bool TryParse(ReadOnlySpan<char> text, out ChordSymbol result)`
  Attempts to parse a complete supported lead-sheet chord symbol.

### `DerivedChord`

Tracks a current chord together with its original chord and cumulative chromatic displacement.

**Declaration**

```csharp
public sealed class DerivedChord
```

#### Properties

- `public Chord CurrentChord { get; }`
  Gets the chord at the current point in the transformation chain.

- `public Chord OriginalChord { get; }`
  Gets the immutable origin of the transformation chain.

- `public int SemitoneDeltaFromOriginal { get; }`
  Gets the cumulative signed semitone displacement from the original chord.

#### Methods

- `public bool Equals(DerivedChord? other)`
  Determines whether this value equals another value.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public static DerivedChord From(Chord chord)`
  Starts a derivation chain with zero displacement.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public DerivedChord Transpose(int semitones, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)`
  Returns the next derived chord while preserving the original provenance.

### `StandardChords`

Provides canonical definitions for the standard triads and seventh chords.

**Declaration**

```csharp
public static class StandardChords
```

#### Properties

- `public static ChordDefinition Augmented { get; }`
  Gets the augmented triad.

- `public static ChordDefinition Diminished { get; }`
  Gets the diminished triad.

- `public static ChordDefinition DiminishedSeventh { get; }`
  Gets the fully diminished seventh chord.

- `public static ChordDefinition DominantSeventh { get; }`
  Gets the dominant seventh chord.

- `public static ChordDefinition HalfDiminishedSeventh { get; }`
  Gets the half-diminished seventh chord.

- `public static ChordDefinition Major { get; }`
  Gets the major triad.

- `public static ChordDefinition MajorSeventh { get; }`
  Gets the major seventh chord.

- `public static ChordDefinition Minor { get; }`
  Gets the minor triad.

- `public static ChordDefinition MinorSeventh { get; }`
  Gets the minor seventh chord.

## `Enaxos.MusicTheory.Intervals`

### `CommonIntervals`

Provides reusable canonical instances of the most common simple intervals.

**Declaration**

```csharp
public static class CommonIntervals
```

#### Properties

- `public static Interval AugmentedFourth { get; }`
  Gets an augmented fourth (6 semitones).

- `public static Interval DiminishedFifth { get; }`
  Gets a diminished fifth (6 semitones).

- `public static Interval MajorSecond { get; }`
  Gets a major second (2 semitones).

- `public static Interval MajorSeventh { get; }`
  Gets a major seventh (11 semitones).

- `public static Interval MajorSixth { get; }`
  Gets a major sixth (9 semitones).

- `public static Interval MajorThird { get; }`
  Gets a major third (4 semitones).

- `public static Interval MinorSecond { get; }`
  Gets a minor second (1 semitone).

- `public static Interval MinorSeventh { get; }`
  Gets a minor seventh (10 semitones).

- `public static Interval MinorSixth { get; }`
  Gets a minor sixth (8 semitones).

- `public static Interval MinorThird { get; }`
  Gets a minor third (3 semitones).

- `public static Interval PerfectFifth { get; }`
  Gets a perfect fifth (7 semitones).

- `public static Interval PerfectFourth { get; }`
  Gets a perfect fourth (5 semitones).

- `public static Interval PerfectOctave { get; }`
  Gets a perfect octave (12 semitones).

- `public static Interval PerfectUnison { get; }`
  Gets a perfect unison (0 semitones).

### `Interval`

Represents a directed-independent musical interval by diatonic number and chromatic distance.

> The diatonic number is inclusive: a unison is one and an octave is eight. Semitones are stored exactly and may be negative for valid spellings such as a diminished unison.

**Declaration**

```csharp
public struct Interval
```

#### Properties

- `public bool IsCompound { get; }`
  Gets whether the interval is larger than an octave by diatonic number.

- `public int Number { get; }`
  Gets the inclusive one-based diatonic interval number.

- `public IntervalQuality Quality { get; }`
  Gets the quality derived from the number and chromatic distance.

- `public int Semitones { get; }`
  Gets the exact chromatic distance in semitones.

#### Methods

- `public static Interval Between(Note from, Note to, IntervalDirection direction = IntervalDirection.Ascending)`
  Measures the interval between two spelled notes in a requested direction.

- `public static Interval Create(int number, IntervalQuality quality)`
  Creates an interval from a diatonic number and a compatible quality.
  - `number`: The inclusive one-based diatonic number.
  - `quality`: A quality valid for the number's perfect or major class.
  - Returns: The corresponding interval.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(Interval other)`
  Determines whether this value equals another value.

- `public static Interval FromDistances(int diatonicNumber, int semitones)`
  Creates an interval from exact diatonic and chromatic distances.
  - `diatonicNumber`: The inclusive one-based diatonic number.
  - `semitones`: The exact chromatic distance, including negative values.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public Interval InvertSimple()`
  Inverts a simple interval so the diatonic numbers sum to nine and semitones sum to twelve.
  - Returns: The complementary simple interval.
  - Throws `InvalidOperationException`: The interval is compound or the result cannot be represented.

- `public static bool operator ==(Interval left, Interval right)`
  Determines whether two values are equal.

- `public static bool operator !=(Interval left, Interval right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `IntervalDirection`

Specifies the direction in which an interval is measured or applied.

**Declaration**

```csharp
public enum IntervalDirection
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Ascending` | 0 | Moves upward in both diatonic and chromatic space. |
| `Descending` | 1 | Moves downward in both diatonic and chromatic space. |

### `IntervalQuality`

Represents an interval quality, including multiply augmented or diminished degrees.

**Declaration**

```csharp
public struct IntervalQuality
```

#### Constructors

- `public IntervalQuality(IntervalQualityKind kind, int degree = 0)`
  Creates and validates an interval quality.
  - `kind`: The base quality family.
  - `degree`: The augmentation or diminution degree; zero for major, minor, or perfect.

#### Properties

- `public int Degree { get; }`
  Gets the positive degree for augmented or diminished qualities, otherwise zero.

- `public IntervalQualityKind Kind { get; }`
  Gets the base quality family.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(IntervalQuality other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(IntervalQuality left, IntervalQuality right)`
  Determines whether two values are equal.

- `public static bool operator !=(IntervalQuality left, IntervalQuality right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `IntervalQualityKind`

Identifies the base quality family of a musical interval.

**Declaration**

```csharp
public enum IntervalQualityKind
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Diminished` | 0 | A quality below the minor or perfect reference. |
| `Minor` | 1 | The minor quality used by seconds, thirds, sixths, and sevenths. |
| `Perfect` | 2 | The perfect quality used by unisons, fourths, fifths, and octaves. |
| `Major` | 3 | The major quality used by seconds, thirds, sixths, and sevenths. |
| `Augmented` | 4 | A quality above the major or perfect reference. |

### `Transposition`

Applies exact intervals while preserving the required diatonic target letter.

**Declaration**

```csharp
public static class Transposition
```

#### Methods

- `public static SpelledPitch Transpose(SpelledPitch source, Interval interval, IntervalDirection direction = IntervalDirection.Ascending)`
  Transposes an octave-independent spelling by an interval.

- `public static Note Transpose(Note source, Interval interval, IntervalDirection direction = IntervalDirection.Ascending)`
  Transposes an octave-independent spelling by an interval.

## `Enaxos.MusicTheory.Midi`

### `MidiNote`

Converts between domain notes and MIDI 1.0 note numbers using the explicit C4 = 60 convention.

**Declaration**

```csharp
public static class MidiNote
```

#### Methods

- `public static Note FromNumber(int number, EnharmonicPreference preference = EnharmonicPreference.PreferSharps)`
  Converts a MIDI number to a conventionally spelled domain note.
  - `number`: A MIDI note number from 0 through 127.
  - `preference`: The spelling policy for chromatic pitch classes.

- `public static int ToNumber(Note note)`
  Converts a representable note to a MIDI number from 0 through 127.
  - Throws `ArgumentOutOfRangeException`: The note lies outside the MIDI range.

- `public static bool TryToNumber(Note note, out int number)`
  Attempts to convert a note to a MIDI number without throwing for range failure.

## `Enaxos.MusicTheory.Presentation`

### `AccidentalGlyphStyle`

Specifies which glyph family is used to display accidentals.

**Declaration**

```csharp
public enum AccidentalGlyphStyle
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Unicode` | 0 | Uses dedicated musical Unicode symbols where available. |
| `Ascii` | 1 | Uses portable ASCII sequences such as #, b, and ##. |

### `ChordNameStyle`

Specifies whether a chord is displayed with a lead-sheet suffix or a full quality name.

**Declaration**

```csharp
public enum ChordNameStyle
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `StandardAbbreviation` | 0 | Uses canonical abbreviations such as m7 and maj7. |
| `Full` | 1 | Uses a localized full chord-quality name. |

### `DegreeDisplayStyle`

Specifies the numeral system used to display scale degrees.

**Declaration**

```csharp
public enum DegreeDisplayStyle
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Arabic` | 0 | Uses Arabic digits from 1 through 12. |
| `Roman` | 1 | Uses uppercase Roman numerals from I through XII. |

### `MusicDisplayDefaults`

Stores thread-safe process-wide defaults used when a formatter call has no override.

**Declaration**

```csharp
public static class MusicDisplayDefaults
```

#### Properties

- `public static MusicTerminology Terminology { get; set; }`
  Gets or atomically sets the process-wide default terminology.

### `MusicFormatOptions`

Overrides presentation settings for one formatting operation.

**Declaration**

```csharp
public sealed class MusicFormatOptions
```

#### Constructors

- `public MusicFormatOptions()`
  Creates a new MusicFormatOptions instance.

#### Properties

- `public AccidentalGlyphStyle Accidentals { get; init; }`
  Gets the accidental glyph style used by the call.

- `public IFormatProvider? Culture { get; init; }`
  Gets the numeric culture, or null for invariant formatting.

- `public MusicTerminology? TerminologyOverride { get; init; }`
  Gets a per-call terminology override, or null to capture the global default.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(MusicFormatOptions other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(MusicFormatOptions left, MusicFormatOptions right)`
  Determines whether two values are equal.

- `public static bool operator !=(MusicFormatOptions left, MusicFormatOptions right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `MusicFormatter`

Formats domain values without changing their invariant identity or stored spelling.

**Declaration**

```csharp
public static class MusicFormatter
```

#### Methods

- `public static string Format(SpelledPitch pitch, MusicFormatOptions? options = null)`
  Formats a spelled pitch using localized note names and configured accidentals.

- `public static string Format(Note note, MusicFormatOptions? options = null)`
  Formats a note as a localized pitch followed by its scientific octave.

- `public static string Format(MusicalKey key, MusicFormatOptions? options = null)`
  Formats a key tonic and localized major/minor mode name.

- `public static string Format(ScaleDefinition definition, MusicFormatOptions? options = null)`
  Formats a known scale definition as a localized display name.

- `public static string Format(HarmonicFunction function, MusicFormatOptions? options = null)`
  Formats a harmonic function as a quality-sensitive Roman numeral.

- `public static string Format(Chord chord, ChordNameStyle style = ChordNameStyle.StandardAbbreviation, MusicFormatOptions? options = null)`
  Formats a chord using either a canonical suffix or a localized full quality name.

- `public static string Format(ScaleDegreeNumber degree, DegreeDisplayStyle style = DegreeDisplayStyle.Arabic, MusicFormatOptions? options = null)`
  Formats a scale degree using Arabic or Roman numerals.

- `public static string FormatChordPitches(Chord chord, MusicFormatOptions? options = null)`
  Formats chord tones as a space-separated sequence in formula order.

- `public static bool TryFormatChordName(Chord chord, out string name, ChordNameStyle style = ChordNameStyle.StandardAbbreviation, MusicFormatOptions? options = null, ChordRecognitionOptions? recognitionOptions = null)`
  Attempts to format a useful chord name, using recognition when the chord definition is not directly named.

- `public static bool TryFormatRomanNumeral(ScaleChord chord, out string romanNumeral, MusicFormatOptions? options = null)`
  Attempts to format a scale chord Roman numeral when its source scale supports that analysis.

### `MusicTerminology`

Specifies the language-specific system used for note and music-theory names.

**Declaration**

```csharp
public enum MusicTerminology
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `French` | 0 | Uses fixed-do note names and French quality terminology. |
| `American` | 1 | Uses letter note names and English-language American terminology. |

## `Enaxos.MusicTheory.Primitives`

### `Accidental`

Represents a written pitch alteration as an unrestricted signed semitone offset.

> Equality is based on the semitone count. Values outside the conventional double-flat through double-sharp range are supported because the domain model permits mathematical spellings such as (+3).

**Declaration**

```csharp
public struct Accidental
```

#### Properties

- `public static Accidental DoubleFlat { get; }`
  Gets the conventional double-flat accidental (-2 semitones).

- `public static Accidental DoubleSharp { get; }`
  Gets the conventional double-sharp accidental (+2 semitones).

- `public static Accidental Flat { get; }`
  Gets the conventional flat accidental (-1 semitone).

- `public static Accidental Natural { get; }`
  Gets the natural accidental (no chromatic alteration).

- `public int Semitones { get; }`
  Gets the signed chromatic alteration in semitones.

- `public static Accidental Sharp { get; }`
  Gets the conventional sharp accidental (+1 semitone).

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(Accidental other)`
  Determines whether this value equals another value.

- `public static Accidental FromSemitones(int semitones)`
  Creates an accidental from any signed semitone alteration.
  - `semitones`: The number of semitones by which the natural letter is altered.
  - Returns: An accidental carrying the exact supplied alteration.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(Accidental left, Accidental right)`
  Determines whether two values are equal.

- `public static bool operator !=(Accidental left, Accidental right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the invariant ASCII representation of the accidental.
  - Returns: An empty string for natural, conventional symbols up to double accidentals, or a parenthesized signed number for larger alterations.

### `EnharmonicPreference`

Specifies how a pitch class should be converted to a written pitch when no spelling is supplied.

**Declaration**

```csharp
public enum EnharmonicPreference
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `FewestAccidentals` | 0 | Uses the canonical spelling with the smallest accidental. |
| `PreferSharps` | 1 | Uses sharps for black-key pitch classes. |
| `PreferFlats` | 2 | Uses flats for black-key pitch classes. |

### `Note`

Represents a spelled pitch placed in a scientific-pitch-notation octave.

> The octave belongs to the written note. Enharmonic spellings can therefore have different octave numbers at a B/C boundary while still sharing the same absolute semitone.

**Declaration**

```csharp
public struct Note
```

#### Constructors

- `public Note(SpelledPitch pitch, int octave)`
  Creates a note from its written pitch and octave.
  - `pitch`: The pitch spelling, including its accidental.
  - `octave`: The scientific-pitch-notation octave.
  - Throws `ArgumentOutOfRangeException`: The computed absolute semitone cannot be represented.

#### Properties

- `public int AbsoluteSemitone { get; }`
  Gets the signed absolute semitone using the project convention where C0 is zero.

- `public int Octave { get; }`
  Gets the scientific-pitch-notation octave attached to the written pitch.

- `public SpelledPitch Pitch { get; }`
  Gets the written pitch component.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(Note other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public bool IsEnharmonicWith(Note other)`
  Determines whether another note denotes the same absolute sounding pitch.
  - `other`: The note to compare without considering spelling.
  - Returns: when both notes have the same absolute semitone.

- `public static bool operator ==(Note left, Note right)`
  Determines whether two values are equal.

- `public static bool operator !=(Note left, Note right)`
  Determines whether two values are different.

- `public static Note Parse(string text)`
  Parses an invariant note spelling such as C#4, Db-1, or F♯3.
  - `text`: The complete note text.
  - Returns: The parsed note.
  - Throws `ArgumentNullException`: `text` is `null`.
  - Throws `FormatException`: `text` is not a valid note.

- `public string ToString()`
  Returns the invariant ASCII spelling followed by its octave.

- `public static bool TryParse(ReadOnlySpan<char> text, out Note result)`
  Attempts to parse a complete invariant note spelling without throwing for invalid syntax.
  - `text`: The text to parse.
  - `result`: Receives the parsed note on success.
  - Returns: when parsing succeeds.

### `NoteLetter`

Identifies one of the seven diatonic note letters.

**Declaration**

```csharp
public enum NoteLetter
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `C` | 0 | The letter C. |
| `D` | 1 | The letter D. |
| `E` | 2 | The letter E. |
| `F` | 3 | The letter F. |
| `G` | 4 | The letter G. |
| `A` | 5 | The letter A. |
| `B` | 6 | The letter B. |

### `PitchClass`

Represents an octave-independent chromatic position normalized to the range 0 through 11.

**Declaration**

```csharp
public struct PitchClass
```

#### Fields

- `public const int Count = 12`
  The number of pitch classes in twelve-tone chromatic arithmetic.

#### Properties

- `public int Value { get; }`
  Gets the normalized chromatic index, where C is 0 and B is 11.

#### Methods

- `public int DistanceUpTo(PitchClass target)`
  Gets the ascending modular distance to another pitch class.
  - `target`: The destination pitch class.
  - Returns: A distance from 0 through 11 semitones.

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(PitchClass other)`
  Determines whether this value equals another value.

- `public static PitchClass FromChromaticIndex(int value)`
  Normalizes any signed chromatic index to a pitch class.
  - `value`: A possibly negative or octave-displaced chromatic index.
  - Returns: The corresponding normalized pitch class.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(PitchClass left, PitchClass right)`
  Determines whether two values are equal.

- `public static bool operator !=(PitchClass left, PitchClass right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the invariant decimal chromatic index.

### `PitchClassSpellings`

Exposes conventional spellings for normalized pitch classes.

**Declaration**

```csharp
public static class PitchClassSpellings
```

#### Properties

- `public static IReadOnlyList<SpelledPitch> NaturalPitches { get; }`
  Gets natural spellings in diatonic order from C through B.

#### Methods

- `public static IReadOnlyList<SpelledPitch> Chromatic(EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)`
  Returns one spelling for every chromatic pitch class from C through B.
  - `preference`: The enharmonic spelling policy used for non-natural pitch classes.
  - Returns: A read-only chromatic spelling table.

- `public static SpelledPitch For(PitchClass pitchClass, EnharmonicPreference preference = EnharmonicPreference.FewestAccidentals)`
  Returns the conventional spelling selected by the requested accidental preference.
  - `pitchClass`: The normalized pitch class to spell.
  - `preference`: The enharmonic spelling policy.
  - Returns: The selected conventional spelling.

### `SpelledPitch`

Represents a pitch spelling as a diatonic letter plus an accidental, without an octave.

> Equality preserves spelling; use to compare sounding pitch classes.

**Declaration**

```csharp
public struct SpelledPitch
```

#### Constructors

- `public SpelledPitch(NoteLetter letter, Accidental accidental)`
  Creates a spelled pitch.
  - `letter`: The diatonic letter.
  - `accidental`: The chromatic alteration applied to that letter.

#### Properties

- `public Accidental Accidental { get; }`
  Gets the written accidental.

- `public NoteLetter Letter { get; }`
  Gets the written diatonic letter.

- `public PitchClass PitchClass { get; }`
  Gets the octave-independent sounding pitch class produced by the spelling.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(SpelledPitch other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public bool IsEnharmonicWith(SpelledPitch other)`
  Determines whether another spelling denotes the same pitch class.

- `public static bool operator ==(SpelledPitch left, SpelledPitch right)`
  Determines whether two values are equal.

- `public static bool operator !=(SpelledPitch left, SpelledPitch right)`
  Determines whether two values are different.

- `public static SpelledPitch Parse(string text)`
  Parses a complete invariant spelling such as C#, Eb, or F𝄪.
  - Throws `ArgumentNullException`: `text` is `null`.
  - Throws `FormatException`: `text` is invalid.

- `public string ToString()`
  Returns the invariant ASCII spelling.

- `public static bool TryParse(ReadOnlySpan<char> text, out SpelledPitch result)`
  Attempts to parse a complete invariant pitch spelling.
  - Returns: when the entire input is valid.

## `Enaxos.MusicTheory.Scales`

### `ExoticScales`

Provides curated non-core scale definitions that are useful in jazz, blues, and modal color work.

**Declaration**

```csharp
public static class ExoticScales
```

#### Properties

- `public static ScaleDefinition DiminishedHalfWhole { get; }`
  Gets the historical diminished half-whole compatibility alias for `OctatonicHalfWhole`.

- `public static ScaleDefinition DiminishedWholeHalf { get; }`
  Gets the historical diminished whole-half compatibility alias for `OctatonicWholeHalf`.

- `public static ScaleDefinition OctatonicHalfWhole { get; }`
  Gets the half-step/whole-step octatonic scale definition.

- `public static ScaleDefinition OctatonicWholeHalf { get; }`
  Gets the whole-step/half-step octatonic scale definition.

- `public static IReadOnlyList<ScaleDefinition> All { get; }`
  Gets every curated exotic scale definition.

- `public static IReadOnlyList<ScaleDefinition> BluesAndBebop { get; }`
  Gets common blues and bebop scales.

- `public static IReadOnlyList<ScaleDefinition> Japanese { get; }`
  Gets curated Japanese pentatonic color scales.

- `public static IReadOnlyList<ScaleDefinition> RareMajorMinor { get; }`
  Gets less common major and minor family scales.

- `public static IReadOnlyList<ScaleDefinition> SymmetricAndJazz { get; }`
  Gets symmetric and jazz color scales.

- `public static IReadOnlyList<ScaleDefinition> WesternizedOriental { get; }`
  Gets westernized oriental color scales.

### `ModeCatalog`

Provides immutable catalogs of rotations for the supported scale parent collections.

**Declaration**

```csharp
public sealed class ModeCatalog
```

#### Properties

- `public IReadOnlyList<ScaleDefinition> All { get; }`
  Gets each distinct catalog definition used by recognition.

- `public IReadOnlyList<ScaleDefinition> AllWithExoticScales { get; }`
  Gets each distinct catalog definition used by recognition, including exotic scales.

- `public IReadOnlyList<ScaleDefinition> AllWithPentatonicAndExoticScales { get; }`
  Gets each distinct catalog definition used by recognition, including pentatonic and exotic scales.

- `public IReadOnlyList<ScaleDefinition> AllWithPentatonicScales { get; }`
  Gets each distinct catalog definition used by recognition, including pentatonic scales.

- `public IReadOnlyList<ScaleDefinition> ExoticScales { get; }`
  Gets curated exotic scale definitions.

- `public IReadOnlyList<ScaleDefinition> HarmonicMinorModes { get; }`
  Gets the seven generated rotations of harmonic minor.

- `public IReadOnlyList<ScaleDefinition> MajorModes { get; }`
  Gets the seven rotations of the major collection in modal order.

- `public IReadOnlyList<ScaleDefinition> MelodicMinorModes { get; }`
  Gets the seven generated rotations of ascending melodic minor.

- `public IReadOnlyList<ScaleDefinition> NaturalMinorModes { get; }`
  Gets the natural-minor rotations starting from Aeolian.

- `public IReadOnlyList<ScaleDefinition> PentatonicScales { get; }`
  Gets the standard major and minor pentatonic scales.

- `public static ModeCatalog Standard { get; }`
  Gets the shared catalog of standard modes.

#### Catalog semantics

The library exposes 52 distinct public scale definitions: 21 principal modes, 2 standard pentatonics, 25 exotic definitions, and four direct parent-collection definitions (`Major`, `NaturalMinor`, `HarmonicMinor`, and `MelodicMinorAscending`). The direct parents remain constructible through `StandardScales`, but are parallel to their corresponding first modes.

`ModeCatalog.Standard.AllWithPentatonicAndExoticScales` is the maximal recognition catalog and contains exactly 48 distinct candidates: 21 principal modes, 2 pentatonics, and 25 exotics. It deliberately excludes the four direct parent definitions to avoid duplicate collections during recognition. `ExoticScales.All` centralizes the 25 exotic definitions. `DiminishedWholeHalf` and `DiminishedHalfWhole` are compatibility aliases only; they do not add scale definitions or candidates.

### `PentatonicDerivation`

Records both the result and the provenance of a pentatonic derivation.

**Declaration**

```csharp
public sealed class PentatonicDerivation
```

#### Properties

- `public Scale Result { get; }`
  Gets the derived five-note scale.

- `public IReadOnlyList<int> SelectedDegrees { get; }`
  Gets the one-based positions retained from the source scale.

- `public Scale Source { get; }`
  Gets the original scale from which pitches were selected.

- `public PentatonicDerivationStrategy Strategy { get; }`
  Gets the strategy that produced the result.

### `PentatonicDerivationStrategy`

Specifies how a five-note scale is derived from a source scale.

**Declaration**

```csharp
public enum PentatonicDerivationStrategy
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `StandardMajorOrMinor` | 0 | Selects the unique standard major or minor pentatonic subset found in the source. |
| `SelectSourceDegrees` | 1 | Uses five explicitly supplied one-based positions from the source scale. |

### `PentatonicScales`

Derives traceable five-note subsets from realized source scales.

**Declaration**

```csharp
public static class PentatonicScales
```

#### Methods

- `public static PentatonicDerivation FromScale(Scale source, PentatonicDerivationStrategy strategy = PentatonicDerivationStrategy.StandardMajorOrMinor, IReadOnlyList<int>? sourceDegrees = null)`
  Derives a pentatonic scale or throws when the requested strategy cannot produce one.

- `public static bool TryFromScale(Scale source, out PentatonicDerivation? result, PentatonicDerivationStrategy strategy = PentatonicDerivationStrategy.StandardMajorOrMinor, IReadOnlyList<int>? sourceDegrees = null)`
  Attempts to derive a pentatonic scale without throwing for an incompatible source or selection.

### `PitchMatchMode`

Controls whether pitch collection comparisons preserve spelling or use enharmonic equivalence.

**Declaration**

```csharp
public enum PitchMatchMode
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `PitchClass` | 0 | Compares normalized pitch classes, so enharmonic spellings such as C# and Db match. |
| `ExactSpelling` | 1 | Compares the exact written pitch spelling, including letter and accidental. |

### `Scale`

Represents an immutable realization of a scale definition on a spelled tonic.

**Declaration**

```csharp
public sealed class Scale
```

#### Properties

- `public ScaleDefinition Definition { get; }`
  Gets the formula from which the scale was realized.

- `public IReadOnlyList<SpelledPitch> Pitches { get; }`
  Gets the immutable realized pitch spellings in formula order.

- `public SpelledPitch Tonic { get; }`
  Gets the tonic spelling used to realize the scale.

#### Methods

- `public static Scale Create(SpelledPitch tonic, ScaleDefinition definition)`
  Realizes every degree of a definition above a tonic while preserving diatonic spelling.

- `public SpelledPitch Degree(int number)`
  Gets a realized pitch by its formula degree number.

- `public bool Equals(Scale? other)`
  Compares tonic, definition, and realized spellings using value semantics.

- `public bool Equals(object obj)`
  Compares tonic, definition, and realized spellings using value semantics.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(Scale? left, Scale? right)`
  Determines whether two scales have equal value content.

- `public static bool operator !=(Scale? left, Scale? right)`
  Determines whether two scales differ.

### `ScaleDefinition`

Defines an immutable tonic-relative scale formula and its stable identifier.

**Declaration**

```csharp
public sealed class ScaleDefinition
```

#### Constructors

- `public ScaleDefinition(string id, IEnumerable<FormulaDegree> degrees)`
  Creates and validates a scale definition.
  - `id`: A stable ordinal identifier, not a localized display name.
  - `degrees`: Tonic-relative degrees in strictly ascending chromatic order.

#### Properties

- `public IReadOnlyList<FormulaDegree> Degrees { get; }`
  Gets the immutable ordered tonic-relative formula.

- `public string Id { get; }`
  Gets the stable, culture-independent definition identifier.

#### Methods

- `public bool Equals(ScaleDefinition? other)`
  Compares identifiers and every formula degree using value semantics.

- `public bool Equals(object obj)`
  Compares identifiers and every formula degree using value semantics.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(ScaleDefinition? left, ScaleDefinition? right)`
  Determines whether two definitions have identical value content.

- `public static bool operator !=(ScaleDefinition? left, ScaleDefinition? right)`
  Determines whether two definitions differ in identifier or formula.

### `ScaleRelations`

Provides set-style relationships between realized scales or pitch collections.

**Declaration**

```csharp
public static class ScaleRelations
```

#### Methods

- `public static IReadOnlyList<SpelledPitch> GetCommonNotes(Scale first, Scale second, params Scale[] others)`
  Returns pitches from the first scale that are present in every supplied scale by pitch class.

- `public static IReadOnlyList<SpelledPitch> GetCommonNotes(PitchMatchMode matchMode, Scale first, Scale second, params Scale[] others)`
  Returns pitches from the first scale that are present in every supplied scale using the requested match mode.

- `public static IReadOnlyList<SpelledPitch> GetCommonNotes(IEnumerable<SpelledPitch> first, IEnumerable<SpelledPitch> second, params IEnumerable<SpelledPitch>[] others)`
  Returns pitches from the first collection that are present in every supplied collection by pitch class.

- `public static IReadOnlyList<SpelledPitch> GetCommonNotes(PitchMatchMode matchMode, IEnumerable<SpelledPitch> first, IEnumerable<SpelledPitch> second, params IEnumerable<SpelledPitch>[] others)`
  Returns pitches from the first collection that are present in every supplied collection using the requested match mode.

### `ScaleStructure`

Represents the cyclic semitone pattern of a scale across one octave.

**Declaration**

```csharp
public sealed class ScaleStructure
```

#### Properties

- `public string CompactPattern { get; }`
  Gets the concatenated step-symbol pattern, such as WWHWWWH for a major scale.

- `public IReadOnlyList<int> SemitoneSteps { get; }`
  Gets the ascending semitone distance from each scale tone to the next, including octave closure.

- `public IReadOnlyList<string> StepSymbols { get; }`
  Gets compact symbols for each step, using W for two semitones and H for one semitone.

#### Methods

- `public string ToString()`
  Returns the compact pattern for diagnostic display.

### `ScaleStructures`

Calculates octave-normalized step structures for standard definitions and realized scales.

**Declaration**

```csharp
public static class ScaleStructures
```

#### Methods

- `public static ScaleStructure GetScaleStruct(string standardScaleId)`
  Returns the step structure for a standard scale or mode identified by its stable id.

- `public static ScaleStructure GetScaleStruct(ScaleDefinition definition)`
  Returns the step structure encoded by a scale definition.

- `public static ScaleStructure GetScaleStruct(Scale scale)`
  Returns the step structure of a realized scale in formula order.

- `public static ScaleStructure GetScaleStruct(IEnumerable<SpelledPitch> pitches)`
  Returns the step structure of an ordered pitch collection across one octave.

- `public static ScaleStructure GetScaleStruct(IEnumerable<Note> notes)`
  Returns the step structure of an ordered note collection, ignoring octaves after preserving pitch order.

### `ScaleFamily`

Classifies a scale definition by its originating collection.

**Declaration**

```csharp
public enum ScaleFamily
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Major` | 0 | The major diatonic collection and its modes. |
| `HarmonicMinor` | 1 | The harmonic-minor collection and its modes. |
| `MelodicMinor` | 2 | The ascending melodic-minor collection and its modes. |
| `Pentatonic` | 3 | A five-note pentatonic collection. |
| `Custom` | 4 | A caller-defined collection outside the standard catalogs. |

### `StandardScales`

Provides canonical immutable definitions for the standard scales and major modes.

**Declaration**

```csharp
public static class StandardScales
```

#### Properties

- `public static ScaleDefinition Aeolian { get; }`
  Gets Aeolian, the sixth mode of the major collection and the natural minor mode.

- `public static ScaleDefinition Dorian { get; }`
  Gets Dorian, the second mode of the major collection.

- `public static ScaleDefinition HarmonicMinor { get; }`
  Gets the harmonic minor scale.

- `public static ScaleDefinition Ionian { get; }`
  Gets Ionian, the first mode of the major collection.

- `public static ScaleDefinition Locrian { get; }`
  Gets Locrian, the seventh mode of the major collection.

- `public static ScaleDefinition Lydian { get; }`
  Gets Lydian, the fourth mode of the major collection.

- `public static ScaleDefinition Major { get; }`
  Gets the seven-degree major scale.

- `public static ScaleDefinition MajorPentatonic { get; }`
  Gets the standard major pentatonic scale (degrees 1, 2, 3, 5, and 6).

- `public static ScaleDefinition MelodicMinorAscending { get; }`
  Gets the ascending melodic minor scale.

- `public static ScaleDefinition MinorPentatonic { get; }`
  Gets the standard minor pentatonic scale (degrees 1, flat 3, 4, 5, and flat 7).

- `public static ScaleDefinition Mixolydian { get; }`
  Gets Mixolydian, the fifth mode of the major collection.

- `public static ScaleDefinition NaturalMinor { get; }`
  Gets the natural minor scale.

- `public static ScaleDefinition Phrygian { get; }`
  Gets Phrygian, the third mode of the major collection.

## `Enaxos.MusicTheory.Tonality`

### `HarmonicChordQuality`

Classifies the chord quality attached to a harmonic scale-degree interpretation.

**Declaration**

```csharp
public enum HarmonicChordQuality
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Major` | 0 | A major triadic quality, including supported major and dominant sevenths. |
| `Minor` | 1 | A minor triadic quality, including the supported minor seventh. |
| `Diminished` | 2 | A diminished triadic quality, including the fully diminished seventh. |
| `HalfDiminished` | 3 | A half-diminished seventh quality. |
| `Augmented` | 4 | An augmented triadic quality. |
| `Other` | 5 | A chord definition not mapped to a standard harmonic quality. |

### `HarmonicFunction`

Describes a chord's scale-degree, quality, and inversion interpretation within a key.

**Declaration**

```csharp
public struct HarmonicFunction
```

#### Constructors

- `public HarmonicFunction(ScaleDegreeNumber degree, HarmonicChordQuality quality, int inversionNumber)`
  Creates a harmonic-function value.

#### Properties

- `public ScaleDegreeNumber Degree { get; }`
  Gets the diatonic scale degree of the chord root.

- `public int InversionNumber { get; }`
  Gets the zero-based inversion number, where zero is root position.

- `public HarmonicChordQuality Quality { get; }`
  Gets the normalized harmonic chord quality.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(HarmonicFunction other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(HarmonicFunction left, HarmonicFunction right)`
  Determines whether two values are equal.

- `public static bool operator !=(HarmonicFunction left, HarmonicFunction right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

### `HarmonicFunctions`

Interprets chord roots and standard qualities in the context of a major or natural-minor key.

**Declaration**

```csharp
public static class HarmonicFunctions
```

#### Methods

- `public static HarmonicFunction Analyze(Chord chord, MusicalKey key)`
  Analyzes a chord or throws when its root is outside the supplied key.

- `public static bool TryAnalyze(Chord chord, MusicalKey key, out HarmonicFunction result)`
  Attempts to assign a diatonic harmonic function to a chord.

### `ScaleChord`

Describes one basic chord obtained by stacking alternate degrees of a supported scale.

**Declaration**

```csharp
public sealed class ScaleChord
```

#### Properties

- `public Chord Chord { get; }`
  Gets the realized chord with its root and chord-tone spellings.

- `public ScaleDegreeNumber Degree { get; }`
  Gets the one-based scale degree used as the chord root.

- `public HarmonicFunction Function { get; }`
  Gets the harmonic-function value that can be formatted as a Roman numeral.

- `public HarmonicChordQuality Quality { get; }`
  Gets the normalized quality used for Roman-numeral formatting.

- `public Scale SourceScale { get; }`
  Gets the realized scale from which the chord was derived.

### `ScaleHarmony`

Builds basic scale-degree chords from realized scales.

**Declaration**

```csharp
public static class ScaleHarmony
```

#### Methods

- `public static IReadOnlyList<ScaleChord> GetDiatonicTriads(Scale scale)`
  Returns one three-note chord per degree by stacking alternate tones of a scale.

### `KeyMode`

Identifies the major or minor mode of a tonal key.

**Declaration**

```csharp
public enum KeyMode
```

#### Values

| Name | Value | Description |
|---|---:|---|
| `Major` | 0 | A major key. |
| `Minor` | 1 | A minor key. |

### `KeyRelationships`

Computes standard relative and parallel relationships between major and minor keys.

**Declaration**

```csharp
public static class KeyRelationships
```

#### Methods

- `public static MusicalKey ParallelOf(MusicalKey key)`
  Gets the opposite mode on the same written tonic.

- `public static MusicalKey RelativeOf(MusicalKey key)`
  Gets the key sharing the same signature in the opposite mode.

### `KeySignature`

Represents a conventional key signature from seven flats through seven sharps.

**Declaration**

```csharp
public sealed class KeySignature
```

#### Properties

- `public Accidental Accidental { get; }`
  Gets the common accidental applied by the signature, or natural for an empty signature.

- `public int AccidentalCount { get; }`
  Gets the number of altered letters in the signature.

- `public IReadOnlyList<NoteLetter> AlteredLetters { get; }`
  Gets altered letters in conventional accumulation order.

- `public int Fifths { get; }`
  Gets the signed circle-of-fifths coordinate: negative for flats and positive for sharps.

#### Methods

- `public bool Equals(KeySignature? other)`
  Compares signatures by their signed fifths coordinate.

- `public bool Equals(object obj)`
  Compares signatures by their signed fifths coordinate.

- `public static KeySignature For(MusicalKey key)`
  Gets the conventional signature for a major or minor key spelling.
  - Throws `ArgumentException`: The spelling has no conventional signature in the supported range.

- `public static KeySignature FromFifths(int fifths)`
  Creates a signature from a signed fifths coordinate in the range -7 through +7.

- `public int GetHashCode()`
  Returns a hash code for this value.

### `MusicalKey`

Identifies a tonal key by its tonic spelling and major/minor mode.

**Declaration**

```csharp
public struct MusicalKey
```

#### Constructors

- `public MusicalKey(SpelledPitch tonic, KeyMode mode)`
  Creates a musical key.
  - `tonic`: The written tonic spelling.
  - `mode`: The major or minor key mode.

#### Properties

- `public KeyMode Mode { get; }`
  Gets the major or minor mode.

- `public SpelledPitch Tonic { get; }`
  Gets the tonic spelling.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(MusicalKey other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static MusicalKey Major(SpelledPitch tonic)`
  Creates a major key on the supplied tonic.

- `public static MusicalKey Minor(SpelledPitch tonic)`
  Creates a minor key on the supplied tonic.

- `public static bool operator ==(MusicalKey left, MusicalKey right)`
  Determines whether two values are equal.

- `public static bool operator !=(MusicalKey left, MusicalKey right)`
  Determines whether two values are different.

- `public static MusicalKey Parse(string text)`
  Parses a key using a major/minor suffix, a compact m suffix, or no suffix for major.

- `public string ToString()`
  Returns the invariant tonic followed by major or minor.

### `ScaleDegreeNumber`

Represents a validated one-based scale degree number.

**Declaration**

```csharp
public struct ScaleDegreeNumber
```

#### Constructors

- `public ScaleDegreeNumber(int value)`
  Creates a scale-degree number.
  - `value`: A value from 1 through 12.

#### Properties

- `public int Value { get; }`
  Gets the one-based degree value.

#### Methods

- `public bool Equals(object obj)`
  Determines whether this value equals another value.

- `public bool Equals(ScaleDegreeNumber other)`
  Determines whether this value equals another value.

- `public int GetHashCode()`
  Returns a hash code for this value.

- `public static bool operator ==(ScaleDegreeNumber left, ScaleDegreeNumber right)`
  Determines whether two values are equal.

- `public static bool operator !=(ScaleDegreeNumber left, ScaleDegreeNumber right)`
  Determines whether two values are different.

- `public string ToString()`
  Returns the string representation of this value.

## `Enaxos.MusicTheory.Tuning`

### `EqualTemperament12`

Implements twelve-tone equal temperament relative to a configurable reference pitch.

**Declaration**

```csharp
public sealed class EqualTemperament12
```

#### Constructors

- `public EqualTemperament12(Note? referenceNote = null, double referenceFrequency = 440)`
  Creates a twelve-tone equal-temperament tuning.
  - `referenceNote`: The reference note, or null for A4.
  - `referenceFrequency`: The positive reference frequency in hertz, defaulting to 440.

#### Properties

- `public double ReferenceFrequency { get; }`
  Gets the frequency assigned to `ReferenceNote`.

- `public Note ReferenceNote { get; }`
  Gets the note from which all frequency ratios are measured.

#### Methods

- `public double GetFrequency(Note note)`
  Gets a note's frequency using a ratio of 2^(semitone distance / 12).

- `public FrequencyMatch GetNearestNote(double frequency, EnharmonicPreference preference = EnharmonicPreference.PreferSharps)`
  Gets the nearest note to a positive measured frequency, its tuned frequency, and the deviation in cents.

### `FrequencyMatch`

Represents the nearest equal-temperament note for a measured frequency.

**Declaration**

```csharp
public sealed record FrequencyMatch(Note Note, double Frequency, double CentsDeviation)
```

#### Properties

- `public Note Note { get; }`
  Gets the nearest note in the selected spelling preference.

- `public double Frequency { get; }`
  Gets the exact frequency assigned to `Note` by the tuning system.

- `public double CentsDeviation { get; }`
  Gets the measured deviation from `Frequency`, in cents.

### `InstrumentFrequencyRange`

Describes an instrument's fundamental range and indicative upper harmonic energy.

**Declaration**

```csharp
public sealed record InstrumentFrequencyRange
```

#### Constructors

- `public InstrumentFrequencyRange(string instrumentName, double fundamentalLowFrequency, double fundamentalHighFrequency, double harmonicHighFrequency)`
  Creates a frequency-only range.

- `public InstrumentFrequencyRange(string instrumentName, Note fundamentalLowNote, Note fundamentalHighNote, double harmonicHighFrequency, ITuningSystem? tuningSystem = null)`
  Creates a range whose fundamentals are defined by notes in the supplied tuning.

#### Properties

- `public string InstrumentName { get; }`
- `public Note? FundamentalLowNote { get; }`
- `public Note? FundamentalHighNote { get; }`
- `public double FundamentalLowFrequency { get; }`
- `public double FundamentalHighFrequency { get; }`
- `public double HarmonicHighFrequency { get; }`

### `InstrumentFrequencyRangeGroup`

Groups instrument ranges by musical family.

**Declaration**

```csharp
public sealed record InstrumentFrequencyRangeGroup(string GroupName, IReadOnlyList<InstrumentFrequencyRange> Ranges)
```

### `InstrumentFrequencyRanges`

Provides documented, display-oriented frequency ranges for common instruments.

**Declaration**

```csharp
public static class InstrumentFrequencyRanges
```

#### Properties

- `public static IReadOnlyList<InstrumentFrequencyRangeGroup> DefaultGroups { get; }`
  Gets the default range groups for a broad 20 Hz to 20 kHz spectrum view.

### `ITuningSystem`

Defines a strategy that maps absolute musical notes to frequencies in hertz.

**Declaration**

```csharp
public interface ITuningSystem
```

#### Methods

- `public double GetFrequency(Note note)`
  Gets the frequency assigned to a note.
  - `note`: The absolute spelled note to evaluate.
  - Returns: A positive frequency in hertz.

---

Generated from the version 1.2.5 assembly and its compiler-validated XML documentation.

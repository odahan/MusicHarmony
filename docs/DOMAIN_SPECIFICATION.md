# Enaxos Music Theory — Domain Specification

Status: current specification aligned with the implementation
Target: .NET 8.0
Nature: maintained independent design

## 1. Intent

The engine represents music-theory concepts and derives results through explicit rules. It does not reproduce the architecture, algorithms, or implementation tables of the former Mingus port.

The normative sources for the implementation are:

- common definitions from Western tonal music theory;
- the conventions declared in this document;
- independent bibliography recorded before each module is implemented;
- the invariants and acceptance examples below.

The former project may be used to inventory use cases, but never to determine an algorithm, an internal structure, or a reference result.

## 2. Initial Scope

Version 1 covers:

- written pitches and their enharmonic equivalences;
- octave-qualified notes using scientific pitch notation;
- simple and compound intervals, ascending and descending;
- diatonic and chromatic transposition;
- scales and modes described by formulas;
- cyclic scale structures and common-note relationships;
- major and minor keys and their key signatures;
- chords described by degree formulas;
- voicings and inversions, separate from chord identity;
- diatonic triads derived from realized scales;
- French or American naming for notes, chords, modes, and harmonic functions;
- ranked chord recognition from a note sequence;
- ranked search for scales compatible with notes or a chord;
- standard pentatonic scales and their explicit derivation;
- the circle of fifths and its relationships;
- twelve-tone equal temperament (12-TET) as the default tuning;
- MIDI, formatting, and UI-geometry adapters with no graphics-framework dependency.

Out of scope for the initial version:

- counterpoint, voice leading, and automatic realization;
- ambiguous functional analysis without an explicit tonal context;
- historical temperaments;
- full rhythmic notation, measures, and MusicXML;
- statistical learning or probabilities calibrated from a musical corpus.

## 3. Conventions

### 3.1 Notes and Octaves

- Letters are `C D E F G A B`.
- The octave changes when moving from `B` to `C`.
- `C4` is middle C in scientific pitch notation and maps to MIDI number 60 in the MIDI adapter.
- The core model does not define a note by its MIDI number. It uses a relative chromatic index where `C0 = 0`; the MIDI adapter adds 12.
- Accidentals are integer chromatic displacements: flat `-1`, natural `0`, sharp `+1`, double flat `-2`, double sharp `+2`. The mathematical model is not limited to double accidentals, although parsers and UI layers may enforce a configurable safety limit.

### 3.2 Temperament

- Default sounding equivalence is evaluated in 12-TET.
- A note's spelling remains independent from its sounding equivalence.
- Frequency is calculated by a tuning service, never by harmony objects.
- The default reference is `A4 = 440 Hz`, and it is configurable.

### 3.3 Culture and Formatting

- Calculations are independent from language and culture.
- The core stores stable identifiers, not localized labels.
- Unicode symbols (`♭`, `♯`, `𝄫`, `𝄪`) and ASCII symbols (`b`, `#`, `bb`, `##`) are handled by dedicated parsers and formatters.
- The library has a default display terminology: French or American.
- Each formatting operation can locally override that default without changing the global default.
- The global default is atomic and presentation-only. No calculation or domain object depends on it.
- A formatting operation captures the default value once at entry, so it remains coherent if the global default changes concurrently.

Minimum terminology:

| Internal spelling | French | American |
|---|---|---|
| C D E F G A B | Do Ré Mi Fa Sol La Si | C D E F G A B |
| Major chord | majeur | major |
| Minor chord | mineur | minor |
| Mixolydian | mode de sol | Mixolydian |

`ToString()` remains invariant and diagnostic-oriented. User-facing output goes through the explicit formatter.

## 4. Domain Vocabulary

### 4.1 Note Letter

`NoteLetter` represents one of seven diatonic positions: `C` through `B`. Natural semitone positions in the octave are:

| Letter | C | D | E | F | G | A | B |
|---|---:|---:|---:|---:|---:|---:|---:|
| Position | 0 | 2 | 4 | 5 | 7 | 9 | 11 |

### 4.2 Accidental

`Accidental` represents a signed chromatic displacement applied to a letter. It never changes the diatonic letter.

### 4.3 Pitch Class

`PitchClass` represents a normalized chromatic value in `[0, 11]`. It contains no letter, accidental, or octave.

### 4.4 Spelled Pitch

`SpelledPitch` combines a letter and an accidental, without an octave.

Two spelled pitches are equal only when both their letter and accidental are equal. Sounding equivalence is a separate operation:

```text
C♯ != D♭                  spelling equality
C♯ enharmonicWith D♭      sounding equivalence in 12-TET
```

### 4.5 Note

`Note` combines a spelled pitch and an octave number. It contains no duration, velocity, or MIDI channel.

The absolute chromatic index is:

```text
12 × octave + naturalPosition(letter) + accidental
```

It is not normalized modulo 12. Therefore `B♯4` preserves its spelling while having the same sounding index as `C5`.

### 4.6 Interval

An `Interval` is canonically defined by:

- a positive diatonic number, starting at 1 for a unison;
- a signed chromatic distance, relative to ascending diatonic motion;
- a direction applied during transposition.

Most common intervals have a non-negative chromatic distance. A signed value still allows a diminished unison to be represented without a special case.

Quality is derived from the number and chromatic distance. Perfect classes are `1, 4, 5`; major classes are `2, 3, 6, 7`, repeated at each octave.

Reference distances for simple major or perfect intervals:

| Number | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Semitones | 0 | 2 | 4 | 5 | 7 | 9 | 11 | 12 |

For a compound interval, each group of seven degrees adds twelve semitones.

### 4.7 Formula Degree

`FormulaDegree` combines a degree number and a relative chromatic alteration. Examples:

- major third: `3`;
- minor third: `♭3`;
- augmented fifth: `♯5`;
- ninth: `9`.

### 4.8 Scale and Mode

A `ScaleDefinition` is a sequence of degrees relative to a tonic, ordered by ascending chromatic height. It contains no concrete note. A definition may contain several chromatic variants of the same degree number when musically justified, for example `♭3` and `3` in some blues or bebop scales.

Examples:

```text
Major            1  2  3  4  5  6  7
Natural minor    1  2 ♭3  4  5 ♭6 ♭7
Harmonic minor   1  2 ♭3  4  5 ♭6  7
```

A `Scale` is the result of applying a definition to a written tonic. Construction preserves the expected diatonic letters.

A scale structure is the cyclic sequence of semitone distances between consecutive tones, including octave closure. `W` and `H` represent whole and half steps; wider gaps remain numeric. This structure describes the chromatic contour of a scale, not its spelling.

Scale relationships expose common notes between multiple scales or pitch collections. The result preserves the order and spelling of the first collection. Comparison can use pitch classes or exact spelling.

The main standard catalog covers at least:

- the seven modes of the major scale: Ionian, Dorian, Phrygian, Lydian, Mixolydian, Aeolian, and Locrian;
- the seven rotations of the natural minor scale, exposed as relative views of the same diatonic collections;
- the seven rotations of the harmonic minor scale;
- the seven rotations of the ascending melodic minor scale.

Two optional catalogs complement this core: standard pentatonics and a curated `exotic` catalog. The `exotic` catalog contains common symmetric and jazz scales, blues and bebop scales, rare major and minor families, Westernized oriental colors, and Japanese scales. It deliberately excludes encyclopedic catalogs, Indonesian approximations, and Hindustani ragas.

Because names for some minor modes are not universal, each definition has a stable identifier and may expose several localized aliases. For major-scale modes, default French terminology is `mode de do`, `mode de ré`, `mode de mi`, `mode de fa`, `mode de sol`, `mode de la`, `mode de si`; American terminology uses `Ionian`, `Dorian`, `Phrygian`, `Lydian`, `Mixolydian`, `Aeolian`, `Locrian`.

### 4.9 Key and Key Signature

A `MusicalKey` combines a written tonic and a major or minor mode.

A `KeySignature` is canonically represented by a signed number of fifths:

- positive: number of sharps, in order `F C G D A E B`;
- negative: number of flats, in order `B E A D G C F`;
- zero: no key-signature accidental.

Version 1 supports conventional key signatures from `-7` through `+7`.

### 4.10 Chord, Voicing, and Inversion

A `ChordDefinition` is a sequence of relative degrees. Examples:

```text
Major              1  3  5
Minor              1 ♭3  5
Dominant seventh   1  3  5 ♭7
Diminished seventh 1 ♭3 ♭5 𝄫7
```

A `Chord` combines a written root and a definition. It exposes:

- a standard symbolic identity independent from language;
- its ordered series of written pitches in root position;
- formatted abbreviated or full names through the presentation service.

Representations of the same chord:

| Style | French | American |
|---|---|---|
| Abbreviated | Do m7 | Cm7 |
| Full | Do mineur septième | C minor seventh |
| Notes | Do Mib Sol Sib | C Eb G Bb |

A `ChordRealization` is an ordered sequence of octave-qualified notes realizing a chord. Inversion is derived from the chord degree placed in the bass. Changing the realization or inversion does not change chord identity.

A transposed chord and a transformed realization preserve provenance:

- `OriginalChord` designates the chord before any transformation;
- `SemitoneDeltaFromOriginal` preserves the exact signed displacement, without modulo-12 reduction;
- `InversionNumber` is `0` in root position, `1` in first inversion, and so on;
- a new transposition accumulates its displacement with the already stored delta;
- an inversion preserves the original chord and transposition delta;
- requesting another inversion replaces the current inversion number; it does not add to it.

Transposing a theoretical chord does not require choosing an octave. It returns a derived object containing the original chord, the resulting chord, and the delta. The octave-qualified realization then adds voicing and inversion information.

### 4.11 Degrees and Harmonic Functions

`ScaleDegreeNumber` is between `1` and `12`, so the library can represent supported scales of different sizes. It can be displayed as an Arabic number. Roman-numeral display remains reserved for harmonic functions known in a heptatonic tonal context.

Default convention:

- major chord: uppercase Roman numeral, for example `I`, `IV`, `V`;
- minor chord: lowercase Roman numeral, for example `ii`, `iii`, `vi`;
- diminished chord: lowercase Roman numeral followed by `°`, for example `vii°`;
- half-diminished chord: lowercase Roman numeral followed by `ø`;
- augmented chord: uppercase Roman numeral followed by `+`.

A harmonic function is never inferred without an explicit key. The same chord may have several functions depending on context.

Diatonic triads of a scale are built by stacking alternate degrees from each scale degree. Major, minor, diminished, and augmented qualities are recognized when they match standard definitions; other formulas remain available with quality `Other`. For heptatonic scales, these results can be formatted as Roman numerals.

### 4.12 Chord Recognition

Recognition accepts a sequence of `Note` or a free chord. It ignores octave to identify pitch classes, but uses bass to determine inversion when ordering or octaves are available.

It returns an ordered list of candidates, because the same sounding collection may allow several roots or spellings. Each candidate contains:

- the recognized theoretical chord;
- the proposed root;
- the optional inversion;
- the recognized bass when determinable;
- the recognized input pitches;
- missing or added notes;
- a raw score and normalized confidence.

An exact match is ranked before a match with omissions or additions. Note spelling contributes to the ranking, but enharmonic equivalence can be enabled by option. Abbreviated or full names are produced later by the presentation service.

### 4.13 Compatible Scale Search

The search accepts a sequence of `Note` or a `Chord`. It evaluates every tonic and every definition in the selected catalog.

By default, the recognizer searches in the main standard catalog. `IncludePentatonicCandidates` and `IncludeExoticCandidates` add standard pentatonics and the `exotic` catalog to that default catalog. When an explicit `Catalog` is supplied, it becomes the sole source of candidate definitions.

Ranking explicitly and configurably accounts for:

- membership of observed notes in the candidate scale;
- coverage of the scale by distinct observed notes;
- notes outside the scale;
- consistency of diatonic spelling;
- root and bass of a chord when known;
- possible tonic evidence in the input data.

Each result exposes a `Score` and a `RelativeProbability` in `[0, 1]`. Relative probabilities of returned candidates sum to 1. This value is a heuristic normalization that depends on the catalog and weights; it is not a statistically calibrated probability.

### 4.14 Pentatonic Scales

Standard definitions are:

```text
Major pentatonic  1  2  3  5  6
Minor pentatonic  1 ♭3  4  5 ♭7
```

Automatic pentatonic derivation is unambiguous only for a scale compatible with one of these formulas. The API therefore provides:

- a `StandardMajorOrMinor` strategy, used by default, which explicitly fails if no standard pentatonic is contained in the source scale;
- a `SelectSourceDegrees` strategy that extracts five positions chosen by the caller;
- a result that preserves the source scale and derivation strategy.

The API never silently replaces a diminished fifth or another absent note to fabricate a standard pentatonic.

### 4.15 Exotic Scales

`ExoticScales` groups non-core definitions useful in jazz, blues, and modal-color contexts:

- `SymmetricAndJazz`: whole tone, diminished, augmented;
- `BluesAndBebop`: minor blues, major blues, dominant, major, and Dorian bebop;
- `RareMajorMinor`: harmonic major, double harmonic major, Hungarian minor, Ukrainian Dorian, Neapolitan major and minor;
- `WesternizedOriental`: Persian, Arabian, Spanish Phrygian, Oriental, Egyptian;
- `Japanese`: Hirajoshi, Insen, Iwato, Yo, Kumoi.

These scales are available for recognition only when explicitly enabled. They are also exposed as catalog definitions so consumers can offer them directly in UIs and derive scale chords when musically useful.

## 5. Calculation Rules

### 5.1 Chromatic Class

```text
pitchClass(spelledPitch) =
    modulo12(naturalPosition(letter) + accidental.semitones)
```

The mathematical modulo always returns a value from 0 to 11.

### 5.2 Written Transposition

To transpose a note by an interval:

1. determine the target letter from the diatonic distance;
2. determine the target chromatic index from the semitone distance;
3. determine the natural octave of the target letter;
4. calculate the accidental required to reach the target chromatic index;
5. preserve that spelling, even if an enharmonic spelling seems simpler.

Normative examples:

```text
C4 + major third = E4
C♯4 + major third = E♯4
D♭4 + major third = F4
B♯4 + major second = C𝄪5
```

Enharmonic simplification is an explicit operation controlled by a strategy; it is not part of transposition.

### 5.3 Interval Between Two Notes

The interval between two written notes depends on:

- the distance between their letters and octaves;
- the distance between their absolute chromatic indexes.

`C♯4 → D♭4` is therefore a diminished second, even though the sounding distance is zero in 12-TET.

### 5.4 Scale Construction

Each degree is obtained by transposing the tonic according to its diatonic number and relative chromatic alteration. A diatonic heptatonic scale uses each letter exactly once before octave repetition.

A scale structure is calculated from the chromatic offsets of its formula or from the pitch classes of an ordered collection. A valid structure contains at least two tones, has no duplicate pitch class, and closes exactly one octave.

### 5.5 Chord Construction

Each theoretical tone is obtained from the root by the formula degree. Doublings, omissions, and dispositions belong to the octave-qualified realization, not to `Chord`.

A diatonic triad is built from a scale by taking the current degree, the degree two positions later, and the degree four positions later, with cyclic wraparound in the scale.

### 5.6 Chord Transposition and Inversion

Chromatic transposition of a chord applies the same signed delta to all notes and to the root. An explicit spelling strategy determines the result spelling. The stored delta remains the one requested by the caller: `+14` is not reduced to `+2`.

Inversion successively moves the lowest notes up an octave while preserving their spelling. For a chord with `n` distinct tones, valid inversions are `0..n-1`.

### 5.7 Frequency in 12-TET

For MIDI index `m`, reference `A4 = f`, and `A4 = 69`:

```text
frequency(m) = f × 2 ^ ((m - 69) / 12)
```

Note duration never participates in this formula.

## 6. Circle of Fifths

### 6.1 Harmonic Model

- The circle contains exactly twelve chromatic positions.
- A clockwise move adds an ascending perfect fifth.
- A counterclockwise move adds an ascending perfect fourth, equivalent to a descending fifth.
- Each position exposes a major key and its relative minor, sharing the same key signature.
- Some positions expose several conventional enharmonic spellings.

Proposed signatures by position, with `C major` at index 0:

| Index | Available signatures | Major keys |
|---:|---|---|
| 0 | 0 | C |
| 1 | +1 | G |
| 2 | +2 | D |
| 3 | +3 | A |
| 4 | +4 | E |
| 5 | +5, -7 | B, C♭ |
| 6 | +6, -6 | F♯, G♭ |
| 7 | +7, -5 | C♯, D♭ |
| 8 | -4 | A♭ |
| 9 | -3 | E♭ |
| 10 | -2 | B♭ |
| 11 | -1 | F |

The primary spelling is selected by an explicit strategy: fewest accidentals, then sharp or flat preference when tied.

### 6.2 UI Model

The domain exposes positions and relationships. An independent geometry module exposes:

- start angle, center angle, and sweep of each sector;
- normalized coordinates in `[-1, 1]`;
- an anchor point for the major label;
- an anchor point for the minor label;
- no color, font, command, or framework-specific class.

A UI can therefore draw the circle with its own graphics system without reimplementing position calculations.

## 7. Invariants

### 7.1 Values and Equality

- `INV-P01` — A `PitchClass` is always in `[0, 11]`.
- `INV-P02` — `SpelledPitch` equality compares letter and accidental.
- `INV-P03` — Enharmonic equivalence compares pitch classes, never object equality.
- `INV-P04` — `Note` contains no rhythmic or MIDI property.
- `INV-P05` — Equal objects always have the same hash code.

### 7.2 Intervals and Transposition

- `INV-I01` — An interval number is greater than or equal to 1.
- `INV-I02` — A transposition respects both diatonic distance and chromatic distance.
- `INV-I03` — A transposition never implicitly simplifies spelling.
- `INV-I04` — Transposing by an interval and then by its opposite restores the exact original written note.
- `INV-I05` — Inversion of two complementary simple intervals totals nine degrees and twelve semitones.

### 7.3 Scales and Keys

- `INV-S01` — A scale definition has unique ordered degrees and starts with natural `1`.
- `INV-S02` — A diatonic heptatonic scale uses all seven letters exactly once.
- `INV-S03` — Optional final tonic repetition belongs to presentation, not scale identity.
- `INV-S04` — A valid scale structure closes exactly twelve semitones.
- `INV-S05` — Common notes preserve the order and spelling of the first collection.
- `INV-K01` — A conventional key signature contains between zero and seven accidentals of a single type.
- `INV-K02` — A major key and its relative minor have the same key signature.

### 7.4 Chords

- `INV-C01` — A chord definition contains natural root `1`.
- `INV-C02` — Definition degrees are unique.
- `INV-C03` — Each note in a voicing is enharmonically compatible with a chord degree unless an extension is explicitly allowed.
- `INV-C04` — Changing a voicing or inversion does not change chord identity.
- `INV-C05` — A chord always exposes a symbolic identity and written pitches coherent with its definition.
- `INV-C06` — A transposition delta is signed, exact, and measured from the original chord.
- `INV-C07` — An inverted chord preserves the original chord and a valid inversion number.
- `INV-C08` — Transposition does not change the inversion number, and inversion does not change the transposition delta.

### 7.5 Naming and Analysis

- `INV-N01` — French and American formatting of an object never modifies that object.
- `INV-N02` — A local override never changes the global display default.
- `INV-N03` — Roman numerals are produced only with an explicit tonal context.
- `INV-N04` — Diminished chords use `°` by default and half-diminished chords use `ø`.
- `INV-N05` — A Roman numeral derived from a `ScaleChord` is produced only for a heptatonic scale.
- `INV-R01` — Recognition candidates are sorted by descending score with a deterministic tie-breaker.
- `INV-R02` — Relative probabilities of returned scales sum to 1 within numeric tolerance.
- `INV-R03` — Raw score and explanatory score factors remain accessible.
- `INV-R04` — Strict search accepts no note outside the candidate scale.

### 7.6 Circle of Fifths

- `INV-F01` — The circle has exactly twelve ordered segments.
- `INV-F02` — Indexes are normalized modulo 12.
- `INV-F03` — Neighboring segments are separated by a perfect fifth clockwise.
- `INV-F04` — The major key and relative minor of a spelling share exactly the same key signature.
- `INV-F05` — A conventional key from `-7` to `+7` belongs to one segment.
- `INV-F06` — Geometry produces twelve 30-degree sectors with no overlap or gap.

### 7.7 Software Quality

- `INV-A01` — All domain objects are immutable and safe for concurrent reads.
- `INV-A02` — No calculation depends on mutable global state.
- `INV-A03` — A public collection is never mutable by the consumer.
- `INV-A04` — Business calculations are deterministic for identical parameters; formatting is deterministic for an identical captured display configuration.
- `INV-A05` — `Parse` throws a documented `FormatException`; `TryParse` never throws for invalid user input.
- `INV-A06` — Business calculations do not use strings as intermediate representation.
- `INV-A07` — The only mutable global default allowed is display terminology, and its access is atomic.

## 8. Acceptance Examples

```text
parse("C#4") = Note(SpelledPitch(C, Sharp), 4)
parse("Db4") = Note(SpelledPitch(D, Flat), 4)
C#4 != Db4
C#4 enharmonicWith Db4
midi(C4) = 60
frequency(A4, A4=440Hz) = 440Hz
transpose(C#4, major third) = E#4
interval(C#4, Db4) = diminished second, 0 semitone
majorScale(F#) = F# G# A# B C# D# E#
scaleStruct(major) = WWHWWWH
commonNotes(C major, G major) = C D E G A B
majorChord(Db) = Db F Ab
diatonicTriads(C major) = C, Dm, Em, F, G, Am, Bdim
format(C minor seventh, French, abbreviated) = "Do m7"
format(C minor seventh, American, full) = "C minor seventh"
romanNumeral(B diminished, key=C major) = "vii°"
transpose(realized C major, +14).deltaFromOriginal = +14
invert(realized C major, 2).originalChord = C major
invert(realized C major, 2).inversionNumber = 2
dominantOf(Eb major) = Bb major
subdominantOf(Eb major) = Ab major
relativeMinorOf(Eb major) = C minor
```

## 9. Test Strategy

Tests are organized by rules, not by methods:

- exhaustive tests of the twelve chromatic classes;
- tests of all simple interval qualities and a set of compound intervals;
- property-style tests for round-trip transposition;
- tests of standard scales in all conventional keys;
- tests of scale structures and common notes by pitch class and exact spelling;
- chord tests from several roots with accidentals;
- tests of diatonic triads and their Roman-numeral formatting;
- tests of provenance after chains of transpositions and inversions;
- tests of French/American names with the global default and concurrent overrides;
- tests of Roman numerals in several keys;
- tests of exact, ambiguous, omitted, and added-tone recognition;
- tests of scale ranking and relative-probability normalization;
- tests of major, harmonic minor, and melodic minor modes;
- tests of pentatonic derivation and explicit failure;
- tests of equality, enharmonic equivalence, and hash-code stability;
- tests of the fifteen key signatures `-7..+7` and their placement on twelve segments;
- rendering-independent geometry tests;
- Unicode and ASCII parsing tests;
- limit tests to avoid excessive inputs.

Historical Mingus results are not test oracles.

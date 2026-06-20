# Provenance

## Clean-room scope

This repository is an independent implementation. Its design and behavior are
derived from the normative specifications listed below and from independently
published music-theory references recorded per module.

No source code, tests, generated artifacts, implementation tables, algorithms,
or runtime observations from HarmonyLibOD, Mingus, or `Enaxos.Harmony.Core`
were consulted or copied.

## Authorized normative inputs

The following two files are the only artifacts copied from HarmonyLibOD. They
are retained verbatim in `docs/`.

| Document | Original path | SHA-256 |
|---|---|---|
| `DOMAIN_SPECIFICATION.md` | `E:\ExJ\Enaxos.Harmony\HarmonyLibOD\docs\DOMAIN_SPECIFICATION.md` | `1781C9681D2F7910827F4F9494E983286E7AA394FE4FBFAEC937993EBF014548` |
| `PUBLIC_API_PROPOSAL.md` | `E:\ExJ\Enaxos.Harmony\HarmonyLibOD\docs\PUBLIC_API_PROPOSAL.md` | `4700442475EBBDFAD958E34D541DF1829F79EE97911EA6C0622AEDD7C9112A59` |

If an independent reference differs from these project conventions, the two
normative documents above take precedence.

## Independent bibliography by module

### Primitives

Consulted on 2026-06-20:

- Open Music Theory, **“Pitches and octave designations”**, source revision
  `0081e1db0ddf9dee66960c2f98108f5dbc424662`,
  <https://openmusictheory.github.io/pitches.html>. Used for pitch versus pitch
  class, enharmonic equivalence, scientific octave designation, `C4` as middle
  C, and the octave boundary from B to C.
- Open Music Theory, **“Pitch (class)”**, source revision
  `8faac39af99ea5d57473695a5570bc1805b513ef`,
  <https://openmusictheory.github.io/pitch(Class).html>. Used for the distinction
  between spelling equality and enharmonic equivalence, and for twelve-tone
  pitch-class integer notation with C as 0.

Project-specific choices such as unlimited mathematical accidentals,
`AbsoluteSemitone = 12 * octave + natural position + accidental`, accepted
ASCII/Unicode syntax, and exact equality semantics come directly from the
normative documents.

### Intervals and transposition

Consulted on 2026-06-20:

- Open Music Theory, **“Intervals and dyads”**, source revision
  `a97156a10595c4252c6e8a4a524fe1ce1cdc0c24`,
  <https://openmusictheory.github.io/intervals.html>. Used for inclusive
  diatonic interval numbers, chromatic distance in semitones, the
  perfect/major interval classes, compound intervals, and the inversion rules
  whose diatonic numbers total nine and chromatic distances total twelve.
- Open Music Theory, **“Transposition”**, source revision
  `1ca8c9488f2ba7cb20a674f0e647effbcd19f956`,
  <https://openmusictheory.github.io/transposition.html>. Used for transposition
  as a distance-preserving operation and for the distinction between an
  operation on written pitches and pitch-class arithmetic modulo twelve.

The signed chromatic distance used for diminished unisons, exact preservation
of diatonic spelling, direction model, compound-interval representation, and
round-trip spelling requirement are project conventions defined by the
normative documents.

### Formulas, scales, modes, and pentatonics

Consulted on 2026-06-20:

- Open Music Theory, **“Scales and scale degrees”**, source revision
  `cdfe920ec85b3e297afd60c359fea1a30a3acff9`,
  <https://openmusictheory.github.io/scales.html>. Used for tonic-relative scale
  degrees, seven-note major and natural-minor structures, harmonic minor, and
  ascending melodic minor.
- Open Music Theory, **“Collections and Scales”**, source revision
  `0f037934ffcc7305a2f45dacdce095335260ced3`,
  <https://openmusictheory.github.io/scales2.html>. Used for the seven rotations
  of the diatonic collection, their modal identities, transposition to any
  tonic, and the major pentatonic subset `1 2 3 5 6`. The standard minor
  pentatonic `1 ♭3 4 5 ♭7` is the corresponding rotation of that five-note
  collection.

Stable identifiers, formula validation, catalogs of harmonic- and
melodic-minor rotations, preservation of written letters, and explicit
pentatonic derivation strategies are project conventions defined by the
normative documents.

### Tonality and circle of fifths

Consulted on 2026-06-20:

- Open Music Theory, **“Key signatures”**, source revision
  `24652431817011647a1c966b40f9171490273e4f`,
  <https://openmusictheory.github.io/keySignatures.html>. Used for the order of
  sharps `F C G D A E B`, the reverse order of flats, relative major/minor
  signatures, and clockwise/counterclockwise motion on the circle of fifths.

The signed `-7..+7` representation, conventional enharmonic spellings, primary
spelling policy, and twelve-segment API are project conventions from the
normative documents.

### Harmony, harmonic functions, and recognition

Consulted on 2026-06-20:

- Open Music Theory, **“Triads and seventh chords”**, source revision
  `8a233f74ec45b5e4a832e22b8cad7c806819d3d6`,
  <https://openmusictheory.github.io/triads.html>. Used for chord roots,
  thirds/fifths/sevenths, standard triad and seventh-chord formulas, bass-based
  inversion, lead-sheet symbols, and Roman-numeral quality conventions.
- Open Music Theory, **“Harmonic functions”**, source revision
  `e5f5bf363bc1bf08db9e1bd632baa6570d1045cc`,
  <https://openmusictheory.github.io/harmonicFunctions.html>. Used for the rule
  that harmonic interpretation depends on scale-degree context.
- Open Music Theory, **“Collections and Scales”**, source revision
  `0f037934ffcc7305a2f45dacdce095335260ced3`,
  <https://openmusictheory.github.io/scales2.html>. Used for tonic-relative
  collections considered by compatible-scale recognition.

Transformation provenance, deterministic candidate scoring, missing/added
tones, enharmonic ranking, softmax relative probabilities, and configurable
analysis weights are project conventions from the normative documents.

### Geometry and presentation

Consulted on 2026-06-20:

- Microsoft .NET API reference, **`System.Math.Sin`**, .NET 8 view,
  <https://learn.microsoft.com/en-us/dotnet/api/system.math.sin?view=net-8.0>.
  Used with the corresponding cosine definition for normalized polar label
  anchors.
- The Open Music Theory key-signature and harmony references above were used
  for circle labels, note names, chord-quality abbreviations, and Roman-numeral
  conventions.

Coordinate orientation, sector angles, French/American terminology, atomic
display defaults, and exact formatter outputs are project conventions from the
normative documents.

### Tuning and MIDI

Consulted on 2026-06-20:

- ISO 16:1975, **Acoustics — Standard tuning frequency (Standard musical pitch)**.
  Used for the `A4 = 440 Hz` reference convention.
- The MIDI Association, **“The MIDI Association offers free download of the
  MIDI 1.0 Specifications document”**,
  <https://midi.org/the-midi-association-offers-free-download-of-the-midi-1-0-specifications-document>.
  Used as the primary MIDI 1.0 specification source.

The explicit `C4 = 60` adapter offset, accepted MIDI range `0..127`, spelling
preferences, and equal-temperament frequency formula are project conventions
from the normative documents.

## Implementation ledger

| Lot | Module | Normative sections | Independent references recorded before implementation | Status |
|---|---|---|---|---|
| 1 | Primitives | Domain §§3.1, 3.2, 4.1–4.5, 5.1, 7.1, 7.7, 8; API §3 | Open Music Theory references above | Complete (2026-06-20) |
| 2 | Intervals and transposition | Domain §§4.6, 5.2, 5.3, 7.2, 8; API §4 | Open Music Theory references above | Complete (2026-06-20) |
| 3 | Formulas, scales, modes, and pentatonics | Domain §§4.7, 4.8, 4.14, 5.4, 7.3, 8; API §5 | Open Music Theory references above | Complete (2026-06-20) |
| 4 | Tonality and key signatures | Domain §§4.9, 4.11, 7.3; API §6 | Open Music Theory key-signature reference | Complete (2026-06-20) |
| 5 | Harmony and harmonic functions | Domain §§4.10, 4.11, 5.5, 5.6, 7.4; API §§6–7 | Open Music Theory harmony references | Complete (2026-06-20) |
| 6 | Chord and scale analysis | Domain §§4.12, 4.13, 7.5; API §§7.1–7.2 | Open Music Theory harmony and collection references | Complete (2026-06-20) |
| 7 | Circle of fifths | Domain §6.1, §7.6; API §9.1 | Open Music Theory key-signature reference | Complete (2026-06-20) |
| 8 | Geometry and presentation | Domain §§3.3, 6.2, 7.5–7.7; API §§9.2, 10 | Microsoft .NET and Open Music Theory references | Complete (2026-06-20) |
| 9 | Tuning and MIDI | Domain §§3.1, 3.2, 5.7, 8; API §8 | ISO 16 and MIDI Association references | Complete (2026-06-20) |

Any later module must add its bibliography entry here before its production
implementation is written.

## Toolchain and dependencies

- Target framework: .NET 8 (`net8.0`).
- SDK selected by `global.json`: 8.0.422.
- Production library: no third-party runtime dependency.
- Test framework dependencies are declared explicitly in the test project and
  are used only to execute tests; they are not sources for domain behavior.

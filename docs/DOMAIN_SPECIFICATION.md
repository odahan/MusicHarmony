# Enaxos Music Theory — Spécification du domaine

Statut : proposition initiale  
Cible : .NET 8.0  
Nature : nouvelle conception indépendante

## 1. Intention

Le nouveau moteur représente les concepts de théorie musicale et en déduit les résultats par des règles explicites. Il ne reproduit ni l'architecture, ni les algorithmes, ni les tables d'implémentation de l'ancien port de Mingus.

Les sources normatives de l'implémentation seront :

- les définitions usuelles de la théorie musicale occidentale tonale ;
- les conventions déclarées dans ce document ;
- une bibliographie indépendante à enregistrer avant l'implémentation de chaque module ;
- les invariants et exemples d'acceptation ci-dessous.

L'ancien projet peut servir à inventorier des cas d'usage, mais jamais à déterminer un algorithme, une structure interne ou un résultat de référence.

## 2. Périmètre initial

La version 1 couvre :

- les hauteurs écrites et leurs équivalences enharmoniques ;
- les notes octaviées selon la notation scientifique ;
- les intervalles simples et composés, ascendants et descendants ;
- les transpositions diatoniques et chromatiques ;
- les gammes et modes décrits par des formules ;
- les tonalités majeures et mineures et leurs armures ;
- les accords décrits par des formules de degrés ;
- les voicings et renversements, séparés de l'identité d'un accord ;
- la représentation et le nommage français ou américain des notes, accords, modes et fonctions harmoniques ;
- la reconnaissance classée d'accords depuis une série de notes ;
- la recherche classée des gammes compatibles avec des notes ou un accord ;
- les gammes pentatoniques standard et leur dérivation explicite ;
- le cercle des quintes et ses relations ;
- le tempérament égal à douze divisions de l'octave (12-TET) comme accordage par défaut ;
- des adaptateurs MIDI, de formatage et de géométrie UI sans dépendance à un framework graphique.

Hors périmètre initial :

- contrepoint, conduite de voix et réalisation automatique ;
- analyse fonctionnelle ambiguë sans contexte tonal explicite ;
- tempéraments historiques ;
- notation rythmique complète, mesures et MusicXML ;
- apprentissage statistique ou probabilités calibrées depuis un corpus musical.

## 3. Conventions

### 3.1 Notes et octaves

- Les lettres sont `C D E F G A B`.
- L'octave change au passage de `B` vers `C`.
- `C4` est le do central selon la notation scientifique et correspond au numéro MIDI 60 dans l'adaptateur MIDI.
- Le noyau ne définit pas une note par son numéro MIDI. Il utilise un index chromatique relatif où `C0 = 0`; l'adaptateur MIDI ajoute 12.
- Les altérations sont des déplacements chromatiques entiers : bémol `-1`, naturel `0`, dièse `+1`, double bémol `-2`, double dièse `+2`. Le modèle mathématique n'est pas limité aux doubles altérations, même si les parseurs et interfaces peuvent appliquer une limite de sécurité configurable.

### 3.2 Tempérament

- L'équivalence sonore par défaut est évaluée en 12-TET.
- L'écriture d'une note reste indépendante de son équivalence sonore.
- La fréquence est calculée par un service d'accordage, jamais par les objets d'harmonie.
- La référence par défaut est `A4 = 440 Hz`, mais elle est configurable.

### 3.3 Culture et formatage

- Les calculs sont indépendants de la langue et de la culture.
- Le noyau stocke des identifiants stables, pas des libellés localisés.
- Les symboles Unicode (`♭`, `♯`, `𝄫`, `𝄪`) et ASCII (`b`, `#`, `bb`, `##`) sont traités par des parseurs et formateurs dédiés.
- La bibliothèque possède une terminologie d'affichage par défaut, française ou américaine.
- Chaque opération d'affichage peut remplacer localement cette valeur sans modifier le défaut global.
- Le défaut global est atomique et ne concerne que la présentation. Aucun calcul ni objet du domaine n'en dépend.
- Une opération de formatage capture la valeur par défaut une seule fois à son entrée afin de rester cohérente si le défaut global change simultanément.

Terminologie minimale :

| Écriture interne | Française | Américaine |
|---|---|---|
| C D E F G A B | Do Ré Mi Fa Sol La Si | C D E F G A B |
| Major chord | majeur | major |
| Minor chord | mineur | minor |
| Mixolydian | mode de sol | Mixolydian |

`ToString()` reste invariant et destiné au diagnostic. Les sorties destinées à l'utilisateur passent par le formateur explicite.

## 4. Vocabulaire du domaine

### 4.1 Lettre de note

`NoteLetter` représente une position diatonique parmi sept : `C` à `B`. Sa position naturelle en demi-tons dans l'octave est :

| Lettre | C | D | E | F | G | A | B |
|---|---:|---:|---:|---:|---:|---:|---:|
| Position | 0 | 2 | 4 | 5 | 7 | 9 | 11 |

### 4.2 Altération

`Accidental` représente un déplacement chromatique signé appliqué à une lettre. Elle ne change jamais la lettre diatonique.

### 4.3 Classe de hauteur

`PitchClass` représente une valeur chromatique normalisée dans `[0, 11]`. Elle ne contient ni lettre, ni altération, ni octave.

### 4.4 Hauteur écrite

`SpelledPitch` associe une lettre et une altération, sans octave.

Deux hauteurs écrites sont égales uniquement si leur lettre et leur altération sont égales. L'équivalence sonore est une opération distincte :

```text
C♯ != D♭                   égalité d'écriture
C♯ enharmonicWith D♭      équivalence sonore en 12-TET
```

### 4.5 Note

`Note` associe une hauteur écrite et un numéro d'octave. Elle ne contient ni durée, ni vélocité, ni canal MIDI.

L'index chromatique absolu est :

```text
12 × octave + positionNaturelle(lettre) + altération
```

Il n'est pas normalisé modulo 12. Ainsi `B♯4` conserve son écriture mais possède le même index sonore que `C5`.

### 4.6 Intervalle

Un `Interval` est défini canoniquement par :

- un numéro diatonique positif, commençant à 1 pour l'unisson ;
- une distance chromatique signée, relative au mouvement diatonique ascendant ;
- une direction appliquée lors d'une transposition.

La plupart des intervalles usuels ont une distance chromatique positive ou nulle. Une valeur signée permet néanmoins de représenter sans cas spécial un unisson diminué.

La qualité est déduite du numéro et de la distance chromatique. Les classes parfaites sont `1, 4, 5`; les classes majeures sont `2, 3, 6, 7`, répétées à chaque octave.

Distances de référence d'un intervalle majeur ou parfait simple :

| Numéro | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Demi-tons | 0 | 2 | 4 | 5 | 7 | 9 | 11 | 12 |

Pour un intervalle composé, chaque groupe de sept degrés ajoute douze demi-tons.

### 4.7 Degré de formule

`FormulaDegree` associe un numéro de degré et une altération chromatique relative. Exemples :

- tierce majeure : `3` ;
- tierce mineure : `♭3` ;
- quinte augmentée : `♯5` ;
- neuvième : `9`.

### 4.8 Gamme et mode

Une `ScaleDefinition` est une séquence ordonnée de degrés relatifs à une tonique. Elle ne contient aucune note concrète.

Exemples :

```text
Majeure          1  2  3  4  5  6  7
Mineure naturelle 1  2 ♭3  4  5 ♭6 ♭7
Mineure harmonique 1 2 ♭3  4  5 ♭6  7
```

Une `Scale` est le résultat de l'application d'une définition à une tonique écrite. La construction conserve les lettres diatoniques attendues.

Le catalogue standard couvre au minimum :

- les sept modes de la gamme majeure : ionien, dorien, phrygien, lydien, mixolydien, éolien et locrien ;
- les sept rotations de la gamme mineure naturelle, exposées comme vues relatives des mêmes collections diatoniques ;
- les sept rotations de la gamme mineure harmonique ;
- les sept rotations de la gamme mineure mélodique ascendante.

Les noms de certains modes mineurs n'étant pas universels, chaque définition possède un identifiant stable et peut exposer plusieurs alias localisés. Pour les modes de la gamme majeure, la terminologie française par défaut est `mode de do`, `mode de ré`, `mode de mi`, `mode de fa`, `mode de sol`, `mode de la`, `mode de si`; la terminologie américaine utilise respectivement `Ionian`, `Dorian`, `Phrygian`, `Lydian`, `Mixolydian`, `Aeolian`, `Locrian`.

### 4.9 Tonalité et armure

Une `MusicalKey` associe une tonique écrite et un mode majeur ou mineur.

Une `KeySignature` est représentée canoniquement par un nombre de quintes signé :

- positif : nombre de dièses, dans l'ordre `F C G D A E B` ;
- négatif : nombre de bémols, dans l'ordre `B E A D G C F` ;
- zéro : aucune altération à l'armure.

La version 1 prend en charge les armures conventionnelles de `-7` à `+7`.

### 4.10 Accord, voicing et renversement

Une `ChordDefinition` est une séquence de degrés relatifs. Exemples :

```text
Majeur             1  3  5
Mineur             1 ♭3  5
Septième dominante 1  3  5 ♭7
Septième diminuée  1 ♭3 ♭5 𝄫7
```

Un `Chord` associe une fondamentale écrite et une définition. Il expose simultanément :

- une identité symbolique standard et indépendante de la langue ;
- sa série ordonnée de hauteurs écrites en position fondamentale ;
- des noms formatés, abrégés ou complets, obtenus par le service de présentation.

Exemples de représentations d'un même accord :

| Style | Français | Américain |
|---|---|---|
| Abrégé | Do m7 | Cm7 |
| Complet | Do mineur septième | C minor seventh |
| Notes | Do Mib Sol Sib | C Eb G Bb |

Un `Voicing` est une séquence ordonnée de notes octaviées réalisant un accord. Le renversement est déduit du degré d'accord placé à la basse. Un changement de voicing ou de renversement ne modifie pas l'identité de l'accord.

Un accord transposé et une réalisation transformée conservent leur provenance :

- `OriginalChord` désigne l'accord avant toute transformation ;
- `SemitoneDeltaFromOriginal` conserve le déplacement signé exact, sans réduction modulo 12 ;
- `InversionNumber` vaut `0` en position fondamentale, `1` au premier renversement, etc. ;
- une nouvelle transposition cumule son déplacement avec le delta déjà mémorisé ;
- un renversement conserve l'accord d'origine et le delta de transposition ;
- demander un autre renversement remplace le numéro courant, il ne l'additionne pas.

La transposition d'un accord théorique ne requiert pas de choisir une octave. Elle retourne un objet dérivé contenant l'accord original, l'accord résultat et le delta. La réalisation octaviée ajoute ensuite les informations de voicing et de renversement.

### 4.11 Degrés et fonctions harmoniques

Dans une gamme heptatonique, `ScaleDegreeNumber` est compris entre `1` et `7`. Il peut être affiché en chiffre arabe ou sous forme de fonction harmonique en chiffres romains lorsqu'un accord et une tonalité sont connus.

Convention par défaut :

- accord majeur : chiffre romain majuscule, par exemple `I`, `IV`, `V` ;
- accord mineur : chiffre romain minuscule, par exemple `ii`, `iii`, `vi` ;
- accord diminué : chiffre romain minuscule suivi de `°`, par exemple `vii°` ;
- accord semi-diminué : chiffre romain minuscule suivi de `ø` ;
- accord augmenté : chiffre romain majuscule suivi de `+`.

La fonction harmonique n'est jamais déduite sans tonalité explicite. Un même accord peut avoir plusieurs fonctions selon le contexte.

### 4.12 Reconnaissance d'accords

La reconnaissance accepte une série de `Note` ou un accord libre. Elle ignore l'octave pour identifier les classes de notes, mais utilise la basse pour déterminer un renversement lorsque l'ordre ou les octaves sont disponibles.

Elle retourne une liste de candidats ordonnée, car une même collection sonore peut admettre plusieurs fondamentales ou orthographes. Chaque candidat contient :

- l'accord théorique reconnu ;
- la fondamentale proposée ;
- le renversement éventuel ;
- les notes manquantes ou supplémentaires ;
- un score brut et une confiance normalisée ;
- ses noms abrégé et complet dans la terminologie demandée.

Une correspondance exacte est classée avant une correspondance avec omission ou ajout. L'orthographe des notes contribue au classement mais l'équivalence enharmonique peut être activée par option.

### 4.13 Recherche de gammes compatibles

La recherche accepte une série de `Note` ou un `Chord`. Elle évalue toutes les toniques et toutes les définitions du catalogue sélectionné.

Le classement prend en compte de manière explicite et configurable :

- la présence des notes observées dans la gamme candidate ;
- la couverture de la gamme par les notes distinctes observées ;
- les notes extérieures à la gamme ;
- la cohérence de l'orthographe diatonique ;
- la fondamentale et la basse d'un accord lorsqu'elles sont connues ;
- la présence éventuelle de la tonique dans les données d'entrée.

Chaque résultat expose un `Score` et une `RelativeProbability` dans `[0, 1]`. Les probabilités relatives des candidats retournés totalisent 1. Cette valeur est une normalisation heuristique dépendant du catalogue et des poids choisis ; elle ne constitue pas une probabilité statistique calibrée.

### 4.14 Gammes pentatoniques

Les définitions standard sont :

```text
Pentatonique majeure  1  2  3  5  6
Pentatonique mineure  1 ♭3  4  5 ♭7
```

La dérivation automatique d'une pentatonique n'est univoque que pour une gamme compatible avec l'une de ces formules. L'API propose donc :

- une stratégie `StandardMajorOrMinor`, utilisée par défaut et qui échoue explicitement si aucune pentatonique standard n'est contenue dans la gamme source ;
- une stratégie `SelectSourceDegrees` qui extrait cinq positions précisées par l'appelant ;
- un résultat qui conserve la gamme source et la stratégie de dérivation.

L'API ne remplace jamais silencieusement une quinte diminuée ou une autre note absente pour fabriquer une pentatonique standard.

## 5. Règles de calcul

### 5.1 Classe chromatique

```text
pitchClass(spelledPitch) =
    modulo12(positionNaturelle(letter) + accidental.semitones)
```

Le modulo mathématique retourne toujours une valeur comprise entre 0 et 11.

### 5.2 Transposition écrite

Pour transposer une note par un intervalle :

1. déterminer la lettre cible par la distance diatonique ;
2. déterminer l'index chromatique cible par la distance en demi-tons ;
3. déterminer l'octave naturelle de la lettre cible ;
4. calculer l'altération nécessaire pour atteindre l'index chromatique cible ;
5. conserver cette écriture, même si une écriture enharmonique semble plus simple.

Exemples normatifs :

```text
C4 + tierce majeure = E4
C♯4 + tierce majeure = E♯4
D♭4 + tierce majeure = F4
B♯4 + seconde majeure = C𝄪5
```

La simplification enharmonique est une opération explicite soumise à une stratégie ; elle ne fait pas partie de la transposition.

### 5.3 Intervalle entre deux notes

L'intervalle entre deux notes écrites dépend :

- de la distance entre leurs lettres et octaves ;
- de la distance entre leurs index chromatiques absolus.

`C♯4 → D♭4` est donc une seconde diminuée, même si la distance sonore vaut zéro en 12-TET.

### 5.4 Construction d'une gamme

Chaque degré est obtenu en transposant la tonique selon son numéro diatonique et son altération chromatique relative. Une gamme heptatonique diatonique utilise exactement une fois chaque lettre avant la répétition de l'octave.

### 5.5 Construction d'un accord

Chaque son théorique est obtenu depuis la fondamentale par le degré de formule. Les doublures, omissions et dispositions appartiennent au `Voicing`, pas au `Chord`.

### 5.6 Transposition et renversement d'un accord

La transposition chromatique d'un accord applique le même delta signé à toutes ses notes et à sa fondamentale. Une stratégie d'orthographe explicite détermine l'écriture du résultat. Le delta mémorisé reste celui demandé par l'appelant : `+14` n'est pas réduit à `+2`.

Le renversement déplace successivement les notes les plus graves d'une octave tout en préservant leur écriture. Pour un accord de `n` sons distincts, les renversements valides sont `0..n-1`.

### 5.7 Fréquence en 12-TET

Pour un index MIDI `m`, une référence `A4 = f` et `A4 = 69` :

```text
frequency(m) = f × 2 ^ ((m - 69) / 12)
```

La durée d'une note n'intervient jamais dans cette formule.

## 6. Cercle des quintes

### 6.1 Modèle harmonique

- Le cercle contient exactement douze positions chromatiques.
- Un déplacement horaire ajoute une quinte juste ascendante.
- Un déplacement antihoraire ajoute une quarte juste ascendante, équivalente à une quinte descendante.
- Chaque position expose une tonalité majeure et sa relative mineure, partageant la même armure.
- Certaines positions exposent plusieurs graphies conventionnelles enharmoniques.

Armures proposées par position, avec `C majeur` à l'index 0 :

| Index | Armures disponibles | Tonalités majeures |
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

La graphie principale est sélectionnée par une stratégie explicite : nombre minimal d'altérations, puis préférence dièses ou bémols en cas d'égalité.

### 6.2 Modèle pour interface utilisateur

Le domaine expose les positions et relations. Un module de géométrie indépendant expose :

- l'angle de début, l'angle central et l'amplitude de chaque secteur ;
- des coordonnées normalisées dans `[-1, 1]` ;
- un point d'ancrage pour le libellé majeur ;
- un point d'ancrage pour le libellé mineur ;
- aucune couleur, police, commande ou classe propre à un framework UI.

Une UI peut ainsi dessiner le cercle avec son propre système graphique sans réimplémenter les calculs de position.

## 7. Invariants

### 7.1 Valeurs et égalité

- `INV-P01` — Une `PitchClass` est toujours dans `[0, 11]`.
- `INV-P02` — L'égalité de `SpelledPitch` compare lettre et altération.
- `INV-P03` — L'équivalence enharmonique compare les classes de hauteur, jamais l'égalité d'objet.
- `INV-P04` — `Note` ne contient aucune propriété rythmique ou MIDI.
- `INV-P05` — Deux objets égaux ont toujours le même hash code.

### 7.2 Intervalles et transposition

- `INV-I01` — Un numéro d'intervalle est supérieur ou égal à 1.
- `INV-I02` — Une transposition respecte simultanément la distance diatonique et la distance chromatique.
- `INV-I03` — Une transposition ne simplifie jamais implicitement l'orthographe.
- `INV-I04` — Transposer par un intervalle puis par son opposé restitue exactement la note écrite initiale.
- `INV-I05` — L'inversion de deux intervalles simples complémentaires totalise neuf degrés et douze demi-tons.

### 7.3 Gammes et tonalités

- `INV-S01` — Une définition de gamme possède des degrés uniques, ordonnés et commençant par `1` naturel.
- `INV-S02` — Une gamme heptatonique diatonique utilise les sept lettres exactement une fois.
- `INV-S03` — La répétition finale éventuelle de la tonique appartient à la présentation, pas à l'identité de la gamme.
- `INV-K01` — Une armure conventionnelle contient entre zéro et sept altérations d'un seul type.
- `INV-K02` — Une majeure et sa relative mineure ont la même armure.

### 7.4 Accords

- `INV-C01` — Une définition d'accord contient la fondamentale `1` naturelle.
- `INV-C02` — Les degrés d'une définition sont uniques.
- `INV-C03` — Chaque note d'un voicing est enharmoniquement compatible avec un degré de l'accord, sauf extension explicitement autorisée.
- `INV-C04` — Modifier un voicing ou un renversement ne modifie pas l'identité de l'accord.
- `INV-C05` — Un accord expose toujours une identité symbolique et une série de hauteurs écrites cohérentes avec sa définition.
- `INV-C06` — Le delta d'une transposition est signé, exact et mesuré depuis l'accord d'origine.
- `INV-C07` — Un accord renversé conserve l'accord d'origine et un numéro de renversement valide.
- `INV-C08` — La transposition ne modifie pas le numéro de renversement et le renversement ne modifie pas le delta de transposition.

### 7.5 Nommage et analyse

- `INV-N01` — Le formatage français et américain d'un objet ne modifie jamais cet objet.
- `INV-N02` — Un override local n'altère jamais le défaut global de présentation.
- `INV-N03` — Les chiffres romains ne sont produits qu'avec une tonalité explicite.
- `INV-N04` — Les accords diminués utilisent `°` par défaut et les accords semi-diminués `ø`.
- `INV-R01` — Les candidats d'une reconnaissance sont triés par score décroissant avec une règle de départage déterministe.
- `INV-R02` — Les probabilités relatives des gammes retournées totalisent 1 à la tolérance numérique près.
- `INV-R03` — Le score brut et ses facteurs explicatifs restent accessibles.
- `INV-R04` — Une recherche stricte n'accepte aucune note extérieure à la gamme candidate.

### 7.6 Cercle des quintes

- `INV-F01` — Le cercle possède exactement douze segments ordonnés.
- `INV-F02` — Les index sont normalisés modulo 12.
- `INV-F03` — Deux segments voisins sont séparés d'une quinte juste dans le sens horaire.
- `INV-F04` — La majeure et la relative mineure d'une graphie partagent exactement la même armure.
- `INV-F05` — Une tonalité conventionnelle de `-7` à `+7` appartient à un segment unique.
- `INV-F06` — La géométrie produit douze secteurs de 30 degrés sans chevauchement ni lacune.

### 7.7 Qualité logicielle

- `INV-A01` — Tous les objets du domaine sont immuables et sûrs pour une lecture concurrente.
- `INV-A02` — Aucun calcul ne dépend d'un état global mutable.
- `INV-A03` — Une collection publique n'est jamais modifiable par le consommateur.
- `INV-A04` — Les calculs métier sont déterministes à paramètres identiques ; le formatage est déterministe pour une configuration d'affichage capturée identique.
- `INV-A05` — `Parse` lève une `FormatException` documentée ; `TryParse` n'en lève jamais pour une entrée utilisateur invalide.
- `INV-A06` — Les calculs métier n'utilisent pas de chaînes comme représentation intermédiaire.
- `INV-A07` — Le seul défaut global mutable autorisé concerne la terminologie d'affichage et son accès est atomique.

## 8. Exemples d'acceptation

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
majorChord(Db) = Db F Ab
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

## 9. Stratégie de tests

Les tests sont organisés par règles, pas par méthodes :

- tests exhaustifs des douze classes chromatiques ;
- tests de toutes les qualités d'intervalles simples et d'un ensemble d'intervalles composés ;
- tests de propriétés pour la transposition aller-retour ;
- tests des gammes standard dans toutes les tonalités conventionnelles ;
- tests d'accords depuis plusieurs fondamentales avec altérations ;
- tests de provenance après chaînes de transpositions et de renversements ;
- tests de noms français/américains avec défaut global et overrides concurrents ;
- tests des chiffres romains dans plusieurs tonalités ;
- tests de reconnaissance exacte, ambiguë, avec omission et avec ajout ;
- tests de classement des gammes et de normalisation des probabilités relatives ;
- tests des modes des gammes majeure, mineure harmonique et mineure mélodique ;
- tests de dérivation et d'échec explicite des pentatoniques ;
- tests d'égalité, d'équivalence enharmonique et de stabilité des hash codes ;
- tests des quinze armures `-7..+7` et de leur placement sur douze segments ;
- tests géométriques indépendants du rendu ;
- tests de parsing Unicode et ASCII ;
- tests de limites pour éviter les entrées excessives.

Les résultats historiques de Mingus ne constituent pas des oracles de test.

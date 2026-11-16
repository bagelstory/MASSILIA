# CHOUF — prompts images pour la version Unity (Gemini ou ChatGPT)

Ce document complète `04-prompts-3d.md`. Il couvre toutes les images listées dans `06-unity-recap-images-sons.md`.

**Un bloc = une image.** Chaque bloc est complet, avec le style de base déjà dedans : copie-le tel quel, sans rien ajouter. Le nom en gras est le nom du fichier à déposer dans `unity/Assets/Art/brut/` (PNG ou JPG). Je fais ensuite le découpage, le détourage, le redimensionnement et la conversion pour Unity.

## Style de base à respecter

Toutes les images du jeu suivent les cinq styles ci-dessous. Ils sont déjà recopiés dans chaque bloc. Ils sont là pour que tu puisses juger si une image « colle » ou non.

| Style | Pour quoi | Règle principale |
|---|---|---|
| **A — Maquette** | écran de lancement, fonds, fins de partie, flux | Marseille en maquette 3D d'argile gris clair, vue aérienne, flou de maquette, lumière du soir, presque monochrome |
| **B — Portrait** | opérateurs et recrues | Personnage 3D réaliste de jeu console, tête et épaules, fond gris anthracite uni, lumière studio, regard un peu à gauche |
| **C — Badge** | icônes des types, indicateurs, tâches | Planche de 9 badges ronds (3 × 3) : disque anthracite brillant, anneau doré biseauté, un symbole blanc en relief, fond noir pur |
| **D — Emblème** | 5 équipes rivales | Un seul badge rond, anneau de la couleur de l'équipe, symbole blanc, fond noir pur |
| **E — Pictogramme** | onglets | Planche de 9 symboles blancs simples en relief, sans disque, fond noir pur |

Palette commune : gris d'argile, anthracite, bleu nuit pour la mer, lumières chaudes jaune-orangé, or pour les anneaux. Le vert vif `#6BCF4F` est la couleur du joueur. C'est le jeu qui l'ajoute : ne la demande pas dans les images.

Interdits (déjà respectés dans les blocs) :
- Aucun texte dans les images, sauf le logo.
- Aucune marque ni aucun logo réel.
- Personnages tous adultes et tous fictifs.
- Aucun mot qui bloque les filtres : les blocs décrivent des lieux, des objets et des personnes ordinaires.

## Gemini ou ChatGPT ?

- **Les deux marchent** avec ces blocs.
- **ChatGPT** respecte mieux les grilles 3 × 3 et le texte du logo. Il peut aussi rendre un vrai fond transparent : ajoute « Transparent background. » à la fin d'un bloc C, D ou E, mais ce n'est pas obligatoire.
- **Gemini** fait de meilleures maquettes de ville, mais il lui arrive de ne pas respecter le format. Redemande alors « vertical 9:16 » ou « square 1:1 ».
- **Même visage sur plusieurs images** : joins l'image déjà faite et ajoute à la fin « Same person as in the attached image. »
- **Refus** : relance une fois tel quel. Si ça bloque encore, dis-moi quel bloc : je le réécris.

## Inventaire

**Mise à jour :** les séries T, K, D, A et B (icônes, emblèmes, onglets) sont déjà dessinées en code par `tools/gen_icons.py` et branchées dans le jeu. Leurs prompts restent ici, seulement si tu veux une version plus « 3D ». **Il reste 29 images à générer** : L, O, N, G et X.

| Série | Fichiers | Images à générer | Priorité |
|---|---|---|---|
| L — logo, icône, lancement | 3 | 3 | 1 |
| T — types d'opérateurs (+ charbon, menace, cadenas) | 1 planche | 1 | 1 |
| K — indicateurs (+ capacité, dette, cycle) | 1 planche | 1 | 1 |
| D — emblèmes des équipes | 5 | 5 | 1 |
| O — opérateurs nommés | 6 | 6 | 2 |
| N — recrues anonymes | 12 | 12 | 2 |
| G — fins de partie | 5 | 5 | 2 |
| A — tâches (+ valider, recruter) | 1 planche | 1 | 3 |
| B — onglets (+ 3 boutons) | 1 planche | 1 | 3 |
| X — flux et fond de carte | 3 | 3 | 3 |
| **Total** | | **38** | |

Les **bannières des 10 zones** existent déjà : ce sont celles de la version web, et leurs prompts sont dans le document 04, série Z. Je les convertis, rien à refaire.

---

# L — LOGO, ICÔNE, LANCEMENT

**logo_chouf**
```
3D render of a video game title logo. The single word "CHOUF" in bold heavy uppercase letters, made of matte light-grey clay like an architectural model, with a thin beveled gold edge, slightly tilted toward the viewer, soft evening light from the left, long soft shadow. Inside the letter O, a tiny miniature of the Notre-Dame de la Garde basilica on its hill. Plain pure black background. Wide 2:1 image, logo centered with empty margins. Only the word CHOUF, no other text.
```

**icon_app**
```
3D render of a mobile game app icon. A stylized eye made of matte light-grey clay, seen from the front, whose iris is a tiny miniature model of the Vieux-Port of Marseille at dusk with small warm window lights and the basilica on its hill, thin beveled gold ring around the iris. Dark charcoal background with a soft vignette and a subtle warm glow behind the eye. Simple, bold, readable at very small size. Square 1:1 image, rounded-square composition with the eye filling the middle two thirds. No text, no letters, no logos.
```

**splash**
```
3D render, strategy game key art. The whole city of Marseille as a miniature architectural model of matte light-grey clay buildings with simple shapes and almost no texture, seen from very high above at night, tilt-shift blur at the top and bottom edges. Dark blue-grey sea on the left, the Vieux-Port in the middle, the basilica lit on its hill, the northern hills with housing towers at the top, the white limestone calanques at the bottom. Thousands of tiny warm window and street lights, cool blue night, soft haze. Calm, mysterious, almost monochrome. Vertical 9:16 image, the top quarter is dark empty sky and haze for the title. No people, no text, no logos.
```

---

# T — TYPES D'OPÉRATEURS (planche de 9, style C)

Ordre : Repérage, Pression, Logistique, Finances, Renseignement, Force, puis charbon, menace et zone verrouillée.

**icones_types**
```
3D render of a sheet of 9 round game badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a compass; an open hand pressing down on a small house; a small delivery van seen from the side; a stack of coins; a magnifying glass over a notebook; a boxing glove; a small market stall with a striped awning; a warning triangle with an exclamation mark; a closed padlock. Square 1:1 image. No text, no letters, no numbers.
```

---

# K — INDICATEURS (planche de 9, style C)

Ordre : Territoire, Trésorerie, Effectifs, Pression, Stabilité, Exposition, puis Capacité, Dette et Cycle.

**icones_indicateurs**
```
3D render of a sheet of 9 round game badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a map pin standing on a folded map; a bundle of banknotes; three person silhouettes side by side; a round pressure gauge with its needle in the red zone; a balanced two-pan scale; a spotlight beam shining down; a door with an arrow pointing in; a chain with one broken link; an hourglass. Square 1:1 image. No text, no letters, no numbers.
```

---

# D — EMBLÈMES DES ÉQUIPES RIVALES (style D, une image par équipe)

Les équipes sont inventées. L'anneau prend la couleur de l'équipe dans le jeu.

**faction_compagnie** · La Compagnie · rouge
```
3D render of a single round emblem for a fictional team in a strategy game. A glossy dark charcoal disc with a thick beveled metal ring painted deep brick red, and one simple bold white embossed symbol in the middle: an old ship's steering wheel. Soft highlight on top, seen straight from the front, centered. Plain pure black background. Square 1:1 image. No text, no letters, no numbers.
```

**faction_gabians** · Les Gabians · bleu
```
3D render of a single round emblem for a fictional team in a strategy game. A glossy dark charcoal disc with a thick beveled metal ring painted bright azure blue, and one simple bold white embossed symbol in the middle: a seagull in flight with wings spread. Soft highlight on top, seen straight from the front, centered. Plain pure black background. Square 1:1 image. No text, no letters, no numbers.
```

**faction_ruisseau** · Le Ruisseau · ocre
```
3D render of a single round emblem for a fictional team in a strategy game. A glossy dark charcoal disc with a thick beveled metal ring painted warm ochre orange, and one simple bold white embossed symbol in the middle: a winding stream flowing between two small hills. Soft highlight on top, seen straight from the front, centered. Plain pure black background. Square 1:1 image. No text, no letters, no numbers.
```

**faction_tramontane** · Tramontane · violet
```
3D render of a single round emblem for a fictional team in a strategy game. A glossy dark charcoal disc with a thick beveled metal ring painted deep violet, and one simple bold white embossed symbol in the middle: three curling lines of strong wind. Soft highlight on top, seen straight from the front, centered. Plain pure black background. Square 1:1 image. No text, no letters, no numbers.
```

**faction_calanquais** · Les Calanquais · turquoise
```
3D render of a single round emblem for a fictional team in a strategy game. A glossy dark charcoal disc with a thick beveled metal ring painted sea turquoise, and one simple bold white embossed symbol in the middle: a tall white cliff above the sea with a single pine tree on top. Soft highlight on top, seen straight from the front, centered. Plain pure black background. Square 1:1 image. No text, no letters, no numbers.
```

---

# O — OPÉRATEURS NOMMÉS (style B)

Les trois premiers sont dans ton équipe au départ (Force, Pression, Repérage). Les trois suivants sont donnés aux premières recrues.

**portrait_zepek** · Zépek
```
3D render of a video game character portrait, realistic modern console game style. A 42-year-old man from Marseille of Italian descent, stocky and broad-shouldered, shaved head, short dark beard with grey hairs, dark navy bomber jacket over a grey t-shirt, steady calm look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_rascasse** · Rascasse
```
3D render of a video game character portrait, realistic modern console game style. A 55-year-old man from Marseille, weathered tanned face with deep lines, thick grey moustache, short grey hair, old dark green fisherman's sweater, sharp amused eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_mika** · Mika
```
3D render of a video game character portrait, realistic modern console game style. A 27-year-old woman from Marseille of Comorian descent, dark skin, hair in a high bun with a few loose curls, small gold hoop earrings, black zip-up sports jacket, quick attentive eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_zizou** · Zizou
```
3D render of a video game character portrait, realistic modern console game style. A 34-year-old man from Marseille of Algerian descent, olive skin, very short dark hair, neatly trimmed beard, plain white polo shirt under a light grey jacket, relaxed half smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_sab** · Sab
```
3D render of a video game character portrait, realistic modern console game style. A 38-year-old woman from Marseille, fair skin, straight dark-blonde hair in a low ponytail, thin rectangular glasses, dark burgundy blazer over a black top, focused serious look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_nono** · Nono
```
3D render of a video game character portrait, realistic modern console game style. A 63-year-old man from Marseille of Armenian descent, thick white eyebrows, short white hair, deep-set dark eyes, beige linen shirt under a brown cardigan, wise patient expression. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

---

# N — RECRUES ANONYMES (style B, 2 par type)

**portrait_reperage_1**
```
3D render of a video game character portrait, realistic modern console game style. A 23-year-old man from Marseille, light brown skin, short curly hair, thin moustache, grey hoodie under a dark blue windbreaker, curious alert eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_reperage_2**
```
3D render of a video game character portrait, realistic modern console game style. A 25-year-old woman from Marseille, fair skin with freckles, short red hair under a black beanie, olive green parka, lively watchful eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_pression_1**
```
3D render of a video game character portrait, realistic modern console game style. A 36-year-old man from Marseille, tanned skin, black hair slicked back, short stubble, black leather jacket over a white t-shirt, confident direct look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_pression_2**
```
3D render of a video game character portrait, realistic modern console game style. A 41-year-old woman from Marseille of Senegalese descent, dark skin, short natural hair, small silver earrings, structured dark grey coat, calm firm expression. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_logistique_1**
```
3D render of a video game character portrait, realistic modern console game style. A 47-year-old man from Marseille of Portuguese descent, round face, short grey-brown hair, thick neck, navy work jacket with reflective stripes, friendly tired look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_logistique_2**
```
3D render of a video game character portrait, realistic modern console game style. A 30-year-old man from Marseille of Moroccan descent, light olive skin, short wavy black hair, short beard, dark grey cap worn backwards, orange high-visibility vest over a black sweater, practical focused look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_finances_1**
```
3D render of a video game character portrait, realistic modern console game style. A 44-year-old man from Marseille, pale skin, neat short brown hair with a side parting, clean-shaven, navy suit with an open-collar light blue shirt, cool calculating look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_finances_2**
```
3D render of a video game character portrait, realistic modern console game style. A 52-year-old woman from Marseille of Vietnamese descent, short silver bob haircut, thin gold necklace, elegant black turtleneck, composed precise expression. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_renseignement_1**
```
3D render of a video game character portrait, realistic modern console game style. A 29-year-old woman from Marseille of Tunisian descent, long dark wavy hair, light makeup, beige trench coat over a cream sweater, quiet observant eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_renseignement_2**
```
3D render of a video game character portrait, realistic modern console game style. A 58-year-old man from Marseille, fair skin, bald on top with short grey hair on the sides, round tortoiseshell glasses, brown corduroy jacket, thoughtful knowing smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_force_1**
```
3D render of a video game character portrait, realistic modern console game style. A 33-year-old man from Marseille of Cape Verdean descent, dark skin, tall and muscular, very short hair, black crewneck sweater, steady quiet look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**portrait_force_2**
```
3D render of a video game character portrait, realistic modern console game style. A 39-year-old woman from Marseille, light skin, athletic build, dark brown hair in a tight braid, dark grey track jacket zipped up, determined calm expression. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

---

# G — FINS DE PARTIE (style A, horizontal 4:3)

**end_victoire** · Victoire
```
3D render, strategy game city view. The whole city of Marseille as a miniature architectural model of matte light-grey clay buildings with simple shapes and almost no texture, seen from high above at sunrise, tilt-shift blur at the top and bottom edges. Golden morning light from the right, long soft shadows, the sea turning pale gold, the basilica catching the first sunlight on its hill, every window glowing warm. Calm, wide and triumphant, almost monochrome with warm gold accents. Horizontal 4:3 image. No people, no text, no logos.
```

**end_faillite** · Faillite
```
3D render, still life from a modern strategy video game, realistic game-engine look. An empty office desk at night in a small room above a Marseille street, a closed ledger, a pile of envelopes, an empty cash box left open, a cold cup of coffee, a desk lamp switched off, blue streetlight coming through the blinds. Muted desaturated palette, cool blue light, soft haze, quiet and heavy mood. Horizontal 4:3 image. No people, no text, no logos.
```

**end_chute** · Chute
```
3D render, strategy game city view. A miniature architectural model of the northern hills of Marseille, matte light-grey clay housing towers with simple shapes, seen from high above at night, tilt-shift blur at the top and bottom edges. Many small blue flashing lights moving along the streets between the towers, a few rooftops lit by white searchlight beams from above, the rest of the city dark. Cold blue palette, haze, tense and silent mood. Horizontal 4:3 image. No people, no text, no logos.
```

**end_abandon** · Abandon
```
3D render, cinematic still from a modern strategy video game set in Marseille, realistic game-engine look. An empty white plastic chair on a flat rooftop of a housing block at dusk, facing the sea and the distant port, two empty soda cans beside it, a jacket left on the back of the chair. Muted desaturated palette, warm sodium streetlights coming on, cool blue sky, soft haze, melancholic and quiet. Horizontal 4:3 image. No people, no text, no logos.
```

**end_temps** · Fin du temps
```
3D render, strategy game city view. A miniature architectural model of the Vieux-Port of Marseille, matte light-grey clay buildings with simple shapes and almost no texture, high three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Dozens of blank paper calendar pages flying over the harbor in a strong wind, the small boats tilted, late afternoon light from the left, long soft shadows. Calm, nostalgic, almost monochrome. Horizontal 4:3 image. No people, no text, no numbers, no logos.
```

---

# A — TÂCHES (planche de 9, style C)

Ordre : Sécuriser, Presser, Repérer, Surveiller, Gérer les flux, Optimiser, Repos, puis Valider le cycle et Recruter.

**icones_taches**
```
3D render of a sheet of 9 round game badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a shield; three arrows pushing forward into a small house; a pair of binoculars; an open eye; two curved arrows forming a loop around a small shipping container; two interlocking gears; a crescent moon; a check mark; a person silhouette with a plus sign. Square 1:1 image. No text, no letters, no numbers.
```

---

# B — ONGLETS (planche de 9, style E)

Ordre : Carte, Bilan, Équipe, Flux, Rivaux, Journal, puis Réglages, Retour et Fermer.

**icones_onglets**
```
3D render of a sheet of 9 simple game interface symbols arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each symbol: bold, clean, white, slightly embossed with a soft highlight on top, no disc and no frame, seen straight from the front, all the same size, readable at very small size. Symbols, left to right then top to bottom: a folded map; a bar chart with three bars; two person silhouettes; a cargo ship seen from the side; two chess knights facing each other; an open notebook; a gear; a curved arrow pointing left; a cross. Square 1:1 image. No text, no letters, no numbers.
```

---

# X — FLUX ET FOND DE CARTE (style A)

**flux_sud** · Partenaire Sud
```
3D render, strategy game city view. A miniature architectural model of the container port of Marseille, matte light-grey clay with simple shapes and almost no texture, high three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Stacked containers in muted colors, tall cranes, a large cargo ship arriving from the open sea at dusk, dark blue-grey water, small warm lights on the quay. Calm, almost monochrome. Wide 2:1 image. No people, no text, no logos.
```

**flux_nord** · Partenaire Nord
```
3D render, strategy game city view. A miniature architectural model of the northern motorway entrance of Marseille, matte light-grey clay with simple shapes and almost no texture, high three-quarter aerial camera, tilt-shift blur at the top and bottom edges. A wide motorway curving between hills toward the city, a line of small trucks with their headlights on, a logistics warehouse zone with flat roofs, dusk light from the left, long soft shadows. Calm, almost monochrome. Wide 2:1 image. No people, no text, no logos.
```

**map_fond** · Fond de carte (facultatif)
```
3D render of a stylized relief map of the Marseille area seen straight from above, made of matte light-grey clay like a topographic model, with no buildings. Smooth hills and limestone massifs around the city basin, the coastline and the islands of the Frioul in the bay, the calanques cliffs in the south, the sea as a flat dark blue-grey surface with a very soft gradient. Soft evening light from the left, gentle shadows on the relief, subtle ambient occlusion, dark vignette around the edges. Very calm and almost monochrome so that colored tiles can be placed on top. Vertical 3:4 image. No text, no labels, no roads, no logos.
```

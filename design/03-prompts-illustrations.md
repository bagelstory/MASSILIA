# CHOUF — prompts Gemini pour TOUTES les illustrations

Document de commande, même méthode que KICK (`design/v62/prompts-*.txt`) : une planche = un prompt =
une image, découpée ensuite par `tools/slice.py` (repris de `v62_slice.py`).

## Comment s'en servir

1. Conversation Gemini neuve pour chaque série (zones, personnages, scènes, icônes). Colle d'abord le
   **BLOC STYLE**, puis le prompt de la planche, puis la liste « Cases ».
2. Garde l'ordre des cases : c'est lui qui dit au découpage quelle image est laquelle.
3. Envoie les PNG d'origine (pas de JPG, pas de capture d'écran), avec le code de la planche dans le nom
   du fichier (`P01.png`, `Z03.png`…). Dépose-les dans `assets/planches/`.
4. Si un personnage ne ressemble plus à lui-même d'une planche à l'autre, joins la planche précédente et
   ajoute : « Same character, same face, same proportions as the attached image. »

## Règles communes (déjà écrites dans chaque prompt)

- **Aucun texte** dans l'image : pas de nom, pas de chiffre, pas d'enseigne lisible, pas de plaque
  d'immatriculation lisible, pas de graffiti lisible.
- **Aucune marque réelle** : pas d'écusson ni de sponsor de l'OM (maillots bleu ciel et blanc unis),
  pas de logo RTM, pas de logo de police lisible, pas de marques de vêtements.
- **Rien d'opérationnel** : pas d'arme, pas de drogue, pas de sang, pas de geste de deal. La
  marchandise n'existe qu'en icônes abstraites (cartons et sacs fermés). Les conséquences se voient
  sur les visages et dans les lieux.
- **Pas de caricature** : les habitants sont dessinés avec dignité, des visages variés et réalistes,
  comme les gens de Marseille. Personne n'a « une tête de délinquant ».
- **Grilles** (maximums de KICK) : décors 3 × 2 (6 par planche), personnages 4 × 2 (8 par planche),
  portraits de famille 3 × 2 (6), icônes 6 × 4 (24). Cases identiques, rien ne déborde d'une case.
- **Fonds** : personnages et icônes sur **magenta pur #FF00FF** uni (le découpage l'efface). Décors et
  scènes **remplis bord à bord**, sans magenta.
- **Lieux réels reconnaissables** (Notre-Dame de la Garde, l'ombrière du Vieux-Port, la Corniche, le
  Vélodrome vu de loin, les collines du Nord, les calanques) mais rues et bâtiments génériques pour les
  cités : on ne dessine pas une cité réelle identifiable avec ses habitants.

## BLOC STYLE (à coller en premier, toujours)

Direction artistique validée le 16 novembre : **vue stratégique de jeu de gestion de territoires**
(références envoyées : ville grise en relief, territoires teintés par équipe, badges ronds). Les images
doivent aller avec la carte du jeu : ville désaturée, gris et béton, avec de rares couleurs fortes.

```
Stylized 3D-rendered look for a premium crime-management strategy game, Mediterranean noir set in
Marseille. Desaturated, almost monochrome palette: concrete greys, charcoal, off-white stone, muted ochre
roofs, dark slate-blue sea, with only a few strong accent colors (a red, a blue, a warm sodium-orange
light). Clean low-to-mid poly shapes with soft global illumination, gentle ambient occlusion, slight tilt-
shift depth of field on landscapes, cinematic dusk or night lighting with sodium streetlights and cold blue
moonlight. Grounded and realistic proportions, humane and sober, never glamorous, never caricatural.
No real brands, no logos, no club crests, no sponsors, no watermark, no readable text, no numbers, no
license plates. No weapons, no drugs, no blood.
```

Couleurs des équipes (à citer quand une image montre un territoire tenu) : La Compagnie rouge #c8352e,
Les Gabians bleu #3d7fd9, Le Ruisseau ocre #d08b2c, Tramontane violet #8a5cd6, Les Calanquais turquoise
#5fc4c4, le joueur vert #6ccf4f.

## Clés des fichiers (pour le jeu)

| Image | Clé dans le jeu |
|---|---|
| Bannière de zone, jour / nuit | `banniere_<zone>` / `banniere_<zone>_nuit` |
| Médaillon de personnage | `medaillon_<id>` ; famille : `medaillon_<id>_<famille>` ; plus âgé : `medaillon_<id>_2025` |
| Protagoniste | `joueur_<famille>_<age>` (`jeune`, `adulte`, `ancien`) et `_abime` |
| Scène d'événement | `scene_<id_evenement>` |
| Icône | `icone_<id>` |
| Grands écrans | `ecran_accueil`, `ecran_fin`, `ecran_chapitre_<id>` |

## Inventaire (30 planches + 6 images seules)

| Série | Planches | Images |
|---|---|---|
| Z — bannières des 10 zones (jour + nuit) + prison + panorama | Z01–Z04 | 24 |
| F — familles (mère, petit frère, grande sœur ×6 familles) + petits frères à 16 ans | F01–F04 | 24 |
| P — médaillons des 33 autres personnages + chef des Calanquais | P01–P05 | 35 (+5 libres) |
| A — les jeunes qui ont grandi (2025) | A01 | 8 |
| H — protagoniste jeune / adulte / ancien, normal et abîmé, ×6 familles | H01–H06 | 36 |
| S — scènes d'événements | S01–S06 | 36 |
| I — icônes | I01–I03 | 72 |
| R — chefs et emblèmes des équipes rivales | R01 | 8 |
| E — grands écrans (images seules, 9:16) | E1–E6 | 6 |

---

# SÉRIE Z — BANNIÈRES DE ZONES

Prompt commun à ajouter après le BLOC STYLE :

```
Sprite sheet of 6 wide landscape scenes, seen from a raised three-quarter aerial angle like a strategy game
camera, in a strict 3 columns x 2 rows grid, every cell the same size
(16:9), thin dark navy gutters between cells, each scene filling its cell edge to edge. Top row: three
places by day (late afternoon Mediterranean light). Bottom row: the SAME three places, same framing,
same buildings, at night (sodium streetlights, lit windows, deep blue sky). The bottom 35% of each scene
is calm and darker because the game puts text there. Few people, small and in the distance, no faces in
close-up. No readable text, no signs, no logos, no brands.
```

### Z01 — Centre historique · Centre nord · La Plaine
1. **centre_hist** — The Vieux-Port of Marseille seen from the quay: the mirrored canopy (ombrière) on
   the left, small fishing boats and moored sailboats, ochre and pale facades of the quays, Notre-Dame de
   la Garde on its hill in the background, a few pigeons, the narrow stairs of the Panier rising on the right.
2. **centre_nord** — A street of the Belle de Mai: tall worn 19th-century buildings with shutters, laundry
   at windows, a corner café with chairs, the brick chimneys and cranes of an old industrial wasteland
   turned cultural space in the distance, a bus stop.
3. **plaine** — The Cours Julien and its stairs: colorful painted walls (abstract shapes only, no letters),
   plane trees, terraces of small bars with folding chairs, a fountain basin, a guitar case on a bench.

### Z02 — Prado / Castellane · Nord-est · Nord central
1. **prado** — Place Castellane with its large monumental fountain at the center of a roundabout, tall
   Haussmann-style buildings, the long avenue lined with plane trees leading to the sea, a scooter.
2. **nord_est** — Large 1960s–70s housing blocks of Marseille's north-east on a hillside, white and beige
   towers with balconies, satellite dishes, a small football pitch with a fence, pine-covered hills behind,
   an elevated metro viaduct crossing the scene.
3. **nord_central** — A neighborhood of the 14th: a mix of low houses with tiled roofs and long housing
   bars, a big shopping center roof in the distance, a busy road with a bus, a flea-market parking lot.

### Z03 — Nord-ouest · Littoral nord · Est
1. **nord_ouest** — Tall housing towers on the hills above the sea in Marseille's north-west, the port
   cranes and the sea far below, a large shopping mall in the valley, a hospital building on a hill,
   dry Mediterranean vegetation.
2. **littoral_nord** — L'Estaque: the small fishing harbor with colorful wooden boats, a kiosk on the
   quay (no text), the old village houses climbing the hill, the railway viaduct, the sea and the port
   cranes on the horizon.
3. **est** — A village-like neighborhood of the east: a small square with plane trees, a church bell
   tower, low houses with tiled roofs, the white Garlaban-like hills behind, a big suburban shopping
   center visible far away.

### Z04 — Sud · Prison · Panorama
1. **sud** — The southern beaches of Marseille: a long beach with a seaside promenade, a park with palm
   trees, the white limestone mountains of the calanques rising at the end of the bay, small boats.
2. **baumettes** — The exterior of a large modern prison in the south of Marseille at the foot of white
   limestone hills: high grey walls, a gate, a waiting area outside with a bench and a few families
   waiting, seen from the street. Calm, sober, sad.
3. **panorama** — Marseille seen from the hill of Notre-Dame de la Garde: the whole city, the Vieux-Port,
   the cathedral by the sea, the islands of the Frioul, the northern hills far away.
(Ligne du bas : les mêmes de nuit.)

---

# SÉRIE F — FAMILLES

Prompt commun :

```
Sprite sheet of 6 character portraits in a strict 3 columns x 2 rows grid, every cell the same size,
on a perfectly flat pure magenta #FF00FF background, no shadow on the ground, nothing touching the cell
borders. Each portrait: head and shoulders, centered, facing slightly to the left, same size and same
eye height in every cell, so they can be cut into round medallions. Realistic, dignified, everyday
people of Marseille, varied faces, natural skin tones, no caricature. Simple everyday clothes, no logos,
no readable text.
```

### F01 — familles Ferrara et Mmadi
Ligne 1, famille **Ferrara** (italo-marseillaise) : 1. **mere** Sandrine, 43, supermarket cashier,
dark blond hair in a clip, tired but warm eyes, small gold cross necklace · 2. **petit_frere** Enzo, 11,
light olive skin, messy brown hair, sky-blue plain football shirt, curious smile · 3. **grande_soeur**
Jessica, 20, long dark brown hair, hoop earrings, sharp lucid gaze.
Ligne 2, famille **Mmadi** (comorienne) : 4. **mere** Zaïnaba, 42, night nursing assistant, dark skin,
colorful patterned headscarf, calm and proud · 5. **petit_frere** Kassim, 11, dark skin, short hair,
big admiring eyes, hoodie · 6. **grande_soeur** Raïssa, 20, dark skin, braids tied up, straight serious look.

### F02 — familles Benali et Sarkissian
Ligne 1, **Benali** (algérienne) : 1. **mere** Samia, 44, cleaning agent, dark curly hair with grey
strands, strong worried face · 2. **petit_frere** Ilyes, 11, tan skin, black hair cut short on the sides,
mischievous smile · 3. **grande_soeur** Leïla, 20, dark hair in a ponytail, glasses, direct look.
Ligne 2, **Sarkissian** (arménienne) : 4. **mere** Anahid, 45, medical secretary, dark hair in a bob,
elegant cardigan, anxious eyes · 5. **petit_frere** Tigran, 11, pale olive skin, thick dark hair, curious
· 6. **grande_soeur** Sona, 20, long black hair, strong eyebrows, determined.

### F03 — familles Diallo et Garcia
Ligne 1, **Diallo** (sénégalaise et marseillaise) : 1. **mere** Aminata, 41, school canteen cook, dark
skin, wax-print headwrap, broad warm smile with tired eyes · 2. **petit_frere** Moussa, 11, dark skin,
short hair, sky-blue plain shirt, laughing · 3. **grande_soeur** Awa, 20, dark skin, short natural hair,
small nose ring, lucid.
Ligne 2, **Garcia** (gitane sédentaire) : 4. **mere** Carmen, 40, market seller, long black hair, gold
earrings, strong and protective expression · 5. **petit_frere** Manolo, 11, tan skin, black curls,
cheeky · 6. **grande_soeur** Esperanza, 20, black hair in a long braid, serious and kind.

### F04 — les six petits frères à 16 ans (2026)
Même prompt commun. Joins F01, F02, F03 et ajoute : « Same six boys as the attached sheets, five years
older, 16 years old, same faces. » Cases : 1. Enzo · 2. Kassim · 3. Ilyes · 4. Tigran · 5. Moussa · 6. Manolo.
Clés : `medaillon_petit_frere_<famille>_2025`. Le joueur doit sentir que son petit frère a grandi en le
regardant ; laisse le jeu choisir l'expression (le même visage, plus dur ou plus apaisé selon la partie,
voir série A pour la règle).

---

# SÉRIE P — MÉDAILLONS DES PERSONNAGES

Prompt commun :

```
Sprite sheet of 8 character portraits in a strict 4 columns x 2 rows grid, every cell the same size,
on a perfectly flat pure magenta #FF00FF background, no shadow, nothing touching the cell borders.
Each portrait: head and shoulders, centered, facing slightly to the left, same size and same eye height
in every cell, so they can be cut into round medallions. Realistic, dignified people of Marseille,
varied faces and skin tones, no caricature, no villain faces. Everyday clothes, no logos, no readable
text, no weapons.
```

### P01 — l'entourage proche
1. **tante** Nadège, 48, hospital nurse, short auburn hair, scrubs under a cardigan, laughing eyes.
2. **ami_enfance** Kevin « Totti », 16, fair skin with freckles, short brown hair, plain sky-blue and white
   football shirt with no crest, friendly grin.
3. **camarade** Manon, 16, light skin, brown hair in a high bun, school backpack strap, ironic half smile.
4. **pitchoun** Ibrahima « Pitchoun », 15 but very tall and broad, dark skin, short hair, tracksuit jacket,
   proud jaw, gentle eyes.
5. **ines** Inès, 16, tan skin, black hair in a tight ponytail, handball team jacket, serious look.
6. **nono** Nordine « Nono », 19, tan skin, short beard, delivery jacket and a helmet under the arm, big smile.
7. **chouchou** Steven « Chouchou », 17, mixed heritage, curly hair, headphones around the neck, dreamy eyes.
8. **coach** Patrice « Coach Pat », 50, bald, grey goatee, club tracksuit, whistle on a cord, strict and warm.

### P02 — les gens du bâtiment
1. **educatrice** Christelle, 38, Corsican look, dark hair with a pencil in it, tired determined eyes.
2. **snack** Hassen, 52, North African, short grey hair, apron over a polo, warm but wary.
3. **voisine** Josiane « Mamie Jo », 71, white hair in curls, glasses on a chain, floral blouse, brave smile.
4. **rose_marie** Rose-Marie Lopes, 62, Portuguese, short dark dyed hair, clipboard against her chest, stubborn.
5. **farid** Farid « Monsieur Farid », 47, North African, moustache, building caretaker work jacket, observant.
6. **madame_lan** Thi Lan « Madame Lan », 55, Vietnamese, hair in a low bun, cardigan, calm knowing look.
7. **pastis** Jean-Claude « Pastis », 68, Provençal, weathered tanned face, flat cap, old docker's jacket, grumpy.
8. **babou** Gérard « Babou », 56, white, grey stubble, deep lines on his face, calm sad eyes, cardigan.

### P03 — le hall
1. **grand_hall** Anthony « Zépek », 24, mixed Mediterranean features, neat short beard, plain dark puffer
   jacket, calm calculating gaze, almost a smile.
2. **souris** Dylan « Souris », 14, small and thin, pale skin, oversized hoodie with the hood half up,
   frightened alert eyes. He is a child: draw him as a child.
3. **rascasse** Grégory « Rascasse », 31, white, shaved head, nervous jaw, plain black jacket, suspicious.
4. **tata** Martine « Tata », 58, white, greying hair, house cardigan, exhausted gentle face, ashamed eyes.
5. **mika** Mickaël « Mika », 28, Corsican-Italian features, short hair, mechanic's hands visible at the
   collar of a work jacket, calm.
6. **zizou** Yacine « Zizou », 20, North African, faded haircut, plain tracksuit, impatient look.
7. **bebe** Rudy « Bébé », 17, Malagasy-French, round young face, nervous smile, cap without logo.
8. **sab** Sabrina « Sab », 26, North African, sleek hair, blazer, minimal makeup, cold precise look.

### P04 — les institutions
1. **prof** Hélène Roux, 46, short grey-blond hair, scarf, reading glasses pushed up, fair and strict.
2. **cpe** Sylvie Bonnet, 44, red bob haircut, energetic, lanyard without readable badge.
3. **commandante** Agnès Vidal, 49, police commander in plain clothes, dark hair with grey, trench coat,
   patient stubborn gaze.
4. **brigadier** Julien « Robocop », 33, uniformed patrol officer, short blond hair, body armor vest with no
   readable text, tired hard face.
5. **belkacem** lieutenant Karim Belkacem, 38, North African, plain clothes, open collar, attentive humane eyes.
6. **avocate** Maître Laure Castelli, 40, lawyer, black robe with white bands, dark hair pulled back, sharp.
7. **juge** Florence Arnaud, 52, children's judge, short white hair, simple dark jacket, attentive and firm.
8. **journaliste** Clara Mouret, 29, curly brown hair, notebook in hand, wary curious eyes.

### P05 — la ville
1. **elu** Denis Martinez, 57, local elected official, grey hair, suit without tie, affable public smile.
2. à 6. libres (réserve pour les prochains personnages) : laisse ces cases vides, magenta uni.
7. **chef_calanquais** « Petit Prince » Lorenzo, 22, sunburnt, bleached hair, too-big smile, impatient eyes.
8. **emblème calanquais** a white limestone cliff above blue water (simple emblem, no letters).

---

# SÉRIE A — LES JEUNES QUI ONT GRANDI (2025)

Même prompt commun que la série P. Joins P01 et P03, ajoute : « Same characters as the attached sheets,
four years older, same faces. » Le jeu affiche ces versions à partir de 2025.
1. **souris** Dylan, 18 · 2. **bebe** Rudy, 21 · 3. **pitchoun** Ibrahima, 19, very tall · 4. **chouchou**
Steven, 21 · 5. **ines** Inès, 20, police cadet uniform without readable text · 6. **camarade** Manon, 20,
medical student · 7. **ami_enfance** Kevin « Totti », 20 · 8. **nono** Nordine, 23.

---

# SÉRIE H — LE PROTAGONISTE (jeune / adulte / ancien)

Une planche par famille, pour que le personnage ressemble à sa mère et à ses frères et sœurs. Joins la
planche F de la famille.

Prompt commun :

```
Sprite sheet of 6 portraits of THE SAME young man in a strict 3 columns x 2 rows grid, every cell the
same size, perfectly flat pure magenta #FF00FF background, no shadow, nothing touching the cell borders.
Head and chest, centered, facing slightly to the left, same size and same eye height in every cell.
He belongs to the family in the attached sheet: same skin tone, same family features. Top row, neutral
face: 1) 16 years old, a teenager, hoodie; 2) 21 years old, short beard, plain dark jacket; 3) 40 years
old, lines on the forehead, a few grey hairs, plain shirt. Bottom row, the SAME three ages and clothes but
marked by life: 4) 16, a bruise on the cheekbone, red eyes, scared; 5) 21, hollow cheeks, prison-pale
skin, very short hair, grey sweatshirt; 6) 40, tired eyes, deep wrinkles, grief. Same face in all cells.
No logos, no readable text, no weapons.
```

| Planche | Famille | Clés |
|---|---|---|
| H01 | Ferrara | `joueur_ferrara_jeune`, `_adulte`, `_ancien`, `_jeune_abime`, `_adulte_abime`, `_ancien_abime` |
| H02 | Mmadi | `joueur_mmadi_…` |
| H03 | Benali | `joueur_benali_…` |
| H04 | Sarkissian | `joueur_sarkissian_…` |
| H05 | Diallo | `joueur_diallo_…` |
| H06 | Garcia | `joueur_garcia_…` |

« Ancien » sert à l'épilogue (« L'après ») : le jeu montre qui il est devenu. La version « abîmée » est
choisie par le jeu selon la santé, le stress, la prison et les deuils.

---

# SÉRIE S — SCÈNES D'ÉVÉNEMENTS

Prompt commun :

```
Sprite sheet of 6 narrative scenes in a strict 3 columns x 2 rows grid, every cell the same size (16:9),
thin dark navy gutters, each scene filling its cell edge to edge. Cinematic framing, characters seen from
a distance or from behind, faces small or turned away, emotion carried by posture, light and place.
Humane and sober, never glamorous. No readable text, no logos, no weapons, no drugs, no blood.
```

### S01 — le prologue
1. **le_quotidien** — Early morning in a family kitchen of a housing block: a mother putting on her coat at
   the door, a boy looking for his school bag, a teenager by the window looking down at the entrance hall.
2. **les_visages** — The entrance hall of a housing block at dusk, a young man leaning on the mailboxes
   gesturing "come here" to a teenager at the door, two silhouettes in the background.
3. **la_promesse** — A mother and her teenage son at a small kitchen table, a school report card on the
   table (no readable text), his phone lighting up face down.
4. **promesse_oubliee** — A person waiting alone at the foot of a building at night under a sodium
   streetlight, arms crossed, looking at a teenager arriving late.
5. **rancune** — In the housing estate courtyard, a group of teenagers turning their backs on one of them
   who stands alone with his hands in his pockets.
6. **essai_au_club** — A small municipal football pitch on a Saturday morning, a coach with a whistle,
   two teenagers in plain shirts, pine hills and housing blocks behind.

### S02 — le quartier
1. **soir_de_match** — A neighborhood snack bar at night full of people watching a football match on a TV
   (screen shows only a green pitch), plastic chairs on the pavement, joy and tension, sky-blue scarves
   without crest.
2. **mistral** — A street swept by a violent mistral wind: shutters banging, plastic bags flying, plane
   trees bending, a woman holding her hair, white clouds racing over blue sky.
3. **souris_dans_le_froid** — A very small boy in an oversized hoodie sitting alone on a low wall at night,
   under a streetlight, hood up, looking at the street. Cold blue light. Loneliness, not glamour.
4. **babou_raconte** — Old men playing pétanque in front of a housing block at sunset, one older man with a
   sad face talking to a teenager who listens.
5. **le_son_de_chouchou** — A tiny night grocery store, a teenager playing music from his phone on a small
   speaker on the counter, a Vietnamese shopkeeper raising an eyebrow, another teenager listening.
6. **l_ascenseur** — A building entrance hall with an "out of order" elevator (shown by a taped plain sheet
   with no text), a retired woman holding a petition clipboard, young men leaning on the opposite wall,
   the caretaker looking down.

### S03 — la police et la peur
1. **la_descente** — 6 a.m. in a housing block stairwell: police officers in plain dark gear going down the
   stairs (no readable text, no weapons drawn), a small boy in pajamas peeking from a half-open door,
   blue light from outside.
2. **le_lieutenant** — In front of a high school gate, a man in plain clothes speaking calmly to a
   teenager with a backpack, other students walking past.
3. **les_absents** — A white march in the neighborhood: families walking slowly with white balloons and
   white roses, a mother holding a framed photo turned away from the viewer, an educator at her side.
4. **controle** — A police identity check at a bus stop at night, three teenagers against a wall, an officer
   with a flashlight, a bus passing.
5. **garde_a_vue** — A bare police custody room: a teenager sitting on a concrete bench, head in his hands,
   harsh white light, a closed metal door.
6. **convocation** — A mother and her son sitting in a corridor of a courthouse for minors, waiting on plastic
   chairs, the mother holding a folder.

### S04 — la prison et le deuil
1. **parloir** — A prison visiting room: a mother across a small table from her son in a grey sweatshirt,
   other families at other tables, hands not touching.
2. **cellule** — A small prison cell shared by two, bunk beds, a barred window with a sliver of blue sky,
   a family photo taped to the wall (faces not visible).
3. **sortie_de_prison** — A young man walking out of a prison gate with a plastic bag, the white hills
   behind, nobody waiting, or a single small figure far away.
4. **enterrement** — A Marseille cemetery on a sunny windy day, a crowd in dark clothes, cypress trees, a
   mother supported by two women, young men standing apart.
5. **mur_des_noms** — A wall of a housing block with flowers, candles and photos (no readable names) at
   its foot, kids passing by on bikes.
6. **mere_qui_attend** — A mother alone at a kitchen window at night, phone in hand, waiting, the city
   lights below.

### S05 — l'argent et la ville
1. **facade_snack** — A small renovated snack bar with a new awning (no text), a teenager behind the counter,
   an accountant with a laptop at a table, too clean, too new.
2. **salle_de_boxe** — A neighborhood boxing gym, kids training on bags, a coach, posters without text,
   a man in a jacket watching from the office window.
3. **reseau_international** — Abstract: a large port at night seen from above, container cranes, a ship
   leaving, lights on the sea. No people, no goods visible.
4. **operation_police** — A housing estate at dawn surrounded by many police vans with blue lights (no
   readable text), a helicopter light in the sky, residents watching from balconies.
5. **mairie** — An inauguration on a sunny square: a local official cutting a ribbon, a few residents,
   some clapping, some with arms crossed, a renovated playground.
6. **journaliste** — A young journalist interviewing a mother on a bench at the foot of a building, notebook
   open, the mother looking away.

### S06 — l'après
1. **depart_train** — Saint-Charles railway station's big staircase at dawn, a young man with a sports bag
   going down alone toward the city.
2. **nouvelle_vie** — A small workshop or kitchen at work, the protagonist as an adult in work clothes,
   sunlight, a coworker laughing.
3. **petit_frere_grandi** — Two brothers sitting on the rocks of the Corniche at sunset, backs to the viewer.
4. **chef** — A man seen from behind on a rooftop terrace above the city at night, phone glowing, the city
   lights spread below, completely alone.
5. **retour_quartier** — The protagonist as a man around 40 walking back into his old housing estate,
   kids playing football, an old woman at her window recognizing him.
6. **table_de_famille** — A family meal on a small balcony in summer, some chairs empty.

---

# SÉRIE I — ICÔNES

Prompt commun :

```
Icon sheet of 24 icons in a strict 6 columns x 4 rows grid, every cell the same size, perfectly flat pure
magenta #FF00FF background, each icon centered, same visual weight, nothing touching cell borders.
Style: round strategy-game badges like map markers: a dark charcoal disc (#1b1c1f) with a thick colored ring
and a simple bold white glyph in the middle, subtle bevel and drop shadow, readable at 32 px. Ring color per
icon: gold #e8b64c by default, blue #3d7fd9 for police, red #e5484d for danger, green #6ccf4f for the player. No readable text, no letters, no numbers, no logos, no weapons,
no drugs.
```

### I01 — interface
1 énergie (lightning bolt) · 2 sang-froid (snowflake inside a calm circle) · 3 argent propre (clean folded
banknote) · 4 argent sale (crumpled banknotes in a rubber band) · 5 police (blue rotating light) · 6 tension
(cracked ground) · 7 humeur du quartier (house with a heart) · 8 respect (raised chin silhouette with a
laurel) · 9 influence (an open hand sending out ripples) · 10 crainte (shadow looming) ·
11 confiance (two hands shaking) · 12 stress (head with tangled lines) · 13 santé (heart with pulse) ·
14 école (backpack) · 15 promesse (pinky swear hands) · 16 journal (notebook) · 17 carte (folded map with a
pin) · 18 gens (three heads) · 19 téléphone (smartphone with a message bubble, no text) · 20 chapitre (open
book with a bookmark) · 21 semaine (calendar page, blank) · 22 sauvegarde (cloud with arrow) · 23 alerte
(exclamation shape without letters: a triangle) · 24 maison (apartment block).

### I02 — compétences, rôles, marchandise
1 communication (two speech bubbles, empty) · 2 organisation (checklist with ticks, no text) · 3 sang-froid
(still water drop) · 4 leadership (person with arrow of others following) · 5 autonomie (key) · 6 guetteur
(eye) · 7 vendeur (open hand) · 8 nourrice (closed apartment door with a lock) · 9 chauffeur (steering wheel)
· 10 bras droit (shoulder with a hand on it) · 11 avocat (lawyer's bands) · 12 comptable (calculator, blank
screen) · 13 petite marchandise (small closed cardboard box) · 14 marchandise moyenne (several closed boxes)
· 15 marchandise lourde (a pallet of closed boxes under a tarp) · 16 snack (fries cone) · 17 boxe (boxing
glove) · 18 restaurant (plate and cutlery) · 19 appartements (building with a key) · 20 origine Maroc
(abstract: dunes and a harbor lighthouse) · 21 origine Pays-Bas (abstract: canal houses silhouette and a
container crane) · 22 enquête (magnifying glass over a footprint) · 23 prison (barred window) · 24 deuil
(white rose).

### I03 — ville et événements
1 match (football, plain white) · 2 mistral (wind gust lines) · 3 été (sun) · 4 pluie (cloud with rain) ·
5 fête (string lights) · 6 marché (fruit crate) · 7 bus (front of a bus, no number) · 8 métro (metro train
front, no logo) · 9 plage (parasol) · 10 Notre-Dame de la Garde (basilica silhouette) · 11 panisse (golden
chickpea fritters) · 12 pastis (glass of cloudy yellow drink with a carafe of water) · 13 pétanque (three
boules and a jack) · 14 bateau (pointu fishing boat) · 15 gabian (seagull) · 16 cigale (cicada) · 17 école
fermée (school gate closed) · 18 incendie (flame on a hillside) · 19 descente (door with a boot print) ·
20 marche blanche (white balloon) · 21 rumeur (ear with waves) · 22 message (envelope) · 23 rendez-vous
(clock) · 24 sortie (open door with light).

---

# SÉRIE R — LES ÉQUIPES RIVALES (inventées)

Prompt commun de la série P (portraits 4 × 2). Ligne 1 : les chefs, ligne 2 : leurs emblèmes.

Ligne 1 : 1. **La Compagnie** — « Le Notaire » Kenzo, 34, seen in a dim car interior, expensive plain coat,
face half in shadow, calm and distant · 2. **Les Gabians** — « Grand-Mère » Jordan, 27, hard face, plain
tracksuit, cold eyes · 3. **Le Ruisseau** — « Tonton » Fabrice, 45, heavy-set, grey hair, kind-looking
uncle face that worries more than it reassures · 4. **Tramontane** — « La Prof » Océane, 31, elegant,
glasses, night-bar light.
Ligne 2 (emblèmes simples, sans lettres, faciles à lire en petit) : 5. **compagnie** a black chess rook ·
6. **gabians** a seagull in flight · 7. **ruisseau** a stream of water between two stones · 8. **tramontane**
a north-wind gust curl.
Les Calanquais (chef et emblème) sont dans les cases 7 et 8 de P05.

---

# SÉRIE E — GRANDS ÉCRANS (une image par prompt, format vertical 9:16)

Ajouter : « Vertical 9:16 full illustration. The top 30% is calm sky because the game puts the title
there. No readable text. »

- **E1 ecran_accueil** — Marseille at night seen from the roof of a housing block in the north: a teenager
  sitting on the edge, back to the viewer, Notre-Dame de la Garde lit in the distance, the port, the sea,
  sodium lights, a few stars.
- **E2 ecran_fin** — The same rooftop at dawn, empty, a hoodie left on the edge, the city waking up.
- **E3 ecran_chapitre_la_confiance** — A hand passing a set of keys to another hand in a dark stairwell.
- **E4 ecran_chapitre_les_responsabilites** — An 18th birthday cake with candles in a small kitchen, a
  phone buzzing next to it, a mother's hands.
- **E5 ecran_chapitre_l_intermediaire** — A city map spread on a table under a lamp, coffee cups, several
  phones, hands pointing (map without readable names).
- **E6 ecran_chapitre_la_direction** — A man in his early twenties standing alone on the Corniche at night,
  wind in his jacket, facing the dark sea.

---

## Ce que le code fera (inutile de le dessiner)

- Le jour et la nuit des scènes : le jeu utilise les deux versions des bannières ; pluie et mistral
  ajoutés par le jeu sur les bannières.
- Les cadres ronds des médaillons, les jauges, les boutons, les textes.
- Le choix entre version normale et « abîmée » du protagoniste.
- La vue de la ville en relief (immeubles, territoires teintés, badges) est dessinée par le jeu, à partir
  de `data/zones.json` : pas besoin d'image de carte. Les icônes I01–I03 remplaceront les badges dessinés.

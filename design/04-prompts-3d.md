# CHOUF — prompts Gemini 3D (version 2, remplace le document 03)

Généré par `tools/gen_prompts.py` à partir des données du jeu. **Un bloc = une image.** Chaque bloc est complet : copie-le tel quel dans Gemini, sans rien ajouter. Le nom en gras est le nom du fichier à m'envoyer (`banniere_sud.png`, `medaillon_babou.png`…), à déposer dans `assets/brut/`.

## Pourquoi cette version

- **Filtres de Gemini** : la version 03 contenait des mots qui bloquent (crime, armes, drogue, même dans « sans armes, sans drogue »), des mineurs dans des scènes de police ou de peur, des blessures. Ici : aucun de ces mots, les enfants et ados n'apparaissent que dans la vie ordinaire, les scènes de police, de justice et de prison ne montrent que des adultes.
- **Ressemblance avec tes références** : la ville est rendue comme une **maquette 3D grise** vue d'en haut (bâtiments lisses, presque sans texture, flou de maquette), comme la vue de ville de tes captures. Les couleurs des équipes sont ajoutées par le jeu, pas par Gemini.
- **Fiabilité** : plus de grandes planches de 6 ou 8 personnages (Gemini ne respecte pas les cases). Une image par personnage, sur fond gris foncé : le jeu les recadre en rond, pas besoin de fond magenta. Les icônes restent par 9 (3 × 3) sur fond noir, faciles à découper.

## Si Gemini refuse ou se trompe

- Refus : relance une fois tel quel (les refus sont parfois aléatoires). Sinon, enlève la phrase qui décrit l'émotion ou l'action et garde le lieu. Dis-moi lequel : je réécris le bloc.
- Pas assez 3D : ajoute au début « 3D render, game engine screenshot, not a drawing, not a painting ».
- Pour garder le même visage (famille, protagoniste à plusieurs âges) : joins l'image déjà faite et ajoute à la fin « Same person as in the attached image. »
- Format : Gemini choisit parfois un carré. Redemande « wide 16:9 » ou recadre, le jeu accepte.

## Inventaire

| Série | Images |
|---|---|
| Z — vues de ville 3D | 12 |
| F — familles | 18 |
| P — personnages | 33 |
| R — chefs des équipes | 5 |
| H — protagoniste (6 familles × 3 âges) | 18 |
| S — scènes | 36 |
| I — icônes (planches de 9) | 8 planches = 72 icônes |
| E — écrans 9:16 | 6 |


---

# Z — VUES DE VILLE 3D (bannières des zones)

Le jeu fait lui-même la version de nuit et la teinte de l'équipe qui tient la zone.

**banniere_centre_hist**
```
3D render, strategy game city view. A miniature architectural model of the Vieux-Port of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. The rectangular old harbor full of small white boats, the large flat mirrored canopy on the quay, dense 4–6 storey buildings around it, the hill of Notre-Dame de la Garde with its basilica and bell tower in the background, the old Panier district climbing on the right. Wide 16:9 image. No people, no text, no logos.
```

**banniere_centre_nord**
```
3D render, strategy game city view. A miniature architectural model of the Belle de Mai district of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. Tight blocks of tall 19th-century buildings, a large old tobacco-factory complex with brick chimneys turned cultural venue, a railway yard on one side. Wide 16:9 image. No people, no text, no logos.
```

**banniere_plaine**
```
3D render, strategy game city view. A miniature architectural model of the Cours Julien and La Plaine district of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. A long pedestrian terraced street with stairs and trees, a large rectangular square lined with plane trees, small dense buildings around. Wide 16:9 image. No people, no text, no logos.
```

**banniere_prado**
```
3D render, strategy game city view. A miniature architectural model of the Castellane and Prado district of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. A big roundabout with a monumental fountain in the middle, a long straight tree-lined avenue running toward the sea, an oval football stadium with a wavy white roof further along. Wide 16:9 image. No people, no text, no logos.
```

**banniere_nord_est**
```
3D render, strategy game city view. A miniature architectural model of the north-eastern districts of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. Large 1970s housing towers and long slab buildings on green hillsides, a small football pitch, an elevated metro line crossing the scene, pine hills behind. Wide 16:9 image. No people, no text, no logos.
```

**banniere_nord_central**
```
3D render, strategy game city view. A miniature architectural model of the 14th district of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. A mix of small houses with tiled roofs and long housing slabs, a large flat shopping-center roof, a busy road and a big parking lot. Wide 16:9 image. No people, no text, no logos.
```

**banniere_nord_ouest**
```
3D render, strategy game city view. A miniature architectural model of the north-western hills of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. Tall housing towers on hills above the sea, the container port with cranes and docks far below, a large shopping mall in the valley, a hospital on a hill. Wide 16:9 image. No people, no text, no logos.
```

**banniere_littoral_nord**
```
3D render, strategy game city view. A miniature architectural model of L'Estaque in Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. A small fishing harbor with little wooden boats, old village houses climbing the hill, a stone railway viaduct, the big container port and its cranes on the horizon. Wide 16:9 image. No people, no text, no logos.
```

**banniere_est**
```
3D render, strategy game city view. A miniature architectural model of the eastern districts of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. Old village centers with small squares and church bell towers, low houses with tiled roofs, white limestone hills behind, a big suburban shopping center in the distance. Wide 16:9 image. No people, no text, no logos.
```

**banniere_sud**
```
3D render, strategy game city view. A miniature architectural model of the southern seafront of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. A long beach with a seaside promenade, a park with palm trees and a small château, the white limestone cliffs of the calanques at the end of the bay. Wide 16:9 image. No people, no text, no logos.
```

**banniere_baumettes**
```
3D render, strategy game city view. A miniature architectural model of a large modern correctional facility at the foot of white limestone hills in the south of Marseille, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. High plain grey walls, rectangular buildings, a gate and a small visitor parking lot with a bench, quiet and sober. Wide 16:9 image. No people, no text, no logos.
```

**banniere_panorama**
```
3D render, strategy game city view. A miniature architectural model of the whole city of Marseille seen from far above, made of matte light-grey clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. The old harbor in the middle, the cathedral by the sea, the basilica on its hill, the islands offshore, the northern hills far away. Wide 16:9 image. No people, no text, no logos.
```


---

# F — FAMILLES

Fais les trois personnes d'une même famille dans la même conversation Gemini, en joignant la mère aux deux suivants (« Same family as the attached image, same features »).


### Famille Ferrara

**medaillon_mere_ferrara**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French-Italian family from Marseille: a 43-year-old woman, dark blond hair held in a clip, small gold cross necklace, supermarket uniform polo, tired but warm eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_petit_frere_ferrara**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French-Italian family from Marseille: an 11-year-old boy, light olive skin, messy brown hair, plain sky-blue t-shirt, curious smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_grande_soeur_ferrara**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French-Italian family from Marseille: a 20-year-old woman, long dark brown hair, small hoop earrings, denim jacket, sharp lucid gaze. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Famille Mmadi

**medaillon_mere_mmadi**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Comorian heritage: a 42-year-old woman, dark skin, colorful patterned headscarf, nursing assistant tunic, calm and proud. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_petit_frere_mmadi**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Comorian heritage: an 11-year-old boy, dark skin, short hair, grey hoodie, big admiring eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_grande_soeur_mmadi**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Comorian heritage: a 20-year-old woman, dark skin, braids tied up, beige trench coat, straight serious look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Famille Benali

**medaillon_mere_benali**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Algerian heritage: a 44-year-old woman, dark curly hair with grey strands, simple cardigan, strong worried face. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_petit_frere_benali**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Algerian heritage: an 11-year-old boy, tan skin, black hair short on the sides, plain red t-shirt, mischievous smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_grande_soeur_benali**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Algerian heritage: a 20-year-old woman, dark hair in a ponytail, thin glasses, direct look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Famille Sarkissian

**medaillon_mere_sarkissian**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Armenian heritage: a 45-year-old woman, dark hair in a bob, elegant navy cardigan, anxious eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_petit_frere_sarkissian**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Armenian heritage: an 11-year-old boy, pale olive skin, thick dark hair, green sweater, curious. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_grande_soeur_sarkissian**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Armenian heritage: a 20-year-old woman, long black hair, strong eyebrows, black turtleneck, determined. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Famille Diallo

**medaillon_mere_diallo**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Senegalese heritage: a 41-year-old woman, dark skin, wax-print headwrap, canteen apron, broad warm smile, tired eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_petit_frere_diallo**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Senegalese heritage: an 11-year-old boy, dark skin, short hair, plain sky-blue t-shirt, laughing. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_grande_soeur_diallo**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French family of Senegalese heritage: a 20-year-old woman, dark skin, short natural hair, small nose ring, lucid look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Famille Garcia

**medaillon_mere_garcia**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French Gitan family settled in Marseille for generations: a 40-year-old woman, long black hair, gold earrings, dark blouse, strong protective expression. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_petit_frere_garcia**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French Gitan family settled in Marseille for generations: an 11-year-old boy, tan skin, black curls, white t-shirt, cheeky smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**medaillon_grande_soeur_garcia**
```
3D render of a video game character portrait, realistic modern console game style. A member of a French Gitan family settled in Marseille for generations: a 20-year-old woman, black hair in a long braid, serious and kind. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


---

# P — PERSONNAGES

Nadège

**medaillon_tante**
```
3D render of a video game character portrait, realistic modern console game style. A 48-year-old hospital nurse, short auburn hair, light blue scrubs under a cardigan, laughing eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Kevin « Totti »

**medaillon_ami_enfance**
```
3D render of a video game character portrait, realistic modern console game style. A 16-year-old boy, fair skin with freckles, short brown hair, plain sky-blue and white football shirt with no badge, friendly grin. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Manon

**medaillon_camarade**
```
3D render of a video game character portrait, realistic modern console game style. A 16-year-old girl, light skin, brown hair in a high bun, school backpack strap on the shoulder, ironic half smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Ibrahima « Pitchoun »

**medaillon_pitchoun**
```
3D render of a video game character portrait, realistic modern console game style. A 15-year-old boy, very tall and broad for his age, dark skin, short hair, track jacket, gentle eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Inès

**medaillon_ines**
```
3D render of a video game character portrait, realistic modern console game style. A 16-year-old girl, tan skin, black hair in a tight ponytail, sports team jacket, serious look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Nordine « Nono »

**medaillon_nono**
```
3D render of a video game character portrait, realistic modern console game style. A 19-year-old young man, tan skin, short beard, delivery rider jacket, a scooter helmet under his arm, big smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Steven « Chouchou »

**medaillon_chouchou**
```
3D render of a video game character portrait, realistic modern console game style. A 17-year-old boy of mixed heritage, curly hair, headphones around the neck, dreamy eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Patrice « Coach Pat »

**medaillon_coach**
```
3D render of a video game character portrait, realistic modern console game style. A 50-year-old football coach, bald head, grey goatee, club tracksuit, whistle on a cord, strict but warm. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Christelle

**medaillon_educatrice**
```
3D render of a video game character portrait, realistic modern console game style. A 38-year-old social worker of Corsican heritage, dark hair with a pencil stuck in it, tired determined eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Hassen

**medaillon_snack**
```
3D render of a video game character portrait, realistic modern console game style. A 52-year-old snack bar owner, short grey hair, apron over a polo shirt, warm but wary look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Josiane « Mamie Jo »

**medaillon_voisine**
```
3D render of a video game character portrait, realistic modern console game style. A 71-year-old woman, white curly hair, glasses on a chain, floral blouse, brave smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Rose-Marie

**medaillon_rose_marie**
```
3D render of a video game character portrait, realistic modern console game style. A 62-year-old woman of Portuguese heritage, short dyed dark hair, holding a clipboard against her chest, stubborn look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Farid « Monsieur Farid »

**medaillon_farid**
```
3D render of a video game character portrait, realistic modern console game style. A 47-year-old building caretaker, moustache, navy work jacket, observant eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Thi Lan « Madame Lan »

**medaillon_madame_lan**
```
3D render of a video game character portrait, realistic modern console game style. A 55-year-old Vietnamese-French shopkeeper, hair in a low bun, cardigan, calm knowing look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Jean-Claude « Pastis »

**medaillon_pastis**
```
3D render of a video game character portrait, realistic modern console game style. A 68-year-old retired docker from Provence, weathered tanned face, flat cap, old work jacket, grumpy. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Gérard « Babou »

**medaillon_babou**
```
3D render of a video game character portrait, realistic modern console game style. A 56-year-old man, grey stubble, deep lines on his face, beige cardigan, calm sad eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Anthony « Zépek »

**medaillon_grand_hall**
```
3D render of a video game character portrait, realistic modern console game style. A 24-year-old man, neat short beard, plain dark puffer jacket, calm calculating gaze, almost a smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Dylan « Souris »

**medaillon_souris**
```
3D render of a video game character portrait, realistic modern console game style. A 14-year-old boy, small and thin, pale skin, oversized grey hoodie, alert eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Grégory « Rascasse »

**medaillon_rascasse**
```
3D render of a video game character portrait, realistic modern console game style. A 31-year-old man, shaved head, tense jaw, plain black jacket, suspicious look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Martine « Tata »

**medaillon_tata**
```
3D render of a video game character portrait, realistic modern console game style. A 58-year-old woman, greying hair, house cardigan, exhausted gentle face. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Mickaël « Mika »

**medaillon_mika**
```
3D render of a video game character portrait, realistic modern console game style. A 28-year-old man, short hair, mechanic's work jacket, calm face. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Yacine « Zizou »

**medaillon_zizou**
```
3D render of a video game character portrait, realistic modern console game style. A 20-year-old man, fade haircut, plain navy tracksuit, impatient look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Rudy « Bébé »

**medaillon_bebe**
```
3D render of a video game character portrait, realistic modern console game style. A 17-year-old boy, round young face, plain cap, nervous smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Sabrina « Sab »

**medaillon_sab**
```
3D render of a video game character portrait, realistic modern console game style. A 26-year-old woman, sleek dark hair, grey blazer, minimal makeup, precise cool look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Hélène

**medaillon_prof**
```
3D render of a video game character portrait, realistic modern console game style. A 46-year-old high school teacher, short grey-blond hair, scarf, reading glasses pushed up on her head, fair and strict. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Sylvie

**medaillon_cpe**
```
3D render of a video game character portrait, realistic modern console game style. A 44-year-old school counselor, red bob haircut, lanyard with a blank badge, energetic. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Agnès

**medaillon_commandante**
```
3D render of a video game character portrait, realistic modern console game style. A 49-year-old woman detective in plain clothes, dark hair with grey, beige trench coat, patient stubborn gaze. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Julien « Robocop »

**medaillon_brigadier**
```
3D render of a video game character portrait, realistic modern console game style. A 33-year-old police officer in a plain dark navy uniform, short blond hair, tired hard face. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Karim

**medaillon_belkacem**
```
3D render of a video game character portrait, realistic modern console game style. A 38-year-old detective in plain clothes, open shirt collar, short dark hair, attentive humane eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Laure

**medaillon_avocate**
```
3D render of a video game character portrait, realistic modern console game style. A 40-year-old French lawyer in a black court robe with white bands, dark hair pulled back, sharp eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Florence

**medaillon_juge**
```
3D render of a video game character portrait, realistic modern console game style. A 52-year-old judge, short white hair, simple dark jacket, attentive and firm. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Clara

**medaillon_journaliste**
```
3D render of a video game character portrait, realistic modern console game style. A 29-year-old journalist, curly brown hair, notebook in hand, curious wary eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

Denis

**medaillon_elu**
```
3D render of a video game character portrait, realistic modern console game style. A 57-year-old local politician, grey hair, suit without tie, affable public smile. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


---

# R — CHEFS DES ÉQUIPES (inventés)

**chef_compagnie**
```
3D render of a video game character portrait, realistic modern console game style. A 34-year-old businessman seen in the back seat of a dark car, expensive plain coat, half of his face in shadow, calm distant look. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**chef_gabians**
```
3D render of a video game character portrait, realistic modern console game style. A 27-year-old man, hard face, plain grey tracksuit, cold eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**chef_ruisseau**
```
3D render of a video game character portrait, realistic modern console game style. A 45-year-old heavy-set man, grey hair, friendly uncle face that does not quite reassure. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**chef_tramontane**
```
3D render of a video game character portrait, realistic modern console game style. A 31-year-old elegant woman, glasses, black blazer, lit by the warm lights of a night bar. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**chef_calanquais**
```
3D render of a video game character portrait, realistic modern console game style. A 22-year-old young man, sunburnt skin, bleached hair, too-big smile, impatient eyes. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


---

# H — LE PROTAGONISTE

Pour chaque famille : fais d'abord **16 ans** en joignant l'image de la mère de la famille (« He is her son, same family features. »), puis 21 et 40 ans en joignant l'image de 16 ans (« Same person as in the attached image, older. »). La version fatiguée ou abîmée est faite par le jeu (teinte, ombres).


### Ferrara

**joueur_ferrara_jeune**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French-Italian family from Marseille: a 16-year-old teenager, grey hoodie. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_ferrara_adulte**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French-Italian family from Marseille: a 21-year-old young man, short beard, plain dark jacket. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_ferrara_ancien**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French-Italian family from Marseille: a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Mmadi

**joueur_mmadi_jeune**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Comorian heritage: a 16-year-old teenager, grey hoodie. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_mmadi_adulte**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Comorian heritage: a 21-year-old young man, short beard, plain dark jacket. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_mmadi_ancien**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Comorian heritage: a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Benali

**joueur_benali_jeune**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Algerian heritage: a 16-year-old teenager, grey hoodie. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_benali_adulte**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Algerian heritage: a 21-year-old young man, short beard, plain dark jacket. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_benali_ancien**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Algerian heritage: a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Sarkissian

**joueur_sarkissian_jeune**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Armenian heritage: a 16-year-old teenager, grey hoodie. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_sarkissian_adulte**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Armenian heritage: a 21-year-old young man, short beard, plain dark jacket. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_sarkissian_ancien**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Armenian heritage: a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Diallo

**joueur_diallo_jeune**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Senegalese heritage: a 16-year-old teenager, grey hoodie. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_diallo_adulte**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Senegalese heritage: a 21-year-old young man, short beard, plain dark jacket. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_diallo_ancien**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French family of Senegalese heritage: a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


### Garcia

**joueur_garcia_jeune**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French Gitan family settled in Marseille for generations: a 16-year-old teenager, grey hoodie. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_garcia_adulte**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French Gitan family settled in Marseille for generations: a 21-year-old young man, short beard, plain dark jacket. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```

**joueur_garcia_ancien**
```
3D render of a video game character portrait, realistic modern console game style. A young man from a French Gitan family settled in Marseille for generations: a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt. Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from Marseille. Square 1:1 image, face in the middle third. No text, no logos.
```


---

# S — SCÈNES

**scene_le_quotidien**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Early morning in a small family kitchen in a housing block: a mother putting on her coat at the door, a boy searching for his school bag, a teenager by the window looking down at the street. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_les_visages**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. The entrance hall of a housing block at dusk: a young man in his twenties leaning on the mailboxes waves to a teenager at the door to come over. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_la_promesse**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A mother and her teenage son sitting at a small kitchen table at night, a school report card on the table, his phone lighting up face down. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_promesse_oubliee**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A woman waiting alone at the foot of a building at night under a streetlight, arms crossed, watching someone arrive late. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_rancune**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. In the courtyard of a housing estate, a group of teenagers turn their backs on one of them who stands alone, hands in his pockets. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_essai_au_club**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A small municipal football pitch on a Saturday morning, a coach with a whistle, two teenagers in plain shirts, pine hills and housing blocks behind. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_soir_de_match**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A neighborhood snack bar at night full of people watching a football match on a TV that shows only a green pitch, plastic chairs on the pavement, sky-blue scarves, joy and tension. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_mistral**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A street in Marseille in a violent mistral wind: shutters banging, plane trees bending, a woman holding her hair, white clouds racing over a deep blue sky. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_souris_dans_le_froid**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Night in an empty housing estate: a young teenager in an oversized hoodie sits alone on a low wall under a streetlight, looking at the street. Cold blue light, loneliness. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_babou_raconte**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Older men playing pétanque in front of a housing block at sunset, one of them talking to a teenager who listens. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_le_son_de_chouchou**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A tiny night grocery store: a teenager plays music from his phone on a small speaker on the counter, the shopkeeper raises an eyebrow, a friend listens. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_l_ascenseur**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A building entrance hall with an elevator out of order, a retired woman holding a clipboard petition, young men leaning on the opposite wall, the caretaker looking at his shoes. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_la_descente**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Dawn in the stairwell of a housing block: several police officers in dark uniforms climb the stairs quietly; on the landing a woman in a dressing gown holds her door half open, worried. Blue light through the window. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_le_lieutenant**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. In front of a high school gate, a man in plain clothes talks calmly with a young man carrying a backpack, students walking past. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_les_absents**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A silent white march through a neighborhood: families walking slowly with white balloons and white roses, a woman at the front holding a framed photo turned away from the viewer. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_controle**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Night at a bus stop: two police officers speak with three young men in their twenties against a wall, a bus passing with its lights on. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_garde_a_vue**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A bare waiting room in a police station at night: a young man about twenty sits alone on a bench, head in his hands, harsh white light, a closed door. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_convocation**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A mother and her son sitting side by side on plastic chairs in a courthouse corridor, waiting, the mother holding a folder. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_parloir**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. The visiting room of a correctional facility: a mother sits across a small table from her adult son in a grey sweatshirt, other families at other tables. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_cellule**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A small shared room in a correctional facility: bunk beds, a barred window with a sliver of blue sky, a family photo taped to the wall. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_sortie_de_prison**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A young man walks out of the gate of a correctional facility with a plastic bag, white hills behind him, a small figure waiting far away. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_enterrement**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A Marseille cemetery on a sunny windy day: a crowd in dark clothes among cypress trees, a mother supported by two women, young men standing apart. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_mur_des_noms**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. The wall of a housing block with flowers, candles and photos at its foot, children passing by on bikes. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_mere_qui_attend**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A mother alone at a kitchen window at night, phone in hand, waiting, the city lights below. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_facade_snack**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A small newly renovated snack bar, a teenager behind the counter, an accountant with a laptop at a table, everything too clean and too new. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_salle_de_boxe**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A neighborhood boxing gym: kids training on bags, a coach, a man in a jacket watching from the office window. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_reseau_international**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A large container port at night seen from above, cranes, a cargo ship leaving, lights on the sea. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_operation_police**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Dawn around a housing estate: many police vans with blue lights, a helicopter light in the sky, residents watching from their balconies. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_mairie**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. An inauguration on a sunny square: a local official cutting a ribbon, some residents clapping, others with arms crossed, a renovated playground. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_journaliste**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A young journalist interviews a mother on a bench at the foot of a building, notebook open, the mother looking away. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_depart_train**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. The monumental staircase of Marseille's Saint-Charles railway station at dawn, a young man with a sports bag walking down alone toward the city. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_nouvelle_vie**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A small workshop in daylight, a young man in work clothes laughing with a coworker. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_petit_frere_grandi**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. Two brothers sitting on the rocks of the Corniche at sunset, seen from behind, the sea and islands ahead. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_chef**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A man seen from behind on a rooftop terrace above Marseille at night, phone glowing in his hand, the city lights below, completely alone. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_retour_quartier**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A man around forty walks back into his old housing estate, kids playing football, an old woman at her window recognizing him. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```

**scene_table_de_famille**
```
3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. A family meal on a small balcony in summer, laughter, two chairs empty. Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, shallow depth of field, people shown at a distance or from behind, sober and humane mood. Wide 16:9 image. No text, no logos.
```


---

# I — ICÔNES (3 × 3 sur fond noir)

Le jeu découpe chaque rond. Donne-moi la planche entière.


I01 — ressources

**icones_i01**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a lightning bolt; a snowflake; a folded banknote; a bundle of banknotes with a rubber band; a police siren light; cracked ground; a house with a heart; a laurel wreath; an open hand sending ripples. No text, no letters, no numbers.
```


I02 — état

**icones_i02**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a looming shadow; two hands shaking; a head with tangled lines; a heart with a pulse line; a school backpack; two hooked little fingers; an open notebook; a folded map with a pin; three heads. No text, no letters, no numbers.
```


I03 — interface

**icones_i03**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a smartphone with a speech bubble; an open book with a bookmark; a blank calendar page; a cloud with an arrow; a warning triangle; an apartment block; a key; an eye; an open hand. No text, no letters, no numbers.
```


I04 — rôles et commerce

**icones_i04**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a closed door; a steering wheel; a hand on a shoulder; a pair of lawyer bands; a calculator; a small closed cardboard box; three closed cardboard boxes; a pallet of boxes under a tarp; a cone of fries. No text, no letters, no numbers.
```


I05 — commerces et réseaux

**icones_i05**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a boxing glove; a plate with fork and knife; a building with a key; sand dunes and a lighthouse; canal houses and a crane; a magnifying glass over a footprint; a barred window; a white rose; a stack of coins. No text, no letters, no numbers.
```


I06 — ville

**icones_i06**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a football; wind gust lines; a sun; a cloud with rain; string lights; a fruit crate; the front of a bus; the front of a metro train; a beach umbrella. No text, no letters, no numbers.
```


I07 — Marseille

**icones_i07**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a hilltop basilica with a tall bell tower; golden fried chickpea sticks; a tall glass of cloudy yellow drink with a water jug; three pétanque balls and a small jack; a small pointed fishing boat; a seagull; a cicada; a closed school gate; a flame on a hill. No text, no letters, no numbers.
```


I08 — événements

**icones_i08**
```
3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, all badges the same size. Symbols, left to right then top to bottom: a door with a boot print; a white balloon; an ear with sound waves; an envelope; a clock; an open door with light; a chess rook; a wave; a curl of wind. No text, no letters, no numbers.
```


---

# E — ÉCRANS (vertical 9:16)

**ecran_accueil**
```
3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine look. Marseille at night seen from the roof of a housing block on the northern hills: a teenager sits on the edge with his back to the viewer, the basilica lit on its hill in the distance, the port and the sea. Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. Vertical 9:16 image, the top third is calm sky. No text, no logos.
```

**ecran_fin**
```
3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine look. The same rooftop above Marseille at dawn, empty, a hoodie left on the edge, the city waking up. Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. Vertical 9:16 image, the top third is calm sky. No text, no logos.
```

**ecran_chapitre_la_confiance**
```
3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine look. Close-up of one hand passing a set of keys to another hand in a dim stairwell. Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. Vertical 9:16 image, the top third is calm sky. No text, no logos.
```

**ecran_chapitre_les_responsabilites**
```
3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine look. A birthday cake with candles in a small kitchen, a phone buzzing next to it, a mother's hands. Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. Vertical 9:16 image, the top third is calm sky. No text, no logos.
```

**ecran_chapitre_l_intermediaire**
```
3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine look. A city map spread on a table under a lamp, coffee cups, several phones, hands pointing at districts. Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. Vertical 9:16 image, the top third is calm sky. No text, no logos.
```

**ecran_chapitre_la_direction**
```
3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine look. A man in his early twenties stands alone on the Corniche at night, wind in his jacket, facing the dark sea. Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. Vertical 9:16 image, the top third is calm sky. No text, no logos.
```

# CHOUF : prompts de génération 3D / images et guide Blender

*20 novembre 2026. Document de production pour l'art. Prêt à copier-coller. Complète `01` à `07` (anciens prompts) sans les remplacer. Aucun code. Toutes les personnes, bandes, enseignes et lieux de gestion sont **inventés** ; seuls les quartiers sont réels (voir règles de contenu dans `00-LISEZMOI.md`).*

**Comment s'en servir.** Chaque prompt est en anglais (les générateurs le comprennent mieux), balisé `[STYLE]` (à coller tel quel en tête de chaque prompt) puis la scène. Les mots entre `<…>` sont à remplacer.

---

## A. Bible visuelle courte

**Intention.** Marseille vue de près, sans carte postale ni glorification : une ville belle et dure, où la lumière est magnifique et ce qu'on y voit ne l'est pas. Le joueur regarde un tableau de bord posé sur une ville vivante.

| Rôle | Couleur | Hex indicatif | Usage |
|---|---|---|---|
| Fond | Anthracite | `#14171C` | écrans, ombres, nuit |
| Surface | Bleu pétrole | `#17323B` | mer, cartes, panneaux |
| Neutre | Gris béton | `#6B7480` | bâtiments, routes |
| Texte / clair | Ivoire | `#ECE6D6` | murs calcaire, texte |
| Accent | Jaune chaud | `#E0A93B` | action principale, lumière de lampadaire |
| Alerte douce | Orange rouille | `#C4642A` | rivaux, tension, toits en tuiles |
| Danger | Rouge assourdi | `#9A3B36` | pertes, police, urgence |
| Succès | Vert discret | `#5F8F5A` | gains, territoire à soi |

**Interdits :** fluo, néon saturé, dégradés violets, rose bonbon, vert « hacker ».

**Lumière marseillaise.** Soleil dur et bas, ciel pâle presque blanc à midi, ombres nettes bleu-gris, murs calcaire qui rosissent en fin de journée. Mistral : air limpide, contrastes forts. Nuit : sodium jaune chaud contre bleu pétrole profond.

**Matières.** Calcaire blanc-crème, enduits écaillés, volets pâles, tuiles rouille, béton lessivé, rideaux métalliques, bâches, câbles aériens, linge aux fenêtres, scooters, bacs de recyclage. Mer bleu pétrole. Végétation : pins d'Alep, platanes, figuiers, garrigue sèche.

**Style.** Illustration peinte semi-réaliste, sobre, légèrement désaturée, grain fin. Pour la 3D : low-poly propre avec textures peintes à la main (« hand-painted »), pas de photoréalisme. Cohérent avec les images existantes (`Resources/Art`).

**Chaîne [STYLE] à coller en tête de chaque prompt d'image :**
```
[STYLE] muted hand-painted semi-realistic illustration, Marseille atmosphere, hard low Mediterranean sun, pale limestone walls, petrol blue sea, anthracite shadows, warm yellow accents, rust orange roofs, desaturated palette, fine film grain, no neon, no fluorescent colors, no text, no logos, no brand names, no real people, no graphic violence, adult characters only
```

---

## B. Prompts d'images

### B1. Vues isométriques par zone (carte du jeu)
Format : carré 1:1 ou 4:3, vue isométrique de 35 à 45 degrés, assez dégagée pour poser des pastilles d'interface dessus.

Modèle :
```
[STYLE] isometric 3D diorama view of <ZONE>, Marseille, readable blocks and streets, clear space between buildings for UI markers, soft ambient occlusion, <DETAILS>, daylight, no characters in focus
```

| Zone | `<DETAILS>` |
|---|---|
| Vieux-Port | rectangular harbor basin full of small boats and masts, quays with café terraces, fish market stalls, pale limestone facades, a sloping hill with a basilica silhouette in the far background |
| Joliette | big restored warehouses beside container cranes, modern glass towers next to old docks, wide empty esplanades, ferry terminals, wind-swept |
| Belle de Mai | former tobacco factory complex with large brick halls, railway tracks, dense low housing, murals on gable walls, narrow stepped streets |
| La Plaine | large sloping square with a market, plane trees, bars with terraces, scaffolding on old buildings, laundry on balconies |
| Noailles | tight old streets, spice and fabric shop awnings, crates on pavements, steep stairs, mixed shutters, crowded and colorful but desaturated |
| Endoume | steep seaside neighborhood, small coves, fishermen's cabins, villas with pine trees, stone stairs down to the water |
| Castellane | big roundabout with a fountain monument, tram lines, wide avenues, tall haussmann-style blocks, metro entrance |
| Baumettes | broad coastal plain by the sea cliffs, a large walled administrative compound seen from afar with high plain walls, parking lots, wire-less sober architecture, cold grey light (no detail of security systems) |
| Estaque | fishing village on the northern coast, pastel facades along a small harbor, railway viaduct, hills behind, chichi-fregi stalls |
| Quartiers nord (de loin) | seen from a hillside at dusk, long modern residential blocks on a hill, bare land, motorway ramps, sodium lights coming on, hazy sea beyond, no identifiable housing estate, no named place |

Variantes : ajouter `night, sodium street lights, petrol blue sky` pour la nuit ; `heavy rain, wet reflective streets` pour la pluie ; `mistral day, crisp air, long shadows` pour le vent.

### B2. Douze lieux de gestion
Format 3:2, vue de rue en légère plongée, **jamais de scène d'infraction** : le lieu est montré à vide ou avec des passants de dos, l'ambiguïté vient de l'ambiance.

Modèle :
```
[STYLE] street-level view of <LIEU>, Marseille, slightly high angle, <AMBIANCE>, empty or with two passers-by seen from behind, no visible illegal activity, no weapons, no drugs shown
```

| # | Lieu (inventé) | `<LIEU>` / `<AMBIANCE>` |
|---|---|---|
| 1 | Le charbon (hall d'immeuble) | a worn apartment building entrance hall, flickering tube light, mailboxes, tiled floor, a spiral staircase, a stroller parked, quiet tension |
| 2 | Le charbon (cour) | an inner courtyard between low blocks, rusty benches, a basketball hoop with a torn net, a surveillance camera pointing nowhere, late afternoon |
| 3 | Alimentation de quartier | small neighborhood grocery, crates of fruit outside, handwritten price signs, a lit fridge, closing time |
| 4 | Chicha-lounge | narrow cafe with cushions and low tables, brass lamps, thick smoke haze, closed shutters, warm yellow light |
| 5 | Garage | open garage door, a car on a lift, oil stains, tires stacked, a radio on a shelf, evening |
| 6 | Snack / friterie | street food counter with a fogged window, menu board (no readable text), plastic stools, rain outside |
| 7 | Club | a nightclub exterior at dusk, a plain door with a velvet rope, empty queue barriers, a street cleaner passing |
| 8 | Label / studio | a small music studio room, mixing desk, foam panels, headphones on a chair, empty, red standby light |
| 9 | Parloir | a plain visiting room in a public building, long tables, plastic chairs bolted down, two empty seats facing each other, cold daylight |
| 10 | Local de repli (cave) | a cellar room with stacked cardboard boxes, a bare bulb, pipe shadows, dust in light |
| 11 | Bar de la Plaine | corner bar with zinc counter, mismatched stools, an old television, half-lit morning |
| 12 | Salle de sport de quartier | a boxing and weights gym, worn mats, a heavy bag, steamed windows, dawn light |

### B3. Vingt-quatre portraits (adultes fictifs)
Format 3:4, cadrage poitrine, fond uni sombre ou flou, regard légèrement hors caméra, **adultes de 20 à 65 ans**, éclairage latéral chaud (jaune) contre ombre bleu pétrole. Tenues civiles ordinaires ; **aucune arme, aucun signe distinctif réel**.

Modèle :
```
[STYLE] chest-up portrait of <AGE>-year-old <DESCRIPTION>, <TENUE>, side lighting warm yellow from the left, petrol blue shadow, plain dark background, calm direct expression, painterly, consistent character sheet style, vertical 3:4
```

| # | Rôle / nom d'usage (inventé) | `<AGE>` · `<DESCRIPTION>` · `<TENUE>` |
|---|---|---|
| 1 | Mika, gérante de hall | 34 · woman, tired alert eyes, short dark hair · quilted vest over a plain tee |
| 2 | Nono, vendeur discret | 27 · man, thin face, light stubble · grey hoodie, wired earbuds |
| 3 | Zizou, guetteur | 22 · young man, athletic, shaved head · tracksuit top, sneakers |
| 4 | Sab, logistique | 41 · woman, warm brown skin, tied curly hair · work jacket, lanyard (blank) |
| 5 | Zepek, force | 38 · broad man, trimmed beard · dark bomber jacket |
| 6 | Rascasse, repérage | 30 · wiry man, sun-weathered skin · faded cap, sleeveless fleece |
| 7 | Le Vieux Lucien | 68 · old man, deep wrinkles, white mustache · wool cardigan, flat cap |
| 8 | L'Armateur | 58 · heavy man, calm eyes, grey temples · pressed shirt, no tie |
| 9 | Marie-Ange, tenancière de chicha | 49 · woman, bright scarf, strong jaw · loose blouse, gold hoop earrings |
| 10 | Karim, mécanicien | 44 · man, oil-stained hands, kind face · blue overalls |
| 11 | Léna, comptable d'association | 36 · woman, glasses, neat bun · cardigan, tote bag |
| 12 | Dr Aubanel, médecin de quartier | 52 · man, tired eyes, gray beard · rumpled shirt, stethoscope |
| 13 | Capitaine Roux, police | 47 · woman, short hair, composed · plain dark jacket, no insignia visible |
| 14 | Inspecteur Vidal, police | 55 · man, heavy eyelids, rain-damp coat · trench coat |
| 15 | Soraya, avocate | 39 · woman, sharp gaze · tailored jacket, folder under arm |
| 16 | Tonton Fred, restaurateur | 61 · round man, apron · rolled sleeves, warm smile that doesn't reach his eyes |
| 17 | Dj Ombre, producteur | 29 · man, tired eyes, studio glow · oversized jacket, headphones on neck |
| 18 | Julie, journaliste | 33 · woman, notebook in hand, windswept hair · scarf, camera strap |
| 19 | Édouard, élu local | 56 · man, polished, tired smile · suit, lapel badge blank |
| 20 | Yanis, jeune livreur (adulte) | 21 · slim man, helmet under arm · delivery jacket (no brand), wary look |
| 21 | Nadia, infirmière | 31 · woman, tired, lanyard blank · scrubs under a coat |
| 22 | Mamie Odette, voisine | 74 · old woman, sharp eyes behind curtains · housecoat, shawl |
| 23 | Baptiste, ancien du port | 63 · man, sun-lined skin, cap · navy sweater, hands like rope |
| 24 | La Directrice, administration | 50 · woman, clipped gaze, grey suit · reading glasses on a chain |

*Les noms sont des surnoms de jeu inventés, à vérifier par rapport à tout nom réel avant publication (règle 5 de `00-LISEZMOI.md`).*

### B4. Éléments d'interface (icônes)
Format 512×512, fond transparent, traits épais, 2 couleurs maximum + une touche jaune, lisibles à 64 px.

```
[STYLE] flat minimal icon of <SUJET>, thick rounded outline in ivory, petrol blue fill, single warm yellow accent, transparent background, centered, no text, 512x512
```
`<SUJET>` : `a map with a pin` · `a bar chart` · `two silhouettes` · `two arrows exchanging` · `a flag` · `a notebook` · `a coin stack` · `a shield` · `an eye` · `a phone with signal waves` · `a small shop awning` · `a hookah` (neutre) · `a car key` · `a vinyl record` · `an hourglass`.

---

## C. Prompts pour générateurs 3D (texte ou image vers 3D)

**Contraintes mobiles (cibles) :**
| Élément | Triangles | Texture | Matériaux |
|---|---|---|---|
| Personnage | 3 000 à 5 000 | 1 atlas 1024², couleur seule | 1 |
| Bâtiment modulaire (module de façade) | 300 à 1 200 | atlas partagé 2048² | 1 |
| Prop (scooter, poubelle, caisse, banc) | 100 à 500 | atlas partagé | 1 |
| Îlot de quartier (assemblé) | 8 000 à 15 000 | atlas partagé | 2 |
| Scène complète visible | < 150 000 | — | < 20 |

**Échelle :** 1 unité Unity = 1 mètre ; porte = 2,1 ; étage = 3 ; personnage = 1,75.

Chaîne `[3D-STYLE]` :
```
low-poly stylized game asset, hand-painted albedo only, no baked lighting, no PBR maps, clean topology, quads, flat shading friendly, pale limestone and rust orange palette, mobile game ready, single mesh, centered on origin, facing +Z
```

**Personnage (texte vers 3D) :**
```
[3D-STYLE] full body adult character, A-pose, neutral expression, <ROLE>, <TENUE>, 1.75 m, under 5000 triangles, separate simple hands, no weapon, no logo
```
**Personnage (image vers 3D) :** fournir le portrait B3 + une vue de dos générée avec le même style, puis `convert to a low-poly stylized adult character, keep proportions, A-pose`.

**Bâtiment modulaire :**
```
[3D-STYLE] modular building facade module, Marseille limestone apartment block, 3 floors, shutters, wrought iron balconies, rust orange terracotta roof, width 6 m, depth 8 m, tileable on X axis, no interior, under 1200 triangles
```
Variantes : `ground floor with metal shutter shop`, `corner module`, `rooftop with water tanks and antennas`, `warehouse hall with sawtooth roof`.

**Props (liste à générer par lot) :** scooter garé, rideau métallique baissé, conteneur de tri, banc, platane (low-poly), lampadaire sodium, camionnette de livraison blanche (sans marque), poubelle, parasol de terrasse, bateau de pêche (pointu), grue de port, clôture de chantier, étal de marché, échelle de linge.

**Carte 3D de quartier :** générer d'abord l'îlot (3 à 6 modules + 6 props), puis assembler dans Blender (voir D) plutôt que de demander une ville entière : les générateurs produisent un maillage unique, lourd et non modifiable.

---

## D. Guide Blender pas à pas

Version cible : Blender 4.x. Cible Unity : URP (2022.3 LTS ou 6.x).

### D1. Pipeline de la carte (blockout)
1. **Plan de référence** : importer `map_fond.jpg` (ou un plan de la zone) en image de référence à plat, échelle 1 px = 1 m ; verrouiller.
2. **Grille** : cases de 10 m, centrées sur l'origine. 1 quartier du jeu = 1 collection `QUARTIER_<id>`.
3. **Blockout** : cubes ajustés sur les emprises des îlots, hauteur par étages (3 m). Rues = plans à −0,1 m.
4. **Validation de la silhouette** : caméra isométrique (rotation X 55°, Z 45°, orthographique) ; vérifier que la zone se lit à 1080 px de large.
5. **Remplacement** par les modules de la section C (instances liées, pas de copies : `Alt+D`).
6. **Passer en 2 niveaux de détail** : proche (jouable) et lointain (cube texturé).

### D2. Modèles modulaires
- Un module = un objet, origine au coin bas-arrière gauche, face à +Y dans Blender.
- Pas de trous cachés ; pas de faces internes ; normales vers l'extérieur (`Shift+N`).
- Modificateurs appliqués avant l'export (`Ctrl+A` → tout), sauf Mirror conservé en édition.

### D3. Atlas de textures
- Un atlas partagé 2048² par famille (façades, sols, props, végétation). Marge de 8 px entre îlots UV.
- Peinture : couleur de base seulement ; pas de lumière cuite (Unity éclaire). Nuances de calcaire, rouille, métal, bâche.
- Nom : `ATLAS_facades_v01.png`. Format sRGB, pas de compression avant l'import Unity.

### D4. Conventions de nommage
| Type | Modèle | Exemple |
|---|---|---|
| Collection | `QUARTIER_<id>` / `PROPS` / `PERSOS` | `QUARTIER_017` |
| Module | `MOD_<famille>_<variante>` | `MOD_facade_balcon_A` |
| Prop | `PROP_<nom>` | `PROP_scooter_01` |
| Personnage | `PERSO_<surnom>` | `PERSO_mika` |
| Matériau | `MAT_<atlas>` | `MAT_facades` |
| LOD | `<nom>_LOD0/1/2` | `MOD_facade_balcon_A_LOD1` |

Tout en minuscules sauf préfixes ; pas d'espaces ni d'accents.

### D5. Export vers Unity
- **Format** : glTF 2.0 binaire (`.glb`) pour les décors ; FBX accepté pour les personnages squelettés.
- **Échelle** : unités = mètres ; dans l'export, `Apply Transform` coché.
- **Axes** : FBX → `-Z Forward`, `Y Up` avec « Apply Transform » ; glTF → `+Y Up` (par défaut).
- **Matériaux** : un matériau par atlas, nommé `MAT_*`. Dans Unity, créer des matériaux **URP/Lit** (ou Unlit pour les décors lointains) et les réassigner ; ne pas importer les matériaux Blender tels quels.
- **Dossier cible** : `unity/Assets/Art3D/<type>/` (à créer) ; réimporter sans toucher aux scripts.

### D6. LOD
- LOD0 : 100 %, LOD1 : 50 %, LOD2 : 20 % (modificateur Decimate, ratio 0,5 puis 0,2).
- Distances Unity indicatives : LOD0 < 25 m, LOD1 < 60 m, LOD2 au-delà, puis cube.
- Ne décimer ni les personnages ni les props de moins de 500 triangles.

### D7. Script Python d'export par lots (Blender)
Script simple, **sans réseau, sans exécution de commandes** : il exporte chaque collection de premier niveau en `.glb`. À lancer depuis l'onglet *Scripting* de Blender, après avoir enregistré le fichier `.blend`.

```python
import bpy
import os

# Dossier d'export : à côté du fichier .blend, sous-dossier "export"
base = os.path.dirname(bpy.data.filepath)
out_dir = os.path.join(base, "export")
os.makedirs(out_dir, exist_ok=True)

def export_collection(col):
    # Sélectionner uniquement les objets de la collection
    bpy.ops.object.select_all(action='DESELECT')
    objets = [o for o in col.all_objects if o.type == 'MESH']
    if not objets:
        return
    for o in objets:
        o.select_set(True)
    bpy.context.view_layer.objects.active = objets[0]
    chemin = os.path.join(out_dir, col.name + ".glb")
    bpy.ops.export_scene.gltf(
        filepath=chemin,
        export_format='GLB',
        use_selection=True,
        export_apply=True,        # applique les modificateurs
        export_yup=True,
    )
    print("OK :", chemin)

for col in bpy.context.scene.collection.children:
    export_collection(col)
```
Limites : ne gère pas les collections imbriquées sous leur nom propre ; vérifier le résultat dans Unity avant tout import massif.

---

## E. Checklist de cohérence et négatifs

**À vérifier sur chaque image / modèle :**
- [ ] Palette : aucun fluo, jaune chaud en accent seulement.
- [ ] Lumière : soleil bas et dur ou sodium nocturne, jamais de lumière « studio ».
- [ ] Aucun texte lisible, aucune enseigne ni logo, aucune marque réelle (voiture, boisson, vêtement).
- [ ] Aucune personne réelle ni sosie volontaire d'un acteur, sportif ou élu.
- [ ] Tous les personnages sont **adultes** (20 ans et plus à l'écran).
- [ ] Aucune violence montrée, aucun corps, aucune blessure, aucune scène d'infraction en cours.
- [ ] Aucune arme détaillée ; pas de substance identifiable ; pas de matériel de laboratoire.
- [ ] Aucun lieu réel nommé comme repaire ; les quartiers sont des ambiances, les lieux de gestion des décors génériques.
- [ ] La ville reste belle : le malaise vient du contexte, pas de la laideur.
- [ ] Même grain et même saturation que `Resources/Art`.

**Négatifs à ajouter à chaque prompt (si le générateur les accepte) :**
```
neon, fluorescent, oversaturated, cyberpunk, purple gradient, text, watermark, logo, brand name, real person, celebrity, child, minor, teenager, weapon, gun, knife, blood, gore, corpse, drugs, syringe, money piles, cartoon, anime, photorealistic skin pores, lens flare
```

**Rangement :** images dans `unity/Assets/Resources/Art/<dossier>/` selon les dossiers existants (`portraits`, `screens`, `types`…) ; fichiers 3D dans `unity/Assets/Art3D/`. Nommage des images : `portrait_<surnom>.jpg`, `lieu_<nom>.jpg`, `iso_<zone>.jpg`.

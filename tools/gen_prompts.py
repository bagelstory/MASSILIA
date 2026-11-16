"""Génère design/04-prompts-3d.md : un bloc = une image, chaque bloc complet (style compris), prêt à coller.
Les personnages viennent de data/personnages.json ; leur apparence est décrite ici (APPARENCE).
Règles d'écriture des prompts (pour passer les filtres de Gemini et ressembler aux références) :
- jamais de mots de crime ou de liste d'interdits (« no weapons, no drugs » déclenche les filtres) ;
- les mineurs n'apparaissent que dans des scènes de vie ordinaires, jamais avec police, prison ou peur ;
- la ville en style « maquette 3D grise » comme les références (vue stratégique), pas en réalisme sale ;
- une image par bloc ; seules les icônes sont en grille 3 × 3 (ronds sur fond noir, découpés en cercle).
Usage : python3 tools/gen_prompts.py"""
import json, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
P = json.load(open(os.path.join(ROOT, "data", "personnages.json"), encoding="utf-8"))
Z = json.load(open(os.path.join(ROOT, "data", "zones.json"), encoding="utf-8"))

VILLE = ("3D render, strategy game city view. A miniature architectural model of {lieu}, made of matte light-grey "
         "clay buildings with simple shapes and almost no texture, like a board-game city or an urban planning model. "
         "High three-quarter aerial camera, tilt-shift blur at the top and bottom edges. Pale grey streets with thin "
         "white lines, dark blue-grey sea, soft evening light from the left, long soft shadows, subtle ambient occlusion, "
         "dark vignette around the edges. Calm and desaturated, almost monochrome, small warm lights in a few windows. "
         "{detail} Wide 16:9 image. No people, no text, no logos.")

PORTRAIT = ("3D render of a video game character portrait, realistic modern console game style. {qui} "
            "Head and shoulders, centered, looking slightly to the left of the camera, calm natural expression. "
            "Soft studio key light from the left, gentle blue rim light, plain dark charcoal-grey background with a "
            "soft vignette. Realistic skin, hair and fabric, natural proportions, dignified, ordinary person from "
            "Marseille. Square 1:1 image, face in the middle third. No text, no logos.")

SCENE = ("3D render, cinematic still from a modern narrative video game set in Marseille, realistic game-engine look. "
         "{quoi} Muted, desaturated colors with warm streetlight orange and cool blue shadows, soft volumetric light, "
         "shallow depth of field, people shown at a distance or from behind, sober and humane mood. "
         "Wide 16:9 image. No text, no logos.")

ICONES = ("3D render of a sheet of 9 round game map badges arranged in a 3 by 3 grid with equal spacing, on a plain "
          "pure black background. Each badge: a glossy dark charcoal disc with a thick beveled gold metal ring and one "
          "simple bold white embossed symbol in the middle, soft highlight on top, seen straight from the front, "
          "all badges the same size. Symbols, left to right then top to bottom: {liste}. No text, no letters, no numbers.")

ECRAN = ("3D render, cinematic key art from a modern narrative video game set in Marseille, realistic game-engine "
         "look. {quoi} Muted desaturated palette, warm sodium streetlights, cool blue night, soft volumetric haze. "
         "Vertical 9:16 image, the top third is calm sky. No text, no logos.")

# ---------- apparence des personnages (âge 2021) ----------
FAM = {
    "ferrara": ("a French-Italian family from Marseille",
                {"mere": "a 43-year-old woman, dark blond hair held in a clip, small gold cross necklace, supermarket uniform polo, tired but warm eyes",
                 "petit_frere": "an 11-year-old boy, light olive skin, messy brown hair, plain sky-blue t-shirt, curious smile",
                 "grande_soeur": "a 20-year-old woman, long dark brown hair, small hoop earrings, denim jacket, sharp lucid gaze"}),
    "mmadi": ("a French family of Comorian heritage",
              {"mere": "a 42-year-old woman, dark skin, colorful patterned headscarf, nursing assistant tunic, calm and proud",
               "petit_frere": "an 11-year-old boy, dark skin, short hair, grey hoodie, big admiring eyes",
               "grande_soeur": "a 20-year-old woman, dark skin, braids tied up, beige trench coat, straight serious look"}),
    "benali": ("a French family of Algerian heritage",
               {"mere": "a 44-year-old woman, dark curly hair with grey strands, simple cardigan, strong worried face",
                "petit_frere": "an 11-year-old boy, tan skin, black hair short on the sides, plain red t-shirt, mischievous smile",
                "grande_soeur": "a 20-year-old woman, dark hair in a ponytail, thin glasses, direct look"}),
    "sarkissian": ("a French family of Armenian heritage",
                   {"mere": "a 45-year-old woman, dark hair in a bob, elegant navy cardigan, anxious eyes",
                    "petit_frere": "an 11-year-old boy, pale olive skin, thick dark hair, green sweater, curious",
                    "grande_soeur": "a 20-year-old woman, long black hair, strong eyebrows, black turtleneck, determined"}),
    "diallo": ("a French family of Senegalese heritage",
               {"mere": "a 41-year-old woman, dark skin, wax-print headwrap, canteen apron, broad warm smile, tired eyes",
                "petit_frere": "an 11-year-old boy, dark skin, short hair, plain sky-blue t-shirt, laughing",
                "grande_soeur": "a 20-year-old woman, dark skin, short natural hair, small nose ring, lucid look"}),
    "garcia": ("a French Gitan family settled in Marseille for generations",
               {"mere": "a 40-year-old woman, long black hair, gold earrings, dark blouse, strong protective expression",
                "petit_frere": "an 11-year-old boy, tan skin, black curls, white t-shirt, cheeky smile",
                "grande_soeur": "a 20-year-old woman, black hair in a long braid, serious and kind"}),
}
APPARENCE = {
    "tante": "A 48-year-old hospital nurse, short auburn hair, light blue scrubs under a cardigan, laughing eyes.",
    "ami_enfance": "A 16-year-old boy, fair skin with freckles, short brown hair, plain sky-blue and white football shirt with no badge, friendly grin.",
    "camarade": "A 16-year-old girl, light skin, brown hair in a high bun, school backpack strap on the shoulder, ironic half smile.",
    "pitchoun": "A 15-year-old boy, very tall and broad for his age, dark skin, short hair, track jacket, gentle eyes.",
    "ines": "A 16-year-old girl, tan skin, black hair in a tight ponytail, sports team jacket, serious look.",
    "nono": "A 19-year-old young man, tan skin, short beard, delivery rider jacket, a scooter helmet under his arm, big smile.",
    "chouchou": "A 17-year-old boy of mixed heritage, curly hair, headphones around the neck, dreamy eyes.",
    "coach": "A 50-year-old football coach, bald head, grey goatee, club tracksuit, whistle on a cord, strict but warm.",
    "educatrice": "A 38-year-old social worker of Corsican heritage, dark hair with a pencil stuck in it, tired determined eyes.",
    "snack": "A 52-year-old snack bar owner, short grey hair, apron over a polo shirt, warm but wary look.",
    "voisine": "A 71-year-old woman, white curly hair, glasses on a chain, floral blouse, brave smile.",
    "rose_marie": "A 62-year-old woman of Portuguese heritage, short dyed dark hair, holding a clipboard against her chest, stubborn look.",
    "farid": "A 47-year-old building caretaker, moustache, navy work jacket, observant eyes.",
    "madame_lan": "A 55-year-old Vietnamese-French shopkeeper, hair in a low bun, cardigan, calm knowing look.",
    "pastis": "A 68-year-old retired docker from Provence, weathered tanned face, flat cap, old work jacket, grumpy.",
    "babou": "A 56-year-old man, grey stubble, deep lines on his face, beige cardigan, calm sad eyes.",
    "grand_hall": "A 24-year-old man, neat short beard, plain dark puffer jacket, calm calculating gaze, almost a smile.",
    "souris": "A 14-year-old boy, small and thin, pale skin, oversized grey hoodie, alert eyes.",
    "rascasse": "A 31-year-old man, shaved head, tense jaw, plain black jacket, suspicious look.",
    "tata": "A 58-year-old woman, greying hair, house cardigan, exhausted gentle face.",
    "mika": "A 28-year-old man, short hair, mechanic's work jacket, calm face.",
    "zizou": "A 20-year-old man, fade haircut, plain navy tracksuit, impatient look.",
    "bebe": "A 17-year-old boy, round young face, plain cap, nervous smile.",
    "sab": "A 26-year-old woman, sleek dark hair, grey blazer, minimal makeup, precise cool look.",
    "prof": "A 46-year-old high school teacher, short grey-blond hair, scarf, reading glasses pushed up on her head, fair and strict.",
    "cpe": "A 44-year-old school counselor, red bob haircut, lanyard with a blank badge, energetic.",
    "commandante": "A 49-year-old woman detective in plain clothes, dark hair with grey, beige trench coat, patient stubborn gaze.",
    "brigadier": "A 33-year-old police officer in a plain dark navy uniform, short blond hair, tired hard face.",
    "belkacem": "A 38-year-old detective in plain clothes, open shirt collar, short dark hair, attentive humane eyes.",
    "avocate": "A 40-year-old French lawyer in a black court robe with white bands, dark hair pulled back, sharp eyes.",
    "juge": "A 52-year-old judge, short white hair, simple dark jacket, attentive and firm.",
    "journaliste": "A 29-year-old journalist, curly brown hair, notebook in hand, curious wary eyes.",
    "elu": "A 57-year-old local politician, grey hair, suit without tie, affable public smile.",
}
CHEFS = {
    "compagnie": "A 34-year-old businessman seen in the back seat of a dark car, expensive plain coat, half of his face in shadow, calm distant look.",
    "gabians": "A 27-year-old man, hard face, plain grey tracksuit, cold eyes.",
    "ruisseau": "A 45-year-old heavy-set man, grey hair, friendly uncle face that does not quite reassure.",
    "tramontane": "A 31-year-old elegant woman, glasses, black blazer, lit by the warm lights of a night bar.",
    "calanquais": "A 22-year-old young man, sunburnt skin, bleached hair, too-big smile, impatient eyes.",
}

# ---------- zones : ce qu'on doit reconnaître ----------
ZDET = {
    "centre_hist": ("the Vieux-Port of Marseille", "The rectangular old harbor full of small white boats, the large flat mirrored canopy on the quay, dense 4–6 storey buildings around it, the hill of Notre-Dame de la Garde with its basilica and bell tower in the background, the old Panier district climbing on the right."),
    "centre_nord": ("the Belle de Mai district of Marseille", "Tight blocks of tall 19th-century buildings, a large old tobacco-factory complex with brick chimneys turned cultural venue, a railway yard on one side."),
    "plaine": ("the Cours Julien and La Plaine district of Marseille", "A long pedestrian terraced street with stairs and trees, a large rectangular square lined with plane trees, small dense buildings around."),
    "prado": ("the Castellane and Prado district of Marseille", "A big roundabout with a monumental fountain in the middle, a long straight tree-lined avenue running toward the sea, an oval football stadium with a wavy white roof further along."),
    "nord_est": ("the north-eastern districts of Marseille", "Large 1970s housing towers and long slab buildings on green hillsides, a small football pitch, an elevated metro line crossing the scene, pine hills behind."),
    "nord_central": ("the 14th district of Marseille", "A mix of small houses with tiled roofs and long housing slabs, a large flat shopping-center roof, a busy road and a big parking lot."),
    "nord_ouest": ("the north-western hills of Marseille", "Tall housing towers on hills above the sea, the container port with cranes and docks far below, a large shopping mall in the valley, a hospital on a hill."),
    "littoral_nord": ("L'Estaque in Marseille", "A small fishing harbor with little wooden boats, old village houses climbing the hill, a stone railway viaduct, the big container port and its cranes on the horizon."),
    "est": ("the eastern districts of Marseille", "Old village centers with small squares and church bell towers, low houses with tiled roofs, white limestone hills behind, a big suburban shopping center in the distance."),
    "sud": ("the southern seafront of Marseille", "A long beach with a seaside promenade, a park with palm trees and a small château, the white limestone cliffs of the calanques at the end of the bay."),
}
EXTRA_Z = [("baumettes", "a large modern correctional facility at the foot of white limestone hills in the south of Marseille", "High plain grey walls, rectangular buildings, a gate and a small visitor parking lot with a bench, quiet and sober."),
           ("panorama", "the whole city of Marseille seen from far above", "The old harbor in the middle, the cathedral by the sea, the basilica on its hill, the islands offshore, the northern hills far away.")]

SCENES = [
    ("le_quotidien", "Early morning in a small family kitchen in a housing block: a mother putting on her coat at the door, a boy searching for his school bag, a teenager by the window looking down at the street."),
    ("les_visages", "The entrance hall of a housing block at dusk: a young man in his twenties leaning on the mailboxes waves to a teenager at the door to come over."),
    ("la_promesse", "A mother and her teenage son sitting at a small kitchen table at night, a school report card on the table, his phone lighting up face down."),
    ("promesse_oubliee", "A woman waiting alone at the foot of a building at night under a streetlight, arms crossed, watching someone arrive late."),
    ("rancune", "In the courtyard of a housing estate, a group of teenagers turn their backs on one of them who stands alone, hands in his pockets."),
    ("essai_au_club", "A small municipal football pitch on a Saturday morning, a coach with a whistle, two teenagers in plain shirts, pine hills and housing blocks behind."),
    ("soir_de_match", "A neighborhood snack bar at night full of people watching a football match on a TV that shows only a green pitch, plastic chairs on the pavement, sky-blue scarves, joy and tension."),
    ("mistral", "A street in Marseille in a violent mistral wind: shutters banging, plane trees bending, a woman holding her hair, white clouds racing over a deep blue sky."),
    ("souris_dans_le_froid", "Night in an empty housing estate: a young teenager in an oversized hoodie sits alone on a low wall under a streetlight, looking at the street. Cold blue light, loneliness."),
    ("babou_raconte", "Older men playing pétanque in front of a housing block at sunset, one of them talking to a teenager who listens."),
    ("le_son_de_chouchou", "A tiny night grocery store: a teenager plays music from his phone on a small speaker on the counter, the shopkeeper raises an eyebrow, a friend listens."),
    ("l_ascenseur", "A building entrance hall with an elevator out of order, a retired woman holding a clipboard petition, young men leaning on the opposite wall, the caretaker looking at his shoes."),
    ("la_descente", "Dawn in the stairwell of a housing block: several police officers in dark uniforms climb the stairs quietly; on the landing a woman in a dressing gown holds her door half open, worried. Blue light through the window."),
    ("le_lieutenant", "In front of a high school gate, a man in plain clothes talks calmly with a young man carrying a backpack, students walking past."),
    ("les_absents", "A silent white march through a neighborhood: families walking slowly with white balloons and white roses, a woman at the front holding a framed photo turned away from the viewer."),
    ("controle", "Night at a bus stop: two police officers speak with three young men in their twenties against a wall, a bus passing with its lights on."),
    ("garde_a_vue", "A bare waiting room in a police station at night: a young man about twenty sits alone on a bench, head in his hands, harsh white light, a closed door."),
    ("convocation", "A mother and her son sitting side by side on plastic chairs in a courthouse corridor, waiting, the mother holding a folder."),
    ("parloir", "The visiting room of a correctional facility: a mother sits across a small table from her adult son in a grey sweatshirt, other families at other tables."),
    ("cellule", "A small shared room in a correctional facility: bunk beds, a barred window with a sliver of blue sky, a family photo taped to the wall."),
    ("sortie_de_prison", "A young man walks out of the gate of a correctional facility with a plastic bag, white hills behind him, a small figure waiting far away."),
    ("enterrement", "A Marseille cemetery on a sunny windy day: a crowd in dark clothes among cypress trees, a mother supported by two women, young men standing apart."),
    ("mur_des_noms", "The wall of a housing block with flowers, candles and photos at its foot, children passing by on bikes."),
    ("mere_qui_attend", "A mother alone at a kitchen window at night, phone in hand, waiting, the city lights below."),
    ("facade_snack", "A small newly renovated snack bar, a teenager behind the counter, an accountant with a laptop at a table, everything too clean and too new."),
    ("salle_de_boxe", "A neighborhood boxing gym: kids training on bags, a coach, a man in a jacket watching from the office window."),
    ("reseau_international", "A large container port at night seen from above, cranes, a cargo ship leaving, lights on the sea."),
    ("operation_police", "Dawn around a housing estate: many police vans with blue lights, a helicopter light in the sky, residents watching from their balconies."),
    ("mairie", "An inauguration on a sunny square: a local official cutting a ribbon, some residents clapping, others with arms crossed, a renovated playground."),
    ("journaliste", "A young journalist interviews a mother on a bench at the foot of a building, notebook open, the mother looking away."),
    ("depart_train", "The monumental staircase of Marseille's Saint-Charles railway station at dawn, a young man with a sports bag walking down alone toward the city."),
    ("nouvelle_vie", "A small workshop in daylight, a young man in work clothes laughing with a coworker."),
    ("petit_frere_grandi", "Two brothers sitting on the rocks of the Corniche at sunset, seen from behind, the sea and islands ahead."),
    ("chef", "A man seen from behind on a rooftop terrace above Marseille at night, phone glowing in his hand, the city lights below, completely alone."),
    ("retour_quartier", "A man around forty walks back into his old housing estate, kids playing football, an old woman at her window recognizing him."),
    ("table_de_famille", "A family meal on a small balcony in summer, laughter, two chairs empty."),
]
ICON = [
    ("I01 — ressources", ["a lightning bolt", "a snowflake", "a folded banknote", "a bundle of banknotes with a rubber band", "a police siren light", "cracked ground", "a house with a heart", "a laurel wreath", "an open hand sending ripples"]),
    ("I02 — état", ["a looming shadow", "two hands shaking", "a head with tangled lines", "a heart with a pulse line", "a school backpack", "two hooked little fingers", "an open notebook", "a folded map with a pin", "three heads"]),
    ("I03 — interface", ["a smartphone with a speech bubble", "an open book with a bookmark", "a blank calendar page", "a cloud with an arrow", "a warning triangle", "an apartment block", "a key", "an eye", "an open hand"]),
    ("I04 — rôles et commerce", ["a closed door", "a steering wheel", "a hand on a shoulder", "a pair of lawyer bands", "a calculator", "a small closed cardboard box", "three closed cardboard boxes", "a pallet of boxes under a tarp", "a cone of fries"]),
    ("I05 — commerces et réseaux", ["a boxing glove", "a plate with fork and knife", "a building with a key", "sand dunes and a lighthouse", "canal houses and a crane", "a magnifying glass over a footprint", "a barred window", "a white rose", "a stack of coins"]),
    ("I06 — ville", ["a football", "wind gust lines", "a sun", "a cloud with rain", "string lights", "a fruit crate", "the front of a bus", "the front of a metro train", "a beach umbrella"]),
    ("I07 — Marseille", ["a hilltop basilica with a tall bell tower", "golden fried chickpea sticks", "a tall glass of cloudy yellow drink with a water jug", "three pétanque balls and a small jack", "a small pointed fishing boat", "a seagull", "a cicada", "a closed school gate", "a flame on a hill"]),
    ("I08 — événements", ["a door with a boot print", "a white balloon", "an ear with sound waves", "an envelope", "a clock", "an open door with light", "a chess rook", "a wave", "a curl of wind"]),
]
ECRANS = [
    ("ecran_accueil", "Marseille at night seen from the roof of a housing block on the northern hills: a teenager sits on the edge with his back to the viewer, the basilica lit on its hill in the distance, the port and the sea."),
    ("ecran_fin", "The same rooftop above Marseille at dawn, empty, a hoodie left on the edge, the city waking up."),
    ("ecran_chapitre_la_confiance", "Close-up of one hand passing a set of keys to another hand in a dim stairwell."),
    ("ecran_chapitre_les_responsabilites", "A birthday cake with candles in a small kitchen, a phone buzzing next to it, a mother's hands."),
    ("ecran_chapitre_l_intermediaire", "A city map spread on a table under a lamp, coffee cups, several phones, hands pointing at districts."),
    ("ecran_chapitre_la_direction", "A man in his early twenties stands alone on the Corniche at night, wind in his jacket, facing the dark sea."),
]


def bloc(cle, texte):
    return f"**{cle}**\n```\n{texte}\n```\n"


out = []
w = out.append
w("# CHOUF — prompts Gemini 3D (version 2, remplace le document 03)\n")
w("Généré par `tools/gen_prompts.py` à partir des données du jeu. **Un bloc = une image.** Chaque bloc est complet : "
  "copie-le tel quel dans Gemini, sans rien ajouter. Le nom en gras est le nom du fichier à m'envoyer "
  "(`banniere_sud.png`, `medaillon_babou.png`…), à déposer dans `assets/brut/`.\n")
w("## Pourquoi cette version\n")
w("- **Filtres de Gemini** : la version 03 contenait des mots qui bloquent (crime, armes, drogue, même dans « sans armes, "
  "sans drogue »), des mineurs dans des scènes de police ou de peur, des blessures. Ici : aucun de ces mots, les enfants "
  "et ados n'apparaissent que dans la vie ordinaire, les scènes de police, de justice et de prison ne montrent que des adultes.\n"
  "- **Ressemblance avec tes références** : la ville est rendue comme une **maquette 3D grise** vue d'en haut (bâtiments "
  "lisses, presque sans texture, flou de maquette), comme la vue de ville de tes captures. Les couleurs des équipes sont "
  "ajoutées par le jeu, pas par Gemini.\n"
  "- **Fiabilité** : plus de grandes planches de 6 ou 8 personnages (Gemini ne respecte pas les cases). Une image par "
  "personnage, sur fond gris foncé : le jeu les recadre en rond, pas besoin de fond magenta. Les icônes restent par 9 "
  "(3 × 3) sur fond noir, faciles à découper.\n")
w("## Si Gemini refuse ou se trompe\n")
w("- Refus : relance une fois tel quel (les refus sont parfois aléatoires). Sinon, enlève la phrase qui décrit l'émotion "
  "ou l'action et garde le lieu. Dis-moi lequel : je réécris le bloc.\n"
  "- Pas assez 3D : ajoute au début « 3D render, game engine screenshot, not a drawing, not a painting ».\n"
  "- Pour garder le même visage (famille, protagoniste à plusieurs âges) : joins l'image déjà faite et ajoute à la fin "
  "« Same person as in the attached image. »\n"
  "- Format : Gemini choisit parfois un carré. Redemande « wide 16:9 » ou recadre, le jeu accepte.\n")
w("## Inventaire\n")
nz = len(ZDET) + len(EXTRA_Z)
nfam = sum(len(f[1]) for f in FAM.values())
w(f"| Série | Images |\n|---|---|\n| Z — vues de ville 3D | {nz} |\n| F — familles | {nfam} |\n| P — personnages | {len(APPARENCE)} |\n"
  f"| R — chefs des équipes | {len(CHEFS)} |\n| H — protagoniste (6 familles × 3 âges) | 18 |\n| S — scènes | {len(SCENES)} |\n"
  f"| I — icônes (planches de 9) | {len(ICON)} planches = {9*len(ICON)} icônes |\n| E — écrans 9:16 | {len(ECRANS)} |\n")

w("\n---\n\n# Z — VUES DE VILLE 3D (bannières des zones)\n")
w("Le jeu fait lui-même la version de nuit et la teinte de l'équipe qui tient la zone.\n")
for z in Z["liste"]:
    lieu, det = ZDET[z["id"]]
    w(bloc("banniere_" + z["id"], VILLE.format(lieu=lieu, detail=det)))
for k, lieu, det in EXTRA_Z:
    w(bloc("banniere_" + k, VILLE.format(lieu=lieu, detail=det)))

w("\n---\n\n# F — FAMILLES\n")
w("Fais les trois personnes d'une même famille dans la même conversation Gemini, en joignant la mère aux deux suivants "
  "(« Same family as the attached image, same features »).\n")
for fid, (orig, mem) in FAM.items():
    w(f"\n### Famille {fid.capitalize()}\n")
    for k, d in mem.items():
        w(bloc(f"medaillon_{k}_{fid}", PORTRAIT.format(qui=f"A member of {orig}: {d}.")))

w("\n---\n\n# P — PERSONNAGES\n")
noms = {p["id"]: p for p in P["liste"]}
for pid, d in APPARENCE.items():
    p = noms[pid]
    titre = p["prenom"] + (" « " + p["surnom"] + " »" if p.get("surnom") else "")
    w(f"{titre}\n")
    w(bloc("medaillon_" + pid, PORTRAIT.format(qui=d)))

w("\n---\n\n# R — CHEFS DES ÉQUIPES (inventés)\n")
for eid, d in CHEFS.items():
    w(bloc("chef_" + eid, PORTRAIT.format(qui=d)))

w("\n---\n\n# H — LE PROTAGONISTE\n")
w("Pour chaque famille : fais d'abord **16 ans** en joignant l'image de la mère de la famille (« He is her son, same "
  "family features. »), puis 21 et 40 ans en joignant l'image de 16 ans (« Same person as in the attached image, "
  "older. »). La version fatiguée ou abîmée est faite par le jeu (teinte, ombres).\n")
AGES = [("jeune", "a 16-year-old teenager, grey hoodie"), ("adulte", "a 21-year-old young man, short beard, plain dark jacket"),
        ("ancien", "a 40-year-old man, lines on the forehead, a few grey hairs, plain shirt")]
for fid, (orig, _) in FAM.items():
    w(f"\n### {fid.capitalize()}\n")
    for a, d in AGES:
        w(bloc(f"joueur_{fid}_{a}", PORTRAIT.format(qui=f"A young man from {orig}: {d}.")))

w("\n---\n\n# S — SCÈNES\n")
for k, d in SCENES:
    w(bloc("scene_" + k, SCENE.format(quoi=d)))

w("\n---\n\n# I — ICÔNES (3 × 3 sur fond noir)\n")
w("Le jeu découpe chaque rond. Donne-moi la planche entière.\n")
for t, l in ICON:
    w(f"\n{t}\n")
    w(bloc("icones_" + t.split(" ")[0].lower(), ICONES.format(liste="; ".join(l))))

w("\n---\n\n# E — ÉCRANS (vertical 9:16)\n")
for k, d in ECRANS:
    w(bloc(k, ECRAN.format(quoi=d)))

open(os.path.join(ROOT, "design", "04-prompts-3d.md"), "w", encoding="utf-8").write("\n".join(out))
print("design/04-prompts-3d.md écrit")

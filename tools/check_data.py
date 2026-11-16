"""Vérifie les données de CHOUF v2 : JSON valide, identifiants uniques, références (zones, factions, tâches,
types d'opérateurs, objectifs), bornes, et absence de noms réels interdits dans data/ et src/.
Usage : python3 tools/check_data.py   (code de sortie 1 si erreur)"""
import glob, json, os, re, sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
err, warn = [], []
D = {}
for p in sorted(glob.glob(os.path.join(ROOT, "data", "*.json"))):
    n = os.path.basename(p)[:-5]
    try:
        D[n] = json.load(open(p, encoding="utf-8"))
    except Exception as e:
        err.append(f"{n}.json : {e}")
if err:
    print("\n".join(err)); sys.exit(1)

B = D["equilibrage"]
Z = {z["id"]: z for z in D["zones"]["liste"]}
E = {e["id"]: e for e in D["equipes"]["liste"]}
Q = D["quartiers"]["liste"]
ids = [q["id"] for q in Q]
if len(ids) != len(set(ids)): err.append("quartiers : identifiants en double")
if len(Q) != 111: warn.append(f"quartiers : {len(Q)} au lieu de 111")
for q in Q:
    if q["zone"] not in Z: err.append(f"quartier {q['nom']} : zone inconnue {q['zone']}")
    if not q["polys"]: err.append(f"quartier {q['nom']} : pas de contour")
for z in Z.values():
    if z["controle"] is not None and z["controle"] not in E: err.append(f"zone {z['id']} : faction inconnue")
    if not any(q["zone"] == z["id"] for q in Q): err.append(f"zone {z['id']} : aucun quartier")
    for k in ("tension", "police", "humeur"):
        if not 0 <= z[k] <= 100: err.append(f"zone {z['id']} : {k} hors bornes")
if not any(z["depart"] for z in Z.values()): err.append("aucune zone de départ")
for e in E.values():
    if not 1 <= e["agressivite"] <= 5: err.append(f"faction {e['id']} : agressivité hors 1-5")
T = B["operateurs"]["types"]
for k, t in T.items():
    if t["tache"] not in B["taches"]: err.append(f"type {k} : tâche inconnue {t['tache']}")
for k, t in B["taches"].items():
    if t["type"] is not None and t["type"] not in T: err.append(f"tâche {k} : type inconnu {t['type']}")
for t in B["depart"]["operateurs"]:
    if t not in T: err.append(f"départ : type inconnu {t}")
for v in B["operateurs"]["visages"]:
    if v not in {p["id"] for p in D["personnages"]["liste"]}: err.append(f"visage inconnu {v}")
TYPES_OBJ = {"quartiers_tenus", "connaissance_zone", "connaissance", "charbons", "tresorerie", "securises", "operateurs", "territoire", "flux"}
for q in [D["quetes"]["ouverture_zone"]] + D["quetes"]["liste"]:
    for o in q["objectifs"]:
        if o["type"] not in TYPES_OBJ: err.append(f"objectif {q.get('id', 'zone')} : type inconnu {o['type']}")
for s in B["evenements"]["saisons"]:
    if s["zones"] != "*" and any(z not in Z for z in s["zones"]): err.append(f"saison {s['id']} : zone inconnue")

# noms réels interdits (dossier de recherche) : jamais dans le jeu
banned = json.load(open(os.path.join(ROOT, "tests", "lint", "interdits.json"), encoding="utf-8"))["noms"]
pat = re.compile(r"(?<![\w])(" + "|".join(re.escape(b) for b in banned) + r")(?![\w])")
for p in glob.glob(os.path.join(ROOT, "data", "*.json")) + glob.glob(os.path.join(ROOT, "src", "**", "*.*"), recursive=True):
    if p.endswith((".png", ".webp", ".jpg")): continue
    for i, line in enumerate(open(p, encoding="utf-8", errors="ignore"), 1):
        m = pat.search(line)
        if m: err.append(f"nom réel interdit « {m.group(1)} » : {os.path.relpath(p, ROOT)}:{i}")

for w in warn: print("attention :", w)
for e in err: print("ERREUR :", e)
print(f"données : {len(Q)} quartiers, {len(Z)} zones, {len(E)} factions, {len(T)} types d'opérateurs, "
      f"{len(D['quetes']['liste'])} objectifs + ouverture de zones — {len(err)} erreur(s), {len(warn)} avertissement(s)")
sys.exit(1 if err else 0)

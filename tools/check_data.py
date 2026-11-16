"""Vérifie les données de CHOUF : JSON valide, identifiants uniques, références existantes,
bornes, et absence de noms réels interdits dans data/ et src/.
Usage : python3 tools/check_data.py   (depuis la racine du dépôt). Code de sortie 1 si erreur."""
import glob, json, os, re, sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
err, warn = [], []


def load(name):
    with open(os.path.join(ROOT, "data", name + ".json"), encoding="utf-8") as f:
        return json.load(f)


D = {}
for p in sorted(glob.glob(os.path.join(ROOT, "data", "*.json"))):
    n = os.path.basename(p)[:-5]
    try:
        D[n] = load(n)
    except Exception as e:  # JSON invalide
        err.append(f"{n}.json : {e}")
if err:
    print("\n".join(err)); sys.exit(1)


def ids(name):
    lst = D[name]["liste"] if "liste" in D[name] else D[name]["chapitres"]
    out = [x["id"] for x in lst]
    dup = {i for i in out if out.count(i) > 1}
    if dup:
        err.append(f"{name} : identifiants en double {sorted(dup)}")
    return set(out)


Z, P, E, R, C = ids("zones"), ids("personnages"), ids("equipes"), ids("roles"), ids("competences")
CH, EV, MI = ids("calendrier"), ids("evenements"), ids("missions")
VILLE = {v["id"] for v in D["calendrier"]["ville"]}

# zones : bornes, contrôle, au moins une zone de départ
for z in D["zones"]["liste"]:
    for k in ("tension", "police", "humeur"):
        if not 0 <= z[k] <= 100:
            err.append(f"zone {z['id']} : {k}={z[k]} hors 0..100")
    if z["controle"] is not None and z["controle"] not in E:
        err.append(f"zone {z['id']} : équipe inconnue {z['controle']}")
if not any(z["depart"] for z in D["zones"]["liste"]):
    err.append("zones : aucune zone de départ")
for e in D["equipes"]["liste"]:
    for z in e["zones"] + [e["origine"]]:
        if z not in Z:
            err.append(f"équipe {e['id']} : zone inconnue {z}")
for p in D["personnages"]["liste"]:
    for o in p.get("liens", {}):
        if o not in P:
            err.append(f"personnage {p['id']} : lien vers inconnu {o}")
for f in D["personnages"].get("familles", []):
    for k in ("mere", "petit_frere", "grande_soeur"):
        if k not in f or not f[k].get("prenom"):
            err.append(f"famille {f['id']} : {k} manquant")
for r in D["roles"]["liste"]:
    if r["chapitre_min"] not in CH:
        err.append(f"rôle {r['id']} : chapitre inconnu {r['chapitre_min']}")

# chaînes de conditions et d'effets : références pnj, compétences, faits, événements
FAITS = EV | {m["fait"] for m in D["missions"]["liste"] if "fait" in m}


def refs(s, where):
    for m in re.finditer(r"relation:(\w+)\.", s):
        if m.group(1) not in P and m.group(1) != "{pnj}":
            err.append(f"{where} : personnage inconnu {m.group(1)}")
    for m in re.finditer(r"competence:(\w+)", s):
        if m.group(1) not in C:
            err.append(f"{where} : compétence inconnue {m.group(1)}")
    for m in re.finditer(r"engagement:(\w+):", s):
        if m.group(1) not in P:
            err.append(f"{where} : engagement envers inconnu {m.group(1)}")
    for m in re.finditer(r"membre:(\w+)\.", s):
        if m.group(1) not in R:
            err.append(f"{where} : rôle inconnu {m.group(1)}")
    for m in re.finditer(r"fait:(\w+)", s):
        if m.group(1) not in FAITS:
            warn.append(f"{where} : fait « {m.group(1)} » produit par aucun événement pour l'instant")


for ev in D["evenements"]["liste"]:
    w = "événement " + ev["id"]
    for c in ev.get("conditions", []):
        refs(c, w)
    if ev.get("ville") and ev["ville"] not in VILLE:
        err.append(f"{w} : rendez-vous de ville inconnu {ev['ville']}")
    if not ev.get("options"):
        err.append(f"{w} : aucune option")
    for o in ev.get("options", []):
        for x in o.get("effets", []) + [o.get("condition", "")]:
            refs(x, w)
        for pr in o.get("programme", []):
            if pr["id"] not in EV:
                err.append(f"{w} : programme un événement inconnu {pr['id']}")
for m in D["missions"]["liste"]:
    if m["chapitre_min"] not in CH:
        err.append(f"mission {m['id']} : chapitre inconnu")
    for k in list(m["gains"]) + list(m["risques"]):
        refs(k + ".", "mission " + m["id"]) if k.startswith("relation:") else refs(k, "mission " + m["id"])
for r in D["calendrier"]["responsabilites"]:
    w = "responsabilité " + r["id"]
    if r["chapitre_min"] not in CH:
        err.append(f"{w} : chapitre inconnu")
    for c in r["conditions_narratives"] + r["conditions_systemiques"] + [r["alternative"]]:
        refs(c, w)
    if not r.get("alternative"):
        err.append(f"{w} : pas d'alternative (GDD V7 §5)")

# noms réels interdits
with open(os.path.join(ROOT, "tests", "lint", "interdits.json"), encoding="utf-8") as f:
    banned = json.load(f)["noms"]
pat = re.compile(r"(?<![\w])(" + "|".join(re.escape(b) for b in banned) + r")(?![\w])")
for p in glob.glob(os.path.join(ROOT, "data", "*.json")) + glob.glob(os.path.join(ROOT, "src", "**", "*.*"), recursive=True):
    if p.endswith((".png", ".webp", ".jpg", ".mp3")):
        continue
    for i, line in enumerate(open(p, encoding="utf-8", errors="ignore"), 1):
        m = pat.search(line)
        if m:
            err.append(f"nom réel interdit « {m.group(1)} » : {os.path.relpath(p, ROOT)}:{i}")

for w in warn:
    print("attention :", w)
for e in err:
    print("ERREUR :", e)
print(f"données : {len(Z)} zones, {len(P)} personnages, {len(E)} équipes, {len(R)} rôles, "
      f"{len(EV)} événements, {len(MI)} missions — {len(err)} erreur(s), {len(warn)} avertissement(s)")
sys.exit(1 if err else 0)

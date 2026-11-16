#!/usr/bin/env python3
"""MARSEILLE — tools/unity_quarters.py
Produit unity/Assets/Resources/Data/quarters.json (étape 10) à partir de data/quartiers.json
(111 quartiers officiels, data.gouv.fr, Licence ouverte 2.0) :
  - id = arrondissement × 100 + numéro (q0101 → 101) ;
  - zoneId 1-10 dans l'ordre de GameConfig.World.cs ;
  - x, y = centre du quartier, normalisés 0-1 sur l'emprise des centres (origine en haut à gauche) ;
  - neighbours = quartiers qui se touchent, calculés comme la version web (rastérisation, pas 2).
Usage : python3 tools/unity_quarters.py"""
import json
import math
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "data", "quartiers.json")
OUT = os.path.join(ROOT, "unity", "Assets", "Resources", "Data", "quarters.json")
ZONES = ["centre_hist", "centre_nord", "plaine", "prado", "nord_est",
         "nord_central", "nord_ouest", "littoral_nord", "est", "sud"]
STEP = 2
MARGIN = 0.03


def inside(p, ring):
    """Point dans un polygone (lancer de rayon)."""
    x, y = p
    res = False
    j = len(ring) - 1
    for i in range(len(ring)):
        xi, yi = ring[i]
        xj, yj = ring[j]
        if (yi > y) != (yj > y) and x < (xj - xi) * (y - yi) / (yj - yi) + xi:
            res = not res
        j = i
    return res


def build():
    src = json.load(open(SRC, encoding="utf-8"))
    cell, info = {}, {}
    for q in src["liste"]:
        pts = [p for ring in q["polys"] for p in ring]
        xs, ys = [p[0] for p in pts], [p[1] for p in pts]
        cells, sx, sy = [], 0.0, 0.0
        x = math.floor(min(xs) / STEP) * STEP
        while x <= max(xs):
            y = math.floor(min(ys) / STEP) * STEP
            while y <= max(ys):
                k = (x, y)
                c = (x + STEP / 2, y + STEP / 2)
                if k not in cell and any(inside(c, r) for r in q["polys"]):
                    cell[k] = q["id"]
                    cells.append(k)
                    sx += c[0]
                    sy += c[1]
                y += STEP
            x += STEP
        if not cells:
            cx, cy = (min(xs) + max(xs)) / 2, (min(ys) + max(ys)) / 2
            k = (math.floor(cx / STEP) * STEP, math.floor(cy / STEP) * STEP)
            cell[k] = q["id"]
            cells = [k]
        else:
            cx, cy = sx / len(cells), sy / len(cells)
        if cell.get((math.floor(cx / STEP) * STEP, math.floor(cy / STEP) * STEP)) != q["id"]:
            best = min(cells, key=lambda p: (p[0] - cx) ** 2 + (p[1] - cy) ** 2)
            cx, cy = best[0] + STEP / 2, best[1] + STEP / 2
        info[q["id"]] = {"q": q, "cx": cx, "cy": cy, "adj": set()}
    for (x, y), a in cell.items():
        for dx, dy in ((STEP, 0), (0, STEP), (STEP, STEP), (STEP, -STEP)):
            b = cell.get((x + dx, y + dy))
            if b and b != a:
                info[a]["adj"].add(b)
                info[b]["adj"].add(a)
    by_name = {v["q"]["nom"]: k for k, v in info.items()}
    iles = by_name.get("Les Îles")
    for other in ("Le Pharo", "Hôtel de Ville"):
        if iles and other in by_name:
            info[iles]["adj"].add(by_name[other])
            info[by_name[other]]["adj"].add(iles)
    return info


def to_int(qid):
    return int(qid[1:])


def main():
    info = build()
    xs = [v["cx"] for v in info.values()]
    ys = [v["cy"] for v in info.values()]
    x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
    out = []
    for qid, v in sorted(info.items(), key=lambda kv: to_int(kv[0])):
        q = v["q"]
        nx = MARGIN + (1 - 2 * MARGIN) * (v["cx"] - x0) / (x1 - x0)
        ny = MARGIN + (1 - 2 * MARGIN) * (v["cy"] - y0) / (y1 - y0)
        out.append({"id": to_int(qid), "name": q["nom"], "arrondissement": q["arr"],
                    "zoneId": ZONES.index(q["zone"]) + 1, "x": round(nx, 4), "y": round(ny, 4),
                    "neighbours": sorted(to_int(n) for n in v["adj"])})
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8") as f:
        f.write('{\n  "_source": "data.gouv.fr, Quartiers de Marseille (Licence ouverte 2.0) via tools/unity_quarters.py",\n  "quarters": [\n')
        f.write(",\n".join("    " + json.dumps(o, ensure_ascii=False) for o in out))
        f.write("\n  ]\n}\n")
    print(len(out), "quartiers écrits dans", os.path.relpath(OUT, ROOT))


if __name__ == "__main__":
    main()

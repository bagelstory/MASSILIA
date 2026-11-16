"""Convertit quartiers-marseille.geojson (111 quartiers officiels, décret de 1946, data.gouv.fr, Licence ouverte,
Joël Gombin) en data/quartiers.json : polygones simplifiés en coordonnées du plan du jeu, arrondissement, zone de jeu.
Usage : python3 tools/geo.py"""
import json, math, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "quartiers-marseille.geojson")
LAT0, LON_MIN, LAT_MAX = 43.28, 5.215, 43.395
K = 15.0                      # unités du plan par kilomètre
KX = 111.32 * math.cos(math.radians(LAT0)) * K
KY = 110.57 * K

# arrondissement → zone de jeu ; exceptions par nom
ZONE_ARR = {1: "centre_hist", 2: "centre_hist", 3: "centre_nord", 4: "plaine", 5: "plaine", 6: "prado", 7: "prado", 8: "sud",
            9: "sud", 10: "est", 11: "est", 12: "est", 13: "nord_est", 14: "nord_central", 15: "nord_ouest", 16: "littoral_nord"}
ZONE_NOM = {"Lodi": "plaine", "Notre-Dame-du-Mont": "plaine", "Périer": "prado", "Le Rouet": "prado", "Saint-Giniez": "prado", "Sainte-Anne": "prado"}


def xy(lon, lat):
    return [round((lon - LON_MIN) * KX, 2), round((LAT_MAX - lat) * KY, 2)]


def dp(pts, tol):
    """Douglas-Peucker."""
    if len(pts) < 3:
        return pts
    a, b = pts[0], pts[-1]
    dmax, idx = 0, 0
    for i in range(1, len(pts) - 1):
        p = pts[i]
        num = abs((b[1] - a[1]) * p[0] - (b[0] - a[0]) * p[1] + b[0] * a[1] - b[1] * a[0])
        den = math.hypot(b[1] - a[1], b[0] - a[0]) or 1e-9
        d = num / den
        if d > dmax:
            dmax, idx = d, i
    if dmax > tol:
        return dp(pts[:idx + 1], tol)[:-1] + dp(pts[idx:], tol)
    return [a, b]


def main():
    g = json.load(open(SRC, encoding="utf-8"))
    out = []
    for f in g["features"]:
        p = f["properties"]
        arr = int(p["DEPCO"][-2:])
        geom = f["geometry"]
        polys = geom["coordinates"] if geom["type"] == "MultiPolygon" else [geom["coordinates"]]
        rings = []
        for poly in polys:
            ring = [xy(c[0], c[1]) for c in poly[0]]
            m = len(ring) // 2                       # anneau fermé : on simplifie les deux moitiés
            s = dp(ring[:m + 1], 0.35)[:-1] + dp(ring[m:], 0.35)
            if len(s) >= 4:
                rings.append(s)
        out.append({"id": "q" + p["code_qua"][-4:], "nom": p["nom"], "arr": arr, "code": p["code_qua"],
                    "zone": ZONE_NOM.get(p["nom"], ZONE_ARR[arr]), "polys": rings})
    rep = {
        "vieux_port": [xy(5.3640, 43.2952), xy(5.3745, 43.2960), xy(5.3757, 43.2944), xy(5.3652, 43.2934)],
        "notre_dame": xy(5.3712, 43.2840), "velodrome": xy(5.3959, 43.2699),
        "grues": [xy(5.343, 43.322), xy(5.336, 43.332), xy(5.352, 43.312), xy(5.327, 43.343)],
        "calanques": [xy(5.37, 43.212), xy(5.40, 43.207), xy(5.43, 43.211), xy(5.46, 43.214), xy(5.49, 43.203)],
    }
    xs = [c[0] for q in out for r in q["polys"] for c in r]
    ys = [c[1] for q in out for r in q["polys"] for c in r]
    doc = {"_doc": "111 quartiers officiels de Marseille. Source : data.gouv.fr « Quartiers de Marseille » (Joël Gombin, Licence ouverte 2.0), "
                   "converti par tools/geo.py en coordonnées du plan (" + str(K) + " unités par km).",
           "taille": [round(max(xs) + 2), round(max(ys) + 2)], "repere": rep, "liste": out}
    json.dump(doc, open(os.path.join(ROOT, "data", "quartiers.json"), "w", encoding="utf-8"), ensure_ascii=False, separators=(",", ":"))
    print(len(out), "quartiers ; plan", doc["taille"], "; points", sum(len(r) for q in out for r in q["polys"]),
          "; fichier", os.path.getsize(os.path.join(ROOT, "data", "quartiers.json")) // 1024, "Ko")


if __name__ == "__main__":
    main()

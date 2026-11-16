"""Importe les images envoyées (assets/sources.json) dans le jeu : découpe des planches, recadrage,
fond magenta remplacé, icônes détourées en rond, export WebP dans dist/img/, index dans assets/index.json.
Repris de l'esprit de tools/v62_slice.py de KICK, simplifié (pas de mannequin d'avatar).
Usage : python3 tools/assets.py   puis   python3 src/build.py"""
import json, os, sys
from PIL import Image, ImageDraw, ImageFilter

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "dist", "img")
os.makedirs(OUT, exist_ok=True)
SRC = json.load(open(os.path.join(ROOT, "assets", "sources.json"), encoding="utf-8"))
FOND = (42, 44, 48)                      # remplace le magenta des portraits
TAILLES = {"banniere": (960, 540), "scene": (960, 540), "vue": (1280, 720), "portrait": (288, 288), "icone": (112, 112)}


def couvrir(im, w, h):
    """Recadre au centre pour remplir w×h, puis redimensionne."""
    r = max(w / im.width, h / im.height)
    im = im.resize((round(im.width * r), round(im.height * r)), Image.LANCZOS)
    x, y = (im.width - w) // 2, (im.height - h) // 2
    return im.crop((x, y, x + w, y + h))


def sans_magenta(im):
    """Fond magenta → gris foncé, avec un fondu sur les bords pour éviter le liseré rose."""
    im = im.convert("RGB")
    px = im.load()
    for y in range(im.height):
        for x in range(im.width):
            r, g, b = px[x, y]
            m = min(r, b) - g                    # « magentitude »
            if m > 60:
                t = min(1, (m - 60) / 80)
                px[x, y] = tuple(round(c * (1 - t) + f * t) for c, f in zip((r, min(g, (r + b) // 3), b), FOND))
    return im


def portrait(im):
    """Carré centré sur le visage : le haut du buste (les portraits sont cadrés tête et épaules)."""
    w, h = im.size
    cote = min(w, h) if h <= w * 1.15 else int(w * 0.92)
    top = 0 if h <= w * 1.15 else int(h * 0.07)
    x = (w - cote) // 2
    return im.crop((x, top, x + cote, top + cote))


def cases(im, cols, rows, bbox=None):
    x0, y0, x1, y1 = bbox or (0, 0, im.width, im.height)
    cw, ch = (x1 - x0) / cols, (y1 - y0) / rows
    m = 0.025                               # retire les traits de grille entre les cases
    for j in range(rows):
        for i in range(cols):
            yield im.crop((round(x0 + (i + m) * cw), round(y0 + (j + m) * ch), round(x0 + (i + 1 - m) * cw), round(y0 + (j + 1 - m) * ch)))


def bbox_clair(im, seuil=40):
    g = im.convert("L").point(lambda v: 255 if v > seuil else 0)
    return g.getbbox()


def icone(cell):
    """Isole le rond d'une case (fond noir) et le détoure en cercle avec transparence."""
    b = bbox_clair(cell)
    if not b:
        return None
    cx, cy, r = (b[0] + b[2]) / 2, (b[1] + b[3]) / 2, max(b[2] - b[0], b[3] - b[1]) / 2 + 2
    c = cell.crop((round(cx - r), round(cy - r), round(cx + r), round(cy + r))).convert("RGBA")
    W, H = TAILLES["icone"]
    c = c.resize((W * 2, H * 2), Image.LANCZOS)
    mask = Image.new("L", c.size, 0)
    ImageDraw.Draw(mask).ellipse((1, 1, c.width - 2, c.height - 2), fill=255)
    c.putalpha(mask.filter(ImageFilter.GaussianBlur(1)))
    return c.resize((W, H), Image.LANCZOS)


def sauver(im, cle, index, q=80):
    p = os.path.join(OUT, cle + ".webp")
    im.save(p, "WEBP", quality=q, method=6)
    index[cle] = "img/" + cle + ".webp"


def main():
    index, manque = {}, []
    for e in SRC["images"]:
        f = os.path.join(ROOT, e["fichier"])
        if not os.path.exists(f):
            manque.append(e["fichier"]); continue
        im = Image.open(f).convert("RGB")
        t = e["type"]
        if t in ("banniere", "scene", "vue"):
            sauver(couvrir(im, *TAILLES[t]), e["cle"], index, 78)
        elif t == "portrait":
            morceaux = list(cases(im, *e["grille"])) if "grille" in e else [im]
            cles = e.get("cases") or [e["cle"]]
            for m, k in zip(morceaux, cles):
                if k:
                    sauver(couvrir(portrait(sans_magenta(m)), *TAILLES["portrait"]), k, index, 82)
        elif t == "icones":
            for m, k in zip(cases(im, *e["grille"], bbox=bbox_clair(im)), e["cases"]):
                ic = icone(m)
                if k and ic:
                    sauver(ic, k, index, 90)
    json.dump(index, open(os.path.join(ROOT, "assets", "index.json"), "w", encoding="utf-8"), indent=1, ensure_ascii=False)
    print(f"{len(index)} images prêtes dans dist/img/ ({sum(os.path.getsize(os.path.join(OUT, k + '.webp')) for k in index)//1024} Ko)")
    if manque:
        print("introuvables :", *manque, sep="\n  ")
        sys.exit(1)


if __name__ == "__main__":
    main()

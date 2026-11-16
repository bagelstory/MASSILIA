#!/usr/bin/env python3
"""MARSEILLE — tools/import_art.py
Range les images générées (Gemini, Grok, ChatGPT) dans le jeu Unity, au bon format :
  python3 tools/import_art.py mapping.json
mapping.json = {"nom_de_l_image": "/chemin/du/fichier/source", ...} (noms de design/07-prompts-unity.md).
Copie aussi chaque original dans assets/brut/unity/ (hors Unity, pour garder les sources)."""
import json
import os
import shutil
import sys
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART = os.path.join(ROOT, "unity", "Assets", "Resources", "Art")
RAW = os.path.join(ROOT, "assets", "brut", "unity")


def cover(im, w, h, top_bias=0.5):
    """Recadre pour remplir w×h sans déformer ; top_bias < 0.5 garde plutôt le haut (visages)."""
    sw, sh = im.size
    scale = max(w / sw, h / sh)
    nw, nh = round(sw * scale), round(sh * scale)
    im = im.resize((nw, nh), Image.LANCZOS)
    x = (nw - w) // 2
    y = int((nh - h) * top_bias)
    return im.crop((x, y, x + w, y + h))


def black_to_alpha(im):
    """Fond noir → transparent (logo) : l'alpha suit la luminosité, sans halo."""
    im = im.convert("RGBA")
    px = im.load()
    for yy in range(im.size[1]):
        for xx in range(im.size[0]):
            r, g, b, _ = px[xx, yy]
            lum = max(r, g, b)
            a = 0 if lum < 18 else min(255, int((lum - 18) * 3))
            px[xx, yy] = (r, g, b, a)
    return im


def target(name):
    """(dossier, extension, largeur, hauteur, biais vertical) selon la série."""
    if name == "splash":
        return "screens", "jpg", 1080, 1920, 0.5
    if name == "map_fond":
        return "screens", "jpg", 1080, 1440, 0.5
    if name.startswith("end_"):
        return "screens", "jpg", 1080, 810, 0.5
    if name.startswith("flux_"):
        return "screens", "jpg", 1080, 540, 0.5
    if name == "icon_app":
        return "icon", "png", 1024, 1024, 0.5
    if name == "logo_chouf":
        return "logo", "png", 1024, 512, 0.5
    if name.startswith("portrait_"):
        return "portraits", "jpg", 512, 512, 0.2
    raise ValueError("nom inconnu : " + name)


def main():
    mapping = json.load(open(sys.argv[1], encoding="utf-8"))
    os.makedirs(RAW, exist_ok=True)
    for name, src in sorted(mapping.items()):
        folder, ext, w, h, bias = target(name)
        im = Image.open(src)
        shutil.copyfile(src, os.path.join(RAW, name + os.path.splitext(src)[1].replace(".jfif", ".jpg")))
        out = cover(im.convert("RGB"), w, h, bias)
        path = os.path.join(ART, folder, name + "." + ext)
        os.makedirs(os.path.dirname(path), exist_ok=True)
        if name == "logo_chouf":
            black_to_alpha(out).save(path)
        elif ext == "png":
            out.save(path, optimize=True)
        else:
            out.save(path, quality=86, optimize=True, progressive=True)
        print("%-28s %4dx%-4d → %s" % (name, im.size[0], im.size[1], os.path.relpath(path, ROOT)))


if __name__ == "__main__":
    main()

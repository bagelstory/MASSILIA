"""Assemble CHOUF en un seul fichier HTML (même principe que build62.py de KICK).
base.html + data/*.json (injecté dans window.DATA) + assets/index.json (window.ASSETS) + src/mod/*.js dans l'ordre.
Usage : python3 src/build.py [--out dist/chouf.html]"""
import glob, json, os, sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))


def build(out):
    base = open(os.path.join(ROOT, "src", "base.html"), encoding="utf-8").read()
    data = {}
    for p in sorted(glob.glob(os.path.join(ROOT, "data", "*.json"))):
        data[os.path.basename(p)[:-5]] = json.load(open(p, encoding="utf-8"))
    idx = os.path.join(ROOT, "assets", "index.json")
    assets = json.load(open(idx, encoding="utf-8")) if os.path.exists(idx) else {}
    mods = []
    for p in sorted(glob.glob(os.path.join(ROOT, "src", "mod", "*.js"))):
        mods.append("/* ---- " + os.path.basename(p) + " ---- */\n" + open(p, encoding="utf-8").read())
    html = (base.replace("/*__DATA__*/", "window.DATA=" + json.dumps(data, ensure_ascii=False, separators=(",", ":")) + ";")
                .replace("/*__ASSETS__*/", "window.ASSETS=" + json.dumps(assets) + ";")
                .replace("/*__MODS__*/", "\n".join(mods)))
    os.makedirs(os.path.dirname(out), exist_ok=True)
    open(out, "w", encoding="utf-8").write(html)
    print(f"{os.path.relpath(out, ROOT)} : {len(html)//1024} Ko, {len(mods)} modules, {len(data)} fichiers de données")


if __name__ == "__main__":
    o = sys.argv[sys.argv.index("--out") + 1] if "--out" in sys.argv else os.path.join(ROOT, "dist", "chouf.html")
    build(os.path.abspath(o))

#!/usr/bin/env python3
"""MARSEILLE — tools/gen_icons.py
Dessine en vectoriel (SVG → PNG via rsvg-convert) les icônes, emblèmes et formes de la version Unity,
sans génération d'image : badges des types d'opérateurs, indicateurs, tâches, pictogrammes d'onglets,
emblèmes des 5 équipes rivales (fictives), pion rond et case de quartier.
Sortie : unity/Assets/Resources/Art/<série>/<nom>.png  +  production/qa/evidence/unity/icones_apercu.png
Usage : python3 tools/gen_icons.py"""
import os
import subprocess
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "unity", "Assets", "Resources", "Art")
WHITE, GOLD, DISC, DISC_HI = "#F2F2F2", "#C9A24B", "#23252C", "#33363F"
def sw(width):
    """Trait blanc arrondi d'une épaisseur donnée."""
    return 'fill="none" stroke="%s" stroke-width="%s" stroke-linecap="round" stroke-linejoin="round"' % (WHITE, width)


S = sw(6)
F = 'fill="%s"' % WHITE

# ---- symboles (repère 100 × 100, centrés sur 50,50, dans le carré 22-78) ----
SYM = {
    # types d'opérateurs
    "compass": f'<circle cx="50" cy="50" r="24" {S}/><path d="M50 30 L57 50 L50 70 L43 50 Z" {F}/>',
    "press": f'<path d="M30 60 L50 46 L70 60 V76 H30 Z" {S}/><path d="M50 18 V36 M41 28 L50 37 L59 28" {sw(7)}/>',
    "van": f'<path d="M22 64 V40 H56 V64 Z M56 48 H68 L76 56 V64 H56" {S}/><circle cx="34" cy="68" r="6" {F}/><circle cx="66" cy="68" r="6" {F}/>',
    "coins": ''.join(f'<ellipse cx="50" cy="{y}" rx="20" ry="7" {S}/>' for y in (66, 54, 42, 30)),
    "magnifier": f'<circle cx="45" cy="45" r="16" {S}/><path d="M57 57 L74 74" {sw(9)}/>',
    "dumbbell": f'<path d="M34 50 H66" {S}/><rect x="22" y="36" width="10" height="28" rx="3" {F}/><rect x="68" y="36" width="10" height="28" rx="3" {F}/><rect x="16" y="42" width="6" height="16" rx="2" {F}/><rect x="78" y="42" width="6" height="16" rx="2" {F}/>',
    "stall": f'<path d="M22 32 H78 V42 A7 7 0 0 1 64 42 A7 7 0 0 1 50 42 A7 7 0 0 1 36 42 A7 7 0 0 1 22 42 Z" {F}/><path d="M28 50 V74 H72 V50 M40 74 V60 H60 V74" {S}/>',
    "warning": f'<path d="M50 24 L77 72 H23 Z" {S}/><path d="M50 42 V56" {S}/><circle cx="50" cy="64" r="3.5" {F}/>',
    "lock": f'<rect x="30" y="46" width="40" height="28" rx="4" {F}/><path d="M38 46 V38 A12 12 0 0 1 62 38 V46" {S}/>',
    # indicateurs
    "pin": f'<path d="M50 74 C40 60 34 52 34 42 A16 16 0 0 1 66 42 C66 52 60 60 50 74 Z" {S}/><circle cx="50" cy="42" r="5" {F}/>',
    "banknotes": f'<rect x="22" y="36" width="50" height="28" rx="3" {S}/><rect x="30" y="28" width="50" height="28" rx="3" {S}/><circle cx="47" cy="50" r="6" {F}/>',
    "people": ''.join(f'<circle cx="{x}" cy="40" r="7" {F}/><path d="M{x-11} 72 V64 A11 11 0 0 1 {x+11} 64 V72" {S}/>' for x in (30, 50, 70)),
    "gauge": f'<path d="M24 64 A26 26 0 0 1 76 64" {S}/><path d="M50 64 L66 44" {S}/><circle cx="50" cy="64" r="5" {F}/>',
    "scale": f'<path d="M50 26 V72 M36 72 H64 M28 36 H72" {S}/><path d="M22 54 L28 36 L34 54 Z M66 54 L72 36 L78 54 Z" {sw(4)}/>',
    "spotlight": f'<rect x="38" y="20" width="24" height="12" rx="3" {F}/><path d="M42 34 L24 76 H76 L58 34 Z" fill="#F2F2F2" fill-opacity="0.3"/><path d="M42 34 L24 76 M58 34 L76 76" {S}/><circle cx="50" cy="64" r="5" {F}/>',
    "door": f'<path d="M48 24 H72 V76 H48" {S}/><path d="M24 50 H54 M44 40 L54 50 L44 60" {S}/>',
    "chain": f'<rect x="20" y="40" width="26" height="20" rx="10" {S}/><rect x="54" y="40" width="26" height="20" rx="10" {S}/><path d="M50 30 V36 M50 64 V70" {sw(4)}/>',
    "hourglass": f'<path d="M32 24 H68 M32 76 H68 M36 24 C36 42 64 44 64 50 C64 56 36 58 36 76 M64 24 C64 42 36 44 36 50 C36 56 64 58 64 76" {S}/>',
    # tâches
    "shield": f'<path d="M50 22 L74 30 V48 C74 62 64 72 50 78 C36 72 26 62 26 48 V30 Z" {S}/>',
    "push": f'<path d="M22 36 L32 50 L22 64 M34 36 L44 50 L34 64" {S}/><path d="M52 52 L64 42 L76 52 V70 H52 Z" {S}/>',
    "binoculars": f'<circle cx="34" cy="58" r="12" {S}/><circle cx="66" cy="58" r="12" {S}/><path d="M38 46 L42 30 H58 L62 46 M46 54 H54" {S}/>',
    "eye": f'<path d="M20 50 C32 32 68 32 80 50 C68 68 32 68 20 50 Z" {S}/><circle cx="50" cy="50" r="9" {F}/>',
    "loop": f'<rect x="40" y="42" width="20" height="16" {F}/><path d="M28 44 A22 22 0 0 1 68 34 M72 56 A22 22 0 0 1 32 66" {S}/><path d="M62 28 L69 35 L61 40 M38 72 L31 65 L39 60" {sw(5)}/>',
    "gear": ''.join(f'<rect x="46" y="20" width="8" height="12" rx="2" {F} transform="rotate({a} 50 50)"/>' for a in range(0, 360, 45)) + f'<circle cx="50" cy="50" r="17" {S}/><circle cx="50" cy="50" r="5" {F}/>',
    "moon": f'<path d="M58 22 A28 28 0 1 0 76 62 A22 22 0 1 1 58 22 Z" {F}/>',
    "check": f'<path d="M26 52 L42 68 L74 34" {sw(9)}/>',
    "recruit": f'<circle cx="42" cy="38" r="10" {F}/><path d="M24 74 V66 A18 18 0 0 1 60 66 V74" {S}/><path d="M70 36 V56 M60 46 H80" {S}/>',
    # onglets
    "map": f'<path d="M22 30 L40 24 L60 30 L78 24 V70 L60 76 L40 70 L22 76 Z M40 24 V70 M60 30 V76" {S}/>',
    "chart": f'<path d="M24 76 H76" {S}/><rect x="28" y="52" width="10" height="20" {F}/><rect x="45" y="36" width="10" height="36" {F}/><rect x="62" y="26" width="10" height="46" {F}/>',
    "duo": ''.join(f'<circle cx="{x}" cy="38" r="9" {F}/><path d="M{x-14} 74 V66 A14 14 0 0 1 {x+14} 66 V74" {S}/>' for x in (36, 64)),
    "ship": f'<path d="M20 58 H80 L72 74 H28 Z" {S}/><rect x="30" y="44" width="12" height="10" {F}/><rect x="44" y="44" width="12" height="10" {F}/><rect x="44" y="32" width="12" height="10" {F}/><rect x="60" y="36" width="8" height="18" {F}/>',
    "flags": f'<path d="M20 50 H42 M34 40 L44 50 L34 60 M80 50 H58 M66 40 L56 50 L66 60" {S}/><path d="M50 30 V70" {sw(4)}/>',
    "notebook": f'<rect x="28" y="22" width="44" height="56" rx="4" {S}/><path d="M38 36 H62 M38 48 H62 M38 60 H54" {sw(4)}/>',
    "back": f'<path d="M66 72 C66 52 58 42 36 42 M46 30 L34 42 L46 54" {S}/>',
    "close": f'<path d="M30 30 L70 70 M70 30 L30 70" {sw(8)}/>',
    # emblèmes des équipes
    "wheel": f'<circle cx="50" cy="50" r="17" {S}/><circle cx="50" cy="50" r="5" {F}/>' + ''.join(f'<path d="M50 50 L50 22" {sw(4)} transform="rotate({a} 50 50)"/><circle cx="50" cy="21" r="3.5" {F} transform="rotate({a} 50 50)"/>' for a in range(0, 360, 45)),
    "seagull": f'<path d="M16 50 C28 34 42 38 50 54 C58 38 72 34 84 50" {sw(8)}/>',
    "stream": f'<path d="M20 62 L38 36 L52 56 L62 44 L80 62" {S}/><path d="M24 72 C34 66 42 76 52 70 C62 64 68 74 78 68" {sw(5)}/>',
    "wind": f'<path d="M20 38 H56 A8 8 0 1 0 48 30 M20 52 H70 A8 8 0 1 1 62 60 M20 66 H44" {S}/>',
    "cliff": f'<path d="M22 78 V52 L40 44 L46 60 H78 V78 Z" {F}/><path d="M36 18 L46 32 H26 Z M36 26 L48 42 H24 Z" {F}/><path d="M36 42 V46" {sw(4)}/>',
}

SERIES = {
    "types": (256, "badge", [("type_reperage", "compass"), ("type_pression", "press"), ("type_logistique", "van"),
                              ("type_finances", "coins"), ("type_renseignement", "magnifier"), ("type_force", "dumbbell"),
                              ("charbon", "stall"), ("menace", "warning"), ("verrou", "lock")]),
    "indicators": (128, "badge", [("ind_territoire", "pin"), ("ind_tresorerie", "banknotes"), ("ind_effectifs", "people"),
                                   ("ind_pression", "gauge"), ("ind_stabilite", "scale"), ("ind_exposition", "spotlight"),
                                   ("ind_capacite", "door"), ("ind_dette", "chain"), ("ind_cycle", "hourglass")]),
    "tasks": (128, "badge", [("task_securiser", "shield"), ("task_presser", "push"), ("task_reperer", "binoculars"),
                              ("task_surveiller", "eye"), ("task_flux", "loop"), ("task_optimiser", "gear"),
                              ("task_repos", "moon"), ("valider", "check"), ("recruter", "recruit")]),
    "tabs": (96, "plain", [("tab_carte", "map"), ("tab_bilan", "chart"), ("tab_equipe", "duo"), ("tab_flux", "ship"),
                            ("tab_rivaux", "flags"), ("tab_journal", "notebook"), ("btn_reglages", "gear"),
                            ("btn_retour", "back"), ("btn_fermer", "close")]),
    "factions": (256, "emblem", [("faction_compagnie", "wheel", "#C8352E"), ("faction_gabians", "seagull", "#3D7FD9"),
                                  ("faction_ruisseau", "stream", "#D08B2C"), ("faction_tramontane", "wind", "#8A5CD6"),
                                  ("faction_calanquais", "cliff", "#5FC4C4")]),
}


def badge(symbol, ring=GOLD):
    """Disque anthracite, reflet, anneau (or ou couleur d'équipe), symbole blanc."""
    return ('<defs><linearGradient id="g" x1="0" y1="0" x2="0" y2="1"><stop offset="0" stop-color="%s"/>'
            '<stop offset="1" stop-color="%s"/></linearGradient></defs>' % (DISC_HI, DISC) +
            '<circle cx="50" cy="50" r="45" fill="url(#g)"/>'
            '<circle cx="50" cy="50" r="45" fill="none" stroke="%s" stroke-width="6"/>' % ring +
            '<circle cx="50" cy="50" r="40" fill="none" stroke="#000" stroke-opacity="0.35" stroke-width="2"/>' +
            '<g transform="translate(50 50) scale(0.72) translate(-50 -50)">%s</g>' % SYM[symbol])


def svg(body):
    return '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100">%s</svg>' % body


def render(svg_text, path, size):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with tempfile.NamedTemporaryFile("w", suffix=".svg", delete=False) as f:
        f.write(svg_text)
    subprocess.run(["rsvg-convert", "-w", str(size), "-h", str(size), "-o", path, f.name], check=True)
    os.unlink(f.name)


def main():
    made = []
    for series, (size, kind, items) in SERIES.items():
        for item in items:
            name, sym = item[0], item[1]
            if kind == "badge":
                body = badge(sym)
            elif kind == "emblem":
                body = badge(sym, item[2])
            else:
                body = '<g transform="translate(50 50) scale(0.9) translate(-50 -50)">%s</g>' % SYM[sym]
            path = os.path.join(OUT, series, name + ".png")
            render(svg(body), path, size)
            made.append((series, name, body))
    # formes : pion rond (teinté par le jeu) et case arrondie (9-slice, bord 24 px sur 128)
    render(svg('<circle cx="50" cy="50" r="48" fill="#FFFFFF"/>'), os.path.join(OUT, "shapes", "pawn.png"), 128)
    render(svg('<rect x="1" y="1" width="98" height="98" rx="18" fill="#FFFFFF"/>'), os.path.join(OUT, "shapes", "tile.png"), 128)
    preview(made)
    print(len(made) + 2, "images écrites dans", os.path.relpath(OUT, ROOT))


def preview(made):
    """Planche d'aperçu de toutes les icônes (fond du jeu) pour vérifier d'un coup d'œil."""
    cols, cell = 9, 110
    rows = len(SERIES)
    parts, r = [], 0
    for series in SERIES:
        items = [m for m in made if m[0] == series]
        for c, (_, _, body) in enumerate(items):
            parts.append('<g transform="translate(%d %d) scale(1)">%s</g>' % (c * cell + 5, r * cell + 5, body))
        r += 1
    w, h = cols * cell, rows * cell
    text = ('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 %d %d"><rect width="%d" height="%d" fill="#121317"/>%s</svg>'
            % (w, h, w, h, "".join(parts).replace('id="g"', 'id="g"')))
    path = os.path.join(ROOT, "production", "qa", "evidence", "unity", "icones_apercu.png")
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with tempfile.NamedTemporaryFile("w", suffix=".svg", delete=False) as f:
        f.write(text)
    subprocess.run(["rsvg-convert", "-w", str(w * 2), "-o", path, f.name], check=True)
    os.unlink(f.name)


if __name__ == "__main__":
    main()

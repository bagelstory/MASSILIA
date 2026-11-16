"""CHOUF v2 — test d'interface (Playwright, téléphone 360×740).
Nouvelle partie, carte (opérateur → quartier → tâche), fiche de quartier et charbons, tous les onglets,
recrutement, flux, validation de plusieurs cycles, rapport, objectifs, reprise de sauvegarde.
Échoue sur toute erreur JS ou débordement. Captures dans production/qa/evidence/v2/.
Usage : python3 tests/ui/smoke.py"""
import os, sys
from playwright.sync_api import sync_playwright

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
OUT = os.path.join(ROOT, "production", "qa", "evidence", "v2")
os.makedirs(OUT, exist_ok=True)
URL = "file://" + os.path.join(ROOT, "dist", "chouf.html")
errs = []


def shot(page, nom):
    page.screenshot(path=os.path.join(OUT, nom + ".png"))


with sync_playwright() as p:
    b = p.chromium.launch()
    page = b.new_page(viewport={"width": 360, "height": 740}, device_scale_factor=2)
    page.on("pageerror", lambda e: errs.append(str(e)))
    page.on("console", lambda m: m.type == "error" and errs.append(m.text))
    page.goto(URL)
    shot(page, "01-accueil")
    page.click("[data-a=nouvelle]")
    page.evaluate("newGame(4242); PLAN.cam = null; PLAN.cache = null; save(); render()")   # partie reproductible
    page.click("[data-a=zplus]"); page.click("[data-a=zplus]")
    page.wait_for_timeout(300)
    assert page.locator(".chip").count() == 6, "il faut 6 indicateurs"
    shot(page, "02-carte")
    box = page.locator("#plan").bounding_box()

    def toucher_xy(x, y):
        page.mouse.click(box["x"] + x, box["y"] + y)
        page.wait_for_timeout(120)

    # opérateur Pression → sa base → Presser
    h = page.evaluate("(PLAN.hits||[]).filter(h=>opParId(h.op).type==='pression')[0]")
    assert h, "pas de badge d'opérateur sur la carte"
    toucher_xy(h["x"], h["y"])
    shot(page, "03-operateur-choisi")
    # un point du quartier de base, loin des badges
    xy = page.evaluate("""(()=>{const P=PLAN.cache.P;for(const c of VILLE.q[G.base].cells){const p=proj(P,c[0]+1,c[1]+1,0),e=versEcran(p[0],p[1]);
      if(e[0]>20&&e[0]<340&&e[1]>20&&e[1]<PLAN.el.clientHeight-140&&(PLAN.hits||[]).every(h=>Math.hypot(h.x-e[0],h.y-e[1])>h.r+6)&&sousDoigt(e[0],e[1]).q===G.base)return e;}return null;})()""")
    assert xy, "aucun point libre dans le quartier de base"
    toucher_xy(xy[0], xy[1])
    assert page.locator("[data-tache]").count() >= 6, "la liste des tâches ne s'ouvre pas"
    shot(page, "04-taches")
    page.click("[data-tache=presser]")
    # fiche de la base et ses charbons
    toucher_xy(xy[0], xy[1])
    assert page.locator(".fiche-q").count(), "la fiche de quartier ne s'ouvre pas"
    shot(page, "05-fiche-base")
    # zone verrouillée : dézoomer, toucher un quartier d'une zone fermée
    page.click("[data-a=fermer]")
    for _ in range(3): page.click("[data-a=zmoins]")
    shot(page, "06-ville-entiere")
    for t in ["objectifs", "equipe", "bord", "factions", "journal"]:
        page.click(f"[data-tab={t}]")
        page.wait_for_timeout(80)
        shot(page, f"10-{t}")
        w = page.evaluate("document.body.scrollWidth")
        assert w <= 362, f"l'onglet {t} déborde ({w}px)"
    page.click("[data-tab=bord]"); page.click("[data-a=voirflux]"); shot(page, "11-flux"); page.click("[data-a=retourbord]")
    page.click("[data-tab=equipe]")
    page.evaluate("G.tresorerie += 3000"); page.click("[data-tab=equipe]")
    page.locator("[data-recruter=force]").click()
    shot(page, "12-recrue")
    # plusieurs cycles
    page.click("[data-tab=carte]")
    for i in range(4):
        page.click("[data-a=valider]")
        page.wait_for_timeout(120)
        if i == 0: shot(page, "13-rapport")
        if page.locator("[data-a=okrapport]").count(): page.click("[data-a=okrapport]")
    shot(page, "14-apres-4-cycles")
    c = page.evaluate("G.cycle")
    assert c == 5, f"le cycle devrait être 5, il est {c}"
    page.reload()
    page.click("[data-a=continuer]")
    assert page.evaluate("G.cycle") == 5, "la sauvegarde ne se recharge pas"
    shot(page, "15-reprise")
    b.close()

if errs:
    print("ERREURS JS :\n" + "\n".join(errs[:10]))
    sys.exit(1)
print(f"OK — captures dans {os.path.relpath(OUT, ROOT)}")

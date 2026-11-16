"""CHOUF — test d'interface (Playwright, téléphone 360×740), même principe que tests/ui de KICK.
Crée une vie, répond aux cartes, fait une mission, tient une promesse, clôt des semaines,
ouvre chaque onglet, la carte, une zone, une fiche de personnage. Échoue sur toute erreur JS.
Captures dans production/qa/evidence/j1/.  Usage : python3 tests/ui/smoke.py"""
import os, sys
from playwright.sync_api import sync_playwright

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
OUT = os.path.join(ROOT, "production", "qa", "evidence", "j1")
os.makedirs(OUT, exist_ok=True)
URL = "file://" + os.path.join(ROOT, "dist", "chouf.html")
errs = []


def shot(page, nom):
    page.screenshot(path=os.path.join(OUT, nom + ".png"), full_page=False)


def cartes(page, n=6):
    """Répond aux cartes ouvertes, au bilan et à l'écran d'absence."""
    for _ in range(n):
        if page.locator("[data-opt]").count():
            page.locator("[data-opt]").first.click()
        elif page.locator("[data-a=okbilan]").count():
            page.locator("[data-a=okbilan]").click()
        elif page.locator("[data-a=okabsence]").count():
            page.locator("[data-a=okabsence]").click()
        else:
            return


with sync_playwright() as p:
    b = p.chromium.launch()
    page = b.new_page(viewport={"width": 360, "height": 740}, device_scale_factor=2)
    page.on("pageerror", lambda e: errs.append(str(e)))
    page.on("console", lambda m: m.type == "error" and errs.append(m.text))
    page.add_init_script("window.__TEST__=true")
    page.goto(URL)
    shot(page, "01-accueil")
    page.fill("#prenom", "Lucas")
    page.click("[data-a=nouvelle]")
    shot(page, "02-creation")
    page.click("[data-a=commencer]")
    assert page.locator(".sheet h2").inner_text() == "Le quotidien", "la première carte doit être « Le quotidien »"
    shot(page, "03-premiere-carte")
    cartes(page)
    shot(page, "04-quartier")
    page.locator("[data-mission=livrer_snack]").click()
    page.wait_for_selector(".toast")
    shot(page, "05-mission")
    for i in range(3):                       # semaines du prologue : Les visages, La promesse
        page.click("[data-a=clore]")
        if i == 0:
            shot(page, "06-bilan-ou-carte")
        cartes(page)
    if page.locator("[data-eng]").count():
        shot(page, "07-promesse")
        page.locator("[data-eng]").first.click()
    for t in ["carte", "gens", "journal", "moi", "quartier"]:
        page.click(f"[data-tab={t}]")
        shot(page, f"10-onglet-{t}")
        h = page.evaluate("document.body.scrollWidth")
        assert h <= 362, f"l'onglet {t} déborde en largeur ({h}px)"
    page.click("[data-tab=carte]")
    page.locator("[data-zone=sud] polygon").click()
    shot(page, "11-zone-inconnue")
    page.click("[data-a=retour]")
    zo = page.evaluate("G.protagoniste.zone_origine")
    page.locator(f"[data-zone={zo}] polygon").click()
    shot(page, "12-zone-chez-soi")
    page.click("[data-tab=gens]")
    page.locator("[data-pnj=mere]").click()
    shot(page, "13-fiche-mere")
    # sauvegarde : recharger la page et continuer
    page.reload()
    page.click("[data-a=continuer]")
    cartes(page)
    assert page.locator(".entete").count() == 1, "la sauvegarde ne se recharge pas"
    shot(page, "14-reprise")
    b.close()

if errs:
    print("ERREURS JS :\n" + "\n".join(errs))
    sys.exit(1)
print(f"OK — captures dans {os.path.relpath(OUT, ROOT)}")

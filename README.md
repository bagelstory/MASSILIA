# CHOUF

Jeu de gestion noir, 18+, à Marseille (2021 → 2026, puis l'après). Fiction : lieux réels, personnages et équipes inventés.

- Jouer : ouvrir `dist/chouf.html` dans un navigateur (téléphone conseillé).
- Construire : `python3 src/build.py`
- Vérifier les données : `python3 tools/check_data.py`
- Simuler des vies : `node tests/sim/sim.js 200 hasard` (styles : `hasard`, `prudent`, `hall`)
- Tester l'interface : `python3 tests/ui/smoke.py` (captures dans `production/qa/evidence/`)

Conception : `design/01-plan-v1.md`, `design/02-modele-donnees.md`, `design/04-prompts-3d.md` (prompts d'images, générés par `tools/gen_prompts.py`).
Équilibrage : tout est dans `data/*.json`.

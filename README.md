# CHOUF

Jeu de gestion stratégique mobile, 18+, sur la vraie carte de Marseille (111 quartiers). Tu diriges un réseau
depuis la carte : opérateurs, charbons, argent, pression, factions rivales inventées. Fiction : lieux réels,
personnages et équipes inventés.

- Jouer : ouvrir `dist/chouf.html` (téléphone conseillé).
- Construire : `python3 src/build.py` · images : `python3 tools/assets.py` · carte : `python3 tools/geo.py`
- Vérifier : `python3 tools/check_data.py` · `node tests/sim/sim.js 40 equilibre` · `python3 tests/ui/smoke.py`

Conception : `design/05-gestion-pure.md` (jeu) · `design/04-prompts-3d.md` (images).
Équilibrage : `data/*.json`. Ancienne version narrative : `archive/v1-narratif/`.

Contours des quartiers : data.gouv.fr, « Quartiers de Marseille » (Joël Gombin), Licence ouverte 2.0.

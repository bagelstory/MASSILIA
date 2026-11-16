# CHOUF v2 — gestion pure (remplace les documents 01 et 02 pour le jeu ; le 04 reste pour les images)

Source : ta « Spécification mécanique — Version gestion pure » (16 novembre), plus tes demandes du même
jour : la vraie carte, des quartiers verrouillés à ouvrir par des objectifs, plusieurs charbons plus ou
moins rentables par quartier, garder les images, lieux et personnages.

## Ce qui a changé

| Avant (v1) | Maintenant (v2) |
|---|---|
| Un ado de 16 ans incarné, famille, école, dialogues | **Aucun personnage incarné.** Le joueur dirige depuis la carte |
| Énergie, missions, cartes de choix, chapitres 2021-2026 | **Cycles d'une semaine** : planifier → valider → résolution → résultats |
| 10 zones schématiques | **Les 111 quartiers officiels**, vrais contours (data.gouv.fr), regroupés en 10 zones |
| Personnages avec mémoire | **Opérateurs** (6 types), avec un numéro, un visage et un surnom pris dans les personnages adultes du jeu, sans dialogue ni mémoire |
| Fins narratives | **Victoire** (70 % de la ville et trésorerie positive), **faillite**, **chute**, abandon, bilan chiffré et note A–E |

Le code et les données de la v1 sont gardés dans `archive/v1-narratif/`. Les images (familles, protagoniste,
scènes) restent dans le dépôt ; la v2 utilise les bannières de zones, les vues 3D, les icônes, et les
portraits des personnages adultes comme visages d'opérateurs dès qu'ils existent.

## La carte

- 111 quartiers, contours officiels (décret de 1946), convertis par `tools/geo.py` dans `data/quartiers.json`,
  dessinés en 3D (immeubles en maquette grise, comme les références) par `src/mod/55-plan.js`.
- Couleurs : vert = à toi, orange = contesté, couleur de la faction = tenu par elle, gris = libre, sombre = inconnu.
- **Zones verrouillées** (cadenas) : au départ, seule la zone de départ est ouverte. Une zone s'ouvre par un
  objectif (voir plus bas). Un Repérage peut entrer dans une zone verrouillée depuis un quartier voisin.
- Accès à un quartier : zone ouverte, et un quartier voisin à 50 % de contrôle ou plus (ou déjà présent).
- Les îles du Frioul sont reliées au Vieux-Port et au Pharo.

## Les charbons

- Chaque quartier a 0 à 4 charbons selon le type de bâti de sa zone (tours : 2-4 ; centre : 1-3 ;
  quartiers résidentiels : 0-2). Chacun a une **rentabilité** (€/cycle), un **risque** (1-3) et un propriétaire.
- Ouvrir un charbon libre : contrôle 30 % ou plus, coût = 1,2 × rentabilité. Reprendre celui d'une faction :
  contrôle 50 % ou plus et plus fort qu'elle, coût = 2 × rentabilité, pression et exposition en hausse.
- Revenu par cycle : rentabilité × contrôle % × efficacité (Pression sur place, sinon 0,5).
- Chaque charbon ouvert fait monter pression et exposition selon son risque. La police peut le **saisir**
  (fermé 3 cycles). Sous 15 % de contrôle, le charbon est perdu.
- Abstrait : un nom de lieu inventé (« le Hall B », « la Passerelle »…), un chiffre, un risque. Rien d'autre.

## Les objectifs (quêtes chiffrées, sans narration)

- **Ouvrir une zone** voisine d'une zone ouverte : tenir N quartiers à 50 % (N augmente avec le nombre de
  zones ouvertes), repérer 2 quartiers de la zone, avoir N charbons ouverts. Récompense : la zone, de
  l'argent, de la capacité toutes les 2 zones.
- **Réseau** : 2e charbon, connaître le voisinage, verrouiller sa base, 4 opérateurs, 15 000 € en caisse,
  5 charbons, 10 % de la ville, premier flux, un quart de la ville. Récompenses : argent ou capacité.
- Tout est dans `data/quetes.json`.

## Opérateurs, factions, économie, indicateurs, temps

Conformes à ta spécification (§4 à §11), avec ces précisions :
- Départ : 3 opérateurs (Force, Pression, Repérage), 5 000 €, base à 60 % avec 2 charbons.
- Fatigue : chaque cycle de travail ajoute 35 % du coût de la tâche (une tâche à 20 ⇒ +7 par cycle).
- Factions : 5 équipes inventées (La Compagnie, Les Gabians, Le Ruisseau, Tramontane, Les Calanquais), IA
  §5.3, **période calme** de 6 cycles au début, chaque action leur coûte du budget, trêves temporaires,
  disparition. La fragmentation (§5.5) affaiblit la faction au lieu de créer une nouvelle faction.
- Un quartier **sécurisé** tient seul (il ne descend pas sous 80 % sans attaque) ; les autres s'effritent sans opérateur.
- L'opérateur Finances fait aussi baisser l'exposition.
- Saisons : été (+30 % au centre et au sud), soirs de match (Prado), mistral.
- Toutes les valeurs : `data/equilibrage.json`.

## Écrans

Carte (par défaut) · Objectifs · Équipe · Bord (indicateurs, prévision, flux internationaux) · Factions ·
Journal. Six indicateurs toujours en haut, bouton unique « Valider le cycle » en bas.

## État de l'équilibrage (simulation, 20 à 40 parties par style)

- Un joueur passif tombe en faillite.
- Le joueur automatique « équilibré » survit 260 cycles, ouvre 7 zones et tient environ 9 % de la ville.
  Il ne gagne jamais : il est simple (binômes Pression + Force, pas d'investissement, pas de flux).
- Une équipe concentrée prend un quartier tenu à 60 % en 3 cycles : un joueur humain ira bien plus vite.
- À régler avec tes parties : la victoire à 70 % est très loin pour l'instant, et l'argent s'accumule en
  milieu de partie.

## Tests

- `python3 tools/check_data.py` : données, références, noms réels interdits.
- `node tests/sim/sim.js 40 equilibre` (styles : equilibre, prudent, agressif, passif) : parties complètes, invariants.
- `python3 tests/ui/smoke.py` : parcours complet sur téléphone, captures dans `production/qa/evidence/v2/`.

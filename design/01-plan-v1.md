# CHOUF — Étape 1 : plan de la V1 et reprise de KICK

Jeu de gestion noir, 18+, à Marseille (dépôt MASSILIA). Une vie de 16 ans à la fin, qui commence
**en 2026**. Pas de tours comme KICK : un rythme de jeu de gestion façon Mafia Wars / Narcos
(énergie, revenus qui tombent en temps réel, minuteurs, missions, raids).
Ce document répond à deux questions : qu'est-ce qu'on prend dans KICK, et que contient la V1.

> **Mise à jour du 16 novembre (après les GDD V6 et V7)** : période **2021 → 2026 puis « L'après »**
> (16 ans en 2021), rythme **mélange** (énergie en temps réel + semaine clôturée par le joueur),
> **10 zones** (les 8 du GDD + La Plaine / Cours Julien + Prado / Castellane). Ces décisions priment
> sur les §3 bis, §3 ter et l'annexe ci-dessous. Détail : `design/02-modele-donnees.md`.

---

## 1. Ce que j'ai lu dans KICK (branche `v62-sprites`, commit f652505)

- `src/kick-v61.html` (1,7 Mo) est la base : 60 versions empilées, chacune ajoute une section
  numérotée et **redéfinit** les fonctions précédentes (`vProfil=function(){…}`, `vArgent` défini
  deux fois, `SUB.arg="bank"` forcé plus loin…). Ça marche, mais c'est une pile de rustines.
- `src/build62.py` : base + `v62.js` + 17 modules dans un ordre fixe, injection de
  `assets/v62/index.json` et de l'audio, version `--test` avec petites images embarquées en base64.
- Moteur : état unique `G` (version `v:3`), `save()` → `localStorage`, `hasSave()`, `next()` qui
  fait avancer un trimestre (`upkeep → relTick → checkDeath → season → world → statuses →
  genOffers → drawCard → lifeEvent`), cartes `G.card` (texte + options), `TAB`/`SUB` + `go()`,
  `setSub()`, `render()` qui choisit l'écran dans une table.
- Monde : DJs simulés, rivaux, messages, carte, classements (`vRank`, `vMsgs`, `vMap`), mode
  « monde connecté » optionnel.
- Assets : planches Gemini (`design/v62/planches/A01…`), prompts dans `design/v62/prompts-*.txt`
  (règles de grille : 6 décors, 8 personnages, 12 objets, 24 icônes par planche), découpe par
  `tools/v62_slice.py` (grille, fond magenta, alignement sur mannequin vert, sortie PNG/WebP +
  `index.json`).
- Tests : `tests/sim/sim.js` (charge le HTML dans `vm` avec un faux DOM, joue N vies, statistiques
  et erreurs), `tests/ui/*.py` (Playwright, téléphone 360×740, parcours de tous les onglets,
  erreurs JS, hauteur des écrans), `tests/play/vie-entiere.py` (vie complète par appuis réels,
  détecte les blocages).

## 2. Verdict : je reprends l'architecture, pas le fichier

Je ne pars pas de `kick-v61.html`. Copier 1,7 Mo de rustines pour en jeter 90 % coûterait plus
cher que réécrire un socle propre de quelques centaines de lignes qui suit **les mêmes
conventions**. On garde la forme (un seul HTML, modules assemblés, état `G`, onglets, cartes,
mêmes tests), sans hériter des redéfinitions.

### À réutiliser presque tel quel
| Élément KICK | Dans MASSILIA |
|---|---|
| Build Python « base + modules dans l'ordre + JSON injecté » (`build62.py`) | `src/build.py` : `base.html` + `src/mod/*.js` + **`data/*.json` injecté** (l'équilibrage) |
| `V62.u()` + pack base64 pour la version de test | identique (`ASSET.u()`) |
| État `G` versionné, `save()`/`hasSave()`/`continueGame()` | identique, clé `massilia_v1`, migration par numéro de version |
| `TAB`/`SUB`, `go()`, `setSub()`, `render()` par table d'écrans | identique |
| Cartes à choix (`G.card`, `drawCard`, `choose`, poids, conditions, `once`, délai de retour) | identique, les cartes viennent de `data/evenements.json` |
| Composants : `sheet()`, `toast()`, `seg()`, lignes de liste, jauges | réécrits à l'identique, nouveau thème |
| Module sécurité des effets (mouvement réduit, bouton panique, plafond d'effets) | repris |
| `manifest.webmanifest` + `sw.js` (jeu installable hors ligne) | repris |
| Simulateur `tests/sim/sim.js` (vm + faux DOM) | repris, nouvelle stratégie de jeu |
| Playwright : tour des onglets + erreurs, vie entière par appuis | repris |
| Découpe des planches `v62_slice.py` (grille, magenta, index.json) | repris sans la partie mannequin d'avatar en couches |

### À adapter
| KICK | MASSILIA |
|---|---|
| Trimestre (`G.q`), `next()`, bouton « + Année » | **Jetés.** Remplacés par une horloge réelle `tick(now)` : énergie, revenus, minuteurs (voir §3 bis) |
| Villes (66) + carte | **Zones de Marseille** avec revenus, tension, police, humeur, services |
| DJs simulés + rivaux | **Équipes rivales inventées** qui agissent en continu (expansion, guerre, alliance, chute) |
| Équipe (manager, booker…) | **Équipe** : guetteurs, vendeurs, nourrices, chauffeurs, bras droit, avocat, comptable |
| Coups durs (arrestation, prison, huissier) | **Police** : contrôles, enquête (jauge cachée), perquisition, garde à vue, procès, Baumettes |
| Santé / addictions / proches | **Stress, santé, famille** (la mère, les frères et sœurs, l'école, les amis d'enfance) |
| Banque, affaires | **Argent sale / propre**, blanchiment par commerces |
| Messages | **Téléphone** : messages de l'équipe, des rivaux, de la famille, des habitants |
| Classements | **« Qui tient la ville »** : part de la ville par équipe, ta place |
| Coups du destin, ambiance (saisons, météo) | **Événements de quartier** : match de l'OM, mistral, fête, deuil, incendie, descente |
| Âge qui avance chaque trimestre | L'âge avance par **chapitres de vie** (voir §3 bis) |
| Fin de vie, héritage, panthéon | Fin : mort, prison longue, sortie (rangé), exil ; bilan, ce que deviennent les proches |
| Avatar en couches | **Portrait en 3 âges** (jeune / adulte / ancien) par planche, sans couches ni éditeur de look |

### À jeter
Casino, studio et production musicale, musique techno générée, réseaux sociaux (posts, reels),
mini-jeux DJ, éditeur de look, mode Phaser, monde connecté multijoueur, `vm`, les 60 anciennes
versions, et le gabarit à 49 agents de `.claude/` (le dépôt MASSILIA reste léger).

## 3. Contenu de la V1

**Une vie complète jouable, de 16 ans à la fin, sur toute la carte.**

- **Carte** de 14 zones réelles (liste en annexe). En V1, toutes les zones existent avec leurs
  données ; le contenu écrit (habitants, événements propres) est complet pour 4 zones et générique
  pour les autres.
- **Boucle** : voir §3 bis.
- **Équipe** : 7 rôles, recrutement parmi des PNJ du quartier, loyauté, fatigue, peur, arrestation,
  trahison possible.
- **Réputation et peur contre confiance du quartier** : on peut tenir une zone par la peur ou par la
  confiance ; les deux ont un coût différent.
- **Police** : présence par zone, enquête invisible qui monte avec le bruit que tu fais, contrôles,
  perquisitions, garde à vue, procès, prison. Opérations massives quand la violence dépasse un seuil
  (inspiré des opérations de « pilonnage » des points de deal, nom inventé dans le jeu).
- **Rivaux** : 5 équipes inventées, chacune avec un chef, une zone d'origine, un style. Alliances,
  trêves, guerres, rachats.
- **Corruption** (un contact acheté, risqué), **blanchiment** (snack, salle de boxe, resto,
  appartements), argent propre / sale.
- **Famille et proches** : la mère, deux frères et sœurs, l'école jusqu'à 18 ans, un ami d'enfance
  qui ne suit pas ce chemin. Ils sont touchés par tes choix.
- **Stress et santé**, sommeil, blessures.
- **Événements de quartier** et **messages**, **classement de la ville**.
- **Marchandise abstraite** : 3 niveaux (« petite », « moyenne », « lourde » marchandise), avec prix,
  marge et peine encourue. Aucun nom de produit précis dans les actions, aucune méthode.
- **Équilibrage** entièrement dans `data/*.json`.
- **Assets** : médaillons PNJ, bannières de zone, personnage en 3 âges (planches Gemini).
- **Tests** : smoke, simulation de 500 vies, interface Playwright, vie entière par appuis, et un test
  « aucun vrai nom » (voir §5).

### Repoussé après la V1
Exil et pilotage depuis l'étranger, extension vers d'autres villes (Avignon, Nîmes, Toulon…),
mode multijoueur, mini-jeux, musique, éditeur de look, contenu écrit complet des 14 zones,
traduction, lignes temporelles alternatives après 2026.

## 3 bis. Rythme de jeu (remplace les tours de KICK)

Modèle : Mafia Wars / Narcos Cartel Wars, mais en solo et avec une vie qui avance.

**Trois réserves qui se rechargent en temps réel** (vitesses dans `data/equilibrage.json`) :
- **Énergie** : les actions de terrain (missions, approvisionner, régler un problème du quartier).
- **Sang-froid** : les confrontations (tenir tête à un rival, défendre un plan, intimider).
- **Santé** : baisse avec les blessures et le stress, remonte au repos.

**Ce qui tourne quand tu ne joues pas** :
- Chaque plan que tu tiens rapporte de l'argent sale **par heure réelle**. L'argent s'entasse chez la
  nourrice, dans une limite de stockage. Plus il s'entasse, plus le risque de saisie, de vol par un
  membre de l'équipe ou de visite d'un rival monte : il faut passer **ramasser**.
- Les **chantiers** (ouvrir un plan, monter un commerce de façade, former un guetteur) ont un minuteur.
- La police et les rivaux continuent : à ton retour, un écran **« Pendant ton absence »** résume ce qui
  s'est passé (saisie, interpellation, rival qui avance, message de la mère).

**Ce que tu fais quand tu joues** :
- **Missions par zone** (comme les « jobs » de Mafia Wars) : coûtent de l'énergie, rapportent argent,
  réputation, expérience ; chaque mission a 3 niveaux de maîtrise. Le jeu montre aussi qui en paie le
  prix (une voisine qui a peur, un petit frère qui regarde).
- **Gérer les zones** : placer l'équipe, approvisionner, ramasser, régler les problèmes, blanchir.
- **Confrontations** contre les équipes rivales (inventées, jouées par l'IA du jeu) : prendre un plan,
  en défendre un, négocier une trêve, s'allier.
- **Cartes de situation** (comme les cartes de KICK) : décisions qui coûtent, souvent avec la famille,
  le quartier, la police.

**L'âge et la fin** :
- Pas d'âge par trimestre. La vie avance par **chapitres** : « 16-17 ans, le hall », « 18-20 ans, le plan »,
  « 21-25 ans, le réseau », « 26-35 ans, la ville », « après 35 ans, ce qui reste ». On passe au chapitre
  suivant quand on atteint un niveau de réputation **et** qu'un événement de vie se produit (majorité,
  première garde à vue, enfant, mort d'un proche…).
- La **prison** fait sauter des années d'un coup : une peine de 4 ans, c'est 4 ans d'âge en plus, une
  séquence jouée aux Baumettes, et une ville qui a changé à la sortie (ton équipe a tenu ou s'est fait
  avaler).
- **Fins** : mort, longue peine, sortie du trafic (on range tout, on vit), ou vieillesse si on tient
  jusque-là. Bilan final : ce que sont devenus la mère, les frères et sœurs, l'équipe, le quartier.

**Pour les tests** : l'horloge est injectée (`now()`), le simulateur peut faire passer des jours en une
seconde et jouer des vies complètes.

## 3 ter. Contexte de départ (2026)

La partie commence en 2026, dans la ville d'après la grande guerre des réseaux et les opérations
massives de police : une **super-structure inventée** tient la majorité des plans, des équipes plus
petites survivent ou se font racheter, la police harcèle les points de deal. Le passé (2010-2025) existe
comme mémoire du quartier : les anciens en parlent, les murs gardent les noms des morts (noms
inventés). La zone de départ est **tirée au sort** parmi les quartiers populaires (Nord 13e à 16e,
Belle de Mai / Saint-Mauront, Belsunce / Noailles, L'Estaque) ; chaque départ change les rivaux voisins,
la pression policière et la famille.

## 4. Jalons (une version testable publiée à chacun)

| Jalon | Contenu | Publication |
|---|---|---|
| J1 | Socle + données + **carte + une zone + une boucle complète** : énergie, mission, plan qui rapporte, ramasser, une descente, une carte de situation (étapes 2 et 3) | branche `j1`, HTML jouable |
| J2 | Équipe complète, police, rivaux, 4 zones écrites | `j2` |
| J3 | Blanchiment, famille, santé, événements, messages, classement, fin de vie | `j3` = V1 jouable |
| J4 | Assets Gemini intégrés, équilibrage sur simulation | `v1` |

## 5. Règles d'écriture traduites en règles techniques

- `data/` ne contient **que des personnages, équipes, chefs et policiers inventés**. Le dossier de
  recherche (`Dossier de Recherche & Lore…txt`) cite de vraies personnes, de vraies équipes et de
  vraies familles : il reste une source de contexte et **aucun de ces noms n'entre dans le jeu**.
  Un test (`tests/lint/vrais-noms.js`) lit une liste de noms interdits et échoue s'il en trouve un
  dans `data/` ou `src/`.
- Les lieux sont réels (zones, rues, places, plages, stades, lignes de bus et de métro). Une cité
  réelle peut être nommée **comme lieu** ; elle n'est jamais présentée comme le fief d'une équipe
  réelle.
- Les événements s'inspirent de types de faits publics (opérations de police, saisies, règlements de
  comptes, victimes collatérales, chantiers de corruption locale) et sont réécrits en scènes avec des
  personnages composites. Aucun événement ne reproduit une affaire identifiable (date + lieu +
  victime).
- Pas de mode d'emploi : pas de méthode pour fabriquer, cacher, faire passer, effacer des traces ou
  se procurer des armes. La violence existe comme décision et comme conséquence, pas comme procédé.
- Chaque système a son prix humain visible : les proches, la peur, le deuil, la prison. Les
  habitants ordinaires ont des cartes à eux (la mère de famille, le commerçant, l'éducateur,
  l'ancien, la jeune qui révise son bac) où ils parlent pour eux-mêmes.

---

## Annexe — les 10 zones (remplace la liste de 14)

Centre historique · Centre nord · La Plaine / Cours Julien · Prado / Castellane · Nord-est · Nord central ·
Nord-ouest · Littoral nord · Est · Sud. Quartiers, lieux et valeurs : `data/zones.json`.

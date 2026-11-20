# CHOUF : revue d'équipe des documents 00, 08 à 16 (incohérences et corrections)

*20 novembre 2026. Relecture croisée par la fonction « équipe narrative » (direction narrative, écriture, univers). **Aucun document existant n'a été modifié** : ce fichier liste ce qui se contredit et propose un correctif. Les décisions marquées « à trancher » reviennent au joueur. Les correctifs de texte ne touchent pas au code.*

## 1. Point central : « 5 ans » contre « 25 à 40 ans »
**Ce que dit le dépôt aujourd'hui :** campagne de **5 ans, 260 semaines** (`00` pitch ; `game-brief` but et MVP ; `08` D1/D3 ; `11` §2 « Cinq ans de dette » ; `14` §1 tableau ; `16` §5). Le code porte `maxCycles = 260`.
**Arbitrage du joueur (`16` §12) :** « simulation de vie de 25 à 40 ans ».

**Interprétation retenue dans `18` §4 (à confirmer)** : le joueur a 18 ans à l'entrée ; la partie dure **25 ans minimum (fin vers 43 ans) et 40 ans maximum (fin vers 58 ans)**, en **actes de 5 ans** (Acte I = « Cinq ans de dette »).
**Autres lectures possibles (à trancher) :**
- (b) Le joueur a *25 à 40 ans* tout au long (une vie d'adulte, pas de prologue à 16 ans) : incompatible avec `14` (prologue 16-17).
- (c) La durée de la partie est tirée au sort entre 25 et 40 ans : possible, mais sans valeur de scénario.
- (d) La simulation couvre la vie de plusieurs personnages (le joueur change de héros) : non évoqué.

**Correctifs proposés** (si on retient `18` §4) :
1. `00` : remplacer « le joueur tient un réseau sur 5 ans (260 semaines) » par « sur une vie (25 à 40 ans, en actes de 5 ans) ». 
2. `game-brief` : le but « 60 % à la fin des 5 ans » devient **le but de l'Acte I** ; ajouter le but long (survivre, sortir, ou transmettre).
3. `11` §2 : intitulé « Acte I » ; garder le contenu tel quel.
4. `14` §1 : ajouter la ligne « Actes II à VIII » après « Cinq ans de dette » (le tableau de `18` §4.1 sert de modèle).
5. `16` : la fin de partie dépend de l'âge, pas seulement des semaines.
6. **Code (plus tard)** : `maxCycles` par acte, âge dérivé du cycle, bilan « Ce qu'ils sont devenus ».
**Risque :** 2 080 semaines en tours d'une semaine est trop long : voir `18` §4.2 (temps élastique).

## 2. Les incohérences, une par une
| # | Où | Problème | Correctif proposé | Gravité |
|---|---|---|---|---|
| 1 | `00` règle 5, `14` §3 | « **Années jamais datées** » mais `11` §0 dit « Marseille de **2010** à aujourd'hui », `10` §6 (« 2010-2020 »), `01` §« le passé (2010-2025) » | Remplacer les dates par « la décennie qui précède » ou « Année N » ; garder « 2010 » seulement hors du jeu, dans un dossier de contexte | Moyenne |
| 2 | `00` règle 5, `10` §0 | `00` dit « **Rue d'Aubagne hors du jeu** » ; `10` §0 la **nomme avec une date** et autorise « une phrase de Madame Ferrand » ; `10` §« corrections » dit qu'elle est « retirée » | **Hors du jeu partout**, y compris la phrase de Ferrand ; effacer la date de `10` §0 ; garder seulement la mention d'interdit dans `00`/`11`/`13` | **Haute** (affaire réelle, deuils réels) |
| 3 | `10` §3 (rangs), `15` §2 (postes), `18` | Deux systèmes de rôles : **Jobbeur, Guetteur, Charbonneur, Gérant, Nourrice, Donneur de go, Lieutenant** (`10`) contre **Guetteur, Vendeur, Gérant, Tête, Chef** (`15`) | Table de correspondance : Charbonneur = **Vendeur** ; Lieutenant ≈ **Tête** ; Jobbeur = **remplaçant temporaire** ; Nourrice et Donneur de go = **rôles annexes** hors des 5 postes ; Chef = chef d'arrondissement | Moyenne |
| 4 | `10` §3, `11` §1 | « **Donneur de go** » alors que `11` interdit « go fast » à l'écran | Renommer (« donneur d'ordre ») ou retirer ; vérifier `OperatorType` dans le code | Basse |
| 5 | `11` §2 (« tu hérites du réseau »), `14` §1, `15` §4 | Que reçoit le joueur à 18 ans : **le réseau entier** (`11`), **une dette** (`14`), **une tête qui monte** (`15`) ? | Départ clair : **un charbon + la dette de Lucien**, apprenti (`15` lecture A) ; le « réseau » se construit par postes ; le chef d'arrondissement est un but, pas un départ | **Haute** |
| 6 | `15` §1, `18` | 16 arrondissements, mais **seuls 3 chefs** sont écrits (Rosalie, Tigran + Cadet) | Banque de 16 chefs : mêmes champs que `18` (priorités, arc) ; les autres sont **vacants** jusqu'à ce que le poids d'une tête dépasse un seuil | Moyenne |
| 7 | `14` §5, `18` | Âges des personnages du prologue (grand frère 22, prof 40, voisine 65) valent pour **16-17 ans** ; à l'Année 1 ils ont **+2 ans** | Déjà corrigé dans `18` (24, 42, 67) ; ajouter la phrase dans `14` §5 | Basse |
| 8 | `11` §2 (année 4), `12` E12, `12` §7 | **Mineurs** : un garçon « de 15 ans » meurt, un « gamin » écrit au Fil, une une dit « un jeune de 15 ans » : conforme à la règle (victimes seulement) mais **précision d'âge** inutile et sensible | Remplacer les âges par « un adolescent », « un jeune » ; relecture externe obligatoire (`12` §10) ; option de contenu pour atténuer | **Haute** |
| 9 | `12` §3 (lexique), `00` règle 5 | « **L'OM a perdu, les nerfs sont là** » : un club réel nommé dans un jeu sur le crime, alors que `00` interdit « club identifiable » | Écrire « le match » ou « les soirs de match » ; ne **jamais lier** un club réel à un acte ; idem « Vélodrome » comme décor seulement | Moyenne |
| 10 | `00` (priorités) | `00` dit **Unity 2022.3 LTS** ; le brief du joueur dit **Unity 6.6** ; `game-brief` dit 2022.3 | Voir `16` §5, décision 8 ; ne rien changer avant la réponse | Moyenne |
| 11 | `00`, `16` | Version de sauvegarde **4** (`00`) contre **2** (brief) | Confirmer 4 ; migration inchangée | Basse |
| 12 | `10` §6 (calendrier des fêtes), `10` voix | `10` garde un **ton festif** et des voix légères, remplacés par `11` | Marquer `10` §6 « obsolète » dans l'en-tête, déplacer les voix légères dans le BRAINSTORM (si utiles) | Basse |
| 13 | `11` §6, `12` §9, `18` §4.5 | Trois listes de **fins** qui se recoupent (8 + 4 + 4) | Les regrouper en **familles de trajectoires** (`14` §2 : « fins libres ») : Chute, Trône, Cercueil, Héritage, Fantôme, Témoin, Sortie, Retraite ; chaque fin garde son texte | Moyenne |
| 14 | `11` §2 (année 5 « les nouveaux du port »), `18` | Avec 25-40 ans, la **nouvelle bande de l'Année 5** ne suffit plus | Une **vague d'arrivants par acte** (inventés, sans pacte) ; les anciens se retirent, meurent ou s'effacent | Moyenne |
| 15 | `16` mockups | La maquette du journal nomme une bande **« Gabians »** alors que « le Gabian » est un personnage (`10`) | Remplacer par un autre nom inventé (ex. « les Mouettes » ou un nom neutre) ; vérifier qu'il ne correspond à aucun groupe réel | Basse |
| 16 | `15` §6, `11` §1 | « Les petites mains sont souvent très jeunes » : **le jeu dit la perte, pas la fiche de poste**, mais `11` §1 parle de « petites mains jetables » sans dire l'âge | Ajouter dans `15` §6 la phrase explicite : **tous les postes à l'écran sont tenus par des adultes** ; l'existence de jeunes impliqués passe par les événements | Moyenne |
| 17 | `08` (D1), `01` | `01-plan-v1` et `08` parlent encore de **trimestre** (ancien modèle) | Marquer `01` « archive » ; garder `08` tel quel (historique) ; `00` impose « semaine » | Basse |
| 18 | `12` §4 (Le Modo, Brasier, Influenceur) | Noms **peu vérifiés** (liste de `00`) : Courrier des Docks, l'Armateur, Vieux Lucien, Brasier, le Modo | Chercher les noms dans un moteur de recherche (hors du jeu) ; changer ceux qui correspondent à une personne, un média ou une marque réels | Moyenne |

## 3. Corrections de ton
- **`10`** : voix et humour du calendrier festif contrastent avec `11` : à marquer comme archive.
- **`11` §10 « La ville la nuit » et `12` §8** : ton noir tenu ; rester sur la **trace** (bougie, scotch, fresque), pas sur l'acte.
- **`13`** : ton neutre/réaliste, sans glorification : ne rien changer.
- **Tous** : un mot de marseillais par réplique, jamais en insulte (`17` §6).

## 4. Ce que l'équipe a ajouté (cohérent avec la revue)
- `17-ecriture-marseille.md` : ambiances par arrondissement, saisons, vie quotidienne, rapports, lexique, **30 textes**.
- `18-personnages-et-vie.md` : **24 personnages** (priorités, relations, arcs, répliques) et la **structure de vie 25-40 ans**.
- La banque de noms anonymes (30 prénoms, 10 surnoms) de `18` §3 suit la recommandation de `15` §8.5.

## 5. À trancher par le joueur
1. **« 25 à 40 ans » :** confirmer l'interprétation (partie de 25 à 40 ans à partir de 18 ans).
2. **Temps élastique** (`18` §4.2, option B) ou semaine partout ?
3. **Départ du joueur :** un charbon + la dette, ou le réseau entier ?
4. **Mineurs :** retirer les âges précis des journaux et événements, avec relecture externe ?
5. **Aubagne :** hors du jeu à 100 % (y compris la phrase de Ferrand) ?
6. **Noms à vérifier :** qui les contrôle (recherche web, hors du jeu) ?

## 6. Ordre de correction conseillé (sans code)
1. `10` : retirer la date et la phrase sur la rue d'Aubagne ; marquer §6 obsolète.
2. `00`, `game-brief`, `11`, `14` : durée et actes (sous réserve de l'arbitrage 1).
3. `12` : retirer « OM » et les âges de mineurs ; vérifier les noms.
4. `15` : table de correspondance des rôles et phrase « adultes seulement ».
5. `16` : renommer « Gabians ».

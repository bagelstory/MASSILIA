# CHOUF : audit de l'interface et proposition de refonte (phases 0 et 1)

*17 novembre 2026. Réponse au brief « Refonte de l'interface et direction visuelle ». **Aucun code n'a été modifié.** Ce document contient l'audit, une architecture minimale, la description des maquettes, un plan par étapes, les risques et les décisions à arbitrer avant toute refonte. Limite de l'audit : lecture du code de `j1` (commit `4128358`), sans pouvoir lancer Unity ni voir d'écran. Les causes visuelles ci-dessous sont des hypothèses fortes, à confirmer par une capture.*

## 1. Ce qui fonctionne déjà (à ne pas casser)
| Domaine | Où | Remarque |
|---|---|---|
| Cycle hebdomadaire et résolution | `Core/CycleManager`, `ICycleSystem` | 12 systèmes dans l'ordre de `GameManager.Systems.cs` |
| Économie, flux, factions, opérateurs, événements, quêtes, fins | `Systems/*` | stables côté logique |
| Ombre, façades, hiérarchie des charbons | `Systems/Shadow*`, `Facade*`, `Crew*`, `HierarchySystem` | **jamais lancés dans Unity** (écrits après le premier lancement) : à valider avant d'y brancher l'UI |
| Sauvegarde | `Core/SaveManager` | version **4** (le brief dit 2), migrations 1→4 |
| Écrans | `UI/*` (27 fichiers, 2 752 lignes) | tout est construit par code, aucun prefab |
| Carte 2D | `Map/*` (945 lignes) | 111 quartiers en cases sur une grille, glisser et pincer |
| Ressources | `Resources/Art` (70 images) | 16 portraits (jpg), 9 écrans (jpg), icônes d'onglets, d'indicateurs, de tâches, 5 emblèmes de factions |
| Tests | `tests/sim`, `tests/ui`, `tests/lint` | simulation JS, smoke UI Python, noms interdits |

## 2. Structure actuelle de l'interface
- **Un seul Canvas** (`UIManager`, plein écran, `ScaleWithScreenSize` 1080×1920, portrait, `matchWidthOrHeight = 0`).
- **Châssis** (`UIManager.Chrome.cs`) : fond, barre du haut (150 px, **deux `Text`** : gauche = cycle, saison, palier d'Ombre ; droite = argent, territoire, pression, effectif), zone centrale, voile d'Ombre, barre d'onglets (140 px, 6 boutons), bandeau court (toast).
- **Navigation** : 6 `UIPanel` créés dans l'ordre Carte, Bilan, Équipe, Flux, Rivaux, Journal. Un onglet = un panneau, un seul visible. `ShowPanel(index)` ou `ShowPanel<T>()`.
- **Rafraîchissement** : `EventBus.OnDataChanged` puis `Refresh()` du panneau visible ; les listes sont vidées et reconstruites à chaque fois (`ListPanel`).
- **Fenêtres** : `UIOverlay` (fond assombri + fenêtre centrée) pour le détail d'un quartier, d'un opérateur, les résultats d'un cycle, la fin de partie.
- **Boîte à outils** (`UiKit`, `UiFactory`, `UiCard`) : étiquettes, boutons, colonnes, défilement ; `UiText` pour les textes.
- **Style** : tout dans `GameConfig.UI` : 1 police intégrée (`LegacyRuntime`), 3 tailles (46, 34, 27), 8 couleurs, hauteur de bouton 110 px, espacement 14, marge 28. Aucun arrondi, aucune ombre, aucune notion de composant stylé.

## 3. Éléments visuels à remplacer
1. **Barre du haut** : deux blocs de texte sans icônes ni infobulles. Les icônes existent déjà (`indicators/ind_*.png`).
2. **Onglets** : fond plat, 110 px de haut dans une barre de 140 px, icône et libellé collés.
3. **Journal** : lignes de texte brutes, filtres en boutons de 110 px.
4. **Tous les panneaux « liste »** : `Label` à hauteur fixe, sans carte, sans hiérarchie.
5. **Fiches** (quartier, opérateur) : fenêtre défilante de lignes de texte ; pas de portrait dans la fiche quartier, pas d'illustration de lieu.
6. **Carte** : cases rectangulaires plates ; image de fond `map_fond.jpg` seulement dans l'écran de départ.

## 4. Chevauchement du journal : causes probables
- **Cause 1 (la plus probable).** `UiKit.Row(…, hauteur = 0,75 × 110 = 82 px)` contient des boutons dont `LayoutElement.minHeight` vaut 110 px (`UiKit.SetHeight` appelé par `MakeButton`). Un groupe horizontal ne peut pas réduire un enfant sous son minimum : les boutons dépassent d'environ 28 px sous la rangée et recouvrent la première ligne du journal.
- **Cause 2.** `AddLine` estime la hauteur à 1,4 ou 2,6 lignes selon `text.Length > 48`, alors qu'un texte de 90 caractères ou plus passe sur 3 lignes en taille 27. `verticalOverflow` est en `Overflow` : le texte déborde sur la ligne suivante.
- **Correction prévue** (à l'étape 4 du plan) : cartes à hauteur calculée par le texte (`ContentSizeFitter` / `preferredHeight` du `Text`) et filtres dans une barre fixe hors du défilement.

## 5. Écarts entre le brief et le dépôt
| Le brief dit | Le dépôt dit | Proposition |
|---|---|---|
| Unity **6.6** | Les outils d'éditeur et les conventions ont été écrits pour Unity 2022.3 LTS ; `ProjectSettings` n'est **pas** dans le dépôt | Voir risques §9 (entrée, `FindObjectOfType`) |
| Sauvegarde **version 2** | Version **4** | Garder les migrations ; ne pas toucher à la version pour l'UI |
| Simulation de vie **25 à 40 ans**, trajectoires émergentes, cinq priorités personnelles | Campagne de **5 ans / 260 semaines** (D2, D3, `11`), prologue puis gestion (`14`), hiérarchie de charbons (`15`) | **À arbitrer** (§10, décision 2). Pendant la refonte, l'UI ne fige aucune durée |
| Cinq priorités (ambition, famille, liberté, loyauté, stabilité) | Absentes du code (un opérateur a compétence, fatigue, spécialité) | La fiche personnage les affiche **seulement quand les données existent** |
| Relations, historique, personnage nommé | Non modélisés (journal global seulement) | Même règle : emplacements prévus, remplis plus tard |
| Cycle hebdomadaire, `ICycleSystem`, `JsonUtility` | Respectés | Rien à changer |

## 6. Architecture minimale proposée
Principe : **aucune nouvelle couche**. On enrichit `GameConfig.UI` et on ajoute quelques composants réutilisables dans `UI/Kit/` (≤ 200 lignes chacun, mêmes conventions).

| Brique | Rôle | Remplace |
|---|---|---|
| `GameConfig.UI` étendu | palette (anthracite, bleu pétrole, gris béton, ivoire, jaune chaud, orange rouille, rouge assourdi, vert discret), rayons, ombres, tailles, durées d'animation | couleurs et tailles en dur |
| `UiSprites` | rectangles arrondis neuf-découpes **générés au lancement** (pas d'asset à fournir) | cases plates |
| `UiChip`, `UiCardView` | puce (indicateur, filtre, étiquette) et carte (titre, texte, conséquences, détail repliable) | `Label` + `SetHeight` |
| `TopBarView` | barre compacte : deux lignes, icônes, infobulle au toucher long | les deux `Text` |
| `TabBarView` | 6 onglets, icône + libellé, onglet actif en jaune, indicateur discret | `BuildTabs` |
| `PortraitView` | portrait ou silhouette de repli | portrait collé à la main |
| `ProfileCard`, `PlaceCard` | fiches réutilisables (§7) | popups de lignes de texte |
| `IMapView` (interface, **sans code carte 3D**) | `ShowQuarter`, `Focus`, `Highlight`, `QuarterSelected` | appels directs à `MapController` |

**Ce qui ne change pas :** `UIManager`, `UIPanel`, `EventBus`, tous les systèmes, `Map/*` jusqu'à la phase 4.
**Ce qui est prêt pour la carte 3D :** la zone centrale reste un conteneur unique ; une `IMapView` permettra de remplacer la carte 2D sans toucher au Journal ni aux fiches.

## 7. Maquettes (rendues dans la conversation ; descriptions de référence)
Format : portrait 1080×1920, tailles ramenées à 340 px dans la maquette.

**A. Écran principal.** Barre du haut (≈ 130 px) : ligne 1 = « Semaine 12 · Année 1 · Automne » (petit, gris) et l'argent en grand ; à droite, le cycle et le palier d'Ombre en orange rouille. Ligne 2 = quatre puces égales : Territoire, Pression (jaune au-dessus de 30), Effectif, Prévision (vert si positive). Zone centrale = carte pleine largeur, quartiers colorés (vert : à toi, rouille : rivaux, gris-bleu : neutre), contour jaune sur le quartier sélectionné. Une seule action principale en jaune : « Valider la semaine ». Fiche de quartier en bandeau replié (nom, arrondissement et chef, barre de contrôle, une ligne d'état) qui s'ouvre sur « Fiche ». Navigation : 6 onglets, icône + libellé 11 px, onglet actif jaune.

**B. Barre du haut.** Voir A. Infobulles : toucher une puce explique le chiffre et sa tendance (ex. « Pression 34 : monte avec les guetteurs manquants et les contrôles »).

**C. Navigation.** 6 onglets conservés. **La Carte est l'écran de départ et revient à un toucher depuis n'importe où.** Les autres onglets restent à un toucher (aucun menu caché).

**D. Journal.** En-tête fixe avec titre et 5 filtres en puces d'une ligne (plus de boutons de 110 px). Fil groupé par semaine. Chaque événement = carte à bande de couleur (rouge assourdi : pertes, vert : gains, rouille : rivaux, gris : info), catégorie et lieu, texte sur 2 à 3 lignes, puces de conséquences (« Pression +3 », « +3 200 € »). Toucher une carte l'ouvre : qui, où, effets chiffrés, « Voir sur la carte ». Aucun texte ne dépasse de sa carte (hauteur calculée).

**E. Fiche de personnage.** Bandeau portrait (existant : jpg ; sinon silhouette), nom, numéro, poste et lieu, état en puces (disponible, fatigue, compétence), cinq priorités en barres **si disponibles**, relations (lien et ton), historique en quelques lignes, bouton principal « Affecter ». Réutilise `OperatorDetailPopup` comme contenu.

**F. Fiche de lieu.** Bandeau d'illustration (emplacement ; repli = silhouette de bâtiments), nom, type, quartier et arrondissement, équipe (les 5 postes de `15`), indicateurs de gestion (rendement, risque, revenu), événements récents, bouton d'action (ex. « Placer un guetteur »). Réutilise `QuarterDetailPopup` et la section Crew existante.

## 8. Plan de développement par étapes
*Chaque étape : un commit Conventional Commits, un test avant/après (capture d'écran conservée), les systèmes intacts.*

| Étape | Contenu | Risque |
|---|---|---|
| 0 | Mettre à jour les deux captures de référence (avant) ; lancer une partie complète ; noter ce qui plante | faible |
| 1 | Palette + `UiSprites` + réglages dans `GameConfig.UI` (aucun écran modifié) | faible |
| 2 | `TopBarView` | moyen (le `RefreshTopBar` existant est remplacé) |
| 3 | `TabBarView` | faible |
| 4 | Journal : cartes, filtres fixes, détail (corrige le chevauchement) | moyen |
| 5 | Panneaux Bilan, Équipe, Flux, Rivaux avec `UiCardView` | moyen (4 écrans) |
| 6 | `PortraitView`, `ProfileCard`, `PlaceCard` branchés sur les popups existants | moyen |
| 7 | Lisibilité à plusieurs résolutions (1080×1920, 720×1280, tablette, fenêtre PC) | moyen |
| 8 | Critères de la phase 3 : aucun texte coupé, navigation, données, compilation, aucune régression | — |

**Hors plan, à décider :** un correctif rapide du chevauchement du journal (cause 1) avant la refonte (≈ 10 lignes).

## 9. Risques techniques
1. **Unity 6.6 et l'entrée.** `StandaloneInputModule` et `Input.touchCount` (`MapViewport`) ne marchent que si « Active Input Handling » vaut Ancien ou Les deux. Si le projet est en « Input System Package » seul : les clics ne passent plus. Correction ciblée : `InputSystemUIInputModule` et `Touchscreen`.
2. **API obsolète.** `FindObjectOfType` est obsolète en Unity 6 (avertissement, puis suppression possible) : remplacer par `FindFirstObjectByType` (une ligne).
3. **Police intégrée.** `LegacyRuntime.ttf` convient, mais son rendu est peu soigné ; une police fournie est préférable (décision 4).
4. **Code jamais lancé.** Ombre, façades et hiérarchie n'ont jamais tourné : une erreur d'exécution peut apparaître dès que la fiche de quartier les affiche.
5. **Reconstruction complète des listes** à chaque rafraîchissement : à surveiller si les cartes deviennent riches (images). Solution : mise à jour ciblée.
6. **Orientation.** Le Canvas est portrait ; une fenêtre PC en paysage rend l'interface minuscule sans règle d'adaptation.
7. **Portraits en jpg sans transparence** et 2 manquants (`renseignement_2`, `finances_2`) : prévoir la silhouette de repli.
8. **`ProjectSettings` absent du dépôt** : la version Unity et ses réglages ne sont pas reproductibles. Proposition : les versionner.
9. **Régression des tests** : `tests/ui/smoke.py` suppose les noms d'objets actuels ; à mettre à jour avec chaque étape.

## 10. Décisions à arbitrer
1. **Journal :** correctif rapide maintenant, ou attendre l'étape 4 ?
2. **Durée du jeu :** campagne de 5 ans (état actuel) ou simulation de vie de 25 à 40 ans (brief) ? L'interface sera neutre dans les deux cas, mais la fin de partie et la progression de l'âge en dépendent.
3. **Plateforme :** portrait mobile seul, ou aussi une fenêtre PC en paysage ?
4. **Police :** garder `LegacyRuntime`, ou fournir une police libre de droits pour l'identité visuelle (laquelle ?).
5. **Priorités, relations, historique :** les afficher seulement quand les données existent (ma proposition), ou créer d'abord les données ?
6. **Navigation :** garder les 6 onglets (ma proposition), ou faire de la Carte l'accès principal avec un menu pour le reste ?
7. **Illustrations de lieux et portraits :** qui les produit, dans quel style, et à quel moment (les maquettes utilisent des silhouettes) ?
8. **Version Unity :** confirmer 6.6 et le réglage d'entrée, et versionner `ProjectSettings` ?
9. **Date du journal :** « Semaine 12 » seul, ou « Semaine 12 · Automne · Année 1 » ?

## 11. Garde-fous respectés
Aucun fichier supprimé, aucun système modifié, aucun changement de règles de jeu. Les textes des maquettes sont inventés, sans personne réelle, sans violence montrée, sans mineurs, sans technique opérationnelle.

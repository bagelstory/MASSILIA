# CHOUF : la hiérarchie du terrain (arrondissement, quartier, charbon)

*17 novembre 2026. Règles données par l'utilisateur, mises au propre. Remplace le découpage « 10 zones » de `10-bible-marseille.md` comme niveau de pouvoir. Idées de conception : rien n'est codé. Règle de contenu inchangée : on montre des rôles, des effectifs et des conséquences, jamais un mode d'emploi.*

## 1. L'emboîtement (décidé)
```
Marseille
 └─ Arrondissement (16)           un chef : le plus puissant de la zone
     └─ Quartier (111 officiels)  tenu par plusieurs personnes (plusieurs têtes, parfois plusieurs bandes)
         └─ Charbon               un point de vente, 5 postes (voir §2)
```
- Un **arrondissement** contient plusieurs **quartiers** (`QuarterData.arrondissement` existe déjà, 1 à 16).
- Un **quartier** est **contrôlé par plusieurs personnes**, pas par un seul propriétaire. Le contrôle se partage entre têtes et bandes.
- Un **quartier** contient **un ou plusieurs charbons** (`PoiData` de type Charbon).
- L'**arrondissement a un chef** : la personne **la plus puissante de la zone**. Il change quand le rapport de force change.

## 2. Le charbon : cinq postes (décidé)
| Poste | Nombre | Lien |
|---|---|---|
| **Guetteur** (le chouf) | **2** | attaché à **un seul** charbon |
| **Vendeur** (le charbonneur) | **1** | attaché à **un seul** charbon |
| **Gérant** | **1** | attaché à **un seul** charbon |
| **La tête** | **1** par charbon | **peut tenir plusieurs charbons** |

Soit **5 postes par charbon**. Règle de rattachement :
- **Guetteurs, vendeurs, gérants : un charbon, pas plus.** Ils ne bougent pas d'un charbon à l'autre.
- **La tête seule peut gérer plusieurs charbons**, jusqu'à une capacité (à fixer, départ : 4).
- Un charbon **sans tête** continue de tourner, mais sans protection : risque et pression en hausse.
- Un **poste vide** baisse le rendement et monte le risque (pas de guetteur = surprise, pas de gérant = désordre, pas de vendeur = pas de revenu).

## 3. La chaîne de pouvoir
`guetteur → vendeur → gérant → tête → chef d'arrondissement`
- On monte d'un cran quand un poste se libère (départ, arrestation, mort, trahison) et que la loyauté et la compétence suffisent.
- Le **chef d'arrondissement** n'est pas forcément une tête de charbon : c'est celui dont le **poids** est le plus fort (voir §5).
- Au-dessus, la bande ou le sommet à distance (`11`, l'Armateur) ne se voit pas dans la hiérarchie du terrain.

## 4. Où est le joueur ?
**DÉCIDÉ (17 nov.) : lecture A, le joueur est une tête qui monte.** Capacité d'une tête : **4 charbons** (valeur de départ, à simuler). Les deux lectures étudiées :
- **A. Le joueur est une tête**, qui monte jusqu'à chef d'arrondissement puis au-delà (la progression de `14` : équipier puis tête).
- **B. Le joueur dirige depuis l'arrondissement** et nomme les têtes, gérants, vendeurs et guetteurs à chaque charbon.

## 5. Le poids et le chef d'arrondissement (proposition de calcul)
- **Poids d'une personne dans un arrondissement** = somme, sur ses quartiers, de `contrôle du quartier × nombre de ses charbons ouverts` + bonus de loyauté de son équipe.
- **Chef d'arrondissement** = poids maximal. En cas d'égalité, celui qui a la plus forte réputation de Crainte (`14`).
- Quand le chef change, **tout l'arrondissement le sait** : une ligne de journal, une peur qui monte, des loyautés qui bougent.
- Un chef peut être **renversé** (arrestation, mort, rival plus puissant) ou **tomber** par usure, pas par une action de menu.

## 6. Conséquences à montrer (le réalisme, sans procédé)
- **Remplacer un guetteur est facile, remplacer une tête ne l'est pas.** Les premiers sont jetables (`11` §1), les têtes sont rares.
- **Une tête qui tient trop de charbons s'épuise** : fatigue, erreurs, trahison plus probable.
- **Les petites mains sont souvent très jeunes** : le jeu le dit comme une perte de la ville (jauge Enfance perdue), **jamais comme une ressource**. Les mineurs ne sont jamais jouables ni recrutables (règle fixe, `00-LISEZMOI.md`) : à l'écran, les postes de guetteur et de vendeur se remplissent avec des **adultes** ; l'existence de jeunes impliqués se raconte par les événements et les unes du journal, pas par une fiche de poste.
- **Un charbon qui tombe** montre ses cinq visages : arrêté, parti, mort, en fuite. Chaque poste a une ligne.
- **Un quartier « tenu par plusieurs personnes »** a des frictions : deux têtes qui se marchent dessus, une dette entre elles, une jalousie.

## 7. Ce que ça change dans le code (analyse)
| Existant | Aujourd'hui | Demain |
|---|---|---|
| `QuarterData.arrondissement` | existe (1-16), non utilisé pour le pouvoir | devient le niveau de pouvoir et de chef |
| `ZoneData` (10 zones) | niveau de regroupement et de déblocage | gardé pour le déblocage de la carte ; le **pouvoir** passe à l'arrondissement |
| `PoiData` (charbon) | un propriétaire (`ownerId`), pas de personnel | + 5 postes (`CrewSlot`), + une tête (`headId`) |
| `OperatorData` / `OperatorType` | équipe du joueur par spécialité (Repérage, Pression, Force…) | + **poste** (`Role`) et **charbon rattaché** (`poiId`) ; les spécialités deviennent des compétences |
| `QuarterData` contrôle | un seul chiffre pour le joueur | **liste de parts** par personne (`ControlShare`) |
| `FactionPresence` | présence par bande | reste, relié aux têtes |
| `ShadowSystem` / `FacadeSystem` | par quartier | inchangés ; les façades se rattachent à une tête |

**Nouveaux types à prévoir :** `enum CrewRole { Guetteur, Vendeur, Gerant, Tete, Chef }` (ajouter à la fin seulement), `struct CrewSlot { role, personId }`, `struct ControlShare { personId, share }`, `ArrondissementData { id, chiefId }`.
**Sauvegarde :** version 4, migration depuis la 3 (les opérateurs existants deviennent vendeurs/guetteurs d'un premier charbon).
**Ordre de résolution :** un `HierarchySystem` juste avant `EconomySystem` (postes vides, promotions, chef), l'économie lit ensuite les postes.

## 8. Questions ouvertes
1. ~~Rôle du joueur~~ **DÉCIDÉ : une tête qui monte** (apprenti, tête, chef d'arrondissement).
2. ~~Capacité d'une tête~~ **DÉCIDÉ : 4 charbons** (à simuler).
3. **Personnes dans un quartier :** combien de têtes au maximum se partagent un quartier (2 ? 3 ?) ?
4. **Un charbon sans tête :** autorisé (mais risqué), ou interdit ?
5. **Les postes sont-ils tenus par des personnages nommés** (avec mémoire, loyauté) **ou par des anonymes** au-dessous du gérant ? *Recommandation :* gérants et têtes nommés, guetteurs et vendeurs semi-anonymes (prénoms qui changent, cf. `11` §1).
6. **Arrondissement chef :** peut-il y avoir une règle de partage (un chef par arrondissement, mais plusieurs bandes dessous) ?

## 9. À coder (ordre conseillé)
1. `CrewRole`, `CrewSlot`, `ControlShare`, `ArrondissementData` (Data/), sauvegarde v4 et migration.
2. Rattachement des opérateurs à un charbon avec un rôle ; une tête peut en avoir plusieurs, pas les autres (`OperatorRules.Assign`).
3. `HierarchySystem` : postes vides, promotions, calcul du poids et du chef d'arrondissement.
4. Écrans : fiche d'un charbon (5 visages), fiche d'un arrondissement (chef, quartiers, têtes).
5. Brancher le rendement du charbon sur les postes (poste vide = rendement en baisse, risque en hausse).

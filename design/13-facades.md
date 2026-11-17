# CHOUF : les Façades (placer l'argent sale)

*17 novembre 2026. Prolonge « Placer l'argent » (`ShadowActions.Inject`, config « argent sale ») et `11-bible-ombre.md` §4. Idées, rien n'est codé. Chiffres à simuler sur 260 semaines.*

## 0. Limites
- **Aucun procédé.** Le jeu ne montre jamais *comment* l'argent devient propre : pas de montage, de facture, de société écran, de montant ni de seuil réel. Le joueur choisit un **secteur** et un **quartier**, puis lit les **conséquences**. Le comment reste hors champ.
- **Tout est inventé :** commerces, clubs, artistes, élus. Aucun club, label, enseigne ou sportif réel. Les lieux (quartiers) restent réels.
- **Aucune glorification :** chaque façade a un coût humain visible dans le quartier.
- **Mineurs :** les clubs de jeunes et l'école de foot sont des **victimes possibles** de la façade (équipes qui perdent leurs entraîneurs, enfants utilisés comme décor), jamais des leviers de gain.

## 1. Le principe
Dans la fiche d'un quartier, « Placer l'argent » devient un menu de **sept façades**. Chacune a :
- **un rendement** : combien d'exposition (argent sale) elle absorbe par semaine ;
- **un délai** : les premières semaines, rien ne se passe ;
- **un risque propre** (contrôle, rumeur, rival) ;
- **une trace dans le quartier** : ce que les habitants voient changer ;
- **une cassure** : l'événement qui la fait tomber.

Règle commune : une façade absorbe l'argent **sans jamais le rendre à zéro**, et plus le quartier en porte, plus la jauge Contamination monte (les commerces de toujours ferment, les habitants partent).

## 2. Les sept façades
| Façade | Absorption | Risque principal | Ce que voit le quartier | Ce qui la fait tomber |
|---|---|---|---|---|
| **L'alimentation de quartier** (alims) | faible, stable | contrôle d'hygiène, rival qui fait pression sur le gérant | la lumière reste allumée tard ; le vieux boulanger perd ses clients | un contrôle qui trouve « trop de rotation » |
| **Le salon de chicha** | moyenne | voisinage, plaintes, fermeture administrative | fumée, voitures garées en double file, musique | arrêté préfectoral de fermeture |
| **Le club de sport** (salle, boxe, foot de quartier) | forte à long terme | image : les parents retirent leurs enfants quand ça se sait | maillots neufs, tournoi du quartier | un ancien entraîneur parle à la presse |
| **Le label rap** (studio, clips, concerts) | forte, **instable** | un clip trop précis attire la police | murs repeints, un clip tourné dans la cage d'escalier | les textes servent de preuve ; l'artiste parle |
| **Le restaurant / snack** | moyenne | concurrence des enseignes réelles du centre | une terrasse en plus, des serveurs qui changent tout le temps | la comptabilité attire un contrôle |
| **Le garage / lavage auto** | faible | police routière, rival | voitures qui restent des semaines | la fourrière trouve ce qu'elle ne devait pas |
| **L'immobilier** (petit locatif, rachat d'immeubles) | très forte, **très lente** | marchands de sommeil, voisins, élus | des immeubles se vident, d'autres se rénovent | un effondrement ou un incendie met l'immeuble sur la place publique |

## 3. Effets sur les jauges existantes
- **Argent sale (exposition)** : −N par semaine par façade active (valeur propre à chaque secteur, dans `GameConfig.Facade`).
- **Ombre** : +1 par façade, +3 quand elle tombe.
- **Loyauté** : le gérant d'une façade gagne en loyauté, mais devient aussi un **témoin** (jauge Silence).
- **Contamination** : +1 par façade et par semaine dans le quartier, au-delà de 2 façades par quartier (limite pour éviter un empilement sans coût).
- **Revenu** : chaque façade rapporte un petit revenu propre, plus lent que le charbon.
- **Enfance perdue** : le club de sport et le label rap peuvent la faire baisser (une salle ouverte aux jeunes) *ou* monter (quand la façade tombe), selon un événement à choix.

## 4. Douze événements de façade (légende : A argent, P pression, C contrôle, L loyauté, Q quartier, F bande, Pe peur, S silence)
1. **L'alim contrôlée.** *Fermer et attendre* : A−, P−. *Soigner l'inspecteur* : P++ à retardement.
2. **La plainte de la chicha.** Les voisins signent. *Payer le bruit* : A−, Q+. *Ignorer* : fermeture, A−−.
3. **Le tournoi du quartier.** Q+, L+, mais les mères en parlent. S+.
4. **L'entraîneur qui part.** Il emmène ses gamins. Q−, Enfance perdue −.
5. **Le clip trop précis.** *Le couper* : L−. *Le laisser* : P++ dans 3 semaines.
6. **Le label qui s'envole.** Un artiste signe ailleurs. Le jeu note l'ironie : « Il a réussi, sans toi. »
7. **Le restaurateur qui veut sortir.** Il veut rendre les clés. *Le laisser* : A−. *L'en empêcher* : Pe+.
8. **L'immeuble qui se vide.** Les derniers locataires partent. Q−−, A+ (valeur du bien). Le jeu affiche le nombre d'habitants perdus.
9. **L'élu qui pose sur la photo.** C+, mais une trace. Une une du *Courrier des Docks* sort 4 semaines plus tard.
10. **Le contrôle fiscal.** Une façade « trop propre ». *La laisser tomber* : A−, T0. *La défendre* : P++.
11. **La concurrence.** Un rival ouvre en face. F+ ou A−.
12. **La façade qui tombe.** Elle ferme, le quartier reste avec le vide : une vitrine éteinte, une salle sans lumière. Pas de choix, juste une semaine de silence.

## 5. Voix (texte court)
- **Gérant d'alim** : *« Je vends du lait. Le reste, je sais pas. »*
- **Coach** : *« Les gamins, je les garde. L'argent, je le regarde pas. »*
- **Manager rap** : *« Le clip, c'est du cinéma. Sauf que le décor, c'est chez toi. »*
- **Vieux commerçant** : *« Quarante ans que je tiens. Eux, ils ouvrent, ils ferment, ils sont partout. »*
- **Mère du collectif** : *« Un club qui ouvre pour pas faire de sport, ça se voit. »*

## 6. À coder
1. `GameConfig.Facade` (7 façades : absorption, délai, risque, revenu) et une structure `FacadeData` dans `ShadowData` (liste par quartier, **version de sauvegarde 3**).
2. Le menu « Placer l'argent » de `QuarterDetailPopup.Shadow.cs` : liste des façades disponibles, limite de 2 par quartier.
3. Un `FacadeSystem` qui s'exécute juste après `ShadowSystem` (absorption, risque, événements).
4. Les 12 événements, après relecture.

## 7. À vérifier
- Noms inventés : aucun label, club ou enseigne ne doit correspondre à une structure réelle.
- Le rap : éviter tout rapprochement avec un artiste réel. Le jeu ne montre jamais *un genre* comme un faux-semblant criminel : seul **ce label inventé** est une façade, la scène rap reste montrée comme une scène vivante (voir un événement positif en contrepoint).
- Relecture externe du texte avant publication.

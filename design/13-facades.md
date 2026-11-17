# CHOUF : les Façades (placer l'argent sale)

*Version 2, 17 novembre 2026. Remplace la v1. Document autonome : une IA ou une personne qui n'a rien lu d'autre peut le reprendre. Contexte et conventions dans `00-LISEZMOI.md`. Idées de design : rien n'est codé. Tous les chiffres sont des points de départ à simuler sur 260 semaines.*

## 0. Limites (valent pour tout le dossier)
- **Pas de notice.** Le jeu ne décrit jamais la technique : pas de montage, de pièces comptables, de sociétés, de montants réels, de seuils légaux, ni de manière d'éviter un contrôle. Le joueur choisit **un secteur, un quartier, un gérant**. Le moteur tire le résultat, le texte montre les **conséquences**. Le réalisme vient de ce que ça coûte, pas d'une procédure.
- **Tout est inventé :** commerces, clubs, artistes, élus, journal. Les quartiers sont réels. Aucun club, label, enseigne, sportif ou élu réel.
- **Mineurs :** victimes ou décor subi, jamais jouables, jamais un gain.

## 1. Le principe de réalisme : la plausibilité
Une façade n'absorbe pas ce qu'on veut : **elle absorbe ce que son quartier peut croire**. D'où une règle centrale, qui reste de la pure simulation :

- Chaque façade a une **capacité plausible** : elle dépend de la **taille réelle du quartier** (population, passage), du **type de commerce** et de son **ancienneté**.
- Au-dessus de cette capacité, la façade « fait trop propre » : la jauge **Éveil** (du quartier, de la mairie, de la presse) monte chaque semaine.
- Au-dessous, elle rapporte peu mais dure.
- La capacité **augmente avec le temps** (un commerce qui existe depuis des années est plus crédible qu'un commerce neuf) et **baisse** quand un concurrent réel ferme ou ouvre en face.

Le joueur apprend donc la même chose que dans la vie : **le tuyau est toujours trop petit pour le flot.** C'est le vrai plafond du jeu.

## 2. Les façades (secteurs)
Chaque ligne est une fiche de données (`GameConfig.Facade`). `Absorption` = pourcentage de l'exposition qu'une semaine peut retirer, **à plein régime plausible**. Les valeurs sont des repères relatifs (1 = alim), pas des montants.

| Secteur | Absorption | Délai avant effet | Croissance de la capacité | Éveil typique | Revenu propre |
|---|---|---|---|---|---|
| **Alimentation de quartier** | 1 | 2 sem. | lente, stable | contrôle sanitaire | très faible |
| **Salon de chicha** | 2 | 3 sem. | moyenne | plaintes, arrêté | faible |
| **Garage / lavage auto** | 1 | 2 sem. | lente | fourrière, voisins | faible |
| **Snack / restaurant** | 2 | 4 sem. | moyenne | contrôles, avis en ligne | moyen |
| **Club de sport** (salle, boxe, foot) | 3 | 8 sem. | forte à long terme | parents, subvention, presse | faible |
| **Label rap** (studio, clips) | 4 | 6 sem. | **imprévisible** | clips, textes, concerts | moyen, en dents de scie |
| **Immobilier** (rachat, locatif) | 6 | 16 sem. | très forte, très lente | élus, marchands de sommeil, incendie | élevé à long terme |

**Contraintes de jeu :**
- 2 façades maximum par quartier. Au-delà, le quartier est « saturé » : chaque semaine, Éveil +3.
- Une façade ne se ferme pas d'un clic : la liquider prend 4 semaines et coûte un tiers de ce qu'elle a absorbé.
- Une façade a un **gérant** (un personnage, adulte) : sa loyauté, ses dettes, sa famille comptent. S'il tourne mal, la façade tourne mal.

## 3. Effets sur le reste du jeu
- **Argent sale (exposition)** : baisse selon l'absorption, plafonnée par la capacité plausible.
- **Éveil** (nouvelle jauge, par quartier, 0-100) : montée quand on dépasse la capacité, quand un événement tombe, ou quand un rival souffle. À 60, des événements de contrôle apparaissent. À 90, un coup de filet est possible.
- **Ombre** : +1 par façade active, +3 quand une façade tombe.
- **Contamination du quartier** : +1 par semaine par façade au-delà de la première. Les commerces de toujours ferment, la population baisse (règle déjà codée).
- **Loyauté / Silence** : le gérant gagne en loyauté, mais devient un témoin possible (jauge Silence).
- **Enfance perdue** : les clubs de sport et le label peuvent la faire baisser (salle ouverte, studio gratuit) ou la faire monter quand la façade tombe. Le jeu montre les deux faces.

## 4. La vie réelle autour de chaque secteur (matière pour les événements et les textes)
Ce qui suit est du **texte d'ambiance et de conséquences**, pas du mode d'emploi.

- **Alim.** Le gérant dort dans l'arrière-boutique. Les voisins notent que la caisse ne désemplit pas alors que personne n'achète. Le vieux boulanger d'en face perd ses clients, puis ferme. L'inspection sanitaire est un rituel annuel que tout le monde connaît.
- **Chicha.** Fumée, musique tard, voitures en double file. Les plaintes de voisinage sont la vraie ligne de front : la préfecture signe un arrêté, le tribunal administratif met des mois, le salon rouvre sous un autre nom. Les jeunes y traînent ; les parents le savent.
- **Garage / lavage.** Les mêmes voitures restent des semaines. Un mécanicien sérieux perd son atelier parce qu'on a racheté le bail. La fourrière est un lieu de scénario.
- **Snack / resto.** L'enseigne change souvent de nom. Le serveur de nuit ne connaît jamais le patron. Les avis en ligne sont le premier juge : une cliente qui parle trop, une photo qui traîne.
- **Club de sport.** Un vrai club de quartier vit de bénévoles, de subventions et de gamins. Une façade le remplace par des maillots neufs et un tournoi. L'ancien entraîneur, qui connaît chaque famille, est le témoin que tout le monde craint. Quand le club tombe, ce sont les enfants qui n'ont plus de salle.
- **Label rap.** La scène rap est **vivante et légitime**, et le jeu la montre comme telle (voir 7). Le label inventé, lui, est une façade : studio, clips tournés dans le quartier, concerts. Le danger, c'est que **le texte et l'image sont des preuves**. Un artiste qui signe ailleurs, c'est un départ, pas une trahison. Un clip qui nomme trop de choses attire les enquêteurs.
- **Immobilier.** Marchands de sommeil, copropriétés vidées, travaux jamais faits, ascenseur en panne depuis deux ans. Un immeuble qui se vide fait monter la valeur du terrain. Quand l'immeuble tombe, le coût humain devient public : le jeu affiche le nombre d'habitants et de familles déplacées. La rue d'Aubagne reste **hors du jeu**.

## 5. Les acteurs autour (personnages inventés, adultes)
- **Le gérant** de chaque façade (un par façade, tiré au sort dans une banque de 20 noms).
- **L'inspecteur** (sanitaire, fiscal) : sensible, ou corruptible, ou les deux. Le jeu ne le dit jamais.
- **L'élu** de l'arrondissement : on le voit sur une photo, jamais dans un acte.
- **Le journaliste local** : écrit pour le *Courrier des Docks* (journal inventé). Il apparaît quand l'Éveil dépasse 50.
- **La Mère du collectif** : elle remarque avant les autres.
- **Le concurrent honnête** : un petit commerçant qui résiste. S'il ferme, le jeu l'affiche.

## 6. Quinze événements de façade
Légende : A argent, P pression, C contrôle, L loyauté, Q quartier, F bande, Pe peur, S silence, **É** Éveil, **Of** Enfance perdue.

1. **L'alim contrôlée.** *Laisser faire* : É+, une semaine de pression. *Lisser* : A−, É− (à court terme), L− du gérant.
2. **La chicha et la plainte.** Les voisins signent. *Réduire les horaires* : A−, Q+. *Ignorer* : arrêté, façade gelée 6 semaines.
3. **Le rouleau compresseur.** Un concurrent ouvre en face. *Le racheter* : A−−, Q−. *Le laisser* : É+ (un commerce voisin souffre).
4. **Le tournoi.** Q+, L+, mais S+ (les mères en parlent).
5. **L'entraîneur qui part.** Il emmène les enfants. Q−, **Of −**.
6. **Le clip trop précis.** *Le couper* : L−. *Le laisser* : É++ dans 3 semaines.
7. **Le label perd son artiste.** L'artiste signe ailleurs. Le jeu écrit : « Il a réussi sans toi. »
8. **Le gérant veut sortir.** *Le laisser partir* : A−. *L'en empêcher* : Pe+, S+.
9. **L'immeuble qui se vide.** Q−−, valeur +. Le jeu liste les habitants partis.
10. **L'élu sur la photo.** C+, mais une trace : une une du journal sort 4 semaines plus tard (É+).
11. **Le contrôle fiscal.** *Abandonner la façade* : A−, É remis à zéro. *La défendre* : É++.
12. **L'avis qui traîne.** Un client laisse un commentaire dérangeant. *Le laisser* : É+ . *Le faire retirer* : le jeu note que tu l'as fait, É++.
13. **L'ancien du quartier** vient te parler. Il ne demande rien. Q− si tu l'ignores, L+ si tu l'écoutes.
14. **L'incendie.** Un local brûle (accident ou pas, le jeu ne le dit jamais). Les voisins sont évacués. A−, Q−−, Of+.
15. **La façade qui tombe.** Elle ferme. Une vitrine éteinte, une salle sans lumière, une semaine de silence sans choix.

## 7. Contrepoids : ce que le jeu doit montrer aussi
- **Le vrai commerçant, le vrai club, la vraie scène** existent à côté des façades. Le jeu doit les afficher en parallèle (événements positifs : « le club du quartier gagne son tournoi, sans toi »). Sinon le joueur croit que tout est façade.
- **La vitrine de rap** n'est jamais *le rap*. Un événement par saison montre la scène vivante, sans lien avec le joueur.
- **Le coût humain est lisible :** chaque façade qui tombe affiche ce que le quartier perd, en une ligne.

## 8. Voix (texte court)
- **Gérant d'alim** : *« Je vends du lait. Le reste, je sais pas. »*
- **Coach** : *« Les gamins, je les garde. L'argent, je le regarde pas. »*
- **Manager rap** : *« Le clip, c'est du cinéma. Sauf que le décor, c'est chez toi. »*
- **Vieux commerçant** : *« Quarante ans que je tiens. Eux, ils ouvrent, ils ferment, ils sont partout. »*
- **Mère du collectif** : *« Un club qui ouvre pour pas faire de sport, ça se voit. »*
- **Locataire d'un immeuble racheté** : *« On nous a dit que c'était provisoire. Ça fait deux ans. »*

## 9. À coder (ordre conseillé)
1. `GameConfig.Facade.cs` : une fiche par secteur (absorption, délai, croissance, plafonds), validée dans `GameConfig.Validation`.
2. `Data/FacadeData.cs` : `FacadeData{sector, quarterId, managerId, age, absorbed, eveil}` ; liste dans `ShadowData` ; **`CurrentVersion = 3`** + `SaveManager.Migrate`.
3. Champ `eveil` dans `QuarterData`.
4. `Systems/FacadeSystem.cs` (juste après `ShadowSystem` dans l'ordre des systèmes) : absorption plafonnée par la capacité, croissance, Éveil, événements.
5. Menu des façades dans `UI/QuarterDetailPopup.Shadow.cs` (remplace le bouton « Placer l'argent »).
6. Les 15 événements (`Resources/Data/events-facades.json`).

## 10. À vérifier
- Noms inventés : aucun label, club, enseigne ou journal ne doit correspondre à une structure réelle.
- Simulation 260 semaines : la plausibilité empêche-t-elle une stratégie dominante (tout en immobilier) ? L'Éveil est-il trop lent ou trop rapide ?
- Relecture externe, et relecture locale du ton.

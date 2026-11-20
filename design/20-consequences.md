# CHOUF : les conséquences sur 25 à 40 ans

*20 novembre 2026. Rédigé par la fonction « équipe narrative 2 ». **Idées de conception : rien n'est codé.** Le principe est celui de `11` §0 : on montre ce que ça coûte, jamais comment on fait. Aucune scène ne détaille un procédé, aucune ne glorifie. Les valeurs sont des **boutons de réglage** (tuning knobs) à calibrer par tests ; les noms entre `accents graves` sont des propositions pour `GameConfig`. Cadre : vie de 18 à 43 ans (jusqu'à 58 ans en option), actes de 5 ans (`18` §4), temps élastique possible (`18` §4.2).*

## 0. Principe : la dette de la vie, pas la dette de l'argent
Chaque choix du joueur laisse une **trace** qui ne disparaît pas quand la partie avance : une condamnation, un corps usé, une famille distante, un quartier qui sait. Huit familles de conséquences, chacune avec une jauge, une durée et des cascades. Elles se **croisent** : la prison use le corps, l'usure abîme la famille, la famille fait baisser la Loyauté, la Loyauté change la rue.

| Jauge (abrégé) | Plage | Où elle vit | Remarque |
|---|---|---|---|
| **Casier** (`Cas`) | 0-100 | joueur et chaque personnage | monte par palier, **ne baisse presque jamais** |
| **Usure** (`Usu`) | 0-100 | joueur et chaque personnage | corps et sommeil ; baisse seulement au repos |
| **Foyer** (`Foy`) | 0-100 | joueur | solidité de la vie privée (couple, mère, enfants) |
| **Réputation** (`Rép`) | −100 à +100 | **par quartier** | qui parle de toi, et comment |
| **Dette** (€/semaine) | ≥ 0 | joueur | s'ajoute à la dette de Lucien (`14`) |
| **Voisinage** (`Voi`) | 0-100 | **par quartier** | tension avec les riverains |
| **Double vie** (`Dua`) | 0-100 | joueur | écart entre l'argent qui circule et la vie visible |
| **Exposition des proches** (`Exp`) | 0-100 | par proche | à quel point un proche paie pour toi |

**Règle d'équilibre :** aucun gain n'est « gratuit ». Tout palier de réussite (Territoire, Ombre, argent) **monte au moins une jauge de conséquence** dans les 4 à 12 semaines qui suivent. C'est le pilier de `13` et `11` (« chaque gain se paie ailleurs »).

## 1. Justice : garde à vue, parloir, procès, peines, sursis
**Déclencheurs :** Pression policière ≥ 60 sur un quartier (`10` §4.1), perquisition, dénonciation, Éveil d'une façade (`13`), arrestation d'un équipier qui parle, Dette de sang (`11` §4).
**Chaîne :** contrôle → **garde à vue** (48 h, une semaine de jeu) → mise en examen ou classement → détention provisoire éventuelle → **procès** → peine ferme, **sursis**, aménagement, ou relaxe.

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `justiceGavSeuilPression` | 60 | 40-90 | Pression qui rend une garde à vue possible |
| `justiceGavDuree` | 1 semaine | 1-2 | durée de jeu ; l'équipe est gelée |
| `justiceCasierGav` | +4 `Cas` | 0-10 | trace d'une garde à vue sans suite |
| `justiceCasierSursis` | +15 `Cas` | 5-30 | condamnation avec sursis |
| `justiceCasierFerme` | +30 `Cas` | 15-50 | condamnation ferme |
| `justiceSursisDuree` | 156 sem. (3 ans) | 52-260 | tout nouveau fait pendant le sursis le rend **ferme** |
| `justicePeineCourte` | 26-104 sem. | | peine ferme jusqu'à 2 ans |
| `justicePeineLongue` | 156-520 sem. | | selon `Cas` (plus il est haut, plus la peine monte) |
| `justiceParloirTaux` | 1 visite/2 sem. | 0-1 | visites possibles ; chaque visite : +3 `Foy`, −2 `Usu` |
| `justiceEmploiApres` | −30 % emplois « normaux » | 0-60 | pénalité de retour à la vie normale par palier de `Cas` |

**Cascades :**
- Détention : −`Usu` récupérée (le corps tient mal) mais **+6 `Usu`/an** ; l'Ordre (`11` §4) arrive avec retard ; la Loyauté des équipiers baisse de 1 par mois sans nouvelles.
- Parloir (`11`, mode Parloir) : le jeu continue depuis la cellule ; **la famille voyage** : `Exp` des proches +2/visite.
- Sortie : −20 % revenus la première année, mais **+10 Loyauté** de qui t'a attendu.
- Sursis : chaque Pression ≥ 70 pendant le sursis ouvre un événement « le sursis saute ? » (`22` E14).

**Journal (3 textes) :**
- « Garde à vue levée à 6 h. Rien contre toi, pour l'instant. Le mot "pour l'instant" reste dans la pièce. »
- « Dossier renvoyé en correctionnelle. L'avocate parle de trois ans, dont un ferme. Ta mère demande l'heure du train. »
- « Tu sors un mardi, avec un sac plastique. La ville a continué sans toi, et sa carte a changé. »

## 2. Santé et corps : usure, blessures, addiction des autres, deuil
**Déclencheurs :** semaines de tension continue (Pression, Ombre), peu de repos, âge, prison, blessures liées à la guerre de bandes (**jamais mises en scène**, seulement leurs suites), consommation de proches, mort d'un proche.
**Principe :** l'**Usure** monte avec le temps et la tension, descend au repos ; au-dessus de seuils, le corps impose des choix (soins, repos, perte de compétence). On ne détaille aucun acte violent ; on montre **une radio, un fauteuil, une ordonnance**.

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `santeUsureAnnuelle` | +3 `Usu`/an (+1 après 35 ans) | 1-6 | usure de base |
| `santeUsureTension` | +0,05 × (Pression − 40)/sem. | | usure par tension |
| `santeRepos` | −2 `Usu`/sem. si une semaine « calme » | 0-5 | récupération |
| `santeSeuilGene` | 50 | 30-70 | −10 % rendement du joueur et de sa tête |
| `santeSeuilAlerte` | 75 | 60-90 | événement médical possible (`22`) |
| `santeBlessureConsequence` | −20 compétence, 26 sem. | | suite d'un fait grave, sans le montrer |
| `santeAddictionProche` | proba 0,02/sem. par proche exposé | 0-0,1 | un proche s'abîme (substance, alcool, jeu) |
| `santeDeuil` | −15 `Foy`, −10 compétence, 13 sem. | | mort d'un proche |
| `santeSoin` | −8 `Usu` pour 600 € | | consultation, repos médical |

**Cascades :**
- `Usu` ≥ 75 → erreurs de gestion (la prévision se trompe de ±15 %) ; la Priorité **Stabilité** monte, **Ambition** descend.
- Addiction d'un proche : coûte en €/sem. (voir §5), monte `Exp` ; **le jeu n'illustre jamais la consommation**, seulement les conséquences (retard, silence, appel d'urgence, médecin).
- Deuil : débloque l'événement « les obsèques » (tout le monde vient, les rivaux aussi) ; la Loyauté des proches bouge dans les deux sens.

**Journal :**
- « Le médecin dit repos. Tu entends : trois semaines. Le quartier entend : une absence. »
- « Ton cousin n'est pas venu au repas. Son téléphone sonne dans le vide. Sa mère dit qu'il est fatigué. »
- « La messe de neuf heures est pleine. Il y a des gens que tu ne verras plus jamais au même endroit. »

## 3. Famille : mère, partenaire, enfants, école
**Déclencheurs :** âges clés (30 ans = « le tournant », `18` §4.3), absences, argent visible, dispute, maladie de la mère (Dolorès, `18` n° 13), naissance, convocation à l'école, séparation.
**Principe :** la **Priorité Famille** du joueur et celle de ses proches décide ce qui reste. Les **enfants du joueur** n'apparaissent qu'en **vie ordinaire** (repas, école, maladie) ; ils ne sont **jamais** dans une scène de gestion ni de danger.

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `familleFoyerDepart` | 60 | 20-90 | solidité de départ |
| `familleAbsence` | −1 `Foy`/sem. si 4 semaines sans temps privé | 0-3 | usure du lien |
| `familleRepas` | +3 `Foy` par repas partagé | 1-6 | action « temps privé » (1 des 3 actions de la semaine) |
| `familleMereMalade` | début vers 28 ans, coût 300 €/mois | 100-800 | mère malade (`18` n° 13) |
| `familleNaissanceFoy` | +10 `Foy`, +120 €/sem. de dépenses | | naissance (événement, 30+) |
| `familleDepartPartenaire` | `Foy` < 25 pendant 26 sem. | | le partenaire part ; −20 Stabilité |
| `familleEcoleConvoc` | proba 0,04/sem. | 0-0,1 | l'école convoque ; voir `22` E22 |
| `familleHonte` | −10 Loyauté (de la mère) si argent visible > seuil | | « d'où ça vient » |

**Cascades :** `Foy` ≥ 70 : −10 % d'`Usu` perdue (une maison où l'on dort) ; `Foy` < 30 : la Priorité Liberté monte (envie de partir), la Loyauté descend. Un enfant né pendant la vie fait monter **Stabilité** de +15 et modifie le seuil de « sécuriser puis partir ». À l'Acte IV, les enfants des personnages ont l'âge de « choisir » : **aucun enfant n'est jamais jouable ni mis en danger à l'écran** ; la perte passe par l'écran « Ce qu'ils sont devenus » (`18` §4.4).

**Journal :**
- « Ta mère a lavé ta chemise. Elle a mis la facture dans la poche, pliée en quatre. Elle ne dit rien. Elle a tout dit. »
- « Dîner à la maison. Le téléphone est sur la table, face contre bois. C'est déjà un progrès. »
- « Réunion à l'école. La maîtresse a une voix douce. Tu comprends qu'elle te connaît par le quartier. »

## 4. Réputation de quartier
**Déclencheurs :** actes publics (aide, don, présence aux enterrements, favoritisme), peur, conflits visibles, discrétion, scandales, silence.
**Principe :** `Rép` va de −100 (craint, haï) à +100 (aimé, protégé). La **peur** donne du contrôle vite mais fait baisser `Rép` ; le respect donne du temps long (`10` §4.4).

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `repDecay` | −1/sem. vers 0 si inactif | 0-3 | les gens oublient |
| `repAide` | +6 | 2-12 | aide visible (loyer, mariage, cantine) |
| `repPeur` | −8 `Rép`, +10 contrôle | | acte d'intimidation (peur, pas de détail) |
| `repEnterrement` | +5 si présent, −10 si absent | | présence aux obsèques d'un habitant |
| `repSeuilTemoin` | `Rép` > 40 | | les habitants **se taisent** pour toi |
| `repSeuilDelation` | `Rép` < −40 | | les habitants parlent aux enquêteurs (+Pression) |

**Cascades :** `Rép` > 40 : Pression −1/sem. et recrutement plus facile (+ Loyauté initiale) ; `Rép` < −40 : revenus −20 % et boycott (commerces qui ferment, `13`). Le chef d'arrondissement (`15` §5) pèse aussi sur `Rép`.

**Journal :**
- « Madame Zohra t'a salué depuis son balcon. Ce n'est pas rien. »
- « Le boulanger a changé de trottoir en te voyant. C'est la boulangerie qui perd. »
- « Une fresque est apparue sur le mur. Tu n'y es pas. Mais ta rue, oui. »

## 5. Dettes
**Déclencheurs :** dette de Lucien (`14`), prêts de survie, frais d'avocat, soins, un proche qui s'abîme, cautions, amendes.
**Principe :** l'argent a une **valeur de vie** : chaque euro prêté est une personne qui t'attend ; le jeu ne parle jamais de menaces nominales, seulement d'un **solde qui monte** et d'une **échéance**.

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `detteTaux` | 0,5 %/sem. | 0,1-1,5 | intérêt (dette personnelle) |
| `detteLucien` | 18 000 € | 5 000-50 000 | dette héritée (à confirmer avec `14`) |
| `detteAvocat` | 1 500-6 000 € | | frais de défense (par affaire) |
| `detteProche` | 40-200 €/sem. | | un proche en difficulté |
| `detteSeuilAlerte` | 25 000 € | 10 000-80 000 | ouvre l'événement « créancier » (`22`) |
| `detteRemboursementQuota` | 10 % du revenu | 5-30 | remboursement automatique |

**Cascades :** dette > seuil : −1 Liberté/mois (tu n'es plus libre de partir) et les décisions de Loyauté se tendent ; une dette **remboursée** donne +10 `Rép` auprès du créancier, +10 `Foy` si c'était un proche.

**Journal :**
- « Le solde a bougé de 400 €. Ce chiffre n'a pas de visage, mais il a un nom, et tu le connais. »
- « L'avocate envoie sa note. Elle est polie. Le montant, lui, ne l'est pas. »
- « Tu rembourses sans qu'on te le demande. Ça change la manière dont on te regarde dans la rue. »

## 6. Voisinage
**Déclencheurs :** bruit, passage, nuisances visibles autour d'un charbon ou d'une façade, plaintes, pétition, rencontre de quartier.
**Principe :** `Voi` monte avec les **signes visibles** (attroupement, odeurs, éclats), descend avec la **présence utile** (réparations, aide, travaux).

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `voiBruit` | +2/sem. par charbon actif sans guetteur discret | 0-5 | nuisances |
| `voiPlainte` | proba = `Voi`/400 par sem. | | plainte aux services |
| `voiPetition` | `Voi` > 60 | | pétition d'habitants (Pression +5) |
| `voiReparation` | −8 `Voi` pour 800 € | | ascenseur, éclairage, aire de jeux |
| `voiMediation` | −12 `Voi` | | réunion avec les locataires |

**Cascades :** `Voi` > 70 : les habitants **orientent** la police (Pression +4) ; `Voi` < 20 : un **réseau d'alerte** d'habitants te protège (−3 Pression) mais **te coûte** (attentes, favoritisme).

**Journal :**
- « Affiche collée dans le hall : réunion des locataires jeudi 18 h. Venez, même fatigués. »
- « L'ascenseur est réparé. Personne ne te remercie. Quelqu'un laisse un panier de figues devant ta porte. »
- « Pétition : 42 signatures. Tu en connais 41. »

## 7. Argent sale contre vie normale
**Déclencheurs :** revenus du réseau, façades (`13`), train de vie, dépenses visibles, achats, travail « normal » à côté.
**Principe :** la **Double vie** mesure l'**écart** entre l'argent qui circule et ce qu'on peut expliquer. Plus l'écart est grand, plus la vie normale est fragile (Stabilité), plus les enquêteurs (Prisca Ndoye, `18` n° 19) s'y intéressent. **Aucun procédé** : on ne montre jamais comment l'écart se réduit, seulement que la façade (`13`) tient ou craque.

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `duaEcartMonte` | +1/sem. pour 2 000 € de revenu non expliqué | | monte avec l'argent visible |
| `duaEcartBaisse` | −1/sem. par façade crédible (`13`) | | descend si la vie visible suit |
| `duaSeuilEnquete` | 60 | 40-85 | une enquête financière s'ouvre (Éveil +) |
| `duaSeuilRuine` | 90 | | la vie normale s'effondre (compte gelé, logement perdu) |
| `duaTrainVie` | −`Dua` 6 si le joueur renonce à une dépense visible | | choix de sobriété |
| `duaRevenuNormal` | +300 €/sem. | 100-700 | un emploi réel à côté ; −30 % de revenu du réseau si pris |

**Cascades :** `Dua` ≥ 60 : Prisca Ndoye gagne un point de **Menace** par semaine (`22` E20), les proches voient la différence (`Exp` +1). Un emploi normal fait baisser `Dua` et monte Stabilité mais coûte de l'**Ambition**.

**Journal :**
- « Tu as payé comptant. Le vendeur te regarde un peu plus longtemps qu'il ne faut. »
- « Ton voisin a vu la voiture. Il a vu la voiture d'avant aussi. »
- « Tu as un vrai contrat de travail. Ça ne rapporte pas grand-chose. Ça te laisse dormir. »

## 8. Retombées sur les proches
**Déclencheurs :** tout ce qui précède, quand il touche quelqu'un d'autre : une perquisition chez la mère, un équipier arrêté et sa famille, une réputation qui éclabousse.
**Principe :** chaque proche a une **Exposition** (`Exp`) : plus elle monte, plus il paie à ta place (emploi perdu, harcèlement, éloignement, santé).

| Bouton | Départ | Plage | Sens |
|---|---|---|---|
| `expMontee` | +2/sem. si `Dua` > 50, +4 si perquisition | | exposition |
| `expSeuilEloignement` | 60 | 40-80 | le proche prend ses distances |
| `expSeuilRupture` | 85 | | le proche coupe (−30 `Foy`) |
| `expSoutien` | −10 `Exp` pour une aide visible | | tu le protèges |
| `expMarque` | +10 `Exp` définitif si le proche est cité dans un article | | le nom reste |

**Cascades :** un proche exposé peut devenir **témoin**, **protégé**, ou **absent**. Les trois fins de la famille (`18` §4.5, « La Famille d'abord ») dépendent de l'`Exp` moyen des proches.

**Journal :**
- « Ta sœur a été licenciée. Rien n'est dit, mais le chef de rayon connaît ton prénom. »
- « Perquisition à l'aube. Ta mère a fait du café aux policiers. Elle a fait du café à tout le monde, toute sa vie. »
- « Ton neveu ne dit plus qui est son oncle à l'école. Il ne ment pas. Il se tait. »

## 9. Calendrier des conséquences (par acte)
| Acte | Âge | Dominante | Jauge qui pèse |
|---|---|---|---|
| I « Cinq ans de dette » | 18-23 | survie, dette, premiers pas | Dette, `Voi` |
| II « Ce qu'on tient » | 23-28 | pouvoir, premières peines | `Cas`, `Rép` |
| III « La ville qui change » | 28-33 | façades, enquêtes, famille | `Dua`, `Foy` |
| IV « L'héritage » | 33-38 | usure, succession | `Usu`, `Exp` |
| V-VIII (option) | 38-58 | transmission, bilan | toutes ; **l'écran « Ce qu'ils sont devenus »** |

## 10. Ce que le jeu ne fait pas
- Il n'explique **aucun procédé** (ni de trafic, ni de dissimulation, ni de violence).
- Il ne montre **aucun mineur** dans une scène de gestion, de danger ou de violence.
- Il ne fait **jamais gagner** sans coût visible.
- Il ne met pas en scène la **consommation**, seulement ses suites.
- Il ne cite **aucune affaire réelle** (pas de rue d'Aubagne, pas de nom de victime réelle).

## 11. À régler par tests
1. L'échelle de `Cas` : un seul palier ferme suffit-il à changer toute la partie ?
2. L'usure à 35 ans : trop tôt ou trop tard ?
3. Le poids de `Foy` contre celui d'Ombre : le joueur peut-il « tout garder » ? (il ne devrait pas).
4. La fréquence des événements familiaux (une scène par an maximum au départ).
5. Compatibilité avec le temps élastique : les jauges avancent par semaine en arrière-plan.

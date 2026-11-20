# CHOUF : brainstorming n° 3 (mécaniques, ton, risques, questions)

*20 novembre 2026. Équipe narrative 2. Suite de `BRAINSTORM.txt` et de `17b`. **Idées en vrac, à trier : rien n'est décidé ni codé.** Principe inchangé : montrer le prix, jamais le procédé ; adultes seulement à l'écran ; lieux réels, personnes fictives. L'OM (club réel) est autorisé comme **décor de ville** uniquement (jamais lié à un acte). Rue d'Aubagne : hors du jeu.*

## 1. Vingt-cinq idées
### Mécaniques de vie (sur 25 à 40 ans)
1. **Le carnet de bord de l'année.** Chaque année se clôt sur une page : trois choix qui ont compté, une perte, un souvenir. Le joueur la relit à la fin (le générique).
2. **Trois actions par semaine** (travail, rue, privé) : on ne peut pas tout faire. Le « temps privé » est une action à part entière (`20` §3).
3. **L'agenda de la mère.** Dolorès appelle à des dates fixes (anniversaire, dimanche). Rater trois appels de suite a un coût qu'on lit dans sa voix.
4. **Le journal de santé.** Une jauge d'Usure par personnage important, visible sur la fiche (`16` maquette E). On voit les cernes arriver.
5. **L'argent a une couleur.** Deux comptes de lecture (l'argent de la vie, l'argent de la rue) et un curseur « Double vie » ; jamais comment on les rapproche.
6. **Les anniversaires.** Tous les cinq ans, un bilan : « Qui est encore là ? » avec les portraits.
7. **La liste des absents.** Un mur dans l'écran de fin qui recense les gens que le joueur a perdus, par ordre d'apparition.
8. **Les saisons de vie.** Chaque acte a un palette-ton : Acte I (aigu), II (chaud), III (gris), IV (clair-obscur) ; la palette de `19` évolue avec l'âge du joueur.
9. **La mémoire du quartier.** Chaque quartier garde 3 souvenirs de toi (une aide, une peur, un silence) qui ressortent dans les textes des années plus tard.
10. **La clé de la mère.** Un objet (une clé d'appartement) que le joueur garde ou rend ; il change le dernier acte.

### Mécaniques de ville et de rues
11. **La météo qui pèse.** Le mistral augmente la Pression (+3) et baisse les sorties (−20 % de fréquentation) ; la canicule monte `Voi`. Déjà prévu dans `17` §1 ; à relier aux jauges de `20`.
12. **Les jours de match.** Les soirs de match (club réel, **décor seulement**), le bruit de la ville monte, la Pression baisse parce que tout le monde regarde ailleurs ; ne jamais lier un acte à un résultat ni à un club.
13. **La carte qui vieillit.** Les bâtiments de la carte (3D plus tard) montrent l'âge : un quartier rénové, un autre dégradé, selon `Voi` et la gentrification (`13`).
14. **L'écho des voisins.** Les Madame Zohra et Odile Ravel sont des « capteurs » de `Voi` : leurs répliques changent selon la jauge.
15. **La tournée du marché.** Un écran de marché (les Capucins, le Prado) où l'on croise des civils et où l'on prend le pouls (cf. `21`).

### Ton et écriture
16. **Tutoiement progressif.** Les personnages passent du « vous » au « tu » selon la confiance ; un retour au « vous » est une rupture.
17. **Un mot de marseillais par réplique**, jamais en insulte (`17` §6), jamais en caricature.
18. **Les silences écrits.** Certaines scènes se terminent sur « … » ou sur une ligne d'action muette (« Elle repose la tasse. »).
19. **Les voix de la ville.** Radio, SMS de la mère, messages de voisins : un fil d'ambiance qui alterne humour et gravité (`17` §8).
20. **Le dernier mot.** À chaque fin, un personnage de la ville (pas le joueur) a la dernière réplique : l'infirmière, le chauffeur de taxi, le prêtre.

### Mécaniques de conséquence
21. **L'effet domino familial.** Une perquisition chez la mère monte `Exp` de la fratrie, qui baisse la Loyauté, qui fait partir un équipier : le journal montre la chaîne, pas seulement le résultat.
22. **Le casier qui colle.** Un palier de `Cas` rend certains choix impossibles (emploi normal, location) ; le joueur découvre la liste en lisant les refus.
23. **Le temps d'après la peine.** La sortie de prison n'est pas un retour : le monde a avancé, les chefs ont changé, l'argent ne vaut plus pareil ; un « tutoriel de sortie ».
24. **Les familles de rivaux.** Les rivaux aussi ont une mère, un enfant, un deuil : une scène par rival majeur qui humanise sans excuser.
25. **L'héritage qui se choisit.** À la fin, le joueur désigne **qui hérite de quoi** : un charbon, une dette, un secret, une clé. L'épilogue le montre.

## 2. Ce que ça change dans le code (quand on y viendra)
| Idée | Modules touchés | Remarque |
|---|---|---|
| 2, 3 | cycle, actions, `OperatorData` | nouvelle ressource « temps privé » |
| 4, 21 | `CrewData`, nouvelles jauges (`20`) | `Usu`, `Exp`, `Foy` à ajouter à la sauvegarde (v5) |
| 5, 22 | `EconomyRules`, `Dua`, `Cas` | pas de procédé, juste des jauges |
| 6, 7, 25 | `EndConditionSystem`, écran de fin | « Ce qu'ils sont devenus » (`18` §4.4) |
| 9, 13, 14 | `QuarterData`, rendu | mémoire de quartier |
| 11, 12 | `SeasonRules`, `GameEventSystem` | météo et jours de match comme modificateurs |

## 3. Risques
1. **Glorification involontaire.** Le jeu récompense la réussite ; il doit **toujours** faire payer (`20` §0). Test : chaque palier de gain doit monter au moins une jauge de conséquence.
2. **Stéréotypes de quartier et de personnes.** Écriture neutre, civils majoritaires (`21`), relecture par des gens du quartier concerné.
3. **Mineurs.** Aucun mineur à l'écran ni dans une scène de danger ; enfants et élèves hors champ ; relecture externe de toute mention (`17b` n° 8).
4. **Violence et deuil.** Pas de mise en scène, seulement les suites (une radio, une chaise vide, un cercueil) ; relecture externe obligatoire (`12` §10).
5. **Affaires réelles et noms.** Rue d'Aubagne hors du jeu ; noms inventés à vérifier hors du jeu (`17b` n° 18) ; club réel en décor seulement.
6. **Longueur.** 2 080 semaines à jouer : le **temps élastique** (`18` §4.2) est une condition de faisabilité, pas un confort.
7. **Complexité des jauges.** Huit jauges de conséquence : risque de surcharge. Solution : n'en afficher que **trois** à la fois, les autres en « signaux » dans le journal.
8. **Code jamais compilé.** Hiérarchie, façades, Ombre et refonte UI n'ont pas été testés dans Unity : toute nouvelle jauge attend un premier lancement propre.
9. **Contenu 18+ en store.** Prévoir un classement d'âge et un avertissement ; revoir les règles Apple/Google avant toute soumission (`12` §10).
10. **Équilibre.** Le joueur ne doit pas pouvoir « tout garder » (famille, argent, liberté, pouvoir) : valider par simulation sur 200 parties.

## 4. Questions ouvertes (à trancher par le joueur)
1. **Genre du joueur :** neutre (`21` §4) ou choix ? Les partenaires possibles (Lina, Yann) sont écrits pour tous.
2. **Âge de départ :** 18 ans (retenu) ou plus tôt/tard ? Cela change la place du prologue (`14`).
3. **Enfants du joueur :** autorisés **hors champ** seulement (proposé) ou absents du jeu ?
4. **Mort du joueur :** autorisée comme fin (« Cercueil ») avec écran de bilan, ou toujours évitée ?
5. **Mode difficile :** un mode « sans seconde chance » pour les joueurs qui veulent le réalisme le plus dur ?
6. **Temps élastique :** option B (semaines chaudes, mois calmes, sauts) acceptée ?
7. **Part de civils :** les 20 civils de `21` sont-ils en trop, ou faut-il en écrire plus (la vie ordinaire est le contrepoids du reste) ?
8. **Nombre de chefs :** les 16 arrondissements demandent 16 chefs (`17b` n° 6) : 3 écrits ; en écrire 5 de plus ?
9. **OM et ville :** « jours de match » comme décor : accepté, ou bannir toute mention de club ?
10. **Ordre de travail suivant :** (a) compiler et corriger l'UI, (b) coder la vie (âge, `Usu`, `Foy`), (c) coder les événements de `22`, (d) produire les images de `19` ?

## 5. Ordre de travail proposé
1. Lancer Unity, corriger les erreurs de compilation de l'UI (`16` étapes 1-4).
2. Ajouter le champ **âge** et les 3 jauges prioritaires (`Foy`, `Usu`, `Cas`) à la sauvegarde v5.
3. Intégrer 10 événements de `22` (un par âge clé) dans `GameEventSystem`.
4. Écrire l'écran « Ce qu'ils sont devenus ».
5. Produire les 24 portraits (`19`) et les 20 de `21`.

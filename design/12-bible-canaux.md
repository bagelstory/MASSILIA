# CHOUF : la bible des Canaux

*17 novembre 2026. Brainstorm narrative (scénario, rédaction locale) sur **le téléphone dans le trafic marseillais** : livraison à domicile, fils de messagerie, stories. S'ajoute à `11-bible-ombre.md` (ton, limites, campagne, jauge Ombre), qui reste la référence. Ce sont des idées : rien n'est codé, et les chiffres sont à simuler sur 260 semaines.*

## 0. Limites propres à ce document
- **Aucun mode d'emploi.** Pas d'application détaillée, pas de réglage, pas de manière d'échapper aux enquêteurs, pas de prix, pas de quantités, pas de routes de livraison. Le jeu montre l'**ambiance, la peur et les traces**, jamais le procédé.
- **Mineurs : victimes seulement.** Ils n'écrivent jamais au joueur comme clients ou recrues *jouables*. Quand un message d'enfant arrive, **les seules réponses possibles protègent l'enfant** (voir E12). Le jeu montre ensuite le coût, quoi qu'on fasse.
- **Les noms d'applications** (Snap, Télégram) restent dans la bouche des personnages, comme des mots de rue. À l'écran, l'interface s'appelle **« le Fil »**. Raison : règles des boutiques d'applications et marques déposées (voir §10).
- Tout reste **inventé** : comptes, bandes, enquêtes, journal. Aucun nom de personne réelle, aucun clan réel, aucune affaire identifiable.

---

## 1. Ce que le téléphone a changé (le phénomène, en gros)
1. **Le point de vente fixe a un concurrent qui ne s'arrête jamais** : le service à domicile. Le client n'a plus à venir dans la cité : le « charbon » se double d'un **Fil** (une case sans adresse fixe, qui dessert plusieurs quartiers).
2. **Le client devient un abonné** : il compare, note, se plaint, menace d'aller voir ailleurs. Le trafic prend les manières d'une appli de livraison, et ça glace.
3. **Tout laisse une trace.** Une capture d'écran vaut un témoin. Le chef dort moins, la police lit plus.
4. **Les messages qui disparaissent** donnent une fausse impression de sécurité. Les ordres s'effacent ; les ennuis, non.
5. **Les enfants sont à portée de message.** La rue n'est plus le seul endroit où l'on se fait happer : la chambre aussi. C'est la blessure de la ville, jamais un levier de gameplay.
6. **La vitrine** : stories, clips, faux profils. La fierté du quartier devient preuve à charge.
7. **La concurrence se fait en ligne** : usurpation, signalements, noms et adresses lâchés, et la peur arrive avant les actes.

---

## 2. Le Fil : douze mécaniques
| # | Mécanique | Ce que voit le joueur |
|---|---|---|
| 1 | **Le Fil** (nouveau type de case) | Une case qui couvre 3 à 5 quartiers voisins. Elle rapporte moins par habitant que le charbon, mais ne ferme jamais la nuit. |
| 2 | **Traces** (jauge 0-100 par Fil) | Monte avec l'audience et les jeunes qui parlent trop. À 70, un événement « Capture » tombe. Ne baisse qu'en fermant le Fil. |
| 3 | **Audience** | Des abonnés qui viennent d'un quartier à l'autre. Plus il y en a, plus le Fil rapporte, plus il laisse de traces. |
| 4 | **La tournée** | Chaque semaine, une ligne dit si la tournée est passée. Un retard fait fuir une partie de l'audience vers un Fil concurrent. |
| 5 | **Messages qui disparaissent** | Un ordre donné par le Fil a une fiabilité de 100 % la première semaine, puis elle s'effrite (comme pour le parloir). Oublier est un risque, pas une protection. |
| 6 | **Avis clients** | L'écran de fin de semaine affiche 3 avis inventés, la note moyenne, et un commentaire de trop. L'ironie est voulue. |
| 7 | **Compte grillé** | Un Fil peut être coupé (signalement, enquête, rival). Il faut en rouvrir un : on perd l'audience, pas l'argent. |
| 8 | **Doxxing** | Un rival lâche le nom d'un de tes équipiers. La peur monte dans son quartier, sa famille déménage, il ne répond plus. |
| 9 | **Vitrine** (profil de quartier) | Une story de fierté fait monter la loyauté, puis, trois semaines plus tard, la pression. Les deux jauges sont liées. |
| 10 | **Le Modo** | Un adulte qui tient le Fil sans jamais se montrer. Il peut disparaître avec la caisse. |
| 11 | **Vocal** | Une note vocale, en son ambiance : respiration, rue, un enfant qu'on entend derrière. Écrite seulement dans le jeu : « 0:42 ». |
| 12 | **Le Fil des morts** | Quand quelqu'un meurt, le Fil s'arrête 24 heures, puis un message de deuil circule dans le quartier. Impossible de le « réutiliser » pour le business. |

**Lien avec le code existant.** La jauge **Ombre** monte de +2 par Fil actif et de +4 par « compte grillé » ; les **Traces** nourrissent la probabilité d'une arrestation dans `FluxSystem` ; la **fiabilité** réutilise `ShadowRules.Reliability`.

---

## 3. Le lexique du Fil (rédaction locale)
**Mots de la rue numérique** : *le Fil* (le groupe, ou la boutique), *la tournée* (les livraisons de la semaine), *l'abonné*, *l'astreinte*, *la capture*, *vu à 3 h 12* (lu mais pas répondu), *dernier vu*, *grillé* (compte coupé), *bloqué*, *en ligne*, *le vocal*, *la story*, *ça tombe* (ça arrive), *il est sur écoute* (il se croit sur écoute).

**Marseille dans la bouche des personnages** :
- **Salutations et réponses** : *« Oh, fada ! »* · *« Té, regarde-moi ça. »* · *« Vas-y, fais pas ton marseillais. »* · *« Peuchère. »* · *« Sur la vie de ma mère. »* · *« C'est tarpin la hess. »* · *« Ça passe ou ça casse. »*
- **Quartier et ville** : *« On est tous sur la même ligne de bus. »* · *« L'OM a perdu, les nerfs sont là. »* · *« La Bonne Mère nous voit pas, ou elle regarde ailleurs. »* · *« Au vieux port, ils vendent des paniers. Chez nous, on vend l'espoir. »*
- **Le doute** : *« Il a arrêté d'écrire. »* · *« Le dernier vu, c'était mardi. »* · *« Il laisse jamais son téléphone. Jamais. »*

**Jamais à l'écran** : les vrais noms d'applications dans les menus, les termes de la presse sur des affaires réelles, tout mot en langue étrangère qui ressemble à un nom de clan réel.

---

## 4. Les visages du Fil (tous inventés, tous adultes)
- **La Standardiste**, 31 ans. Elle tient le Fil depuis une chambre sans fenêtre, au-dessus d'un pressing. *« Je connais plus les gens. Je connais leurs horaires. »*
- **Le Livreur d'astreinte**, 24 ans, scooter, un enfant en bas âge. *« Je suis payé à la course. Pas au drame. »*
- **Le Modo**, voix seule. On ne sait pas s'il s'agit d'un homme, d'une femme, ou de trois personnes. *« T'as besoin de savoir qui je suis ? Non. »*
- **L'Influenceur de quartier**, 28 ans, 80 000 abonnés. Il vend des baskets et de l'ambiance. *« Je poste, c'est tout. »* Il ne sait pas ce qu'il vend vraiment.
- **Brasier**, rappeur local, 25 ans. Ses textes disent tout et son clip est une preuve. *« Si je mens pas dans mes textes, c'est pas ma faute. »*
- **La Mère qui lit**, 47 ans. Elle lit le téléphone de son fils tous les soirs et sait déjà tout. *« Je regarde pas pour savoir. Je regarde pour pas hurler. »*
- **L'Enquêtrice cyber**, 39 ans. Elle trie des captures dans une salle sans fenêtre. *« Chaque jour, des milliers de messages. Aucun ne me parle. »*
- **Le Pigiste de nuit**, 33 ans, journaliste. Il tient un fil, lui aussi, sur un autre sujet.
- **Le Développeur**, 36 ans, freelance, qui « fait juste des outils ». Il a compris le jour où une cliente lui a demandé sa facture.
- **Le Vieux Parrain du quartier** : il comprend rien, il veut juste que ça reste « propre ».

---

## 5. Vingt événements du Fil (légende : A argent, P pression, C contrôle, L loyauté, Q quartier, F bande, Pe peur, **T traces**)
1. **Capture.** Un abonné a gardé ta conversation. *Payer son silence* : A−, T−, mais il revient. *Faire le mort* : T+, P+.
2. **Le retard.** La tournée n'est pas passée. *Compenser* : A−, Q+. *S'en moquer* : audience −, un rival récupère les abonnés.
3. **Le Fil grillé.** Compte coupé un dimanche. *Rouvrir tout de suite* : T+. *Attendre une semaine* : A−−, mais la pression retombe.
4. **Le Livreur ne répond plus.** Dernier vu mardi. *Envoyer chercher* : L+, Pe+. *Laisser* : L−. La suite dépend de ce qu'il est devenu.
5. **Le Modo disparaît** avec la caisse. *Le chercher* : F+, Pe+. *Faire sans* : A−, C−.
6. **Doxxing.** Le nom de ton lieutenant circule. *Le déplacer* : L+, A−. *Le laisser* : Pe+, Q−. Sa mère change de trottoir.
7. **La story de trop.** Un jeune de la bande poste son butin. *Le couper* : L−. *Laisser faire* : Q+ (fierté), P++ dans trois semaines.
8. **Le clip de Brasier.** Il cite ton quartier. *L'interdire* : Q−. *Le laisser* : F+, P++.
9. **Le faux profil.** Un infiltré a gagné la confiance de ton Fil. *Le soupçonner* : paranoïa +, L−. *Ne rien voir* : P++.
10. **Concurrence en ligne.** Un Fil rival te copie ton nom. *Riposter* : F+, Pe+. *Ignorer* : A−.
11. **Les avis.** Une cliente laisse un message en pleurs. *La contacter* : T+. *Supprimer son message* : le jeu note que tu l'as fait.
12. **Le message d'un jeune.** Un adolescent écrit au Fil pour « travailler ». **Les seules réponses :** *le supprimer sans répondre* (le Fil d'un rival le recevra : Enfance perdue +) ou *prévenir sa mère* (P+, L−, mais Enfance perdue −). Aucune réponse n'aide le trafic.
13. **La Mère qui lit** vient frapper à ta porte. Elle ne demande rien. Elle te regarde. *(Aucun choix : le jeu avance d'une semaine.)*
14. **L'Influenceur** rembourse un abonné en public. Le Fil devient une affaire de réputation. Q+, T++.
15. **Panne générale.** Plus de réseau pendant deux jours. La ville vit sans, et personne ne meurt de ça. *Remarque du jeu :* « Le quartier a respiré. »
16. **La Standardiste craque.** Elle veut partir. *La retenir* : L−, Pe+. *La laisser* : A−, C−. Elle dit en partant : *« Je te rends pas les clés. Je te les laisse. »*
17. **Signalement massif.** Un collectif de mères fait fermer trois Fils d'un coup. Q+ (le quartier respire), A−−.
18. **L'enquêtrice** reconnaît un détail dans une photo. *Détruire le Fil* : A−−, T remis à 0. *Le laisser* : arrestation probable dans 4 semaines.
19. **Le vocal.** Un équipier laisse un message à sa mère, sans savoir que le Fil le garde. Le joueur l'entend. *Le garder* ou *l'effacer* : Pe+ dans les deux cas.
20. **Le Fil des morts.** Quelqu'un est mort. Tout s'arrête 24 heures. À la reprise, le jeu n'affiche aucun gain de la journée.

---

## 6. L'écran de fin de semaine : « Le Fil de la semaine »
- Les notifications arrivent **comme un téléphone** : 3 à 5 lignes, une par événement, avec l'heure (« 03:12 »).
- Chaque ligne a un **lu / non lu**. Les lignes non lues se rappellent la semaine suivante.
- **3 avis clients** inventés (une étoile à cinq), puis la note moyenne du Fil.
- En bas, **la Une du jour** (*Courrier des Docks*, voir §7).
- À partir de l'Ombre « Nuit » (palier 3), les notifications arrivent **plus tard dans la nuit** et l'écran vibre une fois de plus.

## 7. Les unes numériques du *Courrier des Docks* (journal inventé)
- *« Livraisons à domicile : la préfecture parle d'une "économie parallèle" »*
- *« Des collectifs de mères réclament la fermeture des fils »*
- *« Un clip, une enquête : la Brigade numérique s'intéresse à un rappeur »*
- *« Un jeune retrouvé : sa mère avait lu ses messages »* (jamais plus de détails)
- *« L'influenceur de la Joliette : "Je vendais des baskets" »*
- *« Une panne géante prive la ville de réseau, pas de recrudescence »*
- *« Des mineurs au centre d'une procédure : le procureur veut "briser la chaîne" »*

---

## 8. Voix du Fil (texte court, pour les notifications et les vocaux)
- **Standardiste** : *« T'es sur le Fil ? Faut que je te dise un truc. »* / *« 03:12 — Pas de réponse de lui depuis hier. »*
- **Livreur** : *« Je suis devant. Y a personne. »* / *« Mon fils m'a demandé ce que je faisais. J'ai dit : livreur. »*
- **Modo** : *« Dernier vu : toi. »*
- **Mère qui lit** (vocal 0:42) : *« Je sais ce que t'es. Je sais pas ce que t'es devenu. »*
- **Influenceur** : *« Merci les frères pour les 80 000. C'est pas moi, c'est nous. »*
- **Brasier** (dernier clip) : image d'un balcon vide. Pas un mot.

---

## 9. Fins ajoutées
| Fin | Condition | Image finale |
|---|---|---|
| **Le Compte supprimé** | Tous les Fils fermés, Ombre < 40 | Un téléphone posé sur une table, écran noir |
| **La Capture** | Traces ≥ 90 trois semaines de suite | Une capture d'écran en plein écran, sans légende |
| **Le Dernier Vu** | Le Modo ou la Standardiste a disparu et l'Ombre ≥ 80 | « Dernier vu : il y a longtemps » |
| **La Mère** | Enfance perdue ≥ 70 | La Mère du collectif lit les noms au micro |

---

## 10. Risques et à vérifier
- **Boutiques d'applications** : un jeu qui montre un trafic par messagerie peut être refusé ou classé 17+/18+. À vérifier avant toute publication (règles Apple, Google).
- **Marques** : ne jamais mettre « Snapchat » ou « Telegram » dans les menus ni dans les images (le jeu dit « le Fil »).
- **Noms inventés à vérifier** : *Brasier*, *le Modo*, *Courrier des Docks* (pas de journal réel du même nom), *l'Armateur*, *Vieux Lucien*.
- **Enfants** : toute scène avec un mineur reste dans les limites du §0 ; à relire par une personne extérieure au projet avant publication.
- **Relecture locale** : faire lire le lexique par un habitant de Marseille.

## 11. À coder en premier
1. **Traces** (champ `traces` sur une case Fil, `GameConfig.Canal`) + événement « Capture ».
2. **L'écran de fin de semaine « Le Fil »** (3 à 5 lignes, avis, une de journal).
3. **Compte grillé** (fermeture d'un Fil, perte d'audience).
4. Puis : doxxing, vitrine, et l'événement 12 (le message d'un jeune), à écrire avec une relecture externe.

# CHOUF : fusion avec le dossier V6 « MARSEILLE »

*17 novembre 2026. Décision de l'utilisateur : **prologue V6, puis gestion CHOUF**. Source : `ref/MARSEILLE_GDD_V6.txt` (conservé tel quel, ne pas le modifier). Ce document dit ce qu'on garde de V6, ce qu'on change, et pourquoi. Idées de conception : rien n'est codé.*

## 1. La structure fusionnée
| Phase | Âge | Durée | Ce que fait le joueur |
|---|---|---|---|
| **Prologue « Le Vide »** | 16-17 ans | 6 scènes, 45-60 min, **pas de boucle de gestion** | Vit, choisit, promet, subit. Aucune tâche criminelle, aucun gain criminel. |
| **Campagne « Cinq ans de dette »** (`11`) | 18-23 ans | 260 semaines | Gestion, ombre, façades, Fil. Le prologue pèse sur toute la partie. |
| **L'après** (`11` §6 + V6 §15) | fin de campagne | épilogue | Fins construites à partir du parcours. |

**Règle d'or :** le joueur n'est un « chef » qu'à partir de 18 ans et d'une dette dont il hérite. Avant, il est un adolescent que des adultes essaient d'utiliser, et le jeu le montre comme tel.

**Passerelle :** la dernière scène du prologue est la mort du Vieux Lucien (`11` §3). À 18 ans, le protagoniste hérite de ce qu'il a promis, refusé ou laissé de côté. La campagne commence là.

## 2. Ce qu'on garde de V6 (tel quel)
- Piliers 1 à 5, promesse centrale (« Marseille ne t'appartient pas »).
- **Personnages autonomes avec mémoire** (V6 §6) : refus, départ, retour, rappel des promesses.
- **Réputation en 4 dimensions** (V6 §5) : Respect, Confiance, Influence, Crainte. Elles ne se résument pas en un score. À ajouter à `11` en plus de la jauge Ombre.
- **Accès progressif aux quartiers** (V6 §7) : toute la ville visible, déblocage par la narration.
- **Trois temporalités de conséquences** : immédiate, différée, long terme.
- **Fins libres** : pas de menu de fins. Les 8 fins noires de `11` deviennent des **familles de trajectoires**, pas des fins imposées.
- **Mission « Le Bruit »** : plusieurs versions d'un même fait, le joueur choisit à qui il croit.
- **Les 5 écrans** (carte, tableau de bord, personnages, journal, gestion).
- **Réseaux internationaux** (V6 §11) : un écran abstrait (engagements, fiabilité, exposition, conséquences humaines). **Même règle que partout :** aucune route, aucun procédé.

## 3. Ce qu'on change
| V6 | Fusion | Pourquoi |
|---|---|---|
| Protagoniste de 16 ans qui « monte » dès 17 | **Prologue sans gestion**, gestion dès 18 ans | Un mineur n'est jamais un gestionnaire criminel (règle fixe de `00-LISEZMOI.md`). |
| Années datées 2021-2026 | **Année 1 à Année 5**, ambiance d'époque sans date | Ne coller à aucune affaire réelle. |
| Jour / semaine / mois / trimestre | **Une semaine par tour** (D1) | Déjà décidé et codé. |
| Six indicateurs V6 (stabilité, trésorerie, confiance interne, pression, ancrage humain, résilience) | Gardés **en lecture** sur le tableau de bord, calculés à partir de nos jauges (`IndicatorSystem`) | Pas de doublon de simulation. |
| Ton « réalisme social » | **Noir réaliste** (`11`) : conséquences d'abord | Décision du 17 novembre. |

## 4. Le prologue : six scènes adaptées (« Le Vide »)
*Toutes sans tâche criminelle. Le joueur choisit, promet, refuse, observe. Les adultes qui cherchent à utiliser l'adolescent sont montrés comme des adultes qui abusent de lui.*

1. **Le Quotidien.** Le matin, la famille, le lycée, le bus. Le quartier de départ existe pour lui-même : le marché, un match de l'OM sur un écran, la mère qui travaille tôt. *Systèmes introduits :* fiche du protagoniste, carte locale, calendrier.
2. **Les Visages.** Cinq à huit personnages (voir §5). Une première demande contradictoire (un grand frère contre une prof). *Systèmes :* confiance, mémoire des choix, journal.
3. **La Promesse.** Il promet quelque chose à quelqu'un, et une autre personne lui demande l'inverse. Aucun choix n'est « bon ». *Systèmes :* promesses, conséquence différée.
4. **Le Bruit.** Une rumeur sur un voisin. Trois versions, aucune vraie en entier. *Systèmes :* information partielle, incertitude.
5. **Les Absents.** Un adulte ne rentre pas. La famille attend, chacun interprète. Le jeu affirme son ton : les événements touchent des familles. *Systèmes :* événement différé, conséquences humaines.
6. **Le Premier Choix.** Trois problèmes en même temps. Il ne peut en traiter qu'un. *Systèmes :* priorités, points d'action. **Puis la mort du Vieux Lucien** : la campagne s'ouvre.

**Ce que le prologue enregistre** (et qui revient dans la campagne) : promesses tenues ou rompues, personnes protégées ou abandonnées, qui lui fait confiance, qui le craint, ce que la famille sait. Ces drapeaux alimentent la sortie, la jauge Ombre de départ et les fins.

**Ce que le prologue ne fait jamais :** donner une récompense, un revenu ou un rang lié à un acte criminel ; montrer un acte opérationnel ; traiter l'adolescent en recrue qui « progresse ». S'il est approché par des adultes, la scène montre la pression et propose toujours une sortie réelle (famille, école, une adulte qui l'aide).

## 5. Les personnages du prologue (inventés)
Écrire 5 à 8 fiches. Pistes, à réconcilier avec `11` §3 :
- **La mère**, qui part très tôt et rentre tard. *« Je t'en demande pas beaucoup. Juste de rentrer. »*
- **Le grand frère** (22 ans), qui a des « amis ». Il aime son petit frère et l'utilise sans le dire.
- **La prof principale** (40 ans), qui a vu défiler des élèves. Elle sait, ne dit pas tout.
- **L'entraîneur du club**, qui tient le foot du quartier avec des bouts de ficelle.
- **Le Vieux Lucien** (déjà dans `11`), l'homme que tout le monde salue.
- **La voisine** (65 ans), qui voit tout depuis son balcon.
- **Un ami d'enfance**, qui prend un autre chemin avant lui.
- **Un adulte « qui propose »**, jamais montré en train de faire quoi que ce soit. Il parle, il invite, il attend.

## 6. Questions que V6 laisse ouvertes (à trancher ici)
- **Secteur de départ :** V6 hésite entre nord, centre, est, sud, ou choix du joueur. *Proposition :* le joueur choisit parmi 3-4 secteurs, chacun avec une famille et une scène d'ouverture différentes (rejouabilité). **À DÉCIDER.**
- **Création du personnage :** prénom et apparence au choix, **famille et motivation écrites** (pour que le prologue soit un vrai récit). **À DÉCIDER.**
- **Intensité de représentation :** noir réaliste (retenu), mais avec un prologue plus sobre, presque documentaire, pour que le contraste serve. **À DÉCIDER.**
- **Les 8 zones V3** de V6 ne sont pas les 10 zones de `10` : à comparer et unifier. **À FAIRE.**

## 7. À coder (ordre conseillé)
1. `design`: finir les 5 à 8 fiches du prologue (voir §5) et les 6 scènes, **avec relecture externe (mineur)**.
2. Un mode « Prologue » dans le jeu : scènes à choix, **sans** `CycleManager`, qui écrit des drapeaux dans `GameData` (version de sauvegarde 3 ou 4).
3. Les 4 dimensions de réputation (Respect, Confiance, Influence, Crainte) dans `GameData`.
4. `Resources/Data/prologue.json` (scènes, choix, drapeaux), chargé par un petit lecteur de scènes.
5. Raccord : la dernière scène lance la campagne avec les drapeaux du prologue.

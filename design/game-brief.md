# CHOUF · Game Brief

*Écrit le 17 novembre 2026. Jeu de gestion noir à Marseille, 18+, lieux réels, personnages 100 % fictifs.*

- **Titre de travail :** CHOUF
- **Pitch :** le jeu de gestion noir de Marseille. Tu tiens un réseau quartier par quartier, et chaque gain se paie ailleurs.

## Boucle de jeu
1. Lire la carte (111 quartiers) et les indicateurs.
2. Assigner tes opérateurs, investir, choisir ton diplomate.
3. Passer la semaine : événements et réactions des factions.
4. Lire les conséquences dans le journal.

## But et échec
- **But :** tenir 60 % des quartiers à la fin de l'Acte I (5 ans, 260 semaines ; la partie entière dure 25 à 40 ans, voir `design/18-personnages-et-vie.md` §4), sans être à découvert.
- **Échec :** faillite, chute, abandon ou temps écoulé. Fin « Sortie » possible, jusqu'à la note A. Chaque fin a son écran et sa note de A à E.

## MVP (7 éléments)
1. Carte de 111 quartiers.
2. Opérateurs et tâches.
3. Économie et flux.
4. Factions pilotées par l'IA.
5. Campagne à carrefours (Acte I de 5 ans ; durée totale : voir `18` §4) (voir `design/09-campagne-marseille.md`).
6. Fins et score.
7. Interface portrait et sauvegarde JSON lisible.

## Hors périmètre
- Saisons et LiveOps (après le MVP).
- Son.
- Multijoueur.
- Monétisation.
- Toute méthode opérationnelle concrète : les effets sont montrés, jamais les procédés.

## Ordre de construction
1. Compiler dans Unity 2022.3 et corriger les erreurs (le code n'a jamais été compilé : risque n°1).
2. Test sur un vrai téléphone (60 fps visés sur Snapdragon 720, build sous 50 Mo).
3. Campagne (Acte I de 5 ans).

## Public, direction et référence
- **Public et sensation :** adultes 18+, fans de gestion. « Je gagne, mais à quel prix. »
- **Direction artistique et sonore :** noir et doré, images générées, pas de son pour l'instant.
- **Jeu de référence :** Rebel Inc. Le MVP garde sa gestion de carte au tour par tour avec conséquences visibles.

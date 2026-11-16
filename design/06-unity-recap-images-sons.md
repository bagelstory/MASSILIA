# CHOUF — Récapitulatif du projet et besoins en images et sons (version Unity)

État au 16 novembre 2026, branche `j1` du dépôt bagelstory/MASSILIA.

## 1. Ce qui a été fait

### Version web (HTML, terminée et publiée)
- **v1 narrative** (archivée dans `archive/v1-narratif/`) : jeu de vie inspiré de KICK, avec un protagoniste et 36 personnages fictifs répartis en 6 familles.
- **Prompts d'illustrations** : documents `03-prompts-illustrations.md` puis `04-prompts-3d.md`. Le second est la version qui passe les filtres, en style maquette 3D façon Godfather II.
- **v2 « gestion pure »** (`05-gestion-pure.md`) :
  - pas de protagoniste ; 6 types d'opérateurs et 5 factions rivales fictives avec IA ;
  - économie, capacité, flux internationaux abstraits, 6 indicateurs ;
  - 111 quartiers réels de Marseille (données data.gouv.fr), regroupés en 10 zones, avec zones verrouillées, objectifs pour les ouvrir et charbons plus ou moins rentables dans chaque quartier.
- Testée par une simulation (bots) et un test d'interface. Publiée en page web.

### Version Unity 2022.3 (C#, les 10 étapes de ton prompt)
Environ 95 fichiers, 8 800 lignes, dans `unity/Assets/`. Aucun fichier ne dépasse 200 lignes. Toutes les méthodes ont un commentaire XML et tous les nombres de gameplay sont dans `GameConfig`.

| Étape | Contenu |
|---|---|
| 1 Data | Structures sérialisables : quartiers, opérateurs, factions, flux, zones, objectifs, sauvegarde |
| 2 Core | GameManager, EventBus, CycleManager, sauvegarde JSON lisible avec copie de secours |
| 3 Systèmes | Opérateurs, Territoire, Économie, construction du monde de départ |
| 4 Systèmes | IA des factions, Flux, Indicateurs, Événements, Objectifs, Fin de partie |
| 5 Carte | Grille 2D portrait des 111 quartiers, couleurs de statut, pions, zoom et déplacement |
| 6 Interface | Barre du haut, onglets, écran Carte (affectation), écran Bilan |
| 7 Interface | Écrans Équipe (recrutement), Flux, Rivaux, Journal |
| 8 Interface | Détail d'un quartier (charbons, investissement), détail d'un opérateur, résultats du cycle, fin de partie (note de A à E) |
| 9 Config | GameConfig regroupé, vérification automatique, menu « Marseille » dans l'éditeur |
| 10 Données | `Resources/Data/quarters.json` : 111 quartiers réels avec leurs voisins |

**Pour lancer :**
1. Ouvrir le dossier `unity/` avec Unity 2022.3 LTS.
2. Menu **Marseille → 2. Créer la scène principale**.
3. Appuyer sur Play.

**Important :** le code n'a pas pu être compilé ici (ni Unity ni compilateur C# disponibles). La première ouverture dans Unity peut afficher quelques erreurs. Il suffit de me les copier pour que je les corrige.

## 2. Images nécessaires

La version Unity actuelle n'utilise **aucune image** : tout est en aplats de couleur et en texte. Les emplacements suivants sont prévus. Une fois les fichiers déposés dans le dépôt, je branche le code.

Règles communes :
- **Format PNG**. Unity 2022.3 ne lit pas le WebP ; je peux convertir les WebP de la version web.
- Fond transparent pour les icônes et les emblèmes.
- Personnages et équipes 100 % fictifs, adultes, sans logo ni marque réelle.
- Budget total du build inférieur à 50 Mo : rester sous 15 Mo d'images au total.

### Priorité 1 : identité et lisibilité
| Fichier (dans `unity/Assets/Art/`) | Taille | Nombre | Usage |
|---|---|---|---|
| `icon_app.png` | 1024×1024 | 1 | Icône de l'application |
| `logo_chouf.png` | 1024×512, transparent | 1 | Écran titre, écran de fin |
| `splash.png` | 1080×1920 | 1 | Écran de lancement (ville + logo) |
| `type_reperage.png` … `type_force.png` | 256×256, transparent | 6 | Pions et fiches des 6 types d'opérateurs |
| `faction_compagnie.png`, `faction_gabians.png`, `faction_ruisseau.png`, `faction_tramontane.png`, `faction_calanquais.png` | 256×256, transparent | 5 | Emblèmes des équipes rivales (dans leurs couleurs) |
| `ind_territoire.png`, `ind_tresorerie.png`, `ind_effectifs.png`, `ind_pression.png`, `ind_stabilite.png`, `ind_exposition.png` | 128×128, transparent | 6 | Les 6 indicateurs |

### Priorité 2 : ambiance
| Fichier | Taille | Nombre | Usage |
|---|---|---|---|
| `zone_1.png` … `zone_10.png` | 1080×400 | 10 | En-tête du détail de quartier. **Déjà disponibles** : les bannières de la version web (`dist/img/banniere_*.webp`), que je peux convertir |
| `portrait_zepek.png`, `portrait_rascasse.png`, `portrait_mika.png`, `portrait_zizou.png`, `portrait_sab.png`, `portrait_nono.png` | 512×512 | 6 | Les 6 opérateurs nommés |
| `portrait_<type>_1.png`, `portrait_<type>_2.png` | 512×512 | 12 | Recrues anonymes, 2 par type |
| `end_victoire.png`, `end_faillite.png`, `end_chute.png`, `end_abandon.png`, `end_temps.png` | 1080×800 | 5 | Écran de fin de partie |

### Priorité 3 : finitions
| Fichier | Taille | Nombre | Usage |
|---|---|---|---|
| `tab_carte.png`, `tab_bilan.png`, `tab_equipe.png`, `tab_flux.png`, `tab_rivaux.png`, `tab_journal.png` | 96×96, transparent | 6 | Onglets |
| `task_securiser.png` … `task_repos.png` | 128×128, transparent | 7 | Boutons de tâche |
| `flux_sud.png`, `flux_nord.png` | 600×300 | 2 | Cartes des partenaires (abstraites : port, conteneurs, route) |
| `map_fond.png` | 1080×1440 | 1 | Fond de carte stylisé (côte, mer, collines), facultatif |

**Formes simples que je fabrique moi-même** : rond des pions, case arrondie des quartiers, pastilles charbon, menace et cadenas.

Les prompts du document `04-prompts-3d.md` couvrent déjà le style. Je peux écrire les prompts manquants (emblèmes, icônes, portraits des 6 opérateurs nommés) au même format.

## 3. Sons

Ta spécification Unity exclut le son (« pas de son »). Rien n'est donc branché. Si tu veux l'ajouter, il faut lever cette règle. Le travail côté code est léger : un AudioManager qui écoute l'EventBus.

Comme tu produis toi-même, la bande-son peut être la tienne. Voici ce qu'il faudrait :

### Musique (`unity/Assets/Audio/Music/`, OGG Vorbis, stéréo, 44,1 kHz, environ 160 kbit/s)
| Fichier | Durée | Rôle |
|---|---|---|
| `music_menu.ogg` | 1 à 2 min, boucle | Écran titre et fin de partie |
| `music_planning.ogg` | 2 à 3 min, boucle sans coupure | Phase de planification (ambiance calme et tendue) |
| `music_tension.ogg` | 1 à 2 min, boucle | Pression supérieure à 80 ou caisse vide (même tempo et même tonalité que la précédente, pour un fondu enchaîné) |
| `stinger_victoire.ogg`, `stinger_defaite.ogg` | 5 à 10 s | Fin de partie |

### Effets (`unity/Assets/Audio/Sfx/`, WAV mono, 44,1 kHz, moins d'1 s sauf mention)
| Fichier | Moment |
|---|---|
| `ui_tap.wav` | Toucher un bouton |
| `ui_back.wav` | Fermer une fenêtre |
| `assign.wav` | Opérateur affecté |
| `recruit.wav` | Recrutement |
| `cycle_validate.wav` | Validation du cycle (1 à 2 s) |
| `cycle_gain.wav`, `cycle_loss.wav` | Résultat positif ou négatif |
| `quarter_gained.wav`, `quarter_lost.wav` | Quartier gagné ou perdu |
| `faction_attack.wav` | Attaque d'une faction |
| `police_alert.wav` | Descente, saisie, alerte de pression (abstrait, pas de sirène réaliste trop longue) |
| `operator_lost.wav` | Opérateur perdu |
| `quest_done.wav`, `zone_unlocked.wav` | Objectif atteint, zone ouverte |
| `flux_ok.wav`, `flux_rupture.wav` | Flux réussi, rupture |

Budget : environ 10 Mo pour la musique et 1 Mo pour les effets. Le build reste sous 50 Mo.

## 4. Prochaines étapes proposées
1. Ouvrir le projet dans Unity et me renvoyer les erreurs de compilation éventuelles.
2. Déposer les images de priorité 1. Je branche les icônes, emblèmes et portraits.
3. Décider pour le son (lever la règle « pas de son » ou non).
4. Équilibrage : rejouer des parties types et ajuster `GameConfig`. Le seuil de victoire à 70 % reste à confirmer.

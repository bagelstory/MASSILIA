# CHOUF : guide pour reprendre le projet (IA ou humain)

*À lire en premier. Mis à jour le 17 novembre 2026.*

## Le jeu
**CHOUF** : jeu de gestion noir, 18+, sur la criminalité à Marseille. Le joueur tient un réseau sur une vie : de 18 ans jusqu'à une fin entre 25 et 40 ans de jeu (âge = 18 + semaines ÷ 52 ; actes de 5 ans, voir `18` §4). Lieux réels, **personnages, bandes, affaires et journaux 100 % inventés**. Le ton est sombre et réaliste : on montre les conséquences, jamais le mode d'emploi.

Dépôt : `bagelstory/MASSILIA`, branche `j1`. Moteur : **Unity 2022.3 LTS, C#**. Le code n'a **jamais été compilé** (pas de Unity là où il a été écrit) : la première chose à faire est d'ouvrir `unity/` dans Unity et de corriger les erreurs.

## Règles de contenu (non négociables)
1. **Pas de procédé réel** : ni blanchiment, ni importation, ni recrutement, ni armes, ni manière d'échapper à la police. Le moteur tire des résultats, le texte montre des conséquences.
2. **Pas de gore.** On montre la trace, jamais le corps.
3. **Mineurs** : victimes ou enjeu de la ville, jamais jouables, jamais un gain.
4. **Aucune glorification** : plus on monte, plus l'écran montre ce que ça coûte.
5. **Rien de réel côté personnes** : aucun nom, clan, label, journal, élu ou affaire identifiable. **Exception décidée le 20/11 : l'OM, club réel, peut être cité comme simple décor de quartier (jamais lié à un acte ni à un personnage).** Rue d'Aubagne hors du jeu. Aucune cité nommée comme repaire. Années jamais datées.
6. Dans l'interface, le téléphone s'appelle **« le Fil »** (jamais de nom d'appli).

## Où lire quoi (par ordre)
| Fichier | Contenu |
|---|---|
| `game-brief.md` | La page de référence : pitch, boucle, objectif (60 % des quartiers, à découvert interdit), MVP. |
| `08-reunion-2026-11-17.md` | Décisions D1 (semaine), D2 (60 %), D3 (campagne remplacée), D4 (fin « Sortie »). **D5 et D6 encore ouvertes.** |
| `10-bible-marseille.md` | Zones, personnages, formules de simulation, lexique. Le ton festif est remplacé par `11`. |
| `11-bible-ombre.md` | **Référence du ton et de la campagne** (« Cinq ans de dette » = Acte I), mécaniques noires, fins, événements. |
| `12-bible-canaux.md` | Le téléphone dans le trafic : le Fil, Traces, événements, écran de fin de semaine. |
| `13-facades.md` | Placer l'argent sale : 7 secteurs, plausibilité, Éveil, événements. |
| `15-hierarchie.md` | **Hiérarchie du terrain** : arrondissement (chef) > quartier (plusieurs têtes) > charbon (2 guetteurs, 1 vendeur, 1 gérant, 1 tête qui peut en tenir plusieurs). |
| `16-audit-refonte-ui.md` | Audit de l'interface, maquettes, plan en 8 étapes, **arbitrages du joueur** (§12). Étapes 1 à 4 codées (jamais compilées). |
| `17-ecriture-marseille.md` | Écriture poussée sur Marseille : arrondissements, saisons, lexique, 30 textes de journal. |
| `17b-revue-equipe.md` | **Revue d'équipe** : 18 incohérences et 6 décisions à trancher (dont Aubagne, mineurs, durée de vie). |
| `18-personnages-et-vie.md` | 24 personnages adultes fictifs, cinq priorités, structure de la vie sur 25 à 40 ans. |
| `19-prompts-3d-blender.md` | Prompts d'images et de 3D, guide Blender et export Unity. |
| `BRAINSTORM.txt` | Carnet vivant : directions, idées en vrac, questions ouvertes. À compléter à chaque séance. |
| `14-fusion-v6.md` | Fusion avec le dossier V6 « MARSEILLE » (`ref/MARSEILLE_GDD_V6.txt`) : prologue à 16-17 ans sans gestion, puis campagne CHOUF dès 18 ans. |
| `09-campagne-marseille.md` | Ancienne campagne (remplacée) : seuls le tempo et les rangs restent valables. |
| `01` à `07` | Plan v1, modèle de données, prompts d'images/3D (utiles pour l'art). |

## Le code (`unity/Assets/Scripts/`)
- **Contraintes** : uGUI construit dans le code, pas de TextMeshPro, pas de coroutines, fichiers ≤ 200 lignes, sauvegarde `JsonUtility` (pas de dictionnaires), chiffres dans le `GameConfig` (ScriptableObject).
- **Cycle** : 1 cycle = 1 semaine. Chaque système implémente `ICycleSystem`. Ordre : Operator, Territory, FactionAI, Flux, **Hierarchy**, Economy, Indicator, **Shadow**, **Facade**, GameEvent, Quest, EndCondition (`GameManager.Systems.cs`).
- **Structs avec listes** : copier, modifier, réaffecter. Enums sérialisés en entiers : **ajouter à la fin seulement**.
- **Sauvegarde** : version courante 4 (`GameData.CurrentVersion`), migrations dans `SaveManager.Migrate`.
- **Déjà codé (non compilé)** : Ordre et Dette de sang, Parloir (Baumettes, quartier 901), Terreur, argent sale et « Placer l'argent », jauge Ombre (voile noir), **façades** (7 secteurs, plausibilité, Éveil, fermer ; v1 de `13`), voir `Systems/Shadow*.cs`, `Systems/Facade*.cs`, **hiérarchie** (postes des charbons, chef d'arrondissement : `Systems/Crew*.cs`, `HierarchySystem.cs`, v1 de `15`), `Config/GameConfig.Shadow.cs`, `UI/ShadowVeil.cs`.
- **Pas encore codé** : fin de « sécuriser puis partir », pression de police par zone, écran de fin de semaine, campagne JSON, carrefours, nouvelles fins, Canaux (`12`), événements de façade (15) et gérant de façade (`13`).

## Priorités
1. **Compiler** (milestone J1) : ouvrir Unity, menu Marseille → 1. Créer la configuration par défaut / 2. Créer la scène principale, corriger les erreurs.
2. Boucle jouable (J2) : économie, flux, factions, fins.
3. MVP complet (J3) : campagne, événements à choix, sauvegarde.
4. Puis : Traces/Fil (`12`), Façades (`13`).

## Conventions de travail
- Commits en Conventional Commits (`feat:`, `fix:`, `docs:`…), un commit par sujet.
- Les chiffres vivent dans `GameConfig.*.cs` et se vérifient par simulation sur la durée d'un acte (260 semaines) puis d'une vie (voir `18` §4).
- Les textes joueur sont en français. Dire « semaine », pas « trimestre ».
- Toute idée d'événement : légende A argent, P pression, C contrôle, L loyauté, Q quartier, F bande, Pe peur, S silence, T traces, É éveil, Of enfance perdue.

## Décisions ouvertes
- **D5** : les affectations persistent-elles d'une semaine à l'autre ? (cible : 8 touches par tour)
- **D6** : les événements datés suivent-ils le temps du jeu ou le calendrier du téléphone ? (recommandation : temps du jeu)
- Noms inventés à vérifier : Courrier des Docks, l'Armateur, Vieux Lucien, Brasier, le Modo.
- Images : deux portraits manquants (`portrait_renseignement_2`, `portrait_finances_2`).

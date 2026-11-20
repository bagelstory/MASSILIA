// MARSEILLE — Data/GameData.cs
// Racine de la sauvegarde. Sérialisée telle quelle par SaveManager avec JsonUtility (lisible, éditable à la main).
// System.Text.Json n'existe pas dans Unity 2022.3 sans DLL externes : JsonUtility est le choix « Unity standard ».
// Contraintes JsonUtility respectées : champs publics, [Serializable], listes (pas de dictionnaires), pas de null imbriqué.

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>État complet d'une partie (spec : racine de sauvegarde).</summary>
    [System.Serializable]
    public struct GameData
    {
        /// <summary>Version actuelle du format (2 : ombre, peur, population ; 3 : façades ; 4 : hiérarchie ; 5 : conséquences de la vie).</summary>
        public const int CurrentVersion = 5;

        /// <summary>Version du format, pour les migrations de sauvegarde.</summary>
        public int saveVersion;

        /// <summary>Graine du générateur aléatoire (rejouer une partie à l'identique).</summary>
        public int randomSeed;

        /// <summary>Cycle courant (commence à 1).</summary>
        public int currentCycle;

        /// <summary>État de la boucle (valeur de l'enum GameState, défini dans Core).</summary>
        public int gameState;

        /// <summary>Les 111 quartiers.</summary>
        public List<QuarterData> quarters;

        /// <summary>Les zones de jeu.</summary>
        public List<ZoneData> zones;

        /// <summary>Les opérateurs, y compris perdus (statut Lost) pour le bilan.</summary>
        public List<OperatorData> operators;

        /// <summary>Les factions rivales.</summary>
        public List<FactionData> factions;

        /// <summary>Les offres et flux internationaux.</summary>
        public List<FluxData> fluxes;

        /// <summary>Les quêtes (objectifs chiffrés).</summary>
        public List<QuestData> quests;

        /// <summary>Les six indicateurs.</summary>
        public IndicatorData indicators;

        /// <summary>Journal des événements (plus récent en premier).</summary>
        public List<JournalEntry> journal;

        /// <summary>Nombre maximal d'opérateurs déployables (spec §7).</summary>
        public int capacity;

        /// <summary>Seuils de territoire déjà récompensés par +1 de capacité.</summary>
        public List<int> capacityThresholdsReached;

        /// <summary>Dette (€), avec intérêts par cycle.</summary>
        public float debt;

        /// <summary>Cycles consécutifs à trésorerie nulle (faillite au seuil de GameConfig).</summary>
        public int zeroTreasuryCycles;

        /// <summary>Quartier de base du joueur.</summary>
        public int baseQuarterId;

        /// <summary>Prochain numéro d'opérateur.</summary>
        public int nextOperatorId;

        /// <summary>Résumé texte de chaque cycle résolu (historique).</summary>
        public List<string> log;

        /// <summary>Fin de partie (None tant que la partie continue).</summary>
        public EndType endType;

        /// <summary>L'ombre : jauge Ombre, morts, dettes de sang, ordres en attente (version 2).</summary>
        public ShadowData shadow;

        /// <summary>Arrondissements et leur chef (design/15-hierarchie.md).</summary>
        public List<ArrondissementData> arrondissements;

        /// <summary>Conséquences de la vie : casier, usure, foyer, justice, réputation (version 5).</summary>
        public ConsequenceData life;

        /// <summary>Crée une sauvegarde vide avec toutes ses listes initialisées.</summary>
        /// <param name="seed">Graine aléatoire.</param>
        /// <param name="treasury">Trésorerie de départ.</param>
        /// <param name="capacity">Capacité de départ.</param>
        /// <returns>La racine initialisée, cycle 1.</returns>
        public static GameData CreateEmpty(int seed, float treasury, int capacity)
        {
            return new GameData
            {
                saveVersion = CurrentVersion,
                randomSeed = seed,
                currentCycle = 1,
                gameState = 0,
                quarters = new List<QuarterData>(),
                zones = new List<ZoneData>(),
                operators = new List<OperatorData>(),
                factions = new List<FactionData>(),
                fluxes = new List<FluxData>(),
                quests = new List<QuestData>(),
                indicators = IndicatorData.Create(treasury, 15f, 55f, 5f),
                journal = new List<JournalEntry>(),
                capacity = capacity,
                capacityThresholdsReached = new List<int>(),
                debt = 0f,
                zeroTreasuryCycles = 0,
                baseQuarterId = Owners.None,
                nextOperatorId = 1,
                log = new List<string>(),
                endType = EndType.None,
                shadow = ShadowData.Create(treasury),
                arrondissements = new List<ArrondissementData>(),
                life = ConsequenceData.Create()
            };
        }

        /// <summary>Index d'un quartier dans <see cref="quarters"/> à partir de son identifiant.</summary>
        /// <param name="quarterId">Identifiant recherché.</param>
        /// <returns>Index, ou -1 s'il n'existe pas.</returns>
        public int IndexOfQuarter(int quarterId)
        {
            if (quarters == null) return -1;
            for (int i = 0; i < quarters.Count; i++) if (quarters[i].id == quarterId) return i;
            return -1;
        }

        /// <summary>Index d'un opérateur dans <see cref="operators"/>.</summary>
        /// <param name="operatorId">Identifiant recherché.</param>
        /// <returns>Index, ou -1.</returns>
        public int IndexOfOperator(int operatorId)
        {
            if (operators == null) return -1;
            for (int i = 0; i < operators.Count; i++) if (operators[i].id == operatorId) return i;
            return -1;
        }

        /// <summary>Index d'une faction dans <see cref="factions"/>.</summary>
        /// <param name="factionId">Identifiant recherché.</param>
        /// <returns>Index, ou -1.</returns>
        public int IndexOfFaction(int factionId)
        {
            if (factions == null) return -1;
            for (int i = 0; i < factions.Count; i++) if (factions[i].id == factionId) return i;
            return -1;
        }

        /// <summary>Index d'une zone dans <see cref="zones"/>.</summary>
        /// <param name="zoneId">Identifiant recherché.</param>
        /// <returns>Index, ou -1.</returns>
        public int IndexOfZone(int zoneId)
        {
            if (zones == null) return -1;
            for (int i = 0; i < zones.Count; i++) if (zones[i].id == zoneId) return i;
            return -1;
        }
    }
}

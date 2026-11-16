// MARSEILLE — Data/QuestData.cs
// Objectifs chiffrés (quêtes) : pas de narration, des seuils à atteindre et une récompense.
// Une quête peut ouvrir une zone verrouillée (zoneToUnlock ≥ 0).

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>Un critère d'une quête.</summary>
    [System.Serializable]
    public struct QuestObjective
    {
        /// <summary>Ce qui est mesuré.</summary>
        public QuestObjectiveType type;

        /// <summary>Quantité à atteindre (nombre de quartiers, €, %, etc.).</summary>
        public float target;

        /// <summary>Seuil associé (ex. contrôle minimal pour QuartersHeld, niveau pour ZoneKnowledge).</summary>
        public float threshold;

        /// <summary>Zone concernée (ZoneKnowledge), sinon <see cref="Owners.None"/>.</summary>
        public int zoneId;

        /// <summary>Crée un critère.</summary>
        /// <param name="type">Mesure.</param>
        /// <param name="target">Quantité à atteindre.</param>
        /// <param name="threshold">Seuil associé.</param>
        /// <param name="zoneId">Zone, ou -1.</param>
        /// <returns>Le critère initialisé.</returns>
        public static QuestObjective Create(QuestObjectiveType type, float target, float threshold, int zoneId)
        {
            return new QuestObjective { type = type, target = target, threshold = threshold, zoneId = zoneId };
        }
    }

    /// <summary>Quête : liste de critères et récompense.</summary>
    [System.Serializable]
    public struct QuestData
    {
        /// <summary>Identifiant texte stable (ex. « zone_3 », « second_charbon »).</summary>
        public string id;

        /// <summary>Nom affiché.</summary>
        public string name;

        /// <summary>Critères, tous requis.</summary>
        public List<QuestObjective> objectives;

        /// <summary>Zone ouverte en récompense, ou <see cref="Owners.None"/>.</summary>
        public int zoneToUnlock;

        /// <summary>Argent gagné (€).</summary>
        public float rewardMoney;

        /// <summary>Capacité d'opérateurs gagnée.</summary>
        public int rewardCapacity;

        /// <summary>Quête terminée.</summary>
        public bool isCompleted;

        /// <summary>Cycle de réussite (0 si non terminée).</summary>
        public int completedCycle;

        /// <summary>Crée une quête non terminée.</summary>
        /// <param name="id">Identifiant stable.</param>
        /// <param name="name">Nom affiché.</param>
        /// <param name="zoneToUnlock">Zone ouverte, ou -1.</param>
        /// <param name="rewardMoney">Argent gagné.</param>
        /// <param name="rewardCapacity">Capacité gagnée.</param>
        /// <returns>La quête initialisée.</returns>
        public static QuestData Create(string id, string name, int zoneToUnlock, float rewardMoney, int rewardCapacity)
        {
            return new QuestData
            {
                id = id,
                name = name,
                objectives = new List<QuestObjective>(),
                zoneToUnlock = zoneToUnlock,
                rewardMoney = rewardMoney,
                rewardCapacity = rewardCapacity,
                isCompleted = false,
                completedCycle = 0
            };
        }
    }
}

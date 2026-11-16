// MARSEILLE — Config/GameConfig.End.cs
// Section « fin de partie et objectifs » de GameConfig (spec §11) : victoire, limite de durée, note finale,
// objectifs chiffrés et ouverture des zones.

using System.Collections.Generic;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Config
{
    /// <summary>Objectif chiffré fixe (sans narration).</summary>
    [System.Serializable]
    public class QuestSettings
    {
        /// <summary>Identifiant.</summary>
        public string id;
        /// <summary>Nom affiché.</summary>
        public string name;
        /// <summary>Critère.</summary>
        public QuestObjectiveType type;
        /// <summary>Quantité à atteindre.</summary>
        public float target;
        /// <summary>Seuil associé (contrôle, niveau de connaissance).</summary>
        public float threshold;
        /// <summary>Récompense en argent (€).</summary>
        [Min(0f)] public float rewardMoney;
        /// <summary>Récompense en capacité.</summary>
        [Min(0)] public int rewardCapacity;
    }

    public partial class GameConfig
    {
        [Header("Fin de partie")]
        [Tooltip("Territoire (%) qui donne la victoire, trésorerie positive.")]
        [Range(0f, 100f)] public float victoryTerritory = 70f;
        [Tooltip("Nombre maximal de cycles (fin par le temps). 260 = 5 ans.")]
        [Min(1)] public int maxCycles = 260;
        [Tooltip("Note finale : seuils A, B, C, D (score).")]
        public List<float> gradeThresholds = new List<float> { 85f, 65f, 45f, 25f };
        [Tooltip("Score : bonus de victoire, malus de défaite.")]
        public Vector2 scoreEndBonus = new Vector2(20f, -25f);
        [Tooltip("Score : € par point de trésorerie, plafond ; € par point de dette ; points par faction disparue ; par opérateur ; pression divisée par.")]
        public float scoreMoneyDivisor = 2000f, scoreMoneyMax = 30f, scorePerFactionGone = 6f, scorePerOperator = 2f, scorePressureDivisor = 5f;

        [Header("Ouverture des zones")]
        [Tooltip("Quartiers à tenir (contrôle ≥ seuil) : nombre de base, + par zone déjà ouverte, seuil.")]
        public Vector3 zoneQuestHeld = new Vector3(2f, 1f, 50f);
        [Tooltip("Quartiers de la zone visée à connaître (niveau ≥ 1).")]
        [Min(0)] public int zoneQuestKnowledge = 2;
        [Tooltip("Charbons ouverts : nombre de base, + par zone déjà ouverte.")]
        public Vector2Int zoneQuestCharbons = new Vector2Int(1, 1);
        [Tooltip("Récompense en argent à l'ouverture d'une zone (€).")]
        [Min(0f)] public float zoneQuestReward = 1500f;
        [Tooltip("Capacité +1 toutes les N zones ouvertes.")]
        [Min(1)] public int zoneQuestCapacityEvery = 2;

        [Header("Objectifs fixes")]
        public List<QuestSettings> quests = new List<QuestSettings>
        {
            new QuestSettings { id = "second_charbon", name = "Deuxième charbon", type = QuestObjectiveType.OpenCharbons, target = 2f, rewardMoney = 800f },
            new QuestSettings { id = "know_around", name = "Connaître le voisinage", type = QuestObjectiveType.Knowledge, target = 4f, threshold = 2f, rewardMoney = 600f },
            new QuestSettings { id = "lock_base", name = "Verrouiller ta base", type = QuestObjectiveType.SecuredQuarters, target = 1f, rewardMoney = 1000f },
            new QuestSettings { id = "crew_4", name = "Quatre opérateurs", type = QuestObjectiveType.Operators, target = 4f, rewardCapacity = 1 },
            new QuestSettings { id = "cash_15k", name = "Une vraie caisse", type = QuestObjectiveType.Treasury, target = 15000f, rewardCapacity = 1 },
            new QuestSettings { id = "five_charbons", name = "Cinq charbons", type = QuestObjectiveType.OpenCharbons, target = 5f, rewardMoney = 2500f },
            new QuestSettings { id = "territory_10", name = "10 % de la ville", type = QuestObjectiveType.Territory, target = 10f, rewardMoney = 3000f },
            new QuestSettings { id = "first_flux", name = "Premier flux", type = QuestObjectiveType.ActiveFluxes, target = 1f, rewardMoney = 2000f },
            new QuestSettings { id = "territory_25", name = "Un quart de la ville", type = QuestObjectiveType.Territory, target = 25f, rewardMoney = 5000f, rewardCapacity = 1 }
        };
    }
}

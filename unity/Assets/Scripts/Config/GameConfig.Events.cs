// MARSEILLE — Config/GameConfig.Events.cs
// Section « événements et calendrier » de GameConfig (spec §10) : arrestations, opérations extérieures,
// opérateurs écartés, saisons qui modifient les revenus.

using System.Collections.Generic;
using UnityEngine;

namespace Marseille.Config
{
    /// <summary>Saison ou moment de l'année qui modifie les revenus de certaines zones.</summary>
    [System.Serializable]
    public class SeasonSettings
    {
        /// <summary>Identifiant.</summary>
        public string id;
        /// <summary>Nom affiché.</summary>
        public string name;
        /// <summary>Semaines actives (incluses) ; (0,0) = pas de plage fixe.</summary>
        public Vector2Int weeks;
        /// <summary>Active les semaines paires (hors plage <see cref="excludedWeeks"/>).</summary>
        public bool evenWeeks;
        /// <summary>Semaines exclues pour le mode « semaines paires ».</summary>
        public Vector2Int excludedWeeks;
        /// <summary>Probabilité d'apparition par cycle (0 = jamais au hasard).</summary>
        [Range(0f, 1f)] public float chance;
        /// <summary>Zones touchées ; liste vide = toute la ville.</summary>
        public List<int> zoneIds = new List<int>();
        /// <summary>Multiplicateur des revenus des charbons.</summary>
        [Min(0f)] public float incomeMultiplier = 1f;
    }

    public partial class GameConfig
    {
        [Header("Calendrier")]
        [Tooltip("Semaine de l'année du cycle 1.")]
        [Range(1, 52)] public int startWeek = 37;
        [Tooltip("Année du cycle 1.")]
        public int startYear = 2021;
        [Tooltip("Saisons et moments de l'année.")]
        public List<SeasonSettings> seasons = new List<SeasonSettings>
        {
            new SeasonSettings { id = "summer", name = "Été", weeks = new Vector2Int(27, 34), zoneIds = new List<int> { 1, 10, 4, 3 }, incomeMultiplier = 1.3f },
            new SeasonSettings { id = "match", name = "Soir de match", evenWeeks = true, excludedWeeks = new Vector2Int(22, 31), zoneIds = new List<int> { 4 }, incomeMultiplier = 1.15f },
            new SeasonSettings { id = "mistral", name = "Mistral", chance = 0.1f, incomeMultiplier = 0.9f }
        };

        [Header("Événements")]
        [Tooltip("Probabilité de base d'arrestation d'un opérateur (× pression/100 × police de zone/référence).")]
        [Range(0f, 1f)] public float arrestChance = 0.1f;
        [Tooltip("Facteur appliqué à un opérateur au repos.")]
        [Range(0f, 1f)] public float arrestRestFactor = 0.3f;
        [Tooltip("Niveau de police de référence des zones.")]
        [Min(1f)] public float policeReference = 60f;
        [Tooltip("Probabilité d'opération extérieure par cycle quand la pression est en alerte.")]
        [Range(0f, 1f)] public float outsideOperationChance = 0.3f;
        [Tooltip("Opération extérieure : pression ajoutée.")]
        [Min(0f)] public float outsideOperationPressure = 3f;
        [Tooltip("Opération extérieure : nombre de quartiers touchés.")]
        [Min(0)] public int outsideOperationQuarters = 2;
        [Tooltip("Opération extérieure : contrôle perdu dans chacun.")]
        [Min(0f)] public float outsideOperationControlLoss = 10f;
        [Tooltip("Opération extérieure : multiplicateur des arrestations ce cycle.")]
        [Min(1f)] public float outsideOperationArrestFactor = 2f;
        [Tooltip("Probabilité qu'un opérateur (hors Force) soit écarté dans un quartier dominé par une faction.")]
        [Range(0f, 1f)] public float pushedOutChance = 0.04f;
        [Tooltip("Présence de la faction dominante à partir de laquelle ce risque existe.")]
        [Range(0f, 100f)] public float pushedOutPresence = 50f;
    }
}

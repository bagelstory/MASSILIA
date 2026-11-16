// MARSEILLE — Config/GameConfig.Indicators.cs
// Section « indicateurs et capacité » de GameConfig (spec §7, §9).

using System.Collections.Generic;
using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Indicateurs : pression")]
        [Tooltip("Baisse fixe de la pression par cycle.")]
        [Min(0f)] public float pressureDecay = 3f;
        [Tooltip("Baisse proportionnelle de la pression par cycle.")]
        [Range(0f, 1f)] public float pressureDecayRatio = 0.05f;
        [Tooltip("Pression ajoutée par quartier gagné.")]
        [Min(0f)] public float pressurePerQuarterGained = 2f;
        [Tooltip("Seuil d'alerte de pression (opérations extérieures possibles).")]
        [Range(0f, 100f)] public float pressureAlert = 80f;
        [Tooltip("Pression qui fait tomber le réseau (fin de partie).")]
        [Range(0f, 100f)] public float pressureCollapse = 100f;

        [Header("Indicateurs : exposition")]
        [Tooltip("Baisse fixe de l'exposition par cycle.")]
        [Min(0f)] public float exposureDecay = 2f;
        [Tooltip("Baisse proportionnelle de l'exposition par cycle.")]
        [Range(0f, 1f)] public float exposureDecayRatio = 0.06f;
        [Tooltip("Au-dessus de ce seuil, l'exposition fait monter la pression.")]
        [Range(0f, 100f)] public float exposureAlert = 80f;
        [Tooltip("Pression ajoutée par cycle d'exposition en alerte.")]
        [Min(0f)] public float exposureToPressure = 4f;

        [Header("Indicateurs : stabilité")]
        [Tooltip("Valeur vers laquelle la stabilité revient.")]
        [Range(0f, 100f)] public float stabilityTarget = 50f;
        [Tooltip("Vitesse de retour (part de l'écart par cycle).")]
        [Range(0f, 1f)] public float stabilityDrift = 0.1f;
        [Tooltip("Stabilité perdue par opérateur perdu.")]
        [Min(0f)] public float stabilityPerOperatorLost = 10f;
        [Tooltip("Fatigue au-dessus de laquelle un opérateur pèse sur la stabilité.")]
        [Range(0f, 100f)] public float stabilityFatigueThreshold = 80f;
        [Tooltip("Stabilité perdue par opérateur trop fatigué.")]
        [Min(0f)] public float stabilityFatiguePenalty = 1f;
        [Tooltip("Stabilité gagnée par quartier tenu avec un opérateur sur place, et plafond de ce gain.")]
        public Vector2 stabilityHeldQuarter = new Vector2(0.5f, 4f);
        [Tooltip("Stabilité perdue par flux actif au-delà du nombre confortable.")]
        [Min(0f)] public float stabilityPerExtraFlux = 3f;

        [Header("Capacité")]
        [Tooltip("Capacité +1 tous les N cycles.")]
        [Min(1)] public int capacityEveryCycles = 12;
        [Tooltip("Capacité +1 la première fois que le territoire atteint chacun de ces seuils (%).")]
        public List<int> capacityTerritoryThresholds = new List<int> { 15, 30, 45, 60 };
    }
}

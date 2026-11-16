// MARSEILLE — Config/GameConfig.Factions.cs
// Section « IA des factions » de GameConfig (spec §5) : budget, recrutement, objectifs, force des actions,
// trêves, fragmentation.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Factions : argent et effectifs")]
        [Tooltip("Part du potentiel de leurs quartiers que les factions encaissent (pondérée par leur présence).")]
        [Range(0f, 2f)] public float factionIncomeRatio = 0.5f;
        [Tooltip("Coût par effectif et par cycle (€).")]
        [Min(0f)] public float factionCostPerEffectif = 120f;
        [Tooltip("Budget maximal d'une faction (€).")]
        [Min(0f)] public float factionBudgetMax = 40000f;
        [Tooltip("Budget au-dessus duquel une faction peut recruter (€, en plus du coût).")]
        [Min(0f)] public float factionRecruitThreshold = 3000f;
        [Tooltip("Coût d'un recrutement de faction (€).")]
        [Min(0f)] public float factionRecruitCost = 1500f;
        [Tooltip("Probabilité de recruter par cycle quand le budget le permet.")]
        [Range(0f, 1f)] public float factionRecruitChance = 0.25f;
        [Tooltip("Effectif maximal d'une faction.")]
        [Min(1)] public int factionEffectifMax = 24;

        [Header("Factions : décisions")]
        [Tooltip("Cycles du début pendant lesquels les factions ne contre-attaquent pas et n'avancent pas sur le joueur.")]
        [Min(0)] public int factionCalmCycles = 6;
        [Tooltip("Contrôle du joueur qui, dans un quartier de faction, déclenche une contre-attaque.")]
        [Range(0f, 100f)] public float factionAttackDetectControl = 50f;
        [Tooltip("Chance de contre-attaque : base + agressivité × ce facteur.")]
        [Range(0f, 1f)] public float factionCounterChancePerAggressiveness = 0.15f;
        [Tooltip("Chance de contre-attaque de base.")]
        [Range(0f, 1f)] public float factionCounterChanceBase = 0.1f;
        [Tooltip("Repli quand le budget passe sous budgetMax × ce facteur × (budget de départ / budgetMax + 0,5).")]
        [Range(0f, 1f)] public float factionRetreatBudget = 0.3f;
        [Tooltip("Chance d'étendre : base + agressivité × facteur.")]
        public Vector2 factionExpandChance = new Vector2(0.25f, 0.12f);
        [Tooltip("Présence en dessous de laquelle un voisin est une cible d'extension.")]
        [Range(0f, 100f)] public float factionExpandTargetPresence = 30f;
        [Tooltip("Contrôle du joueur au-dessus duquel un quartier n'est pas une cible d'extension.")]
        [Range(0f, 100f)] public float factionExpandMaxPlayerControl = 40f;
        [Tooltip("Une action par tranche de cet effectif (au moins une).")]
        [Min(1)] public int factionActionsPerEffectif = 5;
        [Tooltip("Coût d'une action (€).")]
        [Min(0f)] public float factionActionCost = 300f;
        [Tooltip("Force d'une action : base, par point d'agressivité, par effectif.")]
        public Vector3 factionActionForce = new Vector3(4f, 1.2f, 0.25f);
        [Tooltip("Facteur aléatoire appliqué à la force (min, max).")]
        public Vector2 factionActionRandom = new Vector2(0.5f, 1f);
        [Tooltip("Défense maximale du joueur dans un quartier (part de l'attaque annulée).")]
        [Range(0f, 1f)] public float factionMaxDefense = 0.85f;
        [Tooltip("En repli : présence retirée par cycle dans les quartiers où elle est sous le seuil.")]
        public Vector2 factionRetreatPresence = new Vector2(25f, 4f);
        [Tooltip("Chance de baisser l'agressivité en repli, et de la remonter en maintien.")]
        public Vector2 factionAggressivenessShift = new Vector2(0.3f, 0.2f);

        [Header("Factions : trêves et fragmentation")]
        [Tooltip("Probabilité par cycle qu'une trêve se forme entre deux factions.")]
        [Range(0f, 1f)] public float factionAllianceChance = 0.04f;
        [Tooltip("Durée d'une trêve en cycles (min, max).")]
        public Vector2Int factionAllianceDuration = new Vector2Int(6, 12);
        [Tooltip("Territoire minimal atteint avant de pouvoir se fragmenter.")]
        [Min(1)] public int factionFragmentationMinPeak = 4;
        [Tooltip("Fragmentation quand le territoire tombe sous cette part du pic.")]
        [Range(0f, 1f)] public float factionFragmentationRatio = 0.4f;
        [Tooltip("Part de l'effectif conservée après fragmentation.")]
        [Range(0f, 1f)] public float factionFragmentationEffectifs = 0.6f;
        [Tooltip("Présence totale en dessous de laquelle une faction disparaît.")]
        [Min(0f)] public float factionMinPresence = 1f;
    }
}

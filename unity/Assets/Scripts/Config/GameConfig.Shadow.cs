// MARSEILLE — Config/GameConfig.Shadow.cs
// Section « ombre » de GameConfig (design/11-bible-ombre.md §4) : l'Ordre et la Dette de sang, le Parloir,
// la Terreur qui vide, l'argent sale, la jauge Ombre. Points de départ, à vérifier en simulation sur 260 cycles.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Ombre : l'Ordre et la Dette de sang")]
        [Tooltip("Coût d'un ordre (€).")]
        [Min(0f)] public float orderCost = 3000f;
        [Tooltip("Délai d'un ordre donné dehors (cycles).")]
        [Min(1)] public int orderDelay = 1;
        [Tooltip("Présence de la bande retirée du quartier si l'ordre réussit.")]
        [Min(0f)] public float orderPresenceHit = 25f;
        [Tooltip("Effectifs retirés à la bande si l'ordre réussit.")]
        [Min(0)] public int orderEffectifsHit = 1;
        [Tooltip("Force d'un opérateur Force (par point de compétence) dans le calcul de réussite.")]
        [Min(0.1f)] public float orderForcePerCompetence = 1f;
        [Tooltip("Pression globale ajoutée par ordre.")]
        [Min(0f)] public float orderPressure = 4f;
        [Tooltip("Peur ajoutée dans le quartier par ordre (doublée en cas de mort).")]
        [Min(0f)] public float orderFear = 15f;
        [Tooltip("Chance de représailles par cycle et par point de dette.")]
        [Range(0f, 1f)] public float reprisalPerDebt = 0.08f;
        [Tooltip("Plafond de la chance de représailles par cycle.")]
        [Range(0f, 1f)] public float reprisalMaxChance = 0.6f;
        [Tooltip("Part des représailles qui coûtent la vie à l'équipier visé (sinon : un proche, stabilité en baisse).")]
        [Range(0f, 1f)] public float reprisalLethalShare = 0.4f;
        [Tooltip("Stabilité perdue quand les représailles touchent un proche.")]
        [Min(0f)] public float reprisalStabilityHit = 8f;

        [Header("Ombre : le Parloir")]
        [Tooltip("Quartier de la prison (Les Baumettes, 9e) : les équipiers incarcérés y apparaissent sur la carte.")]
        public int prisonQuarterId = 901;
        [Tooltip("Durée de détention après une arrestation (cycles, min et max).")]
        public Vector2Int jailCycles = new Vector2Int(12, 40);
        [Tooltip("Délai d'un ordre donné depuis le parloir (cycles).")]
        [Min(1)] public int parloirDelay = 2;
        [Tooltip("Fiabilité perdue par cycle de détention (sur 100).")]
        [Min(0f)] public float parloirReliabilityLoss = 8f;
        [Tooltip("Fiabilité minimale d'un ordre du parloir (sur 100).")]
        [Range(0f, 100f)] public float parloirReliabilityFloor = 10f;
        [Tooltip("Coût par cycle pour la famille d'un équipier incarcéré (€).")]
        [Min(0f)] public float parloirFamilyCost = 120f;
        [Tooltip("Fatigue à la sortie de prison.")]
        [Range(0f, 100f)] public float jailReleaseFatigue = 70f;

        [Header("Ombre : la Terreur qui vide")]
        [Tooltip("Baisse de la peur par cycle.")]
        [Min(0f)] public float fearDecay = 4f;
        [Tooltip("Seuil de la terreur : au-dessus, le quartier commence à se vider.")]
        [Range(0f, 100f)] public float terrorThreshold = 60f;
        [Tooltip("Cycles consécutifs au-dessus du seuil avant les départs.")]
        [Min(1)] public int terrorDelay = 4;
        [Tooltip("Population perdue par cycle de terreur (points de %).")]
        [Min(0f)] public float terrorPopulationLoss = 1f;
        [Tooltip("Population minimale (un quartier ne se vide jamais complètement).")]
        [Range(0f, 100f)] public float populationFloor = 20f;
        [Tooltip("Peur ajoutée dans les quartiers voisins quand quelqu'un meurt.")]
        [Min(0f)] public float deathFearNeighbours = 5f;

        [Header("Ombre : l'argent sale")]
        [Tooltip("Trésorerie au-delà de laquelle l'argent liquide fait monter l'exposition (€).")]
        [Min(0f)] public float dirtyCashThreshold = 30000f;
        [Tooltip("Exposition par cycle pour chaque tranche de ce montant au-dessus du seuil (€).")]
        [Min(1f)] public float dirtyCashPerExposure = 10000f;
        [Tooltip("Montant placé à chaque fois dans un quartier (€).")]
        [Min(0f)] public float injectAmount = 5000f;
        [Tooltip("Exposition retirée par placement.")]
        [Min(0f)] public float injectExposureRelief = 6f;
        [Tooltip("Contamination ajoutée = montant / (revenu potentiel × ce facteur) × 100.")]
        [Min(1f)] public float contaminationValueFactor = 40f;
        [Tooltip("Plafond de contamination d'un quartier (empêche la boucle sans fin).")]
        [Range(0f, 100f)] public float contaminationMax = 100f;
        [Tooltip("Au-dessus de ce seuil, un commerce honnête ferme tous les N cycles et des familles partent.")]
        [Range(0f, 100f)] public float contaminationThreshold = 50f;
        [Tooltip("Cycles entre deux fermetures de commerce.")]
        [Min(1)] public int contaminationShopCycles = 4;
        [Tooltip("Population perdue par cycle dans un quartier contaminé (loyers).")]
        [Min(0f)] public float contaminationPopulationLoss = 0.5f;

        [Header("Ombre : la jauge")]
        [Tooltip("Ombre par point de territoire gagné.")]
        [Min(0f)] public float ombrePerTerritory = 0.3f;
        [Tooltip("Ombre par point de peur moyenne gagnée.")]
        [Min(0f)] public float ombrePerFear = 0.2f;
        [Tooltip("Ombre par ordre donné.")]
        [Min(0f)] public float ombrePerOrder = 3f;
        [Tooltip("Ombre par % de trésorerie gagné dans le cycle.")]
        [Min(0f)] public float ombrePerTreasuryPercent = 0.1f;
        [Tooltip("Plancher : Ombre minimale par mort.")]
        [Min(0f)] public float ombrePerDeath = 2f;
        [Tooltip("Paliers : Crépuscule, Nuit, Nuit noire, Ombre.")]
        public Vector4 ombreTiers = new Vector4(20f, 40f, 60f, 80f);
        [Tooltip("Stabilité maximale à partir du palier Nuit.")]
        [Range(0f, 100f)] public float ombreNightStabilityCap = 60f;
        [Tooltip("Stabilité perdue par cycle à partir du palier Nuit noire.")]
        [Min(0f)] public float ombreDarkStabilityLoss = 1f;
        [Tooltip("Multiplicateur des représailles au palier Ombre.")]
        [Min(1f)] public float ombreReprisalFactor = 1.5f;
        [Tooltip("Assombrissement maximal de l'écran (alpha du voile à Ombre 100).")]
        [Range(0f, 1f)] public float ombreVeilMax = 0.45f;
    }
}

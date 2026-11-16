// MARSEILLE — Config/GameConfig.Territory.cs
// Section « territoire et charbons » de GameConfig (spec §3.3, §3.4 et charbons).

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Territoire")]
        [Tooltip("Contrôle minimal d'un quartier sécurisé.")]
        [Range(0f, 100f)] public float securedControl = 80f;
        [Tooltip("Présence adverse maximale d'un quartier sécurisé.")]
        [Range(0f, 100f)] public float securedMaxHostile = 20f;
        [Tooltip("Contrôle d'un voisin requis pour accéder à un quartier (spec §3.4 : 50 %).")]
        [Range(0f, 100f)] public float accessNeighbourControl = 50f;
        [Tooltip("Cycles sans opérateur avant que le contrôle s'effrite.")]
        [Min(0)] public int abandonGraceCycles = 2;
        [Tooltip("Contrôle perdu par cycle d'abandon (quartier non sécurisé).")]
        [Min(0f)] public float abandonDecay = 5f;
        [Tooltip("Contrôle perdu par cycle d'abandon (quartier sécurisé, jamais sous le seuil sécurisé).")]
        [Min(0f)] public float securedAbandonDecay = 1f;
        [Tooltip("Contrôle perdu par cycle quand la trésorerie est à zéro avec une dette.")]
        [Min(0f)] public float noMoneyDecay = 3f;
        [Tooltip("Sous cette stabilité, quartiers et opérateurs s'usent plus vite (spec §9.1 : 20).")]
        [Range(0f, 100f)] public float lowStabilityThreshold = 20f;
        [Tooltip("Contrôle perdu par cycle quand la stabilité est basse.")]
        [Min(0f)] public float lowStabilityDecay = 2f;
        [Tooltip("Présence minimale (%) pour qu'un quartier compte dans le territoire d'une faction.")]
        [Range(0f, 100f)] public float factionTerritoryThreshold = 30f;
        [Tooltip("Part de l'excédent pris aux factions quand il n'y a plus de part libre.")]
        [Range(0f, 1f)] public float controlTakenFromHostile = 0.5f;

        [Header("Charbons")]
        [Tooltip("Contrôle requis pour ouvrir un charbon libre.")]
        [Range(0f, 100f)] public float charbonOpenControl = 30f;
        [Tooltip("Contrôle requis pour reprendre le charbon d'une faction.")]
        [Range(0f, 100f)] public float charbonTakeControl = 50f;
        [Tooltip("Coût d'ouverture = rentabilité × ce facteur.")]
        [Min(0f)] public float charbonOpenCostRatio = 3f;
        [Tooltip("Coût de reprise = rentabilité × ce facteur.")]
        [Min(0f)] public float charbonTakeCostRatio = 4f;
        [Tooltip("Pression et exposition ajoutées en reprenant un charbon.")]
        public Vector2 charbonTakeHeat = new Vector2(3f, 2f);
        [Tooltip("Sous ce contrôle, les charbons du joueur sont perdus.")]
        [Range(0f, 100f)] public float charbonLossControl = 15f;
        [Tooltip("Présence minimale d'une faction pour s'emparer d'un charbon libre.")]
        [Range(0f, 100f)] public float charbonFactionClaimPresence = 30f;
        [Tooltip("Probabilité par cycle qu'une faction s'empare d'un charbon libre.")]
        [Range(0f, 1f)] public float charbonFactionClaimChance = 0.2f;
        [Tooltip("Pression ajoutée par cycle et par point de risque d'un charbon ouvert.")]
        [Min(0f)] public float charbonPressurePerRisk = 0.1f;
        [Tooltip("Exposition ajoutée par cycle et par point de risque d'un charbon ouvert.")]
        [Min(0f)] public float charbonExposurePerRisk = 0.075f;
        [Tooltip("Probabilité de saisie de base (× risque × pression/50).")]
        [Range(0f, 1f)] public float charbonSeizureBase = 0.025f;
        [Tooltip("Cycles de fermeture après une saisie.")]
        [Min(1)] public int charbonSeizureCycles = 3;
    }
}

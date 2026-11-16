// MARSEILLE — Config/GameConfig.Economy.cs
// Section « économie » de GameConfig (spec §6).

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Économie")]
        [Tooltip("Efficacité des revenus d'un quartier sans opérateur Pression sur place.")]
        [Range(0f, 1f)] public float incomeWithoutPressure = 0.5f;
        [Tooltip("Intérêts de la dette par cycle (part).")]
        [Range(0f, 1f)] public float debtInterest = 0.03f;
        [Tooltip("Part de la trésorerie positive consacrée au remboursement chaque cycle.")]
        [Range(0f, 1f)] public float debtRepaymentShare = 0.5f;
        [Tooltip("Cycles consécutifs à trésorerie nulle avant la faillite (spec §6.3 : 3).")]
        [Min(1)] public int bankruptcyCycles = 3;
        [Tooltip("Coût d'investissement = potentiel du quartier × ce facteur × (1 + niveau).")]
        [Min(0f)] public float investCostRatio = 2f;
        [Tooltip("Coût d'investissement minimal (€), même pour un quartier sans revenu.")]
        [Min(0f)] public float investMinimumBase = 400f;
        [Tooltip("Hausse du revenu potentiel par niveau d'investissement.")]
        [Min(0f)] public float investIncomeGain = 0.2f;
        [Tooltip("Hausse du coût de maintien par niveau d'investissement.")]
        [Min(0f)] public float investMaintenanceGain = 0.15f;
        [Tooltip("Réduction maximale du maintien d'un quartier (cumul des opérateurs).")]
        [Range(0f, 1f)] public float maxMaintenanceReduction = 0.8f;
    }
}

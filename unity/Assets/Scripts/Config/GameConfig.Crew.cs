// MARSEILLE — Config/GameConfig.Crew.cs
// Section « hiérarchie » de GameConfig (design/15-hierarchie.md). Points de départ, à vérifier en simulation.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Hiérarchie : les cinq postes d'un charbon")]
        [Tooltip("Nombre maximal de charbons qu'une tête (le joueur ou un opérateur) peut tenir.")]
        [Min(1)] public int headCapacity = 4;
        [Tooltip("Poids du vendeur dans le rendement d'un charbon.")]
        [Min(0f)] public float crewWeightVendeur = 0.35f;
        [Tooltip("Poids du gérant dans le rendement d'un charbon.")]
        [Min(0f)] public float crewWeightGerant = 0.2f;
        [Tooltip("Poids de chaque guetteur dans le rendement d'un charbon.")]
        [Min(0f)] public float crewWeightGuetteur = 0.15f;
        [Tooltip("Poids de la tête dans le rendement d'un charbon.")]
        [Min(0f)] public float crewWeightHead = 0.15f;
        [Tooltip("Part du rendement perdue pour un poste entièrement vide (1 = tout le poids du poste).")]
        [Range(0f, 1f)] public float crewMissingPenalty = 0.35f;
        [Tooltip("Pression ajoutée par cycle pour chaque guetteur manquant sur un charbon ouvert.")]
        [Min(0f)] public float crewMissingPressure = 0.25f;
    }
}

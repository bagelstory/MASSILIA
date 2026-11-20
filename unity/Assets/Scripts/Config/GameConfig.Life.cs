// MARSEILLE — Config/GameConfig.Life.cs
// Section « durée de vie » de GameConfig (design/16 §12, design/18) : âge de départ, durée minimale et
// maximale de la partie. Remplace l'ancienne limite fixe de 260 cycles.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Durée de vie")]
        [Tooltip("Âge du personnage au premier cycle.")]
        [Min(1)] public int lifeStartAge = 18;
        [Tooltip("Années de jeu à partir desquelles le joueur peut se retirer.")]
        [Min(1)] public int lifeMinYears = 25;
        [Tooltip("Années de jeu au bout desquelles la partie s'arrête (bilan de vie).")]
        [Min(1)] public int lifeMaxYears = 40;
        [Tooltip("Cycles (semaines) par année de jeu.")]
        [Min(1)] public int weeksPerYear = 52;

        /// <summary>Nombre de cycles à partir duquel la partie s'arrête d'elle-même.</summary>
        public int MaxCycles { get { return lifeMaxYears * weeksPerYear; } }

        /// <summary>Nombre de cycles à partir duquel le joueur peut se retirer.</summary>
        public int MinRetireCycles { get { return lifeMinYears * weeksPerYear; } }

        /// <summary>Âge du personnage pendant un cycle.</summary>
        /// <param name="cycle">Cycle (1 = premier).</param>
        /// <returns>Âge en années.</returns>
        public int AgeAt(int cycle)
        {
            return lifeStartAge + Mathf.Max(0, cycle - 1) / Mathf.Max(1, weeksPerYear);
        }
    }
}

// MARSEILLE — Core/CycleResult.cs
// Ce que l'UI affiche après la résolution d'un cycle : variations des indicateurs et lignes courtes.

using System.Collections.Generic;
using Marseille.Data;

namespace Marseille.Core
{
    /// <summary>Résultat d'un cycle résolu, transmis par EventBus.OnCycleResolved.</summary>
    [System.Serializable]
    public struct CycleResult
    {
        /// <summary>Numéro du cycle qui vient d'être résolu.</summary>
        public int cycle;

        /// <summary>Variation de trésorerie (€).</summary>
        public float treasuryDelta;

        /// <summary>Variation du territoire (points de %).</summary>
        public float territoryDelta;

        /// <summary>Variation de la pression.</summary>
        public float pressureDelta;

        /// <summary>Variation de la stabilité.</summary>
        public float stabilityDelta;

        /// <summary>Variation de l'exposition.</summary>
        public float exposureDelta;

        /// <summary>Lignes produites pendant la résolution (une phrase chacune).</summary>
        public List<JournalEntry> lines;

        /// <summary>Fin de partie déclenchée par ce cycle (None sinon).</summary>
        public EndType endType;

        /// <summary>Calcule les variations entre deux états d'indicateurs.</summary>
        /// <param name="cycle">Cycle résolu.</param>
        /// <param name="before">Indicateurs avant la résolution.</param>
        /// <param name="after">Indicateurs après la résolution.</param>
        /// <param name="lines">Lignes du cycle.</param>
        /// <param name="endType">Fin éventuelle.</param>
        /// <returns>Le résultat rempli.</returns>
        public static CycleResult From(int cycle, IndicatorData before, IndicatorData after, List<JournalEntry> lines, EndType endType)
        {
            return new CycleResult
            {
                cycle = cycle,
                treasuryDelta = after.treasury - before.treasury,
                territoryDelta = after.territory - before.territory,
                pressureDelta = after.pressure - before.pressure,
                stabilityDelta = after.stability - before.stability,
                exposureDelta = after.exposure - before.exposure,
                lines = lines ?? new List<JournalEntry>(),
                endType = endType
            };
        }
    }
}

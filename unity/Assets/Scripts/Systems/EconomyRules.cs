// MARSEILLE — Systems/EconomyRules.cs
// Calculs économiques sans état (spec §5) : revenu et maintien d'un quartier, entretien des opérateurs.
// Partagés par EconomySystem (résolution) et EconomyActions / l'UI (prévisions).

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Formules économiques.</summary>
    public static class EconomyRules
    {
        /// <summary>Revenu potentiel brut d'un quartier : somme des charbons et commerces, investissement compris.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Potentiel en € par cycle (tous propriétaires confondus).</returns>
        public static float Potential(QuarterData q, GameConfig config)
        {
            float total = 0f;
            for (int i = 0; i < q.pois.Count; i++) total += q.pois[i].revenue;
            return total * (1f + q.investmentLevel * config.investIncomeGain);
        }

        /// <summary>Revenu du joueur dans un quartier pour ce cycle.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="hasPressureOperator">Un opérateur Pression est sur place.</param>
        /// <param name="seasonMultiplier">Multiplicateur des saisons pour la zone du quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Revenu en €.</returns>
        public static float QuarterIncome(QuarterData q, bool hasPressureOperator, float seasonMultiplier, GameConfig config)
        {
            if (q.control <= 0f) return 0f;
            float producing = 0f;
            for (int i = 0; i < q.pois.Count; i++) if (q.pois[i].IsProducingForPlayer()) producing += q.pois[i].revenue;
            float efficiency = hasPressureOperator ? 1f : config.incomeWithoutPressure;
            return producing * (1f + q.investmentLevel * config.investIncomeGain) * (q.control / 100f) * efficiency * seasonMultiplier;
        }

        /// <summary>Coût de maintien d'un quartier tenu, après réductions des opérateurs.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="reduction">Réduction cumulée des tâches sur place (0-1, bornée par la configuration).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Coût en €.</returns>
        public static float QuarterMaintenance(QuarterData q, float reduction, GameConfig config)
        {
            if (q.control <= 0f) return 0f;
            float r = System.Math.Min(config.maxMaintenanceReduction, System.Math.Max(0f, reduction));
            return q.maintenanceCost * (1f + q.investmentLevel * config.investMaintenanceGain) * (1f - r);
        }

        /// <summary>Entretien total des opérateurs, avec majoration progressive par opérateur au-delà du premier.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Entretien en €.</returns>
        public static float Upkeep(GameData data, GameConfig config)
        {
            float total = 0f;
            int count = 0;
            for (int i = 0; i < data.operators.Count; i++)
            {
                if (data.operators[i].status == OperatorStatus.Lost) continue;
                total += data.operators[i].upkeepCost;
                count++;
            }
            return total * (1f + System.Math.Max(0, count - 1) * config.progressiveUpkeep);
        }

        /// <summary>Quartiers où se trouve un opérateur Pression actif (ni en trajet, ni épuisé, ni perdu).</summary>
        /// <param name="data">Partie.</param>
        /// <returns>Ensemble d'identifiants de quartiers.</returns>
        public static HashSet<int> PressureQuarters(GameData data)
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.type == OperatorType.Pressure && (op.status == OperatorStatus.Available || op.status == OperatorStatus.Occupied))
                    set.Add(op.currentQuarter);
            }
            return set;
        }

        /// <summary>Réduction de maintien cumulée par quartier, d'après les tâches en cours.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Réduction par identifiant de quartier.</returns>
        public static Dictionary<int, float> MaintenanceReductions(GameData data, GameConfig config)
        {
            Dictionary<int, float> map = new Dictionary<int, float>();
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.status != OperatorStatus.Occupied) continue;
                OperatorTask task = op.GetTask();
                TaskSettings t = config.GetTaskSettings(task);
                if (t == null || t.maintenanceReduction <= 0f) continue;
                float r = t.maintenanceReduction * OperatorRules.Efficiency(op, task, config);
                float prev;
                map.TryGetValue(op.currentQuarter, out prev);
                map[op.currentQuarter] = prev + r;
            }
            return map;
        }
    }
}

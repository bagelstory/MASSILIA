// MARSEILLE — Systems/IndicatorSystem.cs
// Étape 6 de la résolution : pression, exposition, stabilité, effectifs et capacité (spec §7, §9).

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système des indicateurs globaux.</summary>
    public sealed class IndicatorSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Indicators"; } }

        /// <summary>Fait évoluer les indicateurs et la capacité.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            IndicatorData ind = data.indicators;
            int gained = 0, heldWithOperator = 0;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                if (q.control <= 0f) continue;
                if (!context.Ledger.quartersHeldAtStart.Contains(q.id)) gained++;
                if (context.Ledger.quartersWithOperator.Contains(q.id)) heldWithOperator++;
            }
            ind.pressure += gained * c.pressurePerQuarterGained;
            if (ind.exposure > c.exposureAlert)
            {
                ind.pressure += c.exposureToPressure;
                context.Log("Exposition trop haute : la pression monte.", JournalType.Police);
            }
            ind.pressure -= c.pressureDecay + ind.pressure * c.pressureDecayRatio;
            ind.exposure -= c.exposureDecay + ind.exposure * c.exposureDecayRatio;
            ind.stability += (c.stabilityTarget - ind.stability) * c.stabilityDrift;
            ind.stability += System.Math.Min(c.stabilityHeldQuarter.y, heldWithOperator * c.stabilityHeldQuarter.x);
            int available = 0, total = 0;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.status == OperatorStatus.Lost) continue;
                total++;
                if (op.status != OperatorStatus.Unavailable && op.status != OperatorStatus.Jailed) available++;
                if (op.fatigue > c.stabilityFatigueThreshold) ind.stability -= c.stabilityFatiguePenalty;
            }
            int extraFlux = FluxActions.ActiveCount(data) - c.fluxComfortableCount;
            if (extraFlux > 0) ind.stability -= extraFlux * c.stabilityPerExtraFlux;
            ind.pressure = MathHelper.Clamp100(ind.pressure);
            ind.exposure = MathHelper.Clamp100(ind.exposure);
            ind.stability = MathHelper.Clamp100(ind.stability);
            ind.effectifsAvailable = available;
            ind.effectifsTotal = total;
            ind.territory = TerritoryRules.Territory(data);
            data.indicators = ind;
            UpdateCapacity(ref data, context);
        }

        /// <summary>Capacité : +1 à intervalle régulier et au premier passage de chaque seuil de territoire.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void UpdateCapacity(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            if (context.Cycle % c.capacityEveryCycles == 0)
            {
                data.capacity++;
                context.Log("Capacité +1 (" + data.capacity + ").", JournalType.Gain);
            }
            for (int i = 0; i < c.capacityTerritoryThresholds.Count; i++)
            {
                int threshold = c.capacityTerritoryThresholds[i];
                if (data.indicators.territory < threshold || data.capacityThresholdsReached.Contains(threshold)) continue;
                data.capacityThresholdsReached.Add(threshold);
                data.capacity++;
                context.Log("Territoire " + threshold + " % : capacité +1 (" + data.capacity + ").", JournalType.Gain);
            }
        }
    }
}

// MARSEILLE — Systems/OperatorSystem.cs
// Étape 1 de la résolution : les opérateurs voyagent, exécutent leur tâche, se fatiguent, progressent.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système des opérateurs (spec §4).</summary>
    public sealed class OperatorSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Operators"; } }

        /// <summary>Exécute les tâches de tous les opérateurs.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            for (int i = 0; i < data.quarters.Count; i++)
                if (data.quarters[i].control > 0f) context.Ledger.quartersHeldAtStart.Add(data.quarters[i].id);
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                ExecuteOne(ref op, ref data, context);
                data.operators[i] = op;
            }
            data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure);
            data.indicators.exposure = MathHelper.Clamp100(data.indicators.exposure);
        }

        /// <summary>Fait jouer un opérateur pour ce cycle.</summary>
        /// <param name="op">Opérateur modifié.</param>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void ExecuteOne(ref OperatorData op, ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            if (op.status == OperatorStatus.Lost || op.status == OperatorStatus.Jailed) return;
            if (op.status == OperatorStatus.Unavailable)
            {
                op.unavailableCycles--;
                if (op.unavailableCycles <= 0) { op.status = OperatorStatus.Available; op.fatigue = c.fatigueAfterExhaustion; op.SetTask(OperatorTask.Rest); }
                return;
            }
            if (op.status == OperatorStatus.InTransit)
            {
                op.transitCycles--;
                if (op.transitCycles > 0) return;
                op.currentQuarter = op.destinationQuarter;
                op.destinationQuarter = Owners.None;
                op.status = op.GetTask() == OperatorTask.Rest ? OperatorStatus.Available : OperatorStatus.Occupied;
                int qi0 = data.IndexOfQuarter(op.currentQuarter);
                context.Log(op.Label() + " arrive à " + (qi0 >= 0 ? data.quarters[qi0].name : "?") + ".", JournalType.Info, op.currentQuarter);
            }
            context.Ledger.quartersWithOperator.Add(op.currentQuarter);
            OperatorTask task = op.GetTask();
            if (task == OperatorTask.Rest)
            {
                op.fatigue = System.Math.Max(0f, op.fatigue - c.fatigueRecoveryPerRest);
                return;
            }
            TaskSettings t = c.GetTaskSettings(task);
            if (t == null) return;
            float e = OperatorRules.Efficiency(op, task, c);
            bool unstable = data.indicators.stability < c.lowStabilityThreshold;
            op.fatigue = MathHelper.Clamp100(op.fatigue + t.fatiguePerCycle * (unstable ? c.lowStabilityFatigueFactor : 1f));
            Progress(ref op, task, context);
            ApplyTask(op, task, t, e, ref data, context);
            if (op.fatigue >= 100f)
            {
                op.status = OperatorStatus.Unavailable;
                op.unavailableCycles = c.exhaustionCycles;
                context.Log(op.Label() + " est épuisé : indisponible " + c.exhaustionCycles + " cycle(s).", JournalType.Info, op.currentQuarter);
            }
        }

        /// <summary>Expérience et gain de compétence.</summary>
        /// <param name="op">Opérateur modifié.</param>
        /// <param name="task">Tâche effectuée.</param>
        /// <param name="context">Contexte.</param>
        private static void Progress(ref OperatorData op, OperatorTask task, CycleContext context)
        {
            OperatorTypeSettings spec = OperatorRules.SpecialistOf(task, context.Config);
            op.experience += context.Config.experiencePerCycle * (spec != null && spec.type == op.type ? 1f : 0.5f);
            if (op.experience >= 1f && op.competence < 5)
            {
                op.experience = 0f;
                op.competence++;
                context.Log(op.Label() + " progresse : compétence " + op.competence + ".", JournalType.Gain, op.currentQuarter);
            }
        }

        /// <summary>Applique l'effet d'une tâche au quartier, à ses voisins, aux flux ou aux indicateurs.</summary>
        /// <param name="op">Opérateur (lecture).</param>
        /// <param name="task">Tâche.</param>
        /// <param name="t">Paramètres de la tâche.</param>
        /// <param name="e">Efficacité.</param>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="context">Contexte.</param>
        private static void ApplyTask(OperatorData op, OperatorTask task, TaskSettings t, float e, ref GameData data, CycleContext context)
        {
            int qi = data.IndexOfQuarter(op.currentQuarter);
            if (qi < 0) return;
            QuarterData q = data.quarters[qi];
            switch (task)
            {
                case OperatorTask.Secure:
                    float removed = TerritoryRules.RemoveHostile(ref q, t.hostileReduction * e);
                    if (q.control > 0f || removed > 0f) TerritoryRules.GainControl(ref q, t.controlGain * e, context.Config);
                    break;
                case OperatorTask.Press:
                    TerritoryRules.GainControl(ref q, t.controlGain * e, context.Config);
                    break;
                case OperatorTask.Scout:
                    if (context.Chance(System.Math.Min(1f, t.knowledgeChance * e))) q.knowledgeLevel = System.Math.Min(3, q.knowledgeLevel + 1);
                    RevealNeighbours(q, ref data, t.neighbourRevealChance * e, context, false);
                    break;
                case OperatorTask.Watch:
                    q.knowledgeLevel = System.Math.Max(2, q.knowledgeLevel);
                    q.revealedUntilCycle = context.Cycle + 1;
                    RevealNeighbours(q, ref data, 1f, context, true);
                    break;
                case OperatorTask.ManageFlux:
                    for (int f = 0; f < data.fluxes.Count; f++)
                    {
                        if (!data.fluxes[f].isActive) continue;
                        FluxData flux = data.fluxes[f];
                        flux.reliability = MathHelper.Clamp100(flux.reliability + t.fluxReliabilityGain * e);
                        data.fluxes[f] = flux;
                    }
                    break;
                case OperatorTask.Optimize:
                    data.indicators.exposure -= t.exposureReduction * e;
                    break;
            }
            data.indicators.pressure += t.pressureAdded;
            data.indicators.exposure += t.exposureAdded;
            data.quarters[qi] = q;
        }

        /// <summary>Révèle les quartiers voisins (connaissance 1) et, en surveillance, leurs intentions adverses.</summary>
        /// <param name="q">Quartier d'origine.</param>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="chance">Probabilité par voisin.</param>
        /// <param name="context">Contexte.</param>
        /// <param name="revealIntentions">Vrai pour la surveillance (intentions visibles au cycle suivant).</param>
        private static void RevealNeighbours(QuarterData q, ref GameData data, float chance, CycleContext context, bool revealIntentions)
        {
            for (int n = 0; n < q.adjacentQuarters.Count; n++)
            {
                int j = data.IndexOfQuarter(q.adjacentQuarters[n]);
                if (j < 0 || !context.Chance(chance)) continue;
                QuarterData v = data.quarters[j];
                v.knowledgeLevel = System.Math.Max(1, v.knowledgeLevel);
                if (revealIntentions) v.revealedUntilCycle = context.Cycle + 1;
                data.quarters[j] = v;
            }
        }
    }
}

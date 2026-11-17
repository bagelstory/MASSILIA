// MARSEILLE — Systems/ShadowSystem.cs
// Étape « ombre » de la résolution (après les indicateurs) : le parloir, les ordres, les représailles.
// La suite (peur, terreur, argent sale, jauge Ombre) est dans ShadowSystem.City.cs.
// On ne montre jamais l'acte : seulement ce qu'il laisse (une ligne, de la peur, une dette).

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système de l'ombre (design/11-bible-ombre.md §4).</summary>
    public sealed partial class ShadowSystem : ICycleSystem
    {
        /// <summary>Ordres résolus pendant ce cycle (pour la jauge Ombre).</summary>
        private int ordersThisCycle;

        /// <inheritdoc />
        public string Name { get { return "Shadow"; } }

        /// <summary>Résout l'ombre du cycle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            if (data.shadow.debts == null || data.shadow.orders == null) data.shadow = ShadowData.Create(data.indicators.treasury);
            ordersThisCycle = 0;
            Prisoners(ref data, context);
            Orders(ref data, context);
            Reprisals(ref data, context);
            City(ref data, context);
            Ombre(ref data, context);
        }

        /// <summary>Le parloir : la détention avance, la famille paie les trajets, certains sortent.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Prisoners(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.status != OperatorStatus.Jailed) continue;
                op.jailServed++;
                op.jailCycles--;
                Pay(ref data, c.parloirFamilyCost);
                if (op.jailCycles <= 0)
                {
                    op.status = OperatorStatus.Available;
                    op.fatigue = c.jailReleaseFatigue;
                    if (data.IndexOfQuarter(data.baseQuarterId) >= 0) op.currentQuarter = data.baseQuarterId;
                    context.Log(ShadowRules.Name(op) + " sort des Baumettes. Dehors, tout le monde a vieilli. Sauf les murs.", JournalType.Shadow, op.currentQuarter);
                }
                data.operators[i] = op;
            }
        }

        /// <summary>Résout les ordres arrivés à échéance.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private void Orders(ref GameData data, CycleContext context)
        {
            ShadowData s = data.shadow;
            for (int i = s.orders.Count - 1; i >= 0; i--)
            {
                PendingOrder order = s.orders[i];
                if (order.resolveCycle > context.Cycle) continue;
                s.orders.RemoveAt(i);
                data.shadow = s;
                ResolveOrder(ref data, order, context);
                s = data.shadow;
                ordersThisCycle++;
            }
            data.shadow = s;
        }

        /// <summary>Un ordre : compris ou non, réussi ou non, il laisse toujours une dette et de la peur.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="order">Ordre résolu.</param>
        /// <param name="context">Contexte.</param>
        private static void ResolveOrder(ref GameData data, PendingOrder order, CycleContext context)
        {
            GameConfig c = context.Config;
            int qi = data.IndexOfQuarter(order.quarterId);
            int fi = data.IndexOfFaction(order.factionId);
            if (qi < 0 || fi < 0 || !data.factions[fi].isAlive) { context.Log("L'ordre se perd : il n'y a plus personne en face.", JournalType.Shadow); return; }
            string where = data.quarters[qi].name;
            ShadowData s = data.shadow;
            s.AddDebt(order.factionId, 1);
            s.ordersGiven++;
            data.shadow = s;
            data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + c.orderPressure);
            ShadowRules.AddFear(ref data, order.quarterId, c.orderFear);
            if (order.fromOperatorId >= 0 && !context.Chance(order.reliability))
            {
                data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + c.orderPressure);
                context.Log(where + " : le message du parloir s'est perdu en route. Quelqu'un a fait autre chose, et ça se sait.", JournalType.Shadow, order.quarterId);
                return;
            }
            FactionData f = data.factions[fi];
            float force = ShadowRules.PlayerForce(data, c);
            if (!context.Chance(force / (force + System.Math.Max(1, f.effectifs))))
            {
                context.Log(where + " : l'ordre n'a pas abouti. Tout le monde sait d'où il venait.", JournalType.Shadow, order.quarterId);
                return;
            }
            f.effectifs = System.Math.Max(0, f.effectifs - c.orderEffectifsHit);
            data.factions[fi] = f;
            HitPresence(ref data, qi, order.factionId, c.orderPresenceHit);
            int age = context.Random.Next(19, 45);
            ShadowRules.RegisterDeath(ref data, order.quarterId, where + " : un homme de " + age + " ans ne rentrera pas. Les volets restent fermés. " + f.name + " n'oubliera pas.", context);
        }

        /// <summary>Retire de la présence à une bande dans un quartier.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="qi">Index du quartier.</param>
        /// <param name="factionId">Bande.</param>
        /// <param name="amount">Présence retirée.</param>
        private static void HitPresence(ref GameData data, int qi, int factionId, float amount)
        {
            QuarterData q = data.quarters[qi];
            for (int p = 0; p < q.factionPresences.Count; p++)
            {
                if (q.factionPresences[p].factionId != factionId) continue;
                FactionPresence fp = q.factionPresences[p];
                fp.value = System.Math.Max(0f, fp.value - amount);
                q.factionPresences[p] = fp;
            }
            q.RecomputeHostilePresence();
            data.quarters[qi] = q;
        }

        /// <summary>Paie une somme : ce qui manque devient de la dette.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="amount">Montant (€).</param>
        private static void Pay(ref GameData data, float amount)
        {
            float paid = System.Math.Min(data.indicators.treasury, amount);
            data.indicators.treasury -= paid;
            data.debt += amount - paid;
        }
    }
}

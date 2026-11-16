// MARSEILLE — Systems/FactionRules.cs
// Calculs de l'IA des factions (spec §5.3) : revenu, force d'une action, défense du joueur, alliés,
// choix de l'objectif et des quartiers visés.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Règles des factions, sans état.</summary>
    public static class FactionRules
    {
        /// <summary>Revenu d'une faction : potentiel de chaque quartier × sa présence.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="factionId">Faction.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Revenu en €.</returns>
        public static float Income(GameData data, int factionId, GameConfig config)
        {
            float total = 0f;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                float p = data.quarters[i].PresenceOf(factionId);
                if (p > 0f) total += EconomyRules.Potential(data.quarters[i], config) * p / 100f;
            }
            return total * config.factionIncomeRatio;
        }

        /// <summary>Force d'une action : base + agressivité + effectif, avec un facteur aléatoire.</summary>
        /// <param name="f">Faction.</param>
        /// <param name="context">Contexte (aléatoire).</param>
        /// <returns>Présence que l'action cherche à gagner.</returns>
        public static float ActionForce(FactionData f, CycleContext context)
        {
            GameConfig c = context.Config;
            float baseForce = c.factionActionForce.x + f.aggressiveness * c.factionActionForce.y + f.effectifs * c.factionActionForce.z;
            return baseForce * context.Range(c.factionActionRandom.x, c.factionActionRandom.y);
        }

        /// <summary>Part d'une attaque annulée par les opérateurs du joueur sur place (sécuriser, surveiller).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier attaqué.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Défense 0 à factionMaxDefense.</returns>
        public static float Defense(GameData data, int quarterId, GameConfig config)
        {
            float d = 0f;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.status != OperatorStatus.Occupied || op.currentQuarter != quarterId) continue;
                OperatorTask task = op.GetTask();
                TaskSettings t = config.GetTaskSettings(task);
                if (t != null && t.attackReduction > 0f) d += t.attackReduction * OperatorRules.Efficiency(op, task, config);
            }
            return System.Math.Min(config.factionMaxDefense, d);
        }

        /// <summary>Factions en trêve avec une faction.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="factionId">Faction.</param>
        /// <returns>Ensemble des alliées.</returns>
        public static HashSet<int> AlliesOf(GameData data, int factionId)
        {
            HashSet<int> set = new HashSet<int>();
            int i = data.IndexOfFaction(factionId);
            if (i >= 0 && data.factions[i].allianceCyclesLeft > 0) set.Add(data.factions[i].allyFactionId);
            return set;
        }

        /// <summary>Choisit l'objectif du cycle et les quartiers visés (contre-attaque, repli, extension, maintien).</summary>
        /// <param name="f">Faction modifiée (objectif, cible).</param>
        /// <param name="data">Partie.</param>
        /// <param name="startBudget">Budget de départ de la faction (configuration).</param>
        /// <param name="context">Contexte.</param>
        /// <returns>Quartiers visés, au plus un par tranche d'effectif.</returns>
        public static List<int> Decide(ref FactionData f, GameData data, float startBudget, CycleContext context)
        {
            GameConfig c = context.Config;
            List<int> targets = new List<int>();
            bool calm = context.Cycle <= c.factionCalmCycles;
            float counterChance = f.aggressiveness * c.factionCounterChancePerAggressiveness + c.factionCounterChanceBase;
            if (!calm)
                for (int t = 0; t < f.territory.Count; t++)
                {
                    int qi = data.IndexOfQuarter(f.territory[t]);
                    if (qi >= 0 && data.quarters[qi].control > c.factionAttackDetectControl && context.Chance(counterChance)) targets.Add(f.territory[t]);
                }
            float retreatLine = c.factionBudgetMax * c.factionRetreatBudget * (startBudget / c.factionBudgetMax + 0.5f);
            if (targets.Count > 0) f.objective = FactionObjective.CounterAttack;
            else if (f.budget < retreatLine) f.objective = FactionObjective.Retract;
            else
            {
                List<int> weak = WeakNeighbours(f, data, calm, c);
                if (weak.Count > 0 && context.Chance(c.factionExpandChance.x + f.aggressiveness * c.factionExpandChance.y))
                {
                    f.objective = FactionObjective.Expand;
                    targets = weak;
                }
                else f.objective = FactionObjective.Maintain;
            }
            int max = System.Math.Max(1, (f.effectifs + c.factionActionsPerEffectif - 1) / c.factionActionsPerEffectif);
            if (targets.Count > max) targets.RemoveRange(max, targets.Count - max);
            f.targetQuarter = targets.Count > 0 ? targets[0] : Owners.None;
            return targets;
        }

        /// <summary>Voisins de son territoire où la faction est faible et le joueur peu installé, les plus vides d'abord.</summary>
        /// <param name="f">Faction.</param>
        /// <param name="data">Partie.</param>
        /// <param name="calm">Période calme : le joueur n'est pas visé.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Quartiers candidats triés.</returns>
        private static List<int> WeakNeighbours(FactionData f, GameData data, bool calm, GameConfig c)
        {
            List<int> list = new List<int>();
            HashSet<int> allies = AlliesOf(data, f.id);
            for (int t = 0; t < f.territory.Count; t++)
            {
                int qi = data.IndexOfQuarter(f.territory[t]);
                if (qi < 0) continue;
                List<int> adj = data.quarters[qi].adjacentQuarters;
                for (int n = 0; n < adj.Count; n++)
                {
                    int vi = data.IndexOfQuarter(adj[n]);
                    if (vi < 0 || list.Contains(adj[n])) continue;
                    QuarterData v = data.quarters[vi];
                    if (v.PresenceOf(f.id) >= c.factionExpandTargetPresence || v.control >= c.factionExpandMaxPlayerControl) continue;
                    if (calm && v.control > 0f) continue;
                    bool allyHolds = false;
                    foreach (int a in allies) if (v.PresenceOf(a) > c.factionExpandTargetPresence) allyHolds = true;
                    if (!allyHolds) list.Add(adj[n]);
                }
            }
            list.Sort((a, b) => Weight(data, a).CompareTo(Weight(data, b)));
            return list;
        }

        /// <summary>Occupation d'un quartier (contrôle + présence adverse) : plus elle est faible, plus il est visé.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <returns>Poids.</returns>
        private static float Weight(GameData data, int quarterId)
        {
            QuarterData q = data.quarters[data.IndexOfQuarter(quarterId)];
            return q.control + q.hostilePresence;
        }
    }
}

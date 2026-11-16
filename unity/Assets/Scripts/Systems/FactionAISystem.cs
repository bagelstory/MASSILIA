// MARSEILLE — Systems/FactionAISystem.cs
// Étape 3 de la résolution : chaque faction vivante encaisse et paie, recrute parfois, choisit un objectif,
// puis agit sur les quartiers visés. Trêves et fragmentation sont gérées par FactionDiplomacy.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système d'IA des factions (spec §5).</summary>
    public sealed class FactionAISystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "FactionAI"; } }

        /// <summary>Fait jouer toutes les factions.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            for (int i = 0; i < data.factions.Count; i++)
            {
                FactionData f = data.factions[i];
                if (!f.isAlive) continue;
                PlayOne(ref f, ref data, context);
                data.factions[i] = f;
            }
            FactionDiplomacy.Update(ref data, context);
        }

        /// <summary>Tour complet d'une faction.</summary>
        /// <param name="f">Faction modifiée.</param>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void PlayOne(ref FactionData f, ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            float income = FactionRules.Income(data, f.id, c);
            f.budget = MathHelper.Clamp(f.budget + income - f.effectifs * c.factionCostPerEffectif, 0f, c.factionBudgetMax);
            if (f.effectifs < c.factionEffectifMax && f.budget > c.factionRecruitThreshold + c.factionRecruitCost && context.Chance(c.factionRecruitChance))
            {
                f.budget -= c.factionRecruitCost;
                f.effectifs++;
            }
            FactionSettings settings = FindSettings(c, f.id);
            List<int> targets = FactionRules.Decide(ref f, data, settings != null ? settings.budget : f.budget, context);
            if (f.objective == FactionObjective.Retract) Retract(ref f, ref data, context);
            else if (f.objective == FactionObjective.Maintain && f.aggressiveness < f.baseAggressiveness && context.Chance(c.factionAggressivenessShift.y))
                f.aggressiveness++;
            HashSet<int> allies = FactionRules.AlliesOf(data, f.id);
            for (int t = 0; t < targets.Count; t++)
            {
                if (f.budget < c.factionActionCost) break;
                f.budget -= c.factionActionCost;
                Act(f, targets[t], allies, ref data, context);
            }
        }

        /// <summary>Repli : la faction lâche ses positions faibles et se calme parfois.</summary>
        /// <param name="f">Faction modifiée.</param>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Retract(ref FactionData f, ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                float p = q.PresenceOf(f.id);
                if (p <= 0f || p >= c.factionRetreatPresence.x) continue;
                for (int k = 0; k < q.factionPresences.Count; k++)
                {
                    if (q.factionPresences[k].factionId != f.id) continue;
                    FactionPresence fp = q.factionPresences[k];
                    fp.value = System.Math.Max(0f, fp.value - c.factionRetreatPresence.y);
                    q.factionPresences[k] = fp;
                }
                q.factionPresences.RemoveAll(x => x.value <= 0.01f);
                q.RecomputeHostilePresence();
                data.quarters[i] = q;
            }
            if (f.aggressiveness > 1 && context.Chance(c.factionAggressivenessShift.x)) f.aggressiveness--;
        }

        /// <summary>Une action sur un quartier : présence gagnée, contrôle du joueur perdu, ligne au journal.</summary>
        /// <param name="f">Faction (lecture).</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="allies">Factions en trêve, épargnées.</param>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="context">Contexte.</param>
        private static void Act(FactionData f, int quarterId, HashSet<int> allies, ref GameData data, CycleContext context)
        {
            int qi = data.IndexOfQuarter(quarterId);
            if (qi < 0) return;
            QuarterData q = data.quarters[qi];
            float force = FactionRules.ActionForce(f, context) * (1f - FactionRules.Defense(data, quarterId, context.Config));
            float before = q.control;
            float loss = TerritoryRules.AddFactionPresence(ref q, f.id, force, allies);
            data.quarters[qi] = q;
            if (loss > 0.5f)
            {
                context.Log(f.name + " attaque " + q.name + " : contrôle −" + loss.ToString("0") + " %.", JournalType.Attack, q.id);
                if (q.control <= 0f && before > 0f) context.Log(q.name + " est perdu.", JournalType.Loss, q.id);
            }
            else if (before <= 0f && q.knowledgeLevel > 0 && force > 3f && context.Chance(0.3f))
                context.Log(f.name + " avance sur " + q.name + ".", JournalType.Faction, q.id);
        }

        /// <summary>Paramètres de départ d'une faction.</summary>
        /// <param name="c">Configuration.</param>
        /// <param name="factionId">Faction.</param>
        /// <returns>Paramètres, ou null.</returns>
        private static FactionSettings FindSettings(GameConfig c, int factionId)
        {
            for (int i = 0; i < c.factions.Count; i++) if (c.factions[i].id == factionId) return c.factions[i];
            return null;
        }
    }
}

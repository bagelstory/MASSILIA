// MARSEILLE — Systems/ShadowSystem.City.cs
// Suite de ShadowSystem : représailles de la Dette de sang, peur et terreur qui vident les quartiers,
// argent sale (liquide qui expose, placements qui contaminent), et la jauge Ombre avec ses paliers.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    public sealed partial class ShadowSystem
    {
        /// <summary>Les dettes de sang reviennent : sur un équipier, ou sur un proche.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Reprisals(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            float factor = ShadowRules.Tier(data.shadow.ombre, c) >= 4 ? c.ombreReprisalFactor : 1f;
            for (int d = 0; d < data.shadow.debts.Count; d++)
            {
                BloodDebt debt = data.shadow.debts[d];
                int fi = data.IndexOfFaction(debt.factionId);
                if (fi < 0 || !data.factions[fi].isAlive || debt.debt <= 0) continue;
                float chance = System.Math.Min(c.reprisalMaxChance, debt.debt * c.reprisalPerDebt * factor);
                if (!context.Chance(chance)) continue;
                List<int> exposed = new List<int>();
                for (int i = 0; i < data.operators.Count; i++) if (OperatorRules.IsOnSite(data.operators[i])) exposed.Add(i);
                if (exposed.Count == 0) continue;
                int index = exposed[context.Random.Next(exposed.Count)];
                OperatorData op = data.operators[index];
                string name = ShadowRules.Name(op);
                string from = data.factions[fi].name;
                if (context.Chance(c.reprisalLethalShare))
                {
                    OperatorRules.Lose(ref data, index, "il n'est pas rentré", context);
                    ShadowRules.RegisterDeath(ref data, op.currentQuarter, name + " ne répond plus. Sa mère a laissé l'assiette sur la table. " + from + " a rendu la dette.", context);
                }
                else
                {
                    data.indicators.stability = System.Math.Max(0f, data.indicators.stability - c.reprisalStabilityHit);
                    ShadowRules.AddFear(ref data, op.currentQuarter, c.orderFear);
                    context.Log("Un proche de " + name + " a été visé. Il est vivant. " + name + " ne dort plus. Signé " + from + ".", JournalType.Shadow, op.currentQuarter);
                }
            }
        }

        /// <summary>La ville : la peur retombe lentement, la terreur vide les rues, l'argent sale ronge les commerces.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void City(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            bool shopTick = context.Cycle % c.contaminationShopCycles == 0;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                q.fear = MathHelper.Clamp100(q.fear - c.fearDecay);
                q.highFearCycles = q.fear > c.terrorThreshold ? q.highFearCycles + 1 : 0;
                if (q.highFearCycles >= c.terrorDelay)
                {
                    q.population -= c.terrorPopulationLoss;
                    if (q.highFearCycles == c.terrorDelay) context.Log(q.name + " : des familles partent. Les fenêtres s'éteignent une à une.", JournalType.Shadow, q.id);
                }
                if (q.contamination > c.contaminationThreshold)
                {
                    q.population -= c.contaminationPopulationLoss;
                    if (shopTick)
                    {
                        q.closedShops++;
                        context.Log(q.name + " : un commerce de toujours baisse le rideau. Le loyer a triplé, les vitrines d'à côté sont neuves.", JournalType.Shadow, q.id);
                    }
                }
                q.population = MathHelper.Clamp(q.population, c.populationFloor, 100f);
                data.quarters[i] = q;
            }
            float excess = data.indicators.treasury - c.dirtyCashThreshold;
            if (excess <= 0f) return;
            data.indicators.exposure = MathHelper.Clamp100(data.indicators.exposure + excess / c.dirtyCashPerExposure);
            if (shopTick) context.Log("Trop de liquide dort dans des sacs. Ça finit toujours par se voir.", JournalType.Shadow);
        }

        /// <summary>La jauge Ombre monte avec la puissance et ne descend pas ; ses paliers assombrissent tout.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private void Ombre(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ShadowData s = data.shadow;
            float fear = AverageHeldFear(data);
            float territoryGain = System.Math.Max(0f, data.indicators.territory - s.lastTerritory);
            float fearGain = System.Math.Max(0f, fear - s.lastFear);
            float treasuryGain = System.Math.Max(0f, (data.indicators.treasury - s.lastTreasury) / System.Math.Max(1f, s.lastTreasury) * 100f);
            int before = ShadowRules.Tier(s.ombre, c);
            s.ombre += territoryGain * c.ombrePerTerritory + fearGain * c.ombrePerFear + ordersThisCycle * c.ombrePerOrder + treasuryGain * c.ombrePerTreasuryPercent;
            s.ombre = MathHelper.Clamp(System.Math.Max(s.ombre, s.deaths * c.ombrePerDeath), 0f, 100f);
            s.lastTerritory = data.indicators.territory;
            s.lastTreasury = data.indicators.treasury;
            s.lastFear = fear;
            data.shadow = s;
            int tier = ShadowRules.Tier(s.ombre, c);
            if (tier >= 2) data.indicators.stability = System.Math.Min(data.indicators.stability, c.ombreNightStabilityCap);
            if (tier >= 3) data.indicators.stability = System.Math.Max(0f, data.indicators.stability - c.ombreDarkStabilityLoss);
            if (tier > before) context.Log(TierLine(tier), JournalType.Shadow);
        }

        /// <summary>Peur moyenne des quartiers où le joueur a du contrôle.</summary>
        /// <param name="data">Partie.</param>
        /// <returns>Peur moyenne (0-100), 0 si aucun quartier.</returns>
        private static float AverageHeldFear(GameData data)
        {
            float total = 0f;
            int n = 0;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                if (data.quarters[i].control <= 0f) continue;
                total += data.quarters[i].fear;
                n++;
            }
            return n > 0 ? total / n : 0f;
        }

        /// <summary>Ce que le journal dit quand l'Ombre passe un palier.</summary>
        /// <param name="tier">Nouveau palier.</param>
        /// <returns>Une ligne.</returns>
        private static string TierLine(int tier)
        {
            switch (tier)
            {
                case 1: return "Crépuscule. Madame Ferrand ne répond plus à ton bonjour.";
                case 2: return "Nuit. Plus personne ne t'appelle par ton prénom.";
                case 3: return "Nuit noire. L'équipe ne dit plus « nous ».";
                default: return "Ombre. Chaque euro a un visage. Il ne reste que le vent.";
            }
        }
    }
}

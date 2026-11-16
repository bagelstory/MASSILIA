// MARSEILLE — Systems/TerritorySystem.cs
// Étape 2 de la résolution : mise à jour du contrôle (abandon, manque d'argent, stabilité basse),
// des charbons (pertes, prises par les factions, saisies) et du territoire de chaque faction.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système de territoire (spec §3.3).</summary>
    public sealed class TerritorySystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Territory"; } }

        /// <summary>Met à jour contrôle, charbons et territoires.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            bool broke = data.indicators.treasury <= 0f && data.debt > 0f;
            bool unstable = data.indicators.stability < c.lowStabilityThreshold;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                q.RecomputeHostilePresence();
                UpdateAbandonment(ref q, context, broke, unstable);
                UpdateCharbons(ref q, ref data, context);
                data.quarters[i] = q;
                if (q.control > 0f && !context.Ledger.quartersHeldAtStart.Contains(q.id))
                    context.Log("Nouveau quartier : " + q.name + ".", JournalType.Gain, q.id);
                else if (q.control <= 0f && context.Ledger.quartersHeldAtStart.Contains(q.id))
                    context.Log(q.name + " est perdu.", JournalType.Loss, q.id);
            }
            UpdateFactionTerritories(ref data, c);
            data.indicators.territory = TerritoryRules.Territory(data);
        }

        /// <summary>Fait s'effriter le contrôle d'un quartier sans opérateur, sans argent ou instable.</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="context">Contexte.</param>
        /// <param name="broke">Trésorerie à zéro avec dette.</param>
        /// <param name="unstable">Stabilité sous le seuil.</param>
        private static void UpdateAbandonment(ref QuarterData q, CycleContext context, bool broke, bool unstable)
        {
            GameConfig c = context.Config;
            if (q.control <= 0f) { q.cyclesWithoutOperator = 0; return; }
            if (context.Ledger.quartersWithOperator.Contains(q.id)) q.cyclesWithoutOperator = 0;
            else q.cyclesWithoutOperator++;
            if (q.cyclesWithoutOperator > c.abandonGraceCycles)
            {
                // Un quartier sécurisé tient seul (plancher), les autres s'effritent.
                if (TerritoryRules.Status(q, c) == QuarterStatus.Secured)
                    q.control = System.Math.Max(c.securedControl, q.control - c.securedAbandonDecay);
                else
                    q.control = System.Math.Max(0f, q.control - c.abandonDecay);
            }
            if (broke) q.control = System.Math.Max(0f, q.control - c.noMoneyDecay);
            if (unstable) q.control = System.Math.Max(0f, q.control - c.lowStabilityDecay);
        }

        /// <summary>Propriété des charbons selon le rapport de force, chaleur des charbons ouverts, saisies.</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="data">Partie (indicateurs modifiés).</param>
        /// <param name="context">Contexte.</param>
        private static void UpdateCharbons(ref QuarterData q, ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            int dominant = q.DominantFaction();
            for (int p = 0; p < q.pois.Count; p++)
            {
                PoiData poi = q.pois[p];
                if (poi.kind != PoiKind.Charbon) continue;
                if (poi.seizedCycles > 0) { poi.seizedCycles--; q.pois[p] = poi; continue; }
                if (poi.ownerId == Owners.Player && q.control < c.charbonLossControl)
                {
                    poi.ownerId = dominant;
                    poi.isOpen = dominant != Owners.None;
                    context.Log("Charbon perdu : " + poi.name + ", " + q.name + ".", JournalType.Loss, q.id);
                }
                else if (Owners.IsFaction(poi.ownerId) && q.PresenceOf(poi.ownerId) < 5f)
                {
                    poi.ownerId = Owners.None;
                    poi.isOpen = false;
                }
                else if (poi.ownerId == Owners.None && dominant != Owners.None
                         && q.PresenceOf(dominant) >= c.charbonFactionClaimPresence && context.Chance(c.charbonFactionClaimChance))
                {
                    poi.ownerId = dominant;
                    poi.isOpen = true;
                }
                if (poi.IsProducingForPlayer())
                {
                    data.indicators.pressure += poi.risk * c.charbonPressurePerRisk;
                    data.indicators.exposure += poi.risk * c.charbonExposurePerRisk;
                    float chance = c.charbonSeizureBase * poi.risk * (data.indicators.pressure / 50f);
                    if (context.Chance(chance))
                    {
                        poi.seizedCycles = c.charbonSeizureCycles;
                        context.Log("Saisie de police : " + poi.name + ", " + q.name + " fermé " + c.charbonSeizureCycles + " cycles.", JournalType.Police, q.id);
                    }
                }
                q.pois[p] = poi;
            }
        }

        /// <summary>Recalcule la liste des quartiers de chaque faction (présence ≥ seuil).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private static void UpdateFactionTerritories(ref GameData data, GameConfig c)
        {
            for (int f = 0; f < data.factions.Count; f++)
            {
                FactionData faction = data.factions[f];
                if (faction.territory == null) faction.territory = new List<int>();
                faction.territory.Clear();
                for (int i = 0; i < data.quarters.Count; i++)
                    if (data.quarters[i].PresenceOf(faction.id) >= c.factionTerritoryThreshold) faction.territory.Add(data.quarters[i].id);
                if (faction.territory.Count > faction.peakTerritory) faction.peakTerritory = faction.territory.Count;
                data.factions[f] = faction;
            }
            data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure);
            data.indicators.exposure = MathHelper.Clamp100(data.indicators.exposure);
        }
    }
}

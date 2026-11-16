// MARSEILLE — Systems/FactionDiplomacy.cs
// Trêves temporaires entre factions, fragmentation d'une faction qui s'effondre, disparition (spec §5.4).

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Relations entre factions et fin des factions.</summary>
    public static class FactionDiplomacy
    {
        /// <summary>Met à jour trêves, fragmentation et disparition.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="context">Contexte.</param>
        public static void Update(ref GameData data, CycleContext context)
        {
            UpdateTruces(ref data, context);
            MaybeNewTruce(ref data, context);
            for (int i = 0; i < data.factions.Count; i++)
            {
                FactionData f = data.factions[i];
                if (!f.isAlive) continue;
                CheckCollapse(ref f, data, context);
                data.factions[i] = f;
            }
        }

        /// <summary>Fait avancer les trêves en cours et annonce leur fin.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void UpdateTruces(ref GameData data, CycleContext context)
        {
            for (int i = 0; i < data.factions.Count; i++)
            {
                FactionData f = data.factions[i];
                if (f.allianceCyclesLeft <= 0) continue;
                f.allianceCyclesLeft--;
                if (f.allianceCyclesLeft == 0)
                {
                    int other = data.IndexOfFaction(f.allyFactionId);
                    if (other >= 0 && f.id < f.allyFactionId)
                        context.Log("Fin de la trêve entre " + f.name + " et " + data.factions[other].name + ".", JournalType.Faction);
                    f.allyFactionId = Owners.None;
                }
                data.factions[i] = f;
            }
        }

        /// <summary>Forme parfois une trêve entre deux factions vivantes sans trêve.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void MaybeNewTruce(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            if (!context.Chance(c.factionAllianceChance)) return;
            List<int> free = new List<int>();
            for (int i = 0; i < data.factions.Count; i++)
                if (data.factions[i].isAlive && data.factions[i].allianceCyclesLeft <= 0) free.Add(i);
            if (free.Count < 2) return;
            int a = free[context.Random.Next(free.Count)];
            free.Remove(a);
            int b = free[context.Random.Next(free.Count)];
            int duration = context.Random.Next(c.factionAllianceDuration.x, c.factionAllianceDuration.y + 1);
            FactionData fa = data.factions[a], fb = data.factions[b];
            fa.allyFactionId = fb.id; fa.allianceCyclesLeft = duration;
            fb.allyFactionId = fa.id; fb.allianceCyclesLeft = duration;
            data.factions[a] = fa;
            data.factions[b] = fb;
            context.Log("Trêve entre " + fa.name + " et " + fb.name + ".", JournalType.Faction);
        }

        /// <summary>Fragmentation (territoire tombé sous une part de son pic) ou disparition (plus de présence).</summary>
        /// <param name="f">Faction modifiée.</param>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void CheckCollapse(ref FactionData f, GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            float total = 0f;
            for (int q = 0; q < data.quarters.Count; q++) total += data.quarters[q].PresenceOf(f.id);
            int size = f.territory.Count;
            if (size > f.peakTerritory) f.peakTerritory = size;
            if (total < c.factionMinPresence)
            {
                f.isAlive = false;
                f.allianceCyclesLeft = 0;
                f.allyFactionId = Owners.None;
                context.Log(f.name + " a disparu de la ville.", JournalType.Faction);
            }
            else if (!f.isFragmented && f.peakTerritory >= c.factionFragmentationMinPeak && size > 0
                     && size <= f.peakTerritory * c.factionFragmentationRatio)
            {
                f.isFragmented = true;
                f.effectifs = System.Math.Max(1, (int)System.Math.Round(f.effectifs * c.factionFragmentationEffectifs));
                f.aggressiveness = System.Math.Min(5, f.aggressiveness + 1);
                context.Log(f.name + " se divise : moins nombreux, plus nerveux.", JournalType.Faction);
            }
        }
    }
}

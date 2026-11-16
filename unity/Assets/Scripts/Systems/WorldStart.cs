// MARSEILLE — Systems/WorldStart.cs
// Point de départ du joueur : zone de départ tirée au sort parmi les zones possibles, quartier de base
// (contrôle initial, faction repoussée, charbons ouverts), voisins connus, opérateurs de départ.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Placement du joueur au début de la partie.</summary>
    public static class WorldStart
    {
        /// <summary>Choisit la zone et le quartier de base, et y installe le joueur.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        /// <param name="nextPoiId">Compteur d'identifiants de points d'intérêt.</param>
        public static void PlaceBase(ref GameData data, GameConfig config, System.Random rng, ref int nextPoiId)
        {
            List<int> starts = new List<int>();
            for (int z = 0; z < data.zones.Count; z++) if (data.zones[z].canBeStart && data.zones[z].quarterIds.Count > 0) starts.Add(z);
            if (starts.Count == 0 || data.quarters.Count == 0) return;
            int zi = starts[rng.Next(starts.Count)];
            ZoneData zone = data.zones[zi];
            zone.isUnlocked = true;
            data.zones[zi] = zone;
            int baseId = zone.quarterIds[rng.Next(zone.quarterIds.Count)];
            int qi = data.IndexOfQuarter(baseId);
            QuarterData q = data.quarters[qi];
            float excess = q.hostilePresence - config.baseHostilePresence;
            if (excess > 0f) TerritoryRules.RemoveHostile(ref q, excess);
            q.control = System.Math.Min(config.baseControl, q.FreeShare());
            q.knowledgeLevel = 3;
            OpenBaseCharbons(ref q, ref nextPoiId, config, rng);
            data.quarters[qi] = q;
            data.baseQuarterId = baseId;
            for (int n = 0; n < q.adjacentQuarters.Count; n++)
            {
                int j = data.IndexOfQuarter(q.adjacentQuarters[n]);
                if (j < 0) continue;
                QuarterData v = data.quarters[j];
                v.knowledgeLevel = System.Math.Max(1, v.knowledgeLevel);
                data.quarters[j] = v;
            }
        }

        /// <summary>Donne au joueur les charbons de base (repris aux factions ou créés s'il en manque).</summary>
        /// <param name="q">Quartier de base.</param>
        /// <param name="nextPoiId">Compteur d'identifiants.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        private static void OpenBaseCharbons(ref QuarterData q, ref int nextPoiId, GameConfig config, System.Random rng)
        {
            ZoneSettings zs = config.GetZoneSettings(q.zoneId);
            float reference = zs != null ? zs.referenceIncome : 1000f;
            while (q.pois.Count < config.baseCharbons) q.pois.Add(WorldCharbons.Make(ref nextPoiId, reference, config, rng));
            for (int p = 0; p < q.pois.Count && p < config.baseCharbons; p++)
            {
                PoiData poi = q.pois[p];
                poi.ownerId = Owners.Player;
                poi.isOpen = true;
                q.pois[p] = poi;
            }
            for (int p = config.baseCharbons; p < q.pois.Count; p++)
            {
                PoiData poi = q.pois[p];
                poi.ownerId = Owners.None;
                poi.isOpen = false;
                q.pois[p] = poi;
            }
            q.maintenanceCost = EconomyRules.Potential(q, config) * config.maintenanceRatio + config.maintenanceBase;
        }

        /// <summary>Crée les opérateurs de départ dans le quartier de base.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        public static void AddStartingOperators(ref GameData data, GameConfig config, System.Random rng)
        {
            for (int i = 0; i < config.startingOperators.Count; i++)
            {
                int competence = rng.Next(config.startingCompetence.x, config.startingCompetence.y + 1);
                OperatorRules.Create(ref data, config.startingOperators[i], competence, data.baseQuarterId, config);
            }
        }
    }
}

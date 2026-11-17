// MARSEILLE — Systems/WorldBuilder.cs
// Construit le monde d'une nouvelle partie : zones, factions, 111 quartiers, présences adverses,
// charbons, quartier de base, opérateurs de départ, offres de flux et objectifs. Déterministe pour une graine donnée.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Construction du monde initial.</summary>
    public static class WorldBuilder
    {
        /// <summary>Remplit une partie vide.</summary>
        /// <param name="data">Partie créée par GameData.CreateEmpty.</param>
        /// <param name="records">Quartiers du catalogue.</param>
        /// <param name="config">Configuration.</param>
        public static void Build(ref GameData data, List<QuarterRecord> records, GameConfig config)
        {
            System.Random rng = new System.Random(data.randomSeed);
            for (int z = 0; z < config.zones.Count; z++)
            {
                ZoneSettings s = config.zones[z];
                data.zones.Add(ZoneData.Create(s.id, s.name, s.shortName, s.canBeStart));
            }
            for (int f = 0; f < config.factions.Count; f++)
            {
                FactionSettings s = config.factions[f];
                data.factions.Add(FactionData.Create(s.id, s.name, s.colorHex, s.budget, s.effectifs, s.aggressiveness));
            }
            for (int r = 0; r < records.Count; r++) data.quarters.Add(CreateQuarter(ref data, records[r], config, rng));
            for (int i = 0; i < data.quarters.Count; i++) AddBorderPresence(ref data, i, config, rng);
            int poiId = 1;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                WorldCharbons.Populate(ref q, ref poiId, config, rng);
                data.quarters[i] = q;
            }
            WorldStart.PlaceBase(ref data, config, rng, ref poiId);
            WorldStart.AddStartingOperators(ref data, config, rng);
            CrewRules.BuildArrondissements(ref data);
            CrewActions.AutoStaff(ref data, config);
            FluxActions.CreateOffers(ref data, config);
            QuestRules.CreateFixedQuests(ref data, config);
            QuestRules.EnsureZoneQuests(ref data, config);
            data.indicators.territory = TerritoryRules.Territory(data);
        }

        /// <summary>Crée un quartier, le range dans sa zone et y place la faction de la zone.</summary>
        /// <param name="data">Partie (zones modifiées).</param>
        /// <param name="rec">Enregistrement du catalogue.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        /// <returns>Le quartier.</returns>
        private static QuarterData CreateQuarter(ref GameData data, QuarterRecord rec, GameConfig config, System.Random rng)
        {
            QuarterData q = QuarterData.Create(rec.id, rec.name, rec.arrondissement, rec.zoneId);
            q.mapX = rec.x;
            q.mapY = rec.y;
            if (rec.neighbours != null) q.adjacentQuarters.AddRange(rec.neighbours);
            int zi = data.IndexOfZone(rec.zoneId);
            if (zi >= 0) data.zones[zi].quarterIds.Add(rec.id);
            ZoneSettings zs = config.GetZoneSettings(rec.zoneId);
            if (zs != null && zs.startingFactionId > 0)
            {
                float v = Lerp(config.zoneFactionPresence.x, config.zoneFactionPresence.y, (float)rng.NextDouble());
                TerritoryRules.AddPresence(ref q, zs.startingFactionId, v);
            }
            return q;
        }

        /// <summary>Ajoute parfois une petite présence de la faction d'une zone voisine (frontières disputées).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="index">Index du quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        private static void AddBorderPresence(ref GameData data, int index, GameConfig config, System.Random rng)
        {
            QuarterData q = data.quarters[index];
            for (int n = 0; n < q.adjacentQuarters.Count; n++)
            {
                int j = data.IndexOfQuarter(q.adjacentQuarters[n]);
                if (j < 0 || data.quarters[j].zoneId == q.zoneId) continue;
                ZoneSettings other = config.GetZoneSettings(data.quarters[j].zoneId);
                if (other == null || other.startingFactionId <= 0 || rng.NextDouble() >= config.borderPresenceChance) continue;
                float v = Lerp(config.borderFactionPresence.x, config.borderFactionPresence.y, (float)rng.NextDouble());
                TerritoryRules.AddPresence(ref q, other.startingFactionId, System.Math.Min(v, q.FreeShare()));
                break;
            }
            data.quarters[index] = q;
        }

        /// <summary>Interpolation linéaire.</summary>
        /// <param name="a">Début.</param>
        /// <param name="b">Fin.</param>
        /// <param name="t">Facteur 0-1.</param>
        /// <returns>Valeur interpolée.</returns>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}

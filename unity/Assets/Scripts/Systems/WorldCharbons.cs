// MARSEILLE — Systems/WorldCharbons.cs
// Génère les charbons d'un quartier : nombre selon la zone, rentabilité tirée, risque déduit de la
// rentabilité, propriétaire = faction dominante si elle est assez présente. Fixe aussi le coût de maintien.

using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Génération des charbons et du maintien des quartiers.</summary>
    public static class WorldCharbons
    {
        /// <summary>Remplit les charbons d'un quartier et calcule son coût de maintien.</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="nextPoiId">Compteur d'identifiants de points d'intérêt.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        public static void Populate(ref QuarterData q, ref int nextPoiId, GameConfig config, System.Random rng)
        {
            ZoneSettings zs = config.GetZoneSettings(q.zoneId);
            float reference = zs != null ? zs.referenceIncome : 1000f;
            int min = zs != null ? zs.charbonsPerQuarter.x : 1;
            int max = zs != null ? zs.charbonsPerQuarter.y : 1;
            int count = rng.Next(min, max + 1);
            int dominant = q.DominantFaction();
            for (int c = 0; c < count; c++)
            {
                PoiData poi = Make(ref nextPoiId, reference, config, rng);
                if (dominant != Owners.None && q.PresenceOf(dominant) >= config.charbonFactionClaimPresence)
                {
                    poi.ownerId = dominant;
                    poi.isOpen = true;
                }
                q.pois.Add(poi);
            }
            q.maintenanceCost = EconomyRules.Potential(q, config) * config.maintenanceRatio + config.maintenanceBase;
        }

        /// <summary>Crée un charbon libre et fermé, placé au hasard autour du centre du quartier.</summary>
        /// <param name="nextPoiId">Compteur d'identifiants.</param>
        /// <param name="referenceIncome">Revenu de référence de la zone.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="rng">Aléatoire.</param>
        /// <returns>Le charbon.</returns>
        public static PoiData Make(ref int nextPoiId, float referenceIncome, GameConfig config, System.Random rng)
        {
            float ratio = WorldBuilder.Lerp(config.charbonIncomeRatio.x, config.charbonIncomeRatio.y, (float)rng.NextDouble());
            int risk = ratio >= config.charbonRiskThresholds.y ? 3 : ratio >= config.charbonRiskThresholds.x ? 2 : 1;
            string name = config.charbonNames.Count > 0 ? config.charbonNames[rng.Next(config.charbonNames.Count)] : "Charbon";
            float revenue = (float)System.Math.Round(referenceIncome * ratio / 10.0) * 10f;
            PoiData poi = PoiData.Create(nextPoiId++, name, PoiKind.Charbon, revenue, risk);
            poi.mapX = (float)rng.NextDouble();
            poi.mapY = (float)rng.NextDouble();
            return poi;
        }
    }
}

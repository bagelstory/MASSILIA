// MARSEILLE — Systems/SeasonRules.cs
// Calendrier (un cycle = une semaine, spec §10) et saisons qui modifient les revenus.
// Déterministe : le tirage des saisons aléatoires dépend seulement de la graine et du cycle,
// pour que la prévision du tableau de bord et la résolution donnent le même résultat.

using System.Collections.Generic;
using Marseille.Config;

namespace Marseille.Systems
{
    /// <summary>Règles du calendrier.</summary>
    public static class SeasonRules
    {
        private const int WeeksPerYear = 52;

        /// <summary>Semaine de l'année (1-52) d'un cycle.</summary>
        /// <param name="cycle">Cycle (1 = premier).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Semaine.</returns>
        public static int Week(int cycle, GameConfig config)
        {
            return ((config.startWeek - 1 + cycle - 1) % WeeksPerYear) + 1;
        }

        /// <summary>Année d'un cycle.</summary>
        /// <param name="cycle">Cycle.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Année.</returns>
        public static int Year(int cycle, GameConfig config)
        {
            return config.startYear + (config.startWeek - 1 + cycle - 1) / WeeksPerYear;
        }

        /// <summary>Libellé court « S37 · 2021 ».</summary>
        /// <param name="cycle">Cycle.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Libellé.</returns>
        public static string Label(int cycle, GameConfig config)
        {
            return "S" + Week(cycle, config) + " · " + Year(cycle, config);
        }

        /// <summary>Saisons actives pendant un cycle.</summary>
        /// <param name="seed">Graine de la partie.</param>
        /// <param name="cycle">Cycle.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Liste des saisons actives.</returns>
        public static List<SeasonSettings> Active(int seed, int cycle, GameConfig config)
        {
            List<SeasonSettings> list = new List<SeasonSettings>();
            int week = Week(cycle, config);
            System.Random rng = new System.Random(unchecked(seed * 17 + cycle * 104729));
            for (int i = 0; i < config.seasons.Count; i++)
            {
                SeasonSettings s = config.seasons[i];
                double roll = rng.NextDouble();
                bool inRange = s.weeks.x > 0 && week >= s.weeks.x && week <= s.weeks.y;
                bool even = s.evenWeeks && week % 2 == 0 && !(week >= s.excludedWeeks.x && week <= s.excludedWeeks.y);
                bool random = s.chance > 0f && roll < s.chance;
                if (inRange || even || random) list.Add(s);
            }
            return list;
        }

        /// <summary>Multiplicateur de revenu d'une zone pour une liste de saisons.</summary>
        /// <param name="seasons">Saisons actives.</param>
        /// <param name="zoneId">Zone.</param>
        /// <returns>Produit des multiplicateurs applicables.</returns>
        public static float IncomeMultiplier(List<SeasonSettings> seasons, int zoneId)
        {
            float m = 1f;
            for (int i = 0; i < seasons.Count; i++)
            {
                SeasonSettings s = seasons[i];
                if (s.zoneIds == null || s.zoneIds.Count == 0 || s.zoneIds.Contains(zoneId)) m *= s.incomeMultiplier;
            }
            return m;
        }
    }
}

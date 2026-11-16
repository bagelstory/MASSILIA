// MARSEILLE — Map/MapLayout.cs
// Place les quartiers sur le plan : à leur position réelle normalisée, ou rangés sur une grille
// (chaque quartier prend la case libre la plus proche de sa position réelle). Calculé une fois par partie.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Map
{
    /// <summary>Calcul de la disposition des quartiers.</summary>
    public static class MapLayout
    {
        /// <summary>Taille d'un quartier à l'écran pour la configuration donnée.</summary>
        /// <param name="config">Configuration.</param>
        /// <returns>Taille en pixels de référence.</returns>
        public static Vector2 NodeSize(GameConfig config)
        {
            if (!config.mapUseGrid) return config.mapNodeSize;
            Vector2 cell = CellSize(config);
            return cell * (1f - config.mapCellGap);
        }

        /// <summary>Taille d'une case de la grille.</summary>
        /// <param name="config">Configuration.</param>
        /// <returns>Taille en pixels.</returns>
        public static Vector2 CellSize(GameConfig config)
        {
            Vector2 inner = config.mapSize - Vector2.one * (2f * config.mapPadding);
            return new Vector2(inner.x / Mathf.Max(1, config.mapGrid.x), inner.y / Mathf.Max(1, config.mapGrid.y));
        }

        /// <summary>Calcule la position (ancrée au centre du plan) de chaque quartier.</summary>
        /// <param name="quarters">Quartiers (mapX, mapY normalisés 0-1, origine en haut à gauche).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Position par identifiant de quartier.</returns>
        public static Dictionary<int, Vector2> Compute(List<QuarterData> quarters, GameConfig config)
        {
            Dictionary<int, Vector2> result = new Dictionary<int, Vector2>();
            if (!config.mapUseGrid)
            {
                for (int i = 0; i < quarters.Count; i++) result[quarters[i].id] = Exact(quarters[i], config);
                return result;
            }
            int cols = Mathf.Max(1, config.mapGrid.x), rows = Mathf.Max(1, config.mapGrid.y);
            bool[,] used = new bool[cols, rows];
            List<QuarterData> ordered = new List<QuarterData>(quarters);
            ordered.Sort((a, b) => a.id.CompareTo(b.id));
            for (int i = 0; i < ordered.Count; i++)
            {
                QuarterData q = ordered[i];
                Vector2Int wanted = new Vector2Int(Mathf.Clamp(Mathf.FloorToInt(q.mapX * cols), 0, cols - 1),
                                                   Mathf.Clamp(Mathf.FloorToInt(q.mapY * rows), 0, rows - 1));
                Vector2Int cell = NearestFree(used, wanted, q.mapX * cols, q.mapY * rows);
                if (cell.x < 0) { result[q.id] = Exact(q, config); continue; }
                used[cell.x, cell.y] = true;
                result[q.id] = CellCenter(cell, config);
            }
            return result;
        }

        /// <summary>Position exacte d'un quartier, sans grille.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Position ancrée au centre du plan.</returns>
        private static Vector2 Exact(QuarterData q, GameConfig config)
        {
            Vector2 inner = config.mapSize - Vector2.one * (2f * config.mapPadding);
            return new Vector2(-inner.x * 0.5f + q.mapX * inner.x, inner.y * 0.5f - q.mapY * inner.y);
        }

        /// <summary>Centre d'une case, ancré au centre du plan (y vers le haut).</summary>
        /// <param name="cell">Case.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Position.</returns>
        private static Vector2 CellCenter(Vector2Int cell, GameConfig config)
        {
            Vector2 size = CellSize(config);
            Vector2 inner = config.mapSize - Vector2.one * (2f * config.mapPadding);
            return new Vector2(-inner.x * 0.5f + (cell.x + 0.5f) * size.x, inner.y * 0.5f - (cell.y + 0.5f) * size.y);
        }

        /// <summary>Case libre la plus proche de la position réelle (recherche par anneaux croissants).</summary>
        /// <param name="used">Cases occupées.</param>
        /// <param name="start">Case visée.</param>
        /// <param name="fx">Position réelle en colonnes (fractionnaire).</param>
        /// <param name="fy">Position réelle en lignes (fractionnaire).</param>
        /// <returns>Case trouvée, ou (-1,-1) si la grille est pleine.</returns>
        private static Vector2Int NearestFree(bool[,] used, Vector2Int start, float fx, float fy)
        {
            int cols = used.GetLength(0), rows = used.GetLength(1);
            int maxRing = Mathf.Max(cols, rows);
            for (int ring = 0; ring <= maxRing; ring++)
            {
                Vector2Int best = new Vector2Int(-1, -1);
                float bestDist = float.MaxValue;
                for (int dx = -ring; dx <= ring; dx++)
                    for (int dy = -ring; dy <= ring; dy++)
                    {
                        if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != ring) continue;
                        int x = start.x + dx, y = start.y + dy;
                        if (x < 0 || y < 0 || x >= cols || y >= rows || used[x, y]) continue;
                        float d = (x + 0.5f - fx) * (x + 0.5f - fx) + (y + 0.5f - fy) * (y + 0.5f - fy);
                        if (d < bestDist) { bestDist = d; best = new Vector2Int(x, y); }
                    }
                if (best.x >= 0) return best;
            }
            return new Vector2Int(-1, -1);
        }
    }
}

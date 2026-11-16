// MARSEILLE — Map/MapStyle.cs
// Couleurs de la carte (spec §12.2) : statut du quartier, faction dominante, zone verrouillée, inconnu.
// Fonctions pures, appelées seulement quand les données changent.

using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;
using UnityEngine;

namespace Marseille.Map
{
    /// <summary>Règles de couleur de la carte.</summary>
    public static class MapStyle
    {
        /// <summary>Convertit « #RRGGBB » en couleur (gris si invalide).</summary>
        /// <param name="hex">Texte hexadécimal.</param>
        /// <returns>La couleur.</returns>
        public static Color FromHex(string hex)
        {
            Color c;
            return !string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out c) ? c : Color.gray;
        }

        /// <summary>Couleur de fond d'un quartier.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>La couleur.</returns>
        public static Color QuarterColor(QuarterData q, GameData data, GameConfig config)
        {
            if (q.knowledgeLevel <= 0 && q.control <= 0f) return config.mapColorUnknown;
            Color c;
            switch (TerritoryRules.Status(q, config))
            {
                case QuarterStatus.Secured: c = config.mapColorSecured; break;
                case QuarterStatus.Player: c = config.mapColorPlayer; break;
                case QuarterStatus.Contested: c = config.mapColorContested; break;
                case QuarterStatus.Faction: c = FactionColor(data, q.DominantFaction(), config); break;
                default: c = config.mapColorFree; break;
            }
            if (!TerritoryRules.IsZoneUnlocked(data, q.zoneId)) c = Dim(c, config.mapLockedDim);
            return c;
        }

        /// <summary>Couleur d'une faction.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="factionId">Faction.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Sa couleur, ou la couleur « libre ».</returns>
        public static Color FactionColor(GameData data, int factionId, GameConfig config)
        {
            int i = data.IndexOfFaction(factionId);
            return i >= 0 ? FromHex(data.factions[i].colorHex) : config.mapColorFree;
        }

        /// <summary>Couleur d'un type d'opérateur.</summary>
        /// <param name="type">Type.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>La couleur.</returns>
        public static Color OperatorColor(OperatorType type, GameConfig config)
        {
            OperatorTypeSettings s = config.GetOperatorSettings(type);
            return s != null ? s.color : Color.white;
        }

        /// <summary>Assombrit une couleur en gardant son alpha.</summary>
        /// <param name="c">Couleur.</param>
        /// <param name="factor">Multiplicateur 0-1.</param>
        /// <returns>Couleur assombrie.</returns>
        public static Color Dim(Color c, float factor)
        {
            return new Color(c.r * factor, c.g * factor, c.b * factor, c.a);
        }

        /// <summary>Une faction vise-t-elle ce quartier, et le joueur le voit-il (surveillance) ?</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <returns>Vrai si une menace est visible.</returns>
        public static bool ThreatVisible(QuarterData q, GameData data)
        {
            if (q.revealedUntilCycle < data.currentCycle) return false;
            for (int i = 0; i < data.factions.Count; i++)
                if (data.factions[i].isAlive && data.factions[i].targetQuarter == q.id) return true;
            return false;
        }

        /// <summary>Nombre de charbons qui produisent pour le joueur dans un quartier.</summary>
        /// <param name="q">Quartier.</param>
        /// <returns>Nombre.</returns>
        public static int PlayerCharbons(QuarterData q)
        {
            int n = 0;
            for (int p = 0; p < q.pois.Count; p++) if (q.pois[p].kind == PoiKind.Charbon && q.pois[p].IsProducingForPlayer()) n++;
            return n;
        }

        /// <summary>Police d'écriture intégrée de Unity (sans TextMeshPro).</summary>
        /// <returns>La police.</returns>
        public static Font BuiltinFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }
}

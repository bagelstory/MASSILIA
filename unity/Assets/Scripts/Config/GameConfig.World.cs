// MARSEILLE — Config/GameConfig.World.cs
// Section « monde de départ » : zones, factions inventées, charbons générés, réseau initial du joueur.
// Les 111 quartiers eux-mêmes viennent de Resources/Data/quarters.json (étape 10).

using System.Collections.Generic;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Config
{
    /// <summary>Définition d'une zone de jeu.</summary>
    [System.Serializable]
    public class ZoneSettings
    {
        /// <summary>Identifiant (repris par quarters.json).</summary>
        public int id;
        /// <summary>Nom affiché.</summary>
        public string name;
        /// <summary>Nom court pour la carte.</summary>
        public string shortName;
        /// <summary>Peut être tirée comme zone de départ.</summary>
        public bool canBeStart;
        /// <summary>Faction qui tient la zone au départ (0 = aucune).</summary>
        public int startingFactionId;
        /// <summary>Nombre de charbons par quartier : minimum, maximum.</summary>
        public Vector2Int charbonsPerQuarter = new Vector2Int(1, 2);
        /// <summary>Revenu de référence de la zone (€), multiplié par un tirage pour chaque charbon.</summary>
        [Min(0f)] public float referenceIncome = 1500f;
    }

    /// <summary>Définition d'une faction rivale (inventée).</summary>
    [System.Serializable]
    public class FactionSettings
    {
        /// <summary>Identifiant (≥ 1).</summary>
        public int id;
        /// <summary>Nom.</summary>
        public string name;
        /// <summary>Couleur « #RRGGBB ».</summary>
        public string colorHex;
        /// <summary>Budget initial.</summary>
        [Min(0f)] public float budget;
        /// <summary>Effectif initial.</summary>
        [Min(1)] public int effectifs;
        /// <summary>Agressivité 1-5.</summary>
        [Range(1, 5)] public int aggressiveness;
    }

    public partial class GameConfig
    {
        [Header("Monde : zones et factions")]
        [Tooltip("Les 10 zones de jeu.")]
        public List<ZoneSettings> zones = new List<ZoneSettings>
        {
            new ZoneSettings { id = 1, name = "Centre historique", shortName = "Centre", canBeStart = true, startingFactionId = 1, charbonsPerQuarter = new Vector2Int(1, 3), referenceIncome = 1800f },
            new ZoneSettings { id = 2, name = "Centre nord", shortName = "Centre nord", canBeStart = true, startingFactionId = 2, charbonsPerQuarter = new Vector2Int(1, 3), referenceIncome = 1500f },
            new ZoneSettings { id = 3, name = "La Plaine / Cours Julien", shortName = "La Plaine", canBeStart = false, startingFactionId = 4, charbonsPerQuarter = new Vector2Int(1, 3), referenceIncome = 1600f },
            new ZoneSettings { id = 4, name = "Prado / Castellane", shortName = "Prado", canBeStart = false, startingFactionId = 0, charbonsPerQuarter = new Vector2Int(1, 2), referenceIncome = 2000f },
            new ZoneSettings { id = 5, name = "Nord-est", shortName = "Nord-est", canBeStart = true, startingFactionId = 1, charbonsPerQuarter = new Vector2Int(2, 4), referenceIncome = 1700f },
            new ZoneSettings { id = 6, name = "Nord central", shortName = "Nord central", canBeStart = true, startingFactionId = 3, charbonsPerQuarter = new Vector2Int(1, 3), referenceIncome = 1600f },
            new ZoneSettings { id = 7, name = "Nord-ouest", shortName = "Nord-ouest", canBeStart = true, startingFactionId = 1, charbonsPerQuarter = new Vector2Int(2, 4), referenceIncome = 1900f },
            new ZoneSettings { id = 8, name = "Littoral nord", shortName = "Littoral nord", canBeStart = true, startingFactionId = 2, charbonsPerQuarter = new Vector2Int(1, 2), referenceIncome = 1200f },
            new ZoneSettings { id = 9, name = "Est", shortName = "Est", canBeStart = false, startingFactionId = 3, charbonsPerQuarter = new Vector2Int(0, 2), referenceIncome = 1300f },
            new ZoneSettings { id = 10, name = "Sud", shortName = "Sud", canBeStart = false, startingFactionId = 5, charbonsPerQuarter = new Vector2Int(0, 2), referenceIncome = 1400f }
        };

        [Tooltip("Les factions rivales (noms inventés).")]
        public List<FactionSettings> factions = new List<FactionSettings>
        {
            new FactionSettings { id = 1, name = "La Compagnie", colorHex = "#C8352E", budget = 28000f, effectifs = 14, aggressiveness = 4 },
            new FactionSettings { id = 2, name = "Les Gabians", colorHex = "#3D7FD9", budget = 12000f, effectifs = 8, aggressiveness = 5 },
            new FactionSettings { id = 3, name = "Le Ruisseau", colorHex = "#D08B2C", budget = 9000f, effectifs = 7, aggressiveness = 2 },
            new FactionSettings { id = 4, name = "Tramontane", colorHex = "#8A5CD6", budget = 11000f, effectifs = 5, aggressiveness = 3 },
            new FactionSettings { id = 5, name = "Les Calanquais", colorHex = "#5FC4C4", budget = 6000f, effectifs = 4, aggressiveness = 4 }
        };

        [Header("Monde : génération")]
        [Tooltip("Présence de la faction de zone dans chacun de ses quartiers (min, max).")]
        public Vector2 zoneFactionPresence = new Vector2(45f, 80f);
        [Tooltip("Présence possible d'une faction voisine sur les bords de zone (min, max).")]
        public Vector2 borderFactionPresence = new Vector2(0f, 15f);
        [Tooltip("Probabilité qu'une faction voisine soit présente sur un bord de zone.")]
        [Range(0f, 1f)] public float borderPresenceChance = 0.5f;
        [Tooltip("Rentabilité d'un charbon = revenu de zone × tirage dans cet intervalle.")]
        public Vector2 charbonIncomeRatio = new Vector2(0.35f, 1f);
        [Tooltip("Seuils de rentabilité (ratio) pour le risque 2 et le risque 3.")]
        public Vector2 charbonRiskThresholds = new Vector2(0.55f, 0.8f);
        [Tooltip("Coût de maintien d'un quartier = potentiel × ce facteur + base.")]
        [Min(0f)] public float maintenanceRatio = 0.15f;
        [Tooltip("Coût de maintien minimal d'un quartier (€).")]
        [Min(0f)] public float maintenanceBase = 40f;
        [Tooltip("Noms fictifs des charbons.")]
        public List<string> charbonNames = new List<string>
        {
            "le Hall B", "la Tour 3", "le Parking du bas", "la Passerelle", "le Square", "les Arcades", "la Coursive", "le Rond-point", "le Terrain", "l'Escalier 4",
            "le Petit Bois", "la Placette", "l'Abri-bus", "le Muret", "les Garages", "le Kiosque", "le Local vélo", "la Cour", "le Bloc 7", "la Rampe"
        };

        [Header("Monde : réseau de départ")]
        [Tooltip("Types des opérateurs de départ (spec §4.4 : Force, Pression, Repérage).")]
        public List<OperatorType> startingOperators = new List<OperatorType> { OperatorType.Force, OperatorType.Pressure, OperatorType.Recon };
        [Tooltip("Compétence des opérateurs de départ (min, max).")]
        public Vector2Int startingCompetence = new Vector2Int(2, 3);
        [Tooltip("Contrôle du joueur dans son quartier de base.")]
        [Range(0f, 100f)] public float baseControl = 60f;
        [Tooltip("Présence de la faction locale dans le quartier de base.")]
        [Range(0f, 100f)] public float baseHostilePresence = 25f;
        [Tooltip("Charbons ouverts au joueur dans sa base.")]
        [Min(0)] public int baseCharbons = 2;
        [Tooltip("Surnoms donnés aux premiers opérateurs (personnages adultes du jeu).")]
        public List<string> operatorNicknames = new List<string> { "Zépek", "Rascasse", "Mika", "Zizou", "Sab", "Nono" };

        /// <summary>Définition d'une zone.</summary>
        /// <param name="zoneId">Identifiant recherché.</param>
        /// <returns>La définition, ou null.</returns>
        public ZoneSettings GetZoneSettings(int zoneId)
        {
            for (int i = 0; i < zones.Count; i++) if (zones[i].id == zoneId) return zones[i];
            return null;
        }
    }
}

// MARSEILLE — Data/QuarterData.cs
// Un des 111 quartiers officiels de Marseille : données statiques (nom, arrondissement, voisins, position)
// et état de partie (contrôle, présences, connaissance, points d'intérêt).
// Attention : c'est une struct. Après modification d'un élément de liste, le réassigner :
//   var q = data.quarters[i]; q.control += 5f; data.quarters[i] = q;

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>État d'un quartier (spec §3.2).</summary>
    [System.Serializable]
    public struct QuarterData
    {
        /// <summary>Identifiant (1-111), index stable dans quarters.json.</summary>
        public int id;

        /// <summary>Nom officiel (décret de 1946).</summary>
        public string name;

        /// <summary>Arrondissement municipal (1-16).</summary>
        public int arrondissement;

        /// <summary>Zone de jeu (regroupement de quartiers, verrouillable).</summary>
        public int zoneId;

        /// <summary>Identifiants des quartiers voisins.</summary>
        public List<int> adjacentQuarters;

        /// <summary>Position du centre sur la carte, normalisée de 0 à 1 (0 = ouest).</summary>
        public float mapX;

        /// <summary>Position du centre sur la carte, normalisée de 0 à 1 (0 = nord).</summary>
        public float mapY;

        /// <summary>Contrôle du joueur (0-100 %).</summary>
        public float control;

        /// <summary>Présence adverse totale (0-100 %), somme de <see cref="factionPresences"/> recalculée par TerritorySystem.</summary>
        public float hostilePresence;

        /// <summary>Détail de la présence adverse par faction.</summary>
        public List<FactionPresence> factionPresences;

        /// <summary>Revenu potentiel du quartier par cycle (€), avant contrôle et efficacité.</summary>
        public float localFlux;

        /// <summary>Coût de maintien par cycle (€), appliqué au prorata du contrôle.</summary>
        public float maintenanceCost;

        /// <summary>Niveau d'investissement (augmente flux et maintien).</summary>
        public int investmentLevel;

        /// <summary>Ce que le joueur sait du quartier (0-3).</summary>
        public int knowledgeLevel;

        /// <summary>Cycles consécutifs sans opérateur du joueur sur place.</summary>
        public int cyclesWithoutOperator;

        /// <summary>Points d'intérêt, dont les charbons.</summary>
        public List<PoiData> pois;

        /// <summary>Identifiants des événements actifs dans le quartier.</summary>
        public List<string> activeEvents;

        /// <summary>Crée un quartier vide avec ses listes initialisées.</summary>
        /// <param name="id">Identifiant.</param>
        /// <param name="name">Nom officiel.</param>
        /// <param name="arrondissement">Arrondissement (1-16).</param>
        /// <param name="zoneId">Zone de jeu.</param>
        /// <returns>Le quartier initialisé.</returns>
        public static QuarterData Create(int id, string name, int arrondissement, int zoneId)
        {
            return new QuarterData
            {
                id = id,
                name = name,
                arrondissement = arrondissement,
                zoneId = zoneId,
                adjacentQuarters = new List<int>(),
                factionPresences = new List<FactionPresence>(),
                pois = new List<PoiData>(),
                activeEvents = new List<string>()
            };
        }

        /// <summary>Recalcule <see cref="hostilePresence"/> à partir du détail par faction.</summary>
        public void RecomputeHostilePresence()
        {
            float total = 0f;
            if (factionPresences != null)
            {
                for (int i = 0; i < factionPresences.Count; i++) total += factionPresences[i].value;
            }
            hostilePresence = total;
        }

        /// <summary>Part libre du quartier (ni au joueur, ni aux factions).</summary>
        /// <returns>Pourcentage libre (0-100).</returns>
        public float FreeShare()
        {
            float free = 100f - control - hostilePresence;
            return free < 0f ? 0f : free;
        }

        /// <summary>Présence d'une faction donnée.</summary>
        /// <param name="factionId">Faction recherchée.</param>
        /// <returns>Pourcentage, 0 si absente.</returns>
        public float PresenceOf(int factionId)
        {
            if (factionPresences == null) return 0f;
            for (int i = 0; i < factionPresences.Count; i++)
            {
                if (factionPresences[i].factionId == factionId) return factionPresences[i].value;
            }
            return 0f;
        }

        /// <summary>Faction la plus présente dans le quartier.</summary>
        /// <returns>Identifiant de faction, ou <see cref="Owners.None"/>.</returns>
        public int DominantFaction()
        {
            int best = Owners.None;
            float max = 0f;
            if (factionPresences == null) return best;
            for (int i = 0; i < factionPresences.Count; i++)
            {
                if (factionPresences[i].value > max) { max = factionPresences[i].value; best = factionPresences[i].factionId; }
            }
            return best;
        }
    }
}

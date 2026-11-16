// MARSEILLE — Data/FactionData.cs
// Une faction rivale inventée, pilotée par FactionAI (spec §5). Struct : réassigner après modification.

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>État d'une faction rivale (spec §5.2).</summary>
    [System.Serializable]
    public struct FactionData
    {
        /// <summary>Identifiant (≥ 1 ; 0 est réservé au joueur).</summary>
        public int id;

        /// <summary>Nom court (fictif).</summary>
        public string name;

        /// <summary>Couleur d'affichage, format « #RRGGBB ».</summary>
        public string colorHex;

        /// <summary>Quartiers où sa présence est d'au moins 30 % (recalculé chaque cycle).</summary>
        public List<int> territory;

        /// <summary>Capacité financière (€).</summary>
        public float budget;

        /// <summary>Nombre d'opérateurs adverses (abstrait).</summary>
        public int effectifs;

        /// <summary>Agressivité de 1 (passive) à 5 (très offensive).</summary>
        public int aggressiveness;

        /// <summary>Agressivité d'origine, vers laquelle elle revient en période calme.</summary>
        public int baseAggressiveness;

        /// <summary>Objectif courant.</summary>
        public FactionObjective objective;

        /// <summary>Quartier visé en priorité, ou -1.</summary>
        public int targetQuarter;

        /// <summary>Faction encore présente dans la ville.</summary>
        public bool isAlive;

        /// <summary>Plus grand territoire atteint (sert à détecter la fragmentation).</summary>
        public int peakTerritory;

        /// <summary>Déjà fragmentée une fois.</summary>
        public bool isFragmented;

        /// <summary>Faction alliée (trêve), ou <see cref="Owners.None"/>.</summary>
        public int allyFactionId;

        /// <summary>Cycles restants de trêve.</summary>
        public int allianceCyclesLeft;

        /// <summary>Crée une faction vivante, sans territoire calculé.</summary>
        /// <param name="id">Identifiant (≥ 1).</param>
        /// <param name="name">Nom.</param>
        /// <param name="colorHex">Couleur « #RRGGBB ».</param>
        /// <param name="budget">Budget de départ.</param>
        /// <param name="effectifs">Effectif de départ.</param>
        /// <param name="aggressiveness">Agressivité 1-5.</param>
        /// <returns>La faction initialisée.</returns>
        public static FactionData Create(int id, string name, string colorHex, float budget, int effectifs, int aggressiveness)
        {
            return new FactionData
            {
                id = id,
                name = name,
                colorHex = colorHex,
                territory = new List<int>(),
                budget = budget,
                effectifs = effectifs,
                aggressiveness = aggressiveness,
                baseAggressiveness = aggressiveness,
                objective = FactionObjective.Maintain,
                targetQuarter = -1,
                isAlive = true,
                peakTerritory = 0,
                isFragmented = false,
                allyFactionId = Owners.None,
                allianceCyclesLeft = 0
            };
        }

        /// <summary>Indique si cette faction est en trêve avec une autre.</summary>
        /// <param name="otherFactionId">Autre faction.</param>
        /// <returns>Vrai si une trêve est en cours entre les deux.</returns>
        public bool IsAlliedWith(int otherFactionId)
        {
            return allianceCyclesLeft > 0 && allyFactionId == otherFactionId;
        }
    }
}

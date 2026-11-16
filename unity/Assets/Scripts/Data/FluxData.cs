// MARSEILLE — Data/FluxData.cs
// Engagement international abstrait (spec §8) : des chiffres, aucune route, aucune méthode.

namespace Marseille.Data
{
    /// <summary>État d'un flux international (spec §8.2).</summary>
    [System.Serializable]
    public struct FluxData
    {
        /// <summary>Identifiant.</summary>
        public int id;

        /// <summary>Nom affiché du partenaire (ex. « Partenaire Sud »).</summary>
        public string name;

        /// <summary>Zone d'origine fictive (ex. « Nord », « Est »).</summary>
        public string origin;

        /// <summary>Importance de 1 à 5 (revenus et risques).</summary>
        public int volume;

        /// <summary>Argent immobilisé (€), perdu en cas de rupture ou de coupure.</summary>
        public float financialCommitment;

        /// <summary>Coût d'activation (€), payé une fois.</summary>
        public float activationCost;

        /// <summary>Probabilité (0-100 %) que le cycle se passe bien.</summary>
        public float reliability;

        /// <summary>Niveau de risque (0-100), augmente avec le volume et la durée.</summary>
        public float exposure;

        /// <summary>Risque de rupture (0-100).</summary>
        public float instability;

        /// <summary>Revenu par cycle (€) quand tout se passe bien.</summary>
        public float potentialRevenue;

        /// <summary>Coût par cycle (€), payé que le cycle réussisse ou non.</summary>
        public float costPerCycle;

        /// <summary>Flux en cours.</summary>
        public bool isActive;

        /// <summary>Cycles d'activité depuis l'activation.</summary>
        public int activeCycles;

        /// <summary>Résultat du dernier cycle : vrai si le revenu est arrivé.</summary>
        public bool lastCycleSucceeded;

        /// <summary>Crée une offre de flux inactive.</summary>
        /// <param name="id">Identifiant.</param>
        /// <param name="name">Nom du partenaire.</param>
        /// <param name="origin">Zone d'origine fictive.</param>
        /// <param name="volume">Volume 1-5.</param>
        /// <param name="commitment">Argent immobilisé.</param>
        /// <param name="activationCost">Coût d'activation.</param>
        /// <param name="reliability">Fiabilité de départ.</param>
        /// <param name="instability">Instabilité de départ.</param>
        /// <param name="revenue">Revenu par cycle.</param>
        /// <param name="cost">Coût par cycle.</param>
        /// <returns>Le flux initialisé.</returns>
        public static FluxData Create(int id, string name, string origin, int volume, float commitment, float activationCost,
                                      float reliability, float instability, float revenue, float cost)
        {
            return new FluxData
            {
                id = id,
                name = name,
                origin = origin,
                volume = volume,
                financialCommitment = commitment,
                activationCost = activationCost,
                reliability = reliability,
                exposure = 0f,
                instability = instability,
                potentialRevenue = revenue,
                costPerCycle = cost,
                isActive = false,
                activeCycles = 0,
                lastCycleSucceeded = false
            };
        }

        /// <summary>Revenu net attendu par cycle, pondéré par la fiabilité.</summary>
        /// <returns>Montant en €.</returns>
        public float ExpectedNet()
        {
            return potentialRevenue * reliability / 100f - costPerCycle;
        }
    }
}

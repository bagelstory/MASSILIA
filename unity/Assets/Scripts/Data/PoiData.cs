// MARSEILLE — Data/PoiData.cs
// Point d'intérêt d'un quartier. Les charbons sont des points d'intérêt de type Charbon :
// une rentabilité, un risque, un propriétaire. Aucune autre information (gestion abstraite).

namespace Marseille.Data
{
    /// <summary>Point d'intérêt (commerce, dépôt, lieu stratégique, charbon) placé dans un quartier.</summary>
    [System.Serializable]
    public struct PoiData
    {
        /// <summary>Identifiant unique dans la partie.</summary>
        public int id;

        /// <summary>Nom fictif affiché (ex. « le Hall B »).</summary>
        public string name;

        /// <summary>Nature du point d'intérêt.</summary>
        public PoiKind kind;

        /// <summary>Revenu potentiel par cycle (€) quand il est ouvert et tenu.</summary>
        public float revenue;

        /// <summary>Risque de 1 à 3 : pression, exposition et probabilité de saisie.</summary>
        public int risk;

        /// <summary>Propriétaire : <see cref="Owners.None"/>, <see cref="Owners.Player"/> ou identifiant de faction.</summary>
        public int ownerId;

        /// <summary>Ouvert (produit des revenus) ou fermé.</summary>
        public bool isOpen;

        /// <summary>Cycles restants de fermeture après une saisie (0 si actif).</summary>
        public int seizedCycles;

        /// <summary>Position sur la carte, normalisée de 0 à 1.</summary>
        public float mapX;

        /// <summary>Position sur la carte, normalisée de 0 à 1.</summary>
        public float mapY;

        /// <summary>Crée un point d'intérêt libre et fermé.</summary>
        /// <param name="id">Identifiant unique.</param>
        /// <param name="name">Nom affiché.</param>
        /// <param name="kind">Nature.</param>
        /// <param name="revenue">Revenu potentiel par cycle.</param>
        /// <param name="risk">Risque de 1 à 3.</param>
        /// <returns>Le point d'intérêt initialisé.</returns>
        public static PoiData Create(int id, string name, PoiKind kind, float revenue, int risk)
        {
            return new PoiData
            {
                id = id,
                name = name,
                kind = kind,
                revenue = revenue,
                risk = risk,
                ownerId = Owners.None,
                isOpen = false,
                seizedCycles = 0,
                mapX = 0f,
                mapY = 0f
            };
        }

        /// <summary>Indique si le point produit des revenus pour le joueur ce cycle.</summary>
        /// <returns>Vrai s'il est au joueur, ouvert et non saisi.</returns>
        public bool IsProducingForPlayer()
        {
            return ownerId == Owners.Player && isOpen && seizedCycles == 0;
        }
    }
}

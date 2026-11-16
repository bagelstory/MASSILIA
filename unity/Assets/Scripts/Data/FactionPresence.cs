// MARSEILLE — Data/FactionPresence.cs
// Présence d'une faction dans un quartier. JsonUtility ne sérialise pas les dictionnaires :
// on stocke une liste de couples (faction, pourcentage).

namespace Marseille.Data
{
    /// <summary>Part (0-100 %) d'un quartier tenue par une faction rivale.</summary>
    [System.Serializable]
    public struct FactionPresence
    {
        /// <summary>Identifiant de la faction (≥ 1).</summary>
        public int factionId;

        /// <summary>Présence en pourcentage (0-100).</summary>
        public float value;

        /// <summary>Crée une présence.</summary>
        /// <param name="factionId">Faction concernée.</param>
        /// <param name="value">Pourcentage de présence.</param>
        /// <returns>La présence initialisée.</returns>
        public static FactionPresence Create(int factionId, float value)
        {
            return new FactionPresence { factionId = factionId, value = value };
        }
    }
}

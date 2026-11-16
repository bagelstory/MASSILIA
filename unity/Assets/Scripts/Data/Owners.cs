// MARSEILLE — Data/Owners.cs
// Identifiants spéciaux de propriétaire (charbons, présences) : JsonUtility ne gère pas les entiers nullables.

namespace Marseille.Data
{
    /// <summary>Valeurs réservées pour désigner un propriétaire. Les factions ont des identifiants à partir de 1.</summary>
    public static class Owners
    {
        /// <summary>Personne : charbon libre, aucune cible.</summary>
        public const int None = -1;

        /// <summary>Le réseau du joueur.</summary>
        public const int Player = 0;

        /// <summary>Indique si l'identifiant désigne une faction rivale.</summary>
        /// <param name="ownerId">Identifiant de propriétaire.</param>
        /// <returns>Vrai pour une faction (identifiant ≥ 1).</returns>
        public static bool IsFaction(int ownerId)
        {
            return ownerId >= 1;
        }
    }
}

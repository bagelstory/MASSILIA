// MARSEILLE — Data/ZoneData.cs
// Zone de jeu : regroupement de quartiers (ex. « Nord-ouest »). Une zone verrouillée s'ouvre par une quête.

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>Zone de gameplay regroupant plusieurs quartiers.</summary>
    [System.Serializable]
    public struct ZoneData
    {
        /// <summary>Identifiant.</summary>
        public int id;

        /// <summary>Nom affiché (ex. « Centre historique »).</summary>
        public string name;

        /// <summary>Nom court pour la carte (ex. « Centre »).</summary>
        public string shortName;

        /// <summary>Quartiers de la zone (ids).</summary>
        public List<int> quarterIds;

        /// <summary>Le joueur peut y envoyer des opérateurs (sauf Repérage, toujours autorisé depuis un voisin).</summary>
        public bool isUnlocked;

        /// <summary>Peut être tirée comme zone de départ.</summary>
        public bool canBeStart;

        /// <summary>Crée une zone verrouillée.</summary>
        /// <param name="id">Identifiant.</param>
        /// <param name="name">Nom.</param>
        /// <param name="shortName">Nom court.</param>
        /// <param name="canBeStart">Zone de départ possible.</param>
        /// <returns>La zone initialisée.</returns>
        public static ZoneData Create(int id, string name, string shortName, bool canBeStart)
        {
            return new ZoneData
            {
                id = id,
                name = name,
                shortName = shortName,
                quarterIds = new List<int>(),
                isUnlocked = false,
                canBeStart = canBeStart
            };
        }
    }
}

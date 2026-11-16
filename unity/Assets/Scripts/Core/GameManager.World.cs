// MARSEILLE — Core/GameManager.World.cs
// Implémentation de BuildNewWorld (déclarée dans GameManager.cs) : charge le catalogue des quartiers
// et délègue la construction à WorldBuilder.

using Marseille.Data;
using Marseille.Systems;
using UnityEngine;

namespace Marseille.Core
{
    /// <summary>Construction du monde d'une nouvelle partie.</summary>
    public partial class GameManager
    {
        /// <summary>Remplit la partie neuve : zones, factions, quartiers, charbons, base et opérateurs.</summary>
        /// <param name="newData">Partie vide créée par NewGame.</param>
        partial void BuildNewWorld(ref GameData newData)
        {
            var records = QuarterCatalog.Load();
            if (records.Count == 0) Debug.LogWarning("Monde vide : aucun quartier chargé.");
            WorldBuilder.Build(ref newData, records, config);
        }
    }
}

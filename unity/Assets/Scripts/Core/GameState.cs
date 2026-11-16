// MARSEILLE — Core/GameState.cs
// États de la boucle de jeu : PLANNING → (le joueur valide) → RESOLVING → RESULTS → PLANNING.

namespace Marseille.Core
{
    /// <summary>État courant de la boucle tour par tour. Stocké en entier dans GameData.gameState.</summary>
    public enum GameState
    {
        /// <summary>Le joueur assigne, déplace, active ou coupe. Bouton « Valider » actif.</summary>
        Planning = 0,
        /// <summary>Le système résout le cycle. Bouton « Valider » désactivé.</summary>
        Resolving = 1,
        /// <summary>Le joueur lit les résultats du cycle.</summary>
        Results = 2,
        /// <summary>Partie terminée (victoire, faillite, chute, abandon).</summary>
        GameOver = 3
    }
}

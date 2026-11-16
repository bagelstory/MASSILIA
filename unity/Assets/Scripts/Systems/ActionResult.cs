// MARSEILLE — Systems/ActionResult.cs
// Réponse des actions de planification (affecter, recruter, investir, ouvrir un charbon) :
// réussite et message d'une ligne, que l'UI affiche et verse au journal.

namespace Marseille.Systems
{
    /// <summary>Résultat d'une action du joueur pendant la planification.</summary>
    public struct ActionResult
    {
        /// <summary>L'action a été appliquée.</summary>
        public bool ok;

        /// <summary>Message d'une ligne (raison du refus ou confirmation).</summary>
        public string message;

        /// <summary>Action réussie.</summary>
        /// <param name="message">Confirmation.</param>
        /// <returns>Le résultat.</returns>
        public static ActionResult Success(string message)
        {
            return new ActionResult { ok = true, message = message };
        }

        /// <summary>Action refusée.</summary>
        /// <param name="reason">Raison.</param>
        /// <returns>Le résultat.</returns>
        public static ActionResult Fail(string reason)
        {
            return new ActionResult { ok = false, message = reason };
        }
    }
}

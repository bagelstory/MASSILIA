// MARSEILLE — Systems/ConsequenceActions.cs
// Deux gestes du joueur pendant la planification (design/20 §2 et §3) : se soigner (de l'argent contre de l'usure)
// et partager un repas (du temps contre du foyer). Chaque geste dit d'abord ce qu'il coûte.

using Marseille.Config;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Actions de la vie privée.</summary>
    public static class ConsequenceActions
    {
        /// <summary>Se faire soigner : une consultation et du repos.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Care(ref GameData data, GameConfig config)
        {
            if (data.life.usure <= 0f) return ActionResult.Fail("Le corps va bien : rien à soigner.");
            if (data.indicators.treasury < config.santeSoinCout) return ActionResult.Fail("Pas assez d'argent pour une consultation.");
            data.indicators.treasury -= config.santeSoinCout;
            ConsequenceData l = data.life;
            l.usure = MathHelper.Clamp100(l.usure - config.santeSoinUsure);
            data.life = l;
            return ActionResult.Success("Consultation et repos : −" + config.santeSoinUsure.ToString("0") + " d'usure, " + config.santeSoinCout.ToString("N0") + " €.");
        }

        /// <summary>Un repas à la maison, téléphone face contre bois : compte à la prochaine résolution.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Meal(ref GameData data, GameConfig config)
        {
            if (data.life.semainesDetention > 0) return ActionResult.Fail("Pas de repas à la maison pendant une détention.");
            if (data.life.repasEnAttente > 0) return ActionResult.Fail("Un repas est déjà prévu cette semaine.");
            ConsequenceData l = data.life;
            l.repasEnAttente = 1;
            data.life = l;
            return ActionResult.Success("Repas prévu : +" + config.familleRepas.ToString("0") + " de foyer à la fin de la semaine.");
        }
    }
}

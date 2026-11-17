// MARSEILLE — Systems/FacadeActions.cs
// Actions du joueur sur les façades : ouvrir, fermer. Un secteur, un quartier, un coût. Rien d'autre.

using Marseille.Config;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Ouvrir ou fermer une façade.</summary>
    public static class FacadeActions
    {
        /// <summary>Peut-on ouvrir cette façade dans ce quartier ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="sector">Secteur.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Réussite, ou la raison du refus.</returns>
        public static ActionResult CanOpen(GameData data, int quarterId, FacadeSector sector, GameConfig config)
        {
            int qi = data.IndexOfQuarter(quarterId);
            if (qi < 0) return ActionResult.Fail("Quartier inconnu.");
            if (data.quarters[qi].control <= 0f) return ActionResult.Fail("Il faut tenir le quartier.");
            if (FacadeRules.Find(data, quarterId, sector) >= 0) return ActionResult.Fail("Déjà ouverte ici.");
            if (FacadeRules.CountIn(data, quarterId) >= config.facadeMaxPerQuarter) return ActionResult.Fail("Le quartier est saturé.");
            if (data.indicators.treasury < FacadeRules.Spec(sector, config).openCost) return ActionResult.Fail("Pas assez d'argent.");
            return ActionResult.Success(string.Empty);
        }

        /// <summary>Ouvre une façade : paie le coût, l'Ombre monte un peu.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="sector">Secteur.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Open(ref GameData data, int quarterId, FacadeSector sector, GameConfig config)
        {
            ActionResult check = CanOpen(data, quarterId, sector, config);
            if (!check.ok) return check;
            FacadeSpec spec = FacadeRules.Spec(sector, config);
            ShadowData s = data.shadow;
            if (s.facades == null) s.facades = new System.Collections.Generic.List<FacadeData>();
            s.facades.Add(new FacadeData { sector = sector, quarterId = quarterId });
            s.ombre = MathHelper.Clamp(s.ombre + config.facadeOmbreOpen, 0f, 100f);
            data.shadow = s;
            data.indicators.treasury -= spec.openCost;
            return ActionResult.Success(data.quarters[data.IndexOfQuarter(quarterId)].name + " : " + spec.name + " ouvre. Tout le monde a l'air content.");
        }

        /// <summary>Coût pour fermer une façade.</summary>
        /// <param name="sector">Secteur.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Coût (€).</returns>
        public static float CloseCost(FacadeSector sector, GameConfig config)
        {
            return FacadeRules.Spec(sector, config).openCost * config.facadeCloseShare;
        }

        /// <summary>Ferme une façade (immédiat dans cette version) : coûte une part de l'ouverture.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="sector">Secteur.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Close(ref GameData data, int quarterId, FacadeSector sector, GameConfig config)
        {
            int i = FacadeRules.Find(data, quarterId, sector);
            if (i < 0) return ActionResult.Fail("Aucune façade ici.");
            float cost = CloseCost(sector, config);
            if (data.indicators.treasury < cost) return ActionResult.Fail("Pas assez d'argent.");
            data.indicators.treasury -= cost;
            data.shadow.facades.RemoveAt(i);
            return ActionResult.Success("Le rideau est baissé. Le quartier garde le vide.");
        }
    }
}

// MARSEILLE — Systems/FluxActions.cs
// Actions sur les flux internationaux (spec §8.3) : créer les offres, activer, couper.
// Les flux restent abstraits : un partenaire, des montants, des risques, aucun détail opérationnel.

using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Actions du joueur sur les flux.</summary>
    public static class FluxActions
    {
        /// <summary>Ajoute à la partie toutes les offres de la configuration, inactives.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        public static void CreateOffers(ref GameData data, GameConfig config)
        {
            for (int i = 0; i < config.fluxOffers.Count; i++) data.fluxes.Add(FromOffer(config.fluxOffers[i]));
        }

        /// <summary>Crée un flux inactif à partir d'une offre.</summary>
        /// <param name="o">Offre.</param>
        /// <returns>Le flux.</returns>
        public static FluxData FromOffer(FluxOffer o)
        {
            return FluxData.Create(o.id, o.name, o.origin, o.volume, o.commitment, o.activationCost, o.reliability, o.instability, o.revenue, o.costPerCycle);
        }

        /// <summary>Somme à payer pour activer un flux (activation + engagement).</summary>
        /// <param name="flux">Flux.</param>
        /// <returns>Montant en €.</returns>
        public static float ActivationTotal(FluxData flux)
        {
            return flux.activationCost + flux.financialCommitment;
        }

        /// <summary>Nombre de flux actifs.</summary>
        /// <param name="data">Partie.</param>
        /// <returns>Nombre.</returns>
        public static int ActiveCount(GameData data)
        {
            int n = 0;
            for (int i = 0; i < data.fluxes.Count; i++) if (data.fluxes[i].isActive) n++;
            return n;
        }

        /// <summary>Active un flux : paie l'activation et immobilise l'engagement ; repart des valeurs de l'offre.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="fluxId">Flux.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult Activate(ref GameData data, int fluxId, GameConfig config)
        {
            int i = IndexOf(data, fluxId);
            if (i < 0) return ActionResult.Fail("Flux inconnu.");
            if (data.fluxes[i].isActive) return ActionResult.Fail("Déjà actif.");
            FluxOffer offer = config.GetFluxOffer(fluxId);
            FluxData flux = offer != null ? FromOffer(offer) : data.fluxes[i];
            float total = ActivationTotal(flux);
            if (data.indicators.treasury < total) return ActionResult.Fail("Il faut " + total.ToString("0") + " € (activation + engagement).");
            data.indicators.treasury -= total;
            flux.isActive = true;
            flux.activeCycles = 0;
            flux.exposure = 0f;
            data.fluxes[i] = flux;
            return ActionResult.Success("Flux activé : " + flux.name + ", −" + total.ToString("0") + " €.");
        }

        /// <summary>Coupe un flux : l'engagement immobilisé est perdu.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="fluxId">Flux.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult Cut(ref GameData data, int fluxId)
        {
            int i = IndexOf(data, fluxId);
            if (i < 0 || !data.fluxes[i].isActive) return ActionResult.Fail("Flux inactif.");
            FluxData flux = data.fluxes[i];
            float lost = flux.financialCommitment;
            flux.isActive = false;
            flux.financialCommitment = 0f;
            data.fluxes[i] = flux;
            return ActionResult.Success("Flux coupé : " + flux.name + ". " + lost.ToString("0") + " € immobilisés perdus.");
        }

        /// <summary>Position d'un flux dans la liste.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="fluxId">Identifiant.</param>
        /// <returns>Index, ou -1.</returns>
        public static int IndexOf(GameData data, int fluxId)
        {
            for (int i = 0; i < data.fluxes.Count; i++) if (data.fluxes[i].id == fluxId) return i;
            return -1;
        }
    }
}

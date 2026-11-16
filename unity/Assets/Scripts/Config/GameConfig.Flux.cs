// MARSEILLE — Config/GameConfig.Flux.cs
// Section « flux internationaux » de GameConfig (spec §8) : offres abstraites et règles de risque.

using System.Collections.Generic;
using UnityEngine;

namespace Marseille.Config
{
    /// <summary>Offre de flux proposée au joueur (partenaire fictif, sans détail opérationnel).</summary>
    [System.Serializable]
    public class FluxOffer
    {
        /// <summary>Identifiant.</summary>
        public int id;
        /// <summary>Nom du partenaire.</summary>
        public string name;
        /// <summary>Origine fictive.</summary>
        public string origin;
        /// <summary>Volume 1-5.</summary>
        [Range(1, 5)] public int volume = 1;
        /// <summary>Argent immobilisé (€).</summary>
        [Min(0f)] public float commitment;
        /// <summary>Coût d'activation (€).</summary>
        [Min(0f)] public float activationCost;
        /// <summary>Fiabilité de départ (%).</summary>
        [Range(0f, 100f)] public float reliability = 60f;
        /// <summary>Instabilité de départ.</summary>
        [Range(0f, 100f)] public float instability = 20f;
        /// <summary>Revenu par cycle réussi (€).</summary>
        [Min(0f)] public float revenue;
        /// <summary>Coût par cycle (€).</summary>
        [Min(0f)] public float costPerCycle;
    }

    public partial class GameConfig
    {
        [Header("Flux internationaux")]
        [Tooltip("Offres disponibles en début de partie.")]
        public List<FluxOffer> fluxOffers = new List<FluxOffer>
        {
            new FluxOffer { id = 1, name = "Partenaire Sud", origin = "Sud", volume = 2, commitment = 8000f, activationCost = 2500f, reliability = 65f, instability = 20f, revenue = 2600f, costPerCycle = 700f },
            new FluxOffer { id = 2, name = "Partenaire Nord", origin = "Nord", volume = 3, commitment = 15000f, activationCost = 4000f, reliability = 55f, instability = 30f, revenue = 4800f, costPerCycle = 1200f }
        };
        [Tooltip("Exposition du flux gagnée par cycle et par point de volume.")]
        [Min(0f)] public float fluxExposurePerVolume = 3f;
        [Tooltip("Part de cette exposition reportée sur l'indicateur Exposition du joueur.")]
        [Range(0f, 1f)] public float fluxPlayerExposureShare = 0.5f;
        [Tooltip("Instabilité gagnée par cycle actif.")]
        [Min(0f)] public float fluxInstabilityPerCycle = 2f;
        [Tooltip("Probabilité de rupture = instabilité / ce diviseur.")]
        [Min(1f)] public float fluxRuptureDivisor = 250f;
        [Tooltip("Fiabilité perdue après un cycle raté.")]
        [Min(0f)] public float fluxFailedReliabilityLoss = 5f;
        [Tooltip("Sous cette fiabilité, le partenaire disparaît et le flux s'arrête.")]
        [Range(0f, 100f)] public float fluxPartnerLostReliability = 30f;
        [Tooltip("Au-delà de ce nombre de flux actifs, la stabilité baisse.")]
        [Min(0)] public int fluxComfortableCount = 2;
        [Tooltip("Rupture : part de l'engagement ajoutée à la dette.")]
        [Range(0f, 1f)] public float fluxRuptureDebtShare = 0.25f;
        [Tooltip("Rupture : pression ajoutée.")]
        [Min(0f)] public float fluxRupturePressure = 8f;
        [Tooltip("Rupture : probabilité de perdre un opérateur Logistique.")]
        [Range(0f, 1f)] public float fluxRuptureOperatorLoss = 0.4f;

        /// <summary>Offre de flux par identifiant.</summary>
        /// <param name="id">Identifiant.</param>
        /// <returns>L'offre, ou null.</returns>
        public FluxOffer GetFluxOffer(int id)
        {
            for (int i = 0; i < fluxOffers.Count; i++) if (fluxOffers[i].id == id) return fluxOffers[i];
            return null;
        }
    }
}

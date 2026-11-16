// MARSEILLE — UI/FluxPanel.cs
// Écran « Flux » (spec §8) : offres de partenaires fictifs, chiffres abstraits (engagement, fiabilité,
// exposition, instabilité, revenu attendu), activation et coupure (confirmation en deux touches).

using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    /// <summary>Écran des flux internationaux.</summary>
    public class FluxPanel : ListPanel
    {
        private int confirmCut = Owners.None;

        /// <inheritdoc />
        public override string TabLabel { get { return "Flux"; } }

        /// <summary>Remplit la liste des flux.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        protected override void Fill(GameData data, GameConfig config)
        {
            Title("Flux internationaux (" + FluxActions.ActiveCount(data) + " actif(s))", config);
            UiKit.Label(Column, "Au-delà de " + config.fluxComfortableCount + " flux, la stabilité baisse.", config.uiFontSizes.z, config.uiTextDim);
            for (int i = 0; i < data.fluxes.Count; i++) AddFluxCard(data.fluxes[i], data, config);
        }

        /// <summary>Carte d'un flux.</summary>
        /// <param name="f">Flux.</param>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void AddFluxCard(FluxData f, GameData data, GameConfig config)
        {
            UiCard card = UiCard.Create(Column, 5, config);
            string state = f.isActive
                ? "Actif depuis " + f.activeCycles + " cycle(s)" + (f.activeCycles > 0 ? (f.lastCycleSucceeded ? " · dernier cycle réussi" : " · dernier cycle raté") : "")
                : "Inactif";
            card.Text.text = f.name + " · origine " + f.origin + " · volume " + f.volume + "/5\n"
                + state + "\n"
                + "Fiabilité " + f.reliability.ToString("0") + " % · exposition " + f.exposure.ToString("0") + " · instabilité " + f.instability.ToString("0") + "\n"
                + "Revenu " + UiText.Money(f.potentialRevenue) + " · coût " + UiText.Money(f.costPerCycle) + "/cycle · attendu " + UiText.SignedMoney(f.ExpectedNet()) + "\n"
                + (f.isActive ? "Engagement immobilisé " + UiText.Money(f.financialCommitment)
                              : "Activation " + UiText.Money(f.activationCost) + " + engagement " + UiText.Money(f.financialCommitment));
            int id = f.id;
            if (f.isActive)
            {
                bool confirming = confirmCut == id;
                card.AddButton(confirming ? "Confirmer la coupure" : "Couper", confirming ? config.uiNegative : config.uiButton, () => Cut(id));
            }
            else
            {
                bool affordable = data.indicators.treasury >= FluxActions.ActivationTotal(f);
                card.AddButton(affordable ? "Activer" : "Trésorerie insuffisante", affordable ? config.uiAccent : config.uiButton, () => Activate(id)).interactable = affordable;
            }
        }

        /// <summary>Active un flux.</summary>
        /// <param name="fluxId">Flux.</param>
        private void Activate(int fluxId)
        {
            confirmCut = Owners.None;
            if (Manager != null) Manager.Perform((ref GameData d, GameConfig c) => FluxActions.Activate(ref d, fluxId, c), Owners.None);
        }

        /// <summary>Coupe un flux à la seconde touche.</summary>
        /// <param name="fluxId">Flux.</param>
        private void Cut(int fluxId)
        {
            if (confirmCut != fluxId) { confirmCut = fluxId; Refresh(); return; }
            confirmCut = Owners.None;
            if (Manager != null) Manager.Perform((ref GameData d, GameConfig c) => FluxActions.Cut(ref d, fluxId), Owners.None);
        }
    }
}

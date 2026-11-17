// MARSEILLE — UI/QuarterDetailPopup.Facade.cs
// Partie « façades » du détail de quartier : celles qui tournent (crédibilité, Éveil, fermer) et celles qu'on peut ouvrir.
// Chaque bouton dit d'abord ce que ça coûte. Les règles sont dans FacadeActions.

using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    public partial class QuarterDetailPopup
    {
        /// <summary>Façades du quartier : ouvertes et ouvrables.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddFacades(QuarterData q, GameData data, GameConfig c)
        {
            if (q.control <= 0f) return;
            UiKit.Label(Body, "Façades", c.uiFontSizes.y, c.uiText);
            int qid = q.id;
            System.Array sectors = System.Enum.GetValues(typeof(FacadeSector));
            for (int k = 0; k < sectors.Length; k++)
            {
                FacadeSector sector = (FacadeSector)sectors.GetValue(k);
                int fi = FacadeRules.Find(data, qid, sector);
                if (fi >= 0) AddOpenFacade(data.shadow.facades[fi], q, c); else AddClosedFacade(sector, data, qid, c);
            }
        }

        /// <summary>Une façade qui tourne : ce que le quartier peut croire, l'Éveil, fermer.</summary>
        /// <param name="f">Façade.</param>
        /// <param name="q">Quartier.</param>
        /// <param name="c">Configuration.</param>
        private void AddOpenFacade(FacadeData f, QuarterData q, GameConfig c)
        {
            FacadeSpec spec = FacadeRules.Spec(f.sector, c);
            UiCard card = UiCard.Create(Body, 2, c);
            string state = f.frozen > 0 ? "contrôle en cours (" + f.frozen + " sem.)" : f.age < spec.delay ? "démarre dans " + (spec.delay - f.age) + " sem." : "tourne";
            card.Text.text = spec.name + " : " + state + "\nCrédibilité " + FacadeRules.Capacity(f, q, c).ToString("0.0")
                             + " pour " + spec.absorption.ToString("0.0") + "  ·  Éveil " + f.eveil.ToString("0");
            FacadeSector sector = f.sector;
            int qid = q.id;
            float cost = FacadeActions.CloseCost(sector, c);
            card.AddButton("Fermer (" + UiText.Money(cost) + ")", c.uiButton,
                           () => Act((ref GameData d, GameConfig cfg) => FacadeActions.Close(ref d, qid, sector, cfg)));
        }

        /// <summary>Une façade qu'on peut ouvrir (ou la raison pour laquelle on ne peut pas).</summary>
        /// <param name="sector">Secteur.</param>
        /// <param name="data">Partie.</param>
        /// <param name="qid">Quartier.</param>
        /// <param name="c">Configuration.</param>
        private void AddClosedFacade(FacadeSector sector, GameData data, int qid, GameConfig c)
        {
            FacadeSpec spec = FacadeRules.Spec(sector, c);
            ActionResult check = FacadeActions.CanOpen(data, qid, sector, c);
            UiCard card = UiCard.Create(Body, 1, c);
            card.Text.text = spec.name + " : absorbe " + spec.absorption.ToString("0.0") + " par semaine, effet après " + spec.delay + " sem.";
            string label = check.ok ? "Ouvrir (" + UiText.Money(spec.openCost) + ")" : check.message;
            card.AddButton(label, c.uiButton,
                           () => Act((ref GameData d, GameConfig cfg) => FacadeActions.Open(ref d, qid, sector, cfg))).interactable = check.ok;
        }
    }
}

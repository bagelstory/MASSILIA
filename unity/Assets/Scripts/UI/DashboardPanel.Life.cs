// MARSEILLE — UI/DashboardPanel.Life.cs
// Carte « Vie » du bilan : âge, foyer, usure, casier, double vie, dette personnelle, justice en cours,
// et deux gestes (se soigner, un repas). Lecture des jauges de design/20-consequences.md.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    public partial class DashboardPanel
    {
        private Text lifeText;
        private Button careButton;
        private Button mealButton;

        /// <summary>Construit la carte « Vie » dans la colonne du bilan.</summary>
        /// <param name="col">Colonne.</param>
        /// <param name="config">Configuration.</param>
        private void BuildLife(RectTransform col, GameConfig config)
        {
            UiKit.Label(col, "Vie", config.uiFontSizes.x, config.uiText);
            lifeText = MultiLine(col, 5, config);
            RectTransform row = UiKit.Row("LifeActions", col, config.uiButtonHeight * 0.8f, config);
            careButton = UiKit.MakeButton(row, "Se soigner", config.uiButton, () => Manager.Perform(ConsequenceActions.Care, Owners.None), config);
            mealButton = UiKit.MakeButton(row, "Repas", config.uiButton, () => Manager.Perform(ConsequenceActions.Meal, Owners.None), config);
            careButton.GetComponentInChildren<Text>().fontSize = config.uiFontSizes.z;
            mealButton.GetComponentInChildren<Text>().fontSize = config.uiFontSizes.z;
        }

        /// <summary>Met à jour la carte « Vie ».</summary>
        /// <param name="d">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <param name="planning">Vrai pendant la planification.</param>
        private void RefreshLife(GameData d, GameConfig c, bool planning)
        {
            if (lifeText == null) return;
            lifeText.text = ConsequenceRules.Describe(d, c);
            careButton.interactable = planning;
            mealButton.interactable = planning && d.life.repasEnAttente == 0;
            UiKit.SetButtonText(careButton, "Se soigner · " + c.santeSoinCout.ToString("N0") + " €");
            UiKit.SetButtonText(mealButton, d.life.repasEnAttente > 0 ? "Repas prévu" : "Repas");
        }
    }
}

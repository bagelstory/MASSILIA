// MARSEILLE — UI/DashboardPanel.cs
// Écran « Tableau de bord » (spec §12.3) : les 6 indicateurs, la prévision du prochain cycle,
// capacité et dette, les objectifs en cours, le bouton de validation du cycle.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Tableau de bord.</summary>
    public partial class DashboardPanel : UIPanel
    {
        private IndicatorBar territory;
        private IndicatorBar treasury;
        private IndicatorBar effectifs;
        private IndicatorBar pressure;
        private IndicatorBar stability;
        private IndicatorBar exposure;
        private Text forecast;
        private Text network;
        private Text quests;
        private Button validate;

        /// <inheritdoc />
        public override string TabLabel { get { return "Bilan"; } }

        /// <summary>Construit l'écran.</summary>
        /// <param name="config">Configuration.</param>
        protected override void Build(GameConfig config)
        {
            UiKit.Panel("Background", transform, config.uiBackground).raycastTarget = false;
            RectTransform col = UiKit.ScrollColumn(transform, config);
            UiKit.Label(col, "Indicateurs", config.uiFontSizes.x, config.uiText);
            territory = IndicatorBar.Create(col, "Territoire", config);
            treasury = IndicatorBar.Create(col, "Trésorerie", config);
            effectifs = IndicatorBar.Create(col, "Effectifs", config);
            pressure = IndicatorBar.Create(col, "Pression", config);
            stability = IndicatorBar.Create(col, "Stabilité", config);
            exposure = IndicatorBar.Create(col, "Exposition", config);
            UiKit.Label(col, "Prochain cycle", config.uiFontSizes.x, config.uiText);
            forecast = MultiLine(col, 7, config);
            network = MultiLine(col, 3, config);
            UiKit.Label(col, "Objectifs", config.uiFontSizes.x, config.uiText);
            quests = MultiLine(col, 7, config);
            validate = UiKit.MakeButton(col, "Valider le cycle", config.uiAccent, OnValidate, config);
        }

        /// <summary>Met à jour tout l'écran.</summary>
        public override void Refresh()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame || territory == null) return;
            GameConfig c = gm.Config;
            GameData d = gm.Data;
            IndicatorData ind = d.indicators;
            territory.Set(ind.territory / Mathf.Max(1f, c.victoryTerritory), ind.territory.ToString("0.0") + " % (victoire " + c.victoryTerritory.ToString("0") + " %)", false, c);
            treasury.Set(ind.treasury / Mathf.Max(1f, c.startingTreasury * 4f), UiText.Money(ind.treasury), false, c);
            int active = OperatorRules.ActiveCount(d);
            effectifs.Set(d.capacity > 0 ? (float)active / d.capacity : 0f, ind.effectifsAvailable + " dispo · " + active + "/" + d.capacity, false, c);
            pressure.Set(ind.pressure / 100f, ind.pressure.ToString("0"), true, c);
            stability.Set(ind.stability / 100f, ind.stability.ToString("0"), false, c);
            exposure.Set(ind.exposure / 100f, ind.exposure.ToString("0"), true, c);
            forecast.text = ForecastText(d, c);
            network.text = NetworkText(d, c);
            quests.text = QuestsText(d, c);
            if (validate != null) validate.interactable = gm.State == GameState.Planning;
        }

        /// <summary>Texte sur plusieurs lignes, hauteur réservée.</summary>
        /// <param name="parent">Colonne.</param>
        /// <param name="lines">Nombre de lignes prévues.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le texte.</returns>
        private static Text MultiLine(Transform parent, int lines, GameConfig config)
        {
            Text t = UiKit.Label(parent, string.Empty, config.uiFontSizes.z, config.uiText);
            t.alignment = TextAnchor.UpperLeft;
            UiKit.SetHeight(t.rectTransform, config.uiFontSizes.z * 1.25f * lines);
            return t;
        }

        /// <summary>Bouton « Valider le cycle ».</summary>
        private void OnValidate()
        {
            if (Manager != null) Manager.ValidateCycle();
        }
    }
}

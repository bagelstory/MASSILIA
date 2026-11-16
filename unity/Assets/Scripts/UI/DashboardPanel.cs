// MARSEILLE — UI/DashboardPanel.cs
// Écran « Tableau de bord » (spec §12.3) : les 6 indicateurs, la prévision du prochain cycle,
// capacité et dette, les objectifs en cours, le bouton de validation du cycle.

using Marseille.Art;
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
        private Button abandon;
        private bool confirmAbandon;

        /// <inheritdoc />
        public override string TabLabel { get { return "Bilan"; } }

        /// <inheritdoc />
        public override string TabIcon { get { return "tab_bilan"; } }

        /// <summary>Construit l'écran.</summary>
        /// <param name="config">Configuration.</param>
        protected override void Build(GameConfig config)
        {
            UiKit.Panel("Background", transform, config.uiBackground).raycastTarget = false;
            RectTransform col = UiKit.ScrollColumn(transform, config);
            UiKit.Label(col, "Indicateurs", config.uiFontSizes.x, config.uiText);
            territory = IndicatorBar.Create(col, "Territoire", config, ArtLibrary.Indicator("territoire"));
            treasury = IndicatorBar.Create(col, "Trésorerie", config, ArtLibrary.Indicator("tresorerie"));
            effectifs = IndicatorBar.Create(col, "Effectifs", config, ArtLibrary.Indicator("effectifs"));
            pressure = IndicatorBar.Create(col, "Pression", config, ArtLibrary.Indicator("pression"));
            stability = IndicatorBar.Create(col, "Stabilité", config, ArtLibrary.Indicator("stabilite"));
            exposure = IndicatorBar.Create(col, "Exposition", config, ArtLibrary.Indicator("exposition"));
            UiKit.Label(col, "Prochain cycle", config.uiFontSizes.x, config.uiText);
            forecast = MultiLine(col, 7, config);
            network = MultiLine(col, 3, config);
            UiKit.Label(col, "Objectifs", config.uiFontSizes.x, config.uiText);
            quests = MultiLine(col, 7, config);
            validate = UiKit.MakeButton(col, "Valider le cycle", config.uiAccent, OnValidate, config);
            abandon = UiKit.MakeButton(col, "Abandonner la partie", config.uiButton, OnAbandon, config);
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
            if (abandon != null)
            {
                abandon.interactable = gm.State == GameState.Planning;
                UiKit.SetButtonText(abandon, confirmAbandon ? "Confirmer l'abandon" : "Abandonner la partie");
                abandon.targetGraphic.color = confirmAbandon ? c.uiNegative : c.uiButton;
            }
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

        /// <summary>Bouton « Abandonner » : il faut toucher deux fois.</summary>
        private void OnAbandon()
        {
            if (!confirmAbandon) { confirmAbandon = true; Refresh(); return; }
            confirmAbandon = false;
            if (GameManager.Instance != null) GameManager.Instance.Abandon();
        }

        /// <summary>Bouton « Valider le cycle ».</summary>
        private void OnValidate()
        {
            confirmAbandon = false;
            if (Manager != null) Manager.ValidateCycle();
        }
    }
}

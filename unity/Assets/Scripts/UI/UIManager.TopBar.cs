// MARSEILLE — UI/UIManager.TopBar.cs
// Barre du haut compacte : date et trésorerie, palier d'Ombre, quatre puces (territoire, pression,
// effectif, solde prévu).

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Map;
using Marseille.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    public partial class UIManager
    {
        private Text topDate;
        private Text topMoney;
        private Text topCycle;
        private Text topShadow;
        private readonly Text[] chipValue = new Text[4];

        private static readonly string[] ChipCaption = { "Territoire", "Pression", "Effectif", "Prévision" };

        /// <summary>Construit la barre du haut (deux lignes : situation, puces).</summary>
        /// <param name="config">Configuration.</param>
        private void BuildTopBar(GameConfig config)
        {
            Image bar = UiFactory.CreateImage("TopBar", transform, config.uiPanel, true);
            UiFactory.Stretch(bar.rectTransform, new Vector2(0f, 1f), Vector2.one);
            bar.rectTransform.offsetMin = new Vector2(0f, -config.uiBarHeights.x);
            topDate = TopText(bar.transform, "Date", config.uiFontSizes.z, config.uiTextDim, TextAnchor.UpperLeft, new Vector2(0.03f, 0.62f), new Vector2(0.62f, 0.97f));
            topMoney = TopText(bar.transform, "Money", config.uiFontSizes.x, config.uiText, TextAnchor.MiddleLeft, new Vector2(0.03f, 0.40f), new Vector2(0.62f, 0.72f));
            topCycle = TopText(bar.transform, "Cycle", config.uiFontSizes.z, config.uiTextDim, TextAnchor.UpperRight, new Vector2(0.55f, 0.62f), new Vector2(0.97f, 0.97f));
            topShadow = TopText(bar.transform, "Shadow", config.uiFontSizes.z, config.uiWarning, TextAnchor.MiddleRight, new Vector2(0.55f, 0.40f), new Vector2(0.97f, 0.72f));
            for (int i = 0; i < 4; i++)
            {
                float x0 = 0.03f + i * 0.2375f, x1 = x0 + 0.22f;
                Image chip = UiFactory.CreateImage("Chip" + i, bar.transform, config.uiButton, false);
                UiSprites.Round(chip);
                UiFactory.Stretch(chip.rectTransform, new Vector2(x0, 0.05f), new Vector2(x1, 0.40f));
                chipValue[i] = TopText(chip.transform, "Value", config.uiFontSizes.z + 3, config.uiText, TextAnchor.MiddleCenter, new Vector2(0f, 0.42f), new Vector2(1f, 1f));
                TopText(chip.transform, "Caption", config.uiFontSizes.z - 6, config.uiTextDim, TextAnchor.UpperCenter, new Vector2(0f, 0f), new Vector2(1f, 0.46f)).text = ChipCaption[i];
            }
        }

        private static Text TopText(Transform parent, string name, int size, Color color, TextAnchor anchor, Vector2 min, Vector2 max)
        {
            Text t = UiFactory.CreateText(name, parent, UiKit.Font, size, color);
            t.alignment = anchor;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            UiFactory.Stretch(t.rectTransform, min, max);
            return t;
        }

        /// <summary>Barre du haut : date, trésorerie, palier d'Ombre, puces.</summary>
        public void RefreshTopBar()
        {
            GameManager gm = GameManager.Instance;
            if (topDate == null || gm == null || !gm.HasGame) return;
            GameData d = gm.Data;
            GameConfig c = gm.Config;
            int tier = ShadowRules.Tier(d.shadow.ombre, c);
            int year = (d.currentCycle - 1) / Mathf.Max(1, c.weeksPerYear) + 1;
            topDate.text = "Semaine " + d.currentCycle + "  ·  Année " + year + "  ·  " + c.AgeAt(d.currentCycle) + " ans";
            topCycle.text = SeasonRules.Label(d.currentCycle, c);
            topMoney.text = d.indicators.treasury.ToString("N0") + " €" + (d.debt > 0f ? "  (dette " + d.debt.ToString("N0") + ")" : string.Empty);
            topShadow.text = tier > 0 ? ShadowRules.TierName(tier) : string.Empty;
            chipValue[0].text = d.indicators.territory.ToString("0.0") + " %";
            chipValue[1].text = d.indicators.pressure.ToString("0");
            chipValue[1].color = d.indicators.pressure >= c.uiPressureWarn ? c.uiAccent : c.uiText;
            chipValue[2].text = OperatorRules.ActiveCount(d) + "/" + d.capacity;
            float net = ForecastNet(d, c);
            chipValue[3].text = (net >= 0f ? "+" : "−") + Mathf.Abs(net).ToString("N0");
            chipValue[3].color = net >= 0f ? c.uiPositive : c.uiNegative;
        }

        private static float ForecastNet(GameData d, GameConfig c)
        {
            float net = EconomyActions.Forecast(d, c).Net;
            for (int i = 0; i < d.fluxes.Count; i++) if (d.fluxes[i].isActive) net += d.fluxes[i].ExpectedNet();
            return net;
        }
    }
}

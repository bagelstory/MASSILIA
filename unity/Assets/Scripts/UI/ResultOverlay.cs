// MARSEILLE — UI/ResultOverlay.cs
// Résultats d'un cycle (spec §10.3) : variations des indicateurs puis lignes du rapport.
// « Continuer » rend la main à la planification (GameManager.AcknowledgeResults).

using System.Text;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;
using UnityEngine;

namespace Marseille.UI
{
    /// <summary>Écran des résultats d'un cycle.</summary>
    public class ResultOverlay : UIOverlay
    {
        [Tooltip("Nombre maximal de lignes du rapport affichées.")]
        [SerializeField, Min(5)] private int maxLines = 40;

        /// <summary>S'abonne aux cycles résolus et au chargement.</summary>
        protected override void OnInitialized()
        {
            EventBus.OnCycleResolved += Show;
            EventBus.OnGameLoaded += ResumeIfNeeded;
        }

        /// <summary>Désabonnements.</summary>
        private void OnDestroy()
        {
            EventBus.OnCycleResolved -= Show;
            EventBus.OnGameLoaded -= ResumeIfNeeded;
        }

        /// <summary>Une partie rechargée sur un écran de résultats revient en planification.</summary>
        private void ResumeIfNeeded()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.HasGame && gm.State == GameState.Results) gm.AcknowledgeResults();
        }

        /// <summary>Affiche les résultats (pas en fin de partie : GameOverPanel s'en charge).</summary>
        /// <param name="result">Résultat du cycle.</param>
        private void Show(CycleResult result)
        {
            if (result.endType != EndType.None) return;
            GameConfig c = Config;
            if (c == null) return;
            Open();
            ClearContent();
            UiKit.Label(Body, "Cycle " + result.cycle + " · " + SeasonRules.Label(result.cycle, c), c.uiFontSizes.x, c.uiText);
            Delta("Trésorerie", result.treasuryDelta, " €", false, c);
            Delta("Territoire", result.territoryDelta, " %", false, c);
            Delta("Pression", result.pressureDelta, "", true, c);
            Delta("Stabilité", result.stabilityDelta, "", false, c);
            Delta("Exposition", result.exposureDelta, "", true, c);
            UiKit.Label(Body, "Rapport", c.uiFontSizes.y, c.uiText);
            int n = 0;
            for (int i = 0; i < result.lines.Count && n < maxLines; i++, n++)
            {
                JournalEntry e = result.lines[i];
                Paragraph("· " + e.text, e.text.Length > 48 ? 2 : 1, e.type == JournalType.Loss || e.type == JournalType.OperatorLost || e.type == JournalType.Attack ? c.uiNegative : c.uiText);
            }
            if (result.lines.Count == 0) Paragraph("Cycle calme.", 1, c.uiTextDim);
            FooterButton("Continuer", c.uiAccent, Continue);
        }

        /// <summary>Ligne de variation colorée.</summary>
        /// <param name="name">Indicateur.</param>
        /// <param name="value">Variation.</param>
        /// <param name="unit">Unité.</param>
        /// <param name="higherIsWorse">Vrai si une hausse est mauvaise.</param>
        /// <param name="c">Configuration.</param>
        private void Delta(string name, float value, string unit, bool higherIsWorse, GameConfig c)
        {
            bool good = higherIsWorse ? value <= 0f : value >= 0f;
            string v = (value >= 0f ? "+" : "−") + System.Math.Abs(value).ToString(unit == " €" ? "N0" : "0.0") + unit;
            Paragraph(name + " : " + v, 1, System.Math.Abs(value) < 0.05f ? c.uiTextDim : good ? c.uiPositive : c.uiNegative);
        }

        /// <summary>Retour à la planification.</summary>
        private void Continue()
        {
            Close();
            if (GameManager.Instance != null) GameManager.Instance.AcknowledgeResults();
        }
    }
}

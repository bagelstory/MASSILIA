// MARSEILLE — UI/GameOverPanel.cs
// Fin de partie (spec §11) : type de fin, bilan chiffré, score et note de A à E, nouvelle partie.

using System.Text;
using Marseille.Art;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    /// <summary>Écran de fin de partie.</summary>
    public class GameOverPanel : UIOverlay
    {
        /// <summary>S'abonne à la fin de partie et au chargement.</summary>
        protected override void OnInitialized()
        {
            EventBus.OnGameOver += Show;
            EventBus.OnGameLoaded += ShowIfOver;
        }

        /// <summary>Désabonnements.</summary>
        private void OnDestroy()
        {
            EventBus.OnGameOver -= Show;
            EventBus.OnGameLoaded -= ShowIfOver;
        }

        /// <summary>Une partie terminée rechargée réaffiche le bilan.</summary>
        private void ShowIfOver()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.HasGame && gm.State == GameState.GameOver) Show(gm.Data.endType);
            else Close();
        }

        /// <summary>Affiche le bilan.</summary>
        /// <param name="endType">Type de fin.</param>
        private void Show(EndType endType)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame) return;
            GameData d = gm.Data;
            GameConfig c = gm.Config;
            Open();
            ClearContent();
            float score = EndRules.Score(d, c);
            UiKit.Picture(Body, ArtLibrary.Logo(), c.uiButtonHeight * 1.4f);
            UiKit.Picture(Body, ArtLibrary.Screen(EndImage(endType)), c.uiButtonHeight * 5f);
            UiKit.Label(Body, Title(endType), c.uiFontSizes.x, endType == EndType.Victory ? c.uiPositive : c.uiText);
            Paragraph(EndRules.Message(endType), 2, c.uiTextDim);
            UiKit.Label(Body, "Note " + EndRules.Grade(score, c) + " · score " + score.ToString("0"), c.uiFontSizes.x, c.uiAccent);
            Paragraph(Summary(d, c), 7, c.uiText);
            FooterButton("Journal", c.uiButton, OpenJournal);
            FooterButton("Nouvelle partie", c.uiAccent, NewGame);
        }

        /// <summary>Titre selon le type de fin.</summary>
        /// <param name="endType">Type de fin.</param>
        /// <returns>Titre.</returns>
        private static string Title(EndType endType)
        {
            switch (endType)
            {
                case EndType.Victory: return "Victoire";
                case EndType.Bankruptcy: return "Faillite";
                case EndType.Collapse: return "Chute";
                case EndType.Abandon: return "Abandon";
                case EndType.TimeOut: return "Bilan d'une vie";
                case EndType.Retirement: return "Retraite";
            }
            return "Fin de partie";
        }

        /// <summary>Illustration de la fin (Resources/Art/screens).</summary>
        /// <param name="endType">Type de fin.</param>
        /// <returns>Nom du fichier.</returns>
        private static string EndImage(EndType endType)
        {
            switch (endType)
            {
                case EndType.Victory: return "end_victoire";
                case EndType.Bankruptcy: return "end_faillite";
                case EndType.Collapse: return "end_chute";
                case EndType.Abandon: return "end_abandon";
            }
            return "end_temps";
        }

        /// <summary>Bilan chiffré.</summary>
        /// <param name="d">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string Summary(GameData d, GameConfig c)
        {
            int alive = 0;
            for (int i = 0; i < d.factions.Count; i++) if (d.factions[i].isAlive) alive++;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Cycles joués : " + d.currentCycle + " (" + SeasonRules.Label(d.currentCycle, c) + ") · " + c.AgeAt(d.currentCycle) + " ans");
            sb.AppendLine("Territoire : " + d.indicators.territory.ToString("0.0") + " %");
            sb.AppendLine("Trésorerie : " + UiText.Money(d.indicators.treasury) + (d.debt > 0f ? " · dette " + UiText.Money(d.debt) : ""));
            sb.AppendLine("Opérateurs : " + OperatorRules.ActiveCount(d) + " · flux actifs : " + FluxActions.ActiveCount(d));
            sb.AppendLine("Équipes rivales encore là : " + alive + "/" + d.factions.Count);
            sb.Append("Pression " + d.indicators.pressure.ToString("0") + " · stabilité " + d.indicators.stability.ToString("0")
                      + " · exposition " + d.indicators.exposure.ToString("0"));
            return sb.ToString();
        }

        /// <summary>Ferme le bilan et montre le journal (la partie reste terminée).</summary>
        private void OpenJournal()
        {
            Close();
            if (Manager != null) Manager.ShowPanel<JournalPanel>();
        }

        /// <summary>Efface la partie et en lance une nouvelle.</summary>
        private void NewGame()
        {
            Close();
            GameManager gm = GameManager.Instance;
            if (gm == null) return;
            gm.DeleteGame();
            gm.NewGame();
        }
    }
}

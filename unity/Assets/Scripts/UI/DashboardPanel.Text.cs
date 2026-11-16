// MARSEILLE — UI/DashboardPanel.Text.cs
// Partie « textes » du tableau de bord : prévision chiffrée, réseau (capacité, dette, alertes), objectifs.

using System.Text;
using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    public partial class DashboardPanel
    {
        /// <summary>Prévision du prochain cycle : revenus, coûts, flux (espérance), solde.</summary>
        /// <param name="d">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string ForecastText(GameData d, GameConfig c)
        {
            EconomyForecast f = EconomyActions.Forecast(d, c);
            float flux = 0f;
            for (int i = 0; i < d.fluxes.Count; i++) if (d.fluxes[i].isActive) flux += d.fluxes[i].ExpectedNet();
            float net = f.Net + flux;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Revenus des quartiers : " + UiText.SignedMoney(f.income));
            sb.AppendLine("Maintien des quartiers : " + UiText.SignedMoney(-f.maintenance));
            sb.AppendLine("Entretien des opérateurs : " + UiText.SignedMoney(-f.upkeep));
            if (flux != 0f) sb.AppendLine("Flux (attendu) : " + UiText.SignedMoney(flux));
            if (f.interest > 0f) sb.AppendLine("Intérêts de la dette : " + UiText.SignedMoney(-f.interest));
            sb.Append("Solde prévu : " + UiText.SignedMoney(net));
            return sb.ToString();
        }

        /// <summary>Capacité, dette et alertes.</summary>
        /// <param name="d">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string NetworkText(GameData d, GameConfig c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Capacité : " + OperatorRules.ActiveCount(d) + "/" + d.capacity);
            sb.Append("  ·  Flux actifs : " + FluxActions.ActiveCount(d));
            if (d.debt > 0f) sb.Append("\nDette : " + UiText.Money(d.debt));
            if (d.zeroTreasuryCycles > 0) sb.Append("\nCaisse vide depuis " + d.zeroTreasuryCycles + "/" + c.bankruptcyCycles + " cycles.");
            else if (d.indicators.pressure >= c.pressureAlert) sb.Append("\nPression en alerte : descentes possibles.");
            return sb.ToString();
        }

        /// <summary>Objectifs en cours avec avancement (les ouvertures de zone d'abord).</summary>
        /// <param name="d">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string QuestsText(GameData d, GameConfig c)
        {
            StringBuilder sb = new StringBuilder();
            int shown = 0;
            for (int pass = 0; pass < 2 && shown < 6; pass++)
                for (int i = 0; i < d.quests.Count && shown < 6; i++)
                {
                    QuestData q = d.quests[i];
                    bool zone = q.zoneToUnlock != Owners.None;
                    if (q.isCompleted || (pass == 0) != zone) continue;
                    sb.Append(q.name).Append(" : ");
                    for (int o = 0; o < q.objectives.Count; o++)
                    {
                        QuestObjective obj = q.objectives[o];
                        float v = QuestRules.Progress(d, obj, c);
                        sb.Append(o > 0 ? ", " : "").Append(System.Math.Min(v, obj.target).ToString("0")).Append("/").Append(obj.target.ToString("0"));
                    }
                    sb.AppendLine();
                    shown++;
                }
            return shown == 0 ? "Aucun objectif en cours." : sb.ToString().TrimEnd();
        }
    }
}

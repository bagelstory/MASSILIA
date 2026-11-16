// MARSEILLE — UI/QuarterDetailPopup.cs
// Fenêtre « détail d'un quartier » : statut, contrôle, présences adverses, économie locale,
// investissement, opérateurs sur place. Plus la connaissance est haute, plus on en voit (spec §3.5).

using System.Text;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    /// <summary>Détail d'un quartier.</summary>
    public partial class QuarterDetailPopup : UIOverlay
    {
        private int quarterId = Owners.None;

        /// <summary>S'abonne aux demandes d'ouverture.</summary>
        protected override void OnInitialized()
        {
            if (Manager != null) Manager.QuarterDetailRequested += Show;
            EventBus.OnDataChanged += RefreshIfOpen;
        }

        /// <summary>Désabonnements.</summary>
        private void OnDestroy()
        {
            if (Manager != null) Manager.QuarterDetailRequested -= Show;
            EventBus.OnDataChanged -= RefreshIfOpen;
        }

        /// <summary>Ouvre la fenêtre pour un quartier.</summary>
        /// <param name="id">Quartier.</param>
        public void Show(int id)
        {
            quarterId = id;
            Open();
            Fill();
        }

        /// <summary>Remet à jour après une action (charbon, investissement).</summary>
        private void RefreshIfOpen()
        {
            if (IsOpen) Fill();
        }

        /// <summary>Remplit la fenêtre.</summary>
        private void Fill()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame) return;
            GameData data = gm.Data;
            GameConfig c = gm.Config;
            int qi = data.IndexOfQuarter(quarterId);
            ClearContent();
            FooterButton("Fermer", c.uiButton, Close);
            if (qi < 0) return;
            QuarterData q = data.quarters[qi];
            UiKit.Label(Body, q.name, c.uiFontSizes.x, c.uiText);
            Paragraph(Summary(q, data, c), 4, c.uiText);
            if (q.knowledgeLevel <= 0 && q.control <= 0f) { Paragraph("Quartier inconnu. Envoie un Repérage depuis un quartier voisin.", 2, c.uiTextDim); return; }
            Paragraph(Presences(q, data), 1 + q.factionPresences.Count, c.uiText);
            Paragraph(Economy(q, data, c), 3, c.uiText);
            AddCharbons(q, data, c);
            AddInvest(q, data, c);
        }

        /// <summary>Zone, statut, contrôle, connaissance, opérateurs présents.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string Summary(QuarterData q, GameData data, GameConfig c)
        {
            int zi = data.IndexOfZone(q.zoneId);
            StringBuilder sb = new StringBuilder();
            sb.Append(q.arrondissement).Append(q.arrondissement == 1 ? "er" : "e").Append(" arrondissement · ")
              .Append(zi >= 0 ? data.zones[zi].name + (data.zones[zi].isUnlocked ? "" : " (zone verrouillée)") : "?").AppendLine();
            sb.Append(UiText.Status(TerritoryRules.Status(q, c))).Append(" · contrôle ").Append(q.control.ToString("0")).Append(" %")
              .Append(" · connaissance ").Append(q.knowledgeLevel).Append("/3").AppendLine();
            string ops = string.Empty;
            for (int i = 0; i < data.operators.Count; i++)
                if (OperatorRules.IsOnSite(data.operators[i]) && data.operators[i].currentQuarter == q.id)
                    ops += (ops.Length > 0 ? ", " : "") + UiText.OperatorName(data.operators[i]);
            sb.Append("Sur place : ").Append(ops.Length > 0 ? ops : "personne");
            if (q.control > 0f && q.cyclesWithoutOperator > 0) sb.Append(" (depuis ").Append(q.cyclesWithoutOperator).Append(" cycle(s))");
            return sb.ToString();
        }

        /// <summary>Présence de chaque faction (précise à partir de la connaissance 2).</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <returns>Texte.</returns>
        private static string Presences(QuarterData q, GameData data)
        {
            if (q.factionPresences.Count == 0) return "Aucune présence adverse.";
            StringBuilder sb = new StringBuilder("Présence adverse " + q.hostilePresence.ToString("0") + " %");
            for (int i = 0; i < q.factionPresences.Count; i++)
            {
                int fi = data.IndexOfFaction(q.factionPresences[i].factionId);
                string name = fi >= 0 ? data.factions[fi].name : "?";
                string value = q.knowledgeLevel >= 2 ? q.factionPresences[i].value.ToString("0") + " %" : "présente";
                sb.AppendLine().Append("· ").Append(name).Append(" : ").Append(value);
            }
            return sb.ToString();
        }

        /// <summary>Potentiel, maintien, investissement.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string Economy(QuarterData q, GameData data, GameConfig c)
        {
            return "Potentiel du quartier : " + UiText.Money(EconomyRules.Potential(q, c)) + "/cycle\n"
                 + "Maintien si tenu : " + UiText.Money(EconomyRules.QuarterMaintenance(q, 0f, c) > 0f ? EconomyRules.QuarterMaintenance(q, 0f, c) : q.maintenanceCost) + "/cycle\n"
                 + "Investissement : niveau " + q.investmentLevel;
        }
    }
}

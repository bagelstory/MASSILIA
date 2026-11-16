// MARSEILLE — UI/QuarterDetailPopup.Actions.cs
// Partie « actions » du détail de quartier : liste des charbons (ouvrir, reprendre, fermer) et
// investissement. Les règles sont dans EconomyActions ; ici on affiche et on appelle UIManager.Perform.

using Marseille.Art;
using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    public partial class QuarterDetailPopup
    {
        /// <summary>Cartes des charbons du quartier (détail visible à partir de la connaissance 2 ou si tenu).</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddCharbons(QuarterData q, GameData data, GameConfig c)
        {
            UiKit.Label(Body, "Charbons", c.uiFontSizes.y, c.uiText);
            bool detailed = q.knowledgeLevel >= 2 || q.control > 0f;
            if (!detailed) { Paragraph(q.pois.Count + " point(s) repéré(s). Connaissance 2 pour le détail.", 1, c.uiTextDim); return; }
            if (q.pois.Count == 0) { Paragraph("Aucun.", 1, c.uiTextDim); return; }
            for (int p = 0; p < q.pois.Count; p++)
            {
                PoiData poi = q.pois[p];
                if (poi.kind != PoiKind.Charbon) continue;
                UiCard card = UiCard.Create(Body, 2, c);
                card.SetIcon(ArtLibrary.Symbol("charbon"), c.uiFontSizes.z * 2.4f);
                card.Text.text = poi.name + " · " + UiText.Money(poi.revenue) + "/cycle · risque " + poi.risk + "/3\n" + OwnerText(poi, data);
                int qid = q.id, pid = poi.id;
                if (poi.ownerId == Owners.Player)
                    card.AddButton(poi.isOpen ? "Fermer" : "Rouvrir", c.uiButton, () => Act((ref GameData d, GameConfig cfg) => EconomyActions.ToggleCharbon(ref d, qid, pid)));
                else
                {
                    ActionResult check = EconomyActions.CanTakeCharbon(data, qid, pid, c);
                    string verb = Owners.IsFaction(poi.ownerId) ? "Reprendre" : "Ouvrir";
                    string label = check.ok ? verb + " (" + UiText.Money(EconomyActions.CharbonCost(poi, c)) + ")" : check.message;
                    card.AddButton(label, check.ok ? c.uiAccent : c.uiButton, () => Act((ref GameData d, GameConfig cfg) => EconomyActions.TakeCharbon(ref d, qid, pid, cfg))).interactable = check.ok;
                }
            }
        }

        /// <summary>Bouton d'investissement si le quartier est tenu.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddInvest(QuarterData q, GameData data, GameConfig c)
        {
            if (q.control <= 0f) return;
            float cost = EconomyActions.InvestCost(q, c);
            UiCard card = UiCard.Create(Body, 2, c);
            card.Text.text = "Investir : revenus +" + (c.investIncomeGain * 100f).ToString("0") + " %, maintien +"
                             + (c.investMaintenanceGain * 100f).ToString("0") + " % par niveau.";
            int qid = q.id;
            bool affordable = data.indicators.treasury >= cost;
            card.AddButton("Investir (" + UiText.Money(cost) + ")", affordable ? c.uiAccent : c.uiButton,
                           () => Act((ref GameData d, GameConfig cfg) => EconomyActions.Invest(ref d, qid, cfg))).interactable = affordable;
        }

        /// <summary>Propriétaire et état d'un charbon.</summary>
        /// <param name="poi">Charbon.</param>
        /// <param name="data">Partie.</param>
        /// <returns>Texte.</returns>
        private static string OwnerText(PoiData poi, GameData data)
        {
            if (poi.seizedCycles > 0) return "Fermé par la police (" + poi.seizedCycles + " cycle(s))";
            if (poi.ownerId == Owners.Player) return poi.isOpen ? "À toi · ouvert" : "À toi · fermé";
            if (Owners.IsFaction(poi.ownerId))
            {
                int fi = data.IndexOfFaction(poi.ownerId);
                return "Tenu par " + (fi >= 0 ? data.factions[fi].name : "?");
            }
            return "Libre";
        }

        /// <summary>Exécute une action sur ce quartier.</summary>
        /// <param name="action">Action.</param>
        private void Act(GameAction action)
        {
            if (Manager != null) Manager.Perform(action, quarterId);
        }
    }
}

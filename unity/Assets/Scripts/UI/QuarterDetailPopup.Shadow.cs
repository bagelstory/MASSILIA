// MARSEILLE — UI/QuarterDetailPopup.Shadow.cs
// Partie « ombre » du détail de quartier : ce que le quartier a perdu (peur, habitants, rideaux baissés)
// et les trois gestes noirs : donner l'ordre, l'ordre du parloir, placer l'argent.
// Chaque bouton dit d'abord ce que ça coûte. Les règles sont dans ShadowActions.

using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    public partial class QuarterDetailPopup
    {
        /// <summary>État du quartier et gestes de l'ombre.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddShadow(QuarterData q, GameData data, GameConfig c)
        {
            UiKit.Label(Body, "Ce que le quartier a perdu", c.uiFontSizes.y, c.uiText);
            Paragraph(LossText(q), 2, c.uiTextDim);
            AddOrder(q, data, c);
            AddInject(q, data, c);
        }

        /// <summary>Peur, habitants restants, argent sale, rideaux baissés.</summary>
        /// <param name="q">Quartier.</param>
        /// <returns>Texte sur deux lignes.</returns>
        private static string LossText(QuarterData q)
        {
            string people = q.population >= 99.5f ? "Tout le monde est encore là." : (100f - q.population).ToString("0") + " % des habitants sont partis.";
            string shops = q.closedShops > 0 ? "  ·  " + q.closedShops + " rideau(x) baissé(s)" : "";
            return "Peur " + q.fear.ToString("0") + "  ·  argent sale " + q.contamination.ToString("0") + shops + "\n" + people;
        }

        /// <summary>Donner l'ordre contre la bande la plus présente, dehors ou depuis le parloir.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddOrder(QuarterData q, GameData data, GameConfig c)
        {
            int factionId = ShadowRules.TargetFaction(q);
            if (factionId == Owners.None) return;
            int fi = data.IndexOfFaction(factionId);
            string band = fi >= 0 ? data.factions[fi].name : "la bande";
            int debt = data.shadow.DebtOf(factionId);
            UiCard card = UiCard.Create(Body, 3, c);
            card.Text.text = "L'ordre contre " + band + ". On ne voit jamais ce qui se passe.\nDette de sang : " + debt
                             + ". Elle ne revient jamais à zéro, et elle finit toujours par revenir.";
            ActionResult check = ShadowActions.CanGiveOrder(data, q.id, c);
            int qid = q.id;
            string label = check.ok ? "Donner l'ordre (" + UiText.Money(c.orderCost) + ")" : check.message;
            card.AddButton(label, check.ok ? c.uiNegative : c.uiButton,
                           () => Act((ref GameData d, GameConfig cfg) => ShadowActions.GiveOrder(ref d, qid, cfg))).interactable = check.ok;
            int pi = ShadowActions.BestPrisoner(data, c);
            if (pi < 0) return;
            OperatorData prisoner = data.operators[pi];
            float reliability = ShadowRules.Reliability(prisoner, c) * 100f;
            string parloir = check.ok ? "Par le parloir · " + ShadowRules.Name(prisoner) + " · " + reliability.ToString("0") + " % · " + c.parloirDelay + " sem." : check.message;
            card.AddButton(parloir, c.uiButton,
                           () => Act((ref GameData d, GameConfig cfg) => ShadowActions.GiveOrderFromPrison(ref d, qid, cfg))).interactable = check.ok;
        }

        /// <summary>Placer l'argent dans les commerces du quartier.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private void AddInject(QuarterData q, GameData data, GameConfig c)
        {
            if (q.control <= 0f) return;
            UiCard card = UiCard.Create(Body, 2, c);
            card.Text.text = "Placer l'argent : exposition −" + c.injectExposureRelief.ToString("0")
                             + ". Au-dessus de " + c.contaminationThreshold.ToString("0") + " d'argent sale, les commerces de toujours ferment et les familles partent.";
            ActionResult check = ShadowActions.CanInject(data, q.id, c);
            int qid = q.id;
            string label = check.ok ? "Placer " + UiText.Money(c.injectAmount) : check.message;
            card.AddButton(label, c.uiButton,
                           () => Act((ref GameData d, GameConfig cfg) => ShadowActions.Inject(ref d, qid, cfg))).interactable = check.ok;
        }
    }
}

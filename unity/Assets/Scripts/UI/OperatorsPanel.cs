// MARSEILLE — UI/OperatorsPanel.cs
// Écran « Équipe » (spec §4) : opérateurs actifs (état, tâche, fatigue) avec raccourcis, recrutement
// limité par la capacité, liste des opérateurs perdus.

using Marseille.Art;
using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    /// <summary>Écran des opérateurs.</summary>
    public class OperatorsPanel : ListPanel
    {
        /// <inheritdoc />
        public override string TabLabel { get { return "Équipe"; } }

        /// <inheritdoc />
        public override string TabIcon { get { return "tab_equipe"; } }

        /// <summary>Remplit la liste des opérateurs et des recrues possibles.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        protected override void Fill(GameData data, GameConfig config)
        {
            Title("Opérateurs " + OperatorRules.ActiveCount(data) + "/" + data.capacity, config);
            string lost = string.Empty;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.status == OperatorStatus.Lost) { lost += (lost.Length > 0 ? ", " : "") + UiText.OperatorName(op); continue; }
                AddOperatorCard(op, data, config);
            }
            Title("Recruter", config);
            for (int i = 0; i < config.operatorTypes.Count; i++) AddRecruitCard(config.operatorTypes[i], data, config);
            if (lost.Length > 0) UiKit.Label(Column, "Perdus : " + lost, config.uiFontSizes.z, config.uiTextDim);
        }

        /// <summary>Carte d'un opérateur actif.</summary>
        /// <param name="op">Opérateur.</param>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void AddOperatorCard(OperatorData op, GameData data, GameConfig config)
        {
            UiCard card = UiCard.Create(Column, 3, config);
            card.SetIcon(ArtLibrary.Portrait(op) ?? ArtLibrary.TypeIcon(op.type), config.uiFontSizes.z * 3.4f);
            string where = op.status == OperatorStatus.InTransit
                ? "vers " + UiText.QuarterName(data, op.destinationQuarter) + " (" + op.transitCycles + " cycle(s))"
                : UiText.QuarterName(data, op.currentQuarter);
            card.Text.text = UiText.OperatorName(op) + " · " + UiText.Type(op.type, config) + " · compétence " + op.competence + "/5\n"
                             + UiText.OperatorState(op.status) + " · " + UiText.Task(op.GetTask(), config) + " · " + where + "\n"
                             + "Fatigue " + op.fatigue.ToString("0") + " · entretien " + UiText.Money(op.upkeepCost);
            int id = op.id;
            bool canAct = op.status != OperatorStatus.Unavailable && op.status != OperatorStatus.Jailed;
            card.AddButton("Affecter", config.uiAccent, () => BeginAssign(id)).interactable = canAct;
            card.AddButton("Repos", config.uiButton, () => Rest(id)).interactable = canAct;
            card.AddButton("Détails", config.uiButton, () => { if (Manager != null) Manager.RequestOperatorDetail(id); });
        }

        /// <summary>Carte de recrutement d'un type.</summary>
        /// <param name="s">Type.</param>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void AddRecruitCard(OperatorTypeSettings s, GameData data, GameConfig config)
        {
            UiCard card = UiCard.Create(Column, 2, config);
            card.SetIcon(ArtLibrary.TypeIcon(s.type), config.uiFontSizes.z * 2.4f);
            card.Text.text = s.displayName + " · spécialité : " + UiText.Task(s.specialtyTask, config) + "\n"
                             + "Recrutement " + UiText.Money(s.recruitCost) + " · entretien " + UiText.Money(s.upkeep) + "/cycle";
            OperatorType type = s.type;
            ActionResult check = OperatorRules.CanRecruit(data, type, config);
            card.AddButton(check.ok ? "Recruter" : check.message, check.ok ? config.uiAccent : config.uiButton, () => Recruit(type)).interactable = check.ok;
        }

        /// <summary>Va sur la carte pour affecter l'opérateur.</summary>
        /// <param name="operatorId">Opérateur.</param>
        private void BeginAssign(int operatorId)
        {
            if (Manager == null) return;
            MapPanel map = Manager.ShowPanel<MapPanel>();
            if (map != null) map.BeginAssign(operatorId);
        }

        /// <summary>Met l'opérateur au repos sur place.</summary>
        /// <param name="operatorId">Opérateur.</param>
        private void Rest(int operatorId)
        {
            if (Manager == null) return;
            Manager.Perform((ref GameData d, GameConfig c) =>
            {
                int i = d.IndexOfOperator(operatorId);
                return i < 0 ? ActionResult.Fail("Opérateur inconnu.") : OperatorRules.Assign(ref d, operatorId, d.operators[i].currentQuarter, OperatorTask.Rest, c);
            }, Owners.None);
        }

        /// <summary>Recrute un opérateur ; sa compétence est tirée dans la plage de la configuration.</summary>
        /// <param name="type">Type.</param>
        private void Recruit(OperatorType type)
        {
            if (Manager == null) return;
            Manager.Perform((ref GameData d, GameConfig c) =>
            {
                System.Random rng = new System.Random(unchecked(d.randomSeed * 13 + d.nextOperatorId * 7 + d.currentCycle));
                int competence = rng.Next(c.recruitCompetence.x, c.recruitCompetence.y + 1);
                return OperatorRules.Recruit(ref d, type, competence, c);
            }, Owners.None);
        }
    }
}

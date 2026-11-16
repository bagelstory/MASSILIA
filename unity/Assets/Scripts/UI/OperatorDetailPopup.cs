// MARSEILLE — UI/OperatorDetailPopup.cs
// Fenêtre « détail d'un opérateur » : état, compétence, expérience, fatigue, position, tâche,
// efficacité sur chaque tâche (spécialité, fatigue), raccourcis Affecter et Repos.

using System.Text;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    /// <summary>Détail d'un opérateur.</summary>
    public class OperatorDetailPopup : UIOverlay
    {
        private static readonly OperatorTask[] Tasks =
        {
            OperatorTask.Secure, OperatorTask.Press, OperatorTask.Scout, OperatorTask.Watch, OperatorTask.ManageFlux, OperatorTask.Optimize
        };

        private int operatorId = Owners.None;

        /// <summary>S'abonne aux demandes d'ouverture.</summary>
        protected override void OnInitialized()
        {
            if (Manager != null) Manager.OperatorDetailRequested += Show;
        }

        /// <summary>Désabonnement.</summary>
        private void OnDestroy()
        {
            if (Manager != null) Manager.OperatorDetailRequested -= Show;
        }

        /// <summary>Ouvre la fenêtre pour un opérateur.</summary>
        /// <param name="id">Opérateur.</param>
        public void Show(int id)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame) return;
            GameData data = gm.Data;
            GameConfig c = gm.Config;
            int oi = data.IndexOfOperator(id);
            if (oi < 0) return;
            operatorId = id;
            Open();
            ClearContent();
            OperatorData op = data.operators[oi];
            UiKit.Label(Body, UiText.OperatorName(op), c.uiFontSizes.x, c.uiText);
            Paragraph(Profile(op, data, c), 5, c.uiText);
            UiKit.Label(Body, "Efficacité par tâche", c.uiFontSizes.y, c.uiText);
            Paragraph(Efficiencies(op, c), Tasks.Length, c.uiText);
            bool canAct = op.status != OperatorStatus.Lost && op.status != OperatorStatus.Unavailable;
            FooterButton("Affecter", c.uiAccent, Assign).interactable = canAct;
            FooterButton("Repos", c.uiButton, Rest).interactable = canAct;
            FooterButton("Fermer", c.uiButton, Close);
        }

        /// <summary>Type, état, compétence, fatigue, position, tâche, entretien.</summary>
        /// <param name="op">Opérateur.</param>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string Profile(OperatorData op, GameData data, GameConfig c)
        {
            OperatorTypeSettings s = c.GetOperatorSettings(op.type);
            StringBuilder sb = new StringBuilder();
            sb.Append(UiText.Type(op.type, c)).Append(" · spécialité ").Append(s != null ? UiText.Task(s.specialtyTask, c) : "?").AppendLine();
            sb.Append("Compétence ").Append(op.competence).Append("/5 · expérience ").Append((op.experience * 100f).ToString("0")).Append(" %").AppendLine();
            sb.Append(UiText.OperatorState(op.status)).Append(" · fatigue ").Append(op.fatigue.ToString("0"));
            if (op.fatigue >= c.fatigueSlowdownThreshold) sb.Append(" (ralenti)");
            sb.AppendLine();
            sb.Append("Position : ").Append(UiText.QuarterName(data, op.currentQuarter));
            if (op.status == OperatorStatus.InTransit) sb.Append(" → ").Append(UiText.QuarterName(data, op.destinationQuarter));
            sb.Append(" · tâche ").Append(UiText.Task(op.GetTask(), c)).AppendLine();
            sb.Append("Entretien ").Append(UiText.Money(op.upkeepCost)).Append("/cycle");
            return sb.ToString();
        }

        /// <summary>Efficacité actuelle sur chaque tâche, en pourcentage de la référence.</summary>
        /// <param name="op">Opérateur.</param>
        /// <param name="c">Configuration.</param>
        /// <returns>Texte.</returns>
        private static string Efficiencies(OperatorData op, GameConfig c)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Tasks.Length; i++)
            {
                if (i > 0) sb.AppendLine();
                sb.Append(UiText.Task(Tasks[i], c)).Append(" : ").Append((OperatorRules.Efficiency(op, Tasks[i], c) * 100f).ToString("0")).Append(" %");
            }
            return sb.ToString();
        }

        /// <summary>Ferme et commence l'affectation sur la carte.</summary>
        private void Assign()
        {
            Close();
            if (Manager == null) return;
            MapPanel map = Manager.ShowPanel<MapPanel>();
            if (map != null) map.BeginAssign(operatorId);
        }

        /// <summary>Met l'opérateur au repos sur place.</summary>
        private void Rest()
        {
            int id = operatorId;
            Close();
            if (Manager == null) return;
            Manager.Perform((ref GameData d, GameConfig c) =>
            {
                int i = d.IndexOfOperator(id);
                return i < 0 ? ActionResult.Fail("Opérateur inconnu.") : OperatorRules.Assign(ref d, id, d.operators[i].currentQuarter, OperatorTask.Rest, c);
            }, Owners.None);
        }
    }
}

// MARSEILLE — UI/MapPanel.Assign.cs
// Partie « affectation » de MapPanel (spec §4.5) : un opérateur choisi, une tâche (sa spécialité par
// défaut), puis un quartier entouré ; l'affectation passe par OperatorRules.Assign via UIManager.Perform.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    public partial class MapPanel
    {
        private static readonly OperatorTask[] TaskOrder =
        {
            OperatorTask.Secure, OperatorTask.Press, OperatorTask.Scout, OperatorTask.Watch,
            OperatorTask.ManageFlux, OperatorTask.Optimize, OperatorTask.Rest
        };

        private readonly Dictionary<OperatorTask, Button> taskButtons = new Dictionary<OperatorTask, Button>();
        private int pendingOperator = Owners.None;
        private OperatorTask pendingTask = OperatorTask.Rest;

        /// <summary>Crée les boutons de tâche (deux lignes), masqués tant qu'aucun opérateur n'est choisi.</summary>
        /// <param name="parent">Colonne parente.</param>
        /// <param name="config">Configuration.</param>
        private void BuildTaskButtons(RectTransform parent, GameConfig config)
        {
            RectTransform row = null;
            for (int i = 0; i < TaskOrder.Length; i++)
            {
                if (i % 4 == 0) row = UiKit.Row("TaskRow", parent, config.uiButtonHeight * 0.8f, config);
                OperatorTask task = TaskOrder[i];
                Button b = UiKit.MakeButton(row, UiText.Task(task, config), config.uiButton, () => ChooseTask(task), config);
                b.GetComponentInChildren<Text>().fontSize = config.uiFontSizes.z;
                taskButtons[task] = b;
            }
            parent.gameObject.SetActive(false);
        }

        /// <summary>Commence une affectation depuis un autre écran (liste des opérateurs).</summary>
        /// <param name="operatorId">Opérateur.</param>
        public void BeginAssign(int operatorId)
        {
            if (map != null) map.HandleOperatorClicked(operatorId);
        }

        /// <summary>Opérateur touché : il devient l'opérateur à affecter, avec sa tâche de spécialité.</summary>
        /// <param name="operatorId">Opérateur.</param>
        private void OnOperatorSelected(int operatorId)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame) return;
            GameData data = gm.Data;
            int i = data.IndexOfOperator(operatorId);
            if (i < 0) return;
            pendingOperator = operatorId;
            OperatorTypeSettings s = gm.Config.GetOperatorSettings(data.operators[i].type);
            pendingTask = s != null ? s.specialtyTask : OperatorTask.Rest;
            HighlightDestinations();
            RefreshSheet(data, gm.Config);
        }

        /// <summary>Quartier touché : destination si un opérateur attend, sinon simple consultation.</summary>
        /// <param name="quarterId">Quartier.</param>
        private void OnQuarterSelected(int quarterId)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame) return;
            if (pendingOperator != Owners.None && Manager != null)
            {
                int opId = pendingOperator;
                OperatorTask task = pendingTask;
                ActionResult r = Manager.Perform((ref GameData d, GameConfig c) => OperatorRules.Assign(ref d, opId, quarterId, task, c), quarterId);
                if (r.ok) EndAssign();
            }
            RefreshSheet(gm.Data, gm.Config);
        }

        /// <summary>Change la tâche prévue et recalcule les destinations.</summary>
        /// <param name="task">Tâche.</param>
        private void ChooseTask(OperatorTask task)
        {
            if (pendingOperator == Owners.None) return;
            pendingTask = task;
            HighlightDestinations();
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.HasGame) RefreshSheet(gm.Data, gm.Config);
        }

        /// <summary>Entoure les quartiers où l'opérateur peut aller pour la tâche prévue.</summary>
        private void HighlightDestinations()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame || map == null) return;
            GameData data = gm.Data;
            int oi = data.IndexOfOperator(pendingOperator);
            List<int> ids = new List<int>();
            for (int i = 0; i < data.quarters.Count && oi >= 0; i++)
            {
                int id = data.quarters[i].id;
                if (id == data.operators[oi].currentQuarter || TerritoryRules.IsAccessible(data, id, pendingTask, gm.Config)) ids.Add(id);
            }
            map.SetHighlights(ids);
        }

        /// <summary>Ligne de l'opérateur en cours d'affectation et état des boutons de tâche.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void RefreshOperatorLine(GameData data, GameConfig config)
        {
            int oi = data.IndexOfOperator(pendingOperator);
            if (taskRows != null) taskRows.gameObject.SetActive(oi >= 0);
            if (oi < 0) { operatorInfo.text = string.Empty; return; }
            OperatorData op = data.operators[oi];
            operatorInfo.text = UiText.OperatorName(op) + " · " + UiText.Type(op.type, config) + " · " + UiText.OperatorState(op.status)
                                + " · fatigue " + op.fatigue.ToString("0") + "\nTâche : " + UiText.Task(pendingTask, config) + ". Touche un quartier entouré.";
            foreach (KeyValuePair<OperatorTask, Button> kv in taskButtons)
                kv.Value.targetGraphic.color = kv.Key == pendingTask ? config.uiAccent : config.uiButton;
        }

        /// <summary>Termine l'affectation en cours.</summary>
        private void EndAssign()
        {
            pendingOperator = Owners.None;
            if (map != null) map.ClearSelection();
        }

        /// <summary>Annule l'affectation et la sélection.</summary>
        private void CancelAll()
        {
            EndAssign();
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.HasGame && quarterInfo != null) RefreshSheet(gm.Data, gm.Config);
        }
    }
}

// MARSEILLE — Map/MapController.Selection.cs
// Partie « pions et sélection » de MapController : placement des opérateurs, clics, mise en évidence.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Map
{
    public partial class MapController
    {
        /// <summary>Crée les pions manquants et place chaque opérateur sur sa case (en rangée s'ils sont plusieurs).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void RefreshOperators(GameData data, GameConfig config)
        {
            Dictionary<int, int> countPerQuarter = new Dictionary<int, int>();
            Vector2 nodeSize = MapLayout.NodeSize(config);
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                OperatorNode node;
                if (!operatorNodes.TryGetValue(op.id, out node))
                {
                    node = operatorPrefab != null ? Instantiate(operatorPrefab, operatorLayer, false)
                                                  : OperatorNode.CreateDefault(operatorLayer, font, operatorSprite, config);
                    node.Bind(op.id, this);
                    operatorNodes[op.id] = node;
                }
                int anchorQuarter = op.status == OperatorStatus.InTransit ? op.destinationQuarter : op.currentQuarter;
                int slot;
                countPerQuarter.TryGetValue(anchorQuarter, out slot);
                countPerQuarter[anchorQuarter] = slot + 1;
                node.Refresh(op, OperatorPosition(op, slot, nodeSize, config), config, op.id == SelectedOperatorId);
            }
        }

        /// <summary>Position d'un pion : coin bas de sa case, décalé selon son rang ; à mi-chemin s'il voyage.</summary>
        /// <param name="op">Opérateur.</param>
        /// <param name="slot">Rang parmi les pions du même quartier.</param>
        /// <param name="nodeSize">Taille d'une case.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Position dans le plan.</returns>
        private Vector2 OperatorPosition(OperatorData op, int slot, Vector2 nodeSize, GameConfig config)
        {
            Vector2 here, there;
            positions.TryGetValue(op.currentQuarter, out here);
            float step = config.mapOperatorSize * 0.8f;
            int perRow = Mathf.Max(1, Mathf.FloorToInt(nodeSize.x / step));
            Vector2 offset = new Vector2(-nodeSize.x * 0.5f + step * 0.5f + (slot % perRow) * step,
                                         -nodeSize.y * 0.25f - (slot / perRow) * step);
            if (op.status == OperatorStatus.InTransit && positions.TryGetValue(op.destinationQuarter, out there))
                return Vector2.Lerp(here, there, 0.5f) + offset;
            return here + offset;
        }

        /// <summary>Clic sur une case : la sélectionne et prévient les écrans.</summary>
        /// <param name="quarterId">Quartier.</param>
        public void HandleQuarterClicked(int quarterId)
        {
            SelectedQuarterId = quarterId;
            Refresh();
            if (QuarterSelected != null) QuarterSelected(quarterId);
        }

        /// <summary>Clic sur un pion : le sélectionne et prévient les écrans.</summary>
        /// <param name="operatorId">Opérateur.</param>
        public void HandleOperatorClicked(int operatorId)
        {
            SelectedOperatorId = operatorId;
            Refresh();
            if (OperatorSelected != null) OperatorSelected(operatorId);
        }

        /// <summary>Retire toute sélection et toute mise en évidence.</summary>
        public void ClearSelection()
        {
            SelectedQuarterId = Owners.None;
            SelectedOperatorId = Owners.None;
            highlighted.Clear();
            Refresh();
        }

        /// <summary>Met en évidence un ensemble de quartiers (ex. destinations possibles).</summary>
        /// <param name="quarterIds">Quartiers à entourer.</param>
        public void SetHighlights(IEnumerable<int> quarterIds)
        {
            highlighted.Clear();
            if (quarterIds != null) foreach (int id in quarterIds) highlighted.Add(id);
            Refresh();
        }

        /// <summary>Centre la vue sur un quartier.</summary>
        /// <param name="quarterId">Quartier.</param>
        public void FocusQuarter(int quarterId)
        {
            Vector2 p;
            if (viewport != null && positions.TryGetValue(quarterId, out p)) viewport.CenterOn(p);
        }

        /// <summary>Position d'un quartier dans le plan (pour les fenêtres qui s'y accrochent).</summary>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="position">Position trouvée.</param>
        /// <returns>Vrai si le quartier est sur la carte.</returns>
        public bool TryGetQuarterPosition(int quarterId, out Vector2 position)
        {
            return positions.TryGetValue(quarterId, out position);
        }
    }
}

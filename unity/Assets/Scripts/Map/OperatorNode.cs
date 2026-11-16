// MARSEILLE — Map/OperatorNode.cs
// Pion d'opérateur sur la carte : couleur de son type, initiale, position sur son quartier
// (à mi-chemin et transparent s'il est en trajet). Mis à jour par MapController.

using Marseille.Config;
using Marseille.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marseille.Map
{
    /// <summary>Vue d'un opérateur.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class OperatorNode : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image body;
        [SerializeField] private Text letter;
        [SerializeField] private Outline outline;

        private MapController owner;

        /// <summary>Opérateur représenté.</summary>
        public int OperatorId { get; private set; }

        /// <summary>RectTransform du pion.</summary>
        public RectTransform Rect { get { return (RectTransform)transform; } }

        /// <summary>Crée un pion par code (sans prefab).</summary>
        /// <param name="parent">Parent (contenu de la carte).</param>
        /// <param name="font">Police.</param>
        /// <param name="sprite">Forme du pion (rond), ou null pour un carré.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le pion.</returns>
        public static OperatorNode CreateDefault(Transform parent, Font font, Sprite sprite, GameConfig config)
        {
            Image img = UiFactory.CreateImage("Operator", parent, Color.white, true);
            img.sprite = sprite;
            OperatorNode node = img.gameObject.AddComponent<OperatorNode>();
            node.body = img;
            node.Rect.sizeDelta = Vector2.one * config.mapOperatorSize;
            node.letter = UiFactory.CreateText("Letter", img.transform, font, Mathf.RoundToInt(config.mapOperatorSize * 0.55f), Color.black);
            node.letter.fontStyle = FontStyle.Bold;
            UiFactory.Stretch(node.letter.rectTransform, Vector2.zero, Vector2.one);
            node.outline = img.gameObject.AddComponent<Outline>();
            node.outline.enabled = false;
            return node;
        }

        /// <summary>Lie le pion à son opérateur et à la carte.</summary>
        /// <param name="operatorId">Opérateur.</param>
        /// <param name="map">Carte propriétaire.</param>
        public void Bind(int operatorId, MapController map)
        {
            OperatorId = operatorId;
            owner = map;
            gameObject.name = "Operator " + operatorId;
        }

        /// <summary>Met à jour couleur, initiale, position et état.</summary>
        /// <param name="op">Opérateur.</param>
        /// <param name="position">Position calculée par la carte.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="selected">Pion sélectionné.</param>
        public void Refresh(OperatorData op, Vector2 position, GameConfig config, bool selected)
        {
            bool lost = op.status == OperatorStatus.Lost;
            if (gameObject.activeSelf == lost) gameObject.SetActive(!lost);
            if (lost) return;
            OperatorTypeSettings s = config.GetOperatorSettings(op.type);
            Color c = MapStyle.OperatorColor(op.type, config);
            if (op.status == OperatorStatus.InTransit) c.a = 0.55f;
            if (op.status == OperatorStatus.Unavailable) c = MapStyle.Dim(c, 0.5f);
            if (body != null) body.color = c;
            if (letter != null) letter.text = s != null ? s.letter : "?";
            if (outline != null)
            {
                outline.enabled = selected;
                outline.effectColor = config.mapColorSelection;
                outline.effectDistance = new Vector2(config.mapOutlineWidth, -config.mapOutlineWidth);
            }
            Rect.anchoredPosition = position;
        }

        /// <summary>Clic sur le pion (ignoré à la fin d'un glissement).</summary>
        /// <param name="eventData">Données du clic.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging || owner == null) return;
            owner.HandleOperatorClicked(OperatorId);
        }
    }
}

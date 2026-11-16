// MARSEILLE — Map/QuarterNode.cs
// Une case de quartier sur la carte : couleur de statut, barre de contrôle, nom (selon le zoom),
// pastille (charbons du joueur, ou « ! » si une menace est repérée), contour de sélection.
// Ne fait rien à chaque image : il est mis à jour par MapController quand les données changent.

using Marseille.Art;
using Marseille.Config;
using Marseille.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marseille.Map
{
    /// <summary>Vue d'un quartier.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class QuarterNode : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image controlBar;
        [SerializeField] private Text label;
        [SerializeField] private Text badge;
        [SerializeField] private Outline outline;

        private MapController owner;
        private string displayName = string.Empty;

        /// <summary>Quartier représenté.</summary>
        public int QuarterId { get; private set; }

        /// <summary>RectTransform de la case.</summary>
        public RectTransform Rect { get { return (RectTransform)transform; } }

        /// <summary>Crée une case par code (sans prefab).</summary>
        /// <param name="parent">Parent (contenu de la carte).</param>
        /// <param name="font">Police.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>La case.</returns>
        public static QuarterNode CreateDefault(Transform parent, Font font, GameConfig config)
        {
            Image bg = UiFactory.CreateImage("Quarter", parent, config.mapColorFree, true);
            Sprite tile = ArtLibrary.Shape("tile");
            if (tile != null) { bg.sprite = tile; bg.type = Image.Type.Sliced; bg.pixelsPerUnitMultiplier = 4f; }
            QuarterNode node = bg.gameObject.AddComponent<QuarterNode>();
            node.background = bg;
            node.controlBar = UiFactory.CreateImage("Control", bg.transform, config.mapColorPlayer, false);
            node.label = UiFactory.CreateText("Label", bg.transform, font, config.mapLabelFontSize, config.mapColorText);
            UiFactory.Stretch(node.label.rectTransform, new Vector2(0.04f, 0.15f), new Vector2(0.96f, 0.95f));
            node.badge = UiFactory.CreateText("Badge", bg.transform, font, config.mapLabelFontSize + 2, config.mapColorText);
            node.badge.fontStyle = FontStyle.Bold;
            UiFactory.Stretch(node.badge.rectTransform, new Vector2(0.6f, 0.62f), new Vector2(1f, 1f));
            node.outline = bg.gameObject.AddComponent<Outline>();
            node.outline.enabled = false;
            return node;
        }

        /// <summary>Lie la case à son quartier et à la carte.</summary>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="name">Nom affiché.</param>
        /// <param name="map">Carte propriétaire.</param>
        public void Bind(int quarterId, string name, MapController map)
        {
            QuarterId = quarterId;
            displayName = name ?? string.Empty;
            owner = map;
            gameObject.name = "Quarter " + quarterId;
        }

        /// <summary>Met à jour l'affichage d'après les données.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="selected">Case sélectionnée.</param>
        /// <param name="highlighted">Case proposée (ex. destination possible d'un opérateur).</param>
        public void Refresh(QuarterData q, GameData data, GameConfig config, bool selected, bool highlighted)
        {
            bool known = q.knowledgeLevel > 0 || q.control > 0f;
            if (background != null) background.color = MapStyle.QuarterColor(q, data, config);
            if (controlBar != null)
            {
                controlBar.enabled = q.control > 0f;
                controlBar.color = config.mapColorSecured;
                UiFactory.Stretch(controlBar.rectTransform, Vector2.zero, new Vector2(Mathf.Clamp01(q.control / 100f), 0.1f));
            }
            if (label != null) label.text = known ? displayName : "?";
            if (badge != null)
            {
                bool threat = MapStyle.ThreatVisible(q, data);
                int charbons = MapStyle.PlayerCharbons(q);
                badge.text = threat ? "!" : charbons > 0 ? charbons.ToString() : string.Empty;
                badge.color = threat ? config.mapColorThreat : config.mapColorText;
            }
            if (outline != null)
            {
                outline.enabled = selected || highlighted;
                outline.effectColor = selected ? config.mapColorSelection : config.mapColorHighlight;
                outline.effectDistance = new Vector2(config.mapOutlineWidth, -config.mapOutlineWidth);
            }
        }

        /// <summary>Affiche ou masque le nom selon le zoom.</summary>
        /// <param name="visible">Nom visible.</param>
        public void SetLabelVisible(bool visible)
        {
            if (label != null && label.enabled != visible) label.enabled = visible;
        }

        /// <summary>Clic sur la case (ignoré à la fin d'un glissement).</summary>
        /// <param name="eventData">Données du clic.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging || owner == null) return;
            owner.HandleQuarterClicked(QuarterId);
        }
    }
}

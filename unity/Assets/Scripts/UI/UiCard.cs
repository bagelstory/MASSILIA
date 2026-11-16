// MARSEILLE — UI/UiCard.cs
// Carte de liste (fond, texte sur plusieurs lignes, rangée de boutons facultative), utilisée par les
// écrans Opérateurs, Flux, Factions et par les fenêtres de détail.

using Marseille.Config;
using Marseille.Map;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Élément de liste.</summary>
    public class UiCard
    {
        /// <summary>Racine de la carte.</summary>
        public RectTransform Root { get; private set; }

        /// <summary>Texte principal.</summary>
        public Text Text { get; private set; }

        /// <summary>Rangée de boutons (créée à la demande).</summary>
        public RectTransform Buttons { get; private set; }

        private GameConfig config;

        /// <summary>Crée une carte dans une colonne.</summary>
        /// <param name="parent">Colonne.</param>
        /// <param name="lines">Nombre de lignes de texte prévues.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>La carte.</returns>
        public static UiCard Create(Transform parent, int lines, GameConfig config)
        {
            UiCard card = new UiCard { config = config };
            Image bg = UiFactory.CreateImage("Card", parent, config.uiPanel, false);
            card.Root = bg.rectTransform;
            VerticalLayoutGroup v = bg.gameObject.AddComponent<VerticalLayoutGroup>();
            int pad = Mathf.RoundToInt(config.uiPadding * 0.6f);
            v.padding = new RectOffset(pad, pad, pad, pad);
            v.spacing = config.uiSpacing * 0.5f;
            v.childControlWidth = v.childControlHeight = v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            bg.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            card.Text = UiKit.Label(bg.transform, string.Empty, config.uiFontSizes.z, config.uiText);
            card.Text.alignment = TextAnchor.UpperLeft;
            UiKit.SetHeight(card.Text.rectTransform, config.uiFontSizes.z * 1.3f * Mathf.Max(1, lines));
            return card;
        }

        /// <summary>Pose une icône dans le coin haut droit de la carte (sans effet si le sprite manque).</summary>
        /// <param name="sprite">Image.</param>
        /// <param name="size">Côté en pixels.</param>
        public void SetIcon(Sprite sprite, float size)
        {
            if (sprite == null) return;
            Image icon = UiFactory.CreateImage("Icon", Root, Color.white, false);
            icon.sprite = sprite;
            icon.preserveAspect = true;
            icon.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            RectTransform rt = icon.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
            rt.sizeDelta = new Vector2(size, size);
            rt.anchoredPosition = new Vector2(-config.uiPadding * 0.4f, -config.uiPadding * 0.4f);
        }

        /// <summary>Ajoute un bouton dans la rangée du bas de la carte.</summary>
        /// <param name="label">Libellé.</param>
        /// <param name="color">Couleur.</param>
        /// <param name="onClick">Action.</param>
        /// <returns>Le bouton.</returns>
        public Button AddButton(string label, Color color, UnityAction onClick)
        {
            if (Buttons == null) Buttons = UiKit.Row("Buttons", Root, config.uiButtonHeight * 0.8f, config);
            Button b = UiKit.MakeButton(Buttons, label, color, onClick, config);
            b.GetComponentInChildren<Text>().fontSize = config.uiFontSizes.z;
            return b;
        }
    }
}

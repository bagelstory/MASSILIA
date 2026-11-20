// MARSEILLE — UI/UiKit.cs
// Fabriques uGUI pour les écrans : fonds, textes, boutons, lignes et colonnes avec mise en page
// automatique. Tout est construit par code, sans TextMeshPro ni package externe.

using Marseille.Config;
using Marseille.Map;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Fabriques d'éléments d'interface.</summary>
    public static partial class UiKit
    {
        /// <summary>Police partagée : Resources/Fonts/Chouf si elle existe, sinon la police intégrée de Unity.</summary>
        public static Font Font { get { return font != null ? font : (font = Resources.Load<Font>("Fonts/Chouf") ?? MapStyle.BuiltinFont()); } set { font = value; } }
        private static Font font;

        /// <summary>Fond uni étiré sur tout le parent.</summary>
        /// <param name="name">Nom.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="color">Couleur.</param>
        /// <returns>L'image.</returns>
        public static Image Panel(string name, Transform parent, Color color)
        {
            Image img = UiFactory.CreateImage(name, parent, color, true);
            UiFactory.Stretch(img.rectTransform, Vector2.zero, Vector2.one);
            return img;
        }

        /// <summary>Colonne verticale qui s'adapte à son contenu.</summary>
        /// <param name="name">Nom.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le RectTransform de la colonne.</returns>
        public static RectTransform Column(string name, Transform parent, GameConfig config)
        {
            RectTransform rt = UiFactory.CreateRect(name, parent);
            VerticalLayoutGroup v = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            v.spacing = config.uiSpacing;
            v.childControlWidth = true;
            v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            return rt;
        }

        /// <summary>Ligne horizontale de largeurs égales.</summary>
        /// <param name="name">Nom.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="height">Hauteur.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le RectTransform de la ligne.</returns>
        public static RectTransform Row(string name, Transform parent, float height, GameConfig config)
        {
            RectTransform rt = UiFactory.CreateRect(name, parent);
            HorizontalLayoutGroup h = rt.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.spacing = config.uiSpacing;
            h.childControlWidth = true;
            h.childControlHeight = true;
            h.childForceExpandWidth = true;
            h.childForceExpandHeight = true;
            SetHeight(rt, height);
            return rt;
        }

        /// <summary>Texte aligné à gauche, hauteur fixée pour la mise en page.</summary>
        /// <param name="parent">Parent.</param>
        /// <param name="text">Contenu.</param>
        /// <param name="size">Taille de police.</param>
        /// <param name="color">Couleur.</param>
        /// <returns>Le texte.</returns>
        public static Text Label(Transform parent, string text, int size, Color color)
        {
            Text t = UiFactory.CreateText("Label", parent, Font, size, color);
            t.alignment = TextAnchor.MiddleLeft;
            t.text = text;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            SetHeight(t.rectTransform, size * 1.5f);
            return t;
        }

        /// <summary>Bouton avec texte.</summary>
        /// <param name="parent">Parent.</param>
        /// <param name="text">Libellé.</param>
        /// <param name="color">Couleur du fond.</param>
        /// <param name="onClick">Action au clic.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le bouton.</returns>
        public static Button MakeButton(Transform parent, string text, Color color, UnityAction onClick, GameConfig config)
        {
            Image bg = UiFactory.CreateImage("Button", parent, color, true);
            UiSprites.Round(bg);
            Button b = bg.gameObject.AddComponent<Button>();
            b.targetGraphic = bg;
            if (onClick != null) b.onClick.AddListener(onClick);
            Text t = UiFactory.CreateText("Text", bg.transform, Font, config.uiFontSizes.y, config.uiText);
            UiFactory.Stretch(t.rectTransform, Vector2.zero, Vector2.one);
            t.text = text;
            SetHeight(bg.rectTransform, HeightFor(parent, config.uiButtonHeight));
            return b;
        }

        /// <summary>Change le libellé d'un bouton créé par <see cref="MakeButton"/>.</summary>
        /// <param name="button">Bouton.</param>
        /// <param name="text">Nouveau libellé.</param>
        public static void SetButtonText(Button button, string text)
        {
            Text t = button != null ? button.GetComponentInChildren<Text>() : null;
            if (t != null) t.text = text;
        }

        /// <summary>Fixe la hauteur préférée d'un élément pour les groupes de mise en page.</summary>
        /// <param name="rt">Élément.</param>
        /// <param name="height">Hauteur.</param>
        public static void SetHeight(RectTransform rt, float height)
        {
            LayoutElement le = rt.GetComponent<LayoutElement>();
            if (le == null) le = rt.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.minHeight = height;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        }

        /// <summary>Zone défilante verticale ; renvoie la colonne où ajouter le contenu.</summary>
        /// <param name="parent">Parent.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>La colonne de contenu.</returns>
        public static RectTransform ScrollColumn(Transform parent, GameConfig config)
        {
            RectTransform view = UiFactory.CreateRect("Scroll", parent);
            UiFactory.Stretch(view, Vector2.zero, Vector2.one);
            view.gameObject.AddComponent<RectMask2D>();
            Image catcher = view.gameObject.AddComponent<Image>();
            catcher.color = new Color(0f, 0f, 0f, 0f);
            RectTransform col = Column("Content", view, config);
            col.anchorMin = new Vector2(0f, 1f);
            col.anchorMax = new Vector2(1f, 1f);
            col.pivot = new Vector2(0.5f, 1f);
            col.offsetMin = new Vector2(config.uiPadding, 0f);
            col.offsetMax = new Vector2(-config.uiPadding, 0f);
            VerticalLayoutGroup v = col.GetComponent<VerticalLayoutGroup>();
            v.padding = new RectOffset(0, 0, (int)config.uiPadding, (int)config.uiPadding);
            col.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollRect sr = view.gameObject.AddComponent<ScrollRect>();
            sr.content = col;
            sr.horizontal = false;
            sr.movementType = ScrollRect.MovementType.Clamped;
            sr.viewport = view;
            return col;
        }

        /// <summary>Image pleine largeur dans une colonne (ne fait rien si le sprite manque).</summary>
        /// <param name="parent">Colonne.</param>
        /// <param name="sprite">Image.</param>
        /// <param name="height">Hauteur réservée.</param>
        /// <returns>L'image, ou null.</returns>
        public static Image Picture(Transform parent, Sprite sprite, float height)
        {
            if (sprite == null) return null;
            Image img = UiFactory.CreateImage("Picture", parent, Color.white, false);
            img.sprite = sprite;
            img.preserveAspect = true;
            SetHeight(img.rectTransform, height);
            return img;
        }

        /// <summary>Détruit tous les enfants d'un élément (listes reconstruites).</summary>
        /// <param name="parent">Élément.</param>
        public static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--) Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}

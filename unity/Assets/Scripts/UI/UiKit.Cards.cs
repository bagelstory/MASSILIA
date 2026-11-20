// MARSEILLE — UI/UiKit.Cards.cs
// Suite de UiKit : puces, cartes à bande de couleur, hauteur adaptée à la rangée qui contient un bouton.

using Marseille.Config;
using Marseille.Map;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Marseille.UI
{
    public static partial class UiKit
    {
        /// <summary>Hauteur d'un bouton : celle de la rangée horizontale qui le contient, sinon la hauteur par défaut.</summary>
        /// <param name="parent">Parent du bouton.</param>
        /// <param name="fallback">Hauteur par défaut.</param>
        /// <returns>Hauteur.</returns>
        public static float HeightFor(Transform parent, float fallback)
        {
            if (parent == null || parent.GetComponent<HorizontalLayoutGroup>() == null) return fallback;
            LayoutElement le = parent.GetComponent<LayoutElement>();
            return le != null && le.preferredHeight > 0f ? Mathf.Min(fallback, le.preferredHeight) : fallback;
        }

        /// <summary>Puce arrondie (filtre) avec texte centré ; cliquable si une action est donnée.</summary>
        /// <param name="parent">Parent.</param>
        /// <param name="text">Texte.</param>
        /// <param name="back">Fond.</param>
        /// <param name="front">Couleur du texte.</param>
        /// <param name="onClick">Action (facultative).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le fond de la puce.</returns>
        public static Image Chip(Transform parent, string text, Color back, Color front, UnityAction onClick, GameConfig config)
        {
            Image bg = UiFactory.CreateImage("Chip", parent, back, onClick != null);
            UiSprites.Round(bg);
            if (onClick != null)
            {
                Button b = bg.gameObject.AddComponent<Button>();
                b.targetGraphic = bg;
                b.onClick.AddListener(onClick);
            }
            Text t = UiFactory.CreateText("Text", bg.transform, Font, config.uiFontSizes.z, front);
            UiFactory.Stretch(t.rectTransform, Vector2.zero, Vector2.one);
            t.text = text;
            SetHeight(bg.rectTransform, config.uiChipHeight);
            return bg;
        }

        /// <summary>Carte arrondie à bande de couleur à gauche, hauteur calculée par son contenu.</summary>
        /// <param name="parent">Colonne.</param>
        /// <param name="band">Couleur de la bande.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le fond de la carte (colonne verticale).</returns>
        public static Image BandCard(Transform parent, Color band, GameConfig config)
        {
            Image bg = UiFactory.CreateImage("Card", parent, config.uiCard, true);
            UiSprites.Round(bg);
            int pad = Mathf.RoundToInt(config.uiPadding * 0.6f);
            VerticalLayoutGroup v = bg.gameObject.AddComponent<VerticalLayoutGroup>();
            v.padding = new RectOffset(pad + 14, pad, pad, pad);
            v.spacing = config.uiSpacing * 0.4f;
            v.childControlWidth = v.childControlHeight = v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            bg.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Image strip = UiFactory.CreateImage("Band", bg.transform, band, false);
            strip.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            RectTransform rt = strip.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.08f);
            rt.anchorMax = new Vector2(0f, 0.92f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.sizeDelta = new Vector2(8f, 0f);
            rt.anchoredPosition = new Vector2(10f, 0f);
            return bg;
        }

        /// <summary>Texte qui passe à la ligne, dont la hauteur suit le contenu (pas de hauteur fixe).</summary>
        /// <param name="parent">Parent.</param>
        /// <param name="text">Contenu.</param>
        /// <param name="size">Taille.</param>
        /// <param name="color">Couleur.</param>
        /// <returns>Le texte.</returns>
        public static Text Flow(Transform parent, string text, int size, Color color)
        {
            Text t = UiFactory.CreateText("Flow", parent, Font, size, color);
            t.alignment = TextAnchor.UpperLeft;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.text = text;
            return t;
        }
    }
}

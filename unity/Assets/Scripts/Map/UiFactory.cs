// MARSEILLE — Map/UiFactory.cs
// Création d'éléments uGUI par code (Image, Text), pour que la carte fonctionne même sans prefab.
// Utilisé par QuarterNode, OperatorNode et MapController. Pas de TextMeshPro.

using UnityEngine;
using UnityEngine.UI;

namespace Marseille.Map
{
    /// <summary>Petites fabriques d'éléments d'interface.</summary>
    public static class UiFactory
    {
        /// <summary>Crée un objet d'interface vide avec un RectTransform.</summary>
        /// <param name="name">Nom de l'objet.</param>
        /// <param name="parent">Parent.</param>
        /// <returns>Le RectTransform créé.</returns>
        public static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.layer = parent != null ? parent.gameObject.layer : 5;
            RectTransform rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            return rt;
        }

        /// <summary>Crée une Image unie.</summary>
        /// <param name="name">Nom.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="color">Couleur.</param>
        /// <param name="raycast">Reçoit les clics.</param>
        /// <returns>L'image.</returns>
        public static Image CreateImage(string name, Transform parent, Color color, bool raycast)
        {
            RectTransform rt = CreateRect(name, parent);
            Image img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = raycast;
            return img;
        }

        /// <summary>Crée un texte centré, sans réception de clic.</summary>
        /// <param name="name">Nom.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="font">Police.</param>
        /// <param name="size">Taille.</param>
        /// <param name="color">Couleur.</param>
        /// <returns>Le texte.</returns>
        public static Text CreateText(string name, Transform parent, Font font, int size, Color color)
        {
            RectTransform rt = CreateRect(name, parent);
            Text t = rt.gameObject.AddComponent<Text>();
            t.font = font;
            t.fontSize = size;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Truncate;
            t.resizeTextForBestFit = false;
            t.raycastTarget = false;
            return t;
        }

        /// <summary>Étire un RectTransform entre deux ancres (marges nulles).</summary>
        /// <param name="rt">Élément.</param>
        /// <param name="min">Ancre minimale.</param>
        /// <param name="max">Ancre maximale.</param>
        public static void Stretch(RectTransform rt, Vector2 min, Vector2 max)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}

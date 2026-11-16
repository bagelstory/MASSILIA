// MARSEILLE — UI/IndicatorBar.cs
// Ligne d'indicateur : nom, valeur, barre 0-100 colorée (vert, orange, rouge selon le sens).

using Marseille.Config;
using Marseille.Map;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Barre d'indicateur réutilisable.</summary>
    public class IndicatorBar : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text value;
        [SerializeField] private Image fill;

        /// <summary>Crée une barre par code.</summary>
        /// <param name="parent">Parent (colonne).</param>
        /// <param name="name">Nom affiché.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="icon">Icône à gauche (facultative).</param>
        /// <returns>La barre.</returns>
        public static IndicatorBar Create(Transform parent, string name, GameConfig config, Sprite icon = null)
        {
            RectTransform root = UiFactory.CreateRect("Indicator " + name, parent);
            UiKit.SetHeight(root, config.uiFontSizes.y * 2.6f);
            IndicatorBar bar = root.gameObject.AddComponent<IndicatorBar>();
            bar.title = UiFactory.CreateText("Title", root, UiKit.Font, config.uiFontSizes.y, config.uiText);
            bar.title.alignment = TextAnchor.UpperLeft;
            bar.title.text = name;
            float left = icon != null ? 0.12f : 0f;
            if (icon != null)
            {
                Image img = UiFactory.CreateImage("Icon", root, Color.white, false);
                img.sprite = icon;
                img.preserveAspect = true;
                UiFactory.Stretch(img.rectTransform, Vector2.zero, new Vector2(0.1f, 1f));
            }
            UiFactory.Stretch(bar.title.rectTransform, new Vector2(left, 0.4f), new Vector2(0.7f, 1f));
            bar.value = UiFactory.CreateText("Value", root, UiKit.Font, config.uiFontSizes.y, config.uiText);
            bar.value.alignment = TextAnchor.UpperRight;
            UiFactory.Stretch(bar.value.rectTransform, new Vector2(0.5f, 0.4f), new Vector2(1f, 1f));
            Image track = UiFactory.CreateImage("Track", root, config.uiButton, false);
            UiFactory.Stretch(track.rectTransform, new Vector2(left, 0.08f), new Vector2(1f, 0.3f));
            bar.fill = UiFactory.CreateImage("Fill", track.transform, config.uiAccent, false);
            return bar;
        }

        /// <summary>Met à jour la barre.</summary>
        /// <param name="ratio">Remplissage 0-1.</param>
        /// <param name="text">Valeur affichée.</param>
        /// <param name="higherIsWorse">Vrai pour pression et exposition (rouge quand c'est haut).</param>
        /// <param name="config">Configuration.</param>
        public void Set(float ratio, string text, bool higherIsWorse, GameConfig config)
        {
            ratio = Mathf.Clamp01(ratio);
            if (value != null) value.text = text;
            if (fill == null) return;
            UiFactory.Stretch(fill.rectTransform, Vector2.zero, new Vector2(ratio, 1f));
            float danger = higherIsWorse ? ratio : 1f - ratio;
            fill.color = danger > 0.75f ? config.uiNegative : danger > 0.5f ? config.uiWarning : config.uiPositive;
        }
    }
}

// MARSEILLE — UI/UIOverlay.cs
// Base des fenêtres au premier plan : fond assombri qui bloque la carte, fenêtre centrée avec un corps
// défilant et une rangée de boutons en bas. Le contenu est reconstruit à chaque ouverture.

using Marseille.Config;
using Marseille.Core;
using Marseille.Map;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Fenêtre modale.</summary>
    public abstract class UIOverlay : MonoBehaviour
    {
        /// <summary>Corps défilant de la fenêtre.</summary>
        protected RectTransform Body { get; private set; }

        /// <summary>Rangée de boutons du bas.</summary>
        protected RectTransform Footer { get; private set; }

        /// <summary>Gestionnaire d'interface.</summary>
        protected UIManager Manager { get; private set; }

        /// <summary>Configuration.</summary>
        protected GameConfig Config { get { return GameManager.Instance != null ? GameManager.Instance.Config : null; } }

        /// <summary>La fenêtre est-elle ouverte ?</summary>
        public bool IsOpen { get { return gameObject.activeSelf; } }

        /// <summary>Construit la fenêtre (une fois) et la ferme.</summary>
        /// <param name="manager">Gestionnaire.</param>
        /// <param name="config">Configuration.</param>
        public void Initialize(UIManager manager, GameConfig config)
        {
            Manager = manager;
            RectTransform root = (RectTransform)transform;
            UiFactory.Stretch(root, Vector2.zero, Vector2.one);
            Image dim = gameObject.GetComponent<Image>();
            if (dim == null) dim = gameObject.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.75f);
            dim.raycastTarget = true;
            Image window = UiFactory.CreateImage("Window", transform, config.uiBackground, true);
            UiFactory.Stretch(window.rectTransform, new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.94f));
            RectTransform bodyArea = UiFactory.CreateRect("BodyArea", window.transform);
            UiFactory.Stretch(bodyArea, new Vector2(0f, 0f), Vector2.one);
            bodyArea.offsetMin = new Vector2(0f, config.uiButtonHeight + config.uiPadding * 2f);
            Body = UiKit.ScrollColumn(bodyArea, config);
            Footer = UiKit.Row("Footer", window.transform, config.uiButtonHeight, config);
            UiFactory.Stretch(Footer, Vector2.zero, new Vector2(1f, 0f));
            Footer.offsetMin = new Vector2(config.uiPadding, config.uiPadding);
            Footer.offsetMax = new Vector2(-config.uiPadding, config.uiPadding + config.uiButtonHeight);
            OnInitialized();
            gameObject.SetActive(false);
        }

        /// <summary>Abonnements propres à la fenêtre (appelé une fois, la fenêtre étant encore active).</summary>
        protected virtual void OnInitialized() { }

        /// <summary>Ouvre la fenêtre et la place au premier plan.</summary>
        protected void Open()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        /// <summary>Ferme la fenêtre.</summary>
        public void Close()
        {
            gameObject.SetActive(false);
        }

        /// <summary>Vide le corps et les boutons avant de les remplir.</summary>
        protected void ClearContent()
        {
            UiKit.ClearChildren(Body);
            UiKit.ClearChildren(Footer);
        }

        /// <summary>Ajoute un bouton dans la rangée du bas.</summary>
        /// <param name="label">Libellé.</param>
        /// <param name="color">Couleur.</param>
        /// <param name="onClick">Action.</param>
        /// <returns>Le bouton.</returns>
        protected Button FooterButton(string label, Color color, UnityAction onClick)
        {
            return UiKit.MakeButton(Footer, label, color, onClick, Config);
        }

        /// <summary>Ajoute un paragraphe dans le corps.</summary>
        /// <param name="text">Texte.</param>
        /// <param name="lines">Lignes prévues.</param>
        /// <param name="color">Couleur.</param>
        /// <returns>Le texte.</returns>
        protected Text Paragraph(string text, int lines, Color color)
        {
            Text t = UiKit.Label(Body, text, Config.uiFontSizes.z, color);
            t.alignment = TextAnchor.UpperLeft;
            UiKit.SetHeight(t.rectTransform, Config.uiFontSizes.z * 1.3f * Mathf.Max(1, lines));
            return t;
        }
    }
}

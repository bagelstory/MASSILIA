// MARSEILLE — UI/UIManager.Chrome.cs
// Partie « habillage » de UIManager : Canvas portrait, barre du haut (date, trésorerie, territoire,
// pression), zone centrale, barre d'onglets, message court. Construit par code au lancement.

using Marseille.Art;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Map;
using Marseille.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marseille.UI
{
    public partial class UIManager
    {
        private RectTransform contentArea;
        private RectTransform tabBar;
        private Text toast;
        private Image toastBackground;

        /// <summary>Zone centrale où vivent les écrans et les fenêtres.</summary>
        public RectTransform ContentArea { get { return contentArea; } }

        /// <summary>Crée Canvas, EventSystem, barres et zone centrale.</summary>
        /// <param name="config">Configuration.</param>
        private void BuildChrome(GameConfig config)
        {
            EnsureCanvas(config);
            if (FindObjectOfType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Image background = UiKit.Panel("Background", transform, config.uiBackground);
            background.raycastTarget = false;
            Sprite splash = ArtLibrary.Screen("splash");
            if (splash != null) { background.sprite = splash; background.color = new Color(0.28f, 0.28f, 0.3f); background.preserveAspect = false; }
            float top = config.uiBarHeights.x, bottom = config.uiBarHeights.y;

            BuildTopBar(config);

            contentArea = UiFactory.CreateRect("Content", transform);
            UiFactory.Stretch(contentArea, Vector2.zero, Vector2.one);
            contentArea.offsetMin = new Vector2(0f, bottom);
            contentArea.offsetMax = new Vector2(0f, -top);
            ShadowVeil.Create(transform).Fit(bottom, top);

            Image tabs = UiFactory.CreateImage("TabBar", transform, config.uiPanel, true);
            UiFactory.Stretch(tabs.rectTransform, Vector2.zero, new Vector2(1f, 0f));
            tabs.rectTransform.offsetMax = new Vector2(0f, bottom);
            tabBar = tabs.rectTransform;
            HorizontalLayoutGroup h = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.childControlWidth = h.childControlHeight = h.childForceExpandWidth = h.childForceExpandHeight = true;
            h.spacing = 2f;

            toastBackground = UiFactory.CreateImage("Toast", transform, new Color(0.04f, 0.05f, 0.06f, 0.92f), false);
            UiSprites.Round(toastBackground);
            UiFactory.Stretch(toastBackground.rectTransform, new Vector2(0.05f, 0f), new Vector2(0.95f, 0f));
            toastBackground.rectTransform.offsetMin = new Vector2(0f, bottom + 20f);
            toastBackground.rectTransform.offsetMax = new Vector2(0f, bottom + 20f + config.uiButtonHeight * 1.2f);
            toast = UiFactory.CreateText("Text", toastBackground.transform, UiKit.Font, config.uiFontSizes.z, config.uiText);
            UiFactory.Stretch(toast.rectTransform, new Vector2(0.03f, 0f), new Vector2(0.97f, 1f));
            toastBackground.gameObject.SetActive(false);
            toast = toastBackground.GetComponentInChildren<Text>(true);
        }

        /// <summary>Ajoute Canvas, CanvasScaler (portrait) et GraphicRaycaster s'ils manquent.</summary>
        /// <param name="config">Configuration.</param>
        private void EnsureCanvas(GameConfig config)
        {
            if (GetComponent<RectTransform>() == null) gameObject.AddComponent<RectTransform>();
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null) { canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; }
            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = config.uiReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
            if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();
        }

        /// <summary>Crée les écrans par défaut, dans l'ordre des onglets.</summary>
        /// <param name="config">Configuration.</param>
        private void CreateDefaultPanels(GameConfig config)
        {
            panels.Add(CreatePanel<MapPanel>("MapPanel"));
            panels.Add(CreatePanel<DashboardPanel>("DashboardPanel"));
            panels.Add(CreatePanel<OperatorsPanel>("OperatorsPanel"));
            panels.Add(CreatePanel<FluxPanel>("FluxPanel"));
            panels.Add(CreatePanel<FactionsPanel>("FactionsPanel"));
            panels.Add(CreatePanel<JournalPanel>("JournalPanel"));
        }

        /// <summary>Crée un écran vide étiré sur la zone centrale.</summary>
        /// <typeparam name="T">Type d'écran.</typeparam>
        /// <param name="name">Nom de l'objet.</param>
        /// <returns>L'écran.</returns>
        private T CreatePanel<T>(string name) where T : UIPanel
        {
            RectTransform rt = UiFactory.CreateRect(name, contentArea);
            UiFactory.Stretch(rt, Vector2.zero, Vector2.one);
            return rt.gameObject.AddComponent<T>();
        }

        /// <summary>Affiche un message court au-dessus des onglets.</summary>
        /// <param name="message">Texte.</param>
        /// <param name="positive">Vert si vrai, rouge sinon.</param>
        public void ShowToast(string message, bool positive)
        {
            GameConfig config = Config;
            if (toast == null || config == null) return;
            toast.text = message;
            toast.color = positive ? config.uiText : config.uiNegative;
            toastBackground.gameObject.SetActive(true);
            toastHideAt = Time.unscaledTime + config.uiToastSeconds;
        }
    }
}

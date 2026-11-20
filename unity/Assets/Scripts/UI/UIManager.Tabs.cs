// MARSEILLE — UI/UIManager.Tabs.cs
// Barre d'onglets : icône + libellé, onglet actif en jaune avec un liseré en haut.

using System.Collections.Generic;
using Marseille.Art;
using Marseille.Config;
using Marseille.Map;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    public partial class UIManager
    {
        private readonly List<Button> tabButtons = new List<Button>();

        /// <summary>Crée un bouton d'onglet par écran.</summary>
        /// <param name="config">Configuration.</param>
        private void BuildTabs(GameConfig config)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                int index = i;
                Button b = UiKit.MakeButton(tabBar, panels[i].TabLabel, config.uiPanel, () => ShowPanel(index), config);
                UiSprites.Flat(b.GetComponent<Image>());
                Text label = b.GetComponentInChildren<Text>();
                label.fontSize = config.uiFontSizes.z - 4;
                Sprite icon = ArtLibrary.Tab(panels[i].TabIcon);
                if (icon != null)
                {
                    Image img = UiFactory.CreateImage("Icon", b.transform, Color.white, false);
                    img.sprite = icon;
                    img.preserveAspect = true;
                    UiFactory.Stretch(img.rectTransform, new Vector2(0.2f, 0.42f), new Vector2(0.8f, 0.9f));
                    UiFactory.Stretch(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.42f));
                }
                Image line = UiFactory.CreateImage("Line", b.transform, config.uiAccent, false);
                UiFactory.Stretch(line.rectTransform, new Vector2(0.15f, 1f), new Vector2(0.85f, 1f));
                line.rectTransform.offsetMin = new Vector2(0f, -6f);
                tabButtons.Add(b);
            }
        }

        /// <summary>Met en évidence l'onglet courant.</summary>
        private void RefreshTabs()
        {
            GameConfig config = Config;
            if (config == null) return;
            for (int i = 0; i < tabButtons.Count; i++)
            {
                bool on = i == current;
                Color tint = on ? config.uiAccent : config.uiTextDim;
                tabButtons[i].targetGraphic.color = on ? config.uiButton : config.uiPanel;
                tabButtons[i].GetComponentInChildren<Text>().color = tint;
                Transform icon = tabButtons[i].transform.Find("Icon");
                if (icon != null) icon.GetComponent<Image>().color = tint;
                Transform line = tabButtons[i].transform.Find("Line");
                if (line != null) line.gameObject.SetActive(on);
            }
        }
    }
}

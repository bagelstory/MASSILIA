// MARSEILLE — UI/MapPanel.cs
// Écran « Carte » : la carte en haut, en bas une fiche du quartier touché et, quand un opérateur est
// choisi, le choix de la tâche puis de la destination (quartiers accessibles entourés en bleu).

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Map;
using Marseille.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Écran de la carte.</summary>
    public partial class MapPanel : UIPanel
    {
        [SerializeField] private MapController map;
        [Tooltip("Part de la hauteur réservée à la fiche du bas.")]
        [SerializeField, Range(0.15f, 0.6f)] private float sheetHeight = 0.34f;

        private Text quarterInfo;
        private Text operatorInfo;
        private RectTransform taskRows;
        private Button detailButton;

        /// <inheritdoc />
        public override string TabLabel { get { return "Carte"; } }

        /// <summary>La carte affichée.</summary>
        public MapController Map { get { return map; } }

        /// <summary>Construit la carte et la fiche du bas.</summary>
        /// <param name="config">Configuration.</param>
        protected override void Build(GameConfig config)
        {
            if (map == null)
            {
                RectTransform area = UiFactory.CreateRect("MapArea", transform);
                UiFactory.Stretch(area, new Vector2(0f, sheetHeight), Vector2.one);
                MapViewport viewport = area.gameObject.AddComponent<MapViewport>();
                map = area.gameObject.AddComponent<MapController>();
                map.Viewport = viewport;
            }
            map.QuarterSelected += OnQuarterSelected;
            map.OperatorSelected += OnOperatorSelected;

            Image sheet = UiFactory.CreateImage("Sheet", transform, config.uiPanel, true);
            UiFactory.Stretch(sheet.rectTransform, Vector2.zero, new Vector2(1f, sheetHeight));
            RectTransform col = UiKit.ScrollColumn(sheet.transform, config);
            quarterInfo = UiKit.Label(col, "Touche un quartier ou un opérateur.", config.uiFontSizes.z, config.uiText);
            UiKit.SetHeight(quarterInfo.rectTransform, config.uiFontSizes.z * 3.2f);
            operatorInfo = UiKit.Label(col, string.Empty, config.uiFontSizes.z, config.uiAccent);
            UiKit.SetHeight(operatorInfo.rectTransform, config.uiFontSizes.z * 2.2f);
            taskRows = UiKit.Column("Tasks", col, config);
            BuildTaskButtons(taskRows, config);
            RectTransform actions = UiKit.Row("Actions", col, config.uiButtonHeight, config);
            detailButton = UiKit.MakeButton(actions, "Détails", config.uiButton, OpenDetail, config);
            UiKit.MakeButton(actions, "Ma base", config.uiButton, FocusBase, config);
            UiKit.MakeButton(actions, "Annuler", config.uiButton, CancelAll, config);
            UiKit.MakeButton(actions, "Valider", config.uiAccent, ValidateCycle, config);
        }

        /// <summary>Retire les abonnements à la carte.</summary>
        private void OnDestroy()
        {
            if (map == null) return;
            map.QuarterSelected -= OnQuarterSelected;
            map.OperatorSelected -= OnOperatorSelected;
        }

        /// <summary>Met à jour la carte (reconstruite pour une nouvelle partie) et la fiche.</summary>
        public override void Refresh()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame || map == null) return;
            GameData data = gm.Data;
            if (map.BuiltSeed != data.randomSeed) { CancelAll(); map.Rebuild(); }
            else map.Refresh();
            RefreshSheet(data, gm.Config);
        }

        /// <summary>Remplit la fiche du quartier sélectionné.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        private void RefreshSheet(GameData data, GameConfig config)
        {
            int qi = data.IndexOfQuarter(map.SelectedQuarterId);
            if (detailButton != null) detailButton.interactable = qi >= 0;
            if (qi >= 0)
            {
                QuarterData q = data.quarters[qi];
                bool known = q.knowledgeLevel > 0 || q.control > 0f;
                int zi = data.IndexOfZone(q.zoneId);
                string zone = zi >= 0 ? data.zones[zi].shortName + (data.zones[zi].isUnlocked ? "" : " (verrouillée)") : "";
                quarterInfo.text = q.name + " · " + zone + "\n"
                    + (known ? UiText.Status(TerritoryRules.Status(q, config)) + " · contrôle " + q.control.ToString("0") + " % · adverse "
                               + q.hostilePresence.ToString("0") + " % · charbons " + MapStyle.PlayerCharbons(q) + "/" + q.pois.Count
                             : "Quartier inconnu : envoie un Repérage.");
            }
            RefreshOperatorLine(data, config);
        }

        /// <summary>Centre la carte sur le quartier de base.</summary>
        private void FocusBase()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.HasGame) map.FocusQuarter(gm.Data.baseQuarterId);
        }

        /// <summary>Valide le cycle depuis la carte.</summary>
        private void ValidateCycle()
        {
            CancelAll();
            if (Manager != null) Manager.ValidateCycle();
        }

        /// <summary>Ouvre le détail du quartier sélectionné.</summary>
        private void OpenDetail()
        {
            if (Manager != null && map.SelectedQuarterId != Owners.None) Manager.RequestQuarterDetail(map.SelectedQuarterId);
        }
    }
}

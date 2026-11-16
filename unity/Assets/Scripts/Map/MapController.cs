// MARSEILLE — Map/MapController.cs
// Carte 2D en Canvas (spec §12) : crée une case par quartier et un pion par opérateur, les met à jour
// quand les données changent (EventBus), gère la sélection et la mise en évidence des destinations.
// Aucune logique de jeu ici : les écrans réagissent aux événements QuarterSelected / OperatorSelected.

using System;
using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Map
{
    /// <summary>Contrôleur de la carte.</summary>
    public partial class MapController : MonoBehaviour
    {
        [SerializeField] private MapViewport viewport;
        [SerializeField] private RectTransform content;
        [Tooltip("Prefab de case (facultatif : créée par code sinon).")]
        [SerializeField] private QuarterNode quarterPrefab;
        [Tooltip("Prefab de pion (facultatif : créé par code sinon).")]
        [SerializeField] private OperatorNode operatorPrefab;
        [SerializeField] private Font font;
        [Tooltip("Forme ronde des pions (facultatif).")]
        [SerializeField] private Sprite operatorSprite;

        private readonly Dictionary<int, QuarterNode> quarterNodes = new Dictionary<int, QuarterNode>();
        private readonly Dictionary<int, OperatorNode> operatorNodes = new Dictionary<int, OperatorNode>();
        private readonly HashSet<int> highlighted = new HashSet<int>();
        private Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();
        private RectTransform quarterLayer;
        private RectTransform operatorLayer;
        private bool labelsVisible = true;

        /// <summary>Un quartier a été touché (identifiant).</summary>
        public event Action<int> QuarterSelected;

        /// <summary>Un opérateur a été touché (identifiant).</summary>
        public event Action<int> OperatorSelected;

        /// <summary>Quartier sélectionné, ou -1.</summary>
        public int SelectedQuarterId { get; private set; }

        /// <summary>Opérateur sélectionné, ou -1.</summary>
        public int SelectedOperatorId { get; private set; }

        /// <summary>Navigation de la carte (modifiable par code : l'abonnement au zoom suit).</summary>
        public MapViewport Viewport
        {
            get { return viewport; }
            set
            {
                if (viewport != null) viewport.ZoomChanged -= HandleZoom;
                viewport = value;
                if (viewport != null && isActiveAndEnabled) viewport.ZoomChanged += HandleZoom;
            }
        }

        /// <summary>Graine de la partie pour laquelle la carte a été construite (0 si jamais construite).</summary>
        public int BuiltSeed { get; private set; }

        /// <summary>Valeurs par défaut.</summary>
        private void Awake()
        {
            SelectedQuarterId = Owners.None;
            SelectedOperatorId = Owners.None;
            if (font == null) font = MapStyle.BuiltinFont();
        }

        /// <summary>Abonnements aux événements de jeu.</summary>
        private void OnEnable()
        {
            EventBus.OnGameLoaded += Rebuild;
            EventBus.OnDataChanged += Refresh;
            EventBus.OnCycleResolved += HandleCycleResolved;
            if (viewport != null) viewport.ZoomChanged += HandleZoom;
        }

        /// <summary>Désabonnements.</summary>
        private void OnDisable()
        {
            EventBus.OnGameLoaded -= Rebuild;
            EventBus.OnDataChanged -= Refresh;
            EventBus.OnCycleResolved -= HandleCycleResolved;
            if (viewport != null) viewport.ZoomChanged -= HandleZoom;
        }

        /// <summary>Construit la carte si une partie est déjà chargée.</summary>
        private void Start()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.HasGame) Rebuild();
        }

        /// <summary>Recrée toutes les cases et tous les pions pour la partie en cours.</summary>
        public void Rebuild()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame || gm.Config == null) return;
            GameConfig config = gm.Config;
            GameData data = gm.Data;
            EnsureLayers(config);
            Clear();
            positions = MapLayout.Compute(data.quarters, config);
            BuiltSeed = data.randomSeed;
            Vector2 size = MapLayout.NodeSize(config);
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                QuarterNode node = quarterPrefab != null ? Instantiate(quarterPrefab, quarterLayer, false) : QuarterNode.CreateDefault(quarterLayer, font, config);
                node.Bind(q.id, q.name, this);
                node.Rect.sizeDelta = size;
                node.Rect.anchoredPosition = positions[q.id];
                quarterNodes[q.id] = node;
            }
            if (viewport != null)
            {
                viewport.Configure(config.mapZoomRange, config.mapStartZoom, config.mapScrollZoomStep);
                Vector2 basePos;
                if (positions.TryGetValue(data.baseQuarterId, out basePos)) viewport.CenterOn(basePos);
            }
            Refresh();
        }

        /// <summary>Met à jour toutes les cases et tous les pions d'après les données actuelles.</summary>
        public void Refresh()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame || quarterLayer == null) return;
            GameConfig config = gm.Config;
            GameData data = gm.Data;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                QuarterNode node;
                if (!quarterNodes.TryGetValue(q.id, out node)) continue;
                node.Refresh(q, data, config, q.id == SelectedQuarterId, highlighted.Contains(q.id));
                node.SetLabelVisible(labelsVisible);
            }
            RefreshOperators(data, config);
        }

        /// <summary>Après un cycle : tout redessiner.</summary>
        /// <param name="result">Résultat du cycle.</param>
        private void HandleCycleResolved(CycleResult result)
        {
            Refresh();
        }

        /// <summary>Zoom modifié : afficher ou masquer les noms.</summary>
        /// <param name="zoom">Nouveau zoom.</param>
        private void HandleZoom(float zoom)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || gm.Config == null) return;
            bool visible = zoom >= gm.Config.mapLabelMinZoom;
            if (visible == labelsVisible) return;
            labelsVisible = visible;
            foreach (QuarterNode node in quarterNodes.Values) node.SetLabelVisible(visible);
        }

        /// <summary>Crée le plan et ses deux calques (cases, pions) s'ils manquent.</summary>
        /// <param name="config">Configuration.</param>
        private void EnsureLayers(GameConfig config)
        {
            if (content == null)
            {
                Transform parent = viewport != null ? viewport.transform : transform;
                content = UiFactory.CreateRect("MapContent", parent);
                if (viewport != null) viewport.Content = content;
            }
            content.anchorMin = content.anchorMax = content.pivot = new Vector2(0.5f, 0.5f);
            content.sizeDelta = config.mapSize;
            if (quarterLayer == null) { quarterLayer = UiFactory.CreateRect("Quarters", content); UiFactory.Stretch(quarterLayer, Vector2.zero, Vector2.one); }
            if (operatorLayer == null) { operatorLayer = UiFactory.CreateRect("Operators", content); UiFactory.Stretch(operatorLayer, Vector2.zero, Vector2.one); }
            labelsVisible = config.mapStartZoom >= config.mapLabelMinZoom;
        }

        /// <summary>Détruit toutes les cases et tous les pions.</summary>
        private void Clear()
        {
            foreach (QuarterNode n in quarterNodes.Values) if (n != null) Destroy(n.gameObject);
            foreach (OperatorNode n in operatorNodes.Values) if (n != null) Destroy(n.gameObject);
            quarterNodes.Clear();
            operatorNodes.Clear();
            highlighted.Clear();
            SelectedQuarterId = Owners.None;
            SelectedOperatorId = Owners.None;
        }
    }
}

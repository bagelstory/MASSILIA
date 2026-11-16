// MARSEILLE — UI/UIManager.cs
// Chef d'orchestre de l'interface : écrans (onglets), barre du haut, messages courts, exécution des
// actions du joueur (avec journal et sauvegarde), réaction aux états de la partie.
// Il suffit de le poser sur un objet de la scène : Canvas, EventSystem et écrans se créent s'ils manquent.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Systems;
using UnityEngine;

namespace Marseille.UI
{
    /// <summary>Action du joueur appliquée à la partie (affecter, recruter, investir…).</summary>
    /// <param name="data">Partie modifiée.</param>
    /// <param name="config">Configuration.</param>
    /// <returns>Résultat à afficher.</returns>
    public delegate ActionResult GameAction(ref GameData data, GameConfig config);

    /// <summary>Gestionnaire de l'interface.</summary>
    public partial class UIManager : MonoBehaviour
    {
        [Tooltip("Écrans principaux, dans l'ordre des onglets. Vide : écrans par défaut créés par code.")]
        [SerializeField] private List<UIPanel> panels = new List<UIPanel>();
        [Tooltip("Écran des résultats d'un cycle. Vide : ResultOverlay créé par code.")]
        [SerializeField] private MonoBehaviour resultsHandler;
        [SerializeField] private Font font;

        private int current = -1;
        private float toastHideAt;

        /// <summary>Instance de la scène.</summary>
        public static UIManager Instance { get; private set; }

        /// <summary>Écrans enregistrés.</summary>
        public IList<UIPanel> Panels { get { return panels; } }

        /// <summary>Accès raccourci à la configuration.</summary>
        public GameConfig Config { get { return GameManager.Instance != null ? GameManager.Instance.Config : null; } }

        /// <summary>Un écran demande le détail d'un quartier (QuarterDetailPopup, étape 8).</summary>
        public event System.Action<int> QuarterDetailRequested;

        /// <summary>Un écran demande le détail d'un opérateur (OperatorDetailPopup, étape 8).</summary>
        public event System.Action<int> OperatorDetailRequested;

        /// <summary>Singleton de scène.</summary>
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (font != null) UiKit.Font = font;
        }

        /// <summary>Construit l'interface et lance une partie s'il n'y en a pas.</summary>
        private void Start()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || gm.Config == null) { Debug.LogError("UIManager : GameManager ou GameConfig absent."); return; }
            BuildChrome(gm.Config);
            if (panels.Count == 0) CreateDefaultPanels(gm.Config);
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].transform.SetParent(contentArea, false);
                panels[i].Initialize(this);
            }
            BuildTabs(gm.Config);
            BuildOverlays(gm.Config);
            if (toastBackground != null) toastBackground.transform.SetAsLastSibling();
            if (!gm.HasGame && !gm.TryContinue()) gm.NewGame();
            else if (gm.HasGame) EventBus.RaiseGameLoaded();
            ShowPanel(0);
            RefreshTopBar();
        }

        /// <summary>Abonnements.</summary>
        private void OnEnable()
        {
            EventBus.OnDataChanged += RefreshTopBar;
            EventBus.OnGameLoaded += RefreshTopBar;
            EventBus.OnCycleResolved += HandleCycleResolved;
        }

        /// <summary>Désabonnements.</summary>
        private void OnDisable()
        {
            EventBus.OnDataChanged -= RefreshTopBar;
            EventBus.OnGameLoaded -= RefreshTopBar;
            EventBus.OnCycleResolved -= HandleCycleResolved;
        }

        /// <summary>Masque le message court à l'échéance (simple comparaison d'horloge, pas de coroutine).</summary>
        private void Update()
        {
            if (toastBackground != null && toastBackground.gameObject.activeSelf && Time.unscaledTime >= toastHideAt) toastBackground.gameObject.SetActive(false);
        }

        /// <summary>Affiche l'écran d'index donné.</summary>
        /// <param name="index">Index de l'onglet.</param>
        public void ShowPanel(int index)
        {
            if (index < 0 || index >= panels.Count) return;
            current = index;
            for (int i = 0; i < panels.Count; i++) panels[i].SetVisible(i == index);
            RefreshTabs();
        }

        /// <summary>Affiche le premier écran du type donné.</summary>
        /// <typeparam name="T">Type d'écran.</typeparam>
        /// <returns>L'écran affiché, ou null.</returns>
        public T ShowPanel<T>() where T : UIPanel
        {
            for (int i = 0; i < panels.Count; i++) if (panels[i] is T) { ShowPanel(i); return (T)panels[i]; }
            return null;
        }

        /// <summary>Exécute une action du joueur : message, journal si réussie, sauvegarde et rafraîchissement.</summary>
        /// <param name="action">Action.</param>
        /// <param name="quarterId">Quartier concerné pour le journal, ou -1.</param>
        /// <returns>Le résultat.</returns>
        public ActionResult Perform(GameAction action, int quarterId)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame) return ActionResult.Fail("Aucune partie.");
            if (gm.State != GameState.Planning) return ActionResult.Fail("Attends la fin du cycle.");
            ActionResult result = action(ref gm.Data, gm.Config);
            if (!string.IsNullOrEmpty(result.message)) ShowToast(result.message, result.ok);
            if (result.ok)
            {
                if (!string.IsNullOrEmpty(result.message)) gm.AddJournal(result.message, JournalType.Economy, quarterId);
                gm.NotifyDataChanged();
            }
            return result;
        }

        /// <summary>Valide le cycle (bouton « Valider »).</summary>
        public void ValidateCycle()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;
            if (!gm.ValidateCycle()) ShowToast("Impossible de valider maintenant.", false);
        }

        /// <summary>Demande l'affichage du détail d'un quartier.</summary>
        /// <param name="quarterId">Quartier.</param>
        public void RequestQuarterDetail(int quarterId)
        {
            if (QuarterDetailRequested != null) QuarterDetailRequested(quarterId);
        }

        /// <summary>Demande l'affichage du détail d'un opérateur.</summary>
        /// <param name="operatorId">Opérateur.</param>
        public void RequestOperatorDetail(int operatorId)
        {
            if (OperatorDetailRequested != null) OperatorDetailRequested(operatorId);
        }

        /// <summary>Après un cycle : résumé court ; sans écran de résultats, retour direct à la planification.</summary>
        /// <param name="result">Résultat.</param>
        private void HandleCycleResolved(CycleResult result)
        {
            string sign = result.treasuryDelta >= 0f ? "+" : "";
            ShowToast("Cycle " + result.cycle + " : " + sign + result.treasuryDelta.ToString("0") + " €, territoire "
                      + (result.territoryDelta >= 0f ? "+" : "") + result.territoryDelta.ToString("0.0") + " %.", result.treasuryDelta >= 0f);
            if (resultsHandler == null && result.endType == EndType.None && GameManager.Instance != null) GameManager.Instance.AcknowledgeResults();
            RefreshTopBar();
        }
    }
}

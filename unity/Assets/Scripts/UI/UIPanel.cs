// MARSEILLE — UI/UIPanel.cs
// Base des écrans : construction par code si rien n'est câblé dans l'éditeur, affichage, rafraîchissement
// quand les données changent (seulement s'il est visible).

using Marseille.Config;
using Marseille.Core;
using UnityEngine;

namespace Marseille.UI
{
    /// <summary>Écran principal affiché dans la zone centrale (un onglet).</summary>
    public abstract class UIPanel : MonoBehaviour
    {
        private bool built;

        /// <summary>Libellé de l'onglet.</summary>
        public abstract string TabLabel { get; }

        /// <summary>Gestionnaire d'interface propriétaire.</summary>
        protected UIManager Manager { get; private set; }

        /// <summary>Configuration de jeu.</summary>
        protected GameConfig Config { get { return GameManager.Instance != null ? GameManager.Instance.Config : null; } }

        /// <summary>Le panneau est-il affiché ?</summary>
        public bool IsVisible { get { return gameObject.activeSelf; } }

        /// <summary>Prépare le panneau (appelé une fois par UIManager).</summary>
        /// <param name="manager">Gestionnaire.</param>
        public void Initialize(UIManager manager)
        {
            Manager = manager;
            if (!built && Config != null)
            {
                Build(Config);
                built = true;
            }
        }

        /// <summary>Construit le contenu par code.</summary>
        /// <param name="config">Configuration.</param>
        protected abstract void Build(GameConfig config);

        /// <summary>Remet l'affichage à jour d'après la partie.</summary>
        public abstract void Refresh();

        /// <summary>Affiche ou masque le panneau.</summary>
        /// <param name="visible">Visible.</param>
        public void SetVisible(bool visible)
        {
            if (gameObject.activeSelf != visible) gameObject.SetActive(visible);
            if (visible) Refresh();
        }

        /// <summary>Abonnements pendant l'affichage.</summary>
        protected virtual void OnEnable()
        {
            EventBus.OnDataChanged += HandleChange;
            EventBus.OnGameLoaded += HandleChange;
            EventBus.OnCycleResolved += HandleCycle;
        }

        /// <summary>Désabonnements.</summary>
        protected virtual void OnDisable()
        {
            EventBus.OnDataChanged -= HandleChange;
            EventBus.OnGameLoaded -= HandleChange;
            EventBus.OnCycleResolved -= HandleCycle;
        }

        /// <summary>Données modifiées.</summary>
        private void HandleChange()
        {
            if (Manager != null && GameManager.Instance != null && GameManager.Instance.HasGame) Refresh();
        }

        /// <summary>Cycle résolu.</summary>
        /// <param name="result">Résultat.</param>
        private void HandleCycle(CycleResult result)
        {
            HandleChange();
        }
    }
}

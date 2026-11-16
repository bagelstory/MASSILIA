// MARSEILLE — UI/UIManager.Overlays.cs
// Partie « fenêtres » de UIManager : détail de quartier, détail d'opérateur, résultats, fin de partie.
// Créées par code si elles ne sont pas câblées dans l'éditeur, au-dessus de tout le reste.

using Marseille.Config;
using Marseille.Map;
using UnityEngine;

namespace Marseille.UI
{
    public partial class UIManager
    {
        [Header("Fenêtres (facultatif : créées par code)")]
        [SerializeField] private QuarterDetailPopup quarterDetail;
        [SerializeField] private OperatorDetailPopup operatorDetail;
        [SerializeField] private ResultOverlay resultOverlay;
        [SerializeField] private GameOverPanel gameOverPanel;

        /// <summary>Crée et initialise les fenêtres ; l'écran des résultats prend la main après chaque cycle.</summary>
        /// <param name="config">Configuration.</param>
        private void BuildOverlays(GameConfig config)
        {
            if (quarterDetail == null) quarterDetail = CreateOverlay<QuarterDetailPopup>("QuarterDetail");
            if (operatorDetail == null) operatorDetail = CreateOverlay<OperatorDetailPopup>("OperatorDetail");
            if (resultOverlay == null) resultOverlay = CreateOverlay<ResultOverlay>("Results");
            if (gameOverPanel == null) gameOverPanel = CreateOverlay<GameOverPanel>("GameOver");
            quarterDetail.Initialize(this, config);
            operatorDetail.Initialize(this, config);
            resultOverlay.Initialize(this, config);
            gameOverPanel.Initialize(this, config);
            if (resultsHandler == null) resultsHandler = resultOverlay;
        }

        /// <summary>Crée une fenêtre vide sur tout l'écran.</summary>
        /// <typeparam name="T">Type de fenêtre.</typeparam>
        /// <param name="name">Nom.</param>
        /// <returns>La fenêtre.</returns>
        private T CreateOverlay<T>(string name) where T : UIOverlay
        {
            RectTransform rt = UiFactory.CreateRect(name, transform);
            UiFactory.Stretch(rt, Vector2.zero, Vector2.one);
            return rt.gameObject.AddComponent<T>();
        }

        /// <summary>Ferme toutes les fenêtres de détail (pas les résultats ni la fin).</summary>
        public void CloseDetails()
        {
            if (quarterDetail != null) quarterDetail.Close();
            if (operatorDetail != null) operatorDetail.Close();
        }
    }
}

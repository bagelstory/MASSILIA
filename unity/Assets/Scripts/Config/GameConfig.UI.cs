// MARSEILLE — Config/GameConfig.UI.cs
// Section « interface » de GameConfig (spec §12) : résolution de référence portrait, tailles, couleurs.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Interface : mise en page")]
        [Tooltip("Résolution de référence du CanvasScaler (portrait).")]
        public Vector2 uiReferenceResolution = new Vector2(1080f, 1920f);
        [Tooltip("Hauteur de la barre du haut et de la barre d'onglets (pixels de référence).")]
        public Vector2 uiBarHeights = new Vector2(150f, 140f);
        [Tooltip("Marge intérieure des écrans (pixels).")]
        [Min(0f)] public float uiPadding = 28f;
        [Tooltip("Espace entre deux lignes (pixels).")]
        [Min(0f)] public float uiSpacing = 14f;
        [Tooltip("Hauteur d'un bouton (pixels).")]
        [Min(40f)] public float uiButtonHeight = 110f;
        [Tooltip("Tailles de texte : titre, normal, petit.")]
        public Vector3Int uiFontSizes = new Vector3Int(46, 34, 27);
        [Tooltip("Durée d'affichage d'un message (secondes).")]
        [Min(0.5f)] public float uiToastSeconds = 2.5f;

        [Header("Interface : couleurs")]
        public Color uiBackground = new Color(0.07f, 0.075f, 0.09f);
        public Color uiPanel = new Color(0.12f, 0.125f, 0.15f);
        public Color uiButton = new Color(0.2f, 0.21f, 0.25f);
        public Color uiAccent = new Color(0.42f, 0.81f, 0.31f);
        public Color uiText = new Color(0.93f, 0.93f, 0.93f);
        public Color uiTextDim = new Color(0.62f, 0.63f, 0.66f);
        public Color uiPositive = new Color(0.42f, 0.81f, 0.31f);
        public Color uiNegative = new Color(0.9f, 0.3f, 0.3f);
        public Color uiWarning = new Color(0.95f, 0.65f, 0.2f);
    }
}

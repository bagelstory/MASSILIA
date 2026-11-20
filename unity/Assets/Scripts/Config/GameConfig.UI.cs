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
        public Vector2 uiBarHeights = new Vector2(200f, 140f);
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

        [Header("Interface : couleurs (palette CHOUF : anthracite, ivoire, jaune chaud, rouille)")]
        public Color uiBackground = new Color(0.078f, 0.090f, 0.110f);
        public Color uiPanel = new Color(0.106f, 0.125f, 0.145f);
        public Color uiButton = new Color(0.137f, 0.165f, 0.192f);
        public Color uiAccent = new Color(0.878f, 0.663f, 0.231f);
        public Color uiText = new Color(0.925f, 0.902f, 0.839f);
        public Color uiTextDim = new Color(0.604f, 0.639f, 0.659f);
        public Color uiPositive = new Color(0.373f, 0.561f, 0.353f);
        public Color uiNegative = new Color(0.769f, 0.341f, 0.310f);
        public Color uiWarning = new Color(0.769f, 0.392f, 0.165f);
        public Color uiCard = new Color(0.122f, 0.153f, 0.180f);
        public Color uiCardBorder = new Color(0.184f, 0.220f, 0.251f);
        public Color uiInfo = new Color(0.420f, 0.455f, 0.502f);

        [Header("Interface : composants")]
        [Tooltip("Hauteur d'une puce (filtre, indicateur) en pixels de référence.")]
        [Min(30f)] public float uiChipHeight = 64f;
        [Tooltip("Pression à partir de laquelle la puce passe en couleur d'alerte.")]
        [Min(0f)] public float uiPressureWarn = 30f;
        [Tooltip("Nombre de semaines par année affichée dans la barre du haut.")]
        [Min(1)] public int uiWeeksPerYear = 52;

        /// <summary>Remet la palette aux valeurs par défaut (utile quand l'asset garde d'anciennes couleurs).</summary>
        [ContextMenu("Réinitialiser la palette CHOUF")]
        public void ResetPalette()
        {
            GameConfig d = CreateInstance<GameConfig>();
            uiBackground = d.uiBackground; uiPanel = d.uiPanel; uiButton = d.uiButton; uiAccent = d.uiAccent;
            uiText = d.uiText; uiTextDim = d.uiTextDim; uiPositive = d.uiPositive; uiNegative = d.uiNegative;
            uiWarning = d.uiWarning; uiCard = d.uiCard; uiCardBorder = d.uiCardBorder; uiInfo = d.uiInfo;
            uiBarHeights = d.uiBarHeights;
            DestroyImmediate(d);
        }
    }
}

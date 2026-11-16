// MARSEILLE — Config/GameConfig.Map.cs
// Section « carte » de GameConfig (spec §12) : grille, tailles, zoom, couleurs des statuts.

using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Carte : disposition")]
        [Tooltip("Taille du plan en pixels de référence (portrait).")]
        public Vector2 mapSize = new Vector2(1080f, 1440f);
        [Tooltip("Ranger les quartiers sur une grille (lisible sur téléphone) plutôt qu'à leur position exacte.")]
        public bool mapUseGrid = true;
        [Tooltip("Colonnes et lignes de la grille.")]
        public Vector2Int mapGrid = new Vector2Int(12, 16);
        [Tooltip("Marge intérieure du plan (pixels).")]
        [Min(0f)] public float mapPadding = 24f;
        [Tooltip("Espace entre deux cases (part de la case).")]
        [Range(0f, 0.5f)] public float mapCellGap = 0.08f;
        [Tooltip("Taille d'un quartier hors grille (pixels).")]
        public Vector2 mapNodeSize = new Vector2(78f, 78f);
        [Tooltip("Taille d'un pion d'opérateur (pixels).")]
        [Min(4f)] public float mapOperatorSize = 26f;

        [Header("Carte : zoom et déplacement")]
        [Tooltip("Zoom minimal et maximal.")]
        public Vector2 mapZoomRange = new Vector2(0.6f, 3f);
        [Tooltip("Zoom au lancement.")]
        [Min(0.1f)] public float mapStartZoom = 1f;
        [Tooltip("Pas de zoom par cran de molette.")]
        [Min(0f)] public float mapScrollZoomStep = 0.1f;
        [Tooltip("Zoom à partir duquel les noms des quartiers s'affichent.")]
        [Min(0f)] public float mapLabelMinZoom = 1.3f;
        [Tooltip("Taille du texte des noms (pixels).")]
        [Min(6)] public int mapLabelFontSize = 13;

        [Header("Carte : couleurs")]
        public Color mapColorFree = new Color(0.55f, 0.56f, 0.59f);
        public Color mapColorPlayer = new Color(0.42f, 0.81f, 0.31f);
        public Color mapColorSecured = new Color(0.20f, 0.70f, 0.20f);
        public Color mapColorContested = new Color(0.95f, 0.55f, 0.15f);
        public Color mapColorUnknown = new Color(0.18f, 0.19f, 0.22f);
        [Tooltip("Assombrissement des zones verrouillées (multiplicateur).")]
        [Range(0f, 1f)] public float mapLockedDim = 0.4f;
        public Color mapColorSelection = new Color(1f, 0.95f, 0.4f);
        public Color mapColorHighlight = new Color(0.4f, 0.85f, 1f);
        public Color mapColorThreat = new Color(0.9f, 0.2f, 0.2f);
        public Color mapColorText = new Color(0.95f, 0.95f, 0.95f);
        [Tooltip("Épaisseur du contour de sélection (pixels).")]
        [Min(0f)] public float mapOutlineWidth = 3f;
    }
}

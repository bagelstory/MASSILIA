// MARSEILLE — UI/ListPanel.cs
// Base des écrans en liste : une colonne défilante vidée et remplie à chaque rafraîchissement
// (seulement quand les données changent et que l'écran est visible).

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using UnityEngine;

namespace Marseille.UI
{
    /// <summary>Écran à liste défilante.</summary>
    public abstract class ListPanel : UIPanel
    {
        /// <summary>Colonne de contenu.</summary>
        protected RectTransform Column { get; private set; }

        /// <summary>Construit le fond et la colonne défilante.</summary>
        /// <param name="config">Configuration.</param>
        protected override void Build(GameConfig config)
        {
            UiKit.Panel("Background", transform, config.uiBackground).raycastTarget = false;
            Column = UiKit.ScrollColumn(transform, config);
        }

        /// <summary>Vide la liste et la remplit.</summary>
        public override void Refresh()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.HasGame || Column == null) return;
            UiKit.ClearChildren(Column);
            Fill(gm.Data, gm.Config);
        }

        /// <summary>Remplit la liste.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        protected abstract void Fill(GameData data, GameConfig config);

        /// <summary>Titre de section.</summary>
        /// <param name="text">Texte.</param>
        /// <param name="config">Configuration.</param>
        protected void Title(string text, GameConfig config)
        {
            UiKit.Label(Column, text, config.uiFontSizes.x, config.uiText);
        }
    }
}

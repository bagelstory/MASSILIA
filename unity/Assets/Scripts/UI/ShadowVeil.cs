// MARSEILLE — UI/ShadowVeil.cs
// Le voile de l'Ombre : un calque noir posé sur la zone centrale (carte et écrans), qui s'épaissit
// à mesure que la jauge Ombre monte (design/11-bible-ombre.md §4). Ne bloque jamais les touches.

using Marseille.Core;
using Marseille.Map;
using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Assombrit l'écran selon la jauge Ombre.</summary>
    public sealed class ShadowVeil : MonoBehaviour
    {
        /// <summary>Image du voile.</summary>
        private Image veil;

        /// <summary>Crée le voile, étiré sur tout le parent, transparent aux touches.</summary>
        /// <param name="parent">Parent (racine de l'interface).</param>
        /// <returns>Le composant créé.</returns>
        public static ShadowVeil Create(Transform parent)
        {
            Image img = UiFactory.CreateImage("ShadowVeil", parent, new Color(0f, 0f, 0f, 0f), false);
            UiFactory.Stretch(img.rectTransform, Vector2.zero, Vector2.one);
            img.raycastTarget = false;
            ShadowVeil v = img.gameObject.AddComponent<ShadowVeil>();
            v.veil = img;
            v.Refresh();
            return v;
        }

        /// <summary>Limite le voile à la zone centrale (entre la barre du haut et les onglets).</summary>
        /// <param name="bottom">Hauteur de la barre des onglets.</param>
        /// <param name="top">Hauteur de la barre du haut.</param>
        public void Fit(float bottom, float top)
        {
            if (veil == null) return;
            veil.rectTransform.offsetMin = new Vector2(0f, bottom);
            veil.rectTransform.offsetMax = new Vector2(0f, -top);
        }

        /// <summary>S'abonne aux changements de la partie.</summary>
        private void OnEnable()
        {
            EventBus.OnDataChanged += Refresh;
        }

        /// <summary>Se désabonne.</summary>
        private void OnDisable()
        {
            EventBus.OnDataChanged -= Refresh;
        }

        /// <summary>Met à jour l'opacité selon l'Ombre de la partie en cours.</summary>
        public void Refresh()
        {
            GameManager gm = GameManager.Instance;
            if (veil == null) return;
            float alpha = 0f;
            if (gm != null && gm.HasGame && gm.Config != null) alpha = gm.Data.shadow.ombre / 100f * gm.Config.ombreVeilMax;
            veil.color = new Color(0f, 0f, 0f, alpha);
        }
    }
}

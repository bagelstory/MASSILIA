// MARSEILLE — UI/UiSprites.cs
// Rectangle à coins arrondis généré au lancement (sprite « neuf-découpes ») : aucun fichier image à fournir.

using UnityEngine;
using UnityEngine.UI;

namespace Marseille.UI
{
    /// <summary>Sprites d'interface générés par code.</summary>
    public static class UiSprites
    {
        private const int Size = 48;
        private const int Radius = 18;
        private static Sprite rounded;

        /// <summary>Sprite arrondi partagé (bordures de 18 px pour le découpage).</summary>
        public static Sprite Rounded
        {
            get { return rounded != null ? rounded : (rounded = Build()); }
        }

        /// <summary>Donne à une image le fond arrondi (découpé, la couleur de l'image sert de teinte).</summary>
        /// <param name="image">Image.</param>
        public static void Round(Image image)
        {
            if (image == null) return;
            image.sprite = Rounded;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 1f;
        }

        /// <summary>Rend à une image son fond plat.</summary>
        /// <param name="image">Image.</param>
        public static void Flat(Image image)
        {
            if (image == null) return;
            image.sprite = null;
            image.type = Image.Type.Simple;
        }

        private static Sprite Build()
        {
            Texture2D tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            Color32[] px = new Color32[Size * Size];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float dx = Mathf.Max(Radius - 0.5f - x, x - (Size - Radius - 0.5f), 0f);
                    float dy = Mathf.Max(Radius - 0.5f - y, y - (Size - Radius - 0.5f), 0f);
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(Radius - dist);
                    px[y * Size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            }
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0f, 0f, Size, Size), new Vector2(0.5f, 0.5f), 100f, 0u,
                SpriteMeshType.FullRect, new Vector4(Radius, Radius, Radius, Radius));
        }
    }
}

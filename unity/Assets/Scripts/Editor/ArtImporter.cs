// MARSEILLE — Editor/ArtImporter.cs
// Réglages d'import automatiques des images du jeu (Assets/Resources/Art) : sprites sans mipmaps,
// transparence, taille maximale réduite pour le poids du build ; bords 9-slice pour la case de quartier.

using UnityEditor;
using UnityEngine;

namespace Marseille.EditorTools
{
    /// <summary>Import des images de l'interface en sprites.</summary>
    public class ArtImporter : AssetPostprocessor
    {
        private const string ArtFolder = "/Resources/Art/";

        /// <summary>Appliqué par Unity avant l'import de chaque texture.</summary>
        private void OnPreprocessTexture()
        {
            if (!assetPath.Replace('\\', '/').Contains(ArtFolder)) return;
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.maxTextureSize = assetPath.Contains("/screens/") ? 2048
                : assetPath.Contains("/portraits/") || assetPath.Contains("/icon/") || assetPath.Contains("/logo/") ? 1024 : 256;
            importer.textureCompression = TextureImporterCompression.Compressed;
            if (assetPath.EndsWith("/shapes/tile.png")) importer.spriteBorder = new Vector4(24f, 24f, 24f, 24f);
        }
    }
}

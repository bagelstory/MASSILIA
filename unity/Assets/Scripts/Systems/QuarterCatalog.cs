// MARSEILLE — Systems/QuarterCatalog.cs
// Lecture du fichier Resources/Data/quarters.json (111 quartiers officiels, produit à l'étape 10).
// JsonUtility ne lit pas un tableau à la racine : le fichier a la forme { "quarters": [ ... ] }.

using System.Collections.Generic;
using UnityEngine;

namespace Marseille.Systems
{
    /// <summary>Un quartier tel qu'écrit dans quarters.json.</summary>
    [System.Serializable]
    public struct QuarterRecord
    {
        /// <summary>Identifiant (arrondissement × 100 + numéro, ex. 101 pour Belsunce).</summary>
        public int id;
        /// <summary>Nom officiel.</summary>
        public string name;
        /// <summary>Arrondissement (1-16).</summary>
        public int arrondissement;
        /// <summary>Zone de jeu (1-10).</summary>
        public int zoneId;
        /// <summary>Position du centre sur la carte, normalisée 0-1 (0,0 = coin haut gauche).</summary>
        public float x;
        /// <summary>Position verticale normalisée 0-1.</summary>
        public float y;
        /// <summary>Identifiants des quartiers voisins.</summary>
        public List<int> neighbours;
    }

    /// <summary>Enveloppe racine de quarters.json.</summary>
    [System.Serializable]
    public class QuarterCatalogFile
    {
        /// <summary>Les quartiers.</summary>
        public List<QuarterRecord> quarters = new List<QuarterRecord>();
    }

    /// <summary>Chargement du catalogue des quartiers.</summary>
    public static class QuarterCatalog
    {
        /// <summary>Chemin dans Resources, sans extension.</summary>
        public const string ResourcePath = "Data/quarters";

        /// <summary>Lit un catalogue depuis du texte JSON.</summary>
        /// <param name="json">Contenu du fichier.</param>
        /// <returns>Les quartiers (liste vide si le texte est invalide).</returns>
        public static List<QuarterRecord> Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<QuarterRecord>();
            try
            {
                QuarterCatalogFile file = JsonUtility.FromJson<QuarterCatalogFile>(json);
                return file != null && file.quarters != null ? file.quarters : new List<QuarterRecord>();
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("quarters.json illisible : " + e.Message);
                return new List<QuarterRecord>();
            }
        }

        /// <summary>Charge Resources/Data/quarters.json.</summary>
        /// <returns>Les quartiers (liste vide, avec une erreur en console, si le fichier manque).</returns>
        public static List<QuarterRecord> Load()
        {
            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            if (asset == null)
            {
                Debug.LogError("Resources/" + ResourcePath + ".json introuvable (fichier livré à l'étape 10).");
                return new List<QuarterRecord>();
            }
            return Parse(asset.text);
        }
    }
}

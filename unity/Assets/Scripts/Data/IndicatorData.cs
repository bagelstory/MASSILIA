// MARSEILLE — Data/IndicatorData.cs
// Les six indicateurs toujours visibles en haut de l'écran (spec §9).

namespace Marseille.Data
{
    /// <summary>Valeurs des six indicateurs (spec §9.1), recalculées par IndicatorSystem.</summary>
    [System.Serializable]
    public struct IndicatorData
    {
        /// <summary>Part de la ville contrôlée par le joueur (0-100 %).</summary>
        public float territory;

        /// <summary>Argent disponible (€), jamais négatif (le manque devient de la dette).</summary>
        public float treasury;

        /// <summary>Opérateurs disponibles (ni épuisés, ni perdus).</summary>
        public int effectifsAvailable;

        /// <summary>Opérateurs actifs au total (hors perdus).</summary>
        public int effectifsTotal;

        /// <summary>Surveillance extérieure (0-100). À 100 : chute.</summary>
        public float pressure;

        /// <summary>Cohérence du réseau (0-100).</summary>
        public float stability;

        /// <summary>Risque cumulé (0-100).</summary>
        public float exposure;

        /// <summary>Valeurs de départ d'une partie.</summary>
        /// <param name="treasury">Trésorerie initiale.</param>
        /// <param name="pressure">Pression initiale.</param>
        /// <param name="stability">Stabilité initiale.</param>
        /// <param name="exposure">Exposition initiale.</param>
        /// <returns>Les indicateurs initialisés.</returns>
        public static IndicatorData Create(float treasury, float pressure, float stability, float exposure)
        {
            return new IndicatorData
            {
                territory = 0f,
                treasury = treasury,
                effectifsAvailable = 0,
                effectifsTotal = 0,
                pressure = pressure,
                stability = stability,
                exposure = exposure
            };
        }

        /// <summary>Texte « disponibles/total » pour l'affichage.</summary>
        /// <returns>Par exemple « 3/4 ».</returns>
        public string EffectifsLabel()
        {
            return effectifsAvailable + "/" + effectifsTotal;
        }
    }
}

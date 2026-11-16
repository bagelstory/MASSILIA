// MARSEILLE — Utils/MathHelper.cs
// Petites fonctions de calcul utilisées par les systèmes (sans dépendance à UnityEngine.Mathf,
// pour que les systèmes restent testables hors moteur).

namespace Marseille.Utils
{
    /// <summary>Fonctions mathématiques simples.</summary>
    public static class MathHelper
    {
        /// <summary>Borne une valeur entre un minimum et un maximum.</summary>
        /// <param name="value">Valeur.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        /// <returns>Valeur bornée.</returns>
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>Borne une valeur entre 0 et 100 (indicateurs, contrôles, pourcentages).</summary>
        /// <param name="value">Valeur.</param>
        /// <returns>Valeur bornée.</returns>
        public static float Clamp100(float value)
        {
            return Clamp(value, 0f, 100f);
        }

        /// <summary>Arrondit à la dizaine (montants affichés).</summary>
        /// <param name="value">Montant.</param>
        /// <returns>Montant arrondi.</returns>
        public static float RoundToTen(float value)
        {
            return (float)System.Math.Round(value / 10.0) * 10f;
        }
    }
}

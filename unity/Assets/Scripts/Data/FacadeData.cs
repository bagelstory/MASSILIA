// MARSEILLE — Data/FacadeData.cs
// Les façades (design/13-facades.md) : des commerces qui absorbent l'argent sale. Aucun procédé, seulement
// un secteur, un quartier, une jauge d'Éveil et ce que ça coûte à la ville.
// Enums sérialisés en entiers : ajouter à la fin seulement. Structs : réassigner après modification.

namespace Marseille.Data
{
    /// <summary>Secteur d'une façade.</summary>
    [System.Serializable]
    public enum FacadeSector
    {
        Alim = 0, Chicha = 1, Garage = 2, Snack = 3, Club = 4, Label = 5, Immo = 6
    }

    /// <summary>Une façade ouverte dans un quartier.</summary>
    [System.Serializable]
    public struct FacadeData
    {
        /// <summary>Secteur.</summary>
        public FacadeSector sector;

        /// <summary>Quartier où elle est ouverte.</summary>
        public int quarterId;

        /// <summary>Ancienneté (cycles) : plus elle est vieille, plus elle est crédible.</summary>
        public int age;

        /// <summary>Exposition absorbée depuis l'ouverture (points).</summary>
        public float absorbed;

        /// <summary>Éveil (0-100) : ce que le quartier, la mairie et la presse soupçonnent.</summary>
        public float eveil;

        /// <summary>Cycles restants de gel (contrôle en cours) : ni absorption ni revenu.</summary>
        public int frozen;
    }

    /// <summary>Fiche de données d'un secteur (relative : 1 = une alimentation de quartier).</summary>
    [System.Serializable]
    public struct FacadeSpec
    {
        /// <summary>Nom affiché.</summary>
        public string name;

        /// <summary>Coût d'ouverture (€).</summary>
        public float openCost;

        /// <summary>Cycles avant le premier effet.</summary>
        public int delay;

        /// <summary>Exposition retirée par cycle, à plein régime.</summary>
        public float absorption;

        /// <summary>Ce que le quartier peut croire, par cycle (population 100, ancienneté 0).</summary>
        public float capacity;

        /// <summary>Gain de crédibilité par an d'ancienneté (0,5 = +50 % de capacité par an).</summary>
        public float growth;

        /// <summary>Revenu propre (€ par cycle).</summary>
        public float revenue;

        /// <summary>Chance de base d'un contrôle par cycle, multipliée par l'Éveil/100.</summary>
        public float risk;

        /// <summary>Les sept secteurs de départ, à vérifier en simulation sur 260 cycles.</summary>
        /// <returns>Sept fiches, dans l'ordre de <see cref="FacadeSector"/>.</returns>
        public static FacadeSpec[] Defaults()
        {
            return new[]
            {
                Make("Alimentation de quartier", 8000f, 2, 1f, 1f, 0.3f, 150f, 0.10f),
                Make("Salon de chicha", 12000f, 3, 2f, 1.6f, 0.5f, 300f, 0.15f),
                Make("Garage, lavage auto", 10000f, 2, 1f, 1f, 0.3f, 200f, 0.12f),
                Make("Snack, restaurant", 15000f, 4, 2f, 1.8f, 0.5f, 450f, 0.12f),
                Make("Club de sport", 25000f, 8, 3f, 2.2f, 1.2f, 150f, 0.08f),
                Make("Label de rap", 30000f, 6, 4f, 3f, 0.2f, 400f, 0.25f),
                Make("Immobilier", 60000f, 16, 6f, 4f, 1.5f, 800f, 0.06f)
            };
        }

        private static FacadeSpec Make(string name, float cost, int delay, float absorption, float capacity, float growth, float revenue, float risk)
        {
            return new FacadeSpec { name = name, openCost = cost, delay = delay, absorption = absorption, capacity = capacity, growth = growth, revenue = revenue, risk = risk };
        }
    }
}

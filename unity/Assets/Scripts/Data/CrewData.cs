// MARSEILLE — Data/CrewData.cs
// La hiérarchie du terrain (design/15-hierarchie.md) : rôles, postes d'un charbon, arrondissements et leur chef.
// Enums sérialisés en entiers : ajouter à la fin seulement. Structs : réassigner après modification.

namespace Marseille.Data
{
    /// <summary>Rôle d'un opérateur dans la hiérarchie.</summary>
    [System.Serializable]
    public enum CrewRole
    {
        /// <summary>Aucun : disponible pour les tâches.</summary>
        None = 0,
        /// <summary>Guetteur (le chouf) : attaché à un seul charbon.</summary>
        Guetteur = 1,
        /// <summary>Vendeur (le charbonneur) : attaché à un seul charbon.</summary>
        Vendeur = 2,
        /// <summary>Gérant : attaché à un seul charbon.</summary>
        Gerant = 3,
        /// <summary>Tête : peut tenir plusieurs charbons.</summary>
        Tete = 4,
        /// <summary>Chef d'arrondissement (réservé).</summary>
        Chef = 5
    }

    /// <summary>Les cinq postes d'un charbon et les valeurs réservées d'un poste.</summary>
    public static class CrewSlots
    {
        /// <summary>Poste vide.</summary>
        public const int Empty = -1;

        /// <summary>Poste de tête tenu par le joueur lui-même.</summary>
        public const int Player = -2;

        /// <summary>Premier guetteur.</summary>
        public const int Guetteur1 = 0;

        /// <summary>Second guetteur.</summary>
        public const int Guetteur2 = 1;

        /// <summary>Vendeur.</summary>
        public const int Vendeur = 2;

        /// <summary>Gérant.</summary>
        public const int Gerant = 3;

        /// <summary>Tête.</summary>
        public const int Head = 4;

        /// <summary>Nombre de postes par charbon.</summary>
        public const int Count = 5;

        /// <summary>Rôle correspondant à un poste.</summary>
        /// <param name="slot">Poste (0-4).</param>
        /// <returns>Le rôle.</returns>
        public static CrewRole RoleOf(int slot)
        {
            switch (slot)
            {
                case Guetteur1:
                case Guetteur2: return CrewRole.Guetteur;
                case Vendeur: return CrewRole.Vendeur;
                case Gerant: return CrewRole.Gerant;
                default: return CrewRole.Tete;
            }
        }

        /// <summary>Nom affiché d'un poste.</summary>
        /// <param name="slot">Poste (0-4).</param>
        /// <returns>Le nom.</returns>
        public static string Label(int slot)
        {
            switch (slot)
            {
                case Guetteur1: return "Guetteur 1";
                case Guetteur2: return "Guetteur 2";
                case Vendeur: return "Vendeur";
                case Gerant: return "Gérant";
                default: return "Tête";
            }
        }
    }

    /// <summary>Un arrondissement et son chef (la personne la plus puissante de la zone).</summary>
    [System.Serializable]
    public struct ArrondissementData
    {
        /// <summary>Numéro d'arrondissement (1-16).</summary>
        public int id;

        /// <summary>Chef : <see cref="Owners.Player"/>, identifiant de faction, ou <see cref="Owners.None"/>.</summary>
        public int chiefOwnerId;

        /// <summary>Poids du chef (0 et plus).</summary>
        public float chiefWeight;
    }
}

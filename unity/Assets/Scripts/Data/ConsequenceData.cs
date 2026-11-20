// MARSEILLE — Data/ConsequenceData.cs
// Les conséquences de la vie (design/20-consequences.md) : jauges du joueur (Casier, Usure, Foyer,
// Double vie, exposition des proches), dette personnelle, étapes de la justice, et réputation / voisinage
// par quartier. Tout est abstrait : des jauges et des compteurs, jamais une méthode.
// Structs : réassigner après modification d'un élément de liste.

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>Réputation et voisinage d'un quartier tenu par le joueur.</summary>
    [System.Serializable]
    public struct QuarterStanding
    {
        /// <summary>Quartier concerné.</summary>
        public int quarterId;

        /// <summary>Réputation, de -100 (craint, haï) à +100 (aimé, protégé).</summary>
        public float reputation;

        /// <summary>Tension avec les riverains (0-100).</summary>
        public float voisinage;
    }

    /// <summary>Jauges de conséquence de la partie (version 5 de la sauvegarde).</summary>
    [System.Serializable]
    public struct ConsequenceData
    {
        /// <summary>Les valeurs de départ ont été posées depuis la configuration.</summary>
        public bool initialized;

        /// <summary>Casier (0-100) : monte par paliers, ne baisse presque jamais.</summary>
        public float casier;

        /// <summary>Usure du corps et du sommeil (0-100) : baisse au repos.</summary>
        public float usure;

        /// <summary>Solidité de la vie privée (0-100) : couple, mère, enfants.</summary>
        public float foyer;

        /// <summary>Double vie (0-100) : écart entre l'argent qui circule et la vie visible.</summary>
        public float doubleVie;

        /// <summary>Exposition moyenne des proches (0-100) : ce qu'ils paient à ta place.</summary>
        public float expositionProches;

        /// <summary>Dette personnelle (€) : avocat, soins, un proche en difficulté ; avec intérêts.</summary>
        public float dettePersonnelle;

        /// <summary>Gardes à vue subies depuis le début.</summary>
        public int gardesAVue;

        /// <summary>Condamnations depuis le début.</summary>
        public int condamnations;

        /// <summary>Semaines avant le procès en cours (0 = aucune affaire ouverte).</summary>
        public int semainesAvantProces;

        /// <summary>Semaines restantes de sursis (0 = aucun).</summary>
        public int semainesSursis;

        /// <summary>Semaines restantes de détention (0 = libre).</summary>
        public int semainesDetention;

        /// <summary>Semaines de suite avec un foyer très fragile.</summary>
        public int semainesFoyerFragile;

        /// <summary>Semaines depuis le dernier temps privé.</summary>
        public int semainesSansFamille;

        /// <summary>Repas de famille demandés cette semaine (consommés par la résolution).</summary>
        public int repasEnAttente;

        /// <summary>Le partenaire est parti (une seule fois).</summary>
        public bool partenaireParti;

        /// <summary>La dette personnelle a déjà dépassé le seuil d'alerte.</summary>
        public bool alerteDette;

        /// <summary>Réputation et voisinage par quartier.</summary>
        public List<QuarterStanding> standings;

        /// <summary>État initial (listes créées ; les valeurs viennent de la configuration au premier cycle).</summary>
        /// <returns>L'état vide.</returns>
        public static ConsequenceData Create()
        {
            return new ConsequenceData { standings = new List<QuarterStanding>() };
        }

        /// <summary>Index de la fiche d'un quartier, ou -1.</summary>
        /// <param name="quarterId">Quartier.</param>
        /// <returns>Index.</returns>
        public int IndexOfStanding(int quarterId)
        {
            if (standings == null) return -1;
            for (int i = 0; i < standings.Count; i++) if (standings[i].quarterId == quarterId) return i;
            return -1;
        }
    }
}

// MARSEILLE — Core/CycleLedger.cs
// Données de travail d'un cycle, partagées entre systèmes (non sauvegardées) :
// où sont les opérateurs, ce que les flux ont rapporté, le détail de l'économie.

using System.Collections.Generic;

namespace Marseille.Core
{
    /// <summary>Registre éphémère d'un cycle en cours de résolution.</summary>
    public sealed class CycleLedger
    {
        /// <summary>Quartiers où au moins un opérateur du joueur est présent et actif ce cycle.</summary>
        public readonly HashSet<int> quartersWithOperator = new HashSet<int>();

        /// <summary>Quartiers tenus (contrôle &gt; 0) au début du cycle, pour détecter les gains et les pertes.</summary>
        public readonly HashSet<int> quartersHeldAtStart = new HashSet<int>();

        /// <summary>Revenus des flux internationaux ce cycle (€), écrits par FluxSystem.</summary>
        public float fluxIncome;

        /// <summary>Coûts des flux internationaux ce cycle (€), écrits par FluxSystem.</summary>
        public float fluxCost;

        /// <summary>Revenus des quartiers ce cycle (€), écrits par EconomySystem.</summary>
        public float quarterIncome;

        /// <summary>Coût de maintien des quartiers ce cycle (€).</summary>
        public float maintenance;

        /// <summary>Entretien des opérateurs ce cycle (€).</summary>
        public float upkeep;

        /// <summary>Intérêts de la dette ce cycle (€).</summary>
        public float interest;

        /// <summary>Solde net du cycle (€).</summary>
        public float Net
        {
            get { return quarterIncome + fluxIncome - fluxCost - maintenance - upkeep - interest; }
        }
    }
}

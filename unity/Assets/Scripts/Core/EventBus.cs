// MARSEILLE — Core/EventBus.cs
// Publication / abonnement par événements C# : les systèmes et le GameManager publient, l'UI s'abonne.
// Exemple : EventBus.OnCycleResolved += result => { ... };  (penser à se désabonner dans OnDisable).

using System;
using Marseille.Data;

namespace Marseille.Core
{
    /// <summary>Bus d'événements statique, sans dépendance à l'UI.</summary>
    public static class EventBus
    {
        /// <summary>Un cycle vient d'être résolu.</summary>
        public static event Action<CycleResult> OnCycleResolved;

        /// <summary>L'état de la boucle a changé.</summary>
        public static event Action<GameState> OnStateChanged;

        /// <summary>Une partie vient d'être créée ou chargée : l'UI doit tout redessiner.</summary>
        public static event Action OnGameLoaded;

        /// <summary>Les données ont changé pendant la planification (affectation, recrutement, flux…).</summary>
        public static event Action OnDataChanged;

        /// <summary>Une ligne vient d'être ajoutée au journal.</summary>
        public static event Action<JournalEntry> OnJournalEntry;

        /// <summary>La partie est terminée.</summary>
        public static event Action<EndType> OnGameOver;

        /// <summary>Publie la fin d'un cycle.</summary>
        /// <param name="result">Résultat du cycle.</param>
        public static void RaiseCycleResolved(CycleResult result)
        {
            if (OnCycleResolved != null) OnCycleResolved(result);
        }

        /// <summary>Publie un changement d'état.</summary>
        /// <param name="state">Nouvel état.</param>
        public static void RaiseStateChanged(GameState state)
        {
            if (OnStateChanged != null) OnStateChanged(state);
        }

        /// <summary>Publie le chargement ou la création d'une partie.</summary>
        public static void RaiseGameLoaded()
        {
            if (OnGameLoaded != null) OnGameLoaded();
        }

        /// <summary>Publie une modification des données pendant la planification.</summary>
        public static void RaiseDataChanged()
        {
            if (OnDataChanged != null) OnDataChanged();
        }

        /// <summary>Publie une nouvelle ligne de journal.</summary>
        /// <param name="entry">Ligne ajoutée.</param>
        public static void RaiseJournalEntry(JournalEntry entry)
        {
            if (OnJournalEntry != null) OnJournalEntry(entry);
        }

        /// <summary>Publie la fin de partie.</summary>
        /// <param name="endType">Type de fin.</param>
        public static void RaiseGameOver(EndType endType)
        {
            if (OnGameOver != null) OnGameOver(endType);
        }

        /// <summary>Retire tous les abonnés (changement de scène, tests).</summary>
        public static void ClearAll()
        {
            OnCycleResolved = null;
            OnStateChanged = null;
            OnGameLoaded = null;
            OnDataChanged = null;
            OnJournalEntry = null;
            OnGameOver = null;
        }
    }
}

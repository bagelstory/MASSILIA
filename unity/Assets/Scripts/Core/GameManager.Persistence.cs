// MARSEILLE — Core/GameManager.Persistence.cs
// Suite de GameManager (classe partielle) : journal hors résolution, notification des changements,
// sauvegarde et cycle de vie de l'application mobile.

using System.Collections.Generic;
using Marseille.Data;

namespace Marseille.Core
{
    public partial class GameManager
    {
        /// <summary>Ajoute une ligne au journal en dehors de la résolution (actions de planification).</summary>
        /// <param name="text">Phrase courte.</param>
        /// <param name="type">Catégorie.</param>
        /// <param name="quarterId">Quartier concerné, ou -1.</param>
        public void AddJournal(string text, JournalType type, int quarterId)
        {
            if (data.journal == null) data.journal = new List<JournalEntry>();
            JournalEntry entry = JournalEntry.Create(data.currentCycle, text, type, quarterId);
            data.journal.Insert(0, entry);
            if (data.journal.Count > config.journalMaxEntries) data.journal.RemoveRange(config.journalMaxEntries, data.journal.Count - config.journalMaxEntries);
            EventBus.RaiseJournalEntry(entry);
        }

        /// <summary>À appeler après chaque modification de planification (affectation, recrutement…).</summary>
        public void NotifyDataChanged()
        {
            if (config.saveOnEveryChange) Save();
            EventBus.RaiseDataChanged();
        }

        /// <summary>Écrit la partie sur le disque.</summary>
        public void Save()
        {
            if (hasGame && config != null) SaveManager.Save(data, config);
        }

        /// <summary>Sauvegarde quand l'application passe en arrière-plan (mobile).</summary>
        /// <param name="paused">Vrai à la mise en pause.</param>
        private void OnApplicationPause(bool paused)
        {
            if (paused) Save();
        }

        /// <summary>Sauvegarde à la fermeture.</summary>
        private void OnApplicationQuit()
        {
            Save();
        }

        /// <summary>Libère le singleton.</summary>
        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}

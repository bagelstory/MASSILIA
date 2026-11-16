// MARSEILLE — Data/JournalEntry.cs
// Une ligne du journal : une phrase courte, son cycle, sa catégorie et le quartier concerné.

namespace Marseille.Data
{
    /// <summary>Ligne de journal (spec §12.1 JOURNAL : décisions, événements, changements).</summary>
    [System.Serializable]
    public struct JournalEntry
    {
        /// <summary>Cycle où l'événement a eu lieu.</summary>
        public int cycle;

        /// <summary>Texte d'une ligne maximum.</summary>
        public string text;

        /// <summary>Catégorie (couleur dans l'UI).</summary>
        public JournalType type;

        /// <summary>Quartier concerné, ou <see cref="Owners.None"/>.</summary>
        public int quarterId;

        /// <summary>Crée une ligne de journal.</summary>
        /// <param name="cycle">Cycle courant.</param>
        /// <param name="text">Texte court.</param>
        /// <param name="type">Catégorie.</param>
        /// <param name="quarterId">Quartier concerné, ou -1.</param>
        /// <returns>La ligne initialisée.</returns>
        public static JournalEntry Create(int cycle, string text, JournalType type, int quarterId)
        {
            return new JournalEntry { cycle = cycle, text = text, type = type, quarterId = quarterId };
        }
    }
}

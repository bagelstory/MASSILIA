// MARSEILLE — Core/CycleContext.cs
// Ce que chaque système reçoit pendant la résolution : la configuration, un hasard reproductible
// (même graine + même cycle = mêmes tirages, y compris après rechargement) et le rapport du cycle.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Core
{
    /// <summary>Contexte partagé par les systèmes pendant la résolution d'un cycle.</summary>
    public sealed class CycleContext
    {
        /// <summary>Paramètres de gameplay (ScriptableObject).</summary>
        public GameConfig Config { get; private set; }

        /// <summary>Cycle en cours de résolution.</summary>
        public int Cycle { get; private set; }

        /// <summary>Générateur aléatoire du cycle, dérivé de la graine de la partie.</summary>
        public System.Random Random { get; private set; }

        /// <summary>Lignes produites pendant ce cycle, dans l'ordre chronologique.</summary>
        public List<JournalEntry> Report { get; private set; }

        /// <summary>Registre de travail partagé entre systèmes pendant ce cycle.</summary>
        public CycleLedger Ledger { get; private set; }

        /// <summary>Crée le contexte d'un cycle.</summary>
        /// <param name="config">Configuration de jeu.</param>
        /// <param name="cycle">Cycle résolu.</param>
        /// <param name="seed">Graine de la partie.</param>
        public CycleContext(GameConfig config, int cycle, int seed)
        {
            Config = config;
            Cycle = cycle;
            Random = new System.Random(unchecked(seed * 31 + cycle * 7919));
            Report = new List<JournalEntry>();
            Ledger = new CycleLedger();
        }

        /// <summary>Ajoute une ligne au rapport du cycle.</summary>
        /// <param name="text">Phrase courte (une ligne).</param>
        /// <param name="type">Catégorie.</param>
        /// <param name="quarterId">Quartier concerné, ou -1.</param>
        public void Log(string text, JournalType type, int quarterId)
        {
            Report.Add(JournalEntry.Create(Cycle, text, type, quarterId));
        }

        /// <summary>Ajoute une ligne sans quartier associé.</summary>
        /// <param name="text">Phrase courte.</param>
        /// <param name="type">Catégorie.</param>
        public void Log(string text, JournalType type)
        {
            Log(text, type, Owners.None);
        }

        /// <summary>Tirage uniforme dans [0, 1[.</summary>
        /// <returns>Nombre aléatoire.</returns>
        public float Next01()
        {
            return (float)Random.NextDouble();
        }

        /// <summary>Tirage uniforme dans [min, max[.</summary>
        /// <param name="min">Borne basse.</param>
        /// <param name="max">Borne haute.</param>
        /// <returns>Nombre aléatoire.</returns>
        public float Range(float min, float max)
        {
            return min + (max - min) * Next01();
        }

        /// <summary>Vrai avec la probabilité donnée.</summary>
        /// <param name="probability">Probabilité entre 0 et 1.</param>
        /// <returns>Résultat du tirage.</returns>
        public bool Chance(float probability)
        {
            return Next01() < probability;
        }
    }
}

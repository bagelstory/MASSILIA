// MARSEILLE — Core/CycleManager.cs
// Orchestration d'un tour : appelle les systèmes enregistrés dans l'ordre de résolution,
// verse le rapport dans le journal, calcule les variations et passe au cycle suivant.

using System;
using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Core
{
    /// <summary>Gère la résolution des cycles (tour par tour, synchrone).</summary>
    public sealed class CycleManager
    {
        private readonly List<ICycleSystem> systems = new List<ICycleSystem>();

        /// <summary>Systèmes enregistrés, dans l'ordre où ils seront appelés.</summary>
        public IReadOnlyList<ICycleSystem> Systems
        {
            get { return systems; }
        }

        /// <summary>Ajoute un système à la fin de l'ordre de résolution.</summary>
        /// <param name="system">Système à appeler à chaque cycle.</param>
        public void Register(ICycleSystem system)
        {
            if (system == null) throw new ArgumentNullException("system");
            if (!systems.Contains(system)) systems.Add(system);
        }

        /// <summary>Retire tous les systèmes.</summary>
        public void Clear()
        {
            systems.Clear();
        }

        /// <summary>Résout un cycle complet.</summary>
        /// <param name="data">État de la partie, modifié en place.</param>
        /// <param name="config">Configuration de jeu.</param>
        /// <returns>Le résultat du cycle (variations, lignes, fin éventuelle).</returns>
        public CycleResult Resolve(ref GameData data, GameConfig config)
        {
            IndicatorData before = data.indicators;
            int cycle = data.currentCycle;
            CycleContext context = new CycleContext(config, cycle, data.randomSeed);

            for (int i = 0; i < systems.Count; i++)
            {
                try
                {
                    systems[i].Resolve(ref data, context);
                }
                catch (Exception e)
                {
                    // Un système en erreur ne doit pas bloquer la partie : on le signale et on continue.
                    Debug.LogException(e);
                    context.Log("Erreur interne dans " + systems[i].Name + ".", JournalType.Info);
                }
            }

            AppendToJournal(ref data, context.Report, config.journalMaxEntries);
            AppendToLog(ref data, cycle, before, config.logMaxEntries);

            CycleResult result = CycleResult.From(cycle, before, data.indicators, context.Report, data.endType);
            if (data.endType == EndType.None) data.currentCycle++;
            return result;
        }

        /// <summary>Verse les lignes du cycle dans le journal (plus récent en premier) et le limite en taille.</summary>
        /// <param name="data">État de la partie.</param>
        /// <param name="lines">Lignes du cycle, ordre chronologique.</param>
        /// <param name="maxEntries">Taille maximale du journal.</param>
        private static void AppendToJournal(ref GameData data, List<JournalEntry> lines, int maxEntries)
        {
            if (data.journal == null) data.journal = new List<JournalEntry>();
            for (int i = 0; i < lines.Count; i++) data.journal.Insert(0, lines[i]);
            if (maxEntries > 0 && data.journal.Count > maxEntries) data.journal.RemoveRange(maxEntries, data.journal.Count - maxEntries);
        }

        /// <summary>Ajoute une ligne de résumé chiffré à l'historique des cycles.</summary>
        /// <param name="data">État de la partie.</param>
        /// <param name="cycle">Cycle résolu.</param>
        /// <param name="before">Indicateurs avant résolution.</param>
        /// <param name="maxEntries">Taille maximale de l'historique.</param>
        private static void AppendToLog(ref GameData data, int cycle, IndicatorData before, int maxEntries)
        {
            if (data.log == null) data.log = new List<string>();
            IndicatorData now = data.indicators;
            data.log.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "C{0} terr={1:0.0} tres={2:0} ({3:+0;-0;0}) pres={4:0} stab={5:0} expo={6:0}",
                cycle, now.territory, now.treasury, now.treasury - before.treasury, now.pressure, now.stability, now.exposure));
            if (maxEntries > 0 && data.log.Count > maxEntries) data.log.RemoveRange(0, data.log.Count - maxEntries);
        }
    }
}

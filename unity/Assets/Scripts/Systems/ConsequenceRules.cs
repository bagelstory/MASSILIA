// MARSEILLE — Systems/ConsequenceRules.cs
// Règles pures des conséquences (design/20-consequences.md) : part des revenus perdue, durée d'une peine,
// libellés des jauges. Aucune méthode opérationnelle : des chiffres et des suites.

using Marseille.Config;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Systems
{
    /// <summary>Règles des conséquences de la vie.</summary>
    public static class ConsequenceRules
    {
        /// <summary>Part des revenus perdue cette semaine (gêne du corps, détention, boycott).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Part entre 0 et 0,9.</returns>
        public static float LostIncomeShare(GameData data, GameConfig config)
        {
            ConsequenceData l = data.life;
            float share = 0f;
            if (l.usure >= config.santeSeuilGene) share += config.santeMalusRendement;
            if (l.semainesDetention > 0) share += config.justicePerteDetention;
            share += config.repBoycott * BoycottRatio(data, config);
            return Mathf.Clamp(share, 0f, 0.9f);
        }

        /// <summary>Part des quartiers suivis où la réputation est sous le seuil de délation.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Part entre 0 et 1.</returns>
        public static float BoycottRatio(GameData data, GameConfig config)
        {
            if (data.life.standings == null || data.life.standings.Count == 0) return 0f;
            int bad = 0;
            for (int i = 0; i < data.life.standings.Count; i++) if (data.life.standings[i].reputation <= config.repSeuilDelation) bad++;
            return (float)bad / data.life.standings.Count;
        }

        /// <summary>Durée d'une peine ferme : plus le Casier est haut, plus elle monte.</summary>
        /// <param name="casier">Casier (0-100).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Semaines.</returns>
        public static int SentenceWeeks(float casier, GameConfig config)
        {
            int top = casier >= config.justiceSeuilPeineLongue ? config.justicePeineLongueMax : config.justicePeineCourteMax;
            return Mathf.RoundToInt(Mathf.Lerp(config.justicePeineMin, Mathf.Max(config.justicePeineMin, top), Mathf.Clamp01(casier / 100f)));
        }

        /// <summary>Libellé d'une jauge de 0 à 100 (plus c'est haut, plus c'est lourd).</summary>
        /// <param name="value">Valeur.</param>
        /// <returns>Mot court.</returns>
        public static string Level(float value)
        {
            if (value < 20f) return "léger";
            if (value < 45f) return "sensible";
            if (value < 70f) return "lourd";
            return "critique";
        }

        /// <summary>Lignes du bloc « Vie » du bilan.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Texte sur plusieurs lignes.</returns>
        public static string Describe(GameData data, GameConfig config)
        {
            ConsequenceData l = data.life;
            string text = "Âge " + config.AgeAt(data.currentCycle) + " ans · Foyer " + l.foyer.ToString("0") + " · Usure " + l.usure.ToString("0") + " (" + Level(l.usure) + ")"
                          + "\nCasier " + l.casier.ToString("0") + " · Double vie " + l.doubleVie.ToString("0") + " · Proches exposés " + l.expositionProches.ToString("0")
                          + "\nDette personnelle " + l.dettePersonnelle.ToString("N0") + " €";
            if (l.semainesDetention > 0) text += "\nDétention : encore " + l.semainesDetention + " semaines";
            else if (l.semainesAvantProces > 0) text += "\nProcès dans " + l.semainesAvantProces + " semaines";
            else if (l.semainesSursis > 0) text += "\nSursis : encore " + l.semainesSursis + " semaines";
            else if (l.partenaireParti) text += "\nTu vis seul.";
            return text;
        }
    }
}

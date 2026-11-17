// MARSEILLE — Config/GameConfig.Validation.cs
// Contrôle de cohérence de la configuration : appelé par Unity à chaque modification dans l'inspecteur
// (OnValidate) et utilisable par des tests (Validate). Ne corrige rien, signale seulement.

using System.Collections.Generic;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        /// <summary>Liste les incohérences de la configuration.</summary>
        /// <returns>Messages (liste vide si tout va bien).</returns>
        public List<string> Validate()
        {
            List<string> problems = new List<string>();
            foreach (OperatorType type in System.Enum.GetValues(typeof(OperatorType)))
                if (GetOperatorSettings(type) == null) problems.Add("Type d'opérateur sans paramètres : " + type);
            foreach (OperatorTask task in System.Enum.GetValues(typeof(OperatorTask)))
                if (task != OperatorTask.Rest && GetTaskSettings(task) == null) problems.Add("Tâche sans paramètres : " + task);
            CheckIds(problems);
            CheckRange(problems, "transitCycles", transitCycles.x, transitCycles.y);
            CheckRange(problems, "recruitCompetence", recruitCompetence.x, recruitCompetence.y);
            CheckRange(problems, "startingCompetence", startingCompetence.x, startingCompetence.y);
            CheckRange(problems, "zoneFactionPresence", zoneFactionPresence.x, zoneFactionPresence.y);
            CheckRange(problems, "borderFactionPresence", borderFactionPresence.x, borderFactionPresence.y);
            CheckRange(problems, "charbonIncomeRatio", charbonIncomeRatio.x, charbonIncomeRatio.y);
            CheckRange(problems, "charbonRiskThresholds", charbonRiskThresholds.x, charbonRiskThresholds.y);
            CheckRange(problems, "factionAllianceDuration", factionAllianceDuration.x, factionAllianceDuration.y);
            CheckRange(problems, "factionActionRandom", factionActionRandom.x, factionActionRandom.y);
            CheckRange(problems, "mapZoomRange", mapZoomRange.x, mapZoomRange.y);
            CheckRange(problems, "jailCycles", jailCycles.x, jailCycles.y);
            if (ombreTiers.x > ombreTiers.y || ombreTiers.y > ombreTiers.z || ombreTiers.z > ombreTiers.w) problems.Add("ombreTiers doit être croissant.");
            if (facades == null || facades.Length < 7) problems.Add("facades doit contenir 7 fiches (une par secteur).");
            if (facadeEveilControl > facadeEveilFall) problems.Add("facadeEveilControl devrait être ≤ facadeEveilFall.");
            if (contaminationThreshold > contaminationMax) problems.Add("contaminationThreshold devrait être ≤ contaminationMax.");
            if (startingOperators.Count > startingCapacity) problems.Add("Plus d'opérateurs de départ que de capacité.");
            if (securedControl <= accessNeighbourControl) problems.Add("securedControl devrait dépasser accessNeighbourControl.");
            if (charbonTakeControl < charbonOpenControl) problems.Add("charbonTakeControl devrait être ≥ charbonOpenControl.");
            if (mapUseGrid && mapGrid.x * mapGrid.y < 111) problems.Add("Grille de carte trop petite pour 111 quartiers.");
            if (gradeThresholds.Count < 4) problems.Add("gradeThresholds doit contenir 4 seuils (A, B, C, D).");
            bool anyStart = false;
            for (int i = 0; i < zones.Count; i++) anyStart |= zones[i].canBeStart;
            if (!anyStart) problems.Add("Aucune zone de départ possible.");
            return problems;
        }

        /// <summary>Identifiants uniques et références valides (zones, factions, flux, objectifs).</summary>
        /// <param name="problems">Liste complétée.</param>
        private void CheckIds(List<string> problems)
        {
            HashSet<int> zoneIds = new HashSet<int>(), factionIds = new HashSet<int>(), fluxIds = new HashSet<int>();
            HashSet<string> questIds = new HashSet<string>();
            for (int i = 0; i < factions.Count; i++)
            {
                if (factions[i].id < 1) problems.Add("Faction " + factions[i].name + " : identifiant < 1 (0 est le joueur).");
                if (!factionIds.Add(factions[i].id)) problems.Add("Identifiant de faction en double : " + factions[i].id);
            }
            for (int i = 0; i < zones.Count; i++)
            {
                if (!zoneIds.Add(zones[i].id)) problems.Add("Identifiant de zone en double : " + zones[i].id);
                if (zones[i].startingFactionId > 0 && !factionIds.Contains(zones[i].startingFactionId))
                    problems.Add("Zone " + zones[i].name + " : faction de départ inconnue " + zones[i].startingFactionId);
                CheckRange(problems, "charbonsPerQuarter (zone " + zones[i].id + ")", zones[i].charbonsPerQuarter.x, zones[i].charbonsPerQuarter.y);
            }
            for (int i = 0; i < fluxOffers.Count; i++)
                if (!fluxIds.Add(fluxOffers[i].id)) problems.Add("Identifiant de flux en double : " + fluxOffers[i].id);
            for (int i = 0; i < quests.Count; i++)
                if (!questIds.Add(quests[i].id) || quests[i].id.StartsWith("zone_")) problems.Add("Identifiant d'objectif invalide ou en double : " + quests[i].id);
            for (int i = 0; i < seasons.Count; i++)
                for (int z = 0; z < seasons[i].zoneIds.Count; z++)
                    if (!zoneIds.Contains(seasons[i].zoneIds[z])) problems.Add("Saison " + seasons[i].name + " : zone inconnue " + seasons[i].zoneIds[z]);
        }

        /// <summary>Vérifie qu'un intervalle (min, max) est dans l'ordre.</summary>
        /// <param name="problems">Liste complétée.</param>
        /// <param name="name">Nom du champ.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="max">Maximum.</param>
        private static void CheckRange(List<string> problems, string name, float min, float max)
        {
            if (min > max) problems.Add(name + " : minimum (" + min + ") supérieur au maximum (" + max + ").");
        }

        /// <summary>Signale les incohérences dans la console à chaque modification dans l'inspecteur.</summary>
        private void OnValidate()
        {
            List<string> problems = Validate();
            for (int i = 0; i < problems.Count; i++) Debug.LogWarning("GameConfig v" + ConfigVersion + " : " + problems[i], this);
        }
    }
}

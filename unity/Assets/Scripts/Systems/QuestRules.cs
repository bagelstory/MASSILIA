// MARSEILLE — Systems/QuestRules.cs
// Objectifs chiffrés (sans narration) : mesure de l'avancement, création des objectifs d'ouverture de zone
// (une zone verrouillée voisine d'une zone ouverte), création des objectifs fixes.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Règles des objectifs.</summary>
    public static class QuestRules
    {
        /// <summary>Préfixe des identifiants des objectifs d'ouverture de zone.</summary>
        public const string ZonePrefix = "zone_";

        /// <summary>Valeur actuelle mesurée par un critère.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="o">Critère.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Valeur (nombre, € ou %).</returns>
        public static float Progress(GameData data, QuestObjective o, GameConfig config)
        {
            int n = 0;
            switch (o.type)
            {
                case QuestObjectiveType.QuartersHeld:
                    for (int i = 0; i < data.quarters.Count; i++) if (data.quarters[i].control >= o.threshold) n++;
                    return n;
                case QuestObjectiveType.ZoneKnowledge:
                    for (int i = 0; i < data.quarters.Count; i++)
                        if (data.quarters[i].zoneId == o.zoneId && data.quarters[i].knowledgeLevel >= o.threshold) n++;
                    return n;
                case QuestObjectiveType.Knowledge:
                    for (int i = 0; i < data.quarters.Count; i++) if (data.quarters[i].knowledgeLevel >= o.threshold) n++;
                    return n;
                case QuestObjectiveType.OpenCharbons:
                    for (int i = 0; i < data.quarters.Count; i++)
                        for (int p = 0; p < data.quarters[i].pois.Count; p++)
                            if (data.quarters[i].pois[p].kind == PoiKind.Charbon && data.quarters[i].pois[p].IsProducingForPlayer()) n++;
                    return n;
                case QuestObjectiveType.Treasury:
                    return data.indicators.treasury;
                case QuestObjectiveType.SecuredQuarters:
                    for (int i = 0; i < data.quarters.Count; i++) if (TerritoryRules.Status(data.quarters[i], config) == QuarterStatus.Secured) n++;
                    return n;
                case QuestObjectiveType.Operators:
                    return OperatorRules.ActiveCount(data);
                case QuestObjectiveType.Territory:
                    return TerritoryRules.Territory(data);
                case QuestObjectiveType.ActiveFluxes:
                    return FluxActions.ActiveCount(data);
            }
            return 0f;
        }

        /// <summary>Tous les critères d'un objectif sont-ils remplis ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quest">Objectif.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Vrai si atteint.</returns>
        public static bool IsMet(GameData data, QuestData quest, GameConfig config)
        {
            for (int i = 0; i < quest.objectives.Count; i++)
                if (Progress(data, quest.objectives[i], config) < quest.objectives[i].target) return false;
            return true;
        }

        /// <summary>Ajoute les objectifs fixes de la configuration.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        public static void CreateFixedQuests(ref GameData data, GameConfig config)
        {
            for (int i = 0; i < config.quests.Count; i++)
            {
                QuestSettings s = config.quests[i];
                QuestData q = QuestData.Create(s.id, s.name, Owners.None, s.rewardMoney, s.rewardCapacity);
                q.objectives.Add(QuestObjective.Create(s.type, s.target, s.threshold, Owners.None));
                data.quests.Add(q);
            }
        }

        /// <summary>Crée l'objectif d'ouverture de chaque zone verrouillée voisine d'une zone ouverte, s'il manque.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        public static void EnsureZoneQuests(ref GameData data, GameConfig config)
        {
            int unlocked = 0;
            for (int z = 0; z < data.zones.Count; z++) if (data.zones[z].isUnlocked) unlocked++;
            int extra = System.Math.Max(0, unlocked - 1);
            foreach (int zoneId in ProposableZones(data))
            {
                string id = ZonePrefix + zoneId;
                int zi = data.IndexOfZone(zoneId);
                if (zi < 0 || IndexOf(data, id) >= 0) continue;
                ZoneData zone = data.zones[zi];
                QuestData q = QuestData.Create(id, "Ouvrir " + zone.name, zoneId, config.zoneQuestReward, 0);
                q.objectives.Add(QuestObjective.Create(QuestObjectiveType.QuartersHeld,
                    config.zoneQuestHeld.x + config.zoneQuestHeld.y * extra, config.zoneQuestHeld.z, Owners.None));
                q.objectives.Add(QuestObjective.Create(QuestObjectiveType.ZoneKnowledge, config.zoneQuestKnowledge, 1f, zoneId));
                q.objectives.Add(QuestObjective.Create(QuestObjectiveType.OpenCharbons,
                    config.zoneQuestCharbons.x + config.zoneQuestCharbons.y * extra, 0f, Owners.None));
                data.quests.Add(q);
            }
        }

        /// <summary>Zones verrouillées qui touchent une zone ouverte.</summary>
        /// <param name="data">Partie.</param>
        /// <returns>Identifiants de zones.</returns>
        public static HashSet<int> ProposableZones(GameData data)
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                if (!TerritoryRules.IsZoneUnlocked(data, q.zoneId)) continue;
                for (int n = 0; n < q.adjacentQuarters.Count; n++)
                {
                    int j = data.IndexOfQuarter(q.adjacentQuarters[n]);
                    if (j >= 0 && !TerritoryRules.IsZoneUnlocked(data, data.quarters[j].zoneId)) set.Add(data.quarters[j].zoneId);
                }
            }
            return set;
        }

        /// <summary>Position d'un objectif par identifiant.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="id">Identifiant.</param>
        /// <returns>Index, ou -1.</returns>
        public static int IndexOf(GameData data, string id)
        {
            for (int i = 0; i < data.quests.Count; i++) if (data.quests[i].id == id) return i;
            return -1;
        }
    }
}

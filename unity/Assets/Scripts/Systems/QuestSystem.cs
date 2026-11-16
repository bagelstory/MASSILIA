// MARSEILLE — Systems/QuestSystem.cs
// Après les événements : valide les objectifs atteints, donne les récompenses, ouvre les zones,
// puis propose les objectifs des nouvelles zones voisines.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Système des objectifs et de l'ouverture des zones.</summary>
    public sealed class QuestSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Quests"; } }

        /// <summary>Valide les objectifs atteints ce cycle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            QuestRules.EnsureZoneQuests(ref data, c);
            bool zoneOpened = false;
            for (int i = 0; i < data.quests.Count; i++)
            {
                QuestData q = data.quests[i];
                if (q.isCompleted || !QuestRules.IsMet(data, q, c)) continue;
                q.isCompleted = true;
                q.completedCycle = context.Cycle;
                data.quests[i] = q;
                data.indicators.treasury += q.rewardMoney;
                data.capacity += q.rewardCapacity;
                string reward = (q.rewardMoney > 0f ? " (+" + q.rewardMoney.ToString("0") + " €)" : "")
                                + (q.rewardCapacity > 0 ? " (capacité +" + q.rewardCapacity + ")" : "");
                if (q.zoneToUnlock != Owners.None)
                {
                    UnlockZone(ref data, q.zoneToUnlock, c, context);
                    zoneOpened = true;
                    reward += " : zone ouverte";
                }
                context.Log("Objectif atteint : " + q.name + reward + ".", JournalType.Quest);
            }
            if (zoneOpened) QuestRules.EnsureZoneQuests(ref data, c);
        }

        /// <summary>Ouvre une zone : ses quartiers deviennent repérés ; capacité +1 toutes les N zones.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="zoneId">Zone.</param>
        /// <param name="c">Configuration.</param>
        /// <param name="context">Contexte.</param>
        private static void UnlockZone(ref GameData data, int zoneId, GameConfig c, CycleContext context)
        {
            int zi = data.IndexOfZone(zoneId);
            if (zi < 0 || data.zones[zi].isUnlocked) return;
            ZoneData zone = data.zones[zi];
            zone.isUnlocked = true;
            data.zones[zi] = zone;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                if (data.quarters[i].zoneId != zoneId) continue;
                QuarterData q = data.quarters[i];
                q.knowledgeLevel = System.Math.Max(1, q.knowledgeLevel);
                data.quarters[i] = q;
            }
            int opened = 0;
            for (int z = 0; z < data.zones.Count; z++) if (data.zones[z].isUnlocked) opened++;
            if ((opened - 1) % c.zoneQuestCapacityEvery == 0)
            {
                data.capacity++;
                context.Log("Capacité +1 (" + data.capacity + ").", JournalType.Gain);
            }
        }
    }
}

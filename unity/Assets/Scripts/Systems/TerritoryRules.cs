// MARSEILLE — Systems/TerritoryRules.cs
// Règles de contrôle partagées (spec §3.3, §3.4) : statut d'un quartier, accès, gains et pertes de contrôle,
// présence des factions. Fonctions pures sur les structs : l'appelant réassigne la struct modifiée.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Statut d'un quartier (spec §3.3 et §12.2 pour les couleurs).</summary>
    public enum QuarterStatus
    {
        /// <summary>Contrôle 0 et présence adverse 0 (gris).</summary>
        Free = 0,
        /// <summary>Contrôle &gt; 0, pas de présence adverse (vert).</summary>
        Player = 1,
        /// <summary>Contrôle ≥ 80 % et présence adverse ≤ 20 % (vert franc).</summary>
        Secured = 2,
        /// <summary>Contrôle &gt; 0 et présence adverse &gt; 0 (orange).</summary>
        Contested = 3,
        /// <summary>Contrôle 0, présence adverse &gt; 0 (couleur de la faction).</summary>
        Faction = 4
    }

    /// <summary>Règles de territoire sans état.</summary>
    public static class TerritoryRules
    {
        private const float Epsilon = 0.5f;

        /// <summary>Statut d'un quartier.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Le statut.</returns>
        public static QuarterStatus Status(QuarterData q, GameConfig config)
        {
            if (q.control <= 0f && q.hostilePresence <= Epsilon) return QuarterStatus.Free;
            if (q.control >= config.securedControl && q.hostilePresence <= config.securedMaxHostile) return QuarterStatus.Secured;
            if (q.control > 0f && q.hostilePresence > Epsilon) return QuarterStatus.Contested;
            return q.control > 0f ? QuarterStatus.Player : QuarterStatus.Faction;
        }

        /// <summary>Indique si une zone est ouverte au joueur.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="zoneId">Zone.</param>
        /// <returns>Vrai si ouverte.</returns>
        public static bool IsZoneUnlocked(GameData data, int zoneId)
        {
            int i = data.IndexOfZone(zoneId);
            return i >= 0 && data.zones[i].isUnlocked;
        }

        /// <summary>Le joueur peut-il envoyer un opérateur faire cette tâche dans ce quartier ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="task">Tâche prévue (le Repérage entre dans une zone verrouillée depuis un voisin tenu).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Vrai si accessible.</returns>
        public static bool IsAccessible(GameData data, int quarterId, OperatorTask task, GameConfig config)
        {
            int i = data.IndexOfQuarter(quarterId);
            if (i < 0) return false;
            QuarterData q = data.quarters[i];
            if (task == OperatorTask.Scout && q.control <= 0f && HasNeighbourAbove(data, q, 0f)) return true;
            if (!IsZoneUnlocked(data, q.zoneId)) return false;
            if (q.control > 0f) return true;
            return HasNeighbourAbove(data, q, config.accessNeighbourControl - Epsilon);
        }

        /// <summary>Un voisin du quartier a-t-il un contrôle strictement supérieur au seuil ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="q">Quartier.</param>
        /// <param name="threshold">Seuil de contrôle.</param>
        /// <returns>Vrai si au moins un voisin dépasse le seuil.</returns>
        public static bool HasNeighbourAbove(GameData data, QuarterData q, float threshold)
        {
            for (int n = 0; n < q.adjacentQuarters.Count; n++)
            {
                int j = data.IndexOfQuarter(q.adjacentQuarters[n]);
                if (j >= 0 && data.quarters[j].control > threshold) return true;
            }
            return false;
        }

        /// <summary>Ajoute du contrôle : d'abord la part libre, puis aux dépens des factions (en partie).</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="amount">Contrôle visé.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Contrôle réellement gagné.</returns>
        public static float GainControl(ref QuarterData q, float amount, GameConfig config)
        {
            q.RecomputeHostilePresence();
            float before = q.control;
            float fromFree = System.Math.Min(amount, q.FreeShare());
            q.control += fromFree;
            float rest = amount - fromFree;
            if (rest > 0f && q.hostilePresence > 0f)
            {
                float taken = System.Math.Min(q.hostilePresence, rest * config.controlTakenFromHostile);
                RemoveHostile(ref q, taken);
                q.control += taken;
            }
            q.control = MathHelper.Clamp(q.control, 0f, 100f);
            return q.control - before;
        }

        /// <summary>Retire de la présence adverse, au prorata de chaque faction.</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="amount">Points de présence à retirer.</param>
        /// <returns>Présence réellement retirée.</returns>
        public static float RemoveHostile(ref QuarterData q, float amount)
        {
            q.RecomputeHostilePresence();
            float total = q.hostilePresence;
            if (total <= 0f || amount <= 0f) return 0f;
            float removed = System.Math.Min(total, amount);
            for (int i = 0; i < q.factionPresences.Count; i++)
            {
                FactionPresence p = q.factionPresences[i];
                p.value = System.Math.Max(0f, p.value - removed * p.value / total);
                q.factionPresences[i] = p;
            }
            q.factionPresences.RemoveAll(x => x.value <= 0.01f);
            q.RecomputeHostilePresence();
            return removed;
        }

        /// <summary>Une faction gagne de la présence : part libre, puis aux dépens du joueur et des factions non alliées.</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="factionId">Faction qui avance.</param>
        /// <param name="amount">Présence visée.</param>
        /// <param name="alliedFactions">Factions en trêve avec elle (non attaquées).</param>
        /// <returns>Contrôle perdu par le joueur.</returns>
        public static float AddFactionPresence(ref QuarterData q, int factionId, float amount, ICollection<int> alliedFactions)
        {
            q.RecomputeHostilePresence();
            float fromFree = System.Math.Min(amount, q.FreeShare());
            AddPresence(ref q, factionId, fromFree);
            float rest = amount - fromFree, playerLoss = 0f;
            if (rest <= 0f) return 0f;
            float targets = q.control;
            for (int i = 0; i < q.factionPresences.Count; i++)
            {
                FactionPresence p = q.factionPresences[i];
                if (p.factionId != factionId && !alliedFactions.Contains(p.factionId)) targets += p.value;
            }
            if (targets <= 0f) return 0f;
            float taken = System.Math.Min(targets, rest);
            playerLoss = taken * q.control / targets;
            q.control = System.Math.Max(0f, q.control - playerLoss);
            for (int i = 0; i < q.factionPresences.Count; i++)
            {
                FactionPresence p = q.factionPresences[i];
                if (p.factionId == factionId || alliedFactions.Contains(p.factionId)) continue;
                p.value = System.Math.Max(0f, p.value - taken * p.value / targets);
                q.factionPresences[i] = p;
            }
            AddPresence(ref q, factionId, taken);
            q.factionPresences.RemoveAll(x => x.value <= 0.01f);
            q.RecomputeHostilePresence();
            return playerLoss;
        }

        /// <summary>Ajoute de la présence brute à une faction (crée l'entrée si besoin).</summary>
        /// <param name="q">Quartier modifié.</param>
        /// <param name="factionId">Faction.</param>
        /// <param name="amount">Points ajoutés.</param>
        public static void AddPresence(ref QuarterData q, int factionId, float amount)
        {
            if (amount <= 0f) return;
            for (int i = 0; i < q.factionPresences.Count; i++)
            {
                if (q.factionPresences[i].factionId != factionId) continue;
                FactionPresence p = q.factionPresences[i];
                p.value += amount;
                q.factionPresences[i] = p;
                q.RecomputeHostilePresence();
                return;
            }
            q.factionPresences.Add(FactionPresence.Create(factionId, amount));
            q.RecomputeHostilePresence();
        }

        /// <summary>Part de la ville contrôlée par le joueur (moyenne des contrôles, 0-100).</summary>
        /// <param name="data">Partie.</param>
        /// <returns>Territoire en %.</returns>
        public static float Territory(GameData data)
        {
            if (data.quarters == null || data.quarters.Count == 0) return 0f;
            float total = 0f;
            for (int i = 0; i < data.quarters.Count; i++) total += data.quarters[i].control;
            return total / data.quarters.Count;
        }
    }
}

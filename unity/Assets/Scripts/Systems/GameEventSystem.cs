// MARSEILLE — Systems/GameEventSystem.cs
// Étape 7 de la résolution : saisons du cycle, opérations extérieures quand la pression est en alerte,
// arrestations, opérateurs écartés par une faction dominante (spec §10). Conséquences seulement.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système des événements (nommé ainsi pour ne pas entrer en conflit avec UnityEngine.EventSystems.EventSystem).</summary>
    public sealed class GameEventSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Events"; } }

        /// <summary>Tire les événements du cycle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            List<SeasonSettings> seasons = SeasonRules.Active(data.randomSeed, context.Cycle, context.Config);
            for (int i = 0; i < seasons.Count; i++)
                if (!seasons[i].evenWeeks) context.Log(seasons[i].name + " : revenus modifiés.", JournalType.Season);
            bool operation = OutsideOperation(ref data, context);
            for (int i = 0; i < data.operators.Count; i++) OperatorRisks(ref data, i, operation, context);
            data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure);
            data.indicators.territory = TerritoryRules.Territory(data);
        }

        /// <summary>Opération extérieure : descentes dans quelques quartiers tenus.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        /// <returns>Vrai si une opération a eu lieu.</returns>
        private static bool OutsideOperation(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            if (data.indicators.pressure <= c.pressureAlert || !context.Chance(c.outsideOperationChance)) return false;
            data.indicators.pressure += c.outsideOperationPressure;
            List<int> held = new List<int>();
            for (int i = 0; i < data.quarters.Count; i++) if (data.quarters[i].control > 0f) held.Add(i);
            for (int n = 0; n < c.outsideOperationQuarters && held.Count > 0; n++)
            {
                int pick = context.Random.Next(held.Count);
                QuarterData q = data.quarters[held[pick]];
                q.control = System.Math.Max(0f, q.control - c.outsideOperationControlLoss);
                data.quarters[held[pick]] = q;
                held.RemoveAt(pick);
            }
            context.Log("Opération extérieure : descentes dans tes quartiers, contrôle en baisse.", JournalType.Police);
            return true;
        }

        /// <summary>Risques d'un opérateur sur place : arrestation, ou mis à l'écart par une faction dominante.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="index">Index de l'opérateur.</param>
        /// <param name="operation">Une opération extérieure a lieu ce cycle.</param>
        /// <param name="context">Contexte.</param>
        private static void OperatorRisks(ref GameData data, int index, bool operation, CycleContext context)
        {
            GameConfig c = context.Config;
            OperatorData op = data.operators[index];
            if (!OperatorRules.IsOnSite(op)) return;
            int qi = data.IndexOfQuarter(op.currentQuarter);
            if (qi < 0) return;
            QuarterData q = data.quarters[qi];
            ZoneSettings zone = c.GetZoneSettings(q.zoneId);
            float police = zone != null ? zone.policeLevel : c.policeReference;
            float chance = c.arrestChance * (data.indicators.pressure / 100f) * (police / c.policeReference)
                           * (operation ? c.outsideOperationArrestFactor : 1f) * (op.GetTask() == OperatorTask.Rest ? c.arrestRestFactor : 1f);
            if (context.Chance(chance))
            {
                ShadowRules.Jail(ref data, index, "arrêté", context);
                return;
            }
            int dominant = q.DominantFaction();
            if (op.type == OperatorType.Force || dominant == Owners.None || q.control <= 0f) return;
            if (q.PresenceOf(dominant) > c.pushedOutPresence && context.Chance(c.pushedOutChance))
            {
                int fi = data.IndexOfFaction(dominant);
                OperatorRules.Lose(ref data, index, "écarté par " + (fi >= 0 ? data.factions[fi].name : "une équipe"), context);
            }
        }
    }
}

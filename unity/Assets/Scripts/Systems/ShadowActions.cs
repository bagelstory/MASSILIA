// MARSEILLE — Systems/ShadowActions.cs
// Actions de planification de l'ombre : donner l'ordre (dehors ou depuis le parloir) et placer l'argent.
// L'ordre n'est jamais montré : une carte, un coût, une dette. Le placement n'est qu'un chiffre de contamination.

using Marseille.Config;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Actions du joueur liées à l'ombre.</summary>
    public static class ShadowActions
    {
        /// <summary>Peut-on donner l'ordre contre la bande dominante de ce quartier ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Réussite, ou la raison du refus.</returns>
        public static ActionResult CanGiveOrder(GameData data, int quarterId, GameConfig config)
        {
            int qi = data.IndexOfQuarter(quarterId);
            if (qi < 0) return ActionResult.Fail("Quartier inconnu.");
            int factionId = ShadowRules.TargetFaction(data.quarters[qi]);
            if (factionId == Owners.None) return ActionResult.Fail("Aucune bande ici.");
            if (data.shadow.orders != null)
                for (int i = 0; i < data.shadow.orders.Count; i++)
                    if (data.shadow.orders[i].quarterId == quarterId) return ActionResult.Fail("Un ordre est déjà en route.");
            if (data.indicators.treasury < config.orderCost) return ActionResult.Fail("Pas assez d'argent.");
            return ActionResult.Success(string.Empty);
        }

        /// <summary>L'équipier incarcéré le plus fiable, s'il y en a un.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Index dans la liste des opérateurs, ou -1.</returns>
        public static int BestPrisoner(GameData data, GameConfig config)
        {
            int best = -1;
            float bestReliability = -1f;
            for (int i = 0; i < data.operators.Count; i++)
            {
                if (data.operators[i].status != OperatorStatus.Jailed) continue;
                float r = ShadowRules.Reliability(data.operators[i], config);
                if (r > bestReliability) { bestReliability = r; best = i; }
            }
            return best;
        }

        /// <summary>Donne l'ordre dehors : résolu au cycle suivant, sûr d'être compris.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult GiveOrder(ref GameData data, int quarterId, GameConfig config)
        {
            return Queue(ref data, quarterId, config, -1, 1f, config.orderDelay);
        }

        /// <summary>Donne l'ordre depuis le parloir : en retard, et parfois mal compris.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult GiveOrderFromPrison(ref GameData data, int quarterId, GameConfig config)
        {
            int pi = BestPrisoner(data, config);
            if (pi < 0) return ActionResult.Fail("Personne aux Baumettes.");
            OperatorData op = data.operators[pi];
            return Queue(ref data, quarterId, config, op.id, ShadowRules.Reliability(op, config), config.parloirDelay);
        }

        /// <summary>Met un ordre en attente et paie son coût.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="fromOperatorId">Équipier incarcéré, ou -1.</param>
        /// <param name="reliability">Chance d'être compris (0-1).</param>
        /// <param name="delay">Délai en cycles.</param>
        /// <returns>Résultat affiché.</returns>
        private static ActionResult Queue(ref GameData data, int quarterId, GameConfig config, int fromOperatorId, float reliability, int delay)
        {
            ActionResult check = CanGiveOrder(data, quarterId, config);
            if (!check.ok) return check;
            int qi = data.IndexOfQuarter(quarterId);
            int factionId = ShadowRules.TargetFaction(data.quarters[qi]);
            ShadowData s = data.shadow;
            if (s.orders == null) s.orders = new System.Collections.Generic.List<PendingOrder>();
            s.orders.Add(new PendingOrder
            {
                factionId = factionId,
                quarterId = quarterId,
                resolveCycle = data.currentCycle + delay,
                fromOperatorId = fromOperatorId,
                reliability = reliability
            });
            data.shadow = s;
            data.indicators.treasury -= config.orderCost;
            string how = fromOperatorId >= 0 ? "Le parloir a parlé. Dans " + delay + " semaines, peut-être." : "L'ordre est donné. On ne revient pas dessus.";
            return ActionResult.Success(data.quarters[qi].name + " : " + how);
        }

        /// <summary>Peut-on placer l'argent dans ce quartier ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Réussite, ou la raison du refus.</returns>
        public static ActionResult CanInject(GameData data, int quarterId, GameConfig config)
        {
            int qi = data.IndexOfQuarter(quarterId);
            if (qi < 0) return ActionResult.Fail("Quartier inconnu.");
            if (data.quarters[qi].control <= 0f) return ActionResult.Fail("Il faut tenir le quartier.");
            if (data.quarters[qi].contamination >= config.contaminationMax) return ActionResult.Fail("Le quartier n'absorbe plus rien.");
            if (data.indicators.treasury < config.injectAmount) return ActionResult.Fail("Pas assez d'argent.");
            return ActionResult.Success(string.Empty);
        }

        /// <summary>Place l'argent dans les commerces du quartier : l'exposition baisse, la contamination monte.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Inject(ref GameData data, int quarterId, GameConfig config)
        {
            ActionResult check = CanInject(data, quarterId, config);
            if (!check.ok) return check;
            int qi = data.IndexOfQuarter(quarterId);
            QuarterData q = data.quarters[qi];
            float value = System.Math.Max(1f, EconomyRules.Potential(q, config) * config.contaminationValueFactor);
            q.contamination = MathHelper.Clamp(q.contamination + config.injectAmount / value * 100f, 0f, config.contaminationMax);
            data.quarters[qi] = q;
            data.indicators.treasury -= config.injectAmount;
            data.indicators.exposure = MathHelper.Clamp100(data.indicators.exposure - config.injectExposureRelief);
            return ActionResult.Success(q.name + " : l'argent est placé. Les vitrines ont l'air propres.");
        }
    }
}

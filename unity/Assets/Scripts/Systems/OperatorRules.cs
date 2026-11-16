// MARSEILLE — Systems/OperatorRules.cs
// Règles et actions de planification des opérateurs (spec §4.4, §4.5) : efficacité, trajet,
// recrutement limité par la capacité (§7), affectation à un quartier et une tâche.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Règles des opérateurs, sans état.</summary>
    public static class OperatorRules
    {
        /// <summary>Type spécialiste d'une tâche (null pour le repos).</summary>
        /// <param name="task">Tâche.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Paramètres du type spécialiste, ou null.</returns>
        public static OperatorTypeSettings SpecialistOf(OperatorTask task, GameConfig config)
        {
            for (int i = 0; i < config.operatorTypes.Count; i++) if (config.operatorTypes[i].specialtyTask == task) return config.operatorTypes[i];
            return null;
        }

        /// <summary>Efficacité d'un opérateur sur une tâche : compétence, spécialité (50 % hors spécialité), fatigue.</summary>
        /// <param name="op">Opérateur.</param>
        /// <param name="task">Tâche.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Efficacité (1 = référence ; 0 pour le repos).</returns>
        public static float Efficiency(OperatorData op, OperatorTask task, GameConfig config)
        {
            if (task == OperatorTask.Rest) return 0f;
            float e = (float)op.competence / config.competenceReference;
            OperatorTypeSettings spec = SpecialistOf(task, config);
            if (spec == null || spec.type != op.type) e *= config.offSpecialtyEfficiency;
            if (op.fatigue >= config.fatigueSlowdownThreshold) e *= config.fatigueSlowdownFactor;
            return e;
        }

        /// <summary>Durée du trajet entre deux quartiers (0 sur place, voisin ou non voisin).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="from">Départ.</param>
        /// <param name="to">Arrivée.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Nombre de cycles.</returns>
        public static int TransitDuration(GameData data, int from, int to, GameConfig config)
        {
            if (from == to) return 0;
            int i = data.IndexOfQuarter(from);
            bool adjacent = i >= 0 && data.quarters[i].adjacentQuarters.Contains(to);
            return adjacent ? config.transitCycles.x : config.transitCycles.y;
        }

        /// <summary>Nombre d'opérateurs non perdus.</summary>
        /// <param name="data">Partie.</param>
        /// <returns>Effectif total.</returns>
        public static int ActiveCount(GameData data)
        {
            int n = 0;
            for (int i = 0; i < data.operators.Count; i++) if (data.operators[i].status != OperatorStatus.Lost) n++;
            return n;
        }

        /// <summary>Peut-on recruter ce type maintenant ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="type">Type voulu.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat (message = raison du refus).</returns>
        public static ActionResult CanRecruit(GameData data, OperatorType type, GameConfig config)
        {
            OperatorTypeSettings s = config.GetOperatorSettings(type);
            if (s == null) return ActionResult.Fail("Type inconnu.");
            if (data.endType != EndType.None) return ActionResult.Fail("Partie terminée.");
            if (ActiveCount(data) >= data.capacity) return ActionResult.Fail("Capacité atteinte (" + data.capacity + ").");
            if (data.indicators.treasury < s.recruitCost) return ActionResult.Fail("Trésorerie insuffisante.");
            return ActionResult.Success(string.Empty);
        }

        /// <summary>Recrute un opérateur dans le quartier de base.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="type">Type voulu.</param>
        /// <param name="competence">Compétence tirée par l'appelant (bornée par la configuration).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult Recruit(ref GameData data, OperatorType type, int competence, GameConfig config)
        {
            ActionResult check = CanRecruit(data, type, config);
            if (!check.ok) return check;
            OperatorTypeSettings s = config.GetOperatorSettings(type);
            data.indicators.treasury -= s.recruitCost;
            OperatorData op = Create(ref data, type, competence, data.baseQuarterId, config);
            data.indicators.stability = System.Math.Min(100f, data.indicators.stability + config.stabilityPerRecruit);
            return ActionResult.Success("Recrue : " + op.Label() + " (" + s.displayName + "), −" + s.recruitCost.ToString("0") + " €.");
        }

        /// <summary>Crée un opérateur (départ ou recrutement) et lui donne un surnom s'il en reste.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="type">Type.</param>
        /// <param name="competence">Compétence.</param>
        /// <param name="quarterId">Quartier de départ.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>L'opérateur ajouté.</returns>
        public static OperatorData Create(ref GameData data, OperatorType type, int competence, int quarterId, GameConfig config)
        {
            OperatorTypeSettings s = config.GetOperatorSettings(type);
            int comp = System.Math.Max(1, System.Math.Min(5, competence));
            OperatorData op = OperatorData.Create(data.nextOperatorId++, type, comp, quarterId, s != null ? s.upkeep : 0f);
            int nick = op.id - 1;
            if (nick >= 0 && nick < config.operatorNicknames.Count) { op.nickname = config.operatorNicknames[nick]; op.faceId = op.nickname.ToLowerInvariant(); }
            data.operators.Add(op);
            return op;
        }

        /// <summary>Affecte un opérateur à un quartier et une tâche pour le cycle à venir (spec §4.5).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="operatorId">Opérateur.</param>
        /// <param name="quarterId">Quartier visé.</param>
        /// <param name="task">Tâche.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult Assign(ref GameData data, int operatorId, int quarterId, OperatorTask task, GameConfig config)
        {
            int i = data.IndexOfOperator(operatorId);
            if (i < 0) return ActionResult.Fail("Opérateur inconnu.");
            OperatorData op = data.operators[i];
            if (op.status == OperatorStatus.Lost) return ActionResult.Fail("Opérateur perdu.");
            if (op.status == OperatorStatus.Unavailable) return ActionResult.Fail("Épuisé : au repos ce cycle.");
            if (quarterId != op.currentQuarter && !TerritoryRules.IsAccessible(data, quarterId, task, config))
                return ActionResult.Fail("Quartier inaccessible.");
            op.SetTask(task);
            int duration = TransitDuration(data, op.currentQuarter, quarterId, config);
            if (duration > 0)
            {
                op.destinationQuarter = quarterId;
                op.transitCycles = duration;
                op.status = OperatorStatus.InTransit;
            }
            else
            {
                op.destinationQuarter = Owners.None;
                op.transitCycles = 0;
                op.status = task == OperatorTask.Rest ? OperatorStatus.Available : OperatorStatus.Occupied;
            }
            data.operators[i] = op;
            TaskSettings t = config.GetTaskSettings(task);
            return ActionResult.Success(op.Label() + " : " + (t != null ? t.displayName : task.ToString()) + (duration > 0 ? " (trajet " + duration + " cycle(s))." : "."));
        }

        /// <summary>Perd définitivement un opérateur (arrêté, écarté) : stabilité en baisse, ligne au journal.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="index">Index de l'opérateur dans la liste.</param>
        /// <param name="reason">Raison courte (« arrêté »).</param>
        /// <param name="context">Contexte du cycle.</param>
        public static void Lose(ref GameData data, int index, string reason, CycleContext context)
        {
            OperatorData op = data.operators[index];
            if (op.status == OperatorStatus.Lost) return;
            op.status = OperatorStatus.Lost;
            op.SetTask(OperatorTask.Rest);
            op.destinationQuarter = Owners.None;
            data.operators[index] = op;
            data.indicators.stability = System.Math.Max(0f, data.indicators.stability - context.Config.stabilityPerOperatorLost);
            OperatorTypeSettings s = context.Config.GetOperatorSettings(op.type);
            context.Log(op.Label() + " (" + (s != null ? s.displayName : op.type.ToString()) + ") perdu : " + reason + ".", JournalType.OperatorLost, op.currentQuarter);
        }

        /// <summary>L'opérateur est-il sur place dans un quartier (ni en trajet, ni épuisé, ni perdu) ?</summary>
        /// <param name="op">Opérateur.</param>
        /// <returns>Vrai s'il agit ce cycle.</returns>
        public static bool IsOnSite(OperatorData op)
        {
            return op.status == OperatorStatus.Available || op.status == OperatorStatus.Occupied;
        }
    }
}

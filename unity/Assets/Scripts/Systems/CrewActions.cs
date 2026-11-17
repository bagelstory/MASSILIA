// MARSEILLE — Systems/CrewActions.cs
// Actions du joueur sur les postes d'un charbon : placer un équipier, le retirer, mise en place du départ.
// Règle : guetteur, vendeur et gérant sont attachés à UN charbon ; seule la tête peut en tenir plusieurs.

using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Placer ou retirer un équipier à un poste de charbon.</summary>
    public static class CrewActions
    {
        /// <summary>Peut-on placer cet équipier à ce poste ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="poiId">Charbon.</param>
        /// <param name="slot">Poste (0-4).</param>
        /// <param name="operatorId">Équipier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Réussite, ou la raison du refus.</returns>
        public static ActionResult CanAssign(GameData data, int poiId, int slot, int operatorId, GameConfig config)
        {
            int qi, pi;
            CrewRules.FindPoi(data, poiId, out qi, out pi);
            if (qi < 0) return ActionResult.Fail("Charbon inconnu.");
            PoiData poi = data.quarters[qi].pois[pi];
            if (poi.ownerId != Owners.Player) return ActionResult.Fail("Ce n'est pas ton charbon.");
            int current = CrewRules.GetSlot(poi, slot);
            if (current != CrewSlots.Empty && !(slot == CrewSlots.Head && current == CrewSlots.Player)) return ActionResult.Fail("Poste déjà tenu.");
            int oi = data.IndexOfOperator(operatorId);
            if (oi < 0) return ActionResult.Fail("Équipier inconnu.");
            OperatorData op = data.operators[oi];
            if (op.status == OperatorStatus.Lost || op.status == OperatorStatus.Jailed) return ActionResult.Fail("Indisponible.");
            if (slot == CrewSlots.Head)
            {
                if (op.role != CrewRole.None && op.role != CrewRole.Tete) return ActionResult.Fail("Attaché à un autre charbon.");
                if (CrewRules.HeadCount(data, op.id) >= config.headCapacity) return ActionResult.Fail("Il tient déjà " + config.headCapacity + " charbons.");
                return ActionResult.Success(string.Empty);
            }
            if (op.role != CrewRole.None) return ActionResult.Fail("Déjà attaché à un poste.");
            return ActionResult.Success(string.Empty);
        }

        /// <summary>Place un équipier à un poste : il rejoint le quartier du charbon et n'en bouge plus (sauf la tête).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="poiId">Charbon.</param>
        /// <param name="slot">Poste (0-4).</param>
        /// <param name="operatorId">Équipier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Assign(ref GameData data, int poiId, int slot, int operatorId, GameConfig config)
        {
            ActionResult check = CanAssign(data, poiId, slot, operatorId, config);
            if (!check.ok) return check;
            int qi, pi;
            CrewRules.FindPoi(data, poiId, out qi, out pi);
            QuarterData q = data.quarters[qi];
            q.pois[pi] = CrewRules.SetSlot(q.pois[pi], slot, operatorId);
            data.quarters[qi] = q;
            int oi = data.IndexOfOperator(operatorId);
            OperatorData op = data.operators[oi];
            op.role = CrewSlots.RoleOf(slot);
            if (slot != CrewSlots.Head)
            {
                op.poiId = poiId;
                op.currentQuarter = q.id;
                op.destinationQuarter = Owners.None;
                op.transitCycles = 0;
                op.SetTask(OperatorTask.Rest);
                op.status = op.status == OperatorStatus.Unavailable ? OperatorStatus.Unavailable : OperatorStatus.Available;
            }
            data.operators[oi] = op;
            return ActionResult.Success(op.Label() + " : " + CrewSlots.Label(slot) + " à " + q.pois[pi].name + ".");
        }

        /// <summary>Retire l'équipier d'un poste (la tête du joueur ne se retire pas).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="poiId">Charbon.</param>
        /// <param name="slot">Poste (0-4).</param>
        /// <returns>Résultat affiché.</returns>
        public static ActionResult Unassign(ref GameData data, int poiId, int slot)
        {
            int qi, pi;
            CrewRules.FindPoi(data, poiId, out qi, out pi);
            if (qi < 0) return ActionResult.Fail("Charbon inconnu.");
            QuarterData q = data.quarters[qi];
            int who = CrewRules.GetSlot(q.pois[pi], slot);
            if (who == CrewSlots.Empty) return ActionResult.Fail("Poste vide.");
            if (who == CrewSlots.Player) return ActionResult.Fail("C'est toi, la tête. Ça ne se délègue pas d'un clic.");
            q.pois[pi] = CrewRules.SetSlot(q.pois[pi], slot, CrewSlots.Empty);
            data.quarters[qi] = q;
            int oi = data.IndexOfOperator(who);
            if (oi >= 0)
            {
                OperatorData op = data.operators[oi];
                if (slot != CrewSlots.Head) { op.role = CrewRole.None; op.poiId = -1; }
                else if (CrewRules.HeadCount(data, op.id) == 0) op.role = CrewRole.None;
                data.operators[oi] = op;
            }
            return ActionResult.Success(CrewSlots.Label(slot) + " : le poste est vide.");
        }

        /// <summary>Le meilleur équipier libre pour un poste (compétence la plus haute).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="slot">Poste (0-4).</param>
        /// <param name="poiId">Charbon visé.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Id d'opérateur, ou -1 s'il n'y en a pas.</returns>
        public static int BestCandidate(GameData data, int slot, int poiId, GameConfig config)
        {
            int best = -1, bestCompetence = -1;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (!CanAssign(data, poiId, slot, op.id, config).ok) continue;
                if (op.competence > bestCompetence) { bestCompetence = op.competence; best = op.id; }
            }
            return best;
        }

        /// <summary>Met en place l'équipe de départ : vendeur, gérant, guetteurs, dans l'ordre, avec les équipiers libres.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="config">Configuration.</param>
        public static void AutoStaff(ref GameData data, GameConfig config)
        {
            int[] order = { CrewSlots.Vendeur, CrewSlots.Gerant, CrewSlots.Guetteur1, CrewSlots.Guetteur2 };
            for (int i = 0; i < data.quarters.Count; i++)
            {
                for (int p = 0; p < data.quarters[i].pois.Count; p++)
                {
                    PoiData poi = data.quarters[i].pois[p];
                    if (poi.kind != PoiKind.Charbon || poi.ownerId != Owners.Player) continue;
                    for (int k = 0; k < order.Length; k++)
                    {
                        int who = BestCandidate(data, order[k], poi.id, config);
                        if (who >= 0) Assign(ref data, poi.id, order[k], who, config);
                    }
                }
            }
        }
    }
}

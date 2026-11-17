// MARSEILLE — Systems/HierarchySystem.cs
// Étape « hiérarchie » (avant l'économie) : remet les postes en ordre (morts, arrêtés, charbons perdus),
// donne la tête du joueur aux charbons libres (dans la limite de sa capacité), fait peser les guetteurs
// manquants sur la pression, et désigne le chef de chaque arrondissement : le plus puissant de la zone.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système de la hiérarchie du terrain (design/15-hierarchie.md).</summary>
    public sealed class HierarchySystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Hierarchy"; } }

        /// <summary>Résout la hiérarchie du cycle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            if (data.arrondissements == null || data.arrondissements.Count == 0) CrewRules.BuildArrondissements(ref data);
            Validate(ref data);
            FillPlayerHeads(ref data, c);
            Pressure(ref data, c);
            Chiefs(ref data, context);
        }

        /// <summary>Vide les postes invalides (opérateur perdu, arrêté, charbon perdu) et libère les opérateurs orphelins.</summary>
        /// <param name="data">Partie.</param>
        private static void Validate(ref GameData data)
        {
            HashSet<int> placed = new HashSet<int>();
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                for (int p = 0; p < q.pois.Count; p++)
                {
                    PoiData poi = q.pois[p];
                    for (int s = 0; s < CrewSlots.Count; s++)
                    {
                        int who = CrewRules.GetSlot(poi, s);
                        if (who == CrewSlots.Empty) continue;
                        if (!SlotIsValid(data, poi, s, who)) poi = CrewRules.SetSlot(poi, s, CrewSlots.Empty);
                        else if (who >= 0) placed.Add(who);
                    }
                    q.pois[p] = poi;
                }
                data.quarters[i] = q;
            }
            for (int o = 0; o < data.operators.Count; o++)
            {
                OperatorData op = data.operators[o];
                if (op.role == CrewRole.None || placed.Contains(op.id)) continue;
                op.role = CrewRole.None;
                op.poiId = -1;
                data.operators[o] = op;
            }
        }

        /// <summary>Un poste tenu est-il encore valable ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="poi">Charbon.</param>
        /// <param name="slot">Poste.</param>
        /// <param name="who">Occupant (id d'opérateur ou tête du joueur).</param>
        /// <returns>Vrai si le poste reste tenu.</returns>
        private static bool SlotIsValid(GameData data, PoiData poi, int slot, int who)
        {
            if (poi.ownerId != Owners.Player) return false;
            if (who == CrewSlots.Player) return slot == CrewSlots.Head;
            int oi = data.IndexOfOperator(who);
            if (oi < 0) return false;
            OperatorData op = data.operators[oi];
            if (op.status == OperatorStatus.Lost || op.status == OperatorStatus.Jailed) return false;
            if (op.role != CrewSlots.RoleOf(slot)) return false;
            return slot == CrewSlots.Head || op.poiId == poi.id;
        }

        /// <summary>Le joueur est la tête des charbons sans tête, dans la limite de sa capacité.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private static void FillPlayerHeads(ref GameData data, GameConfig c)
        {
            int held = CrewRules.HeadCount(data, CrewSlots.Player);
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                for (int p = 0; p < q.pois.Count; p++)
                {
                    PoiData poi = q.pois[p];
                    if (poi.kind != PoiKind.Charbon || poi.ownerId != Owners.Player || poi.slotHead != CrewSlots.Empty || held >= c.headCapacity) continue;
                    poi.slotHead = CrewSlots.Player;
                    q.pois[p] = poi;
                    held++;
                }
                data.quarters[i] = q;
            }
        }

        /// <summary>Chaque guetteur manquant sur un charbon ouvert pèse sur la pression.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="c">Configuration.</param>
        private static void Pressure(ref GameData data, GameConfig c)
        {
            float add = 0f;
            for (int i = 0; i < data.quarters.Count; i++)
                for (int p = 0; p < data.quarters[i].pois.Count; p++)
                {
                    PoiData poi = data.quarters[i].pois[p];
                    if (poi.kind == PoiKind.Charbon && poi.IsProducingForPlayer()) add += CrewRules.MissingGuetteurs(poi) * c.crewMissingPressure;
                }
            if (add > 0f) data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + add);
        }

        /// <summary>Désigne le chef de chaque arrondissement : le plus fort poids de charbons ouverts, pondéré par le contrôle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Chiefs(ref GameData data, CycleContext context)
        {
            for (int a = 0; a < data.arrondissements.Count; a++)
            {
                ArrondissementData arr = data.arrondissements[a];
                Dictionary<int, float> weights = new Dictionary<int, float>();
                for (int i = 0; i < data.quarters.Count; i++)
                {
                    QuarterData q = data.quarters[i];
                    if (q.arrondissement != arr.id) continue;
                    for (int p = 0; p < q.pois.Count; p++)
                    {
                        PoiData poi = q.pois[p];
                        if (poi.kind != PoiKind.Charbon || !poi.isOpen || poi.ownerId == Owners.None) continue;
                        float share = poi.ownerId == Owners.Player ? q.control : q.PresenceOf(poi.ownerId);
                        float w;
                        weights.TryGetValue(poi.ownerId, out w);
                        weights[poi.ownerId] = w + share / 100f;
                    }
                }
                int chief = Owners.None;
                float best = 0f;
                foreach (KeyValuePair<int, float> kv in weights)
                {
                    if (kv.Value <= best + 0.0001f) continue;
                    best = kv.Value;
                    chief = kv.Key;
                }
                float incumbent;
                if (weights.TryGetValue(arr.chiefOwnerId, out incumbent) && incumbent > 0f && incumbent >= best - 0.0001f)
                {
                    chief = arr.chiefOwnerId;
                    best = incumbent;
                }
                if (chief != arr.chiefOwnerId && (arr.chiefOwnerId != Owners.None || chief == Owners.Player))
                    context.Log(ChiefLine(data, arr.id, chief), JournalType.Faction);
                arr.chiefOwnerId = chief;
                arr.chiefWeight = chief == Owners.None ? 0f : best;
                data.arrondissements[a] = arr;
            }
        }

        /// <summary>Ligne de journal quand le chef d'un arrondissement change.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="arrondissement">Numéro.</param>
        /// <param name="chief">Nouveau chef.</param>
        /// <returns>Une ligne.</returns>
        private static string ChiefLine(GameData data, int arrondissement, int chief)
        {
            string place = "Dans le " + CrewRules.ArrondissementLabel(arrondissement) + " : ";
            if (chief == Owners.Player) return place + "c'est toi qu'on appelle, maintenant. Ça ne se dit pas, ça se voit à la façon dont les gens baissent les yeux.";
            if (chief == Owners.None) return place + "plus personne ne tient l'arrondissement. Ça ne dure jamais.";
            return place + CrewRules.ChiefName(data, chief) + " est devenu le plus fort. Les autres le savent avant qu'on le dise.";
        }
    }
}

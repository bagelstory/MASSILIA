// MARSEILLE — Systems/EconomyActions.cs
// Actions économiques de planification (spec §5.2) : investir dans un quartier, prendre, ouvrir ou fermer
// un charbon. Plus une prévision du prochain cycle pour le tableau de bord.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Prévision du solde du prochain cycle (hors flux et événements).</summary>
    public struct EconomyForecast
    {
        /// <summary>Revenus des quartiers.</summary>
        public float income;
        /// <summary>Maintien des quartiers.</summary>
        public float maintenance;
        /// <summary>Entretien des opérateurs.</summary>
        public float upkeep;
        /// <summary>Intérêts de la dette.</summary>
        public float interest;
        /// <summary>Solde prévu.</summary>
        public float Net { get { return income - maintenance - upkeep - interest; } }
    }

    /// <summary>Actions économiques du joueur.</summary>
    public static class EconomyActions
    {
        /// <summary>Calcule la prévision du prochain cycle avec l'état actuel.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>La prévision.</returns>
        public static EconomyForecast Forecast(GameData data, GameConfig config)
        {
            EconomyForecast f = new EconomyForecast();
            HashSet<int> pressure = EconomyRules.PressureQuarters(data);
            Dictionary<int, float> reductions = EconomyRules.MaintenanceReductions(data, config);
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                f.income += EconomyRules.QuarterIncome(q, pressure.Contains(q.id), config);
                float r;
                reductions.TryGetValue(q.id, out r);
                f.maintenance += EconomyRules.QuarterMaintenance(q, r, config);
            }
            f.upkeep = EconomyRules.Upkeep(data, config);
            f.interest = data.debt * config.debtInterest;
            return f;
        }

        /// <summary>Coût du prochain niveau d'investissement d'un quartier.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Coût en €.</returns>
        public static float InvestCost(QuarterData q, GameConfig config)
        {
            float potential = EconomyRules.Potential(q, config);
            return System.Math.Max(config.investMinimumBase, potential * config.investCostRatio) * (1 + q.investmentLevel);
        }

        /// <summary>Investit dans un quartier tenu (spec §5.2 : revenu et maintien augmentent).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult Invest(ref GameData data, int quarterId, GameConfig config)
        {
            int i = data.IndexOfQuarter(quarterId);
            if (i < 0) return ActionResult.Fail("Quartier inconnu.");
            QuarterData q = data.quarters[i];
            if (q.control <= 0f) return ActionResult.Fail("Quartier non tenu.");
            float cost = InvestCost(q, config);
            if (data.indicators.treasury < cost) return ActionResult.Fail("Trésorerie insuffisante (" + cost.ToString("0") + " €).");
            data.indicators.treasury -= cost;
            q.investmentLevel++;
            data.quarters[i] = q;
            return ActionResult.Success(q.name + " : niveau " + q.investmentLevel + ", −" + cost.ToString("0") + " €.");
        }

        /// <summary>Coût pour prendre un charbon : libre (ouverture) ou tenu par une faction (prise).</summary>
        /// <param name="poi">Charbon.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Coût en €.</returns>
        public static float CharbonCost(PoiData poi, GameConfig config)
        {
            float ratio = Owners.IsFaction(poi.ownerId) ? config.charbonTakeCostRatio : config.charbonOpenCostRatio;
            return poi.revenue * ratio;
        }

        /// <summary>Le joueur peut-il prendre ce charbon ?</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="poiId">Charbon.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat (message = raison du refus).</returns>
        public static ActionResult CanTakeCharbon(GameData data, int quarterId, int poiId, GameConfig config)
        {
            int i = data.IndexOfQuarter(quarterId);
            if (i < 0) return ActionResult.Fail("Quartier inconnu.");
            QuarterData q = data.quarters[i];
            int p = IndexOfPoi(q, poiId);
            if (p < 0 || q.pois[p].kind != PoiKind.Charbon) return ActionResult.Fail("Charbon inconnu.");
            PoiData poi = q.pois[p];
            if (poi.ownerId == Owners.Player) return ActionResult.Fail("Déjà à toi.");
            if (poi.seizedCycles > 0) return ActionResult.Fail("Fermé par la police (" + poi.seizedCycles + " cycles).");
            float needed = Owners.IsFaction(poi.ownerId) ? config.charbonTakeControl : config.charbonOpenControl;
            if (q.control < needed) return ActionResult.Fail("Contrôle insuffisant (" + needed.ToString("0") + " % requis).");
            if (data.indicators.treasury < CharbonCost(poi, config)) return ActionResult.Fail("Trésorerie insuffisante.");
            return ActionResult.Success(string.Empty);
        }

        /// <summary>Prend un charbon : il passe au joueur, ouvert ; une prise sur une faction fait monter la pression.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="poiId">Charbon.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult TakeCharbon(ref GameData data, int quarterId, int poiId, GameConfig config)
        {
            ActionResult check = CanTakeCharbon(data, quarterId, poiId, config);
            if (!check.ok) return check;
            int i = data.IndexOfQuarter(quarterId);
            QuarterData q = data.quarters[i];
            int p = IndexOfPoi(q, poiId);
            PoiData poi = q.pois[p];
            float cost = CharbonCost(poi, config);
            bool fromFaction = Owners.IsFaction(poi.ownerId);
            data.indicators.treasury -= cost;
            if (fromFaction)
            {
                data.indicators.pressure = System.Math.Min(100f, data.indicators.pressure + config.charbonTakeHeat.x);
                data.indicators.exposure = System.Math.Min(100f, data.indicators.exposure + config.charbonTakeHeat.y);
            }
            poi.ownerId = Owners.Player;
            poi.isOpen = true;
            q.pois[p] = poi;
            data.quarters[i] = q;
            return ActionResult.Success((fromFaction ? "Charbon repris : " : "Charbon ouvert : ") + poi.name + ", −" + cost.ToString("0") + " €.");
        }

        /// <summary>Ouvre ou ferme un charbon du joueur (fermé : pas de revenu, pas de chaleur).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="poiId">Charbon.</param>
        /// <returns>Résultat de l'action.</returns>
        public static ActionResult ToggleCharbon(ref GameData data, int quarterId, int poiId)
        {
            int i = data.IndexOfQuarter(quarterId);
            if (i < 0) return ActionResult.Fail("Quartier inconnu.");
            QuarterData q = data.quarters[i];
            int p = IndexOfPoi(q, poiId);
            if (p < 0 || q.pois[p].ownerId != Owners.Player) return ActionResult.Fail("Ce charbon n'est pas à toi.");
            PoiData poi = q.pois[p];
            poi.isOpen = !poi.isOpen;
            q.pois[p] = poi;
            data.quarters[i] = q;
            return ActionResult.Success(poi.name + (poi.isOpen ? " rouvert." : " fermé."));
        }

        /// <summary>Position d'un point d'intérêt dans la liste du quartier.</summary>
        /// <param name="q">Quartier.</param>
        /// <param name="poiId">Identifiant.</param>
        /// <returns>Index, ou -1.</returns>
        public static int IndexOfPoi(QuarterData q, int poiId)
        {
            for (int p = 0; p < q.pois.Count; p++) if (q.pois[p].id == poiId) return p;
            return -1;
        }
    }
}

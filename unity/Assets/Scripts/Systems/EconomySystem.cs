// MARSEILLE — Systems/EconomySystem.cs
// Étape 5 de la résolution : revenus des quartiers et des flux, maintien, entretien, dette, faillite.
// Les montants des flux sont déposés dans le CycleLedger par FluxSystem (étape 4), qui passe avant.

using System.Collections.Generic;
using Marseille.Config;
using Marseille.Core;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Système économique (spec §5).</summary>
    public sealed class EconomySystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Economy"; } }

        /// <summary>Encaisse, paie, gère la dette et compte les cycles à zéro.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            CycleLedger ledger = context.Ledger;
            HashSet<int> pressure = EconomyRules.PressureQuarters(data);
            Dictionary<int, float> reductions = EconomyRules.MaintenanceReductions(data, c);
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                ledger.quarterIncome += EconomyRules.QuarterIncome(q, pressure.Contains(q.id), c);
                float r;
                reductions.TryGetValue(q.id, out r);
                ledger.maintenance += EconomyRules.QuarterMaintenance(q, r, c);
            }
            ledger.upkeep = EconomyRules.Upkeep(data, c);
            ledger.interest = data.debt * c.debtInterest;
            ApplyNet(ref data, ledger.Net, context);
            if (ledger.Net < 0f)
                context.Log("Cycle déficitaire : " + ledger.Net.ToString("0") + " €.", JournalType.Loss);
        }

        /// <summary>Applique le solde du cycle à la trésorerie et à la dette.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="net">Solde du cycle.</param>
        /// <param name="context">Contexte.</param>
        private static void ApplyNet(ref GameData data, float net, CycleContext context)
        {
            GameConfig c = context.Config;
            float treasury = data.indicators.treasury + net;
            if (treasury < 0f)
            {
                data.debt += -treasury;
                treasury = 0f;
                context.Log("La caisse est vide : la dette monte à " + data.debt.ToString("0") + " €.", JournalType.Loss);
            }
            else if (data.debt > 0f && treasury > 0f)
            {
                float repay = System.Math.Min(data.debt, treasury * c.debtRepaymentShare);
                data.debt -= repay;
                treasury -= repay;
            }
            data.indicators.treasury = treasury;
            if (treasury <= 0f) data.zeroTreasuryCycles++;
            else data.zeroTreasuryCycles = 0;
            if (data.zeroTreasuryCycles > 0 && data.zeroTreasuryCycles < c.bankruptcyCycles)
                context.Log("Trésorerie à zéro (" + data.zeroTreasuryCycles + "/" + c.bankruptcyCycles + ").", JournalType.Loss);
        }
    }
}

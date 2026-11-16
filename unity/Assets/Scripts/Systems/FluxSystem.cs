// MARSEILLE — Systems/FluxSystem.cs
// Étape 4 de la résolution : chaque flux actif réussit ou rate son cycle selon sa fiabilité, s'expose,
// devient instable, peut rompre. Les montants vont dans le CycleLedger, encaissés par EconomySystem.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système des flux internationaux (spec §8).</summary>
    public sealed class FluxSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Flux"; } }

        /// <summary>Résout tous les flux actifs.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            for (int i = 0; i < data.fluxes.Count; i++)
            {
                if (!data.fluxes[i].isActive) continue;
                FluxData flux = data.fluxes[i];
                ResolveOne(ref flux, ref data, context);
                data.fluxes[i] = flux;
            }
            data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure);
            data.indicators.exposure = MathHelper.Clamp100(data.indicators.exposure);
        }

        /// <summary>Un cycle d'un flux : livraison, exposition, instabilité, rupture éventuelle.</summary>
        /// <param name="flux">Flux modifié.</param>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void ResolveOne(ref FluxData flux, ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            flux.activeCycles++;
            float exposureGain = flux.volume * c.fluxExposurePerVolume;
            data.indicators.exposure += exposureGain * c.fluxPlayerExposureShare;
            flux.exposure = MathHelper.Clamp100(flux.exposure + exposureGain);
            flux.instability = MathHelper.Clamp100(flux.instability + c.fluxInstabilityPerCycle);
            context.Ledger.fluxCost += flux.costPerCycle;
            if (context.Next01() * 100f < flux.reliability)
            {
                context.Ledger.fluxIncome += flux.potentialRevenue;
                flux.lastCycleSucceeded = true;
            }
            else
            {
                flux.lastCycleSucceeded = false;
                flux.reliability = MathHelper.Clamp100(flux.reliability - c.fluxFailedReliabilityLoss);
                context.Log(flux.name + " : cycle raté, pas de revenu.", JournalType.Flux);
            }
            if (context.Chance(flux.instability / c.fluxRuptureDivisor)) Rupture(ref flux, ref data, context);
            else if (flux.reliability < c.fluxPartnerLostReliability)
            {
                flux.isActive = false;
                flux.financialCommitment = 0f;
                context.Log("Le partenaire de " + flux.name + " disparaît. Flux perdu.", JournalType.Flux);
            }
        }

        /// <summary>Rupture : engagement perdu (une part passe en dette), pression, parfois un opérateur Logistique perdu.</summary>
        /// <param name="flux">Flux modifié.</param>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Rupture(ref FluxData flux, ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            context.Log("Rupture : " + flux.name + ". " + flux.financialCommitment.ToString("0") + " € perdus, la pression monte.", JournalType.Flux);
            data.debt += flux.financialCommitment * c.fluxRuptureDebtShare;
            data.indicators.pressure += c.fluxRupturePressure;
            flux.isActive = false;
            flux.financialCommitment = 0f;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.type != OperatorType.Logistics || !OperatorRules.IsOnSite(op)) continue;
                if (context.Chance(c.fluxRuptureOperatorLoss)) OperatorRules.Lose(ref data, i, "arrêté après la rupture du flux", context);
                break;
            }
        }
    }
}

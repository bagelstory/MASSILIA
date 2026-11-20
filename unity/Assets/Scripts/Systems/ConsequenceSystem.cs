// MARSEILLE — Systems/ConsequenceSystem.cs
// Étape « conséquences » de la résolution (après l'ombre, avant les façades) : le corps, la famille, la justice,
// l'argent, les quartiers. Une semaine = une dérive. On montre ce que ça coûte, jamais comment on fait.
// La justice est dans ConsequenceSystem.Justice.cs, l'argent et les quartiers dans ConsequenceSystem.Money.cs.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;
using UnityEngine;

namespace Marseille.Systems
{
    /// <summary>Système des conséquences de la vie (design/20-consequences.md).</summary>
    public sealed partial class ConsequenceSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Consequence"; } }

        /// <summary>Résout les conséquences du cycle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            if (l.standings == null) l.standings = new System.Collections.Generic.List<QuarterStanding>();
            if (!l.initialized) { l.foyer = c.familleFoyerDepart; l.initialized = true; }
            data.life = l;
            Justice(ref data, context);
            Body(ref data, context);
            Family(ref data, context);
            Money(ref data, context);
            Quarters(ref data, context);
        }

        /// <summary>Le corps : usure de l'âge et de la tension, repos, alerte médicale.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Body(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            float weeks = Mathf.Max(1, c.weeksPerYear);
            float yearly = c.AgeAt(context.Cycle) >= c.santeAgeBascule ? c.santeUsureAn.y : c.santeUsureAn.x;
            float pressure = data.indicators.pressure;
            float gain = yearly / weeks + c.santeUsureTension * Mathf.Max(0f, pressure - c.santeTensionBase);
            if (l.semainesDetention > 0) gain += c.justiceDetentionUsureAn / weeks;
            if (l.foyer >= c.santeFoyerAbri) gain *= 1f - c.santeFoyerAbriPart;
            l.usure += gain;
            if (pressure < c.santeCalmePression && l.semainesDetention == 0) l.usure -= c.santeRepos;
            l.usure = MathHelper.Clamp100(l.usure);
            if (l.usure >= c.santeSeuilAlerte)
            {
                if (context.Chance(c.santeAlerteChance))
                {
                    context.Log("Le médecin dit repos. Tu entends : trois semaines. Le quartier entend : une absence.", JournalType.Loss);
                    data.indicators.stability = MathHelper.Clamp100(data.indicators.stability - 2f);
                }
            }
            data.life = l;
        }

        /// <summary>La famille : soins de la mère, temps privé, usure du lien, départ du partenaire.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Family(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            if (c.AgeAt(context.Cycle) >= c.familleMereAge) Pay(ref data, c.familleMereCoutMois * 12f / Mathf.Max(1, c.weeksPerYear));
            if (l.repasEnAttente > 0)
            {
                l.foyer += c.familleRepas;
                l.semainesSansFamille = 0;
                l.repasEnAttente = 0;
            }
            else
            {
                l.semainesSansFamille++;
                if (l.semainesSansFamille >= c.familleAbsenceSeuil && l.semainesDetention == 0) l.foyer -= c.familleAbsence;
            }
            if (l.expositionProches >= c.expSeuilEloignement) l.foyer -= c.expPerteFoyer;
            l.foyer = MathHelper.Clamp100(l.foyer);
            if (l.foyer < c.familleFoyerBas && context.Chance(0.03f))
                context.Log("Tu penses à partir. Pas pour toujours. Juste ailleurs.", JournalType.Info);
            l.semainesFoyerFragile = l.foyer < c.familleFoyerFragile ? l.semainesFoyerFragile + 1 : 0;
            if (l.semainesFoyerFragile >= c.familleDepartSemaines && !l.partenaireParti)
            {
                l.partenaireParti = true;
                data.indicators.stability = MathHelper.Clamp100(data.indicators.stability - c.familleDepartStabilite);
                context.Log("La maison est silencieuse. Les affaires de l'armoire ont disparu, pas la poussière.", JournalType.Loss);
            }
            data.life = l;
        }

        /// <summary>Paie une dépense : la trésorerie d'abord, le reste en dette de partie.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="amount">Montant (€).</param>
        private static void Pay(ref GameData data, float amount)
        {
            float paid = System.Math.Min(data.indicators.treasury, amount);
            data.indicators.treasury -= paid;
            data.debt += amount - paid;
        }
    }
}

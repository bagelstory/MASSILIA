// MARSEILLE — Systems/ConsequenceSystem.Money.cs
// L'argent et les quartiers (design/20 §4 à §8) : double vie, exposition des proches, dette personnelle, revenus
// perdus, réputation et voisinage par quartier. Aucun procédé : seulement ce que l'écart fait à la vie visible.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;
using UnityEngine;

namespace Marseille.Systems
{
    public sealed partial class ConsequenceSystem
    {
        /// <summary>Double vie, proches, dette personnelle, revenus perdus.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Money(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            float income = EconomyActions.Forecast(data, c).income;
            int facades = data.shadow.facades == null ? 0 : data.shadow.facades.Count;
            l.doubleVie = MathHelper.Clamp100(l.doubleVie + income / c.duaRevenuParPoint - facades * c.duaFacadeBaisse);
            if (l.doubleVie >= c.duaSeuilEnquete)
            {
                data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + c.duaEnquetePression);
                if (context.Chance(0.05f)) context.Log("Tu as payé comptant. Le vendeur te regarde un peu plus longtemps qu'il ne faut.", JournalType.Police);
            }
            if (l.doubleVie >= c.duaSeuilRuine)
            {
                data.indicators.stability = MathHelper.Clamp100(data.indicators.stability - 2f);
                if (context.Chance(0.1f)) context.Log("La vie normale craque : le compte est examiné, le bail est relu.", JournalType.Loss);
            }
            l.expositionProches = MathHelper.Clamp100(l.expositionProches + (l.doubleVie > 50f ? c.expMontee : -c.expRetombee));
            if (l.expositionProches >= c.expSeuilEloignement) l.dettePersonnelle += c.detteProche;
            l.dettePersonnelle *= 1f + c.detteTaux;
            float repay = Mathf.Min(l.dettePersonnelle, data.indicators.treasury * c.detteRemboursementPart);
            data.indicators.treasury -= repay;
            l.dettePersonnelle -= repay;
            if (l.dettePersonnelle >= c.detteSeuilAlerte && !l.alerteDette)
                context.Log("Le solde a bougé de 400 €. Ce chiffre n'a pas de visage, mais il a un nom, et tu le connais.", JournalType.Loss);
            if (l.dettePersonnelle >= c.detteSeuilAlerte) l.alerteDette = true;
            else if (l.dettePersonnelle < c.detteSeuilAlerte * 0.8f) l.alerteDette = false;
            data.life = l;
            float lost = income * ConsequenceRules.LostIncomeShare(data, c);
            if (lost > 0f) data.indicators.treasury -= Mathf.Min(data.indicators.treasury, lost);
        }

        /// <summary>Réputation et voisinage de chaque quartier tenu.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Quarters(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                if (q.control >= c.repControlMin && l.IndexOfStanding(q.id) < 0) l.standings.Add(new QuarterStanding { quarterId = q.id });
            }
            for (int i = 0; i < l.standings.Count; i++)
            {
                QuarterStanding s = l.standings[i];
                int qi = data.IndexOfQuarter(s.quarterId);
                if (qi < 0) continue;
                QuarterData q = data.quarters[qi];
                s.reputation = Mathf.MoveTowards(s.reputation, 0f, c.repDecay);
                if (q.fear >= c.repPeurSeuil) s.reputation -= c.repPeurPerte;
                bool noisy = false;
                for (int p = 0; p < q.pois.Count; p++) if (q.pois[p].IsProducingForPlayer() && CrewRules.MissingGuetteurs(q.pois[p]) > 0) noisy = true;
                s.voisinage += noisy ? c.voiBruit : -c.voiCalme;
                s.reputation = MathHelper.Clamp(s.reputation, -100f, 100f);
                s.voisinage = MathHelper.Clamp100(s.voisinage);
                l.standings[i] = s;
                if (s.voisinage >= c.voiPetition && context.Chance(0.1f))
                {
                    data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + 2f);
                    context.Log("Pétition : 42 signatures. Tu en connais 41.", JournalType.Police, q.id);
                }
                else if (s.reputation <= c.repSeuilDelation && context.Chance(0.05f))
                {
                    data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + 1f);
                    context.Log("Le boulanger a changé de trottoir en te voyant. C'est la boulangerie qui perd.", JournalType.Loss, q.id);
                }
                else if (s.reputation >= c.repSeuilTemoin) data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure - 0.3f);
            }
            data.life = l;
        }
    }
}

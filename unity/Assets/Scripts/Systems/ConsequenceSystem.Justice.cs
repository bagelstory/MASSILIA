// MARSEILLE — Systems/ConsequenceSystem.Justice.cs
// La justice (design/20 §1) : garde à vue, affaire, procès, sursis, détention. Abstrait : des délais, un casier,
// une conséquence sur la vie. Pendant une détention, le jeu continue (parloir) ; les revenus baissent.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    public sealed partial class ConsequenceSystem
    {
        /// <summary>Avance la justice d'une semaine.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Justice(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            float pressure = data.indicators.pressure;
            if (l.semainesDetention > 0)
            {
                l.semainesDetention--;
                if (c.justiceParloirCadence > 0 && context.Cycle % c.justiceParloirCadence == 0)
                {
                    l.foyer = MathHelper.Clamp100(l.foyer + c.justiceParloirFoyer);
                    l.usure = MathHelper.Clamp100(l.usure - c.justiceParloirUsure);
                    l.expositionProches = MathHelper.Clamp100(l.expositionProches + c.justiceParloirExposition);
                }
                if (l.semainesDetention == 0) context.Log("Tu sors un mardi, avec un sac plastique. La ville a continué sans toi, et sa carte a changé.", JournalType.Police);
                data.life = l;
                return;
            }
            if (l.semainesSursis > 0)
            {
                l.semainesSursis--;
                if (pressure >= c.justiceSursisPression && context.Chance(c.justiceSursisChance))
                {
                    context.Log("Le sursis saute. Le mot « ferme » arrive avant la lettre.", JournalType.Police);
                    l.semainesSursis = 0;
                    data.life = l;
                    Sentence(ref data, context, true);
                    return;
                }
            }
            if (l.semainesAvantProces > 0)
            {
                l.semainesAvantProces--;
                data.life = l;
                if (l.semainesAvantProces == 0) Verdict(ref data, context);
                return;
            }
            data.life = l;
            if (pressure >= c.justiceGavSeuilPression && context.Chance(c.justiceGavChance * (1f + l.casier / 100f))) CustodyHold(ref data, context);
        }

        /// <summary>Garde à vue : une trace, parfois une affaire.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void CustodyHold(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            l.gardesAVue++;
            l.casier = MathHelper.Clamp100(l.casier + c.justiceCasierGav);
            if (context.Chance(c.justiceChanceAffaire))
            {
                l.semainesAvantProces = c.justiceDelaiProces;
                l.dettePersonnelle += context.Range(c.justiceAvocat.x, c.justiceAvocat.y);
                context.Log("Dossier renvoyé en correctionnelle. L'avocate envoie sa note. Elle est polie. Le montant, lui, ne l'est pas.", JournalType.Police);
            }
            else context.Log("Garde à vue levée à 6 h. Rien contre toi, pour l'instant. Le mot « pour l'instant » reste dans la pièce.", JournalType.Police);
            data.life = l;
        }

        /// <summary>Procès : relaxe, sursis ou peine ferme.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        private static void Verdict(ref GameData data, CycleContext context)
        {
            GameConfig c = context.Config;
            float roll = context.Next01();
            if (roll < c.justiceChanceRelaxe)
            {
                context.Log("Relaxe. Ta mère demande l'heure du train. Elle ne dit pas pour qui elle a prié.", JournalType.Police);
                return;
            }
            if (roll < c.justiceChanceRelaxe + c.justiceChanceSursis)
            {
                ConsequenceData l = data.life;
                l.casier = MathHelper.Clamp100(l.casier + c.justiceCasierSursis);
                l.semainesSursis = c.justiceSursisDuree;
                l.condamnations++;
                data.life = l;
                context.Log("Condamné avec sursis. Dehors, avec une règle : plus un faux pas pendant trois ans.", JournalType.Police);
                return;
            }
            Sentence(ref data, context, false);
        }

        /// <summary>Peine ferme : le casier monte, la détention commence.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte.</param>
        /// <param name="revoked">Vrai si elle vient d'un sursis révoqué.</param>
        private static void Sentence(ref GameData data, CycleContext context, bool revoked)
        {
            GameConfig c = context.Config;
            ConsequenceData l = data.life;
            l.casier = MathHelper.Clamp100(l.casier + c.justiceCasierFerme);
            l.semainesDetention = ConsequenceRules.SentenceWeeks(l.casier, c);
            l.condamnations++;
            data.life = l;
            context.Log((revoked ? "Peine ferme. " : "Condamné. ") + "Tu passes " + l.semainesDetention + " semaines là où le temps n'a pas la même longueur.", JournalType.Police);
        }
    }
}

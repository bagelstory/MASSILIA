// MARSEILLE — Systems/ShadowRules.cs
// Règles de l'ombre partagées par ShadowSystem, ShadowActions et l'interface (design/11-bible-ombre.md §4) :
// paliers de l'Ombre, force du réseau, fiabilité du parloir, incarcération, morts et peur.
// Rien de concret : une mort est une ligne au journal, de la peur et un trou dans le quartier.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Règles et calculs de l'ombre.</summary>
    public static class ShadowRules
    {
        /// <summary>Palier de l'Ombre : 0 Pénombre, 1 Crépuscule, 2 Nuit, 3 Nuit noire, 4 Ombre.</summary>
        /// <param name="ombre">Valeur de la jauge (0-100).</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Palier de 0 à 4.</returns>
        public static int Tier(float ombre, GameConfig config)
        {
            if (ombre >= config.ombreTiers.w) return 4;
            if (ombre >= config.ombreTiers.z) return 3;
            if (ombre >= config.ombreTiers.y) return 2;
            return ombre >= config.ombreTiers.x ? 1 : 0;
        }

        /// <summary>Nom d'un palier de l'Ombre.</summary>
        /// <param name="tier">Palier (0-4).</param>
        /// <returns>Nom affiché.</returns>
        public static string TierName(int tier)
        {
            switch (tier)
            {
                case 1: return "Crépuscule";
                case 2: return "Nuit";
                case 3: return "Nuit noire";
                case 4: return "Ombre";
                default: return "Pénombre";
            }
        }

        /// <summary>Force du réseau pour un ordre : compétence des équipiers Force libres, plus une base de 1.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Force (toujours ≥ 1).</returns>
        public static float PlayerForce(GameData data, GameConfig config)
        {
            float force = 1f;
            for (int i = 0; i < data.operators.Count; i++)
            {
                OperatorData op = data.operators[i];
                if (op.type == OperatorType.Force && OperatorRules.IsOnSite(op)) force += op.competence * config.orderForcePerCompetence;
            }
            return force;
        }

        /// <summary>Fiabilité d'un ordre donné depuis le parloir (0-1) : baisse avec le temps passé dedans.</summary>
        /// <param name="op">Équipier incarcéré.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Chance que l'ordre soit bien compris.</returns>
        public static float Reliability(OperatorData op, GameConfig config)
        {
            float r = 100f - config.parloirReliabilityLoss * op.jailServed;
            return MathHelper.Clamp(r, config.parloirReliabilityFloor, 100f) / 100f;
        }

        /// <summary>Nom affiché d'un équipier (surnom, sinon matricule).</summary>
        /// <param name="op">Équipier.</param>
        /// <returns>Libellé.</returns>
        public static string Name(OperatorData op)
        {
            return string.IsNullOrEmpty(op.nickname) ? op.Label() : op.nickname;
        }

        /// <summary>Envoie un équipier aux Baumettes (remplace la perte définitive en cas d'arrestation).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="index">Index de l'équipier.</param>
        /// <param name="reason">Raison courte.</param>
        /// <param name="context">Contexte du cycle.</param>
        public static void Jail(ref GameData data, int index, string reason, CycleContext context)
        {
            GameConfig c = context.Config;
            OperatorData op = data.operators[index];
            if (op.status == OperatorStatus.Lost || op.status == OperatorStatus.Jailed) return;
            op.status = OperatorStatus.Jailed;
            op.jailCycles = context.Random.Next(c.jailCycles.x, c.jailCycles.y + 1);
            op.jailServed = 0;
            op.SetTask(OperatorTask.Rest);
            op.destinationQuarter = Owners.None;
            op.transitCycles = 0;
            if (data.IndexOfQuarter(c.prisonQuarterId) >= 0) op.currentQuarter = c.prisonQuarterId;
            data.operators[index] = op;
            data.indicators.stability = System.Math.Max(0f, data.indicators.stability - c.stabilityPerOperatorLost);
            context.Log(Name(op) + " part aux Baumettes : " + reason + ". " + op.jailCycles + " semaines. Sa famille fera les trajets.", JournalType.Police, c.prisonQuarterId);
        }

        /// <summary>Ajoute de la peur à un quartier (bornée de 0 à 100).</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="amount">Peur ajoutée.</param>
        public static void AddFear(ref GameData data, int quarterId, float amount)
        {
            int qi = data.IndexOfQuarter(quarterId);
            if (qi < 0) return;
            QuarterData q = data.quarters[qi];
            q.fear = MathHelper.Clamp100(q.fear + amount);
            data.quarters[qi] = q;
        }

        /// <summary>Compte une mort : peur dans le quartier et ses voisins, ligne au journal. Jamais de détail.</summary>
        /// <param name="data">Partie modifiée.</param>
        /// <param name="quarterId">Quartier où ça s'est passé.</param>
        /// <param name="text">Ligne de journal (la conséquence, pas l'acte).</param>
        /// <param name="context">Contexte du cycle.</param>
        public static void RegisterDeath(ref GameData data, int quarterId, string text, CycleContext context)
        {
            GameConfig c = context.Config;
            ShadowData s = data.shadow;
            s.deaths++;
            data.shadow = s;
            AddFear(ref data, quarterId, c.orderFear * 2f);
            int qi = data.IndexOfQuarter(quarterId);
            if (qi >= 0)
            {
                QuarterData q = data.quarters[qi];
                for (int n = 0; n < q.adjacentQuarters.Count; n++) AddFear(ref data, q.adjacentQuarters[n], c.deathFearNeighbours);
            }
            context.Log(text, JournalType.Shadow, quarterId);
        }

        /// <summary>Bande visée par un ordre dans ce quartier : la plus présente.</summary>
        /// <param name="q">Quartier visé.</param>
        /// <returns>Bande dominante, ou <see cref="Owners.None"/>.</returns>
        public static int TargetFaction(QuarterData q)
        {
            return q.hostilePresence > 0f ? q.DominantFaction() : Owners.None;
        }
    }
}

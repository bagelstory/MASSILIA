// MARSEILLE — Data/ShadowData.cs
// L'ombre de la partie (design/11-bible-ombre.md §4) : jauge Ombre, morts, Dette de sang par bande,
// ordres en attente (dont ceux donnés depuis le parloir). Tout est abstrait : des chiffres et des conséquences.
// Structs : réassigner après modification d'un élément de liste.

using System.Collections.Generic;

namespace Marseille.Data
{
    /// <summary>Dette de sang envers une bande : chaque ordre l'augmente, elle ne revient jamais à zéro.</summary>
    [System.Serializable]
    public struct BloodDebt
    {
        /// <summary>Bande concernée.</summary>
        public int factionId;

        /// <summary>Niveau de dette (nombre d'ordres donnés contre elle).</summary>
        public int debt;
    }

    /// <summary>Un ordre donné, résolu plus tard (immédiat : cycle suivant ; parloir : avec retard).</summary>
    [System.Serializable]
    public struct PendingOrder
    {
        /// <summary>Bande visée.</summary>
        public int factionId;

        /// <summary>Quartier où l'ordre s'exécute.</summary>
        public int quarterId;

        /// <summary>Cycle de résolution.</summary>
        public int resolveCycle;

        /// <summary>Opérateur incarcéré qui a donné l'ordre, ou -1 si donné dehors.</summary>
        public int fromOperatorId;

        /// <summary>Chance (0-1) que l'ordre soit bien compris.</summary>
        public float reliability;
    }

    /// <summary>État de l'ombre de la partie.</summary>
    [System.Serializable]
    public struct ShadowData
    {
        /// <summary>Jauge Ombre (0-100) : monte avec la puissance, ne baisse presque jamais.</summary>
        public float ombre;

        /// <summary>Morts liées au réseau depuis le début (plancher de l'Ombre).</summary>
        public int deaths;

        /// <summary>Ordres donnés depuis le début.</summary>
        public int ordersGiven;

        /// <summary>Territoire au cycle précédent (calcul de la montée de l'Ombre).</summary>
        public float lastTerritory;

        /// <summary>Trésorerie au cycle précédent (calcul de la montée de l'Ombre).</summary>
        public float lastTreasury;

        /// <summary>Peur moyenne des quartiers tenus au cycle précédent (calcul de la montée de l'Ombre).</summary>
        public float lastFear;

        /// <summary>Dettes de sang par bande.</summary>
        public List<BloodDebt> debts;

        /// <summary>Ordres en attente de résolution.</summary>
        public List<PendingOrder> orders;

        /// <summary>Façades ouvertes (design/13-facades.md).</summary>
        public List<FacadeData> facades;

        /// <summary>Ombre vide, listes initialisées.</summary>
        /// <param name="treasury">Trésorerie de départ.</param>
        /// <returns>L'état initial.</returns>
        public static ShadowData Create(float treasury)
        {
            return new ShadowData
            {
                ombre = 0f,
                deaths = 0,
                ordersGiven = 0,
                lastTerritory = 0f,
                lastTreasury = treasury,
                lastFear = 0f,
                debts = new List<BloodDebt>(),
                orders = new List<PendingOrder>(),
                facades = new List<FacadeData>()
            };
        }

        /// <summary>Dette de sang envers une bande.</summary>
        /// <param name="factionId">Bande.</param>
        /// <returns>Niveau, 0 si aucune.</returns>
        public int DebtOf(int factionId)
        {
            if (debts == null) return 0;
            for (int i = 0; i < debts.Count; i++) if (debts[i].factionId == factionId) return debts[i].debt;
            return 0;
        }

        /// <summary>Ajoute à la dette de sang envers une bande (crée l'entrée si besoin).</summary>
        /// <param name="factionId">Bande.</param>
        /// <param name="amount">Quantité ajoutée.</param>
        public void AddDebt(int factionId, int amount)
        {
            if (debts == null) debts = new List<BloodDebt>();
            for (int i = 0; i < debts.Count; i++)
            {
                if (debts[i].factionId != factionId) continue;
                BloodDebt d = debts[i];
                d.debt += amount;
                debts[i] = d;
                return;
            }
            debts.Add(new BloodDebt { factionId = factionId, debt = amount });
        }
    }
}

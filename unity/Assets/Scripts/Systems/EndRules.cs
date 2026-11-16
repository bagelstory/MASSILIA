// MARSEILLE — Systems/EndRules.cs
// Conditions de fin (spec §11) et bilan : score et note de A à E.

using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Règles de fin de partie.</summary>
    public static class EndRules
    {
        /// <summary>Fin atteinte avec l'état actuel, ou None.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="cycle">Cycle qui vient d'être résolu.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Type de fin.</returns>
        public static EndType Check(GameData data, int cycle, GameConfig config)
        {
            if (data.indicators.pressure >= config.pressureCollapse) return EndType.Collapse;
            if (data.zeroTreasuryCycles >= config.bankruptcyCycles) return EndType.Bankruptcy;
            if (data.indicators.territory >= config.victoryTerritory && data.indicators.treasury > 0f) return EndType.Victory;
            if (OperatorRules.ActiveCount(data) == 0 && data.indicators.treasury < CheapestRecruit(config)) return EndType.Bankruptcy;
            if (cycle >= config.maxCycles) return EndType.TimeOut;
            return EndType.None;
        }

        /// <summary>Coût du recrutement le moins cher.</summary>
        /// <param name="config">Configuration.</param>
        /// <returns>Montant en €.</returns>
        public static float CheapestRecruit(GameConfig config)
        {
            float min = float.MaxValue;
            for (int i = 0; i < config.operatorTypes.Count; i++) min = System.Math.Min(min, config.operatorTypes[i].recruitCost);
            return min == float.MaxValue ? 0f : min;
        }

        /// <summary>Score final : territoire, argent, factions disparues, réseau, moins pression et dette.</summary>
        /// <param name="data">Partie terminée.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Score.</returns>
        public static float Score(GameData data, GameConfig config)
        {
            int gone = 0;
            for (int i = 0; i < data.factions.Count; i++) if (!data.factions[i].isAlive) gone++;
            float score = data.indicators.territory
                          + System.Math.Min(config.scoreMoneyMax, data.indicators.treasury / config.scoreMoneyDivisor)
                          + gone * config.scorePerFactionGone
                          + OperatorRules.ActiveCount(data) * config.scorePerOperator
                          - data.indicators.pressure / config.scorePressureDivisor
                          - data.debt / config.scoreMoneyDivisor;
            if (data.endType == EndType.Victory) score += config.scoreEndBonus.x;
            else if (data.endType != EndType.TimeOut) score += config.scoreEndBonus.y;
            return score;
        }

        /// <summary>Note de A à E d'après le score.</summary>
        /// <param name="score">Score.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Lettre.</returns>
        public static string Grade(float score, GameConfig config)
        {
            string letters = "ABCDE";
            for (int i = 0; i < config.gradeThresholds.Count && i < 4; i++) if (score >= config.gradeThresholds[i]) return letters[i].ToString();
            return "E";
        }

        /// <summary>Phrase de fin affichée au journal.</summary>
        /// <param name="type">Type de fin.</param>
        /// <returns>Texte.</returns>
        public static string Message(EndType type)
        {
            switch (type)
            {
                case EndType.Victory: return "La ville est à toi.";
                case EndType.Bankruptcy: return "Faillite : le réseau ne peut plus payer.";
                case EndType.Collapse: return "Chute : la pression a fait tomber le réseau.";
                case EndType.Abandon: return "Partie abandonnée.";
                case EndType.TimeOut: return "Cinq ans ont passé. Fin de partie.";
            }
            return string.Empty;
        }
    }
}

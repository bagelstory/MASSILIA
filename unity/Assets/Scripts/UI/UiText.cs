// MARSEILLE — UI/UiText.cs
// Libellés français des états du jeu et formats des nombres, partagés par tous les écrans.

using Marseille.Config;
using Marseille.Data;
using Marseille.Systems;

namespace Marseille.UI
{
    /// <summary>Textes d'interface.</summary>
    public static class UiText
    {
        /// <summary>Nom d'un statut de quartier.</summary>
        /// <param name="s">Statut.</param>
        /// <returns>Libellé.</returns>
        public static string Status(QuarterStatus s)
        {
            switch (s)
            {
                case QuarterStatus.Player: return "Tenu";
                case QuarterStatus.Secured: return "Sécurisé";
                case QuarterStatus.Contested: return "Disputé";
                case QuarterStatus.Faction: return "Adverse";
            }
            return "Libre";
        }

        /// <summary>Nom d'un état d'opérateur.</summary>
        /// <param name="s">État.</param>
        /// <returns>Libellé.</returns>
        public static string OperatorState(OperatorStatus s)
        {
            switch (s)
            {
                case OperatorStatus.InTransit: return "En trajet";
                case OperatorStatus.Occupied: return "Occupé";
                case OperatorStatus.Unavailable: return "Épuisé";
                case OperatorStatus.Lost: return "Perdu";
                case OperatorStatus.Jailed: return "Aux Baumettes";
            }
            return "Disponible";
        }

        /// <summary>Nom d'une tâche.</summary>
        /// <param name="task">Tâche.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Libellé.</returns>
        public static string Task(OperatorTask task, GameConfig config)
        {
            if (task == OperatorTask.Rest) return "Repos";
            TaskSettings t = config.GetTaskSettings(task);
            return t != null ? t.displayName : task.ToString();
        }

        /// <summary>Nom d'un type d'opérateur.</summary>
        /// <param name="type">Type.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Libellé.</returns>
        public static string Type(OperatorType type, GameConfig config)
        {
            OperatorTypeSettings s = config.GetOperatorSettings(type);
            return s != null ? s.displayName : type.ToString();
        }

        /// <summary>Nom affiché d'un opérateur : surnom s'il en a un, sinon matricule.</summary>
        /// <param name="op">Opérateur.</param>
        /// <returns>Libellé.</returns>
        public static string OperatorName(OperatorData op)
        {
            return string.IsNullOrEmpty(op.nickname) ? op.Label() : op.nickname + " (" + op.Label() + ")";
        }

        /// <summary>Montant en euros arrondi (« 12 500 € »).</summary>
        /// <param name="value">Montant.</param>
        /// <returns>Texte.</returns>
        public static string Money(float value)
        {
            return value.ToString("N0") + " €";
        }

        /// <summary>Montant signé (« +1 200 € », « −300 € »).</summary>
        /// <param name="value">Montant.</param>
        /// <returns>Texte.</returns>
        public static string SignedMoney(float value)
        {
            return (value >= 0f ? "+" : "−") + System.Math.Abs(value).ToString("N0") + " €";
        }

        /// <summary>Nom d'un quartier par identifiant.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <returns>Nom, ou « ? ».</returns>
        public static string QuarterName(GameData data, int quarterId)
        {
            int i = data.IndexOfQuarter(quarterId);
            return i >= 0 ? data.quarters[i].name : "?";
        }
    }
}

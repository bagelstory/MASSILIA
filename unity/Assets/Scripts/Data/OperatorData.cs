// MARSEILLE — Data/OperatorData.cs
// Un opérateur : une fonction spécialisée déployable (spec §4). Ni dialogue ni mémoire.
// Struct : réassigner l'élément de liste après modification.

namespace Marseille.Data
{
    /// <summary>État d'un opérateur (spec §4.3).</summary>
    [System.Serializable]
    public struct OperatorData
    {
        /// <summary>Numéro d'identification (affiché « OP-007 »).</summary>
        public int id;

        /// <summary>Spécialité.</summary>
        public OperatorType type;

        /// <summary>Efficacité de 1 à 5.</summary>
        public int competence;

        /// <summary>Expérience accumulée vers le prochain point de compétence (0-1).</summary>
        public float experience;

        /// <summary>Fatigue (0-100). À 100, l'opérateur devient indisponible.</summary>
        public float fatigue;

        /// <summary>Disponibilité.</summary>
        public OperatorStatus status;

        /// <summary>Quartier où il se trouve (id).</summary>
        public int currentQuarter;

        /// <summary>Quartier de destination pendant un transit, sinon <see cref="Owners.None"/>.</summary>
        public int destinationQuarter;

        /// <summary>Tâche active, sous forme de texte lisible dans la sauvegarde (nom de <see cref="OperatorTask"/>).</summary>
        public string activeTask;

        /// <summary>Coût d'entretien par cycle (€).</summary>
        public float upkeepCost;

        /// <summary>Cycles de trajet restants (0 si sur place).</summary>
        public int transitCycles;

        /// <summary>Cycles d'indisponibilité restants après épuisement.</summary>
        public int unavailableCycles;

        /// <summary>Identifiant de visage (portrait facultatif, ex. « mika »), vide si aucun.</summary>
        public string faceId;

        /// <summary>Surnom affiché à côté du numéro, vide si aucun.</summary>
        public string nickname;

        /// <summary>Crée un opérateur disponible, sur place, au repos.</summary>
        /// <param name="id">Numéro.</param>
        /// <param name="type">Spécialité.</param>
        /// <param name="competence">Compétence 1-5.</param>
        /// <param name="quarterId">Quartier de départ.</param>
        /// <param name="upkeepCost">Entretien par cycle.</param>
        /// <returns>L'opérateur initialisé.</returns>
        public static OperatorData Create(int id, OperatorType type, int competence, int quarterId, float upkeepCost)
        {
            return new OperatorData
            {
                id = id,
                type = type,
                competence = competence,
                experience = 0f,
                fatigue = 0f,
                status = OperatorStatus.Available,
                currentQuarter = quarterId,
                destinationQuarter = Owners.None,
                activeTask = OperatorTask.Rest.ToString(),
                upkeepCost = upkeepCost,
                transitCycles = 0,
                unavailableCycles = 0,
                faceId = string.Empty,
                nickname = string.Empty
            };
        }

        /// <summary>Tâche active convertie en énumération (texte inconnu ou vide : repos).</summary>
        /// <returns>La tâche courante.</returns>
        public OperatorTask GetTask()
        {
            if (string.IsNullOrEmpty(activeTask)) return OperatorTask.Rest;
            OperatorTask task;
            return System.Enum.TryParse(activeTask, out task) ? task : OperatorTask.Rest;
        }

        /// <summary>Définit la tâche active.</summary>
        /// <param name="task">Nouvelle tâche.</param>
        public void SetTask(OperatorTask task)
        {
            activeTask = task.ToString();
        }

        /// <summary>Quartier où l'opérateur agira : sa destination s'il voyage, sinon sa position.</summary>
        /// <returns>Identifiant de quartier.</returns>
        public int ActingQuarter()
        {
            return status == OperatorStatus.InTransit && destinationQuarter != Owners.None ? destinationQuarter : currentQuarter;
        }

        /// <summary>Numéro formaté pour l'interface.</summary>
        /// <returns>Par exemple « OP-007 ».</returns>
        public string Label()
        {
            return "OP-" + id.ToString("000");
        }
    }
}

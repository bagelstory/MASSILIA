// MARSEILLE — Config/GameConfig.Operators.cs
// Section « opérateurs et tâches » de GameConfig (classe partielle). Valeurs par défaut = équilibrage de la version web.

using System.Collections.Generic;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Config
{
    /// <summary>Paramètres d'un type d'opérateur.</summary>
    [System.Serializable]
    public class OperatorTypeSettings
    {
        /// <summary>Type concerné.</summary>
        public OperatorType type;
        /// <summary>Nom affiché.</summary>
        public string displayName;
        /// <summary>Tâche de spécialité.</summary>
        public OperatorTask specialtyTask;
        /// <summary>Coût de recrutement (€).</summary>
        [Min(0f)] public float recruitCost;
        /// <summary>Entretien par cycle (€).</summary>
        [Min(0f)] public float upkeep;
        /// <summary>Couleur de l'icône sur la carte.</summary>
        public Color color = Color.white;
        /// <summary>Initiale affichée sur le pion.</summary>
        public string letter = "?";
    }

    /// <summary>Effets d'une tâche par cycle, à efficacité 1. Les champs sans objet restent à 0.</summary>
    [System.Serializable]
    public class TaskSettings
    {
        /// <summary>Tâche concernée.</summary>
        public OperatorTask task;
        /// <summary>Nom affiché.</summary>
        public string displayName;
        /// <summary>Fatigue ajoutée par cycle.</summary>
        [Min(0f)] public float fatiguePerCycle;
        /// <summary>Présence adverse retirée (points).</summary>
        [Min(0f)] public float hostileReduction;
        /// <summary>Contrôle gagné (points).</summary>
        [Min(0f)] public float controlGain;
        /// <summary>Pression ajoutée.</summary>
        [Min(0f)] public float pressureAdded;
        /// <summary>Exposition ajoutée.</summary>
        [Min(0f)] public float exposureAdded;
        /// <summary>Exposition retirée.</summary>
        [Min(0f)] public float exposureReduction;
        /// <summary>Probabilité de gagner un niveau de connaissance sur place.</summary>
        [Range(0f, 1f)] public float knowledgeChance;
        /// <summary>Probabilité de révéler chaque quartier voisin (connaissance 1).</summary>
        [Range(0f, 1f)] public float neighbourRevealChance;
        /// <summary>Part du coût de maintien retirée sur place.</summary>
        [Range(0f, 1f)] public float maintenanceReduction;
        /// <summary>Fiabilité ajoutée à chaque flux actif.</summary>
        [Min(0f)] public float fluxReliabilityGain;
        /// <summary>Part d'une attaque adverse absorbée sur place.</summary>
        [Range(0f, 1f)] public float attackReduction;
    }

    public partial class GameConfig
    {
        [Header("Opérateurs")]
        [Tooltip("Paramètres par type (un élément par OperatorType).")]
        public List<OperatorTypeSettings> operatorTypes = new List<OperatorTypeSettings>
        {
            new OperatorTypeSettings { type = OperatorType.Recon, letter = "R", displayName = "Repérage", specialtyTask = OperatorTask.Scout, recruitCost = 900f, upkeep = 150f, color = new Color(0.56f, 0.78f, 1f) },
            new OperatorTypeSettings { type = OperatorType.Pressure, letter = "P", displayName = "Pression", specialtyTask = OperatorTask.Press, recruitCost = 1200f, upkeep = 220f, color = new Color(1f, 0.6f, 0.2f) },
            new OperatorTypeSettings { type = OperatorType.Logistics, letter = "L", displayName = "Logistique", specialtyTask = OperatorTask.ManageFlux, recruitCost = 1400f, upkeep = 240f, color = new Color(0.3f, 0.8f, 0.4f) },
            new OperatorTypeSettings { type = OperatorType.Finance, letter = "€", displayName = "Finances", specialtyTask = OperatorTask.Optimize, recruitCost = 1600f, upkeep = 260f, color = new Color(0.95f, 0.85f, 0.25f) },
            new OperatorTypeSettings { type = OperatorType.Intel, letter = "I", displayName = "Intelligence", specialtyTask = OperatorTask.Watch, recruitCost = 1500f, upkeep = 250f, color = new Color(0.7f, 0.55f, 1f) },
            new OperatorTypeSettings { type = OperatorType.Force, letter = "F", displayName = "Force", specialtyTask = OperatorTask.Secure, recruitCost = 1300f, upkeep = 240f, color = new Color(0.9f, 0.28f, 0.3f) }
        };

        [Tooltip("Effets des tâches (un élément par OperatorTask).")]
        public List<TaskSettings> tasks = new List<TaskSettings>
        {
            new TaskSettings { task = OperatorTask.Rest, displayName = "Repos" },
            new TaskSettings { task = OperatorTask.Secure, displayName = "Sécuriser", fatiguePerCycle = 8.75f, hostileReduction = 18f, controlGain = 4f, pressureAdded = 0.4f, exposureAdded = 0.3f, attackReduction = 0.5f },
            new TaskSettings { task = OperatorTask.Press, displayName = "Presser", fatiguePerCycle = 7f, controlGain = 13f, pressureAdded = 0.25f, exposureAdded = 0.15f },
            new TaskSettings { task = OperatorTask.Scout, displayName = "Repérer", fatiguePerCycle = 3.5f, knowledgeChance = 0.9f, neighbourRevealChance = 0.6f },
            new TaskSettings { task = OperatorTask.Watch, displayName = "Surveiller", fatiguePerCycle = 4.2f, attackReduction = 0.35f },
            new TaskSettings { task = OperatorTask.ManageFlux, displayName = "Gérer les flux", fatiguePerCycle = 5.25f, maintenanceReduction = 0.25f, fluxReliabilityGain = 6f },
            new TaskSettings { task = OperatorTask.Optimize, displayName = "Optimiser", fatiguePerCycle = 3.5f, maintenanceReduction = 0.4f, exposureReduction = 2.5f }
        };

        [Tooltip("Compétence de référence (efficacité 1).")]
        [Min(1)] public int competenceReference = 3;
        [Tooltip("Compétence des recrues : minimum et maximum.")]
        public Vector2Int recruitCompetence = new Vector2Int(1, 3);
        [Tooltip("Efficacité hors spécialité (spec §4.4 : 50 %).")]
        [Range(0f, 1f)] public float offSpecialtyEfficiency = 0.5f;
        [Tooltip("Fatigue retirée par cycle de repos.")]
        [Min(0f)] public float fatigueRecoveryPerRest = 10f;
        [Tooltip("Au-delà de cette fatigue, l'efficacité baisse.")]
        [Range(0f, 100f)] public float fatigueSlowdownThreshold = 70f;
        [Tooltip("Efficacité multipliée par ce facteur au-delà du seuil.")]
        [Range(0f, 1f)] public float fatigueSlowdownFactor = 0.7f;
        [Tooltip("Cycles d'indisponibilité à 100 de fatigue.")]
        [Min(1)] public int exhaustionCycles = 1;
        [Tooltip("Fatigue après la période d'indisponibilité.")]
        [Range(0f, 100f)] public float fatigueAfterExhaustion = 60f;
        [Tooltip("Durée du trajet vers un quartier voisin / non voisin (cycles).")]
        public Vector2Int transitCycles = new Vector2Int(1, 2);
        [Tooltip("Expérience gagnée par cycle de travail en spécialité (1 = +1 compétence).")]
        [Min(0f)] public float experiencePerCycle = 0.08f;
        [Tooltip("Hausse de l'entretien par opérateur au-delà de 6 (part).")]
        [Min(0f)] public float progressiveUpkeep = 0.04f;
        [Tooltip("Fatigue multipliée par ce facteur quand la stabilité est basse.")]
        [Min(1f)] public float lowStabilityFatigueFactor = 1.3f;
        [Tooltip("Stabilité gagnée par recrutement.")]
        [Min(0f)] public float stabilityPerRecruit = 3f;

        /// <summary>Paramètres d'un type d'opérateur.</summary>
        /// <param name="type">Type recherché.</param>
        /// <returns>Les paramètres, ou null si absent de la configuration.</returns>
        public OperatorTypeSettings GetOperatorSettings(OperatorType type)
        {
            for (int i = 0; i < operatorTypes.Count; i++) if (operatorTypes[i].type == type) return operatorTypes[i];
            return null;
        }

        /// <summary>Paramètres d'une tâche.</summary>
        /// <param name="task">Tâche recherchée.</param>
        /// <returns>Les paramètres, ou null si absente de la configuration.</returns>
        public TaskSettings GetTaskSettings(OperatorTask task)
        {
            for (int i = 0; i < tasks.Count; i++) if (tasks[i].task == task) return tasks[i];
            return null;
        }
    }
}

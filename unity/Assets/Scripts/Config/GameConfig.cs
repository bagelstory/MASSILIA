// MARSEILLE — Config/GameConfig.cs
// Tous les paramètres de gameplay, dans un ScriptableObject (Assets > Create > Marseille > Game Config).
// Classe partielle, une section par fichier (chacun reste sous 200 lignes) :
//   GameConfig.cs            sauvegarde, départ de partie, version de la configuration
//   GameConfig.World.cs      zones, factions, charbons générés, réseau de départ
//   GameConfig.Operators.cs  types d'opérateurs, tâches, fatigue, trajets
//   GameConfig.Territory.cs  contrôle, abandon, charbons (ouverture, prise, saisie)
//   GameConfig.Economy.cs    revenus, dette, faillite, investissement
//   GameConfig.Factions.cs   IA des factions, trêves, fragmentation
//   GameConfig.Flux.cs       offres et risques des flux internationaux
//   GameConfig.Indicators.cs pression, exposition, stabilité, capacité
//   GameConfig.Events.cs     calendrier, saisons, opérations, arrestations
//   GameConfig.End.cs        fin de partie, note, objectifs et ouverture des zones
//   GameConfig.Map.cs        carte (grille, zoom, couleurs)
//   GameConfig.UI.cs         interface (tailles, couleurs)
//   GameConfig.Validation.cs contrôle de cohérence dans l'éditeur (OnValidate)
// Ce fichier porte le nom de la classe : Unity l'exige pour un ScriptableObject.

using UnityEngine;

namespace Marseille.Config
{
    /// <summary>Paramètres de jeu (aucune valeur de gameplay en dur dans le code).</summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Marseille/Game Config")]
    public partial class GameConfig : ScriptableObject
    {
        /// <summary>Version de la configuration, écrite dans les journaux de vérification.</summary>
        public const int ConfigVersion = 1;

        [Header("Performance")]
        [Tooltip("Images par seconde visées (mobile : 60).")]
        [Min(15)] public int targetFrameRate = 60;

        [Header("Sauvegarde")]
        [Tooltip("Nom du fichier dans Application.persistentDataPath.")]
        public string saveFileName = "marseille_save.json";

        [Tooltip("JSON indenté (lisible et éditable à la main).")]
        public bool prettyPrintSave = true;

        [Tooltip("Sauvegarde automatique après chaque action de planification.")]
        public bool saveOnEveryChange = true;

        [Tooltip("Nombre maximal de lignes gardées dans le journal.")]
        [Min(10)] public int journalMaxEntries = 400;

        [Tooltip("Nombre maximal de lignes gardées dans l'historique des cycles.")]
        [Min(10)] public int logMaxEntries = 500;

        [Header("Départ de partie")]
        [Tooltip("Trésorerie initiale (€).")]
        [Min(0f)] public float startingTreasury = 5000f;

        [Tooltip("Capacité initiale (opérateurs déployables en même temps).")]
        [Min(1)] public int startingCapacity = 3;

        [Tooltip("Pression initiale (0-100).")]
        [Range(0f, 100f)] public float startingPressure = 15f;

        [Tooltip("Stabilité initiale (0-100).")]
        [Range(0f, 100f)] public float startingStability = 55f;

        [Tooltip("Exposition initiale (0-100).")]
        [Range(0f, 100f)] public float startingExposure = 5f;
    }
}

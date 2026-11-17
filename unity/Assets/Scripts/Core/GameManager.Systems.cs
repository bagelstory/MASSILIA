// MARSEILLE — Core/GameManager.Systems.cs
// Implémentation de RegisterSystems (déclarée dans GameManager.cs) : ordre de résolution d'un cycle.
// Ordre de la spécification, plus l'ombre après les indicateurs et les objectifs juste avant la fin de partie.

using Marseille.Systems;

namespace Marseille.Core
{
    /// <summary>Enregistrement des systèmes du cycle.</summary>
    public partial class GameManager
    {
        /// <summary>Enregistre les systèmes dans l'ordre de résolution.</summary>
        /// <param name="manager">Gestionnaire de cycle.</param>
        partial void RegisterSystems(CycleManager manager)
        {
            manager.Register(new OperatorSystem());
            manager.Register(new TerritorySystem());
            manager.Register(new FactionAISystem());
            manager.Register(new FluxSystem());
            manager.Register(new HierarchySystem());
            manager.Register(new EconomySystem());
            manager.Register(new IndicatorSystem());
            manager.Register(new ShadowSystem());
            manager.Register(new FacadeSystem());
            manager.Register(new GameEventSystem());
            manager.Register(new QuestSystem());
            manager.Register(new EndConditionSystem());
        }
    }
}

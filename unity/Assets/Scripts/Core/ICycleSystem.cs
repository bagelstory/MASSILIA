// MARSEILLE — Core/ICycleSystem.cs
// Contrat commun des systèmes de résolution (Economy, Territory, Operator, FactionAI…).
// Les systèmes ne sont pas des MonoBehaviour : ce sont des classes légères appelées dans l'ordre par CycleManager.

using Marseille.Data;

namespace Marseille.Core
{
    /// <summary>Une étape de la résolution d'un cycle.</summary>
    public interface ICycleSystem
    {
        /// <summary>Nom court du système (journal d'erreurs, tests).</summary>
        string Name { get; }

        /// <summary>Applique l'étape à l'état de la partie.</summary>
        /// <param name="data">État de la partie, modifié en place (struct passée par référence).</param>
        /// <param name="context">Configuration, hasard du cycle et rapport.</param>
        void Resolve(ref GameData data, CycleContext context);
    }
}

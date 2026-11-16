// MARSEILLE — Systems/EndConditionSystem.cs
// Dernière étape de la résolution : vérifie les conditions de fin. CycleManager n'avance pas le cycle
// et GameManager passe en GameOver quand data.endType n'est plus None.

using Marseille.Core;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Système des conditions de fin (spec §11).</summary>
    public sealed class EndConditionSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "EndCondition"; } }

        /// <summary>Fixe le type de fin si une condition est remplie.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            if (data.endType != EndType.None) return;
            EndType end = EndRules.Check(data, context.Cycle, context.Config);
            if (end == EndType.None) return;
            data.endType = end;
            context.Log(EndRules.Message(end), JournalType.End);
        }
    }
}

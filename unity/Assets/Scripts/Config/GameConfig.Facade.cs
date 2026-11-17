// MARSEILLE — Config/GameConfig.Facade.cs
// Section « façades » de GameConfig (design/13-facades.md). Points de départ, à vérifier en simulation sur 260 cycles.

using Marseille.Data;
using UnityEngine;

namespace Marseille.Config
{
    public partial class GameConfig
    {
        [Header("Façades : placer l'argent")]
        [Tooltip("Fiches des secteurs, dans l'ordre de FacadeSector (Alim, Chicha, Garage, Snack, Club, Label, Immo).")]
        public FacadeSpec[] facades = FacadeSpec.Defaults();
        [Tooltip("Nombre maximal de façades par quartier.")]
        [Min(1)] public int facadeMaxPerQuarter = 2;
        [Tooltip("Éveil gagné par cycle quand la façade absorbe plus que le quartier ne peut croire (par point de dépassement).")]
        [Min(0f)] public float facadeOverEveil = 8f;
        [Tooltip("Éveil perdu par cycle quand la façade reste dans ce que le quartier peut croire.")]
        [Min(0f)] public float facadeEveilDecay = 1f;
        [Tooltip("Éveil à partir duquel des contrôles peuvent tomber.")]
        [Range(0f, 100f)] public float facadeEveilControl = 60f;
        [Tooltip("Éveil à partir duquel la façade tombe.")]
        [Range(0f, 100f)] public float facadeEveilFall = 90f;
        [Tooltip("Cycles de gel après un contrôle.")]
        [Min(1)] public int facadeFreezeCycles = 6;
        [Tooltip("Éveil retiré par un contrôle (la façade a répondu, le soupçon retombe un peu).")]
        [Min(0f)] public float facadeControlEveilRelief = 25f;
        [Tooltip("Ombre ajoutée à l'ouverture d'une façade.")]
        [Min(0f)] public float facadeOmbreOpen = 1f;
        [Tooltip("Ombre ajoutée quand une façade tombe.")]
        [Min(0f)] public float facadeOmbreFall = 3f;
        [Tooltip("Contamination ajoutée par cycle pour chaque façade au-delà de la première dans un quartier.")]
        [Min(0f)] public float facadeContamination = 1f;
        [Tooltip("Part du coût d'ouverture payée pour fermer une façade.")]
        [Range(0f, 1f)] public float facadeCloseShare = 0.33f;
        [Tooltip("Peur ajoutée dans le quartier quand une façade tombe.")]
        [Min(0f)] public float facadeFallFear = 10f;
    }
}

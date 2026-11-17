// MARSEILLE — Systems/FacadeSystem.cs
// Étape « façades » (juste après l'ombre) : chaque façade ouverte absorbe un peu d'exposition, rapporte un peu,
// et laisse de l'Éveil quand elle dépasse ce que son quartier peut croire. Contrôles, gel, chute.
// Le texte ne montre que ce que la ville perd. Aucune technique.

using Marseille.Config;
using Marseille.Core;
using Marseille.Data;
using Marseille.Utils;

namespace Marseille.Systems
{
    /// <summary>Système des façades (design/13-facades.md).</summary>
    public sealed class FacadeSystem : ICycleSystem
    {
        /// <inheritdoc />
        public string Name { get { return "Facade"; } }

        /// <summary>Résout les façades du cycle.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="context">Contexte du cycle.</param>
        public void Resolve(ref GameData data, CycleContext context)
        {
            if (data.shadow.facades == null || data.shadow.facades.Count == 0) return;
            GameConfig c = context.Config;
            for (int i = data.shadow.facades.Count - 1; i >= 0; i--)
            {
                FacadeData f = data.shadow.facades[i];
                int qi = data.IndexOfQuarter(f.quarterId);
                if (qi < 0) { data.shadow.facades.RemoveAt(i); continue; }
                FacadeSpec spec = FacadeRules.Spec(f.sector, c);
                QuarterData q = data.quarters[qi];
                f.age++;
                if (f.frozen > 0) f.frozen--;
                else if (f.age >= spec.delay) Run(ref data, ref f, ref q, spec, c);
                if (CountBefore(data, f, i) > 0) q.contamination = MathHelper.Clamp(q.contamination + c.facadeContamination, 0f, c.contaminationMax);
                data.quarters[qi] = q;
                bool fell = Control(ref data, ref f, q, c, context);
                if (fell) { data.shadow.facades.RemoveAt(i); continue; }
                data.shadow.facades[i] = f;
            }
        }

        /// <summary>Absorption, revenu et Éveil d'une façade qui tourne.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="f">Façade.</param>
        /// <param name="q">Quartier.</param>
        /// <param name="spec">Fiche du secteur.</param>
        /// <param name="c">Configuration.</param>
        private static void Run(ref GameData data, ref FacadeData f, ref QuarterData q, FacadeSpec spec, GameConfig c)
        {
            float before = data.indicators.exposure;
            data.indicators.exposure = MathHelper.Clamp100(before - spec.absorption);
            f.absorbed += before - data.indicators.exposure;
            data.indicators.treasury += spec.revenue * System.Math.Max(0.1f, q.population / 100f);
            float capacity = FacadeRules.Capacity(f, q, c);
            float ratio = spec.absorption / System.Math.Max(0.1f, capacity);
            if (ratio > 1f) f.eveil = MathHelper.Clamp100(f.eveil + c.facadeOverEveil * (ratio - 1f));
            else f.eveil = MathHelper.Clamp100(f.eveil - c.facadeEveilDecay);
        }

        /// <summary>Nombre d'autres façades du même quartier déjà traitées avant celle-ci (la 2e pèse sur le quartier).</summary>
        /// <param name="data">Partie.</param>
        /// <param name="f">Façade.</param>
        /// <param name="index">Index de la façade.</param>
        /// <returns>Nombre de façades du quartier avec un index plus petit.</returns>
        private static int CountBefore(GameData data, FacadeData f, int index)
        {
            int n = 0;
            for (int j = 0; j < index; j++) if (data.shadow.facades[j].quarterId == f.quarterId) n++;
            return n;
        }

        /// <summary>Contrôle possible quand l'Éveil est haut ; chute à l'Éveil maximal.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="f">Façade.</param>
        /// <param name="q">Quartier.</param>
        /// <param name="c">Configuration.</param>
        /// <param name="context">Contexte.</param>
        /// <returns>Vrai si la façade tombe.</returns>
        private static bool Control(ref GameData data, ref FacadeData f, QuarterData q, GameConfig c, CycleContext context)
        {
            if (f.eveil < c.facadeEveilControl) return false;
            if (f.eveil >= c.facadeEveilFall)
            {
                ShadowData s = data.shadow;
                s.ombre = MathHelper.Clamp(s.ombre + c.facadeOmbreFall, 0f, 100f);
                data.shadow = s;
                ShadowRules.AddFear(ref data, q.id, c.facadeFallFear);
                context.Log(FacadeRules.FallLine(f.sector, q.name), JournalType.Shadow, q.id);
                return true;
            }
            FacadeSpec spec = FacadeRules.Spec(f.sector, c);
            if (f.frozen > 0 || !context.Chance(spec.risk * f.eveil / 100f)) return false;
            f.frozen = c.facadeFreezeCycles;
            f.eveil = System.Math.Max(0f, f.eveil - c.facadeControlEveilRelief);
            data.indicators.pressure = MathHelper.Clamp100(data.indicators.pressure + 3f);
            context.Log(FacadeRules.ControlLine(f.sector, q.name), JournalType.Police, q.id);
            return false;
        }
    }
}

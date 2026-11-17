// MARSEILLE — Systems/FacadeRules.cs
// Règles pures des façades (design/13-facades.md) : fiche d'un secteur, ce que le quartier peut croire,
// recherche. Le texte ne décrit jamais une technique : seulement ce que la ville voit et perd.

using Marseille.Config;
using Marseille.Data;

namespace Marseille.Systems
{
    /// <summary>Règles de calcul des façades.</summary>
    public static class FacadeRules
    {
        /// <summary>Fiche d'un secteur (défaut si la configuration est incomplète).</summary>
        /// <param name="sector">Secteur.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Fiche.</returns>
        public static FacadeSpec Spec(FacadeSector sector, GameConfig config)
        {
            int i = (int)sector;
            if (config.facades != null && i >= 0 && i < config.facades.Length) return config.facades[i];
            FacadeSpec[] fallback = FacadeSpec.Defaults();
            return fallback[System.Math.Max(0, System.Math.Min(fallback.Length - 1, i))];
        }

        /// <summary>Ce que le quartier peut croire de cette façade, par cycle.</summary>
        /// <param name="f">Façade.</param>
        /// <param name="q">Quartier.</param>
        /// <param name="config">Configuration.</param>
        /// <returns>Capacité plausible (en points d'exposition par cycle).</returns>
        public static float Capacity(FacadeData f, QuarterData q, GameConfig config)
        {
            FacadeSpec s = Spec(f.sector, config);
            float years = f.age / 52f;
            float people = System.Math.Max(0.1f, q.population / 100f);
            return s.capacity * (1f + s.growth * years) * people;
        }

        /// <summary>Index d'une façade dans la liste, ou -1.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <param name="sector">Secteur.</param>
        /// <returns>Index, ou -1.</returns>
        public static int Find(GameData data, int quarterId, FacadeSector sector)
        {
            if (data.shadow.facades == null) return -1;
            for (int i = 0; i < data.shadow.facades.Count; i++)
                if (data.shadow.facades[i].quarterId == quarterId && data.shadow.facades[i].sector == sector) return i;
            return -1;
        }

        /// <summary>Nombre de façades ouvertes dans un quartier.</summary>
        /// <param name="data">Partie.</param>
        /// <param name="quarterId">Quartier.</param>
        /// <returns>Nombre.</returns>
        public static int CountIn(GameData data, int quarterId)
        {
            int n = 0;
            if (data.shadow.facades == null) return 0;
            for (int i = 0; i < data.shadow.facades.Count; i++) if (data.shadow.facades[i].quarterId == quarterId) n++;
            return n;
        }

        /// <summary>Ligne de journal quand un contrôle tombe sur la façade.</summary>
        /// <param name="sector">Secteur.</param>
        /// <param name="quarter">Nom du quartier.</param>
        /// <returns>Une ligne.</returns>
        public static string ControlLine(FacadeSector sector, string quarter)
        {
            switch (sector)
            {
                case FacadeSector.Alim: return quarter + " : un contrôle à l'alimentation. « Trop de rotation pour un si petit magasin. »";
                case FacadeSector.Chicha: return quarter + " : les voisins ont signé. Le salon ferme sur arrêté, pour un temps.";
                case FacadeSector.Garage: return quarter + " : la fourrière passe devant le garage. Les mêmes voitures restent des semaines.";
                case FacadeSector.Snack: return quarter + " : un contrôle au snack. Une cliente a laissé un avis qui traîne.";
                case FacadeSector.Club: return quarter + " : des parents retirent leurs enfants du club. Quelqu'un a parlé à quelqu'un.";
                case FacadeSector.Label: return quarter + " : un clip tourné dans la cage d'escalier. Quelqu'un l'a regardé de près.";
                default: return quarter + " : un contrôle sur les immeubles. Les locataires parlent aux journalistes.";
            }
        }

        /// <summary>Ligne de journal quand la façade tombe.</summary>
        /// <param name="sector">Secteur.</param>
        /// <param name="quarter">Nom du quartier.</param>
        /// <returns>Une ligne.</returns>
        public static string FallLine(FacadeSector sector, string quarter)
        {
            switch (sector)
            {
                case FacadeSector.Club: return quarter + " : le club ferme. Les gamins n'ont plus de salle.";
                case FacadeSector.Immo: return quarter + " : l'immeuble se vide, des familles déménagent. Les noms sont dans le journal.";
                case FacadeSector.Label: return quarter + " : le studio est vide. « Il a réussi sans toi », dit quelqu'un.";
                default: return quarter + " : la vitrine s'éteint. Le quartier reste avec le vide.";
            }
        }
    }
}

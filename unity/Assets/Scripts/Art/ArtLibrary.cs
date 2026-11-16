// MARSEILLE — Art/ArtLibrary.cs
// Accès aux images du jeu rangées dans Resources/Art (icônes dessinées par tools/gen_icons.py, puis
// portraits et illustrations à venir). Chaque image manquante renvoie null : l'interface garde alors
// son affichage texte, le jeu fonctionne sans aucune image.

using System.Collections.Generic;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Art
{
    /// <summary>Bibliothèque d'images avec cache.</summary>
    public static class ArtLibrary
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        private static readonly Dictionary<OperatorType, string> TypeIcons = new Dictionary<OperatorType, string>
        {
            { OperatorType.Recon, "type_reperage" }, { OperatorType.Pressure, "type_pression" },
            { OperatorType.Logistics, "type_logistique" }, { OperatorType.Finance, "type_finances" },
            { OperatorType.Intel, "type_renseignement" }, { OperatorType.Force, "type_force" }
        };

        private static readonly Dictionary<OperatorTask, string> TaskIcons = new Dictionary<OperatorTask, string>
        {
            { OperatorTask.Secure, "task_securiser" }, { OperatorTask.Press, "task_presser" },
            { OperatorTask.Scout, "task_reperer" }, { OperatorTask.Watch, "task_surveiller" },
            { OperatorTask.ManageFlux, "task_flux" }, { OperatorTask.Optimize, "task_optimiser" },
            { OperatorTask.Rest, "task_repos" }
        };

        private static readonly string[] FactionEmblems =
        {
            "faction_compagnie", "faction_gabians", "faction_ruisseau", "faction_tramontane", "faction_calanquais"
        };

        /// <summary>Charge un sprite de Resources/Art (mis en cache, null si absent).</summary>
        /// <param name="path">Chemin relatif à Resources/Art, sans extension (ex. « types/type_force »).</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Get(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            Sprite s;
            if (Cache.TryGetValue(path, out s)) return s;
            s = Resources.Load<Sprite>("Art/" + path);
            Cache[path] = s;
            return s;
        }

        /// <summary>Icône d'un type d'opérateur.</summary>
        /// <param name="type">Type.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite TypeIcon(OperatorType type)
        {
            string name;
            return TypeIcons.TryGetValue(type, out name) ? Get("types/" + name) : null;
        }

        /// <summary>Icône d'une tâche.</summary>
        /// <param name="task">Tâche.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite TaskIcon(OperatorTask task)
        {
            string name;
            return TaskIcons.TryGetValue(task, out name) ? Get("tasks/" + name) : null;
        }

        /// <summary>Emblème d'une faction (identifiants 1 à 5 dans l'ordre de la configuration).</summary>
        /// <param name="factionId">Faction.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite FactionEmblem(int factionId)
        {
            return factionId >= 1 && factionId <= FactionEmblems.Length ? Get("factions/" + FactionEmblems[factionId - 1]) : null;
        }

        /// <summary>Icône d'un indicateur (« territoire », « tresorerie », « effectifs », « pression »…).</summary>
        /// <param name="key">Clé sans accent.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Indicator(string key)
        {
            return Get("indicators/ind_" + key);
        }

        /// <summary>Pictogramme d'onglet ou de bouton (« tab_carte », « btn_fermer »…).</summary>
        /// <param name="name">Nom du fichier.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Tab(string name)
        {
            return Get("tabs/" + name);
        }

        /// <summary>Forme de base : « pawn » (rond) ou « tile » (case arrondie, 9-slice).</summary>
        /// <param name="name">Nom.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Shape(string name)
        {
            return Get("shapes/" + name);
        }

        /// <summary>Portrait d'un opérateur : son surnom s'il en a un (portrait_zepek), sinon un portrait de
        /// recrue de son type, choisi par son identifiant (portrait_force_1 ou _2). Null si absent.</summary>
        /// <param name="op">Opérateur.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Portrait(OperatorData op)
        {
            if (!string.IsNullOrEmpty(op.nickname))
            {
                Sprite named = Get("portraits/portrait_" + Plain(op.nickname));
                if (named != null) return named;
            }
            string type;
            if (!TypeIcons.TryGetValue(op.type, out type)) return null;
            return Get("portraits/portrait_" + type.Substring("type_".Length) + "_" + (op.id % 2 == 0 ? 2 : 1));
        }

        /// <summary>Grande image : « splash », « map_fond », « end_victoire », « flux_sud »…</summary>
        /// <param name="name">Nom du fichier.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Screen(string name)
        {
            return Get("screens/" + name);
        }

        /// <summary>Image d'un partenaire de flux (1 = sud, 2 = nord).</summary>
        /// <param name="fluxId">Flux.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Flux(int fluxId)
        {
            return fluxId == 1 ? Screen("flux_sud") : fluxId == 2 ? Screen("flux_nord") : null;
        }

        /// <summary>Logo CHOUF sur fond transparent.</summary>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Logo()
        {
            return Get("logo/logo_chouf");
        }

        /// <summary>Minuscules sans accents (« Zépek » → « zepek ») pour les noms de fichiers.</summary>
        /// <param name="text">Texte.</param>
        /// <returns>Texte simplifié.</returns>
        public static string Plain(string text)
        {
            string d = text.ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char ch in d)
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark) sb.Append(ch);
            return sb.ToString();
        }

        /// <summary>Icône d'un symbole de carte : « charbon », « menace », « verrou ».</summary>
        /// <param name="name">Nom.</param>
        /// <returns>Le sprite, ou null.</returns>
        public static Sprite Symbol(string name)
        {
            return Get("types/" + name);
        }
    }
}

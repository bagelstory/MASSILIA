// MARSEILLE — Core/SaveManager.cs
// Sauvegarde JSON lisible dans Application.persistentDataPath, avec JsonUtility (standard Unity).
// Écriture sûre : fichier temporaire, copie de secours, puis remplacement. Lecture : secours si le fichier est abîmé.

using System;
using System.IO;
using Marseille.Config;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Core
{
    /// <summary>Lecture et écriture de la sauvegarde.</summary>
    public static class SaveManager
    {
        private const string TempSuffix = ".tmp";
        private const string BackupSuffix = ".bak";

        /// <summary>Chemin complet du fichier de sauvegarde.</summary>
        /// <param name="config">Configuration (nom du fichier).</param>
        /// <returns>Chemin absolu.</returns>
        public static string SavePath(GameConfig config)
        {
            return Path.Combine(Application.persistentDataPath, config.saveFileName);
        }

        /// <summary>Indique si une sauvegarde existe.</summary>
        /// <param name="config">Configuration.</param>
        /// <returns>Vrai si le fichier (ou sa copie de secours) existe.</returns>
        public static bool HasSave(GameConfig config)
        {
            string path = SavePath(config);
            return File.Exists(path) || File.Exists(path + BackupSuffix);
        }

        /// <summary>Écrit la partie sur le disque.</summary>
        /// <param name="data">État à sauvegarder.</param>
        /// <param name="config">Configuration (nom du fichier, mise en forme).</param>
        /// <returns>Vrai si l'écriture a réussi.</returns>
        public static bool Save(GameData data, GameConfig config)
        {
            string path = SavePath(config);
            string temp = path + TempSuffix;
            try
            {
                string json = JsonUtility.ToJson(data, config.prettyPrintSave);
                File.WriteAllText(temp, json);
                if (File.Exists(path)) File.Copy(path, path + BackupSuffix, true);
                File.Copy(temp, path, true);
                File.Delete(temp);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Sauvegarde impossible : " + e.Message);
                return false;
            }
        }

        /// <summary>Lit la sauvegarde, ou à défaut sa copie de secours.</summary>
        /// <param name="config">Configuration.</param>
        /// <param name="data">État lu (valeur par défaut si échec).</param>
        /// <returns>Vrai si une sauvegarde valide a été lue.</returns>
        public static bool TryLoad(GameConfig config, out GameData data)
        {
            string path = SavePath(config);
            if (TryRead(path, out data)) return true;
            if (TryRead(path + BackupSuffix, out data))
            {
                Debug.LogWarning("Sauvegarde principale illisible : copie de secours utilisée.");
                return true;
            }
            data = default(GameData);
            return false;
        }

        /// <summary>Efface la sauvegarde et sa copie de secours.</summary>
        /// <param name="config">Configuration.</param>
        public static void Delete(GameConfig config)
        {
            string path = SavePath(config);
            try
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(path + BackupSuffix)) File.Delete(path + BackupSuffix);
            }
            catch (Exception e)
            {
                Debug.LogError("Suppression de la sauvegarde impossible : " + e.Message);
            }
        }

        /// <summary>Lit et valide un fichier de sauvegarde.</summary>
        /// <param name="path">Fichier à lire.</param>
        /// <param name="data">État lu.</param>
        /// <returns>Vrai si le fichier existe et contient une partie valide.</returns>
        private static bool TryRead(string path, out GameData data)
        {
            data = default(GameData);
            if (!File.Exists(path)) return false;
            try
            {
                data = JsonUtility.FromJson<GameData>(File.ReadAllText(path));
                if (!IsValid(data)) return false;
                Migrate(ref data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Lecture de " + path + " impossible : " + e.Message);
                return false;
            }
        }

        /// <summary>Met une ancienne sauvegarde au format actuel (version 1 → 5 : ombre, peur, population, façades, hiérarchie, conséquences).</summary>
        /// <param name="data">État lu, modifié en place.</param>
        private static void Migrate(ref GameData data)
        {
            if (data.saveVersion >= GameData.CurrentVersion) return;
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                if (q.population <= 0f) q.population = 100f;
                data.quarters[i] = q;
            }
            float treasury = data.indicators.treasury;
            if (data.shadow.debts == null || data.shadow.orders == null) data.shadow = ShadowData.Create(treasury);
            if (data.saveVersion < 4) MigrateCrew(ref data);
            if (data.shadow.facades == null)
            {
                ShadowData sh = data.shadow;
                sh.facades = new System.Collections.Generic.List<FacadeData>();
                data.shadow = sh;
            }
            if (data.saveVersion < 5 || data.life.standings == null) data.life = ConsequenceData.Create();
            data.saveVersion = GameData.CurrentVersion;
        }

        /// <summary>Version 3 → 4 : postes vides sur tous les charbons, opérateurs sans rattachement, arrondissements.</summary>
        /// <param name="data">État lu, modifié en place.</param>
        private static void MigrateCrew(ref GameData data)
        {
            for (int i = 0; i < data.quarters.Count; i++)
            {
                QuarterData q = data.quarters[i];
                for (int p = 0; p < q.pois.Count; p++)
                {
                    PoiData poi = q.pois[p];
                    poi.slotGuetteur1 = CrewSlots.Empty;
                    poi.slotGuetteur2 = CrewSlots.Empty;
                    poi.slotVendeur = CrewSlots.Empty;
                    poi.slotGerant = CrewSlots.Empty;
                    poi.slotHead = CrewSlots.Empty;
                    q.pois[p] = poi;
                }
                data.quarters[i] = q;
            }
            for (int o = 0; o < data.operators.Count; o++)
            {
                OperatorData op = data.operators[o];
                op.role = CrewRole.None;
                op.poiId = -1;
                data.operators[o] = op;
            }
            data.arrondissements = new System.Collections.Generic.List<ArrondissementData>();
            Marseille.Systems.CrewRules.BuildArrondissements(ref data);
        }

        /// <summary>Vérifications minimales d'une sauvegarde lue.</summary>
        /// <param name="data">État lu.</param>
        /// <returns>Vrai si la partie est exploitable.</returns>
        private static bool IsValid(GameData data)
        {
            return data.saveVersion > 0 && data.currentCycle > 0 && data.quarters != null && data.quarters.Count > 0
                   && data.operators != null && data.factions != null;
        }
    }
}

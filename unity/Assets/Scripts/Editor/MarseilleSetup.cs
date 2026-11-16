// MARSEILLE — Editor/MarseilleSetup.cs
// Outils d'éditeur (exclus du build) : créer l'asset GameConfig par défaut dans Resources, créer la
// scène principale (GameManager + UIManager), régler le projet en portrait. Menu « Marseille ».

using Marseille.Config;
using Marseille.Core;
using Marseille.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Marseille.EditorTools
{
    /// <summary>Mise en place du projet en deux clics.</summary>
    public static class MarseilleSetup
    {
        private const string ConfigPath = "Assets/Resources/GameConfig.asset";
        private const string ScenePath = "Assets/Scenes/Main.unity";

        /// <summary>Crée Assets/Resources/GameConfig.asset avec les valeurs par défaut (sans écraser l'existant).</summary>
        /// <returns>L'asset.</returns>
        [MenuItem("Marseille/1. Créer la configuration par défaut")]
        public static GameConfig CreateConfig()
        {
            GameConfig existing = AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);
            if (existing != null) { Selection.activeObject = existing; return existing; }
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
            GameConfig config = ScriptableObject.CreateInstance<GameConfig>();
            AssetDatabase.CreateAsset(config, ConfigPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = config;
            Debug.Log("GameConfig créé : " + ConfigPath);
            return config;
        }

        /// <summary>Crée la scène principale, l'ajoute au build et règle le jeu en portrait.</summary>
        [MenuItem("Marseille/2. Créer la scène principale")]
        public static void CreateScene()
        {
            GameConfig config = CreateConfig();
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject camera = new GameObject("Main Camera", typeof(Camera));
            camera.tag = "MainCamera";
            Camera cam = camera.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = config.uiBackground;
            GameObject game = new GameObject("GameManager");
            GameManager gm = game.AddComponent<GameManager>();
            SerializedObject so = new SerializedObject(gm);
            SerializedProperty prop = so.FindProperty("config");
            if (prop != null) { prop.objectReferenceValue = config; so.ApplyModifiedPropertiesWithoutUndo(); }
            new GameObject("UI", typeof(RectTransform), typeof(UIManager));
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            ApplyMobileSettings();
            Debug.Log("Scène créée : " + ScenePath + ". Appuie sur Play.");
        }

        /// <summary>Portrait, 60 images/s visées sur mobile, nom du produit.</summary>
        [MenuItem("Marseille/3. Régler le projet (portrait, mobile)")]
        public static void ApplyMobileSettings()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.productName = "CHOUF";
            QualitySettings.vSyncCount = 0;
            Debug.Log("Projet réglé en portrait.");
        }

        /// <summary>Affiche les incohérences de la configuration.</summary>
        [MenuItem("Marseille/Vérifier la configuration")]
        public static void ValidateConfig()
        {
            GameConfig config = CreateConfig();
            var problems = config.Validate();
            if (problems.Count == 0) Debug.Log("GameConfig : aucune incohérence.");
            foreach (string p in problems) Debug.LogWarning("GameConfig : " + p, config);
        }
    }
}

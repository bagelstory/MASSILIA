// MARSEILLE — Core/GameManager.cs
// Singleton de scène : détient l'état de la partie (GameData), la boucle Planning → Resolving → Results,
// la sauvegarde automatique et la publication des événements. Les systèmes s'enregistrent via
// RegisterSystems (GameManager.Systems.cs, étape 4) et le monde initial via BuildNewWorld (étape 3).
// Classe partielle : ces méthodes partielles sans corps sont ignorées tant que leur fichier n'existe pas.

using System;
using Marseille.Config;
using Marseille.Data;
using UnityEngine;

namespace Marseille.Core
{
    /// <summary>Point d'entrée de la partie. À placer sur un GameObject de la scène Main.</summary>
    [DisallowMultipleComponent]
    public partial class GameManager : MonoBehaviour
    {
        /// <summary>Instance unique.</summary>
        public static GameManager Instance { get; private set; }

        [SerializeField, Tooltip("Paramètres de jeu (asset GameConfig).")]
        private GameConfig config;

        [SerializeField, Tooltip("Reprendre automatiquement la sauvegarde au lancement.")]
        private bool continueOnStart = true;

        private GameData data;
        private bool hasGame;
        private readonly CycleManager cycleManager = new CycleManager();

        /// <summary>Paramètres de jeu.</summary>
        public GameConfig Config { get { return config; } }

        /// <summary>Une partie est en cours (créée ou chargée).</summary>
        public bool HasGame { get { return hasGame; } }

        /// <summary>État courant de la boucle.</summary>
        public GameState State { get { return (GameState)data.gameState; } }

        /// <summary>Accès direct à l'état de la partie (référence : les modifications sont conservées).</summary>
        public ref GameData Data { get { return ref data; } }

        /// <summary>Gestionnaire des cycles (lecture des systèmes enregistrés).</summary>
        public CycleManager Cycles { get { return cycleManager; } }

        /// <summary>Enregistre les systèmes de résolution dans l'ordre (implémenté à l'étape 4).</summary>
        /// <param name="manager">Gestionnaire à remplir.</param>
        partial void RegisterSystems(CycleManager manager);

        /// <summary>Remplit une nouvelle partie : quartiers, zones, factions, opérateurs, quêtes (implémenté à l'étape 3).</summary>
        /// <param name="newData">Partie vide à remplir.</param>
        partial void BuildNewWorld(ref GameData newData);

        /// <summary>Initialise le singleton et le pipeline de résolution.</summary>
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (config == null) config = Resources.Load<GameConfig>("GameConfig");
            if (config == null)
            {
                // Sans asset, les valeurs par défaut du code s'appliquent (identiques à un asset neuf).
                Debug.LogWarning("GameManager : aucun GameConfig assigné ni dans Resources ; valeurs par défaut.");
                config = ScriptableObject.CreateInstance<GameConfig>();
            }
            Application.targetFrameRate = config.targetFrameRate;
            cycleManager.Clear();
            RegisterSystems(cycleManager);
        }

        /// <summary>Reprend la sauvegarde si demandé.</summary>
        private void Start()
        {
            if (continueOnStart && config != null) TryContinue();
        }

        /// <summary>Crée une nouvelle partie avec une graine donnée (reproductible).</summary>
        /// <param name="seed">Graine aléatoire.</param>
        public void NewGame(int seed)
        {
            data = GameData.CreateEmpty(seed, config.startingTreasury, config.startingCapacity);
            data.indicators = IndicatorData.Create(config.startingTreasury, config.startingPressure, config.startingStability, config.startingExposure);
            BuildNewWorld(ref data);
            hasGame = true;
            AddJournal("Cycle 1. Ton réseau s'installe.", JournalType.Info, data.baseQuarterId);
            SetState(GameState.Planning);
            Save();
            EventBus.RaiseGameLoaded();
        }

        /// <summary>Crée une nouvelle partie avec une graine tirée de l'horloge.</summary>
        public void NewGame()
        {
            NewGame(Environment.TickCount & int.MaxValue);
        }

        /// <summary>Charge la sauvegarde si elle existe.</summary>
        /// <returns>Vrai si une partie a été chargée.</returns>
        public bool TryContinue()
        {
            GameData loaded;
            if (!SaveManager.TryLoad(config, out loaded)) return false;
            data = loaded;
            hasGame = true;
            // Une partie interrompue en pleine résolution reprend en planification (la résolution est atomique).
            if (State == GameState.Resolving) data.gameState = (int)GameState.Planning;
            EventBus.RaiseGameLoaded();
            EventBus.RaiseStateChanged(State);
            return true;
        }

        /// <summary>Le joueur valide son plan : le cycle est résolu de façon synchrone.</summary>
        /// <returns>Vrai si un cycle a été résolu.</returns>
        public bool ValidateCycle()
        {
            if (!hasGame || State != GameState.Planning) return false;
            SetState(GameState.Resolving);
            CycleResult result = cycleManager.Resolve(ref data, config);
            for (int i = 0; i < result.lines.Count; i++) EventBus.RaiseJournalEntry(result.lines[i]);
            if (result.endType != EndType.None)
            {
                SetState(GameState.GameOver);
                Save();
                EventBus.RaiseCycleResolved(result);
                EventBus.RaiseGameOver(result.endType);
            }
            else
            {
                SetState(GameState.Results);
                Save();
                EventBus.RaiseCycleResolved(result);
            }
            return true;
        }

        /// <summary>Le joueur a lu les résultats : retour à la planification.</summary>
        public void AcknowledgeResults()
        {
            if (State == GameState.Results) { SetState(GameState.Planning); Save(); }
        }

        /// <summary>Abandon volontaire : fin de partie neutre avec bilan (spec §11.4).</summary>
        public void Abandon()
        {
            if (!hasGame || State == GameState.GameOver) return;
            data.endType = EndType.Abandon;
            AddJournal("Partie abandonnée.", JournalType.End, Owners.None);
            SetState(GameState.GameOver);
            Save();
            EventBus.RaiseGameOver(EndType.Abandon);
        }

        /// <summary>Efface la sauvegarde et la partie en mémoire.</summary>
        public void DeleteGame()
        {
            SaveManager.Delete(config);
            hasGame = false;
            data = default(GameData);
        }

        /// <summary>Change l'état de la boucle et le publie.</summary>
        /// <param name="state">Nouvel état.</param>
        private void SetState(GameState state)
        {
            data.gameState = (int)state;
            EventBus.RaiseStateChanged(state);
        }
    }
}

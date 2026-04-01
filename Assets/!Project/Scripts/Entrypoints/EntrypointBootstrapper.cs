using System;
using UnityEngine;

namespace RPGProject {
    public class EntrypointBootstrapper : MonoBehaviour {
        [Header("Настройки")]
        [SerializeField] bool autoInitializeOnAwake = true;

        [Header("Entrypoints")]
        [SerializeField] GameEntrypoint[] entrypoints;

        public static EntrypointBootstrapper Instance { get; private set; }
        public EntrypointInstaller Installer { get; private set; }
        public bool IsInitialized { get; private set; }

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if (autoInitializeOnAwake) Initialize();
        }

        public void Initialize() {
            if (IsInitialized) return;

            Installer = new EntrypointInstaller();
            RegisterNonMonoServices();

            if (entrypoints != null) 
                foreach (GameEntrypoint entrypoint in entrypoints)
                    if (entrypoint != null) {
                        Installer.RegisterEntrypoint(entrypoint);
                        RegisterServices(entrypoint);
                    }
            Installer.InitializeAll();
            Installer.StartAll();
            IsInitialized = true;
        }

        void RegisterServices(GameEntrypoint entrypoint) {
            Type type = entrypoint.GetType();

            if (type == typeof(Player)) Installer.Register((Player)entrypoint);
            else if (type == typeof(HUD)) Installer.Register((HUD)entrypoint);
            else if (type == typeof(GameMenu)) Installer.Register((GameMenu)entrypoint);
            else if (type == typeof(MainMenu)) Installer.Register((MainMenu)entrypoint);
            else if (type == typeof(CameraController)) Installer.Register((CameraController)entrypoint);
            else if (type == typeof(GameStateManager)) Installer.Register((GameStateManager)entrypoint);
            else if (type == typeof(GameManager)) Installer.Register((GameManager)entrypoint);
        }

        void RegisterNonMonoServices() {
            SaveService saveService = new SaveService();
            Installer.Register(saveService);
            Installer.RegisterEntrypoint(saveService);
        }

        public void Shutdown() {
            if (!IsInitialized) return;
            Installer?.ShutdownAll();
            Installer?.Clear();
            IsInitialized = false;
        }

        void OnDestroy() {
            if (Instance == this) {
                Shutdown();
                Instance = null;
            }
        }

        void OnApplicationQuit() {
            Shutdown();
        }
    }
}

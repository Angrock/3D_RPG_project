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
                Debug.LogError("[EntrypointBootstrapper] Уже существует другой экземпляр!");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Debug.Log("[EntrypointBootstrapper] Awake вызван");

            if (autoInitializeOnAwake) Initialize();
        }

        public void Initialize() {
            if (IsInitialized) {
                Debug.LogWarning("[EntrypointBootstrapper] Уже инициализирован!");
                return;
            }

            Debug.Log("[EntrypointBootstrapper] Начало инициализации...");
            Debug.Log($"[EntrypointBootstrapper] Entrypoints в массиве: {entrypoints?.Length ?? 0}");

            Installer = new EntrypointInstaller();
            RegisterNonMonoServices();

            if (entrypoints != null) 
                foreach (GameEntrypoint entrypoint in entrypoints) {
                    if (entrypoint != null) {
                        Debug.Log($"[EntrypointBootstrapper] Регистрирую: {entrypoint.GetType().Name}");
                        Installer.RegisterEntrypoint(entrypoint);
                        RegisterServices(entrypoint);
                    }
                    else Debug.LogWarning("[EntrypointBootstrapper] Найден null entrypoint!");
                }
            else Debug.LogWarning("[EntrypointBootstrapper] Массив entrypoints = null!");

            // Сначала инициализируем все entrypoint-ы
            Debug.Log("[EntrypointBootstrapper] Вызов InitializeAll...");
            Installer.InitializeAll();
            // Затем запускаем их
            Debug.Log("[EntrypointBootstrapper] Вызов StartAll...");
            Installer.StartAll();

            IsInitialized = true;
            Debug.Log("[EntrypointBootstrapper] Инициализация завершена!");
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
            Installer.Register<ISaveService>(saveService);
            Installer.RegisterEntrypoint(saveService);
        }

        public void Shutdown() {
            if (!IsInitialized) return;

            Debug.Log("[EntrypointBootstrapper] Остановка всех систем...");
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

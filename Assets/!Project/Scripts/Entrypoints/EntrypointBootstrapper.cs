using System;
using UnityEngine;

namespace RPGProject {
    public class EntrypointBootstrapper : MonoBehaviour {
        [Header("Настройки")]
        [SerializeField] public bool autoInitializeOnAwake = true;

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

        /// <summary>
        /// Отвечает за регистрацию конкретных типов entrypoint в DI-контейнере Installer
        /// </summary>
        void RegisterServices(GameEntrypoint entrypoint)
        {
            // Регистрируем конкретный тип через рефлексию
            Type type = entrypoint.GetType();
            var registerMethod = typeof(EntrypointInstaller)
                .GetMethod("Register")
                .MakeGenericMethod(type);

            registerMethod.Invoke(Installer, new object[] { entrypoint });
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

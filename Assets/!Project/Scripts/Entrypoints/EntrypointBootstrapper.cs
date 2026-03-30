using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Центральный класс управления всеми entrypoint-ами проекта.
    /// Composition Root - единственная точка входа для всех зависимостей.
    /// </summary>
    public class EntrypointBootstrapper : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private bool _autoInitializeOnAwake = true;

        [Header("Entrypoints")]
        [SerializeField] private GameEntrypoint[] _entrypoints;

        private static EntrypointBootstrapper _instance;
        private EntrypointInstaller _installer;
        private bool _isInitialized;

        public static EntrypointBootstrapper Instance => _instance;
        public EntrypointInstaller Installer => _installer;
        public bool IsInitialized => _isInitialized;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError("[EntrypointBootstrapper] Уже существует другой экземпляр!");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("[EntrypointBootstrapper] Awake вызван");

            if (_autoInitializeOnAwake)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[EntrypointBootstrapper] Уже инициализирован!");
                return;
            }

            Debug.Log("[EntrypointBootstrapper] Начало инициализации...");
            Debug.Log($"[EntrypointBootstrapper] Entrypoints в массиве: {_entrypoints?.Length ?? 0}");

            _installer = new EntrypointInstaller();
            RegisterNonMonoServices();

            if (_entrypoints != null)
            {
                foreach (var entrypoint in _entrypoints)
                {
                    if (entrypoint != null)
                    {
                        Debug.Log($"[EntrypointBootstrapper] Регистрирую: {entrypoint.GetType().Name}");
                        _installer.RegisterEntrypoint(entrypoint);
                        RegisterServices(entrypoint);
                    }
                    else
                    {
                        Debug.LogWarning("[EntrypointBootstrapper] Найден null entrypoint!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[EntrypointBootstrapper] Массив _entrypoints = null!");
            }

            // Сначала инициализируем все entrypoint-ы
            Debug.Log("[EntrypointBootstrapper] Вызов InitializeAll...");
            _installer.InitializeAll();
            // Затем запускаем их
            Debug.Log("[EntrypointBootstrapper] Вызов StartAll...");
            _installer.StartAll();

            _isInitialized = true;
            Debug.Log("[EntrypointBootstrapper] Инициализация завершена!");
        }

        private void RegisterServices(GameEntrypoint entrypoint)
        {
            var type = entrypoint.GetType();

            if (type == typeof(Player))
                _installer.Register((Player)entrypoint);
            else if (type == typeof(HUD))
                _installer.Register((HUD)entrypoint);
            else if (type == typeof(GameMenu))
                _installer.Register((GameMenu)entrypoint);
            else if (type == typeof(MainMenu))
                _installer.Register((MainMenu)entrypoint);
            else if (type == typeof(CameraController))
                _installer.Register((CameraController)entrypoint);
            else if (type == typeof(GameStateManager))
                _installer.Register((GameStateManager)entrypoint);
        }

        private void RegisterNonMonoServices()
        {
            var saveService = new SaveService();
            _installer.Register<ISaveService>(saveService);
            _installer.RegisterEntrypoint(saveService);
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            Debug.Log("[EntrypointBootstrapper] Остановка всех систем...");
            _installer?.ShutdownAll();
            _installer?.Clear();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                Shutdown();
                _instance = null;
            }
        }

        private void OnApplicationQuit()
        {
            Shutdown();
        }
    }
}

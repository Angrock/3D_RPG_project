using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Базовый интерфейс для всех entrypoint-ов проекта.
    /// </summary>
    public interface IEntrypoint
    {
        void Initialize();
        void EntrypointStart();
        void Shutdown();
    }

    /// <summary>
    /// Базовый класс для MonoBehaviour entrypoint-ов.
    /// </summary>
    public abstract class GameEntrypoint : MonoBehaviour, IEntrypoint
    {
        protected bool _isInitialized;
        protected bool _isStarted;

        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning($"[{GetType().Name}] Уже инициализирован!");
                return;
            }

            OnInitialize();
            _isInitialized = true;
        }

        public void EntrypointStart()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"[{GetType().Name}] Попытка запуска до инициализации!");
                return;
            }

            if (_isStarted)
            {
                Debug.LogWarning($"[{GetType().Name}] Уже запущен!");
                return;
            }

            OnStart();
            _isStarted = true;
        }

        public void Shutdown()
        {
            if (!_isStarted)
            {
                Debug.LogWarning($"[{GetType().Name}] Попытка остановки до запуска!");
                return;
            }

            OnShutdown();
            _isStarted = false;
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnStart() { }
        protected virtual void OnShutdown() { }

        protected virtual void OnDestroy()
        {
            if (_isStarted)
            {
                Shutdown();
            }
        }
    }
}

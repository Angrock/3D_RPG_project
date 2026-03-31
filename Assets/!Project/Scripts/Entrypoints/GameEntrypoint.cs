using UnityEngine;

namespace RPGProject {
    public interface IEntrypoint {
        void Initialize();
        void EntrypointStart();
        void Shutdown();
    }

    public abstract class GameEntrypoint : MonoBehaviour, IEntrypoint {
        protected bool isInitialized;
        protected bool isStarted;

        public void Initialize() {
            if (isInitialized) {
                Debug.LogWarning($"[{GetType().Name}] Уже инициализирован!");
                return;
            }

            OnInitialize();
            isInitialized = true;
        }

        public void EntrypointStart() {
            if (!isInitialized) {
                Debug.LogError($"[{GetType().Name}] Попытка запуска до инициализации!");
                return;
            }

            if (isStarted) {
                Debug.LogWarning($"[{GetType().Name}] Уже запущен!");
                return;
            }

            OnStart();
            isStarted = true;
        }

        public void Shutdown() {
            if (!isStarted) {
                Debug.LogWarning($"[{GetType().Name}] Попытка остановки до запуска!");
                return;
            }

            OnShutdown();
            isStarted = false;
        }

        protected virtual void OnInitialize() {}
        protected virtual void OnStart() {}
        protected virtual void OnShutdown() {}

        protected virtual void OnDestroy() {
            if (isStarted) Shutdown();
        }
    }
}

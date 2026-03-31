using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject {
    public class EntrypointInstaller {
        readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        readonly List<IEntrypoint> entrypoints = new List<IEntrypoint>();

        public void Register<T>(T service) where T : class {
            Type type = typeof(T);
            if (services.ContainsKey(type)) Debug.LogWarning($"[EntrypointInstaller] Сервис {type.Name} уже зарегистрирован!");
            services[type] = service;
        }

        public T Resolve<T>() where T : class {
            Type type = typeof(T);
            if (!services.TryGetValue(type, out object service)) throw new InvalidOperationException($"[EntrypointInstaller] Сервис {type.Name} не зарегистрирован!");
            return (T)service;
        }

        public bool TryResolve<T>(out T service) where T : class {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object resolved)) {
                service = (T)resolved;
                return true;
            }
            service = null;
            return false;
        }

        public void RegisterEntrypoint(IEntrypoint entrypoint) {
            entrypoints.Add(entrypoint);
        }

        public void InitializeAll() {
            foreach (IEntrypoint entrypoint in entrypoints) entrypoint.Initialize();
        }

        public void StartAll() {
            Debug.Log($"[EntrypointInstaller] StartAll: {entrypoints.Count} entrypoint-ов");
            foreach (IEntrypoint entrypoint in entrypoints) {
                Debug.Log($"[EntrypointInstaller] Вызов EntrypointStart для {entrypoint.GetType().Name}");
                entrypoint.EntrypointStart();
            }
        }

        public void ShutdownAll() {
            for (int i = entrypoints.Count - 1; i >= 0; i--) entrypoints[i].Shutdown();
        }

        public void Clear() {
            services.Clear();
            entrypoints.Clear();
        }
    }
}

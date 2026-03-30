using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Контейнер для регистрации и получения зависимостей.
    /// </summary>
    public class EntrypointInstaller
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly List<IEntrypoint> _entrypoints = new List<IEntrypoint>();

        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[EntrypointInstaller] Сервис {type.Name} уже зарегистрирован!");
            }
            _services[type] = service;
        }

        public T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service))
            {
                throw new InvalidOperationException($"[EntrypointInstaller] Сервис {type.Name} не зарегистрирован!");
            }
            return (T)service;
        }

        public bool TryResolve<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var resolved))
            {
                service = (T)resolved;
                return true;
            }
            service = null;
            return false;
        }

        public void RegisterEntrypoint(IEntrypoint entrypoint)
        {
            _entrypoints.Add(entrypoint);
        }

        public void InitializeAll()
        {
            foreach (var entrypoint in _entrypoints)
            {
                entrypoint.Initialize();
            }
        }

        public void StartAll()
        {
            Debug.Log($"[EntrypointInstaller] StartAll: {_entrypoints.Count} entrypoint-ов");
            foreach (var entrypoint in _entrypoints)
            {
                Debug.Log($"[EntrypointInstaller] Вызов EntrypointStart для {entrypoint.GetType().Name}");
                entrypoint.EntrypointStart();
            }
        }

        public void ShutdownAll()
        {
            for (int i = _entrypoints.Count - 1; i >= 0; i--)
            {
                _entrypoints[i].Shutdown();
            }
        }

        public void Clear()
        {
            _services.Clear();
            _entrypoints.Clear();
        }
    }
}

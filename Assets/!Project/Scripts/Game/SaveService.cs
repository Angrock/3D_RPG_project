using System;
using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Сервис сохранений.
    /// </summary>
    public interface ISaveService
    {
        void SaveGame();
        void LoadGame();
        bool HasSaveData();
        void DeleteSave();
    }

    /// <summary>
    /// Реализация сервиса сохранений.
    /// </summary>
    public class SaveService : ISaveService, IEntrypoint
    {
        private const string SaveKey = "SAVE_DATA";
        private GameData _currentData;
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _currentData = new GameData();
            _isInitialized = true;
            Debug.Log("[SaveService] Инициализирован");
        }

        public void EntrypointStart() { }

        public void Shutdown()
        {
            _currentData = null;
            _isInitialized = false;
        }

        public void SaveGame()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[SaveService] Попытка сохранения до инициализации!");
                return;
            }

            var player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            if (player != null)
            {
                _currentData.playerHP = player.CurrentHP;
                _currentData.playerMP = player.CurrentMP;
                _currentData.playerPosition = player.transform.position;
            }

            string json = JsonUtility.ToJson(_currentData, true);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("[SaveService] Игра сохранена");
        }

        public void LoadGame()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[SaveService] Попытка загрузки до инициализации!");
                return;
            }

            if (!PlayerPrefs.HasKey(SaveKey))
            {
                Debug.LogWarning("[SaveService] Нет данных для загрузки");
                return;
            }

            string json = PlayerPrefs.GetString(SaveKey);
            _currentData = JsonUtility.FromJson<GameData>(json);

            var player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            if (player != null)
            {
                player.transform.position = _currentData.playerPosition;
            }

            Debug.Log("[SaveService] Игра загружена");
        }

        public bool HasSaveData() => PlayerPrefs.HasKey(SaveKey);

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            Debug.Log("[SaveService] Сохранение удалено");
        }
    }

    [Serializable]
    public class GameData
    {
        public float playerHP;
        public float playerMP;
        public Vector3 playerPosition;
    }
}

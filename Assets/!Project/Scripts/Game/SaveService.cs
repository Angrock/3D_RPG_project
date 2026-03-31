using System;
using UnityEngine;

namespace RPGProject {
    public interface ISaveService {
        void SaveGame();
        void LoadGame();
        bool HasSaveData();
        void DeleteSave();
    }

    public class SaveService : ISaveService, IEntrypoint {
        private const string SaveKey = "SAVE_DATA";
        private GameData currentData;
        private bool isInitialized;

        public void Initialize() {
            if (isInitialized) return;
            currentData = new GameData();
            isInitialized = true;
            Debug.Log("[SaveService] Инициализирован");
        }

        public void EntrypointStart() { }

        public void Shutdown() {
            currentData = null;
            isInitialized = false;
        }

        public void SaveGame() {
            if (!isInitialized) {
                Debug.LogError("[SaveService] Попытка сохранения до инициализации!");
                return;
            }

            Player player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            if (player != null) {
                currentData.playerHP = player.CurrentHP;
                currentData.playerMP = player.CurrentMP;
                currentData.playerPosition = player.transform.position;
            }

            string json = JsonUtility.ToJson(currentData, true);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("[SaveService] Игра сохранена");
        }

        public void LoadGame() {
            if (!isInitialized) {
                Debug.LogError("[SaveService] Попытка загрузки до инициализации!");
                return;
            }

            if (!PlayerPrefs.HasKey(SaveKey)) {
                Debug.LogWarning("[SaveService] Нет данных для загрузки");
                return;
            }

            string json = PlayerPrefs.GetString(SaveKey);
            currentData = JsonUtility.FromJson<GameData>(json);

            Player player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            if (player != null) player.transform.position = currentData.playerPosition;

            Debug.Log("[SaveService] Игра загружена");
        }

        public bool HasSaveData() => PlayerPrefs.HasKey(SaveKey);

        public void DeleteSave() {
            PlayerPrefs.DeleteKey(SaveKey);
            Debug.Log("[SaveService] Сохранение удалено");
        }
    }

    [Serializable]
    public class GameData {
        public float playerHP;
        public float playerMP;
        public Vector3 playerPosition;
    }
}

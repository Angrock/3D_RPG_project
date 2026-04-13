using System;
using Enfity.SaveAndLoad;
using UnityEngine;

namespace RPGProject {
    public interface ISaveService {
        void SaveGame();
        void LoadGame();
        void DeleteSave();
    }

    public class SaveService : ISaveService, IEntrypoint {
        const string SaveKey = "GameParameters.enfity";
        bool isInitialized;

        public void Initialize() {
            if (isInitialized) return;
            isInitialized = true;
            InternalParams.SetSaveFileName(SaveKey);
        }

        public void EntrypointStart() { }

        public void Shutdown() {
            isInitialized = false;
        }

        public void SaveGame() {
            if (!isInitialized) return;

            Player player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            GameManager gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();

            InternalParams.SetVector3("PlayerPosition", player.transform.position);
            InternalParams.SetVector3("PlayerRotation", player.transform.rotation.eulerAngles);
            InternalParams.SetFloat("PlayerHP", player.CurrentHP);
            InternalParams.SetFloat("PlayerMP", player.CurrentMP);
            InternalParams.SetInt("PlayerScores", gameManager.Scores);

            InternalParams.SetInt("CountEnemies", gameManager.enemies.Count);
            for (int i = 0; i < gameManager.enemies.Count; i++) {
                InternalParams.SetString($"Enemy{i}Type", gameManager.enemies[i].type.ToString());
                InternalParams.SetVector3($"Enemy{i}Position", gameManager.enemies[i].transform.position);
                InternalParams.SetVector3($"Enemy{i}Rotation", gameManager.enemies[i].transform.rotation.eulerAngles);
                InternalParams.SetFloat($"Enemy{i}HP", gameManager.enemies[i].CurrentHP);
            }
        }

        public void LoadGame() {
            if (!isInitialized) {
                Debug.LogError("[SaveService] Попытка загрузки до инициализации!");
                return;
            }

            Player player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            GameManager gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();

            player.transform.SetPositionAndRotation(InternalParams.GetVector3("PlayerPosition"),
            Quaternion.Euler(InternalParams.GetVector3("PlayerRotation")));
            player.SetNewHP(InternalParams.GetFloat("PlayerHP"));
            player.SetNewMP(InternalParams.GetFloat("PlayerMP"));
            gameManager.Scores = InternalParams.GetInt("PlayerScores");

            int countEnemies = InternalParams.GetInt("CountEnemies");
            for (int i = 0; i < countEnemies; i++) gameManager.CreateEnemy(
                (GameManager.EnemiesTypes)Enum.Parse(
                    typeof(GameManager.EnemiesTypes), InternalParams.GetString($"Enemy{i}Type")),
                InternalParams.GetVector3($"Enemy{i}Position"),
                InternalParams.GetVector3($"Enemy{i}Rotation"),
                InternalParams.GetFloat($"Enemy{i}HP"));
        }

        public void DeleteSave() {
            InternalParams.DeleteAll();
            Debug.Log("[SaveService] Сохранение удалено");
        }
    }
}

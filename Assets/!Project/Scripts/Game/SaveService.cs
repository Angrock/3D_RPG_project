using System;
using System.Collections.Generic;
using Enfity.SaveAndLoad;
using UnityEngine;

namespace RPGProject {
    public interface ISaveService {
        void SaveGame();
        void LoadGame();
        void DeleteSave();
    }

    public class SaveService : ISaveService, IEntrypoint {
        bool isInitialized;

        public void Initialize() {
            if (isInitialized) return;
            isInitialized = true;
            InternalParams.SetSaveFileName(Constants.SaveFileName);
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

            // Создаем копии списков для безопасной итерации
            List<BaseEnemy> enemiesCopy = new List<BaseEnemy>(gameManager.enemies);
            List<BossController> bossesCopy = new List<BossController>(gameManager.Bosses);

            //Debug.Log($"gameManager.enemies = {gameManager.enemies.Count}");
            //Debug.Log($"enemiesCopy = {enemiesCopy.Count}");

            InternalParams.SetInt("CountEnemies", enemiesCopy.Count);
            for (int i = 0; i < enemiesCopy.Count; i++)
            {
                Debug.Log($"saving... {i}");
                if ((enemiesCopy[i] != null) && enemiesCopy[i].IsAlive)
                {
                    //Debug.Log($"saved {i}");
                    //Debug.Log($"name = {enemiesCopy[i].name}, is alive = {enemiesCopy[i].IsAlive}");
                    InternalParams.SetString($"Enemy{i}Type", enemiesCopy[i].type.ToString());
                    InternalParams.SetVector3($"Enemy{i}Position", enemiesCopy[i].transform.position);
                    InternalParams.SetVector3($"Enemy{i}Rotation", enemiesCopy[i].transform.rotation.eulerAngles);
                    InternalParams.SetFloat($"Enemy{i}HP", enemiesCopy[i].CurrentHP);
                }
                else if (InternalParams.HasKeyString($"Enemy{i}Type"))
                {
                    InternalParams.DeleteKeyString($"Enemy{i}Type");
                    InternalParams.DeleteKeyVector3($"Enemy{i}Position");
                    InternalParams.DeleteKeyVector3($"Enemy{i}Rotation");
                    InternalParams.DeleteKeyFloat($"Enemy{i}HP");
                }
            }

            InternalParams.SetInt("CountBosses", bossesCopy.Count);
            for (int i = 0; i < bossesCopy.Count; i++)
            {
                if ((bossesCopy[i] != null) && (bossesCopy[i].health > 0))
                {
                    InternalParams.SetString($"Boss{i}Name", bossesCopy[i].name.Split("(Clone)")[0]);
                    InternalParams.SetVector3($"Boss{i}Position", bossesCopy[i].transform.position);
                    InternalParams.SetVector3($"Boss{i}Rotation", bossesCopy[i].transform.rotation.eulerAngles);
                    InternalParams.SetFloat($"Boss{i}HP", bossesCopy[i].health);
                }
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
            gameManager.InitialEnemiesCount = countEnemies;
            for (int i = 0; i < countEnemies; i++)
            {
                Debug.Log($"i = {i}, has key = {InternalParams.HasKeyString($"Enemy{i}Type")}, |{InternalParams.GetString($"Enemy{i}Type")}|");

                if (InternalParams.HasKeyString($"Enemy{i}Type"))
                {
                    gameManager.CreateEnemy(
                        (GameManager.EnemiesTypes)Enum.Parse(
                            typeof(GameManager.EnemiesTypes), InternalParams.GetString($"Enemy{i}Type")),
                        InternalParams.GetVector3($"Enemy{i}Position"),
                        InternalParams.GetVector3($"Enemy{i}Rotation"),
                        InternalParams.GetFloat($"Enemy{i}HP"));
                }
            }

            int bossesCount = InternalParams.GetInt("CountBosses");
            for (int i = 0; i < bossesCount; i++)
            {
                for (int j = 0; j < gameManager.BossPrefabs.Count; j++)
                {
                    if (gameManager.BossPrefabs[j].name == InternalParams.GetString($"Boss{i}Name"))
                    {
                        BossController boss = gameManager.BossPrefabs[j].GetComponent<BossController>();
                        boss.SetNewHealth(InternalParams.GetFloat($"Boss{i}HP"));

                        gameManager.CreateBoss(
                            boss,
                            InternalParams.GetVector3($"Boss{i}Position"),
                            InternalParams.GetVector3($"Boss{i}Rotation"),
                            InternalParams.GetFloat($"Boss{i}HP"));
                        break;
                    }
                }
            }
        }

        public void DeleteSave() {
            InternalParams.DeleteAll();
            Debug.Log("[SaveService] Сохранение удалено");
        }
    }
}

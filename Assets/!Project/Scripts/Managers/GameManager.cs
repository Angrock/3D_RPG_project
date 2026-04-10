using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject {
    public class GameManager : GameEntrypoint {
        public List<GameObject> enemyPrefabs;
        [NonSerialized] public List<BaseEnemy> enemies;

        public List<GameObject> BossPrefabs;
        [NonSerialized] public List<BossController> Bosses;

        protected override void OnInitialize() {
            enemies = new List<BaseEnemy>();
            Bosses = new List<BossController>();
        }

        protected override void OnStart() {
            if (!Settings.isLoadGame) return;
            SaveService saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<SaveService>();
            saveService.LoadGame();
        }

        public void AddEnemy(BaseEnemy enemy) {
            if (!enemies.Contains(enemy)) enemies.Add(enemy);
        }

        public void RemoveEnemy(BaseEnemy enemy) {
            Destroy(enemy.gameObject);
            enemies.Remove(enemy);
        }

        public void CreateEnemy(EnemiesTypes typeEnemy, Vector3 position, Vector3 rotation, float newHP) {
            if (typeEnemy == EnemiesTypes.Base) return;
            GameObject enemyPrefab = null;
            foreach (GameObject prefab in enemyPrefabs)
                if (prefab.GetComponent<BaseEnemy>().name == "Enemy" + typeEnemy) {
                    Debug.Log(typeEnemy);
                    enemyPrefab = prefab;
                    break;
                }
            Instantiate(enemyPrefab, position, Quaternion.Euler(rotation)).GetComponent<BaseEnemy>().SetNewHP(newHP);
        }

        public void ClearNullEnemies() {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (!enemies[i].IsAlive)
                    StartCoroutine(RemoveEnemyWithDelay(enemies[i]));
            }
        }

        public void RemoveAllEnemies() {
            for (int i = enemies.Count - 1; i >= 0; i--) RemoveEnemy(enemies[i]);
        }

        public void AddBoss(BossController boss)
        {
            if (!Bosses.Contains(boss))
                Bosses.Add(boss);
        }

        public void CheckWin() {
            if (enemies.Count == 0) Debug.Log("Player Win!");
        }

        public enum EnemiesTypes {
            Base,
            Meele,
            Range
        }

        private IEnumerator RemoveEnemyWithDelay(BaseEnemy enemy)
        {
            yield return new WaitForSeconds(5.0f);

            try
            {
                RemoveEnemy(enemy);
            }
            catch (MissingReferenceException exception)
            {
                Debug.LogWarning($"GameManager.RemoveEnemyWithDelay: you are trying to remove already removed enemy: {exception.Message}");
            }

            yield break;
        }
    }
}

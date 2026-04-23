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

        private HUD hud;

        private int scores = 0;
        public int Scores
        {
            get => scores;
            set
            {
                scores = value;
                hud.SetScoresText(scores);
            }
        }

        protected override void OnInitialize() {
            enemies = new List<BaseEnemy>();
            Bosses = new List<BossController>();
            hud = EntrypointBootstrapper.Instance.Installer.Resolve<HUD>();
        }

        protected override void OnStart() {
            if (!Settings.isLoadGame) return;
            SaveService saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<SaveService>();
            saveService.LoadGame();
        }

        public void AddEnemy(BaseEnemy enemy)
        {
            Debug.Log($"AddEnemy: Enemy = {enemy.gameObject.name}, Type = {enemy.type}, MaxHP = {enemy.MaxHP}");
            enemies.Add(enemy);

            //if (!enemies.Contains(enemy))
            //    enemies.Add(enemy);
        }

        public void RemoveEnemy(BaseEnemy enemy) {
            enemies.Remove(enemy);
            Debug.Log("some enemy died and removed");
            Destroy(enemy.gameObject);
        }

        public void CreateEnemy(EnemiesTypes typeEnemy, Vector3 position, Vector3 rotation, float newHP)
        {
            Debug.Log($"Start CreateEnemy: Type = {typeEnemy}, newHP = {newHP}");
            if (typeEnemy == EnemiesTypes.Base)
                return;

            Debug.Log($"Creating enemy ...");
            GameObject enemyPrefab = null;
            foreach (GameObject prefab in enemyPrefabs)
            {
                if (prefab.GetComponent<BaseEnemy>().name == "Enemy" + typeEnemy)
                {
                    enemyPrefab = prefab;
                    break;
                }
            }

            Instantiate(enemyPrefab, position, Quaternion.Euler(rotation)).GetComponent<BaseEnemy>().SetNewHP(newHP);
            Debug.Log($"Enemy created");
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
            Bosses.Add(boss);

            //if (!Bosses.Contains(boss))
            //    Bosses.Add(boss);
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

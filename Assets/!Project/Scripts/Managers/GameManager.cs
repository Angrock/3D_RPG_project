using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject {
    public class GameManager : GameEntrypoint {
        [NonSerialized] public List<BaseEnemy> enemies;

        void Awake() {
            enemies = new List<BaseEnemy>();
        }

        public void AddEnemy(BaseEnemy enemy) {
            if (!enemies.Contains(enemy)) enemies.Add(enemy);
        }

        public void RemoveEnemy(BaseEnemy enemy) {
            Destroy(enemy.gameObject);
            enemies.Remove(enemy);
        }

        public void ClearNullEnemies() {
            for (int i = enemies.Count - 1; i >= 0; i--) if (!enemies[i].IsAlive) RemoveEnemy(enemies[i]);
        }

        public void CheckWin() {
            if (enemies.Count == 0) Debug.Log("Player Win!");
        }
    }
}

using UnityEngine;

namespace RPGProject {
    public class EnemySpawner : GameEntrypoint {
        [System.Serializable]
        public class SpawnPoint {
            public Vector3 position;
            public BaseEnemy enemyPrefab;
        }

        [Header("Настройки спавна")]
        [SerializeField] SpawnPoint[] spawnPoints;
        [SerializeField] bool spawnOnStart = true;

        protected override void OnStart() {
            if (spawnOnStart) SpawnAll();
        }

        public void SpawnAll() {
            foreach (SpawnPoint spawnPoint in spawnPoints)
                if (spawnPoint.enemyPrefab != null)
                    Instantiate(spawnPoint.enemyPrefab, spawnPoint.position, Quaternion.identity);
        }

        public void Spawn(int index) {
            if (index < 0 || index >= spawnPoints.Length) {
                Debug.LogError($"[EnemySpawner] Индекс {index} вне диапазона!");
                return;
            }

            SpawnPoint spawnPoint = spawnPoints[index];
            if (spawnPoint.enemyPrefab != null) Instantiate(spawnPoint.enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}

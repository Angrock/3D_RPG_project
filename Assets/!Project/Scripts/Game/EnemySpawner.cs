using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Спавнер врагов.
    /// </summary>
    public class EnemySpawner : GameEntrypoint
    {
        [System.Serializable]
        public class SpawnPoint
        {
            public Vector3 position;
            public BaseEnemy enemyPrefab;
        }

        [Header("Настройки спавна")]
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private bool _spawnOnStart = true;

        protected override void OnStart()
        {
            if (_spawnOnStart)
                SpawnAll();
        }

        public void SpawnAll()
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint.enemyPrefab != null)
                {
                    Object.Instantiate(spawnPoint.enemyPrefab, spawnPoint.position, Quaternion.identity);
                }
            }
        }

        public void Spawn(int index)
        {
            if (index < 0 || index >= _spawnPoints.Length)
            {
                Debug.LogError($"[EnemySpawner] Индекс {index} вне диапазона!");
                return;
            }

            var spawnPoint = _spawnPoints[index];
            if (spawnPoint.enemyPrefab != null)
            {
                Object.Instantiate(spawnPoint.enemyPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}

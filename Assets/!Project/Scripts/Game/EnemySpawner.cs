using UnityEngine;

namespace RPGProject {
    public class EnemySpawner : GameEntrypoint {
        [System.Serializable]
        public class SpawnPoint {
            public Transform position;
            public GameManager.EnemiesTypes type;
        }

        [System.Serializable]
        public class BossSpawnPoint
        {
            public Transform position;
            public BossController bossPrefab;
        }

        [Header("Spawn Settings")]
        [SerializeField] SpawnPoint[] spawnPoints;
        [SerializeField] private BossSpawnPoint[] bossSpawnPoints;

        private GameManager gameManager;
        private bool isBossSpawned = false;

        protected override void OnStart()
        {
            gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            SpawnAll();
            gameManager.InitialEnemiesCount = spawnPoints.Length;

            gameManager.OnEnemisCountChanged += SpawnBoss;
        }

        public void SpawnAll()
        {
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                if ((spawnPoint.position != null) && (spawnPoint.type != GameManager.EnemiesTypes.Base))
                    Spawn(spawnPoint);
            }
        }

        public void Spawn(SpawnPoint spawnPoint)
        {
            //Debug.Log($"Spawn: Type = {spawnPoint.type}");
            BaseEnemy enemy = null;

            for (int i = 0; i < gameManager.enemyPrefabs.Count; i++)
            {
                enemy = gameManager.enemyPrefabs[i].GetComponent<BaseEnemy>();
                enemy.InitializeValues();

                if (enemy.type == spawnPoint.type)
                    break;
            }

            gameManager.AddEnemy(enemy);
            gameManager.CreateEnemy(
                spawnPoint.type,
                spawnPoint.position.position,
                Vector3.zero,
                enemy.MaxHP);
        }

        private void SpawnBoss(int enemiesCount)
        {
            if ((!isBossSpawned) & (enemiesCount < gameManager.InitialEnemiesCount / 2))
            {
                isBossSpawned = true;

                BossSpawnPoint bossSpawnPoint = bossSpawnPoints[Random.Range(0, bossSpawnPoints.Length)];
                gameManager.CreateBoss(bossSpawnPoint.bossPrefab, bossSpawnPoint.position.position, bossSpawnPoint.position.rotation.eulerAngles);
            }
        }
    }
}
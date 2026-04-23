using UnityEngine;

namespace RPGProject {
    public class EnemySpawner : GameEntrypoint {
        [System.Serializable]
        public class SpawnPoint {
            public Transform position;
            public GameManager.EnemiesTypes type;
        }

        [Header("Spawn Settings")]
        [SerializeField] SpawnPoint[] spawnPoints;
        [SerializeField] bool spawnOnStart = true;

        private GameManager gameManager;

        protected override void OnStart()
        {
            gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();

            if (spawnOnStart)
                SpawnAll();
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
            Debug.Log($"Spawn: Type = {spawnPoint.type}");
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
    }
}
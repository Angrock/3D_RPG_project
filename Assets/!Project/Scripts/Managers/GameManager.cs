using System.Collections.Generic;
using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Менеджер игры. Управляет состоянием игры и врагами.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<BaseEnemy> _enemies = new List<BaseEnemy>();

        public IReadOnlyList<BaseEnemy> Enemies => _enemies;

        private void Awake()
        {
            _enemies = new List<BaseEnemy>();
        }

        public void AddEnemy(BaseEnemy enemy)
        {
            if (!_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
            }
        }

        public void RemoveEnemy(BaseEnemy enemy)
        {
            _enemies.Remove(enemy);
        }

        public void ClearNullEnemies()
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (!_enemies[i].IsAlive)
                {
                    var enemy = _enemies[i];
                    _enemies.RemoveAt(i);
                    Destroy(enemy.gameObject);
                }
            }
        }

        public bool CheckWin()
        {
            return _enemies.Count == 0;
        }
    }
}

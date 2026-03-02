using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public List<BaseEnemy> enemies;

    public static GameManager instance;

    void Awake() {
        instance = this;
    }

    public void ClearNullEnemies() {
        for (int i = 0; i < enemies.Count; i++)
            if (!enemies[i].isAlive) {
                Destroy(enemies[i].gameObject);
                enemies.RemoveAt(i);
                i--;
            }
    }
}

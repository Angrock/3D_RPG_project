using UnityEngine;

public class RangeEnemy : BaseEnemy {
    public override void Attack() {
        Debug.Log("Test text attack range enemy. Range attack - 5-10 m, radius target - 10 m");
    }
}

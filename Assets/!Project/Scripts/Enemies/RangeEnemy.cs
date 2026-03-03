public class RangeEnemy : BaseEnemy {
    protected override void InizializeValues() {
        maxHP = 50f;
        damage = 12f;
        speed = 1.2f;
        attackDistance = 8.8f;
        attackCooldown = 3.25f;
    }
}

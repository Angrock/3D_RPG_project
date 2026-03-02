public class MeeleEnemy : BaseEnemy {
    protected override void InizializeValues() {
        maxHP = 100f;
        damage = 10f;
        speed = 3.4f;
        attackDistance = 0.6f;
        attackCooldown = 1f;
    }
}

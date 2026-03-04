public class MeeleEnemy : BaseEnemy {
    protected override void InizializeValues() {
        maxHP = 100f;
        damage = 5f;
        speed = 3.4f;
        attackDistance = 1.0f;
        attackCooldown = 1.5f;
    }
}

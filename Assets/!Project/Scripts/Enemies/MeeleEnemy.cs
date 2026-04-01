namespace RPGProject {
    public class MeleeEnemy : BaseEnemy {
        protected override void InitializeValues() {
            type = GameManager.EnemiesTypes.Meele;
            MaxHP = 100f;
            Damage = 5f;
            Speed = 3.4f;
            AttackDistance = 1.0f;
            AttackCooldown = 1.5f;
        }
    }
}

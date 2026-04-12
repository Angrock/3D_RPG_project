namespace RPGProject {
    public class MeleeEnemy : BaseEnemy {
        protected override void InitializeValues() {
            base.InitializeValues();
            type = GameManager.EnemiesTypes.Meele;
            MaxHP = 50f;
            Damage = 1.5f;
            Speed = 3.4f;
            AttackDistance = 1.0f;
            AttackCooldown = 1.5f;
            IdleRadius = 5f;
        }
    }
}

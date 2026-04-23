namespace RPGProject {
    public class MeleeEnemy : BaseEnemy {
        public override void InitializeValues() {
            base.InitializeValues();
            type = GameManager.EnemiesTypes.Meele;
            MaxHP = 35f;
            Damage = 1.0f;
            Speed = 3.4f;
            AttackDistance = 1.0f;
            AttackCooldown = 1.5f;
            IdleRadius = 5f;
        }
    }
}

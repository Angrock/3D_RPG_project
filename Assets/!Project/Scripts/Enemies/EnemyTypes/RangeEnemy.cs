namespace RPGProject {
    public class RangeEnemy : BaseEnemy {
        public override void InitializeValues() {
            base.InitializeValues();
            type = GameManager.EnemiesTypes.Range;
            MaxHP = 20f;
            Damage = 1.25f;
            Speed = 1.2f;
            AttackDistance = 6.0f;
            AttackCooldown = 3.5f;
            IdleRadius = 10f;
        }
    }
}

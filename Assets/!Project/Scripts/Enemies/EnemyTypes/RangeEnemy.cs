namespace RPGProject {
    public class RangeEnemy : BaseEnemy {
        protected override void InitializeValues() {
            base.InitializeValues();
            type = GameManager.EnemiesTypes.Range;
            MaxHP = 30f;
            Damage = 3.5f;
            Speed = 1.2f;
            AttackDistance = 8.8f;
            AttackCooldown = 3.25f;
            IdleRadius = 10f;
        }
    }
}

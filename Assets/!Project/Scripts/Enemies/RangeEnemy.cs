namespace RPGProject {
    public class RangeEnemy : BaseEnemy {
        protected override void InitializeValues() {
            type = GameManager.EnemiesTypes.Range;
            MaxHP = 50f;
            Damage = 12f;
            Speed = 1.2f;
            AttackDistance = 8.8f;
            AttackCooldown = 3.25f;
        }
    }
}

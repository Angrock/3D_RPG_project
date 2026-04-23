namespace RPGProject
{
    public class StrongRangeEnemy : BaseEnemy
    {
        public override void InitializeValues()
        {
            base.InitializeValues();
            type = GameManager.EnemiesTypes.StrongRange;
            MaxHP = 20f;
            Damage = 1.4f;
            Speed = 1.2f;
            AttackDistance = 6.5f;
            AttackCooldown = 3.5f;
            IdleRadius = 10f;
        }
    }
}
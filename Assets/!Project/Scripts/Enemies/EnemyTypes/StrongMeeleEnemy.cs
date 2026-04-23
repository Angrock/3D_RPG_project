namespace RPGProject
{
    public class StrongMeleeEnemy : BaseEnemy
    {
        public override void InitializeValues()
        {
            base.InitializeValues();
            type = GameManager.EnemiesTypes.StrongMeele;
            MaxHP = 35f;
            Damage = 1.2f;
            Speed = 3.2f;
            AttackDistance = 0.9f;
            AttackCooldown = 2.0f;
            IdleRadius = 5f;
        }
    }
}
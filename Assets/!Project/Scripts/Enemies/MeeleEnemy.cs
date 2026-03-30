namespace RPGProject
{
    /// <summary>
    /// Враг ближнего боя.
    /// </summary>
    public class MeleeEnemy : BaseEnemy
    {
        protected override void InitializeValues()
        {
            MaxHP = 100f;
            Damage = 5f;
            Speed = 3.4f;
            AttackDistance = 1.0f;
            AttackCooldown = 1.5f;
        }
    }
}

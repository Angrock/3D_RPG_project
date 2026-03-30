namespace RPGProject
{
    /// <summary>
    /// Враг дальнего боя.
    /// </summary>
    public class RangeEnemy : BaseEnemy
    {
        protected override void InitializeValues()
        {
            MaxHP = 50f;
            Damage = 12f;
            Speed = 1.2f;
            AttackDistance = 8.8f;
            AttackCooldown = 3.25f;
        }
    }
}

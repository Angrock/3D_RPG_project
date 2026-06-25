using UnityEngine;

namespace RPGProject
{
    public abstract class EnemyState
    {
        protected BaseEnemy enemy;

        public virtual void EnterState(BaseEnemy enemy)
        {
            this.enemy = enemy;
        }
        public virtual void ExitState() { }
        public virtual void FixedUpdate() { }
    }
}
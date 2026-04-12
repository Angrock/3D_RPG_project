using UnityEngine;

namespace RPGProject
{
    public class EnemyState
    {
        protected BaseEnemy enemy;
        protected EnemyStateMachine stateMachine;

        public EnemyState(BaseEnemy enemy, EnemyStateMachine stateMachine)
        {
            this.enemy = enemy;
            this.stateMachine = stateMachine;
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void FixedUpdate() { }
    }
}

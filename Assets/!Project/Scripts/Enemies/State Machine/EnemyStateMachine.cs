using UnityEngine;

namespace RPGProject
{
    public class EnemyStateMachine
    {
        public EnemyState CurrentEnemyState { get; set; }
        private BaseEnemy enemy;

        public EnemyStateMachine(BaseEnemy enemy)
        {
            this.enemy = enemy;
        }

        public void Initialize(EnemyState startingState)
        {
            CurrentEnemyState = startingState;
            CurrentEnemyState.EnterState(enemy);
        }

        public void ChangeState(EnemyState newState)
        {
            CurrentEnemyState.ExitState();
            CurrentEnemyState = newState;
            CurrentEnemyState.EnterState(enemy);
        }
    }
}
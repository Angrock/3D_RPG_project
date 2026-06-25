using UnityEngine;

namespace RPGProject
{
    public class EnemyAttackState : EnemyState
    {
        private float timeAttack;

        public override void EnterState(BaseEnemy enemy)
        {
            base.EnterState(enemy);
            timeAttack = 5f;
            enemy.agent.isStopped = true;
        }

        public override void ExitState()
        {
            base.ExitState();
            enemy.agent.isStopped = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            timeAttack += Time.fixedDeltaTime;

            if (Settings.IsPeacefulGame)
            {
                enemy.StateMachine.ChangeState(new EnemyGetawayState());
                return;
            }

            if (enemy.CurrentHP <= enemy.MaxHP * enemy.GetawayHPThreshold)
            {
                enemy.StateMachine.ChangeState(new EnemyGetawayState());
                return;
            }

            if (enemy.GetDistanceToPlayer() > enemy.AttackDistance)
            {
                enemy.StateMachine.ChangeState(new EnemyAgressiveState());
                return;
            }

            if (timeAttack >= enemy.AttackCooldown)
            {
                enemy.Attack();
                timeAttack = 0f;
            }
        }
    }
}
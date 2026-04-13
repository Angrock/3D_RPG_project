using UnityEngine;

namespace RPGProject
{
    public class EnemyAttackState : EnemyState
    {
        private float timeAttack;

        public EnemyAttackState(BaseEnemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
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
                stateMachine.ChangeState(enemy.GetawayState);
                return;
            }

            if (enemy.CurrentHP <= enemy.MaxHP * enemy.GetawayHPThreshold)
            {
                stateMachine.ChangeState(enemy.GetawayState);
                return;
            }

            if (enemy.GetDistanceToPlayer() > enemy.AttackDistance)
            {
                stateMachine.ChangeState(enemy.AgressiveState);
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

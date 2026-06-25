using UnityEngine;

namespace RPGProject
{
    public class EnemyAgressiveState : EnemyState
    {
        public override void EnterState(BaseEnemy enemy)
        {
            base.EnterState(enemy);
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

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

            Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;
            float approachDistance = enemy.AttackDistance * 0.8f;
            Vector3 attackPosition = enemy.player.transform.position - directionToPlayer.normalized * approachDistance;

            enemy.MoveTo(attackPosition);

            float distanceToPlayer = enemy.GetDistanceToPlayer();
            if (distanceToPlayer <= enemy.AttackDistance)
            {
                enemy.animator.SetBool("isMove", false);
                enemy.StateMachine.ChangeState(new EnemyAttackState());
            }

            if (enemy.PlayerNotInRange(enemy.player))
            {
                enemy.StateMachine.ChangeState(new EnemyIdleState());
            }
        }
    }
}
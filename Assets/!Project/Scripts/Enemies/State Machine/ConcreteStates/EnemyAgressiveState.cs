using UnityEditorInternal;
using UnityEngine;

namespace RPGProject
{
    public class EnemyAgressiveState : EnemyState
    {
        public EnemyAgressiveState(BaseEnemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (enemy.CurrentHP <= enemy.MaxHP * enemy.GetawayHPThreshold)
            {
                stateMachine.ChangeState(enemy.GetawayState);
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
                stateMachine.ChangeState(enemy.AttackState);
            }

            if (enemy.PlayerNotInRange(enemy.player))
            {
                stateMachine.ChangeState(enemy.IdleState);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

namespace RPGProject
{
    public class EnemyGetawayState : EnemyState
    {
        private Vector3 _targetPosition;

        public override void EnterState(BaseEnemy enemy)
        {
            base.EnterState(enemy);
            NavigateToSafePoint();
            Debug.Log("Entered Getaway State");
        }

        public override void ExitState()
        {
            base.ExitState();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (enemy.AgentHasReachedDestination())
            {
                if (!enemy.PlayerNotInRange(enemy.player))
                {
                    NavigateToSafePoint();
                    return;
                }
                enemy.StateMachine.ChangeState(new EnemyIdleState());
            }
        }

        private void NavigateToSafePoint()
        {
            _targetPosition = FindSafePoint();
            enemy.MoveTo(_targetPosition);
        }

        private Vector3 FindSafePoint()
        {
            Vector3 playerPos = enemy.player.transform.position;
            Vector3 awayDirection = (enemy.transform.position - playerPos).normalized;

            Vector3 idealPoint = enemy.transform.position + awayDirection * enemy.AggressiveDistance;

            float searchRadius = 15f;
            int attempts = 30;

            for (int i = 0; i < attempts; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * searchRadius;
                randomOffset.y = 0;

                Vector3 candidatePoint = enemy.transform.position + randomOffset;

                if (NavMesh.SamplePosition(candidatePoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    float distToPlayer = Vector3.Distance(hit.position, playerPos);
                    if (distToPlayer > enemy.AggressiveDistance)
                    {
                        return hit.position;
                    }
                }
            }

            return idealPoint;
        }
    }
}
using UnityEngine;

namespace RPGProject
{
    public class EnemyIdleState : EnemyState
    {
        private Vector3 _targetPosition;
        public EnemyIdleState(BaseEnemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            MoveToRandomPoint();
            Debug.Log("Entered Idle State");
        }

        public override void ExitState()
        {
            base.ExitState();
            Debug.Log("Exited Idle State");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Settings.IsPeacefulGame)
            {
                if (enemy.AgentHasReachedDestination())
                {
                    MoveToRandomPoint();
                }
                return;
            }

            if (enemy.AgentHasReachedDestination() && enemy.PlayerNotInRange(enemy.player))
            {
                MoveToRandomPoint();
                Debug.DrawLine(enemy.transform.position, _targetPosition, Color.red, 1f);
            }
            else if (!enemy.PlayerNotInRange(enemy.player))
            {
                stateMachine.ChangeState(enemy.AgressiveState);
            }
        }

        private void MoveToRandomPoint()
        {
            _targetPosition = GetRandomPointAroundEnemy(enemy.IdleRadius);
            enemy.MoveTo(_targetPosition);
        }

        private Vector3 GetRandomPointAroundEnemy(float radius = 5f)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            return enemy.transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
        }

        
    }
}

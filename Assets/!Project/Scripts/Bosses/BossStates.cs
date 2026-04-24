using UnityEngine;

namespace RPGProject
{
    public class IdleState : BossState
    {
        private float idleTimer;
        private float idleDuration = 1.5f;

        public override void Enter(BossController boss)
        {
            base.Enter(boss);
            idleTimer = 0;
            boss.animator.SetBool("isIdle", true);
            boss.navMeshAgent.isStopped = true;
        }

        public override void Update()
        {
            // Если увидел игрока - сразу в Attack
            if (boss.CanSeePlayer())
            {
                if ((!Settings.IsPeacefulGame) || (Settings.IsPeacefulGame && boss.isAgressive))
                    boss.stateMachine.ChangeState(new AttackState());
            }

            idleTimer += Time.deltaTime;

            // Переход в Walk через время
            if (idleTimer >= idleDuration)
            {
                boss.stateMachine.ChangeState(new WalkState());
            }
        }

        public override void OnSeePlayer()
        {
            if ((!Settings.IsPeacefulGame) || (Settings.IsPeacefulGame && boss.isAgressive))
                boss.stateMachine.ChangeState(new AttackState());
        }

        public override void Exit()
        {
            base.Exit();
            boss.animator.SetBool("isIdle", false);
        }
    }

    public class WalkState : BossState
    {
        private Vector3 targetPosition;

        public override void Enter(BossController boss)
        {
            base.Enter(boss);
            boss.animator.SetTrigger("Walk");
            boss.navMeshAgent.isStopped = false;
            SetRandomPatrolPoint();
        }

        private void SetRandomPatrolPoint()
        {
            // Случайная точка в радиусе патрулирования
            Vector3 randomDir = Random.insideUnitSphere * boss.patrolRadius;
            randomDir += boss.transform.position;
            UnityEngine.AI.NavMesh.SamplePosition(randomDir, out var hit, boss.patrolRadius, -1);
            targetPosition = hit.position;
            boss.navMeshAgent.SetDestination(targetPosition);
        }

        public override void Update()
        {
            // Если достиг точки патрулирования
            if (!boss.navMeshAgent.pathPending && boss.navMeshAgent.remainingDistance < 0.5f)
            {
                boss.stateMachine.ChangeState(new IdleState());
            }

            // Увидел игрока - атаковать
            if (boss.CanSeePlayer())
            {
                if ((!Settings.IsPeacefulGame) || (Settings.IsPeacefulGame && boss.isAgressive))
                    boss.stateMachine.ChangeState(new AttackState());
            }
        }

        public override void OnSeePlayer()
        {
            if ((!Settings.IsPeacefulGame) || (Settings.IsPeacefulGame && boss.isAgressive))
                boss.stateMachine.ChangeState(new AttackState());
        }
    }

    public class AttackState : BossState
    {
        private float attackCooldown;
        private float attackDelay = 2.19f;
        private bool hasAttacked;
        private bool isStrongAttackNext;

        public override void Enter(BossController boss)
        {
            base.Enter(boss);
            attackCooldown = 0;
            hasAttacked = false;
            boss.navMeshAgent.isStopped = true;

            boss.animator.SetBool("isAttack", true);
            //boss.animator.SetTrigger("Attack");

            isStrongAttackNext = (Random.Range(0, 101) < boss.strongAttackChance) ? true : false;

            Debug.Log($"AttackState -> Enter");
            Debug.Log($"isStrongAttackNext = {isStrongAttackNext}");
        }

        public override void Update()
        {
            attackCooldown += Time.deltaTime;
            boss.transform.LookAt(boss.player.transform, Vector3.up);

            // Атака с задержкой
            if (!hasAttacked && attackCooldown >= attackDelay * 0.5f)
            {
                hasAttacked = true;
                boss.AttackPlayer();
            }

            // После анимации атаки проверяем дистанцию
            if (attackCooldown >= attackDelay)
            {
                if (boss.CanSeePlayer() && boss.IsInAttackRange())
                {
                    // Повторная атака
                    if (isStrongAttackNext)
                    {
                        boss.stateMachine.ChangeState(new StrongAttackState());
                    }
                    else
                    {
                        boss.stateMachine.ChangeState(new AttackState());
                    }
                }
                else if (boss.CanSeePlayer() && !boss.IsInAttackRange())
                {
                    // Подойти ближе
                    boss.stateMachine.ChangeState(new WalkState());
                }
                else
                {
                    // Потерял игрока
                    boss.stateMachine.ChangeState(new IdleState());
                }
            }
        }

        public override void OnLosePlayer()
        {
            boss.stateMachine.ChangeState(new IdleState());
        }

        public override void Exit()
        {
            base.Exit();

            boss.animator.SetBool("isAttack", false);
            Debug.Log($"AttackState -> Exit");
        }
    }

    public class StrongAttackState : BossState
    {
        private float attackCooldown;
        private float attackDelay = 2.19f;
        private bool hasAttacked;

        public override void Enter(BossController boss)
        {
            base.Enter(boss);

            attackCooldown = 0;
            hasAttacked = false;
            boss.navMeshAgent.isStopped = true;
            boss.animator.SetBool("isStrongAttack", true);

            Debug.Log($"StrongAttackState -> Enter");
        }

        public override void Update()
        {
            //Debug.Log($"StrongAttackState: Update");
            attackCooldown += Time.deltaTime;
            boss.transform.LookAt(boss.player.transform, Vector3.up);

            // Атака с задержкой
            if (!hasAttacked && attackCooldown >= attackDelay * 0.5f)
            {
                Debug.Log($"StrongAttackState: Update -> boss strong attacks player");
                hasAttacked = true;
                boss.StrongAttackPlayer();
            }

            // После анимации атаки проверяем дистанцию
            if (attackCooldown >= attackDelay)
            {
                if (boss.CanSeePlayer() && boss.IsInAttackRange())
                {
                    Debug.Log($"StrongAttackState: Update -> switch to ordinary attack");
                    // Обычная атака
                    boss.stateMachine.ChangeState(new AttackState());
                }
                else if (boss.CanSeePlayer() && !boss.IsInAttackRange())
                {
                    // Подойти ближе
                    boss.stateMachine.ChangeState(new WalkState());
                }
                else
                {
                    // Потерял игрока
                    boss.stateMachine.ChangeState(new IdleState());
                }
            }
        }

        public override void OnLosePlayer()
        {
            boss.stateMachine.ChangeState(new IdleState());
        }

        public override void Exit()
        {
            base.Exit();
            boss.animator.SetBool("isStrongAttack", false);

            Debug.Log($"StrongAttackState -> Exit");
        }
    }

    public class DeathState : BossState
    {
        private float destroyDelay = 10f;
        private float timer = 0.0f;

        public override void Enter(BossController boss)
        {
            base.Enter(boss);
            timer = 0;
            boss.animator.SetTrigger("Death");
            boss.navMeshAgent.speed = 0;
            boss.navMeshAgent.isStopped = true;

            // Отключаем коллайдер, чтобы не мешал
            boss.GetComponent<Collider>().enabled = false;

            boss.gameManager.Scores += Constants.BossKillScore;
        }

        public override void Update()
        {
            timer += Time.deltaTime;
            if (timer >= destroyDelay)
            {
                boss.enabled = false; // Отключаем компонент врага
                boss.gameManager.RemoveBoss(boss);
            }
        }

        public override void OnTakeDamage(float damage) { } // Мёртвый не получает урон
    }
}
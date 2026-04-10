using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Codice.CM.Common.CmCallContext;

namespace RPGProject
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] protected Slider sliderHP;

        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public float viewDistance = 3.0f;
        public float attackRange = 4.5f; // attackRange is always must be higher than viewDistance !
        public float patrolRadius = 15f;
        public float health = 500.0f;
        public float damage = 5.0f;

        public Player player;
        public BossStateMachine stateMachine;

        void Start()
        {
            stateMachine = new BossStateMachine(this);
            stateMachine.ChangeState(new IdleState());

            GameManager gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            gameManager.AddBoss(this);

            if (sliderHP != null)
            {
                sliderHP.maxValue = health;
                sliderHP.value = health;
            }
        }

        void Update()
        {
            stateMachine.Update();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        public bool CanSeePlayer()
        {
            if (player == null)
                return false;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            return distance <= viewDistance;
        }

        public bool IsInAttackRange()
        {
            return Vector3.Distance(transform.position, player.transform.position) < attackRange;
        }

        public void AttackPlayer()
        {
            player.TakeDamage(damage);
        }

        public void TakeDamage(float damageAmount)
        {
            health = Mathf.Max(0, health - damageAmount);
            sliderHP.value = health;

            if (health <= 0)
                stateMachine.ChangeState(new DeathState());
        }

        public void OnSeePlayer() => stateMachine.CurrentState?.OnSeePlayer();
        public void OnLosePlayer() => stateMachine.CurrentState?.OnLosePlayer();
    }
}
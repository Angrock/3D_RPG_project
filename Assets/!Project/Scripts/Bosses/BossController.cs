using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPGProject
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private Slider sliderHP;

        public Animator animator;
        public NavMeshAgent navMeshAgent;
        public float viewDistance = 5f; // viewDistance is always must be higher than attackRange !
        public float attackRange = 4f;
        public float patrolRadius = 15f;
        public float health = 350f;
        public float damage = 1.5f;
        public float strongDamage = 2.0f;
        public float strongAttackChance = 25f;

        [NonSerialized] public bool isAgressive = false;
        [NonSerialized] public Player player;
        [NonSerialized] public GameManager gameManager;
        public BossStateMachine stateMachine;

        void Start()
        {
            stateMachine = new BossStateMachine(this);
            stateMachine.ChangeState(new IdleState());

            gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            gameManager.AddBoss(this);

            player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();

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
            //Debug.Log($"AttackPlayer");
            player.TakeDamage(damage);
        }

        public void StrongAttackPlayer()
        {
            //Debug.Log($"         StrongAttackPlayer");
            player.TakeDamage(strongDamage);
        }

        public void TakeDamage(float damageAmount)
        {
            health = Mathf.Max(0, health - damageAmount);
            sliderHP.value = health;

            if (!isAgressive)
                isAgressive = true;

            if (health <= 0)
                stateMachine.ChangeState(new DeathState());
        }

        public void OnSeePlayer() => stateMachine.CurrentState?.OnSeePlayer();
        public void OnLosePlayer() => stateMachine.CurrentState?.OnLosePlayer();
    }
}
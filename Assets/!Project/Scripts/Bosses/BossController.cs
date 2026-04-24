using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPGProject
{
    public class BossController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Slider sliderHP;
        public NavMeshAgent navMeshAgent;
        public Animator animator;

        [Header("Boss Fields")]
        public float viewDistance = 5f; // viewDistance is always must be higher than attackRange !
        public float attackRange = 4f;
        public float patrolRadius = 15f;
        public float health = 350f;
        public float damage = 1.35f;
        public float strongDamage = 1.9f;
        public float strongAttackChance = 40f;

        [NonSerialized] public Player player;
        [NonSerialized] public GameManager gameManager;
        public BossStateMachine stateMachine;

        [NonSerialized] public bool isAgressive = false;

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
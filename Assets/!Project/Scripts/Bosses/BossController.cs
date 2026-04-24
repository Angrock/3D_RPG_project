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

        [Header("Effects")]
        [SerializeField] private List<AttackEfects> bossAttackEfects;

        [Header("Boss Fields")]
        public float viewDistance = 5f; // viewDistance is always must be higher than attackRange !
        public float attackRange = 4f;
        public float patrolRadius = 15f;
        public float health = 350f;
        public float damage = 1.35f;
        public float strongDamage = 1.9f;

        [Header("Chances")]
        public float strongAttackChance = 50f;
        public float effectAttackChance = 60f;

        [NonSerialized] public Player player;
        [NonSerialized] public GameManager gameManager;
        public BossStateMachine stateMachine;

        [NonSerialized] public bool isAgressive = false;
        [HideInInspector] public bool isEffectAttack = false;

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

        public void PickAttackEffect(bool isStrongAttack)
        {
            isEffectAttack = (UnityEngine.Random.Range(0, 101) < effectAttackChance) ? true : false;

            if (isEffectAttack)
            {
                int randomEffectIndex = UnityEngine.Random.Range(0, bossAttackEfects.Count);
                List<ParticleSystem> currentEffects;

                if (isStrongAttack)
                    currentEffects = bossAttackEfects[randomEffectIndex].strongAttackEffects;
                else
                    currentEffects = bossAttackEfects[randomEffectIndex].attackEffects;

                foreach (ParticleSystem effect in currentEffects)
                    effect.Play();
            }
        }

        public void OnSeePlayer() => stateMachine.CurrentState?.OnSeePlayer();
        public void OnLosePlayer() => stateMachine.CurrentState?.OnLosePlayer();

        [Serializable]
        public class AttackEfects
        {
            public List<ParticleSystem> attackEffects;
            public List<ParticleSystem> strongAttackEffects;
        }
    }
}
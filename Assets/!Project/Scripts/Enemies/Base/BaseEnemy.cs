using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPGProject {
    public class BaseEnemy : MonoBehaviour {
        [Header("UI")]
        [SerializeField] protected Slider sliderHP;

        public Animator animator;
        public NavMeshAgent agent;

        public float MaxHP { get; protected set; }
        public float CurrentHP { get; protected set; }
        public float Damage { get; protected set; }
        public float Speed { get; protected set; }
        public float AttackDistance { get; protected set; }
        public float AttackCooldown { get; protected set; }
        public float AggressiveDistance => AttackDistance + IdleRadius;
        public bool IsAlive { get; protected set; }
        public float IdleRadius { get; protected set; }
        public float GetawayHPThreshold { get; protected set; } = 0.25f;
        public virtual GameManager.EnemiesTypes type { get; protected set; }

        public EnemyStateMachine StateMachine { get; set; }
        public EnemyIdleState IdleState { get; set; }
        public EnemyAgressiveState AgressiveState { get; set; }
        public EnemyAttackState AttackState { get; set; }
        public EnemyGetawayState GetawayState { get; set; }

        public bool isAttack;
        
        public Player player;
        bool isInitialized;
        public GameManager gameManager;

        void Awake() {
            
            StateMachine = new EnemyStateMachine();

            IdleState = new EnemyIdleState(this, StateMachine);
            AgressiveState = new EnemyAgressiveState(this, StateMachine);
            AttackState = new EnemyAttackState(this, StateMachine);
            GetawayState = new EnemyGetawayState(this, StateMachine);

            if (isInitialized) return;

            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            InitializeValues();

            if (sliderHP != null) sliderHP.maxValue = MaxHP;
    
            CurrentHP = MaxHP;
            IsAlive = true;
            isAttack = false;

            isInitialized = true;
        }

        void Start() {
            StateMachine.Initialize(IdleState);
            player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();

            gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            gameManager.AddEnemy(this);
        }

        void FixedUpdate() {

            StateMachine.CurrentEnemyState.FixedUpdate();
            // if (!IsAlive) return;

            // if (!isAttack) Move();

            // timeAttack += Time.fixedDeltaTime;
            // if (isAttack && timeAttack >= AttackCooldown) {
            //     isAttack = false;
            //     agent.isStopped = false;
            // }
        }

        // void Update() {
        //     if (!IsAlive) return;
        //     transform.LookAt(player.transform, Vector3.up);
        // }

        // void Move() {
        //     if (Vector3.Distance(transform.position, player.transform.position) < AttackDistance) {
        //         agent.SetDestination(transform.position);
        //         animator.SetBool("isMove", false);

        //         if (!isAttack) Attack();
        //     }
        //     else {
        //         agent.SetDestination(player.transform.position);
        //         animator.SetBool("isMove", true);
        //     }
        // }

        public void MoveTo(Vector3 position) {
            agent.SetDestination(position);
            animator.SetBool("isMove", true);
        }

        public bool AgentHasReachedDestination() {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
                return !agent.hasPath || agent.velocity.sqrMagnitude == 0f;
            }
            return false;
        }

        public void Attack() {
            if (!IsAlive)
                return;

            player.TakeDamage(Damage);

            animator.SetTrigger("TriggerAttack");
            animator.SetBool("isMove", false);
            transform.LookAt(player.transform, Vector3.up);

            isAttack = true;

            Debug.Log($"Enemy {type} attacked player for {Damage} damage.");
        }

        public void TakeDamage(float damage) {
            if (!IsAlive)
                return;

            CurrentHP = Mathf.Max(0, CurrentHP - damage);
            sliderHP.value = CurrentHP;
            if (CurrentHP <= 0) Death();
        }

        public void SetNewHP(float newHP) {
            CurrentHP = Mathf.Clamp(newHP, 0, MaxHP);
            sliderHP.value = CurrentHP;
        }

        protected virtual void InitializeValues() {
            // Initialize any values here
            agent.stoppingDistance = 0.1f;
        }

        protected virtual void Death() {
            gameManager.Scores += Constants.EnemyKillScore;
            animator.SetTrigger("isDeath");
            IsAlive = false;
        }

        public float GetDistanceToPlayer() {
            if (player == null) return float.MaxValue;
            return Vector3.Distance(transform.position, player.transform.position);
        }

        public bool PlayerNotInRange(Player player)
        {
            if (player == null) return true;

            float distanceToPlayer = GetDistanceToPlayer();
            return distanceToPlayer > AggressiveDistance;
        }
    }
}

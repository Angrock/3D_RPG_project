using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPGProject {
    public abstract class BaseEnemy : MonoBehaviour {
        [Header("UI")]
        [SerializeField] protected Slider sliderHP;

        protected Animator animator;
        protected NavMeshAgent agent;

        public float MaxHP { get; protected set; }
        public float CurrentHP { get; protected set; }
        public float Damage { get; protected set; }
        public float Speed { get; protected set; }
        public float AttackDistance { get; protected set; }
        public float AttackCooldown { get; protected set; }
        public bool IsAlive { get; protected set; }

        bool isAttack;
        float timeAttack;
        Player player;
        bool isInitialized;

        void Awake() {
            if (isInitialized) return;

            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();

            InitializeValues();

            if (sliderHP != null) sliderHP.maxValue = MaxHP;

            CurrentHP = MaxHP;
            IsAlive = true;
            isAttack = false;
            timeAttack = 0f;

            isInitialized = true;
            Debug.Log($"[BaseEnemy] Awake вызван для {gameObject.name}");
        }

        void Start() {
            player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            Debug.Log($"[BaseEnemy] Start вызван, Player найден: {player != null}");

            GameManager gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            gameManager?.AddEnemy(this);
            Debug.Log($"[BaseEnemy] {gameObject.name}: Зарегистрирован в GameManager");
        }

        void FixedUpdate() {
            if (!IsAlive) return;

            if (!isAttack) Move();

            timeAttack += Time.fixedDeltaTime;
            if (isAttack && timeAttack >= AttackCooldown) {
                isAttack = false;
                agent.isStopped = false;
            }
        }

        void Update() {
            if (!IsAlive) return;
            transform.LookAt(player.transform, Vector3.up);
        }

        void Move() {
            if (Vector3.Distance(transform.position, player.transform.position) < AttackDistance) {
                agent.SetDestination(transform.position);
                animator.SetBool("isMove", false);

                if (!isAttack) Attack();
            }
            else {
                agent.SetDestination(player.transform.position);
                animator.SetBool("isMove", true);
            }
        }

        public void Attack() {
            Debug.Log($"[BaseEnemy] Атака игрока! Урон: {Damage}");

            player.TakeDamage(Damage);
            agent.isStopped = true;

            animator.SetTrigger("TriggerAttack");
            animator.SetBool("isMove", false);

            isAttack = true;
            timeAttack = 0f;
        }

        public void TakeDamage(float damage) {
            CurrentHP = Mathf.Max(0, CurrentHP - damage);
            sliderHP.value = CurrentHP;
            if (CurrentHP <= 0) Death();
        }

        protected abstract void InitializeValues();

        protected virtual void Death() {
            Debug.Log($"[{gameObject}] Враг умер");
            IsAlive = false;
        }
    }
}

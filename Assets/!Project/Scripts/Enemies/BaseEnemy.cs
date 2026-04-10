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
        public virtual GameManager.EnemiesTypes type { get; protected set; }

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
        }

        void Start() {
            player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();

            GameManager gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            gameManager.AddEnemy(this);
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
            if (!IsAlive)
                return;

            player.TakeDamage(Damage);

            animator.SetTrigger("TriggerAttack");
            animator.SetBool("isMove", false);

            isAttack = true;
            timeAttack = 0f;
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

        protected abstract void InitializeValues();

        protected virtual void Death() {
            animator.SetTrigger("isDeath");
            IsAlive = false;
        }
    }
}

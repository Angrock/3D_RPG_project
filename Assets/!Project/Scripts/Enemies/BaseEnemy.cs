using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPGProject
{
    /// <summary>
    /// Базовый класс врага.
    /// </summary>
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] protected Slider _sliderHP;

        protected Animator _animator;
        protected NavMeshAgent _agent;

        public float MaxHP { get; protected set; }
        public float CurrentHP { get; protected set; }
        public float Damage { get; protected set; }
        public float Speed { get; protected set; }
        public float AttackDistance { get; protected set; }
        public float AttackCooldown { get; protected set; }
        public bool IsAlive { get; protected set; }

        private bool _isAttack;
        private float _timeAttack;
        private Player _player;
        private bool _isInitialized;

        private void Awake()
        {
            if (_isInitialized) return;
            
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            InitializeValues();

            if (_sliderHP != null)
            {
                _sliderHP.maxValue = MaxHP;
            }

            CurrentHP = MaxHP;
            IsAlive = true;
            _isAttack = false;
            _timeAttack = 0f;
            
            _isInitialized = true;
            Debug.Log($"[BaseEnemy] Awake вызван для {gameObject.name}");
        }

        private void Start()
        {
            if (!_isInitialized) Awake();
            
            _player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            Debug.Log($"[BaseEnemy] Start вызван, Player найден: {_player != null}");
            
            // Регистрируем врага в GameManager
            var gameManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameManager>();
            gameManager?.AddEnemy(this);
            Debug.Log($"[BaseEnemy] {gameObject.name}: Зарегистрирован в GameManager");
        }

        private void FixedUpdate()
        {
            if (!IsAlive) return;

            if (!_isAttack)
            {
                Move();
            }

            _timeAttack += Time.fixedDeltaTime;
            if (_isAttack && _timeAttack >= AttackCooldown)
            {
                _isAttack = false;
                _agent.isStopped = false;
            }
        }

        private void Update()
        {
            if (!IsAlive || _player == null) return;
            transform.LookAt(_player.transform, Vector3.up);
        }

        private void Move()
        {
            if (_player == null) return;

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            
            if (distance < AttackDistance)
            {
                // Останавливаемся на дистанции атаки
                _agent.SetDestination(transform.position);
                _animator.SetBool("isMove", false);
                
                if (!_isAttack)
                {
                    Attack();
                }
            }
            else
            {
                // Идём к игроку
                _agent.SetDestination(_player.transform.position);
                _animator.SetBool("isMove", true);
            }
        }

        public void Attack()
        {
            if (_player == null) return;

            Debug.Log($"[BaseEnemy] Атака игрока! Урон: {Damage}");
            
            _player.TakeDamage(Damage);
            _agent.isStopped = true;
            
            if (_animator != null)
            {
                _animator.SetTrigger("TriggerAttack");
                _animator.SetBool("isMove", false);
            }
            
            _isAttack = true;
            _timeAttack = 0f;
        }

        public void TakeDamage(float damage)
        {
            CurrentHP = Mathf.Max(0, CurrentHP - damage);
            if (_sliderHP != null)
            {
                _sliderHP.value = CurrentHP;
            }

            if (CurrentHP <= 0)
            {
                Death();
            }
        }

        protected abstract void InitializeValues();

        protected virtual void Death()
        {
            Debug.Log("[BaseEnemy] Враг умер");
            IsAlive = false;
            // Враг просто умирает, GameManager больше не нужен
        }
    }
}

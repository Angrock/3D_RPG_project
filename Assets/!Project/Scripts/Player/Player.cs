using UnityEngine;

namespace RPGProject
{
    public class Player : GameEntrypoint
    {
        [Header("Атаки")]
        [SerializeField] private AnimationClip _physicAttackClip;
        [SerializeField] private AnimationClip _mageAttackClip;

        [Header("Характеристики")]
        public const float MaxHP = 100f;
        public const float MaxMP = 100f;
        public const float Speed = 3.0f;

        [field: SerializeField] public float PhysicDamage { get; private set; } = 10f;
        [field: SerializeField] public float MageDamage { get; private set; } = 15f;
        [field: SerializeField] public float AttackPhysicDistance { get; private set; } = 1f;
        [field: SerializeField] public float AttackMageDistance { get; private set; } = 8f;
        [field: SerializeField] public float CostSpell { get; private set; } = 20f;

        public float CurrentHP { get; private set; }
        public float CurrentMP { get; private set; }

        public event System.Action<float> OnHPChanged;
        public event System.Action<float> OnMPChanged;
        public event System.Action OnDeath;

        private float _timer;
        private float _currentAttackCooldown;
        private bool _isAttack;

        private Rigidbody _rigidbody;
        private Animator _animator;

        protected override void OnInitialize()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
            _timer = 0;
            _currentAttackCooldown = 0;
            _isAttack = false;
        }

        private void FixedUpdate()
        {
            if (!_isStarted) return;

            Move();
            InputAttacks();
            ChangeTimerAttack();
            RecoverMP(1f / 18f);
        }

        private void Update()
        {
            if (!_isStarted) return;

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                GameMenu.Instance?.SetActiveCursor(!GameMenu.Instance.gameObject.activeSelf);
                GameMenu.Instance?.gameObject.SetActive(!GameMenu.Instance.gameObject.activeSelf);
            }
        }

        private void InputAttacks()
        {
            if (Input.GetKeyDown(Settings.PhysicAttackKey) && !_isAttack)
                PhysicAttack();
            else if (Input.GetKeyDown(Settings.MageAttackKey) && !_isAttack)
                MageAttack();
        }

        private void ChangeTimerAttack()
        {
            if (_timer < _currentAttackCooldown)
            {
                _timer += Time.fixedDeltaTime;
            }
            else if (_timer >= _currentAttackCooldown && _isAttack)
            {
                _isAttack = false;
            }
        }

        private void Move()
        {
            float runEffect = Input.GetKey(Settings.RunKey) ? 2f : 1f;
            float speedX = (Input.GetKey(Settings.LeftwardKey) ? -Speed : Input.GetKey(Settings.RightwardKey) ? Speed : 0) * runEffect;
            float speedZ = (Input.GetKey(Settings.ForwardKey) ? Speed : Input.GetKey(Settings.BackwardKey) ? -Speed : 0) * runEffect;
            float coefficient = (speedX != 0 && speedZ != 0) ? Mathf.Sqrt(2) : 1;
            _rigidbody.linearVelocity = transform.right * (speedX / coefficient) + new Vector3(0, _rigidbody.linearVelocity.y, 0) + transform.forward * (speedZ / coefficient);
            _animator.SetBool("isMove", !(speedX == 0 && speedZ == 0));
        }

        private void PhysicAttack()
        {
            if (_physicAttackClip == null)
            {
                Debug.LogWarning("[Player] _physicAttackClip не назначен!");
                return;
            }

            Debug.Log("Test text phisic attack");
            _timer = 0;
            _currentAttackCooldown = _physicAttackClip.length;
            _isAttack = true;
            _animator.SetTrigger("TriggerPhisycAttack");

            // Находим всех врагов на сцене и атакуем
            var enemies = FindObjectsOfType<BaseEnemy>();
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive && Vector3.Distance(transform.position, enemy.transform.position) < AttackPhysicDistance)
                {
                    enemy.TakeDamage(PhysicDamage);
                }
            }
        }

        private void MageAttack()
        {
            if (CurrentMP < CostSpell) return;

            if (_mageAttackClip == null)
            {
                Debug.LogWarning("[Player] _mageAttackClip не назначен!");
                return;
            }

            SpendMP(CostSpell);
            HUD.Instance?.SetMageCooldown(0);

            Debug.Log("Test text mage attack");
            _timer = 0;
            _currentAttackCooldown = _mageAttackClip.length;
            _isAttack = true;
            _animator.SetTrigger("TriggerMageAttack");

            // Находим всех врагов на сцене и атакуем
            var enemies = FindObjectsOfType<BaseEnemy>();
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive && Vector3.Distance(transform.position, enemy.transform.position) < AttackMageDistance)
                {
                    enemy.TakeDamage(PhysicDamage);
                    break;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            Debug.Log($"[Player] Получен урон: {damage}, было HP: {CurrentHP}");
            Debug.Log($"[Player] HUD.Instance = {HUD.Instance != null}");
            
            CurrentHP = Mathf.Max(0, CurrentHP - damage);
            
            Debug.Log($"[Player] Новое HP: {CurrentHP}");
            
            OnHPChanged?.Invoke(CurrentHP);
            
            if (HUD.Instance != null)
            {
                HUD.Instance.SetHP(CurrentHP);
            }
            else
            {
                Debug.LogWarning("[Player] HUD.Instance = null, не могу обновить HP!");
            }

            if (CurrentHP <= 0)
            {
                Death();
            }
        }

        public void SpendMP(float amount)
        {
            CurrentMP = Mathf.Max(0, CurrentMP - amount);
            OnMPChanged?.Invoke(CurrentMP);
            HUD.Instance?.SetMP(CurrentMP);
        }

        public void RecoverMP(float amount)
        {
            CurrentMP = Mathf.Min(CurrentMP + amount, MaxMP);
            OnMPChanged?.Invoke(CurrentMP);
            HUD.Instance?.SetMP(CurrentMP);
        }

        private void Death()
        {
            Debug.Log("Test text death player");
            OnDeath?.Invoke();
            HUD.Instance?.GameOver();
        }
    }
}

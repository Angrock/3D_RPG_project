using UnityEngine;

namespace RPGProject {
    public class Player : GameEntrypoint {
        [Header("Атаки")]
        [SerializeField] AnimationClip physicAttackClip;
        [SerializeField] AnimationClip mageAttackClip;

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

        float timer;
        float currentAttackCooldown;
        bool isAttack;

        new Rigidbody rigidbody;
        Animator animator;
        
        GameMenu gameMenu;
        GameManager gameManager;
        HUD hud;

        protected override void OnInitialize() {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
            timer = 0;
            currentAttackCooldown = 0;
            isAttack = false;
        }

        protected override void OnStart() {
            gameMenu = EntrypointBootstrapper.Instance.Installer.Resolve<GameMenu>();
            gameManager = EntrypointBootstrapper.Instance.Installer.Resolve<GameManager>();
            hud = EntrypointBootstrapper.Instance.Installer.Resolve<HUD>();
        }

        void FixedUpdate() {
            if (!isStarted) return;

            Move();
            InputAttacks();
            ChangeTimerAttack();
            RecoverMP(1f / 18f);
        }

        void Update() {
            if (!isStarted) return;
            if (Input.GetKeyDown(Settings.GameMenuKey) || Input.GetKeyDown(Settings.AltGameMenuKey))
                gameMenu.SetActive(!gameMenu.gameObject.activeSelf);
        }

        void InputAttacks() {
            if (Input.GetKeyDown(Settings.PhysicAttackKey) && !isAttack) PhysicAttack();
            else if (Input.GetKeyDown(Settings.MageAttackKey) && !isAttack) MageAttack();
        }

        void ChangeTimerAttack() {
            if (timer < currentAttackCooldown) timer += Time.fixedDeltaTime;
            else if (timer >= currentAttackCooldown && isAttack) isAttack = false;
        }

        void Move() {
            float runEffect = Input.GetKey(Settings.RunKey) ? 2f : 1f;
            float speedX = (Input.GetKey(Settings.LeftwardKey) ? -Speed : Input.GetKey(Settings.RightwardKey) ? Speed : 0) * runEffect;
            float speedZ = (Input.GetKey(Settings.ForwardKey) ? Speed : Input.GetKey(Settings.BackwardKey) ? -Speed : 0) * runEffect;
            float coefficient = (speedX != 0 && speedZ != 0) ? Mathf.Sqrt(2) : 1;
            rigidbody.linearVelocity = transform.right * (speedX / coefficient) + new Vector3(0, rigidbody.linearVelocity.y, 0) + transform.forward * (speedZ / coefficient);
            animator.SetBool("isMove", !(speedX == 0 && speedZ == 0));
        }

        void PhysicAttack() {
            Debug.Log("Test text phisic attack");
            timer = 0;
            currentAttackCooldown = physicAttackClip.length;
            isAttack = true;
            animator.SetTrigger("TriggerPhisycAttack");

            foreach (BaseEnemy enemy in gameManager.enemies)
                if (Vector3.Distance(transform.position, enemy.transform.position) < AttackPhysicDistance)
                    enemy.TakeDamage(PhysicDamage);
            gameManager.ClearNullEnemies();
            gameManager.CheckWin();
        }

        void MageAttack() {
            if (CurrentMP < CostSpell) return;

            SpendMP(CostSpell);
            hud.SetMageCooldown(0);

            Debug.Log("Test text mage attack");
            timer = 0;
            currentAttackCooldown = mageAttackClip.length;
            isAttack = true;
            animator.SetTrigger("TriggerMageAttack");

            foreach (BaseEnemy enemy in gameManager.enemies)
                if (Vector3.Distance(transform.position, enemy.transform.position) < AttackMageDistance) {
                    Debug.Log(enemy);
                    enemy.TakeDamage(PhysicDamage);
                    break;
                }
            gameManager.ClearNullEnemies();
            gameManager.CheckWin();
        }

        public void TakeDamage(float damage) {
            Debug.Log($"[Player] Получен урон: {damage}, было HP: {CurrentHP}");

            CurrentHP = Mathf.Max(0, CurrentHP - damage);

            Debug.Log($"[Player] Новое HP: {CurrentHP}");

            hud.SetHP(CurrentHP);

            if (CurrentHP <= 0) Death();
        }

        public void SpendMP(float amount) {
            CurrentMP = Mathf.Max(0, CurrentMP - amount);
            hud.SetMP(CurrentMP);
        }

        public void RecoverMP(float amount) {
            CurrentMP = Mathf.Min(CurrentMP + amount, MaxMP);
            hud.SetMP(CurrentMP);
        }

        void Death() {
            Debug.Log("Test text death player");
            hud.GameOver();
        }
    }
}

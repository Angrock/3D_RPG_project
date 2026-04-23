using System.Collections.Generic;
using UnityEngine;

namespace RPGProject {
    public class Player : GameEntrypoint {
        [Header("Weapons")]
        [SerializeField] private GameObject sword;
        [SerializeField] private GameObject bow;

        [Header("Характеристики")]
        public const float MaxHP = 100f;
        public const float MaxMP = 100f;
        public const float Speed = 3.0f;

        [field: SerializeField] public float PhysicDamage { get; private set; } = 15f;
        [field: SerializeField] public float MageDamage { get; private set; } = 21f;
        [field: SerializeField] public float AttackPhysicDistance { get; private set; } = 1.5f;
        [field: SerializeField] public float AttackMageDistance { get; private set; } = 8f;
        [field: SerializeField] public float CostSpell { get; private set; } = 20f;
        [field: SerializeField] public float MageAttackCooldown { get; private set; } = 3.0f;
        [field: SerializeField] public float PhysicAttackCooldown { get; private set; } = 2.0f;

        [field: SerializeField] public bool IsGodMode { get; private set; } = false;

        public float CurrentHP { get; private set; }
        public float CurrentMP { get; private set; }
        public bool IsAlive { get; private set; }

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

            CurrentHP = (!IsGodMode) ? MaxHP : 9999999.0f;
            CurrentMP = MaxMP;
            timer = 0;
            currentAttackCooldown = 0;
            isAttack = false;
            IsAlive = true;
        }

        protected override void OnStart() {
            gameMenu = EntrypointBootstrapper.Instance.Installer.Resolve<GameMenu>();
            gameManager = EntrypointBootstrapper.Instance.Installer.Resolve<GameManager>();
            hud = EntrypointBootstrapper.Instance.Installer.Resolve<HUD>();
        }

        void FixedUpdate() {
            if (!isStarted) return;
            if (!IsAlive) return;

            Move();
            InputAttacks();
            ChangeTimerAttack();
            RecoverMP(1f / 18f);
        }

        void Update() {
            if (!isStarted) return;
            if (!IsAlive) return;
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
            timer = 0;
            isAttack = true;
            currentAttackCooldown = PhysicAttackCooldown;
            animator.SetTrigger("TriggerPhisycAttack");
            SwapWeapons(true);

            // Создаем копии списков для безопасной итерации
            List<BaseEnemy> enemiesCopy = new List<BaseEnemy>(gameManager.enemies);
            List<BossController> bossesCopy = new List<BossController>(gameManager.Bosses);

            foreach (BaseEnemy enemy in enemiesCopy)
            {
                if ((enemy != null) && (Vector3.Distance(transform.position, enemy.transform.position) < AttackPhysicDistance))
                {
                    enemy.TakeDamage(PhysicDamage);
                    gameManager.Scores += Constants.EnemyDamageScore;
                }
            }

            if (gameManager.Bosses.Count > 0)
            {
                foreach (BossController boss in bossesCopy)
                {
                    if ((boss != null) && (Vector3.Distance(transform.position, boss.transform.position) < AttackPhysicDistance))
                    {
                        boss.TakeDamage(PhysicDamage);
                        gameManager.Scores += Constants.BossDamageScore;
                    }
                }
            }

            gameManager.ClearNullEnemies();
            gameManager.CheckWin();
        }

        void MageAttack() {
            if (CurrentMP < CostSpell) return;

            timer = 0;
            isAttack = true;
            currentAttackCooldown = MageAttackCooldown;
            animator.SetTrigger("TriggerMageAttack");
            SwapWeapons(false);

            SpendMP(CostSpell);
            hud.SetMageCooldown(0);

            // Создаем копии списков для безопасной итерации
            List<BaseEnemy> enemiesCopy = new List<BaseEnemy>(gameManager.enemies);
            List<BossController> bossesCopy = new List<BossController>(gameManager.Bosses);

            foreach (BaseEnemy enemy in enemiesCopy)
            {
                if ((enemy != null) && (Vector3.Distance(transform.position, enemy.transform.position) < AttackMageDistance))
                {
                    enemy.TakeDamage(MageDamage);
                    gameManager.Scores += Constants.EnemyDamageScore;
                    break;
                }
            }

            if (gameManager.Bosses.Count > 0)
            {
                foreach (BossController boss in bossesCopy)
                {
                    if ((boss != null) && (Vector3.Distance(transform.position, boss.transform.position) < AttackMageDistance))
                    {
                        boss.TakeDamage(MageDamage);
                        gameManager.Scores += Constants.BossDamageScore;
                        break;
                    }
                }
            }

            gameManager.ClearNullEnemies();
            gameManager.CheckWin();
        }

        public void TakeDamage(float damage) {
            CurrentHP = Mathf.Max(0, CurrentHP - damage);
            hud.SetHP(CurrentHP);
            if (CurrentHP <= 0) Death();
        }

        public void SetNewHP(float newHP) {
            CurrentHP = Mathf.Clamp(newHP, 0, MaxHP);
            hud.SetHP(CurrentHP);
        }

        public void SpendMP(float amount) {
            CurrentMP = Mathf.Max(0, CurrentMP - amount);
            hud.SetMP(CurrentMP);
        }

        public void RecoverMP(float amount) {
            CurrentMP = Mathf.Min(CurrentMP + amount, MaxMP);
            hud.SetMP(CurrentMP);
        }

        public void SetNewMP(float newMP) {
            CurrentMP = Mathf.Clamp(newMP, 0, MaxMP);
            hud.SetMP(CurrentMP);
        }

        public void SwapWeapons(bool isSword)
        {
            sword.SetActive(isSword);
            bow.SetActive(!isSword);
        }

        void Death() {
            //Debug.Log("Test text death player");
            IsAlive = false;
            gameManager.RemoveAllEnemies();
            hud.GameOver();
        }
    }
}

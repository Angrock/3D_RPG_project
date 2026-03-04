using UnityEngine;

public class Player : MonoBehaviour {
    public AnimationClip physicAttackClip;
    public AnimationClip mageAttackClip;

    public const float maxHP = 100f;
    public const float maxMP = 100f;
    public const float speed = 3.0f;
    public float currentHP { get; private set; }
    public float currentMP { get; private set; }
    public float physicDamage { get; private set; }
    public float mageDamage { get; private set; }
    public float attackPhisycDistance { get; private set; }
    public float attackMageDistance { get; private set; }
    public float costSpell { get; private set; }

    float timer;
    float currentAttackCooldown;
    bool isAttack;

    new Rigidbody rigidbody;
    Animator animator;

    public static Player instance;

    void Awake() {
        instance = this;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        physicDamage = 10f;
        mageDamage = 15f;
        attackPhisycDistance = 1f;
        attackMageDistance = 8f;
        currentHP = maxHP;
        currentMP = maxMP;
        costSpell = 20f;
        timer = 0;
        currentAttackCooldown = 0;
        isAttack = false;
    }

    void Start() {
        HUD.instance.sliderHP.maxValue = maxHP;
        HUD.instance.sliderMP.maxValue = maxMP;
        HUD.instance.sliderMageCooldown.maxValue = mageAttackClip.length;
    }

    void FixedUpdate() {
        Move();
        InputAttacks();
        ChangeTimerAttack();
        RecoverMP(1f / 18f);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            GameMenu.instance.SetActiveCursor(!GameMenu.instance.gameObject.activeSelf);
            GameMenu.instance.gameObject.SetActive(!GameMenu.instance.gameObject.activeSelf);
        }
    }

    void InputAttacks() {
        if (Input.GetKeyDown(Settings.physicAttackKey) && (!isAttack)) PhysicAttack();
        else if (Input.GetKeyDown(Settings.mageAttackKey) && (!isAttack)) MageAttack();
    }

    void ChangeTimerAttack() {
        if (timer < currentAttackCooldown) {
            timer += Time.fixedDeltaTime;
        }
        else if (timer >= currentAttackCooldown && isAttack) {
            isAttack = false;
        }
    }

    void Move() {
        float runEffect = Input.GetKey(Settings.runKey) ? 2f : 1f;
        float speedX = (Input.GetKey(Settings.leftwardKey) ? -speed : Input.GetKey(Settings.rightwardKey) ? speed : 0) * runEffect;
        float speedZ = (Input.GetKey(Settings.forwardKey) ? speed : Input.GetKey(Settings.backwardKey) ? -speed : 0) * runEffect;
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
        foreach (BaseEnemy enemy in GameManager.instance.enemies)
            if (Vector3.Distance(transform.position, enemy.transform.position) < attackPhisycDistance)
                enemy.GetDamage(physicDamage);
        GameManager.instance.ClearNullEnemies();
    }

    void MageAttack() {
        if (currentMP < costSpell) return;
        SpendMP(costSpell);
        HUD.instance.sliderMageCooldown.value = 0;
        Debug.Log("Test text mage attack");
        timer = 0;
        currentAttackCooldown = mageAttackClip.length;
        isAttack = true;
        animator.SetTrigger("TriggerMageAttack");
        foreach (BaseEnemy enemy in GameManager.instance.enemies)
            if (Vector3.Distance(transform.position, enemy.transform.position) < attackMageDistance) {
                enemy.GetDamage(physicDamage);
                break;
            }
        GameManager.instance.ClearNullEnemies();
    }

    public void SetNewHP(float newHP) {
        currentHP = Mathf.Clamp(newHP, 0, maxHP);
        HUD.instance.sliderHP.value = currentHP;
    }

    public void GetDamage(float damageHP) {
        currentHP = Mathf.Max(0, currentHP - damageHP);
        HUD.instance.sliderHP.value = currentHP;
        if (currentHP <= 0) Death();
    }

    public void SetNewMP(float newMP) {
        currentMP = Mathf.Clamp(newMP, 0, maxMP);
        HUD.instance.sliderMP.value = currentMP;
    }

    public void SpendMP(float spendValue) {
        currentMP = Mathf.Max(0, currentMP - spendValue);
        HUD.instance.sliderMP.value = currentMP;
    }

    public void RecoverMP(float recoverValue) {
        currentMP = Mathf.Min(currentMP + recoverValue, maxMP);
        HUD.instance.sliderMP.value = currentMP;
    }

    void Death() {
        Debug.Log("Test text death player");
        HUD.instance.GameOver();
    }
}

using UnityEngine;

public class Player : MonoBehaviour {
    public const float maxHP = 100f;
    public const float speed = 3.0f;
    public float currentHP { get; private set; }
    public float physicDamage { get; private set; }
    public float mageDamage { get; private set; }
    public float attackPhisycDistance { get; private set; }
    public float attackMageDistance { get; private set; }

    new Rigidbody rigidbody;
    Animator animator;
    new Camera camera;

    public static Player instance;

    void Awake() {
        instance = this;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        physicDamage = 10f;
        mageDamage = 15f;
        attackPhisycDistance = 1f;
        attackMageDistance = 8f;
        currentHP = maxHP;
    }

    void Start() {
        HUD.instance.sliderHP.maxValue = maxHP;
    }

    void FixedUpdate() {
        Move();
        if (Input.GetKey(Settings.physicAttackKey)) PhysicAttack();
        else if (Input.GetKey(Settings.mageAttackKey)) MageAttack();
    }

    void Move() {
        float runEffect = Input.GetKey(Settings.runKey) ? 2f : 1f;
        float speedX = (Input.GetKey(Settings.leftwardKey) ? -speed : Input.GetKey(Settings.rightwardKey) ? speed : 0) * runEffect;
        float speedZ = (Input.GetKey(Settings.forwardKey) ? speed : Input.GetKey(Settings.backwardKey) ? -speed : 0) * runEffect;
        float coefficient = (speedX != 0 && speedZ != 0) ? Mathf.Sqrt(2) : 1;
        rigidbody.linearVelocity = camera.transform.right * (speedX / coefficient) + new Vector3(0, rigidbody.linearVelocity.y, 0) + camera.transform.forward * (speedZ / coefficient);
        animator.SetBool("isMove", !(speedX == 0 && speedZ == 0));
    }

    void PhysicAttack() {
        Debug.Log("Test text phisic attack");
        animator.SetTrigger("TriggerPhisycAttack");
        foreach (BaseEnemy enemy in GameManager.instance.enemies)
            if (Vector3.Distance(transform.position, enemy.transform.position) < attackPhisycDistance)
                enemy.GetDamage(physicDamage);
        GameManager.instance.ClearNullEnemies();
    }

    void MageAttack() {
        Debug.Log("Test text mage attack");
        animator.SetTrigger("TriggerMageAttack");
        foreach (BaseEnemy enemy in GameManager.instance.enemies)
            if (Vector3.Distance(transform.position, enemy.transform.position) < attackMageDistance) {
                enemy.GetDamage(physicDamage);
                break;
            }
        GameManager.instance.ClearNullEnemies();
    }

    public void GetDamage(float damageHP) {
        currentHP = Mathf.Max(0, currentHP - damageHP);
        HUD.instance.sliderHP.value = currentHP;
        if (currentHP <= 0) Death();
    }

    void Death() {
        Debug.Log("Test text death player");
        HUD.instance.GameOver();
    }
}

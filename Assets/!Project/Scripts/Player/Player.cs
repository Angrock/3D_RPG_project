using UnityEngine;

public class Player : MonoBehaviour {
    public const float maxHP = 100f;
    public const float speed = 4.4f;
    public float currentHP { get; private set; }
    public float physicDamage { get; private set; }
    public float mageDamage { get; private set; }

    new Rigidbody rigidbody;

    public static Player instance;

    void Awake() {
        instance = this;
        rigidbody = GetComponent<Rigidbody>();
        physicDamage = 10f;
        mageDamage = 15f;
        currentHP = maxHP;
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
        rigidbody.linearVelocity = new Vector3(speedX / coefficient, rigidbody.linearVelocity.y, speedZ / coefficient);
    }

    void PhysicAttack() {
        Debug.Log("Test text phisic attack");
    }

    void MageAttack() {
        Debug.Log("Test text mage attack");
    }

    public void GetDamage(float damageHP) {
        currentHP = Mathf.Max(0, currentHP - damageHP);
        if (currentHP <= 0) Death();
    }

    void Death() {
        Debug.Log("Test text death player");
        MainMenu.instance.GameOver();
    }
}

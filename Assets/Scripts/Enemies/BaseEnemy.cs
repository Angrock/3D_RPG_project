using UnityEngine;
using UnityEngine.UI;

public abstract class BaseEnemy : MonoBehaviour {
    [SerializeField] Slider sliderHP;

    public float maxHP { get; protected set; }
    public float currentHP { get; protected set; }
    public float damage { get; protected set; }
    public float speed { get; protected set; }

    void Awake() {
        maxHP = 100f;
        currentHP = maxHP;
        damage = 10f;
        speed = 3f;
    }

    void FixedUpdate() {
        Move();
    }

    void Move() {
        Debug.Log("Test text move enemy. Need connect NavAgent and implement target player");
    }

    public abstract void Attack();

    public void GetDamage(float damageHP) {
        currentHP = Mathf.Max(0, currentHP - damageHP);
        sliderHP.value = currentHP / maxHP;
        if (currentHP <= 0) Death();
    }

    void Death() {
        Debug.Log("Test death enemy. Need add death animation");
        Destroy(gameObject);
    }
}

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class BaseEnemy : MonoBehaviour {
    [SerializeField] Slider sliderHP;
    Animator animator;
    NavMeshAgent agent;

    public float maxHP { get; protected set; }
    public float currentHP { get; protected set; }
    public float damage { get; protected set; }
    public float speed { get; protected set; }
    public float attackDistance { get; protected set; }
    public float attackCooldown { get; protected set; }
    
    bool isAttack;
    float timeAttack;

    void Awake() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        maxHP = 100f;
        currentHP = maxHP;
        damage = 10f;
        speed = 3f;
        attackDistance = 0.6f;
        attackCooldown = 1f;
        isAttack = false;
        timeAttack = 0f;
    }

    void FixedUpdate() {
        if (!isAttack) Move();
        CheckAbilityAttack();
        timeAttack += Time.fixedDeltaTime;
        if (isAttack && timeAttack >= attackCooldown) isAttack = false;
    }

    void Move() {
        Debug.Log("Test text move enemy. Need connect NavAgent and implement target player");
        agent.SetDestination(Player.instance.transform.position);
        animator.SetBool("isMove", true);
    }

    void CheckAbilityAttack() {
        if (Vector3.Distance(transform.position, Player.instance.transform.position) > attackDistance) return;
        Attack();
        animator.SetTrigger("TriggerAttack");
        animator.SetBool("isMove", false);
        isAttack = true;
        timeAttack = 0f;
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

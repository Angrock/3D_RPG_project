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
    public bool isAlive { get; protected set; }
    
    bool isAttack;
    float timeAttack;

    void Awake() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        InizializeValues();
        sliderHP.maxValue = maxHP;
        currentHP = maxHP;
        isAlive = true;
        isAttack = false;
        timeAttack = 0f;
    }

    protected abstract void InizializeValues();

    void FixedUpdate() {
        if (!isAlive) return;
        if (!isAttack) Move();
        timeAttack += Time.fixedDeltaTime;
        if (isAttack && timeAttack >= attackCooldown) {
            isAttack = false;
            agent.isStopped = false;
        }
    }

    private void Update() {
        transform.LookAt(Player.instance.transform, Vector3.up);
    }

    void Move() {
        agent.SetDestination(Player.instance.transform.position);
        animator.SetBool("isMove", true);
        if (Vector3.Distance(transform.position, Player.instance.transform.position) < attackDistance) Attack();
    }

    public void Attack() {
        Player.instance.GetDamage(damage);
        agent.isStopped = true;
        animator.SetTrigger("TriggerAttack");
        animator.SetBool("isMove", false);
        isAttack = true;
        timeAttack = 0f;
    }

    public void GetDamage(float damageHP) {
        currentHP = Mathf.Max(0, currentHP - damageHP);
        sliderHP.value = currentHP;
        if (currentHP <= 0) Death();
    }

    void Death() {
        Debug.Log("Test death enemy. Need add death animation");
        isAlive = false;
    }
}

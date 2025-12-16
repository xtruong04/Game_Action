using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float chaseRange = 5f;

    [Header("Attack Settings")]
    [SerializeField] protected float attackRange = 1f;        // Tầm gây damage
    [SerializeField] protected float attackStopRange = 0.8f;    // Khoảng dừng lại để đánh
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected int attackDamage = 10;

    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 50;
    protected int currentHealth;

    [Header("UI Settings")]
    [SerializeField] private EnemyHealthUI healthUI;
    [SerializeField] private GameObject healthBarCanvas;

    protected Player player;
    protected Animator animator;
    protected float lastAttackTime = -999f;

    protected enum EnemyState { Idle, Chase, Attack, Die }
    protected EnemyState currentState = EnemyState.Idle;

    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (healthUI != null)
            healthUI.SetMaxHealth(maxHealth);

        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(false);
    }

    protected virtual void Update()
    {
        if (player == null || currentState == EnemyState.Die)
            return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= attackRange)
            ChangeState(EnemyState.Attack);
        else if (distance <= chaseRange)
            ChangeState(EnemyState.Chase);
        else
            ChangeState(EnemyState.Idle);

        HandleState(distance);
    }

    protected void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    protected void HandleState(float distance)
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("isWalking", false);
                break;

            case EnemyState.Chase:
                animator.SetBool("isWalking", true);

                if (distance > attackStopRange)
                    MoveToPlayer();

                break;

            case EnemyState.Attack:
                animator.SetBool("isWalking", false);
                TryAttack(distance);
                break;
        }
    }

    protected void MoveToPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.transform.position,
            moveSpeed * Time.deltaTime
        );

        Flip();
    }

    protected void Flip()
    {
        float dir = player.transform.position.x - transform.position.x;
        transform.localScale = dir < 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
    }

    protected void TryAttack(float distance)
    {
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetBool("isAttacking", true);
        }
        else
        {
            // Khi ra khỏi tầm hoặc chưa cooldown thì tắt attack
            animator.SetBool("isAttacking", false);
        }
    }

    // ===== Animation Event =====
    public void DealDamage()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= attackRange)
        {
            player.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Chỉ gọi trigger một lần khi bị đánh
        animator.SetTrigger("Hurt");

        if (healthBarCanvas != null && !healthBarCanvas.activeSelf)
            healthBarCanvas.SetActive(true);

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }



    protected virtual void Die()
    {
        currentState = EnemyState.Die;
        animator.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }

    // ===== Gizmos =====
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackStopRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}

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

    [SerializeField] private GameObject soulItemPrefab; 
    [SerializeField] private GameObject healthItemPrefab; [SerializeField] private GameObject damageBuffItemPrefab; 
    [Range(0f, 1f)][SerializeField] private float healthDropChance = 0.3f; // 30%
    [Range(0f, 1f)][SerializeField] private float buffDropChance = 0.2f; // 20%
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
        GameManager.Instance.RegisterEnemy();
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

    protected virtual void TryAttack(float distance) 
    { 
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown) 
        {
            lastAttackTime = Time.time; 
            animator.SetTrigger("Attack1"); 
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

    public virtual void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Die) return; // tránh gọi khi đã chết

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (healthBarCanvas != null && !healthBarCanvas.activeSelf)
            healthBarCanvas.SetActive(true);

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            currentState = EnemyState.Die; // đánh dấu đã chết
            GameManager.Instance.OnEnemyKilled(); // báo chết một lần duy nhất
            Die();
        }
    }




    protected virtual void Die()
    {
        currentState = EnemyState.Die; 
        animator.SetTrigger("Die"); // Luôn rớt SoulItem
        if (soulItemPrefab != null)
        { 
            Instantiate(soulItemPrefab, transform.position, Quaternion.identity);
        }
        // Random rớt HealthItem
        if (healthItemPrefab != null && Random.value <= healthDropChance)
        { 
            Instantiate(healthItemPrefab, transform.position, Quaternion.identity); 
        } 
        // Random rớt DamageBuffItem
        if (damageBuffItemPrefab != null && Random.value <= buffDropChance)
        { 
            Instantiate(damageBuffItemPrefab, transform.position, Quaternion.identity); 
        }

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

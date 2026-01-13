using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float chaseRange = 5f;

    [Header("Attack Settings")]
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackStopRange = 1f;
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected int attackDamage = 10;

    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 50;
    protected int currentHealth;

    [Header("UI Settings")]
    [SerializeField] private EnemyHealthUI healthUI;
    [SerializeField] private GameObject healthBarCanvas;

    [Header("Drop Items")]
    [SerializeField] private GameObject soulItemPrefab;
    [SerializeField] private GameObject healthItemPrefab;
    [SerializeField] private GameObject damageBuffItemPrefab;
    [Range(0f, 1f)][SerializeField] private float healthDropChance = 0.3f;
    [Range(0f, 1f)][SerializeField] private float buffDropChance = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip dieSound;

    protected Transform playerTransform;
    protected Animator animator;
    protected float lastAttackTime = -999f;

    protected enum EnemyState { Idle, Chase, Attack, Die }
    protected EnemyState currentState = EnemyState.Idle;

    // ===== START =====
    protected virtual void Start()
    {
        // Tìm player theo tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("Player not found in scene! Make sure it has tag 'Player'.");

        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (healthUI != null)
            healthUI.SetMaxHealth(maxHealth);

        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(false);

        GameManager.Instance.RegisterEnemy();
    }

    // ===== UPDATE =====
    protected virtual void Update()
    {
        if (playerTransform == null || currentState == EnemyState.Die)
            return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
            ChangeState(EnemyState.Attack);
        else if (distance <= chaseRange)
            ChangeState(EnemyState.Chase);
        else
            ChangeState(EnemyState.Idle);

        HandleState(distance);
    }

    // ===== STATE MANAGEMENT =====
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

    // ===== MOVEMENT =====
    protected void MoveToPlayer()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTransform.position,
            moveSpeed * Time.deltaTime
        );
        Flip();
    }

    protected void Flip()
    {
        float dir = playerTransform.position.x - transform.position.x;
        transform.localScale = new Vector3(Mathf.Sign(dir), 1, 1);
    }

    // ===== ATTACK =====
    protected virtual void TryAttack(float distance)
    {
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack1");
        }
    }

    // Animation Event gọi khi hit
    public void DealDamage()
    {
        if (playerTransform == null) return;

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance <= attackRange)
        {
            // Hỗ trợ cả Player (knight) và PlayerMage
            Player p = playerTransform.GetComponent<Player>();
            if (p != null) p.TakeDamage(attackDamage);

            PlayerMage pm = playerTransform.GetComponent<PlayerMage>();
            if (pm != null) pm.TakeDamage(attackDamage);
        }
    }

    // ===== TAKE DAMAGE =====
    public virtual void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Die) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (hurtSound != null && audioSource != null)
            audioSource.PlayOneShot(hurtSound);

        if (healthBarCanvas != null && !healthBarCanvas.activeSelf)
            healthBarCanvas.SetActive(true);

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            currentState = EnemyState.Die;
            GameManager.Instance.OnEnemyKilled();
            Die();
        }
    }

    // ===== DIE =====
    protected virtual void Die()
    {
        currentState = EnemyState.Die;

        if (dieSound != null && audioSource != null)
            audioSource.PlayOneShot(dieSound);

        animator.SetTrigger("Die");

        // Drop items
        if (soulItemPrefab != null)
            Instantiate(soulItemPrefab, transform.position, Quaternion.identity);

        if (healthItemPrefab != null && Random.value <= healthDropChance)
            Instantiate(healthItemPrefab, transform.position, Quaternion.identity);

        if (damageBuffItemPrefab != null && Random.value <= buffDropChance)
            Instantiate(damageBuffItemPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, 1f);
    }

    // ===== GIZMOS =====
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

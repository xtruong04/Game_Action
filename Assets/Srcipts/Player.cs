using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Damage Settings")]
    [SerializeField] private float hurtCooldown = 0.5f;
    private float lastHurtTime = -999f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private int extraAttackDamage = 30;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float comboResetTime = 0.3f;

    private int clickCount = 0;
    private float lastClickTime = 0f;

    [Header("UI Settings")]
    [SerializeField] private PlayerHealthUI healthUI;
    [SerializeField] private GameOverUI gameOverUI;

    public int soulCount = 0;          // số soul hiện tại
    public int soulsRequired = 0;      // số soul cần để triệu hồi boss

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        healthUI.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        MovePlayer();
        HandleAttackInput();
    }

    void MovePlayer()
    {
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool isMoving = playerInput.magnitude > 0.1f;
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);

        float currentMoveSpeed = walkSpeed;
        bool isCurrentlyRunning = false;

        if (isRunningInput && isMoving)
        {
            currentMoveSpeed = runSpeed;
            isCurrentlyRunning = true;
        }
        else if (isMoving)
        {
            currentMoveSpeed = walkSpeed;
        }

        rb.linearVelocity = playerInput.normalized * currentMoveSpeed;

        if (playerInput.x < 0)
            spriteRenderer.flipX = true;
        else if (playerInput.x > 0)
            spriteRenderer.flipX = false;

        animator.SetBool("isWalk", isMoving);
        animator.SetBool("isRun", isCurrentlyRunning);
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            lastClickTime = Time.time;

            if (clickCount == 1)
            {
                animator.SetTrigger("Attack");
                PerformAttack(attackDamage, attackRange);
            }
            else if (clickCount == 2 && Time.time - lastClickTime <= comboResetTime)
            {
                animator.SetTrigger("AttackExtra");
                PerformAttack(extraAttackDamage, attackRange);
                clickCount = 0; // reset combo sau khi đánh extra
            }
        }

        if (Time.time - lastClickTime > comboResetTime)
        {
            clickCount = 0;
        }
    }

    void PerformAttack(int damage, float range)
    {
        // Xác định hướng nhân vật
        bool isFacingRight = !spriteRenderer.flipX;

        // Tính vị trí vùng đánh phía trước mặt
        Vector2 attackOrigin = (Vector2)transform.position + new Vector2(isFacingRight ? range * 0.5f : -range * 0.5f, 0);
        Vector2 boxSize = new Vector2(range, 1f); // vùng đánh hình chữ nhật

        // Kiểm tra enemy trong vùng đánh
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackOrigin, boxSize, 0f);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastHurtTime < hurtCooldown) return;
        lastHurtTime = Time.time;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        Debug.Log("Player takes " + damage + " damage! Current HP: " + currentHealth);

        healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    // ===== Heal =====
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthUI.SetHealth(currentHealth);
        Debug.Log("Player healed. Current HP: " + currentHealth);
    }

    // ===== Damage Buff =====
    public IEnumerator DamageBuff(int extraDamage, float duration)
    {
        attackDamage += extraDamage;
        Debug.Log("Damage buffed! Current damage: " + attackDamage);

        yield return new WaitForSeconds(duration);

        attackDamage -= extraDamage;
        Debug.Log("Damage buff ended. Current damage: " + attackDamage);
    }

    public void AddSoul(int amount)
    {
        soulCount += amount;

       
    }
    private void Die()
    {
        animator.SetTrigger("Die");
        gameOverUI.ShowGameOver();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (spriteRenderer == null) return;
        bool isFacingRight = !spriteRenderer.flipX;

        Vector2 attackOrigin = (Vector2)transform.position + new Vector2(isFacingRight ? attackRange * 0.5f : -attackRange * 0.5f, 0);
        Vector2 boxSize = new Vector2(attackRange, 1f);

        Gizmos.DrawWireCube(attackOrigin, boxSize);
    }
}

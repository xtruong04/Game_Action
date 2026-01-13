using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IHealable 
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
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float extraCooldown = 2f;

    private float lastAttackTime = -999f;
    private float lastExtraTime = -999f;


    [Header("UI Settings")]
    [SerializeField] private PlayerHealthUI healthUI;
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Soul Settings")]
    public int soulCount = 0;
    public int soulsRequired = 0;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip extraAttackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip damageBuffSound;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

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
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = input.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        float speed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = input.normalized * speed;

        if (input.x < 0) spriteRenderer.flipX = true;
        else if (input.x > 0) spriteRenderer.flipX = false;

        animator.SetBool("isWalk", isMoving);
        animator.SetBool("isRun", isRunning);
    }

    void HandleAttackInput()
    {
        // Chuột trái – Đánh thường
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < lastAttackTime + attackCooldown)
                return;

            lastAttackTime = Time.time;

            animator.SetTrigger("Attack");
            PlaySound(attackSound);
            PerformAttack(attackDamage);
        }

        // Chuột phải – Đánh Extra
        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time < lastExtraTime + extraCooldown)
                return;

            lastExtraTime = Time.time;

            animator.SetTrigger("AttackExtra");
            PlaySound(extraAttackSound);
            PerformAttack(extraAttackDamage);
        }
    }


    void PerformAttack(int damage)
    {
        bool facingRight = !spriteRenderer.flipX;

        Vector2 origin = (Vector2)transform.position +
                         new Vector2(facingRight ? attackRange * 0.5f : -attackRange * 0.5f, 0);
        Vector2 size = new Vector2(attackRange, 1f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, size, 0f);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

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
        PlaySound(hurtSound);

        healthUI.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthUI.SetHealth(currentHealth);
        PlaySound(healSound);
    }

    public IEnumerator DamageBuff(int extraDamage, float duration)
    {
        attackDamage += extraDamage;
        PlaySound(damageBuffSound);

        yield return new WaitForSeconds(duration);

        attackDamage -= extraDamage;
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

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        if (spriteRenderer == null) return;

        Gizmos.color = Color.red;
        bool facingRight = !spriteRenderer.flipX;

        Vector2 origin = (Vector2)transform.position +
                         new Vector2(facingRight ? attackRange * 0.5f : -attackRange * 0.5f, 0);
        Vector2 size = new Vector2(attackRange, 1f);

        Gizmos.DrawWireCube(origin, size);
    }
}

using System.Collections;
using UnityEngine;

public class PlayerMage : MonoBehaviour, IHealable
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
    [SerializeField] private GameObject normalFireball;
    [SerializeField] private GameObject extraFireball;
    [SerializeField] private Transform firePoint;

    [SerializeField] private float extraCooldown = 3f;
    private float lastExtraTime = -999f;
    [SerializeField] private float fireCooldown = 0.5f;
    private float lastFireTime = -999f;
    [Header("Soul Settings")]
    public int soulCount = 0;
    public int soulsRequired = 0;

    [Header("UI Settings")]
    [SerializeField] private PlayerHealthUI healthUI;
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip extraAttackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip damageBuffSound;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentHealth = maxHealth;
        if (healthUI != null)
            healthUI.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        Move();
        HandleAttackInput();
        RotateFirePoint();
    }

    // ===== MOVE =====
    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = input.magnitude > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        rb.velocity = input.normalized * (isRunning ? runSpeed : walkSpeed);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = mouseWorldPos.x < transform.position.x
            ? new Vector3(-1, 1, 1)
            : new Vector3(1, 1, 1);

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
    }

    // ===== ATTACK INPUT =====
    void HandleAttackInput()
    {
        bool isMoving = animator.GetBool("isMoving");
        bool isRunning = animator.GetBool("isRunning");

        // Chu?t trái – Fireball th??ng
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < lastFireTime + fireCooldown)
                return; // ch?a h?t cooldown

            lastFireTime = Time.time;
            if (isRunning)
                animator.SetTrigger("RunAttack");
            else if (isMoving)
                animator.SetTrigger("WalkAttack");
            else
                animator.SetTrigger("Attack");

            PlaySound(attackSound);
        }

        // Chu?t ph?i – Extra (có cooldown)
        if (Input.GetMouseButtonDown(1))
        {
            if (Time.time < lastExtraTime + extraCooldown)
                return;

            lastExtraTime = Time.time;

            animator.SetTrigger("AttackExtra");
            PlaySound(extraAttackSound);
        }
    }

    // ===== FIREBALL =====
    public void ShootNormalFireball() => Shoot(normalFireball);
    public void ShootExtraFireball() => Shoot(extraFireball);

    void Shoot(GameObject prefab)
    {
        if (prefab == null || firePoint == null) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - firePoint.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (mouseWorldPos - firePoint.position).normalized;
        GameObject fb = Instantiate(prefab, firePoint.position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fb.transform.rotation = Quaternion.Euler(0, 0, angle);

        fb.GetComponent<MageFireball>().Init(dir);
    }

    // ===== HEALTH =====
    public void TakeDamage(int damage)
    {
        if (Time.time - lastHurtTime < hurtCooldown) return;
        lastHurtTime = Time.time;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        PlaySound(hurtSound);

        healthUI?.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthUI?.SetHealth(currentHealth);
        PlaySound(healSound);
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        gameOverUI?.ShowGameOver();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
   

    void RotateFirePoint()
    {
        if (!firePoint) return;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - firePoint.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = mouseWorldPos - firePoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }
}

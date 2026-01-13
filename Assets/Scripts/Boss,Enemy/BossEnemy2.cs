using System.Collections;
using UnityEngine;

public class BossEnemy2 : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private float enragedThreshold = 0.3f;
    [SerializeField] private float enragedSpeedMultiplier = 2f;
    [SerializeField] private int enragedDamageBonus = 20;

    [Header("Skill Settings")]
    [SerializeField] private float teleportCooldown = 6f;
    private float lastTeleportTime = -999f;

    [Header("UI Settings")]
    [SerializeField] private BossHealthUI bossHealthUI;

    [Header("Attack Cooldowns")]
    [SerializeField] private float attack1Cooldown = 5f;
    [SerializeField] private float attack2Cooldown = 8f;

    private float lastAttack1Time = -999f;
    private float lastAttack2Time = -999f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject portalPrefab;
    private Transform playerTransform;
    private bool isEnraged = false;
    private bool isFacingRight = true;

    [Header("Audio")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Attack Settings")]
    [SerializeField] private float attackRangeForward = 1.5f;   // kho?ng cách tr??c m?t boss
    [SerializeField] private float attackRadius = 1f;           // bán kính vùng chém
    [SerializeField] private LayerMask playerLayer;             // layer c?a player

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        if (bossHealthUI != null)
        {
            bossHealthUI.SetMaxHealth(maxHealth);
            bossHealthUI.Show();
        }

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateFacingDirection();
        if (!isEnraged && currentHealth <= maxHealth * enragedThreshold)
        {
            EnterEnragedMode();
        }

        TryTeleportAndAttack();
    }

    private void EnterEnragedMode()
    {
        isEnraged = true;
        moveSpeed *= enragedSpeedMultiplier;
        attackDamage += enragedDamageBonus;
        animator.SetTrigger("Enraged");
    }
    private void UpdateFacingDirection()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        if (rb.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
            isFacingRight = true;
        }
        else if (rb.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
            isFacingRight = false;
        }
    }


    private void TryTeleportAndAttack()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // ===== TELEPORT (FIX D?T ?I?M) =====
        if (distance > attackStopRange &&          // ch?a ??ng sát ?? ?ánh
            distance <= chaseRange &&               // trong t?m ?u?i
            Time.time - lastTeleportTime >= teleportCooldown)
        {
            lastTeleportTime = Time.time;

            Vector2 dirFromPlayer =
                (transform.position - playerTransform.position).normalized;

            Vector3 targetPos =
                playerTransform.position + (Vector3)dirFromPlayer * 1.5f;

            transform.position = targetPos;

            animator.SetTrigger("Teleport");
            if (teleportClip != null)
                audioSource.PlayOneShot(teleportClip);
        }

        // ===== ATTACK =====
        if (distance <= attackRange)
        {
            if (Time.time - lastAttack1Time >= attack1Cooldown ||
                Time.time - lastAttack2Time >= attack2Cooldown)
            {
                if (Random.value > 0.5f && Time.time - lastAttack1Time >= attack1Cooldown)
                {
                    lastAttack1Time = Time.time;
                    animator.SetTrigger("Attack1");
                    if (attackClip != null) audioSource.PlayOneShot(attackClip);
                }
                else if (Time.time - lastAttack2Time >= attack2Cooldown)
                {
                    lastAttack2Time = Time.time;
                    animator.SetTrigger("Attack2");
                    if (attackClip != null) audioSource.PlayOneShot(attackClip);
                }
            }
        }
    }


    // G?i t? Animation Event ?? gây damage ?úng lúc chém
    public void DealDamage()
    {
        if (playerTransform == null) return;

        // Luôn tính h??ng t? Boss t?i Player
        Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;

        // V? trí vùng chém n?m c? ??nh phía tr??c m?t Boss
        Vector2 attackPos = (Vector2)transform.position + dirToPlayer * attackRangeForward;

        // Ki?m tra player trong vùng chém
        Collider2D hit = Physics2D.OverlapCircle(attackPos, attackRadius, playerLayer);
        if (hit != null)
        {
            PlayerMage player = hit.GetComponent<PlayerMage>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (bossHealthUI != null)
            bossHealthUI.SetHealth(currentHealth);
    }

    protected override void Die()
    {
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        base.Die();

        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
        }
    }

    // V? vùng chém ?? debug trong Scene view
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        // Luôn v? vùng chém theo h??ng Boss ? Player
        Vector2 dirToPlayer = (playerTransform.position - transform.position).normalized;
        Vector2 attackPos = (Vector2)transform.position + dirToPlayer * attackRangeForward;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRadius);

        // V? thêm m?i tên h??ng nhìn
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)dirToPlayer * 2f);
    }

}

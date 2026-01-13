using System.Collections.Generic;
using UnityEngine;

public class BossEnemy3 : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private float enragedThreshold = 0.3f;
    [SerializeField] private float enragedSpeedMultiplier = 2f;
    [SerializeField] private int enragedDamageBonus = 20;

    [Header("Teleport")]
    [SerializeField] private float teleportCooldown = 6f;
    private float lastTeleportTime = -999f;

    [Header("Boss UI")]
    [SerializeField] private BossHealthUI bossHealthUI;

    [Header("Attack Cooldowns")]
    [SerializeField] private float attack1Cooldown = 4f;
    [SerializeField] private float attack2Cooldown = 6f;
    [SerializeField] private float attack3Cooldown = 9f;

    private float lastAttack1Time = -999f;
    private float lastAttack2Time = -999f;
    private float lastAttack3Time = -999f;

    [Header("Attack Area")]
    [SerializeField] private float attackRangeForward = 1.5f;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Drop")]
    [SerializeField] private GameObject portalPrefab;

    private bool isEnraged = false;
    private int currentAttackIndex = 0;

    // ================= START =================
    protected override void Start()
    {
        base.Start();

        if (bossHealthUI != null)
        {
            bossHealthUI.SetMaxHealth(maxHealth);
            bossHealthUI.SetHealth(currentHealth);
            bossHealthUI.Show();
        }
    }

    // ================= UPDATE =================
    protected override void Update()
    {
        base.Update(); // ?? C?C K? QUAN TR?NG

        CheckEnrage();
    }

    // ================= ENRAGE =================
    private void CheckEnrage()
    {
        if (isEnraged) return;

        if (currentHealth <= maxHealth * enragedThreshold)
        {
            isEnraged = true;
            moveSpeed *= enragedSpeedMultiplier;
            attackDamage += enragedDamageBonus;
            animator.SetTrigger("Enraged");
        }
    }

    // ================= ATTACK (OVERRIDE ?ÚNG) =================
    protected override void TryAttack(float distance)
    {
        if (distance > attackRange) return;

        // ===== TELEPORT =====
        if (distance > attackStopRange &&
            Time.time - lastTeleportTime >= teleportCooldown)
        {
            lastTeleportTime = Time.time;

            Vector2 dir = (transform.position - playerTransform.position).normalized;
            transform.position = playerTransform.position + (Vector3)dir * 1.5f;

            animator.SetTrigger("Teleport");
            if (teleportClip != null)
                audioSource.PlayOneShot(teleportClip);

            return;
        }

        // ===== CH?N ATTACK CÒN COOLDOWN =====
        List<int> ready = new List<int>();

        if (Time.time - lastAttack1Time >= attack1Cooldown) ready.Add(1);
        if (Time.time - lastAttack2Time >= attack2Cooldown) ready.Add(2);
        if (Time.time - lastAttack3Time >= attack3Cooldown) ready.Add(3);

        if (ready.Count == 0) return;

        currentAttackIndex = ready[Random.Range(0, ready.Count)];

        switch (currentAttackIndex)
        {
            case 1:
                lastAttack1Time = Time.time;
                animator.SetTrigger("attack1");
                break;

            case 2:
                lastAttack2Time = Time.time;
                animator.SetTrigger("attack2");
                break;

            case 3:
                lastAttack3Time = Time.time;
                animator.SetTrigger("attack3");
                break;
        }

        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);
    }

    // ================= DAMAGE (ANIMATION EVENT) =================
    public void DealDamage()
    {
        if (playerTransform == null) return;

        float dir = Mathf.Sign(playerTransform.position.x - transform.position.x);
        Vector2 attackPos = (Vector2)transform.position + Vector2.right * dir * attackRangeForward;

        Collider2D hit = Physics2D.OverlapCircle(attackPos, attackRadius, playerLayer);
        if (hit != null)
        {
            hit.GetComponent<Player>()?.TakeDamage(attackDamage);
            hit.GetComponent<PlayerMage>()?.TakeDamage(attackDamage);
        }
    }

    // ================= TAKE DAMAGE =================
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (bossHealthUI != null)
            bossHealthUI.SetHealth(currentHealth);
    }

    // ================= DIE =================
    protected override void Die()
    {
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        base.Die();

        if (portalPrefab != null)
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
    }

    // ================= GIZMO =================
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;

        float dir = Mathf.Sign(playerTransform.position.x - transform.position.x);
        Vector2 pos = (Vector2)transform.position + Vector2.right * dir * attackRangeForward;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRadius);
    }
}

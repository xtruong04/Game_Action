using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private float enragedThreshold = 0.3f;
    [SerializeField] private float enragedSpeedMultiplier = 2f;
    [SerializeField] private int enragedDamageBonus = 20;

    [Header("Skill Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform skillSpawnPoint;
    [SerializeField] private float skillCooldown = 5f;
    private float lastSkillTime = -999f;

    [Header("UI Settings")]
    [SerializeField] private BossHealthUI bossHealthUI;

    [Header("Attack Cooldowns")]
    [SerializeField] private float attack1Cooldown = 5f;
    [SerializeField] private float attack2Cooldown = 8f;
    [SerializeField] private float attack3Cooldown = 3f;

    [SerializeField] private GameObject portalPrefab;
    private Transform playerTransform;

    private float lastAttack1Time = -999f;
    private float lastAttack2Time = -999f;
    private float lastAttack3Time = -999f;

    private bool isEnraged = false;

    // ===== AUDIO (CHỈ THÊM PHẦN NÀY) =====
    [Header("Audio")]
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip fireballClip;
    [SerializeField] private AudioClip deathClip;

    protected override void Start()
    {
        base.Start();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        if (bossHealthUI != null)
        {
            bossHealthUI.SetMaxHealth(maxHealth);
            bossHealthUI.Show();
        }
    }

    public void StartAttack2()
    {
        StartCoroutine(FireballBurst());
    }

    protected override void Update()
    {
        base.Update();

        if (!isEnraged && currentHealth <= maxHealth * enragedThreshold)
        {
            EnterEnragedMode();
        }

        TryCastSkill();
    }

    private void EnterEnragedMode()
    {
        isEnraged = true;
        moveSpeed *= enragedSpeedMultiplier;
        attackDamage += enragedDamageBonus;
        animator.SetTrigger("Enraged");
    }

    protected override void TryAttack(float distance)
    {
        if (distance <= attackRange)
        {
            if (Time.time - lastAttack1Time >= attack1Cooldown)
            {
                lastAttack1Time = Time.time;
                animator.SetTrigger("Attack1");
                if (attackClip != null)
                    audioSource.PlayOneShot(attackClip);
            }
            else if (Time.time - lastAttack2Time >= attack2Cooldown)
            {
                lastAttack2Time = Time.time;
                animator.SetTrigger("Attack2");
                if (attackClip != null)
                    audioSource.PlayOneShot(attackClip);
            }
            else if (Time.time - lastAttack3Time >= attack3Cooldown)
            {
                lastAttack3Time = Time.time;
                animator.SetTrigger("Attack3");
                if (attackClip != null)
                    audioSource.PlayOneShot(attackClip);
            }
        }
    }

    private void TryCastSkill()
    {
        if (Time.time - lastSkillTime >= skillCooldown)
        {
            lastSkillTime = Time.time;
            animator.SetTrigger("SkillCast");

            if (fireballClip != null)
                audioSource.PlayOneShot(fireballClip);
        }
    }

    // ===== Animation Event =====
    public void CastFireball()
    {
        if (fireballPrefab != null && skillSpawnPoint != null && playerTransform != null)
        {
            int numberOfFireballs = 5;
            float spreadAngle = 30f;

            Vector2 directionToPlayer =
                (playerTransform.transform.position - skillSpawnPoint.position).normalized;

            for (int i = 0; i < numberOfFireballs; i++)
            {
                float angle = -spreadAngle / 2 +
                              (spreadAngle / (numberOfFireballs - 1)) * i;

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                GameObject fb =
                    Instantiate(fireballPrefab, skillSpawnPoint.position, Quaternion.identity);

                Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 shootDir = rotation * directionToPlayer;
                    rb.linearVelocity = shootDir * 10f;

                    float angleRad =
                        Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
                    fb.transform.rotation =
                        Quaternion.AngleAxis(angleRad, Vector3.forward);
                }
            }
        }
    }

    private IEnumerator FireballBurst()
    {
        int shots = 10;
        float delay = 0.2f;

        for (int i = 0; i < shots; i++)
        {
            ShootFireballAtPlayer();
            yield return new WaitForSeconds(delay);
        }
    }

    public void CastSingleFireball()
    {
        if (fireballPrefab != null && skillSpawnPoint != null && playerTransform != null)
        {
            Vector2 dir =
                (playerTransform.transform.position - skillSpawnPoint.position).normalized;

            GameObject fb =
                Instantiate(fireballPrefab, skillSpawnPoint.position, Quaternion.identity);

            Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * 10f;

                float angle =
                    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                fb.transform.rotation =
                    Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void ShootFireballAtPlayer()
    {
        if (fireballPrefab != null && skillSpawnPoint != null && playerTransform != null)
        {
            Vector2 dir =
                (playerTransform.transform.position - skillSpawnPoint.position).normalized;

            GameObject fb =
                Instantiate(fireballPrefab, skillSpawnPoint.position, Quaternion.identity);

            Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * 10f;

                float angle =
                    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                fb.transform.rotation =
                    Quaternion.AngleAxis(angle, Vector3.forward);
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
}

using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] private float enragedThreshold = 0.3f; // Khi m�u < 30% th� n?i ?i�n
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
    private float lastAttack1Time = -999f;
    private float lastAttack2Time = -999f;
    private float lastAttack3Time = -999f;

    private bool isEnraged = false;

    protected override void Start()
    {
        base.Start();

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

        // Debug ?? ki?m tra boss c� nh?n player kh�ng
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log("Boss state: " + currentState + " | Distance: " + distance);
        }

        // Khi m�u th?p th� v�o tr?ng th�i Enraged
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
        Debug.Log("Boss ?� n?i ?i�n!");
    }

    protected override void TryAttack(float distance)
    {
        if (distance <= attackRange)
        {
            // Attack1
            if (Time.time - lastAttack1Time >= attack1Cooldown)
            {
                lastAttack1Time = Time.time;
                animator.SetTrigger("Attack1");
            }
            // Attack2
            else if (Time.time - lastAttack2Time >= attack2Cooldown)
            {
                lastAttack2Time = Time.time;
                animator.SetTrigger("Attack2");
            }
            // Attack3
            else if (Time.time - lastAttack3Time >= attack3Cooldown)
            {
                lastAttack3Time = Time.time;
                animator.SetTrigger("Attack3");
            }
        }
    }
    private void TryCastSkill()
    {
        if (Time.time - lastSkillTime >= skillCooldown)
        {
            lastSkillTime = Time.time;
            animator.SetTrigger("SkillCast");
        }

    }
    // G?i t? Animation Event trong clip SkillCast
    public void CastFireball()
    {
        if (fireballPrefab != null && skillSpawnPoint != null && player != null)
        {
            int numberOfFireballs = 5;
            float spreadAngle = 30f;

            // Vector từ boss tới player
            Vector2 directionToPlayer = (player.transform.position - skillSpawnPoint.position).normalized;

            for (int i = 0; i < numberOfFireballs; i++)
            {
                float angle = -spreadAngle / 2 + (spreadAngle / (numberOfFireballs - 1)) * i;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                GameObject fb = Instantiate(fireballPrefab, skillSpawnPoint.position, Quaternion.identity);

                Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Hướng bay
                    Vector2 shootDir = rotation * directionToPlayer;
                    rb.linearVelocity = shootDir * 10f;

                    // Xoay sprite/animation theo hướng bay
                    float angleRad = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
                    fb.transform.rotation = Quaternion.AngleAxis(angleRad, Vector3.forward);
                }
            }
        }
    }
    
    private IEnumerator FireballBurst()
    {
        int shots = 10; // số lần bắn
        float delay = 0.2f; // thời gian giữa mỗi lần

        for (int i = 0; i < shots; i++)
        {
            ShootFireballAtPlayer();
            yield return new WaitForSeconds(delay);
        }
    }
    public void CastSingleFireball()
    {
        Debug.Log("CastSingleFireball called"); // kiểm tra event

        if (fireballPrefab != null && skillSpawnPoint != null && player != null)
        {
            Vector2 dir = (player.transform.position - skillSpawnPoint.position).normalized;

            GameObject fb = Instantiate(fireballPrefab, skillSpawnPoint.position, Quaternion.identity);

            Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * 10f;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                fb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }


    private void ShootFireballAtPlayer()
    {
        if (fireballPrefab != null && skillSpawnPoint != null && player != null)
        {
            Vector2 dir = (player.transform.position - skillSpawnPoint.position).normalized;
            GameObject fb = Instantiate(fireballPrefab, skillSpawnPoint.position, Quaternion.identity);

            Rigidbody2D rb = fb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * 10f;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                fb.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
        base.Die();

        if (portalPrefab != null) 
        { 
            Instantiate(portalPrefab, transform.position, Quaternion.identity); 
        }
        // Th�m hi?u ?ng ??c bi?t khi boss ch?t: m? c?ng, drop item hi?m...
    }
}

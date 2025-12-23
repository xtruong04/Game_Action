using UnityEngine;

public class OrcEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        moveSpeed = 2f;       // tốc độ di chuyển
        maxHealth = 30;       // máu tối đa
        attackDamage = 15;    // sát thương mỗi đòn
        attackCooldown = 2f;  // mỗi 2 giây tấn công một lần

        // Khởi tạo máu hiện tại
        currentHealth = maxHealth;
    }
}

using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.Heal(healAmount);
            }
            Destroy(gameObject);
        }
    }
}

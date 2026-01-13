using UnityEngine;

public class DamageBuffItem : MonoBehaviour
{
    public float buffDuration = 5f;
    public int extraDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.StartCoroutine(player.DamageBuff(extraDamage, buffDuration));
            }
            Destroy(gameObject);
        }
    }
}

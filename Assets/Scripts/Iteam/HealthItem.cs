using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IHealable healable = collision.GetComponent<IHealable>();
            if (healable != null)
            {
                healable.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}

using UnityEngine;

public class MageFireball : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Vector2 direction;

    // Kh?i t?o h??ng bay
    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

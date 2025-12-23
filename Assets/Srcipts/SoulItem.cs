using UnityEngine;

public class SoulItem : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, player.position) < 0.3f)
        {
            Player p = player.GetComponent<Player>();
            if (p != null)
            {
                p.AddSoul(1);
            }
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int enemyCount = 0; // s? quái nh? còn l?i
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    private bool bossSpawned = false;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy()
    {
        enemyCount++;
        Debug.Log("Enemy registered. Total: " + enemyCount);
    }

    public void OnEnemyKilled()
    {
        enemyCount--;

        if (enemyCount <= 0 && !bossSpawned)
        {
            bossSpawned = true;
            SpawnBoss();
        }
    }



    private void SpawnBoss()
    {
        bossSpawned = true;
        Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        Debug.Log("Boss spawned!");
    }
}

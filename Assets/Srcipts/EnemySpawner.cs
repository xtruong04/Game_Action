using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int minEnemiesPerGroup = 4;
    [SerializeField] private int maxEnemiesPerGroup = 5;
    [SerializeField] private int numberOfGroups = 5;
    [SerializeField] private Tilemap groundTilemap; // tham chiếu tới Tilemap của map

    void Start()
    {
        SpawnGroups();
    }

    void SpawnGroups()
    {
        BoundsInt bounds = groundTilemap.cellBounds;

        for (int i = 0; i < numberOfGroups; i++)
        {
            // chọn vị trí trung tâm ngẫu nhiên trong tilemap
            Vector3Int randomCell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

            Vector3 groupCenter = groundTilemap.CellToWorld(randomCell);

            int enemyCount = Random.Range(minEnemiesPerGroup, maxEnemiesPerGroup + 1);

            for (int j = 0; j < enemyCount; j++)
            {
                Vector2 offset = Random.insideUnitCircle * 2f;
                Vector3 spawnPos = groupCenter + (Vector3)offset;

                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}

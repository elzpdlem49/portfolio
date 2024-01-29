using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public PoolManager poolManager;
    public Transform[] spawnPoints;
    public float spawnInterval = 5.0f;
    public int spawnCount = 0;
    public int maxSpawnCount = 5;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (spawnCount < maxSpawnCount)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemy = poolManager.GetEnemyFromPool();

            enemy.transform.position = spawnPoint.position;
            enemy.SetActive(true);
            yield return new WaitForSeconds(spawnInterval);
            spawnCount++;
        }
    }
}

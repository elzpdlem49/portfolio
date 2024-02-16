using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public PoolManager poolManager;
    public float spawnInterval = 5.0f;
    public int spawnCount = 0;
    public int maxSpawnCount = 5;

    public Transform[] redSpawnPoints;
    public Transform[] blueSpawnPoints;
    void Start()
    {
        StartCoroutine(SpawnMinionWaves());
    }

    IEnumerator SpawnMinionWaves()
    {
        while (spawnCount < maxSpawnCount)
        {
            // Spawn Red Minion
            Transform redSpawnPoint = redSpawnPoints[Random.Range(0, redSpawnPoints.Length)];
            GameObject redMinion = poolManager.GetEnemyFromPool("Red");
            redMinion.transform.position = redSpawnPoint.position;
            redMinion.SetActive(true);

            // Spawn Blue Minion
            Transform blueSpawnPoint = blueSpawnPoints[Random.Range(0, blueSpawnPoints.Length)];
            GameObject blueMinion = poolManager.GetEnemyFromPool("Blue");
            blueMinion.transform.position = blueSpawnPoint.position;
            blueMinion.SetActive(true);

            yield return new WaitForSeconds(spawnInterval);
            spawnCount++;
        }
    }
}
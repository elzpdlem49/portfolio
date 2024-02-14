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
        StartCoroutine(SpawnMinionWaves());
    }

    IEnumerator SpawnMinionWaves()
    {
        while (spawnCount < maxSpawnCount)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject redMinion = poolManager.GetEnemyFromPool("Red"); // Pass minion type
            redMinion.transform.position = spawnPoint.position;
            redMinion.SetActive(true);

            GameObject blueMinion = poolManager.GetEnemyFromPool("Blue"); // Pass minion type
            blueMinion.transform.position = spawnPoint.position;
            blueMinion.SetActive(true);

            yield return new WaitForSeconds(spawnInterval);
            spawnCount++;
        }
    }
}
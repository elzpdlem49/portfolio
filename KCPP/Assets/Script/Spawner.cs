using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public PoolManager poolManager;
    public Transform[] spawnPoints;
    public float spawnInterval = 5.0f;

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Choose a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Get an inactive enemy from the pool manager
            GameObject enemy = poolManager.GetEnemyFromPool();

            // Set the enemy's position and activate it
            enemy.transform.position = spawnPoint.position;
            enemy.SetActive(true);

            // Wait for the specified interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

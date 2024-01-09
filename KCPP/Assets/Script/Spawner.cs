using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int poolSize = 5;
    public float spawnInterval = 5.0f;

    public List<GameObject> enemyPool = new List<GameObject>();

    void Start()
    {
        InitializePool();
        StartCoroutine(SpawnEnemies());
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < poolSize; i++)
        {
            // Choose a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Get an inactive enemy from the pool
            GameObject enemy = GetInactiveEnemy();

            if (enemy != null)
            {
                // Set the enemy's position and activate it
                enemy.transform.position = spawnPoint.position;
                enemy.SetActive(true);
            }

            // Wait for the specified interval before spawning the next enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    GameObject GetInactiveEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeSelf)
            {
                return enemy;
            }
        }

        // If no inactive enemy is found, expand the pool (Instantiate a new one)
        GameObject newEnemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        newEnemy.SetActive(false);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }
}
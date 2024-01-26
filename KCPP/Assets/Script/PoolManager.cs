using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int initialPoolSize = 5;

    private List<GameObject> enemyPool = new List<GameObject>();

    void Awake()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject enemy = GetEnemyFromPool();
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    public GameObject GetEnemyFromPool()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        GameObject newEnemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
        newEnemy.SetActive(true);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    // 추가로 필요한 기능들을 구현할 수 있습니다.
}

using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

public class PoolManager : MonoBehaviour
{
    static public PoolManager instance;

    public GameObject enemyPrefab;
    public GameObject enemyTarget;
    public GameObject hpBarPrefab;
    public int initialPoolSize = 5;

    public List<GameObject> enemyPool = new List<GameObject>();


    void Awake()
    {
        InitializePool();
        instance = this;
    }
    
    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("CanvasWorld").transform);
            newEnemy.SetActive(false);
            

            Enemycontroller newEnemyController = newEnemy.GetComponent<Enemycontroller>();
            if (newEnemyController != null)
            {
                newEnemyController.m_objTarget = enemyTarget;
                
            }

            enemyPool.Add(newEnemy);

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

        Enemycontroller newEnemyController = newEnemy.GetComponent<Enemycontroller>();
        if (newEnemyController != null)
        {
            newEnemyController.m_objTarget = enemyTarget;
        }

        enemyPool.Add(newEnemy);
        return newEnemy;
    }
    public void RemoveFromPool(GameObject enemy)
    {
        if (enemyPool.Contains(enemy))
        {
            enemyPool.Remove(enemy);
        }
    }
}



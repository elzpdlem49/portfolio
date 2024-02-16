using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static public PoolManager instance;

    public GameObject redEnemyPrefab;
    public GameObject blueEnemyPrefab;
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
            GameObject redEnemy = Instantiate(redEnemyPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("CanvasWorld").transform);
            redEnemy.SetActive(false);
            
            GameObject blueEnemy = Instantiate(blueEnemyPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("CanvasWorld").transform);
            blueEnemy.SetActive(false);

            Enemycontroller newRedEnemyController = redEnemy.GetComponent<Enemycontroller>();
            if (newRedEnemyController != null)
            {
                newRedEnemyController.gameObject.tag = "Red";
            }

            Enemycontroller newBlueEnemyController = blueEnemy.GetComponent<Enemycontroller>();
            if (newBlueEnemyController != null)
            {
                newBlueEnemyController.gameObject.tag = "Blue";
            }

            enemyPool.Add(redEnemy);
            enemyPool.Add(blueEnemy);
        }
    }

    public GameObject GetEnemyFromPool(string minionType)
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        GameObject minionPrefab = (minionType == "Red") ? redEnemyPrefab : blueEnemyPrefab;
        GameObject newMinion = Instantiate(minionPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("CanvasWorld").transform);
        newMinion.SetActive(false);

        // Remove the Faction setting and use tag instead
        newMinion.tag = (minionType == "Red") ? "Red" : "Blue";

        enemyPool.Add(newMinion);
        return newMinion;
    }
    public void RemoveFromPool(GameObject enemy)
    {
        if (enemyPool.Contains(enemy))
        {
            enemyPool.Remove(enemy);
        }
    }
}



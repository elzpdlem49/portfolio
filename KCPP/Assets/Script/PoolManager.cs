using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    static public PoolManager instance;

    public GameObject redEnemyPrefab;
    public GameObject blueEnemyPrefab;
    public GameObject[] redEnemyTargets;
    public GameObject[] blueEnemyTargets;
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
                newRedEnemyController.m_objTarget = GetRandomTarget(redEnemyTargets);
                newRedEnemyController.m_eFaction = Enemycontroller.Faction.Red;
            }

            Enemycontroller newBlueEnemyController = blueEnemy.GetComponent<Enemycontroller>();
            if (newBlueEnemyController != null)
            {
                newBlueEnemyController.m_objTarget = GetRandomTarget(blueEnemyTargets);
                newBlueEnemyController.m_eFaction = Enemycontroller.Faction.Blue;
            }

            enemyPool.Add(redEnemy);
            enemyPool.Add(blueEnemy);
        }
    }

    GameObject GetRandomTarget(GameObject[] targets)
    {
        int randomIndex = Random.Range(0, targets.Length);
        return targets[randomIndex];
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

        Enemycontroller newEnemyController = newMinion.GetComponent<Enemycontroller>();
        if (newEnemyController != null)
        {
            // Set other properties based on minion type if needed
            newEnemyController.m_objTarget = (minionType == "Red") ? redEnemyTarget : blueEnemyTarget;
        }

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



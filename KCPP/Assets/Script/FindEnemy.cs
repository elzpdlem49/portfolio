using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEnemy : MonoBehaviour
{
    public static FindEnemy Instance;
    public bool isCameraFixed = false;
    public Follow cameraFollow;

    private GameObject currentEnemy;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraFollow = FindObjectOfType<Follow>();
    }

    // Update is called once per frame
    void Update()
    {
       /* if (Input.GetMouseButtonDown(2))
        {
            ToggleCam();
        }*/
        //if (isCameraFixed)
        {
            FindAndLookAtEnemy();
        }
    }
    void FindAndLookAtEnemy()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int bossLayer = LayerMask.NameToLayer("Boss");
        int annieLayer = LayerMask.NameToLayer("Annie");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        GameObject[] annies = GameObject.FindGameObjectsWithTag("Annie");

        if (enemies.Length > 0 || bosses.Length > 0 || annies.Length > 0)
        {
            GameObject nearestEnemy = GetNearestEnemy(enemies, bosses, enemyLayer, bossLayer);

            if (nearestEnemy != null)
            {
                Vector3 targetPosition = nearestEnemy.transform.position + Vector3.up * 1.5f;
                targetPosition.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(targetPosition, Vector3.up * 1.5f);
                transform.rotation = toRotation;
                Camera.main.transform.rotation = Quaternion.Euler(0f, toRotation.eulerAngles.y, 0f);
                transform.LookAt(targetPosition);
            }
        }
    }

    GameObject GetNearestEnemy(GameObject[] enemies, GameObject[] bosses, int enemyLayer, int bossLayer)
    {
        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            if (enemy.layer == enemyLayer)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        foreach (GameObject boss in bosses)
        {
            if (boss.layer == bossLayer)
            {
                float distance = Vector3.Distance(transform.position, boss.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = boss;
                }
            }
        }

        return nearestEnemy;
    }

    /*void ToggleCam()
    {
        isCameraFixed = !isCameraFixed;
        if (isCameraFixed)
        {
            cameraFollow.DisableMouseRotation();
            FindAndLookAtEnemy();
        }
        else
        {
            cameraFollow.EnableMouseRotation();
        }
    }*/
}

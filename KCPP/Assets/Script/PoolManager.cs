using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;

    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject selectedObject = FindInactiveObject(index);

        if (selectedObject == null)
        {
            selectedObject = InstantiateAndAddToPool(index);
        }

        return selectedObject;
    }

    private GameObject FindInactiveObject(int index)
    {
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        return null;
    }

    private GameObject InstantiateAndAddToPool(int index)
    {
        GameObject newInstance = Instantiate(prefabs[index], transform);
        pools[index].Add(newInstance);
        return newInstance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject playerInstance;
    public Vector3 spawnPosition = new Vector3(0f, 0f, 0f);

    void Start()
    {
        SpawnPlayer(spawnPosition);
    }

    void SpawnPlayer(Vector3 position)
    {
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }

        playerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        // Additional initialization logic (e.g., setting health, etc.)
    }

    void RespawnPlayer()
    {
        // You can customize the spawn position when respawning
        Vector3 customSpawnPosition = new Vector3(10f, 0f, 5f); // Set your desired spawn position
        SpawnPlayer(customSpawnPosition);
    }
}

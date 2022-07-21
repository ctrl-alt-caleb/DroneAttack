using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public float spawnDelay = 0.3f;

    float nextTimeToSpawn = 0f;

    public GameObject drone;

    public GameObject clone;

    public Transform[] spawnPoints;

    private void Update()
    {
        {
            if (nextTimeToSpawn <= Time.time)
            {
                SpawnDrone();
                nextTimeToSpawn = Time.time + spawnDelay;

                DestroyGameObject();
            }
        }
    }

    void SpawnDrone()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        clone = Instantiate(drone, spawnPoint.position, spawnPoint.rotation);
    }

    public void DestroyGameObject()
    {
        //Debug.Log("clone destroyed!!");
        Destroy(clone, 3);
    }


}


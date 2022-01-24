using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnTime
{
    OnStart,
    OnEvent,
}

public class SpawnList : MonoBehaviour
{
    public GameObject[] toSpawn;
    public SpawnTime spawnTime;

    void Start()
    {
        if(spawnTime == SpawnTime.OnStart)
        {
            DoSpawn();
        }
    }

    public void DoSpawn()
    {
        foreach(GameObject element in toSpawn)
        {
            Instantiate(element, transform);
        }
    }
}

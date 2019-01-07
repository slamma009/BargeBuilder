using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extractor : PlacableObject
{
    public Transform OreSpawn;
    public float SpawnRatePerMinute = 20;
    public GameObject SpawnObject;

    private float TimeSinceLastSpawn;
    private float SecondsToSpawn;

    public bool Placed = false;

    public void Start()
    {
        SecondsToSpawn = 60 / SpawnRatePerMinute;
    }
    public void FixedUpdate()
    {
        if (Placed)
        {
            TimeSinceLastSpawn += Time.deltaTime;
            if (TimeSinceLastSpawn > SecondsToSpawn)
            {
                TimeSinceLastSpawn = 0;
                Instantiate(SpawnObject, OreSpawn.position, Quaternion.identity);
            }
        }
    }

}

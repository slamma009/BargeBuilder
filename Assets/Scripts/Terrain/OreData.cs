

using System;
using UnityEngine;

public class OreData : MonoBehaviour
{
    public float PerlinScale = 1;
    public int Octaves = 4;
    public float Persistance = 0.5f;
    public float Lacunarity = 2;
    public float Chance = 0.8f;
    public int Seed;

    public OreData (ScriptableOre ore, int seed)
    {
        PerlinScale = ore.PerlinScale;
        Octaves = ore.Octavies;
        Persistance = ore.Persistance;
        Lacunarity = ore.Lacunarity;
        Chance = ore.Chance;
        Seed = seed;
    }
}


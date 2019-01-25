using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOre", menuName = "Ore Generation")]
public class ScriptableOre : ScriptableObject
{
    public float PerlinScale = 1;
    public int Octavies = 4;
    public float Persistance = 0.5f;
    public float Lacunarity = 2;
    public float Chance = 0.8f;
    
}

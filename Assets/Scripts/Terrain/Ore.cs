using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOre", menuName = "Ore Generation")]
public class Ore : ScriptableObject
{
    public float PerlinScale = 1;
    public float Chance = 0.8f;

    [HideInInspector]
    public float Seed;

    public float GetOreChance(int x, int z, Vector2Int Offset)
    {
        return Mathf.PerlinNoise((Seed + Offset.x + x) /  PerlinScale, (Seed + Offset.y + z) / PerlinScale);
    }
}

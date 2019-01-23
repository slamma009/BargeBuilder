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

    public float GetOreChance(int x, int z, Vector2Int Offset, Vector2Int GridSize)
    {
        return Mathf.PerlinNoise(((Seed + Offset.x + x) / (float)(GridSize.x + 1)) * PerlinScale, ((Seed + Offset.y + z) / (float)(GridSize.y + 1)) * PerlinScale);
    }
}

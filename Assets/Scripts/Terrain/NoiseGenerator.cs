using System;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] Generate(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve animationCurve, float heightScale, int gridSize, Vector2 offset, int seed)
    {
        AnimationCurve meshHeightCurve = null;
        
        if(animationCurve != null)
            meshHeightCurve = new AnimationCurve(animationCurve.keys);
        float[,] heightGrid = new float[gridSize + 1, gridSize + 1];

        float maxPossibleHeight = 0;
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        for (var i = 0; i < octaves; ++i)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = (gridSize + 1) / 2f;
        float halfHeight = (gridSize + 1) / 2f;


        for (int z = 0; z <= gridSize; ++z)
        {
            for (var x = 0; x <= gridSize; ++x)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < octaves; ++i)
                {
                    float sampleX = (x + octaveOffsets[i].x) / perlinScale * frequency;
                    float sampleY = (z + octaveOffsets[i].y) / perlinScale * frequency;

                    float height = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += height * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                heightGrid[x, z] = noiseHeight;
            }
        }


        for (int z = 0; z <= gridSize; ++z)
        {
            for (var x = 0; x <= gridSize; ++x)
            {
                float normalizedHeight = (heightGrid[x, z] + 1) / (maxPossibleHeight / 0.9f);
                if (meshHeightCurve != null)
                {
                    heightGrid[x, z] = meshHeightCurve.Evaluate(Mathf.Clamp(normalizedHeight, 0, int.MaxValue)) * heightScale;
                } else
                {
                    heightGrid[x, z] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }

        }
        return heightGrid;
    }

}


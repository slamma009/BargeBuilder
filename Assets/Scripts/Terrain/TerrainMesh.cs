﻿using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{

    public Mesh GenerateMesh(int octaves, float persistance, float lacunarity,  float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, Vector2 offset, int seed)
    {
        float[,] heightGrid = GetHeight(octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, gridSize, offset, seed);
        Mesh mesh = new Mesh();
        List<Vector3> Verticies = new List<Vector3>();
        List<Vector3> Normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int z = 0, i = 0; z < gridSize; ++z)
        {
            for (var x = 0; x < gridSize; ++x)
            {
                Verticies.AddRange(new Vector3[] {
                new Vector3(x, heightGrid[x, z],z),
                new Vector3(x, heightGrid[x, z + 1],(z+1)),
                new Vector3((x+1), heightGrid[x + 1, z],z)
            });
                Verticies.AddRange(new Vector3[] {
                new Vector3(x, heightGrid[x, z + 1],(z+1)),
                new Vector3((x+1), heightGrid[x + 1, z],z),
                new Vector3((x+1), heightGrid[x + 1, z + 1], (z + 1))
            });

                triangles.AddRange(new int[]
                {
                i*6,i*6+1,i*6+2,
                i*6+3,i*6+5,i*6+4,
                });
                Vector3 Normal1 = Vector3.Cross(Verticies[triangles[triangles.Count - 5]] - Verticies[triangles[triangles.Count - 6]],
                    Verticies[triangles[triangles.Count - 4]] - Verticies[triangles[triangles.Count - 6]]);
                Vector3 Normal2 = Vector3.Cross(Verticies[triangles[triangles.Count - 2]] - Verticies[triangles[triangles.Count - 3]],
                    Verticies[triangles[triangles.Count - 1]] - Verticies[triangles[triangles.Count - 3]]);
                //Vector3 newNormal = (Normal1 + Normal2) * 0.5f;
                Normals.Add(Normal1);
                Normals.Add(Normal1);
                Normals.Add(Normal1);
                Normals.Add(Normal2);
                Normals.Add(Normal2);
                Normals.Add(Normal2);
                i++;

            }
        }
        mesh.vertices = Verticies.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = Normals.ToArray();
        List<Vector2> uvs = new List<Vector2>();
        float max = mesh.vertexCount / 6f;
        int y = 0;
        int xPos = 0;
        for (int i = 0; i < max; ++i)
        {

            float textureOffset = 0;
            //for (var j = 0; j < Ores.Length; ++j)
            //{
            //    if (Ores[j].GetOreChance(xPos, y, Offset, gridSize) > Ores[j].Chance)
            //    {
            //        offset = (j + 1) * 0.25f;
            //        break;
            //    }
            //}
            uvs.Add(new Vector2(textureOffset, 0.75f));
            uvs.Add(new Vector2(textureOffset, 1));
            uvs.Add(new Vector2(textureOffset + 0.25f, 0.75f));
            uvs.Add(new Vector2(textureOffset, 1));
            uvs.Add(new Vector2(textureOffset + 0.25f, 0.75f));
            uvs.Add(new Vector2(textureOffset + 0.25f, 1));

            xPos++;
            if (xPos >= gridSize)
            {
                y++;
                xPos = 0;
            }
        }

        mesh.uv = uvs.ToArray();



        return mesh;
    }

    public float[,] GetHeight(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, Vector2 offset, int seed)
    {
        float[,] heightGrid = new float[gridSize + 1, gridSize + 1];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        float octaveFreq = 1;
        for(var i=0; i<octaves; ++i)
        {
            float offsetX = offset.x;//prng.Next(-100000, 100000) + offset.x;
            float offsetY = offset.y;// prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            octaveFreq *= lacunarity;
        }
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = (gridSize + 1) / 2f;
        float halfHeight = (gridSize + 1) / 2f;
        for (int z = 0; z <= gridSize; ++z)
        {
            for (var x = 0; x <= gridSize; ++x)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < octaves; ++i)
                {
                    float sampleX = (x) / perlinScale * frequency + octaveOffsets[i].x;
                    float sampleY = (z) / perlinScale * frequency + octaveOffsets[i].y;

                    float height = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += height * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if(noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                heightGrid[x, z] = noiseHeight;
            }
        }


        for (int z = 0; z <= gridSize; ++z)
        {
            for (var x = 0; x <= gridSize; ++x)
            {
                //heightGrid[x, z] = heightGrid[x, z] * heightScale;
                heightGrid[x, z] = meshHeightCurve.Evaluate( Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightGrid[x, z])) * heightScale;
            }

        }
        return heightGrid;
    }
}

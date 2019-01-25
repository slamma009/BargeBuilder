using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    Queue<MapThreadInfo<NoiseData>> ThreadHeightQueue = new Queue<MapThreadInfo<NoiseData>>();
    Queue<MapThreadInfo<MeshData>> ThreadMeshQueue = new Queue<MapThreadInfo<MeshData>>();

    #region Threading
    public void RequestHeight(Action<NoiseData> callback, int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, Vector2 offset, int seed, int levelOfDetail)
    {
        ThreadStart threadStart = delegate
        {
            GetHeight(callback, octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, gridSize, offset, seed, levelOfDetail);
        };

        new Thread(threadStart).Start();
    }
    void GetHeight(Action<NoiseData> callback, int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, Vector2 offset, int seed, int levelOfDetail)
    {

        NoiseData mapData = new NoiseData(GetHeight(octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, gridSize, offset, seed), levelOfDetail);

        lock (ThreadHeightQueue)
        {
            ThreadHeightQueue.Enqueue(new MapThreadInfo<NoiseData>(callback, mapData));
        }
    }
    
    public void RequestMeshData(Action<MeshData> callback, int gridSize, float[,] noise, int levelOfDetail)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(callback, gridSize,  noise, levelOfDetail);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(Action<MeshData> callback, int gridSize, float[,] noise, int levelOfDetail)
    {
        MeshData meshData = GenerateMesh(gridSize, noise, levelOfDetail);
        lock (ThreadMeshQueue)
        {
            ThreadMeshQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    #endregion

    private void Update()
    {
        if (ThreadHeightQueue.Count > 0)
        {
            for (var i = 0; i < ThreadHeightQueue.Count; i++)
            {
                MapThreadInfo<NoiseData> threadInfo = ThreadHeightQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (ThreadMeshQueue.Count > 0)
        {
            for (var i = 0; i < ThreadMeshQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = ThreadMeshQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MeshData GenerateMesh(int gridSize, float[,] heightGrid, int levelOfDetail)
    {

        List<Vector3> Verticies = new List<Vector3>();
        List<Vector3> Normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        int meshSimplificationIncrement = levelOfDetail;
        for (int z = 0, i = 0; z < gridSize; z+= meshSimplificationIncrement)
        {
            for (var x = 0; x < gridSize; x+= meshSimplificationIncrement)
            {
                Verticies.AddRange(new Vector3[] {
                new Vector3(x, heightGrid[x, z],z),
                new Vector3(x, heightGrid[x, z + meshSimplificationIncrement],(z+meshSimplificationIncrement)),
                new Vector3((x+meshSimplificationIncrement), heightGrid[x + meshSimplificationIncrement, z],z)
            });
                Verticies.AddRange(new Vector3[] {
                new Vector3(x, heightGrid[x, z + meshSimplificationIncrement],(z+meshSimplificationIncrement)),
                new Vector3((x+meshSimplificationIncrement), heightGrid[x + meshSimplificationIncrement, z],z),
                new Vector3((x+meshSimplificationIncrement), heightGrid[x + meshSimplificationIncrement, z + meshSimplificationIncrement], (z + meshSimplificationIncrement))
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
        List<Vector2> uvs = new List<Vector2>();
        float max = Verticies.Count / 6f;
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

        return new MeshData(Verticies.ToArray(), Normals.ToArray(), triangles.ToArray(), uvs.ToArray(), levelOfDetail);
    }

    float[,] GetHeight(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve animationCurve, float heightScale, int gridSize, Vector2 offset, int seed)
    {
        AnimationCurve meshHeightCurve = new AnimationCurve(animationCurve.keys);
        float[,] heightGrid = new float[gridSize + 1, gridSize + 1];

        float maxPossibleHeight = 0;
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        for (var i=0; i<octaves; ++i)
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

                if(noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                else if(noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;

                heightGrid[x, z] = noiseHeight;
            }
        }


        for (int z = 0; z <= gridSize; ++z)
        {
            for (var x = 0; x <= gridSize; ++x)
            {
                float normalizedHeight = (heightGrid[x, z] + 1) / (maxPossibleHeight / 0.9f);
                heightGrid[x, z] = meshHeightCurve.Evaluate(Mathf.Clamp(normalizedHeight, 0, int.MaxValue)) * heightScale;
                //heightGrid[x, z] = heightGrid[x, z] * heightScale;
                //heightGrid[x, z] = meshHeightCurve.Evaluate( Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, heightGrid[x, z])) * heightScale;
            }

        }
        return heightGrid;
    }
 


    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }

}
public struct MeshData
{
    public readonly Vector3[] Verticies;
    public readonly Vector3[] Normals;
    public readonly int[] Triangles;
    public readonly Vector2[] UVs;
    public int LevelOfDetail;

    public MeshData(Vector3[] verticies, Vector3[] normals, int[] triangles, Vector2[] uvs, int levelOfDetail)
    {
        this.Verticies = verticies;
        this.Normals = normals;
        this.Triangles = triangles;
        this.UVs = uvs;
        this.LevelOfDetail = levelOfDetail;
    }

}

public struct NoiseData
{
    public float[,] HeightGrid;
    public int LevelOfDetail;

    public NoiseData(float[,] heightGrid, int levelOfDetail)
    {
        this.HeightGrid = heightGrid;
        this.LevelOfDetail = levelOfDetail;
    }
}

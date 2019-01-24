using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;


    public const int mapChunkSize = 25;
    [Range(0, 3)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float MeshHeightMultiplier;
    public AnimationCurve MeshHeightCurve;

    Queue<MapThreadInfo<float[,]>> MapDataThreadInfoQueue = new Queue<MapThreadInfo<float[,]>>();
    Queue<MapThreadInfo<MeshData>> MeshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public float[,] GenerateMapData()
    {
        return Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);


        // WorldDisplay display = FindObjectOfType<WorldDisplay>();
        //display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), MeshMaterial);
      
    }

    public void RequestMapData(Action<float[,]> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }
    
    void MapDataThread(Action<float[,]> callback)
    {
        float[,] mapData = GenerateMapData();
        lock (MapDataThreadInfoQueue)
        {
            MapDataThreadInfoQueue.Enqueue(new MapThreadInfo<float[,]>(callback, mapData));
        }
    }

    public void RequestMeshData(Action<MeshData> callback, float[,] noise)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(callback, noise);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(Action<MeshData> callback, float[,] noise)
    {
        MeshData MeshData = MeshGenerator.GenerateTerrainMesh(noise, MeshHeightMultiplier, MeshHeightCurve, levelOfDetail);
        lock (MeshDataThreadInfoQueue)
        {
            MeshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, MeshData));
        }
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    private void Update()
    {
        if(MapDataThreadInfoQueue.Count > 0)
        {
            for(var i=0;i<MapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<float[,]> threadInfo = MapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (MeshDataThreadInfoQueue.Count > 0)
        {
            for (var i = 0; i < MeshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = MeshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo (Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }

    }
}


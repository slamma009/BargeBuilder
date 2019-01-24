using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{

    public Material MeshMaterial;
    public const float maxViewDst = 450;
    public Transform viewer;

    static WorldGenerator WorldGen;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        WorldGen = FindObjectOfType<WorldGenerator>();
        Debug.Log(WorldGen.name);
        chunkSize = WorldGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, MeshMaterial));
                }

            }
        }
    }

    public class TerrainChunk
    {

        GameObject MeshObject;
        Vector2 Position;
        Bounds Bounds;

        MeshRenderer Renderer;
        MeshFilter Filter;
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            Position = coord * size;
            Bounds = new Bounds(Position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(Position.x, 0, Position.y);

            MeshObject = new GameObject("Terrain Chunk");
            Renderer = MeshObject.AddComponent<MeshRenderer>();
            Filter = MeshObject.AddComponent<MeshFilter>();
            MeshObject.transform.position = positionV3;
            MeshObject.transform.parent = parent;

            Renderer.material = material;

            SetVisible(false);
            WorldGen.RequestMapData(OnMapDataReceive);
        }

        void OnMapDataReceive(float[,] mapData)
        {
            Debug.Log("Got Map Data");
            WorldGen.RequestMeshData(OnMeshDataReceived, mapData);
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            Filter.mesh = meshData.CreateMesh();
        }
        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(Bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            MeshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return MeshObject.activeSelf;
        }

    }
}
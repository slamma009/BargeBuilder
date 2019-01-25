using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    Mesh TerrainMesh;
    // Start is called before the first frame update

    public int Seed;
    public LODInfo[] DetailLevels;
    public float PerlinScale = 1;
    public float HeightScale = 20;
    public AnimationCurve MeshHeightCurve;
    public Material MeshMaterial;
    public int Octives = 4;
    public float Persistance = 0.5f;
    public float Lacurarity = 2;
    public Vector2Int MeshOffset;

    public Transform Viewer;


    private readonly int GridSize = 64;

    public Ore[] Ores;


    public Vector2 ViewerPosition;
    Vector2 ViewerPositionOld;

    float MaxViewDistance;
    int ChunkSize;
    int ChunksVisibleInViewDst;


    Dictionary<Vector2, TerrainChunk> TerrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> TerrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    public void Start()
    {
        Initiate();
    }

    public void Initiate()
    {

        MaxViewDistance = DetailLevels[DetailLevels.Length - 1].visibleDstThreshold;
        ChunkSize = GridSize;
        ChunksVisibleInViewDst = Mathf.RoundToInt(MaxViewDistance / ChunkSize);
 
    }

    void Update()
    {
        UpdateVisibleChunks();

        //viewerPosition = new Vector2(Viewer.position.x, Viewer.position.z) / scale;
        ViewerPosition = new Vector2(Viewer.position.x, Viewer.position.z);
        //if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        //{
        //    viewerPositionOld = viewerPosition;
        //    UpdateVisibleChunks();
        //}
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < TerrainChunksVisibleLastUpdate.Count; i++)
        {
            TerrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        TerrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);
        

        for (int yOffset = -ChunksVisibleInViewDst; yOffset <= ChunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -ChunksVisibleInViewDst; xOffset <= ChunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (TerrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    float viewerDstFromNearestEdge = Mathf.Sqrt(TerrainChunkDictionary[viewedChunkCoord].ChunkBounds.SqrDistance(ViewerPosition));
                    
                    int lodIndex = 0;
                    for (int i = 0; i < DetailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > DetailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!TerrainChunkDictionary[viewedChunkCoord].Renderer.enabled)
                    {
                        TerrainChunkDictionary[viewedChunkCoord].SetVisible(true);
                    }
                    TerrainChunkDictionary[viewedChunkCoord].UpdateChunk(Octives, Persistance, Lacurarity, PerlinScale, MeshHeightCurve, HeightScale, Seed, DetailLevels[lodIndex].lod);
                    TerrainChunksVisibleLastUpdate.Add(TerrainChunkDictionary[viewedChunkCoord]);
                }
                else
                {
                    TerrainMesh meshObj = new GameObject().AddComponent<TerrainMesh>();
                    meshObj.name = "Terrain Chunk (" + viewedChunkCoord.x +", " + viewedChunkCoord.y + ")";
                    meshObj.transform.parent = this.transform;

                    meshObj.transform.position = new Vector3(viewedChunkCoord.x * GridSize, 0, viewedChunkCoord.y * GridSize);

                    MeshRenderer renderer = meshObj.gameObject.AddComponent<MeshRenderer>();
                    renderer.material = MeshMaterial; ;

                    Vector2 newOffset = new Vector2(viewedChunkCoord.x * GridSize, viewedChunkCoord.y * GridSize);
                    TerrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(meshObj, renderer, meshObj.gameObject.AddComponent<MeshFilter>(), newOffset, GridSize));
                }

            }
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;
    }

    private void OnValidate()
    {
        if (PerlinScale < 0.001f)
            PerlinScale = 0.001f;
        foreach (Vector2 key in TerrainChunkDictionary.Keys)
        {
            TerrainChunkDictionary[key].ClearMesh(TerrainChunkDictionary[key].SetLevelOfDetail);
            TerrainChunkDictionary[key].SetLevelOfDetail = 0;
            TerrainChunkDictionary[key].UpdateChunk(Octives, Persistance, Lacurarity, PerlinScale, MeshHeightCurve, HeightScale, Seed, TerrainChunkDictionary[key].SetLevelOfDetail);
        }
    }
}


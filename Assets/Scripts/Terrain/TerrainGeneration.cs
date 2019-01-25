using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGeneration : MonoBehaviour
{
    Mesh TerrainMesh;
    // Start is called before the first frame update
    
    public int GridSize = 64;
    public float GridScale = 2;
    public float PerlinScale = 1;
    public int Octives = 4;
    public float Persistance = 0.5f;
    public float Lacurarity = 2;
    public float HeightScale = 20;
    public Vector2Int MeshOffset;
    public Material MeshMaterial;

    public AnimationCurve MeshHeightCurve;

    public int Seed;
    
    public Ore[] Ores;
    float[,] HeightGrid;
    public void Start()
    {
        Initiate();
    }
    List<EditorTerrainMeshData> CreatedMeshes = new List<EditorTerrainMeshData>();
    public void Initiate()
    {

        for(var z = -2; z < 2; ++z)
        {
            for(var x = -2; x < 2; ++x)
            {
                TerrainMesh meshObj = new GameObject().AddComponent<TerrainMesh>();

                meshObj.transform.position = new Vector3(x * GridSize, 0, z * GridSize);

                meshObj.gameObject.AddComponent<MeshRenderer>().material = MeshMaterial;

                Vector2 newOffset = new Vector2(x * (GridSize), z * (GridSize));
                CreatedMeshes.Add(new EditorTerrainMeshData(meshObj, meshObj.gameObject.AddComponent<MeshFilter>(), newOffset));
                CreatedMeshes[CreatedMeshes.Count - 1].GenerateMesh(Octives, Persistance, Lacurarity, PerlinScale, MeshHeightCurve, HeightScale, GridSize, Seed);
            }
        }

    }


    private void OnValidate()
    {
        foreach(EditorTerrainMeshData data in CreatedMeshes)
        {
            data.GenerateMesh(Octives, Persistance, Lacurarity, PerlinScale, MeshHeightCurve, HeightScale, GridSize, Seed);
        }
    }


    struct EditorTerrainMeshData
    {
        public TerrainMesh Mesh;
        private MeshFilter Filter;
        private Vector2 Offset; 

        public EditorTerrainMeshData(TerrainMesh mesh, MeshFilter filter, Vector2 position)
        {
            this.Mesh = mesh;
            this.Filter = filter;
            this.Offset = position;
        }

        public void GenerateMesh(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, int seed)
        {
            Filter.mesh = Mesh.GenerateMesh(octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, gridSize, Offset, seed);
        }
    }




}

// n1, n3, n2, n4
// v1, v3, v2, v4
// 0, 2, 1,

// 1, 2, 3

//Debug.Log(Vertecies[Triangles[5]]);
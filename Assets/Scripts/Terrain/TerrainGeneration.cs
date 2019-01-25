using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGeneration : MonoBehaviour
{
    Mesh TerrainMesh;
    // Start is called before the first frame update

    public int Seed;
    public float PerlinScale = 1;
    public float HeightScale = 20;
    public AnimationCurve MeshHeightCurve;
    public Material MeshMaterial;
    public int Octives = 4;
    public float Persistance = 0.5f;
    public float Lacurarity = 2;
    public Vector2Int MeshOffset;


    private readonly int GridSize = 64;

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
                CreatedMeshes.Add(new EditorTerrainMeshData(meshObj, meshObj.gameObject.AddComponent<MeshFilter>(), newOffset, GridSize));
                CreatedMeshes[CreatedMeshes.Count - 1].GenerateMesh(Octives, Persistance, Lacurarity, PerlinScale, MeshHeightCurve, HeightScale, Seed);
            }
        }

    }


    private void OnValidate()
    {
        foreach(EditorTerrainMeshData data in CreatedMeshes)
        {
            data.GenerateMesh(Octives, Persistance, Lacurarity, PerlinScale, MeshHeightCurve, HeightScale, Seed);
        }
    }


    struct EditorTerrainMeshData
    {
        private readonly TerrainMesh MeshLogic;
        private readonly MeshFilter Filter;
        private readonly Vector2 Offset;
        public int LevelOfDetail;
        private readonly int GridSize;

        public EditorTerrainMeshData(TerrainMesh mesh, MeshFilter filter, Vector2 position, int gridSize, int levelOfDetail = 1)
        {
            this.MeshLogic = mesh;
            this.Filter = filter;
            this.Offset = position;
            this.LevelOfDetail = levelOfDetail;
            this.GridSize = gridSize;
        }

        public void GenerateMesh(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int seed)
        {
            MeshLogic.RequestHeight(NoiseCallback, octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, GridSize, Offset, seed);
        }

        void NoiseCallback(float[,] heightGrid)
        {
            MeshLogic.RequestMeshData(MeshCallBack , GridSize, heightGrid);
        }

        void MeshCallBack(MeshData data)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = data.Verticies;
            mesh.normals = data.Normals;
            mesh.triangles = data.Triangles;
            mesh.uv = data.UVs;
            Filter.mesh = mesh;
        }
    }




}

// n1, n3, n2, n4
// v1, v3, v2, v4
// 0, 2, 1,

// 1, 2, 3

//Debug.Log(Vertecies[Triangles[5]]);
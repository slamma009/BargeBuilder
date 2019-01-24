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
    public float HeightScale = 20;
    public Vector2Int MeshOffset;
    public Material MeshMaterial;

    public AnimationCurve MeshHeightCurve;

    public float Seed;
    
    public Ore[] Ores;
    float[,] HeightGrid;
    public void Start()
    {
        Initiate();
    }
    public void Initiate()
    {

        for(var z = 0; z < 4; ++z)
        {
            for(var x = 0; x < 4; ++x)
            {
                TerrainMesh meshObj = new GameObject().AddComponent<TerrainMesh>();
                meshObj.transform.position = new Vector3(x * GridSize, 0, z * GridSize);
                Vector2 newOffset = new Vector2(x * GridSize, z * GridSize);
                meshObj.gameObject.AddComponent<MeshFilter>().mesh = meshObj.GenerateMesh(Seed, PerlinScale, MeshHeightCurve, HeightScale, GridSize, newOffset);
                meshObj.gameObject.AddComponent<MeshRenderer>().material = MeshMaterial;
            }
        }

    }




    
}

// n1, n3, n2, n4
// v1, v3, v2, v4
// 0, 2, 1,

// 1, 2, 3

//Debug.Log(Vertecies[Triangles[5]]);
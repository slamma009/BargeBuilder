using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGeneration : MonoBehaviour
{
    Mesh TerrainMesh;
    // Start is called before the first frame update
    
    public Vector2Int GridSize = new Vector2Int(64, 64);
    public float GridScale = 2;
    public float First = 10;
    public float Second = 1;
    public float PerlinScale = 1;
    public Vector2Int Offset;

    public float Seed;
    
    public Ore[] Ores;
    float[,] HeightGrid;

    public void Initiate()
    {
        HeightGrid = new float[GridSize.x + 1, GridSize.y + 1];

        for (int z = 0; z <= GridSize.y; ++z)
        {
            for (var x = 0; x <= GridSize.x; ++x)
            {

                HeightGrid[x, z] = GetHeight(x, z);
            }
        }
        TerrainMesh = GenerateMesh();
        GetComponent<MeshFilter>().mesh = TerrainMesh;

    }

    private float savedTime;
    private void FixedUpdate()
    {
        if (Time.time - savedTime > 1)
        {
            savedTime = Time.time;
        }
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> Verticies = new List<Vector3>();
        List<Vector3> Normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int z = 0, i = 0; z < GridSize.y; ++z)
        {
            for (var x = 0; x < GridSize.x; ++x)
            {
                Verticies.AddRange(new Vector3[] {
                    new Vector3(x * GridScale,GetAverageHeight(x,z),z * GridScale),
                    new Vector3(x * GridScale,GetAverageHeight(x, z + 1),(z+1) * GridScale),
                    new Vector3((x+1) * GridScale,GetAverageHeight(x + 1, z),z * GridScale)
                });
                Verticies.AddRange(new Vector3[] {
                    new Vector3(x * GridScale,GetAverageHeight(x, z + 1),(z+1) * GridScale),
                    new Vector3((x+1) * GridScale,GetAverageHeight(x + 1, z),z * GridScale),
                    new Vector3((x+1)* GridScale, GetAverageHeight(x + 1, z + 1), (z + 1)* GridScale)
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

            float offset = 0;
            for (var j = 0; j < Ores.Length; ++j)
            {
                if(Ores[j].GetOreChance(xPos, y, Offset, GridSize) > Ores[j].Chance)
                {
                    offset = (j + 1) * 0.25f;
                    break;
                }
            }
            uvs.Add(new Vector2(offset, 0.75f));
            uvs.Add(new Vector2(offset, 1));
            uvs.Add(new Vector2(offset + 0.25f, 0.75f));
            uvs.Add(new Vector2(offset, 1));
            uvs.Add(new Vector2(offset + 0.25f, 0.75f));
            uvs.Add(new Vector2(offset + 0.25f, 1));

            xPos++;
            if(xPos >= GridSize.x)
            {
                y++;
                xPos = 0;
            }
        }

        mesh.uv = uvs.ToArray();



        return mesh;
    }

    public float GetAverageHeight(int x, int z)
    {
        float height = !(x > GridSize.x || z + 1 > GridSize.y) ? HeightGrid[x, z + 1] : GetHeight(x, z + 1);
        height += !(x+1 > GridSize.x || z > GridSize.y) ? HeightGrid[x + 1, z] : GetHeight(x + 1, z);
        height += z - 1 >=0 ? HeightGrid[x, z - 1] : GetHeight(x, z - 1);
        height += x - 1 >= 0  ? HeightGrid[x - 1, z] : GetHeight(x - 1, z);

        return height * 0.25f * GridScale;
    }


    public float GetHeight(int x, int z)
    {
        return Mathf.Round(Mathf.PerlinNoise((Seed + (Offset.x + x) / (float)(GridSize.x + 1)) * PerlinScale, (Seed + (Offset.y + z) / (float)(GridSize.y + 1)) * PerlinScale) * First) / Second;
    }

    
}

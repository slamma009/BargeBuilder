using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{

    public Mesh GenerateMesh(float seed, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, Vector2 offset)
    {
        float[,] heightGrid = GetHeight(seed, perlinScale, meshHeightCurve, heightScale, gridSize, offset);
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

    public float[,] GetHeight(float seed, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int gridSize, Vector2 offset)
    {
        float[,] heightGrid = new float[gridSize + 1, gridSize + 1];
        for (int z = 0; z <= gridSize; ++z)
        {
            for (var x = 0; x <= gridSize; ++x)
            {
                float height = Mathf.PerlinNoise((seed + (offset.x + x) / (float)(gridSize + 1)) * perlinScale, (seed + (offset.y + z) / (float)(gridSize + 1)) * perlinScale);
                heightGrid[x, z] = meshHeightCurve.Evaluate(height) * heightScale;
            }
        }
        return heightGrid;
    }
}

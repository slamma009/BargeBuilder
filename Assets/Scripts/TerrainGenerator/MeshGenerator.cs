using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class MeshGenerator
{

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = ((width - 1) * 2 + 1)  / meshSimplificationIncrement;

        int vertexIndex = 0;

        List<Vector3> verticies = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < height - 1; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width - 1; x += meshSimplificationIncrement)
            {
                try
                { }
                catch (Exception ex)
                {
                    Debug.Log(vertexIndex + " :s " + (verticesPerLine * verticesPerLine) + " :x " + x + " :y " + y + " :w " + width + " :h " + height);
                    throw new Exception();
                }
                verticies.Add(new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y));
                verticies.Add(new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - (y + 1)));
                verticies.Add(new Vector3(topLeftX + (x + 1), heightCurve.Evaluate(heightMap[(x + 1), (y + 1)]) * heightMultiplier, topLeftZ - y));
                verticies.Add(new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - (y + 1)));
                verticies.Add(new Vector3(topLeftX + (x + 1), heightCurve.Evaluate(heightMap[(x + 1), (y + 1)]) * heightMultiplier, topLeftZ - y));
                verticies.Add(new Vector3(topLeftX + (x + 1), heightCurve.Evaluate(heightMap[(x + 1), (y + 1)]) * heightMultiplier, topLeftZ - (y + 1)));


                
                
                triangles.AddRange(new int[] {vertexIndex, vertexIndex + 1, vertexIndex + 2,
                    vertexIndex + 3, vertexIndex + 5, vertexIndex + 4});
                

                float textureOffset = 0;
                uvs.Add(new Vector2(textureOffset, 0.75f));
                uvs.Add(new Vector2(textureOffset, 1));
                uvs.Add(new Vector2(textureOffset + 0.25f, 0.75f));
                uvs.Add(new Vector2(textureOffset, 1));
                uvs.Add(new Vector2(textureOffset + 0.25f, 0.75f));
                uvs.Add(new Vector2(textureOffset + 0.25f, 1));
                vertexIndex += 6;
            }
        }
        MeshData meshData = new MeshData(verticies.Count, triangles.Count);
        meshData.triangles = triangles.ToArray();
        meshData.uvs = uvs.ToArray();
        meshData.vertices = verticies.ToArray();
        return meshData;

    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int vertexCount, int triCount)
    {
        vertices = new Vector3[vertexCount];
        uvs = new Vector2[vertexCount];
        triangles = new int[triCount];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

}
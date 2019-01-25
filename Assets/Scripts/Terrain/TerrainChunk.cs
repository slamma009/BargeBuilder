using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public readonly Bounds ChunkBounds;
    public readonly MeshRenderer Renderer;
    private readonly TerrainMesh MeshLogic;
    private readonly MeshFilter Filter;
    private readonly Vector2 Offset;
    private readonly int GridSize;
    public int SetLevelOfDetail = 0;

    private Dictionary<int, LODMesh> LevelOfDetailMeshes = new Dictionary<int, LODMesh>();

    public TerrainChunk(TerrainMesh mesh, MeshRenderer renderer, MeshFilter filter, Vector2 position, int gridSize)
    {
        this.MeshLogic = mesh;
        this.Renderer = renderer;
        this.Filter = filter;
        this.Offset = position;
        this.GridSize = gridSize;
        this.ChunkBounds = new Bounds(position, Vector2.one * GridSize);
    }

    public void SetVisible(bool visible)
    {
        Renderer.enabled = visible;
    }
    
    public void ClearMesh(int lodKey)
    {
        if (LevelOfDetailMeshes.ContainsKey(lodKey))
        {
            LevelOfDetailMeshes.Remove(lodKey);
        }
    }

    public void UpdateChunk(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int seed, int levelOfDetail)
    {
        if (levelOfDetail != SetLevelOfDetail)
        {
            if (!LevelOfDetailMeshes.ContainsKey(levelOfDetail))
            {
                GenerateMesh(octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, seed, levelOfDetail);
                LevelOfDetailMeshes.Add(levelOfDetail, new LODMesh());
            }

            if (LevelOfDetailMeshes[levelOfDetail].MeshIsGenerated)
            {
                Filter.mesh = LevelOfDetailMeshes[levelOfDetail].TerrainMesh;
                SetLevelOfDetail = levelOfDetail;
            }
        }
    }

    public void GenerateMesh(int octaves, float persistance, float lacunarity, float perlinScale, AnimationCurve meshHeightCurve, float heightScale, int seed, int levelOfDetail)
    {
        MeshLogic.RequestHeight(NoiseCallback, octaves, persistance, lacunarity, perlinScale, meshHeightCurve, heightScale, GridSize, Offset, seed, levelOfDetail);
    }

    void NoiseCallback(NoiseData noiseData)
    {
        MeshLogic.RequestMeshData(MeshCallBack, GridSize, noiseData.HeightGrid, noiseData.LevelOfDetail);
    }

    void MeshCallBack(MeshData data)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = data.Verticies;
        mesh.normals = data.Normals;
        mesh.triangles = data.Triangles;
        mesh.uv = data.UVs;

        LevelOfDetailMeshes[data.LevelOfDetail].TerrainMesh = mesh;
        LevelOfDetailMeshes[data.LevelOfDetail].MeshIsGenerated = true;
    }

}


public class LODMesh
{
    public Mesh TerrainMesh;
    public bool MeshIsGenerated;
}

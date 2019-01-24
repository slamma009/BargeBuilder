using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDisplay : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawMesh(MeshData meshData, Material material)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.material = material;
    }
}

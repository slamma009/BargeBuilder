using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGeneration))]
public class TerrainGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TerrainGeneration tg = (TerrainGeneration)target;
        if (GUILayout.Button("Update Meshes"))
            tg.EditorUpdateMesh();
    }
}

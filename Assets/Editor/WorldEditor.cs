using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {
    void OnSceneGUI() {
        World w = target as World;
        if (Handles.Button(w.transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CubeHandleCap)) {
            w.GenerateDataMap();
        }
    }

    public override void OnInspectorGUI() {
        World w = target as World;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate Test Map")) {
            w.GenerateDataMap();
            w.UpdateChunks();
        }
        if (GUILayout.Button("Destroy Test")) {
            w.DestroyImmediateChunks();
        }
    }
}

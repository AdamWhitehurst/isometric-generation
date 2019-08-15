using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldMap))]
public class WorldMapEditor : Editor {
    private Editor _editor;
    void OnSceneGUI() {
        WorldMap w = target as WorldMap;
        if (Handles.Button(w.transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CubeHandleCap)) {
            // w.GenerateWorld();
        }
    }

    public override void OnInspectorGUI() {
        WorldMap w = target as WorldMap;

        DrawDefaultInspector();
        serializedObject.Update();
        if (GUILayout.Button("Generate Test Map")) {
            // w.GenerateWorld();
            // w.UpdateAllChunks();
        }
        if (GUILayout.Button("Destroy Test Map")) {
            w.DestroyAllChunks();
        }
        var worldPreset = serializedObject.FindProperty("preset");
        CreateCachedEditor(worldPreset.objectReferenceValue, null, ref _editor);
        _editor.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }
}

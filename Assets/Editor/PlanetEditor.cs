using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Worlds;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    #region Fields

    private Planet _planet;
    #endregion

    public override void OnInspectorGUI() {
        _planet = (Planet)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Planet")) {
            _planet.Initialize();
        }
        if (_planet.preset) DrawPresetEditor(_planet.preset, _planet.Initialize);
    }

    void DrawPresetEditor(Object preset, System.Action onPresetUpdated) {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            Editor editor = CreateEditor(preset);
            editor.OnInspectorGUI();

            // if (check.changed) {
            //     if (onPresetUpdated != null) {
            //         onPresetUpdated();
            //     }
            // }
        }
    }

}

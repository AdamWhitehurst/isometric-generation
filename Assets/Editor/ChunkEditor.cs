using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Worlds;

[CustomEditor(typeof(Chunk))]
public class ChunkEditor : Editor {
    #region Fields

    private Chunk _chunk;

    #endregion

    #region ChunkEditor Methods

    public override void OnInspectorGUI() {
        _chunk = (Chunk)target;

        _chunk.CheckForUpdate();

    }

    #endregion
}

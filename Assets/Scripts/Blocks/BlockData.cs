using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Blocks {
    [Serializable]
    [CreateAssetMenu(fileName = "New BlockData", menuName = "BlockData", order = 0)]
    public class BlockData : ScriptableObject {
        public Sprite Top;
        public Sprite North;
        public Sprite South;
        public Sprite East;
        public Sprite West;
        public Sprite Bot;
    }
}
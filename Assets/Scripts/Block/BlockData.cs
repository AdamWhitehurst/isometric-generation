using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Block {
    [Serializable]
    [CreateAssetMenu(fileName = "New BlockData", menuName = "BlockData", order = 0)]
    public class BlockData : ScriptableObject {
        [UniqueIdentifier]
        public string UID;
        public ushort Top;
        public ushort North;
        public ushort South;
        public ushort East;
        public ushort West;
        public ushort Bot;

        public BlockType type;

        public BlockData(BlockType type, ushort face) {

            this.type = type;
            Top = face;
            North = face;
            South = face;
            East = face;
            West = face;
            Bot = face;
        }

        public BlockData(BlockType type, ushort topFace, ushort botFace, ushort sideFace) {

            this.type = type;
            Top = topFace;
            North = sideFace;
            South = sideFace;
            East = sideFace;
            West = sideFace;
            Bot = botFace;
        }

        public BlockData(BlockType type, ushort topFace, ushort northFace, ushort southFace, ushort eastFace, ushort westFace, ushort botFace) {

            this.type = type;
            Top = topFace;
            North = northFace;
            South = southFace;
            East = eastFace;
            West = westFace;
            Bot = botFace;
        }

        public ushort this[Face face] {
            get {
                switch (face) {
                    case (Face.Top):
                        return this.Top;
                    case (Face.North):
                        return this.North;
                    case (Face.South):
                        return this.South;
                    case (Face.East):
                        return this.East;
                    case (Face.West):
                        return this.West;
                    case (Face.Bot):
                        return this.Bot;
                    default:
                        throw new System.Exception($"BlockData Face not set: {type}, {face}");
                }
            }
        }
    }
}
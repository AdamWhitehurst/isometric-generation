using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;

namespace Generation {

    public class Sizes {
        public const int MapHorizontal = 16;
        public const int MapVertical = 160;

        public const int ChunkSize = 16;

        public static int MapWorldSizeHorizontal {
            get {
                return MapHorizontal * ChunkSize;
            }
        }

        public static int MapWorldSizeVertical {
            get {
                return MapVertical * ChunkSize;
            }
        }

    }

    [System.Serializable]
    public class Strata {
        [SerializeField]
        public string name;
        [SerializeField]
        [Range(0f, 1f)]
        public float end;

        [SerializeField]
        [Range(0.1f, 100f)]
        public float scale = 1;

        [SerializeField]
        [Range(0.1f, 10f)]
        public float power = 1;

        [SerializeField]
        public BlockType[] blockDistribution;
    }


}
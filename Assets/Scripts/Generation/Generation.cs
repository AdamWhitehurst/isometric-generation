using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;

namespace Generation {

    public class Sizes {
        public const int MapHorizontal = 64;
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

}
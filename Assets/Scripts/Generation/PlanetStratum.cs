using System.Collections;
using System.Collections.Generic;
using Block;
using UnityEngine;

namespace Generation {
    [System.Serializable]
    public class PlanetStratum {

        [SerializeField]
        public string name;

        public int size = 1;

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
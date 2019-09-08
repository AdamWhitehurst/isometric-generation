using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Blocks;

namespace Worlds {
    [Serializable]
    [CreateAssetMenu(fileName = "New Planet Preset", menuName = "Planet Preset", order = 0)]
    public class Preset : ScriptableObject {


        [SerializeField]
        public string presetName = "";

        public string planetSeed = "1337";

        public Material planetMaterial = null;

        public int totalSize = -1;

        public int centerSize = -1;
        public int extent = -1;

        [Range(0.01f, 500f)]
        public float mapElevationScale = 30;
        [Range(0.1f, 10f)]
        public float mapElevationPower = 1;
        [Range(0f, 1f)]
        public float mapElevationDamping = 0.5f;

        public float lacunarity = 3;

        public int octaves = 4;

        public FastNoise.FractalType fractalType = FastNoise.FractalType.Billow;

        public FastNoise.NoiseType noiseType = FastNoise.NoiseType.Cellular;

        [SerializeField]
        public PlanetStratum[] strata;

        public void OnValidate() {
            if (presetName == null) {
                throw new System.Exception("Planet preset cannot have an empty name!");
            }

            if (strata.Length == 0) {
                throw new System.Exception($"{presetName} must have at least on stratum.");
            }
            int newExtent = 0;
            for (int i = 0; i < strata.Length; i++) {
                PlanetStratum s = strata[i];
                if (i != 0) newExtent += s.size;

                if (s.name == null) {
                    s.name = $"Stratum {i}";
                }
                if (s.blockDistribution.Length == 0) {
                    throw new System.Exception($"{s.name} needs a block distribution");
                }
            }
            this.centerSize = strata[0].size;
            this.extent = newExtent;
            this.totalSize = this.centerSize + (this.extent * 2);

            // if (biomeMap == null) {
            //     Debug.LogWarning($"{presetName} needs a Biome Map! Cannot add strata until then");
            //     strata = null;
            //     return;
            // }

            // int biomeMapColorCount = 0;
            // List<Color> colorList = new List<Color>();
            // colorToStrataDict = new Dictionary<Color, PlanetStratum>();

            // for (int x = 0; x < biomeMap.width; x++) {
            //     for (int z = 0; z < biomeMap.height; z++) {
            //         Color pixel = biomeMap.GetPixel(x, z);
            //         if (!colorList.Contains(pixel)) {
            //             biomeMapColorCount++;
            //             colorList.Add(pixel);
            //         }
            //     }
            // }

            // if (strata.Length != biomeMapColorCount) {
            //     Array.Resize(ref strata, biomeMapColorCount);
            // }

            // if (!biomeMap.isReadable) {
            //     Debug.LogWarning("Please check biomeMap's texture 'Read/Write Enabled' setting in inspector!");
            // }

            // float maxStrataEnd = 0;
            // for (int i = 0; i < strata.Length; i++) {
            //     PlanetStratum s = strata[i];
            //     s.name = $"Strata {i}";
            //     colorToStrataDict.Add(colorList[i], s);
            //     if (s.end <= maxStrataEnd) {
            //         Debug.LogWarning($"{s.name} needs a higher end, Strata must cascade in increasing end levels");
            //         s.end = maxStrataEnd;
            //     } else {
            //         maxStrataEnd = s.end;
            //     }
            //     if (s.blockDistribution.Length == 0) {
            //         Debug.LogWarning($"{s.name} needs a block distribution");
            //     }
            // }

        }
    }

}
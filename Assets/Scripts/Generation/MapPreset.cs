using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Block;

namespace Generation {
    [Serializable]
    [CreateAssetMenu(fileName = "New WorldPreset", menuName = "WorldPreset", order = 0)]
    public class MapPreset : ScriptableObject {
        [SerializeField]
        public string presetName = "New Map Preset";

        [Range(0.01f, 50f)]
        public float mapElevationScale = 30;
        [Range(0.1f, 10f)]
        public float mapElevationPower = 1;

        public int verticalOffset = 0;

        [SerializeField]
        public Dictionary<Color, Strata> colorToStrataDict;

        [SerializeField] public Texture2D biomeMap;

        [SerializeField]
        public Strata[] strata;

        public void OnValidate() {
            if (biomeMap == null) {
                Debug.LogWarning($"{presetName} needs a Biome Map! Cannot add strata until then");
                strata = null;
                return;
            }

            int biomeMapColorCount = 0;
            List<Color> colorList = new List<Color>();
            colorToStrataDict = new Dictionary<Color, Strata>();

            for (int x = 0; x < biomeMap.width; x++) {
                for (int z = 0; z < biomeMap.height; z++) {
                    Color pixel = biomeMap.GetPixel(x, z);
                    if (!colorList.Contains(pixel)) {
                        biomeMapColorCount++;
                        colorList.Add(pixel);
                    }
                }
            }

            if (strata.Length != biomeMapColorCount) {
                Array.Resize(ref strata, biomeMapColorCount);
            }

            if (!biomeMap.isReadable) {
                Debug.LogWarning("Please check biomeMap's texture 'Read/Write Enabled' setting in inspector!");
            }

            float maxStrataEnd = 0;
            for (int i = 0; i < strata.Length; i++) {
                Strata s = strata[i];
                s.name = $"Strata {i}";
                colorToStrataDict.Add(colorList[i], s);
                if (s.end <= maxStrataEnd) {
                    Debug.LogWarning($"{s.name} needs a higher end, Strata must cascade in increasing bandEnd levels");
                    s.end = maxStrataEnd;
                } else {
                    maxStrataEnd = s.end;
                }
                if (s.blockDistribution.Length == 0) {
                    Debug.LogWarning($"{s.name} needs a block distribution");
                }
            }

        }
    }

}
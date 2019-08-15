using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;

public class EndlessTerrain : MonoBehaviour {

    /// <summary>
    /// Number of blocks visible to viewer
    /// </summary>
    public const float maxViewDistance = 1000;

    public GameObject chunkPrefab = null;
    public GameObject blockItemPrefab = null;
    public bool useRandomSeed = true;

    public string seed;

    public Transform viewer;

    /// <summary>
    /// The position of the viewer in the XZ-plane
    /// </summary>
    public static Vector2 viewerHorizontalPosition;

    [SerializeField]
    MapPreset preset = null;

    Dictionary<Vector2, WorldMap> worldMapDictionary = new Dictionary<Vector2, WorldMap>();
    List<WorldMap> mapsVisibleLastUpdate = new List<WorldMap>();

    FastNoise noise;

    /// <summary>
    /// Number of maps visible to viewer, based on maxViewDistance
    /// </summary>
    int mapsVisibleInViewDist {
        get {
            return Mathf.RoundToInt(maxViewDistance / Sizes.MapWorldSizeHorizontal);
        }
    }

    void Start() {
        if (useRandomSeed) seed = System.DateTime.Now.ToString();
        int worldSeed = seed.GetHashCode();
        noise = new FastNoise(worldSeed);
        noise.SetNoiseType(FastNoise.NoiseType.Simplex);
        noise.SetFractalOctaves(1);
    }

    void Update() {
        viewerHorizontalPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleMaps();
    }

    void UpdateVisibleMaps() {

        ResetMapsVisibleLastUpdate();

        // Vector2.y used for z axis of world
        int viewerMapCoordX = Mathf.RoundToInt(viewerHorizontalPosition.x / Sizes.ChunkSize);
        int viewerMapCoordZ = Mathf.RoundToInt(viewerHorizontalPosition.y / Sizes.ChunkSize);

        for (int zOffset = -mapsVisibleInViewDist; zOffset <= mapsVisibleInViewDist; zOffset++) {
            for (int xOffset = -mapsVisibleInViewDist; xOffset <= mapsVisibleInViewDist; xOffset++) {
                Vector2 viewerMapCoord = new Vector2(viewerMapCoordX + xOffset, viewerMapCoordZ + zOffset);

                if (worldMapDictionary.ContainsKey(viewerMapCoord)) {
                    worldMapDictionary[viewerMapCoord].UpdateViewable(viewerMapCoord, maxViewDistance);
                    if (worldMapDictionary[viewerMapCoord].IsVisible) {
                        mapsVisibleLastUpdate.Add(worldMapDictionary[viewerMapCoord]);
                    }
                } else {
                    WorldMap newWorldMap = new GameObject().AddComponent<WorldMap>();
                    newWorldMap.name = $"{viewerMapCoord.x} , {viewerMapCoord.y}";
                    newWorldMap.IsVisible = false;
                    newWorldMap.SetParentWorld(this);
                    newWorldMap.SetMapPreset(preset);
                    newWorldMap.SetWorldPositionFromCoord(viewerMapCoord);
                    newWorldMap.GenerateWorld(noise);

                    worldMapDictionary.Add(viewerMapCoord, newWorldMap);
                }
            }
        }
    }

    /// <summary>
    /// Make all WorldMaps in mapsVisibleLastUpdate
    /// visiblility to false and clears the mapsVisibleLastUpdate list
    /// </summary>
    void ResetMapsVisibleLastUpdate() {
        foreach (WorldMap map in mapsVisibleLastUpdate) {
            map.IsVisible = false;
        }
        mapsVisibleLastUpdate.Clear();
    }

    public void CallMapChunkUpdate(WorldMap map, int chunkX, int chunkY, int chunkZ) {
        // /Debug.Log("TODO: CallMapChunkUpdate method implementation");
    }
}

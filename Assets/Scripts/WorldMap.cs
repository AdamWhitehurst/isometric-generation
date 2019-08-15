using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;
// using SimplexNoise;
using Generation;
public class WorldMap : MonoBehaviour {

    public BlockType[,,] data;

    private float[,] _elevationMap;
    public Chunk[,,] chunks;

    [SerializeField]
    private MapPreset preset = null;

    public EndlessTerrain world;


    /// <summary>
    /// Position of map in XZ-plane
    /// Note: horizontalPosition is Z-Axis
    /// </summary>
    private Vector2 horizontalPosition;

    /// <summary>
    /// The XZ-plane bounding box of this Map adjusted to world size
    /// </summary>
    private Bounds worldBounds;

    /// <summary>
    /// DO NOT USE. Backing store for IsVisible property. 
    /// Use property IsVisible instead 
    /// </summary>
    private bool _visible;

    /// <summary>
    /// Property controlling whether map is visible and active
    /// </summary>
    /// <value></value>
    public bool IsVisible {
        get {
            return _visible;
        }
        set {
            this.gameObject.SetActive(value);
            _visible = value;
        }
    }

    public void SetParentWorld(EndlessTerrain world) {
        this.world = world;
        this.transform.SetParent(world.transform, false);
    }

    public void SetMapPreset(MapPreset preset) {
        this.preset = preset;
    }

    public void SetWorldPositionFromCoord(Vector2 coord) {
        horizontalPosition = coord * Sizes.MapHorizontal;
        worldBounds = new Bounds(horizontalPosition, Vector2.one * Sizes.MapHorizontal);
        Vector3 worldPosition = new Vector3(horizontalPosition.x, 0, horizontalPosition.y);
        this.transform.position = worldPosition;

    }

    public void UpdateViewable(Vector2 viewerMapCoord, float maxViewDistance) {
        float viewerDistanceFromEdge = Mathf.Sqrt(worldBounds.SqrDistance(viewerMapCoord));
        bool mapVisibleToViewer = viewerDistanceFromEdge <= maxViewDistance;
        IsVisible = mapVisibleToViewer;
    }

    public void GenerateWorld(FastNoise noise) {
        if (preset == null) {
            Debug.LogError("World preset is null");
            return;
        }
        _elevationMap = GenerateElevationMap(noise, Sizes.MapHorizontal, Sizes.MapHorizontal, world.seed);
        float[,] _elevationMap2 = GenerateElevationMap(noise, Sizes.MapHorizontal, Sizes.MapHorizontal, world.seed + "2394825209384");
        data = GenerateBlockMapFromBiomeMap(noise, _elevationMap, _elevationMap2, Sizes.MapVertical);

        GenerateChunks();
    }

    void GenerateChunks() {
        DestroyAllChunks();
        InitializeChunksArray();
        for (int x = 0; x < chunks.GetLength(0); x++) {
            for (int y = 0; y < chunks.GetLength(1); y++) {
                for (int z = 0; z < chunks.GetLength(2); z++) {

                    GameObject newChunk = Instantiate(world.chunkPrefab, new Vector3(x * Sizes.ChunkSize - 0.5f,
                     y * Sizes.ChunkSize + 0.5f, z * Sizes.ChunkSize - 0.5f), new Quaternion(0, 0, 0, 0)) as GameObject;
                    newChunk.transform.SetParent(this.transform, false);
                    chunks[x, y, z] = newChunk.GetComponent<Chunk>();
                    chunks[x, y, z].parentMap = gameObject;
                    chunks[x, y, z].mapX = x * Sizes.ChunkSize;
                    chunks[x, y, z].mapY = y * Sizes.ChunkSize;
                    chunks[x, y, z].mapZ = z * Sizes.ChunkSize;
                    chunks[x, y, z].needsUpdate = true;
                }
            }
        }
    }

    void InitializeChunksArray() {
        chunks = new Chunk[Mathf.FloorToInt(Sizes.MapHorizontal / Sizes.ChunkSize),
                            Mathf.FloorToInt(Sizes.MapVertical / Sizes.ChunkSize),
                            Mathf.FloorToInt(Sizes.MapHorizontal / Sizes.ChunkSize)];
    }

    public void DestroyAllChunks() {
        Chunk[] sceneChunks = this.gameObject.GetComponentsInChildren<Chunk>();
        foreach (Chunk chunk in sceneChunks) {
            if (chunk && chunk.gameObject) {
                if (Application.isEditor) {
                    DestroyImmediate(chunk.gameObject);
                } else {
                    Destroy(chunk.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Updates the chunk containing block at specified coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void UpdateChunkAt(int x, int y, int z) {
        int chunkX = Mathf.FloorToInt(x / Sizes.ChunkSize);
        int chunkY = Mathf.FloorToInt(y / Sizes.ChunkSize);
        int chunkZ = Mathf.FloorToInt(z / Sizes.ChunkSize);

        chunks[chunkX, chunkY, chunkZ].needsUpdate = true;


        // If block update occurred in first chunk of map ..
        if (x - (Sizes.ChunkSize * chunkX) == 0) {
            // But not the first block of first chunk ..
            if (chunkX != 0) {
                // Just update that chunk
                chunks[chunkX - 1, chunkY, chunkZ].needsUpdate = true;
            } else {
                // Otherwise, also call chunk in neighboring map
                world.CallMapChunkUpdate(this, Sizes.MapHorizontal - 1, chunkY, chunkZ);
            }
        }

        // If block update occurred in last chunk of map ..
        if (x - (Sizes.ChunkSize * chunkX) == 15) {
            // But not the last block of last chunk ..
            if (chunkX != chunks.GetLength(0) - 1) {
                // Just update that chunk
                chunks[chunkX + 1, chunkY, chunkZ].needsUpdate = true;
            } else {
                // Otherwise, also call chunk in neighboring map
                world.CallMapChunkUpdate(this, Sizes.MapHorizontal + 1, chunkY, chunkZ);
            }
        }
        // If block update occurred in first chunk of map ..
        if (z - (Sizes.ChunkSize * chunkZ) == 0) {
            // But not the first block of first chunk ..
            if (chunkZ != 0) {
                // Just update that chunk
                chunks[chunkX, chunkY, chunkZ - 1].needsUpdate = true;
            } else {
                // Otherwise, also call chunk in neighboring map
                world.CallMapChunkUpdate(this, chunkX, chunkY, Sizes.MapHorizontal - 1);
            }
        }
        // If block update occurred in last chunk of map ...
        if (z - (Sizes.ChunkSize * chunkZ) == 15) {
            // But not the last block of last chunk ..
            if (chunkZ != chunks.GetLength(2) - 1) {
                // Just update that chunk
                chunks[chunkX, chunkY, chunkZ + 1].needsUpdate = true;
            } else {
                // Otherwise, also call chunk in neighboring map
                world.CallMapChunkUpdate(this, Sizes.MapHorizontal + 1, chunkY, Sizes.MapHorizontal + 1);
            }
        }

        // World is assumed to only have one map in Y-axis, 
        // So no need to call updates to neighbor maps in either Y direction
        // (there are none). Only update chunks in that updated map if necessary
        if (y - (Sizes.ChunkSize * chunkY) == 0 && chunkY != 0) {
            chunks[chunkX, chunkY - 1, chunkZ].needsUpdate = true;
        }

        if (y - (Sizes.ChunkSize * chunkY) == 15 && chunkY != chunks.GetLength(1) - 1) {
            chunks[chunkX, chunkY + 1, chunkZ].needsUpdate = true;
        }
    }

    public float[,] GenerateElevationMap(FastNoise noise, int worldSizeX, int worldSizeZ, string seed) {

        float[,] eMap = new float[worldSizeX, worldSizeZ];

        noise.SetFrequency(1 / preset.mapElevationScale);
        for (int x = 0; x < worldSizeX; x++) {
            for (int z = 0; z < worldSizeZ; z++) {
                // Get value of noise in Range(-1f, 1f)
                float noiseVal = noise.GetSimplexFractal((Mathf.RoundToInt(horizontalPosition.x) + x), (Mathf.RoundToInt(horizontalPosition.y) + z));
                // Convert to Range(0f, 1f)
                float e = (noiseVal + 1) / 2;
                eMap[x, z] = Mathf.Pow(e, preset.mapElevationPower);

            }
        }

        return eMap;
    }

    public BlockType[,,] GenerateBlockMap(FastNoise noise, float[,] elevationMap, int worldSizeY) {
        int worldSizeX = elevationMap.GetLength(0);
        int worldSizeZ = elevationMap.GetLength(1);
        BlockType[,,] blockMap = new BlockType[worldSizeX, worldSizeY, worldSizeZ];

        for (int x = 0; x < worldSizeX; x++) {
            for (int z = 0; z < worldSizeZ; z++) {
                for (int y = 0; y < worldSizeY; y++) {
                    BlockType type = BlockType.Air;
                    float adjustedElevation = elevationMap[x, z] * Sizes.MapVertical;
                    if (y <= adjustedElevation) {
                        foreach (Strata s in preset.strata) {
                            float height = s.end * Sizes.MapVertical;
                            if (s.end * Sizes.MapVertical >= y) {
                                noise.SetFrequency(1 / s.scale);
                                // Get noise in Range(-1f, 1f)
                                float noiseVal = noise.GetSimplexFractal((Mathf.RoundToInt(horizontalPosition.x) + x), y, (Mathf.RoundToInt(horizontalPosition.y) + z));
                                // Convert to Range(0f, 1f)
                                float e = (noiseVal + 1) / 2;
                                // Raise to strata power level
                                e = Mathf.Pow(e, s.power);
                                // Scale from e Range(0,1) to size of blockTypes array
                                int index = Mathf.FloorToInt(e * s.blockDistribution.Length);
                                // Set to type at that index in array
                                type = s.blockDistribution[index];
                                break;
                            }
                        }
                    }
                    blockMap[x, y, z] = type;
                }
            }
        }

        return blockMap;
    }


    BlockType CalculateBlockFromStrata(FastNoise noise, Strata s, int x, int y, int z) {
        BlockType type = BlockType.Air;
        noise.SetFrequency(1 / s.scale);
        // Get noise in Range(-1f, 1f)
        float noiseVal = noise.GetSimplexFractal((Mathf.RoundToInt(horizontalPosition.x) + x), y, (Mathf.RoundToInt(horizontalPosition.y) + z));
        // Convert to Range(0f, 1f)
        float e = (noiseVal + 1) / 2;
        // Raise to strata power level
        e = Mathf.Pow(e, s.power);
        // Scale from e Range(0,1) to size of blockTypes array
        int index = Mathf.FloorToInt(e * s.blockDistribution.Length);
        // Set to type at that index in array
        type = s.blockDistribution[index];

        return type;
    }


    public BlockType[,,] GenerateBlockMapFromBiomeMap(FastNoise noise, float[,] elevationMap, float[,] moistureMap, int worldSizeY) {
        int worldSizeX = elevationMap.GetLength(0);
        int worldSizeZ = elevationMap.GetLength(1);
        BlockType[,,] blockMap = new BlockType[worldSizeX, worldSizeY, worldSizeZ];

        for (int x = 0; x < worldSizeX; x++) {
            for (int z = 0; z < worldSizeZ; z++) {
                //Convert to coords
                float coordX = moistureMap[x, z];
                float coordZ = elevationMap[x, z];

                for (int y = 0; y < Sizes.MapVertical; y++) {
                    if (y <= elevationMap[x, z] * Sizes.MapVertical + preset.verticalOffset) {
                        float coordY = (y * 1f) / (Sizes.MapVertical * 1f);
                        Color color = GetColorFromCoords(coordX, coordY);
                        Strata s = ColorToStrata(color);
                        blockMap[x, y, z] = CalculateBlockFromStrata(noise, s, x, y, z);
                    } else {
                        blockMap[x, y, z] = BlockType.Air;
                    }
                }
            }
        }

        return blockMap;

    }
    public BlockType Block(int x, int y, int z) {

        if (x >= Sizes.MapHorizontal || x < 0 || y >= Sizes.MapVertical || y < 0 || z >= Sizes.MapHorizontal || z < 0) {
            return BlockType.Air;
        }

        return data[x, y, z];
    }

    private Color GetColorFromCoords(float coordX, float coordZ) {
        if (coordX < 0 || coordX >= 1 || coordZ < 0 || coordZ >= 1) {
            throw new System.Exception($"Invalid coords requested from biomeMap: ({coordX}, {coordZ})");
        }
        return preset.biomeMap.GetPixel(Mathf.RoundToInt(coordX * preset.biomeMap.width), Mathf.RoundToInt(coordZ * preset.biomeMap.height));
    }

    private Strata ColorToStrata(Color color) {
        if (!preset.colorToStrataDict.ContainsKey(color)) {
            throw new System.Exception($"Color {color} not found in colorToStrataDict!");
        }

        return preset.colorToStrataDict[color];
    }
}

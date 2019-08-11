using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;
public class World : MonoBehaviour
{
    public BlockType[,,] data;

    public readonly TerrainModifier terrainModifier;
    public int worldX = 16;
    public int worldY = 16;
    public int worldZ = 16;

    public GameObject chunk = null;
    public GameObject blockItemPrefab = null;
    GameObject chunkParent;
    public Chunk[,,] chunks;
    public int chunkSize = 16;

    [System.Serializable]
    class GenerationalLayer
    {
        public float scale = 1;
        public float height = 1;
        public float power = 1;
        public BlockType tile = BlockType.Air;
    }

    [SerializeField]
    GenerationalLayer[] layers = null;

    public World() : base()
    {
        this.terrainModifier = new TerrainModifier(this);
    }


    void Start()
    {
        GenerateDataMap();
        GenerateChunks();
        UpdateChunks();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            terrainModifier.ReplaceBlockCursor(BlockType.Air);
        }

        if (Input.GetMouseButtonDown(1))
        {
            terrainModifier.AddBlockCursor(BlockType.Grass_Solid);
        }
    }

    void InitializeChunksArray()
    {
        if (chunks != null) DestroyImmediateChunks();
        chunkParent = new GameObject("Chunks");
        chunkParent.transform.SetParent(this.transform);

        chunks = new Chunk[Mathf.FloorToInt(worldX / chunkSize),
                            Mathf.FloorToInt(worldY / chunkSize),
                            Mathf.FloorToInt(worldZ / chunkSize)];
    }

    void GenerateChunks()
    {
        if (chunks == null) InitializeChunksArray();
        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {

                    GameObject newChunk = Instantiate(chunk, new Vector3(x * chunkSize - 0.5f,
                     y * chunkSize + 0.5f, z * chunkSize - 0.5f), new Quaternion(0, 0, 0, 0)) as GameObject;
                    newChunk.transform.SetParent(chunkParent.transform, false);
                    chunks[x, y, z] = newChunk.GetComponent<Chunk>();
                    chunks[x, y, z].worldGO = gameObject;
                    chunks[x, y, z].chunkSize = chunkSize;
                    chunks[x, y, z].chunkX = x * chunkSize;
                    chunks[x, y, z].chunkY = y * chunkSize;
                    chunks[x, y, z].chunkZ = z * chunkSize;
                    UpdateChunk(x, y, z);
                }
            }
        }
    }

    public void UpdateChunks()
    {
        if (chunks == null) GenerateChunks();

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    UpdateChunk(x, y, z);
                }
            }
        }
    }
    void UpdateChunk(int x, int y, int z)
    {
        chunks[x, y, z].update = true;
    }

    public void GenerateDataMap()
    {

        data = new BlockType[worldX, worldY, worldZ];

        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {

                for (int y = 0; y < worldY; y++)
                {
                    uint sum = 0;
                    for (int i = 0; i < layers.Length; i++)
                    {
                        sum += (uint)PerlinNoise(x, y, z, layers[i].scale, layers[i].height, layers[i].power);
                        if (y <= sum)
                        {
                            data[x, y, z] = layers[i].tile;
                            break;
                        }
                    }

                }
            }
        }
    }

    public void DestroyImmediateChunks()
    {
        if (chunks == null) return;

        foreach (Chunk chunk in chunks)
        {
            if (chunk && chunk.gameObject)
            {
                DestroyImmediate(chunk.gameObject);
            }
        }

        chunks = null;
        DestroyImmediate(chunkParent);
    }

    int PerlinNoise(int x, int y, int z, float scale, float height, float power)
    {
        float rValue;
        rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
        rValue *= height;

        if (power != 0)
        {
            rValue = Mathf.Pow(rValue, power);
        }

        return (int)rValue;
    }

    public BlockType Block(int x, int y, int z)
    {

        if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
        {
            return BlockType.Air;
        }

        return data[x, y, z];
    }
}

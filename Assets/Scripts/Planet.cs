using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;
using Block;
using FauxGravity;

public class Planet : MonoBehaviour, IModifiable, IAttractor {
    #region Fields 
    public BlockType[,,] data { get; private set; }
    public Bounds planetBounds { get; private set; }
    public Chunk[,,] chunks { get; private set; }

    public float gravity = -9.81f;
    public float raySpacing = 0.05f;
    public float rayVerticalOffset = 0.1f;

    /// <summary>
    /// Property controlling whether map is visible and active
    /// </summary>
    /// <value></value>
    public bool IsVisible {
        get {
            return _iv;
        }
        set {
            this.gameObject.SetActive(value);
            _iv = value;
        }
    }

    #region Private 
    #endregion

    #region Backing Variables 
    /// <summary>
    /// DO NOT USE. 
    /// <para> Backing store for IsVisible property. 
    /// Use property IsVisible instead </para>
    /// </summary>
    private bool _iv;
    #endregion
    #endregion

    #region IModifiable 
    public Planet planet => this;
    #endregion

    #region IAttractor 
    public Vector3 Attract(Body body) {
        Vector3 distance = body.transform.position - transform.position;
        Vector3 gravityUp = distance.normalized;
        Vector3 targetUp = body.transform.up;

        Vector3 originFL = body.transform.position + (body.transform.up * rayVerticalOffset) + (body.transform.forward * raySpacing) + (-body.transform.right * raySpacing);
        Vector3 originFR = body.transform.position + (body.transform.up * rayVerticalOffset) + (body.transform.forward * raySpacing) + (body.transform.right * raySpacing);
        Vector3 originRL = body.transform.position + (body.transform.up * rayVerticalOffset) + (-body.transform.forward * raySpacing) + (-body.transform.right * raySpacing);
        Vector3 originRR = body.transform.position + (body.transform.up * rayVerticalOffset) + (-body.transform.forward * raySpacing) + (body.transform.right * raySpacing);

        RaycastHit fl;
        RaycastHit fr;
        RaycastHit rl;
        RaycastHit rr;

        Physics.Linecast(originFL, transform.position, out fl, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
        Physics.Linecast(originFR, transform.position, out fr, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
        Physics.Linecast(originRL, transform.position, out rl, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);
        Physics.Linecast(originRR, transform.position, out rr, LayerMask.GetMask("World"), QueryTriggerInteraction.Collide);

        // Debug.Log($"{fl.normal}, {fr.normal},{c.normal}, {rl.normal}, {rr.normal}");

        if (fl.normal == rr.normal
        && fr.normal == rl.normal) {
            // Debug.DrawRay(originFL, fl.normal, Color.yellow, 0.01f);
            // Debug.DrawRay(originFR, fr.normal, Color.yellow, 0.01f);
            // Debug.DrawRay(originRL, rl.normal, Color.yellow, 0.01f);
            // Debug.DrawRay(originRR, rr.normal, Color.yellow, 0.01f);

            targetUp = fr.normal;
        }

        body.AddGravityForce(gravityUp * gravity);
        return targetUp;
    }

    public SphereCollider attractorRange { get; private set; }

    #endregion

    #region MonoBehaviour Methods
    private void Start() {
        if (data == null) {
            SetBlockData(PlanetCrucible.GeneratePlanetData(Settings.DefaultPlanetPreset));
        }
        InitializeAttractorRange();
    }

    #endregion

    /// <summary>
    /// Sets the planet's Block data to the passed array
    /// </summary>
    /// <param name="planet">Block Data to set planet to</param>
    public void SetBlockData(BlockType[,,] planet) {
        var sizeX = planet.GetLength(0);
        var sizeY = planet.GetLength(1);
        var sizeZ = planet.GetLength(2);

        Vector3 sizeVec = new Vector3(sizeX, sizeY, sizeZ);

        this.planetBounds = new Bounds(transform.position, sizeVec);
        this.data = planet;

        GenerateChunks();
    }
    /// <summary>
    /// Generates enough chunks and updates the to the Planet's BlockType data array
    /// </summary>
    private void GenerateChunks() {

        DestroyChildChunks();

        InitializeChunksArray();

        for (int x = 0; x < chunks.GetLength(0); x++) {
            for (int y = 0; y < chunks.GetLength(1); y++) {
                for (int z = 0; z < chunks.GetLength(2); z++) {

                    GameObject newChunk = Instantiate(Settings.ChunkPrefab, new Vector3(x * Sizes.ChunkSize - 0.5f - planetBounds.extents.x,
                     y * Sizes.ChunkSize + 0.5f - planetBounds.extents.y, z * Sizes.ChunkSize - 0.5f - planetBounds.extents.z), new Quaternion(0, 0, 0, 0)) as GameObject;
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

    private void InitializeAttractorRange() {
        attractorRange = new GameObject().AddComponent<SphereCollider>();
        attractorRange.name = "Attractor";
        attractorRange.transform.SetParent(this.transform, false);
        attractorRange.radius = data.GetLength(0);
        attractorRange.isTrigger = true;
    }
    /// <summary>
    /// Initializes the 3D array of Chunks, with size necessary for 
    /// the Planets BlockType data array
    /// </summary>
    private void InitializeChunksArray() {

        int chunkAmtX = CalculateChunksNeededForDirection(planetBounds.size.x);
        int chunkAmtY = CalculateChunksNeededForDirection(planetBounds.size.y);
        int chunkAmtZ = CalculateChunksNeededForDirection(planetBounds.size.z);

        chunks = new Chunk[chunkAmtX, chunkAmtY, chunkAmtZ];
    }
    /// <summary>
    /// Determines how many chunks will be needed to render the amount of
    /// blocks in given directions 
    /// </summary>
    /// <param name="directionLength"></param>
    /// <returns>How many chunks will be needed to render blocks in given direction</returns>
    private int CalculateChunksNeededForDirection(float directionLength) {
        int chunkAmt = Mathf.CeilToInt((directionLength * 1f) / (Sizes.ChunkSize * 1f));
        if (chunkAmt == 0) chunkAmt = 1;
        return chunkAmt;
    }
    /// <summary>
    /// Get all children Chunk components and Destroys them
    /// </summary>
    void DestroyChildChunks() {
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
    /// <param name="x">Coordinate X on Planet, determining which chunk should update</param>
    /// <param name="y">Coordinate Y on Planet, determining which chunk should update</param>
    /// <param name="z">Coordinate Z on Planet, determining which chunk should update</param>
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
                // world.CallMapChunkUpdate(this, Sizes.MapHorizontal - 1, chunkY, chunkZ);
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
                //.CallMapChunkUpdate(this, Sizes.MapHorizontal + 1, chunkY, chunkZ);
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
                // world.CallMapChunkUpdate(this, chunkX, chunkY, Sizes.MapHorizontal - 1);
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
                // world.CallMapChunkUpdate(this, Sizes.MapHorizontal + 1, chunkY, Sizes.MapHorizontal + 1);
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
    /// <summary>
    /// Return the BlockType at the given local planet coordinate
    /// </summary>
    /// <param name="x">Local planet coordinate X</param>
    /// <param name="y">Local planet coordinate Y</param>
    /// <param name="z">Local planet coordinate Z</param>
    /// <returns>Type of blcok at given local planet coordinate</returns>
    public BlockType BlockAt(int x, int y, int z) {
        if (x >= planetBounds.size.x || x < 0 || y >= planetBounds.size.y || y < 0 || z >= planetBounds.size.z || z < 0) {
            return BlockType.Air;
        }

        return data[x, y, z];
    }
    /// <summary>
    /// Returns the distance vector between position and planet's center
    /// </summary>
    /// <param name="position">Position vector of other object</param>
    /// <returns>Distance vector between passed position and planet center</returns>
    public float DistanceFromCenter(Vector3 position) {
        return Vector3.Distance(planetBounds.center, position);
    }
}
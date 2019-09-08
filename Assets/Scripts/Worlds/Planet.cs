using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks;
using FauxGravity;
using System;
using UnityEditor;

namespace Worlds {
    public class Planet : MonoBehaviour, IModifiable, IAttractor {
        #region Fields 
        public BlockData[,,] data { get; private set; }
        public Bounds planetBounds { get; private set; }
        public Chunk[,,] chunks { get; private set; }

        public float gravityScale = -9.81f;
        public float raySpacing = 0.05f;
        public float rayVerticalOffset = 0.1f;

        public FloatReference maxOrientationDistance;

        public static int ChunkSize = 16;

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
        [SerializeField] public Preset preset = null;

        private GameObject chunkParent = null;


        /// <summary>
        /// The planet data with strata initialized,
        /// before elevation maps are applied.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private BlockData[,,] planetStrataData;
        [SerializeField]
        [HideInInspector]
        private float[][,] elevationMapData;
        #endregion

        #region Backing Variables 
        /// <summary> 
        /// Backing store for IsVisible property. 
        /// Use property IsVisible instead
        /// </summary>
        private bool _iv;
        #endregion
        #endregion

        #region IModifiable 
        public Planet planet => this;
        #endregion

        #region IAttractor 
        public Vector3 Attract(Body body) {
            Vector3 distance = transform.position - body.transform.position;

            Vector3 targetDir = distance.normalized;

            Vector3 originFL = body.bodyCollider.bounds.center + (body.transform.forward * raySpacing) + (-body.transform.right * raySpacing);
            Vector3 originFR = body.bodyCollider.bounds.center + (body.transform.forward * raySpacing) + (body.transform.right * raySpacing);
            Vector3 originRL = body.bodyCollider.bounds.center + (-body.transform.forward * raySpacing) + (-body.transform.right * raySpacing);
            Vector3 originRR = body.bodyCollider.bounds.center + (-body.transform.forward * raySpacing) + (body.transform.right * raySpacing);
            Vector3 originC = body.bodyCollider.bounds.center - (body.transform.up * raySpacing);

            RaycastHit fl;
            RaycastHit fr;
            RaycastHit rl;
            RaycastHit rr;
            RaycastHit c;

            Physics.Linecast(originFL, transform.position, out fl);
            Physics.Linecast(originFR, transform.position, out fr);
            Physics.Linecast(originRL, transform.position, out rl);
            Physics.Linecast(originRR, transform.position, out rr);
            Physics.Linecast(originC, transform.position, out c);

            if (fr.transform != null
                && fr.distance < maxOrientationDistance
                && fl.normal == rr.normal
                && fr.normal == rl.normal
                && fr.normal == c.normal
                && fl.normal == c.normal) {

                Debug.DrawLine(originFL, body.transform.position, Color.yellow, 0.01f);
                Debug.DrawLine(originFR, body.transform.position, Color.yellow, 0.01f);
                Debug.DrawLine(originRL, body.transform.position, Color.yellow, 0.01f);
                Debug.DrawLine(originRR, body.transform.position, Color.yellow, 0.01f);
                targetDir = -fr.normal;
            }

            return targetDir * gravityScale;
        }

        public SphereCollider attractorRange { get; private set; }

        #endregion

        #region MonoBehaviour Methods

        private void Start() {
            Initialize();
        }

        #endregion

        public void Initialize(Preset newPreset) {
            this.preset = newPreset;
            Initialize();
        }



        public void Initialize() {
            if (this.preset == null) throw new System.Exception("Planet initialized without preset");

            planetStrataData = Crucible.GeneratePlanetStrataData(preset);
            elevationMapData = Crucible.GenerateElevationMaps(preset);
            data = Crucible.ApplyElevations(preset, planetStrataData, elevationMapData);

            SetBounds();
            InitializeAttractorRange();
            GenerateChunks();
        }

        private void SetBounds() {

            var sizeX = data.GetLength(0);
            var sizeY = data.GetLength(1);
            var sizeZ = data.GetLength(2);

            Vector3 sizeVec = new Vector3(sizeX, sizeY, sizeZ);

            this.planetBounds = new Bounds(transform.position, sizeVec);
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

                        GameObject newChunk = new GameObject($"Chunk_{x}_{y}_{z}");
                        newChunk.transform.position = new Vector3(x * ChunkSize - 0.5f - planetBounds.extents.x,
                         y * ChunkSize + 0.5f - planetBounds.extents.y, z * ChunkSize - 0.5f - planetBounds.extents.z);
                        newChunk.transform.SetParent(this.chunkParent.transform, false);
                        chunks[x, y, z] = newChunk.AddComponent<Chunk>();
                        chunks[x, y, z].Initialize(this, x * ChunkSize, y * ChunkSize, z * ChunkSize, preset.planetMaterial);

                    }
                }
            }
        }

        private void InitializeAttractorRange() {
            if (attractorRange != null) {
                if (Application.isEditor) {
                    DestroyImmediate(attractorRange.gameObject);
                } else {
                    Destroy(attractorRange.gameObject);
                }
            }
            attractorRange = new GameObject("Attractor").AddComponent<SphereCollider>();
            attractorRange.transform.SetParent(this.transform, false);
            attractorRange.radius = preset.totalSize * 2;
            attractorRange.gameObject.layer = LayerMask.NameToLayer("World");
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
            int chunkAmt = Mathf.CeilToInt((directionLength * 1f) / (ChunkSize * 1f));
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
            if (this.chunkParent != null) {
                if (Application.isEditor) {

                    DestroyImmediate(this.chunkParent);
                } else {
                    Destroy(this.chunkParent);
                }
            }

            this.chunkParent = new GameObject("Chunk Parent");
            this.chunkParent.transform.SetParent(this.transform, false);
        }
        /// <summary>
        /// Updates the chunk containing block at specified coordinates
        /// </summary>
        /// <param name="x">Coordinate X on Planet, determining which chunk should update</param>
        /// <param name="y">Coordinate Y on Planet, determining which chunk should update</param>
        /// <param name="z">Coordinate Z on Planet, determining which chunk should update</param>
        public void UpdateChunkAt(int x, int y, int z) {
            int chunkX = Mathf.FloorToInt(x / ChunkSize);
            int chunkY = Mathf.FloorToInt(y / ChunkSize);
            int chunkZ = Mathf.FloorToInt(z / ChunkSize);

            chunks[chunkX, chunkY, chunkZ].needsUpdate = true;


            // If block update occurred in first chunk of map ..
            if (x - (ChunkSize * chunkX) == 0) {
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
            if (x - (ChunkSize * chunkX) == 15) {
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
            if (z - (ChunkSize * chunkZ) == 0) {
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
            if (z - (ChunkSize * chunkZ) == 15) {
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
            if (y - (ChunkSize * chunkY) == 0 && chunkY != 0) {
                chunks[chunkX, chunkY - 1, chunkZ].needsUpdate = true;
            }

            if (y - (ChunkSize * chunkY) == 15 && chunkY != chunks.GetLength(1) - 1) {
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
        public BlockData BlockAt(int x, int y, int z) {
            if (x >= planetBounds.size.x || x < 0 || y >= planetBounds.size.y || y < 0 || z >= planetBounds.size.z || z < 0) {
                return null;
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
}
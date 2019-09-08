using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks;
using System;
using UnityEditor;

namespace Worlds {
    public class NewPlanet : MonoBehaviour {
        #region Fields 

        public Material planetMaterial;
        public Bounds planetBounds { get; private set; }

        public static int ChunkSize = 16;


        public BlockData[,,] data { get; private set; }
        [SerializeField, HideInInspector]
        MeshFilter[,,] chunkMeshFilters;
        NewChunk[,,] chunks;

        [SerializeField, HideInInspector]
        private BlockData[,,] planetStrataData;
        [SerializeField, HideInInspector]
        private float[][,] elevationMapData;

        [SerializeField] private Preset preset = null;




        #endregion

        #region MonoBehaviour Methods 
        void OnValidate() {

            if (preset != null) {
                EditorApplication.delayCall += () => {
                    Initialize();
                };
            }
        }
        #endregion

        #region NewPlanet Methods

        void Initialize() {
            if (this.planetStrataData == null) {
                this.planetStrataData = Crucible.GeneratePlanetStrataData(preset);
            }
            if (this.elevationMapData == null) {
                this.elevationMapData = Crucible.GenerateElevationMaps(preset);
            }
            if (this.data == null) {
                this.data = planetStrataData;//Crucible.ApplyElevations(preset, planetStrataData, elevationMapData);
            }

            SetBounds();

            GenerateChunks();

            GenerateMesh();
        }

        void GenerateMesh() {
            foreach (NewChunk chunk in chunks) {
                chunk.GenerateMesh();
            }
        }

        void GenerateChunks() {
            InitializeChunks();
            for (int x = 0; x < chunks.GetLength(0); x++) {
                for (int y = 0; y < chunks.GetLength(1); y++) {
                    for (int z = 0; z < chunks.GetLength(2); z++) {
                        if (chunkMeshFilters[x, y, z] == null) {
                            GameObject meshObj = new GameObject($"Chunk_Mesh_{x}_{y}_{z}");
                            meshObj.transform.SetParent(this.transform, false);
                            meshObj.AddComponent<MeshRenderer>().sharedMaterial = planetMaterial;
                            chunkMeshFilters[x, y, z] = meshObj.AddComponent<MeshFilter>();
                            chunkMeshFilters[x, y, z].sharedMesh = new Mesh();
                        }

                        chunkMeshFilters[x, y, z].gameObject.transform.position = new Vector3(
                                transform.position.x + (x * ChunkSize - 0.5f - planetBounds.extents.x),
                                transform.position.y + (y * ChunkSize + 0.5f - planetBounds.extents.y),
                                transform.position.z + (z * ChunkSize - 0.5f - planetBounds.extents.z)
                            );


                        chunks[x, y, z] = new NewChunk(
                            this,
                            chunkMeshFilters[x, y, z].sharedMesh,
                            ChunkSize,
                            x * ChunkSize,
                            y * ChunkSize,
                            z * ChunkSize
                        );
                    }
                }
            }
        }

        private void SetBounds() {

            var sizeX = data.GetLength(0);
            var sizeY = data.GetLength(1);
            var sizeZ = data.GetLength(2);

            Vector3 sizeVec = new Vector3(sizeX, sizeY, sizeZ);

            this.planetBounds = new Bounds(transform.position, sizeVec);
        }

        private void InitializeChunks() {

            int chunkAmtX = CalculateChunksNeededForDirection(planetBounds.size.x);
            int chunkAmtY = CalculateChunksNeededForDirection(planetBounds.size.y);
            int chunkAmtZ = CalculateChunksNeededForDirection(planetBounds.size.z);

            chunks = new NewChunk[chunkAmtX, chunkAmtY, chunkAmtZ];
            chunkMeshFilters = new MeshFilter[chunkAmtX, chunkAmtY, chunkAmtZ];
        }

        private int CalculateChunksNeededForDirection(float directionLength) {
            int chunkAmt = Mathf.CeilToInt((directionLength * 1f) / (ChunkSize * 1f));
            if (chunkAmt == 0) chunkAmt = 1;
            return chunkAmt;
        }

        public BlockData BlockAt(int x, int y, int z) {
            if (x >= planetBounds.size.x || x < 0 || y >= planetBounds.size.y || y < 0 || z >= planetBounds.size.z || z < 0) {
                return null;
            }

            return data[x, y, z];
        }

        #endregion
    }
}
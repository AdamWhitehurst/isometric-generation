using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks;
namespace Worlds {

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour {
        public int mapX;
        public int mapY;
        public int mapZ;
        public bool needsUpdate;
        private Planet _parent;
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();

        private List<Vector3> _normals = new List<Vector3>();

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        private Material _planetMaterial;

        private int faceCount;


        void LateUpdate() {
            CheckForUpdate();
        }

        public void CheckForUpdate() {
            if (needsUpdate) {
                GenerateMesh();
                needsUpdate = false;
            }
        }

        public void Initialize(Planet parent, int mapX, int mapY, int mapZ, Material planetMaterial) {
            this._parent = parent;
            this.mapX = mapX;
            this.mapY = mapY;
            this.mapZ = mapZ;
            this._planetMaterial = planetMaterial;
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();

            ClearMeshes();
            GenerateMesh();
        }
        public void GenerateMesh() {

            ResetMeshData();

            for (int x = 0; x < Planet.ChunkSize; x++) {
                for (int y = 0; y < Planet.ChunkSize; y++) {
                    for (int z = 0; z < Planet.ChunkSize; z++) {
                        //This code will run for every block in the chunk
                        BlockData currentBlock = Block(x, y, z);
                        if (currentBlock != null) {
                            //If the block is solid

                            if (Block(x, y + 1, z) == null) {
                                CubeTop(x, y, z, currentBlock);
                            }

                            if (Block(x, y - 1, z) == null) {
                                CubeBot(x, y, z, currentBlock);
                            }

                            if (Block(x + 1, y, z) == null) {
                                CubeEast(x, y, z, currentBlock);
                            }

                            if (Block(x - 1, y, z) == null) {
                                CubeWest(x, y, z, currentBlock);
                            }

                            if (Block(x, y, z + 1) == null) {
                                CubeNorth(x, y, z, currentBlock);
                            }

                            if (Block(x, y, z - 1) == null) {
                                CubeSouth(x, y, z, currentBlock);
                            }

                        }

                    }
                }
            }

            ApplyMesh();
        }

        void CubeTop(int x, int y, int z, BlockData block) {
            _vertices.Add(new Vector3(x + 0, y + 0, z + 0));
            _vertices.Add(new Vector3(x + 0, y + 0, z + 1));
            _vertices.Add(new Vector3(x + 1, y + 0, z + 1));
            _vertices.Add(new Vector3(x + 1, y + 0, z + 0));

            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));
            _normals.Add(new Vector3(0, 1, 0));

            ApplyTexture(block.Top);

        }

        void CubeNorth(int x, int y, int z, BlockData block) {
            _vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            _vertices.Add(new Vector3(x + 1, y + 0, z + 1));
            _vertices.Add(new Vector3(x + 0, y + 0, z + 1));
            _vertices.Add(new Vector3(x + 0, y - 1, z + 1));

            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));
            _normals.Add(new Vector3(0, 0, 1));

            ApplyTexture(block.North);
        }

        void CubeEast(int x, int y, int z, BlockData block) {
            _vertices.Add(new Vector3(x + 1, y - 1, z + 0));
            _vertices.Add(new Vector3(x + 1, y + 0, z + 0));
            _vertices.Add(new Vector3(x + 1, y + 0, z + 1));
            _vertices.Add(new Vector3(x + 1, y - 1, z + 1));

            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));
            _normals.Add(new Vector3(1, 0, 0));

            ApplyTexture(block.East);
        }

        void CubeSouth(int x, int y, int z, BlockData block) {
            _vertices.Add(new Vector3(x + 0, y - 1, z + 0));
            _vertices.Add(new Vector3(x + 0, y + 0, z + 0));
            _vertices.Add(new Vector3(x + 1, y + 0, z + 0));
            _vertices.Add(new Vector3(x + 1, y - 1, z + 0));

            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));
            _normals.Add(new Vector3(0, 0, -1));

            ApplyTexture(block.South);
        }

        void CubeWest(int x, int y, int z, BlockData block) {
            _vertices.Add(new Vector3(x + 0, y - 1, z + 1));
            _vertices.Add(new Vector3(x + 0, y + 0, z + 1));
            _vertices.Add(new Vector3(x + 0, y + 0, z + 0));
            _vertices.Add(new Vector3(x + 0, y - 1, z + 0));

            _normals.Add(new Vector3(-1, 0, 0));
            _normals.Add(new Vector3(-1, 0, 0));
            _normals.Add(new Vector3(-1, 0, 0));
            _normals.Add(new Vector3(-1, 0, 0));

            ApplyTexture(block.West);
        }

        void CubeBot(int x, int y, int z, BlockData block) {
            _vertices.Add(new Vector3(x + 1, y - 1, z + 0));
            _vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            _vertices.Add(new Vector3(x + 0, y - 1, z + 1));
            _vertices.Add(new Vector3(x + 0, y - 1, z + 0));

            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));
            _normals.Add(new Vector3(0, -1, 0));

            ApplyTexture(block.Bot);
        }

        void ApplyTexture(Sprite blockSprite) {
            _triangles.Add(faceCount * 4 + 0);
            _triangles.Add(faceCount * 4 + 2);
            _triangles.Add(faceCount * 4 + 3);

            _triangles.Add(faceCount * 4 + 0);
            _triangles.Add(faceCount * 4 + 1);
            _triangles.Add(faceCount * 4 + 2);
            _uvs.AddRange(blockSprite.uv);

            faceCount++;
        }

        void ResetMeshData() {
            _vertices.Clear();
            _uvs.Clear();
            _triangles.Clear();
            _normals.Clear();

            faceCount = 0;
        }

        public void ClearMeshes() {
            _meshCollider.sharedMesh = null;
            _meshFilter.sharedMesh = null;
        }

        void ApplyMesh() {
            ClearMeshes();

            Mesh mesh = new Mesh();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            mesh.subMeshCount = 1;
            mesh.SetVertices(_vertices.ToArray());
            mesh.SetTriangles(_triangles.ToArray(), 0);
            mesh.SetUVs(0, _uvs.ToArray());
            mesh.SetNormals(_normals.ToArray());

            _meshRenderer.sharedMaterial = _planetMaterial;

            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;

        }

        BlockData Block(int x, int y, int z) {
            return _parent.BlockAt(x + mapX, y + mapY, z + mapZ);
        }
    }
}
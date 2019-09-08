using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks;

namespace Worlds {
    public class NewChunk {
        #region Fields
        Mesh mesh;
        int extents;

        public int mapX, mapY, mapZ;
        private NewPlanet planet;
        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();

        private List<Vector3> normals = new List<Vector3>();

        private int faceCount;

        #endregion

        #region Constructors 

        public NewChunk(NewPlanet planet, Mesh mesh, int extents, int mapX, int mapY, int mapZ) {
            this.planet = planet;
            this.mesh = mesh;
            this.extents = extents;
            this.mapX = mapX;
            this.mapY = mapY;
            this.mapZ = mapZ;

            this.mesh.Clear();
        }

        #endregion

        #region NewChunk Methods

        public void GenerateMesh() {
            ResetMeshData();

            for (int x = 0; x < extents; x++) {
                for (int y = 0; y < extents; y++) {
                    for (int z = 0; z < extents; z++) {
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

        void ResetMeshData() {
            vertices.Clear();
            uvs.Clear();
            triangles.Clear();
            normals.Clear();

            faceCount = 0;
        }

        void ApplyMesh() {
            mesh.subMeshCount = 1;
            mesh.SetVertices(vertices.ToArray());
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.SetUVs(0, uvs.ToArray());
            mesh.SetNormals(normals.ToArray());
        }

        void CubeTop(int x, int y, int z, BlockData block) {
            vertices.Add(new Vector3(x + 0, y + 0, z + 0));
            vertices.Add(new Vector3(x + 0, y + 0, z + 1));
            vertices.Add(new Vector3(x + 1, y + 0, z + 1));
            vertices.Add(new Vector3(x + 1, y + 0, z + 0));

            normals.Add(new Vector3(0, 1, 0));
            normals.Add(new Vector3(0, 1, 0));
            normals.Add(new Vector3(0, 1, 0));
            normals.Add(new Vector3(0, 1, 0));

            ApplyTexture(block.Top);

        }

        void CubeNorth(int x, int y, int z, BlockData block) {
            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            vertices.Add(new Vector3(x + 1, y + 0, z + 1));
            vertices.Add(new Vector3(x + 0, y + 0, z + 1));
            vertices.Add(new Vector3(x + 0, y - 1, z + 1));

            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));
            normals.Add(new Vector3(0, 0, 1));

            ApplyTexture(block.North);
        }

        void CubeEast(int x, int y, int z, BlockData block) {
            vertices.Add(new Vector3(x + 1, y - 1, z + 0));
            vertices.Add(new Vector3(x + 1, y + 0, z + 0));
            vertices.Add(new Vector3(x + 1, y + 0, z + 1));
            vertices.Add(new Vector3(x + 1, y - 1, z + 1));

            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));
            normals.Add(new Vector3(1, 0, 0));

            ApplyTexture(block.East);
        }

        void CubeSouth(int x, int y, int z, BlockData block) {
            vertices.Add(new Vector3(x + 0, y - 1, z + 0));
            vertices.Add(new Vector3(x + 0, y + 0, z + 0));
            vertices.Add(new Vector3(x + 1, y + 0, z + 0));
            vertices.Add(new Vector3(x + 1, y - 1, z + 0));

            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));
            normals.Add(new Vector3(0, 0, -1));

            ApplyTexture(block.South);
        }

        void CubeWest(int x, int y, int z, BlockData block) {
            vertices.Add(new Vector3(x + 0, y - 1, z + 1));
            vertices.Add(new Vector3(x + 0, y + 0, z + 1));
            vertices.Add(new Vector3(x + 0, y + 0, z + 0));
            vertices.Add(new Vector3(x + 0, y - 1, z + 0));

            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));
            normals.Add(new Vector3(-1, 0, 0));

            ApplyTexture(block.West);
        }

        void CubeBot(int x, int y, int z, BlockData block) {
            vertices.Add(new Vector3(x + 1, y - 1, z + 0));
            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
            vertices.Add(new Vector3(x + 0, y - 1, z + 1));
            vertices.Add(new Vector3(x + 0, y - 1, z + 0));

            normals.Add(new Vector3(0, -1, 0));
            normals.Add(new Vector3(0, -1, 0));
            normals.Add(new Vector3(0, -1, 0));
            normals.Add(new Vector3(0, -1, 0));

            ApplyTexture(block.Bot);
        }

        void ApplyTexture(Sprite blockSprite) {
            triangles.Add(faceCount * 4 + 0);
            triangles.Add(faceCount * 4 + 2);
            triangles.Add(faceCount * 4 + 3);

            triangles.Add(faceCount * 4 + 0);
            triangles.Add(faceCount * 4 + 1);
            triangles.Add(faceCount * 4 + 2);
            uvs.AddRange(blockSprite.uv);

            faceCount++;
        }

        BlockData Block(int x, int y, int z) {
            return planet.BlockAt(x + mapX, y + mapY, z + mapZ);
        }

        #endregion
    }
}
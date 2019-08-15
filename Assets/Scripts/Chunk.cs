using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;
using Generation;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class Chunk : MonoBehaviour {

    public GameObject parentMap;
    public int mapX;
    public int mapY;
    public int mapZ;
    public bool needsUpdate;
    private WorldMap world;
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private MeshFilter filter;
    private MeshRenderer rndrr;
    private MeshCollider col;

    private int faceCount;

    void Start() {
        filter = GetComponent<MeshFilter>();
        rndrr = GetComponent<MeshRenderer>();
        col = GetComponent<MeshCollider>();
    }


    void LateUpdate() {
        CheckForUpdate();
    }

    void OnGUI() {
        CheckForUpdate();
    }

    void CheckForUpdate() {
        if (needsUpdate) {
            GenerateMesh();
            needsUpdate = false;
        }
    }
    public void GenerateMesh() {

        ResetMeshData();

        for (int x = 0; x < Sizes.ChunkSize; x++) {
            for (int y = 0; y < Sizes.ChunkSize; y++) {
                for (int z = 0; z < Sizes.ChunkSize; z++) {
                    //This code will run for every block in the chunk

                    if (Block(x, y, z) != 0) {
                        //If the block is solid

                        if (Block(x, y + 1, z) == 0) {
                            //Block above is air
                            CubeTop(x, y, z, Block(x, y, z));
                        }

                        if (Block(x, y - 1, z) == 0) {
                            //Block below is air
                            CubeBot(x, y, z, Block(x, y, z));

                        }

                        if (Block(x + 1, y, z) == 0) {
                            //Block east is air
                            CubeEast(x, y, z, Block(x, y, z));

                        }

                        if (Block(x - 1, y, z) == 0) {
                            //Block west is air
                            CubeWest(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z + 1) == 0) {
                            //Block north is air
                            CubeNorth(x, y, z, Block(x, y, z));

                        }

                        if (Block(x, y, z - 1) == 0) {
                            //Block south is air
                            CubeSouth(x, y, z, Block(x, y, z));

                        }

                    }

                }
            }
        }

        ApplyMesh();
    }

    void CubeTop(int x, int y, int z, BlockType tile) {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        ApplyTexture(tile, global::Block.Face.Top);

    }

    void CubeNorth(int x, int y, int z, BlockType tile) {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        ApplyTexture(tile, global::Block.Face.North);
    }

    void CubeEast(int x, int y, int z, BlockType tile) {
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        ApplyTexture(tile, global::Block.Face.South);
    }

    void CubeSouth(int x, int y, int z, BlockType tile) {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        ApplyTexture(tile, global::Block.Face.East);
    }

    void CubeWest(int x, int y, int z, BlockType tile) {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        ApplyTexture(tile, global::Block.Face.West);
    }

    void CubeBot(int x, int y, int z, BlockType tile) {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        ApplyTexture(tile, global::Block.Face.Bot);
    }

    void ApplyTexture(BlockType tile, Block.Face face) {
        newTriangles.Add(faceCount * 4 + 0); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4
        newTriangles.Add(faceCount * 4 + 0); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newUV.AddRange(Loader.TileUVs(tile, face));

        faceCount++;
    }

    void ResetMeshData() {
        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }

    public void ClearMeshes() {
        col.sharedMesh = null;
        filter.sharedMesh = null;
    }

    void ApplyMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        filter.sharedMesh = mesh;
        col.sharedMesh = mesh;

    }

    BlockType Block(int x, int y, int z) {
        if (world == null) world = parentMap.GetComponent<WorldMap>();
        return world.Block(x + mapX, y + mapY, z + mapZ);
    }
}

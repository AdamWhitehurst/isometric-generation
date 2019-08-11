using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;
using Item;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class BlockItem : MonoBehaviour, IInventoryItem
{
    #region Fields 
    public ItemData data { get; private set; }

    private Transform target { get; set; }
    private BlockType tile;
    private MeshFilter filter;

    public BlockType Tile { get => tile; private set => tile = value; }

    #endregion

    #region MonoBehaviour Methods
    void Awake()
    {
        data = ScriptableObject.CreateInstance<ItemData>();

        data.name = "UNSET BLOCK";
        data.sizeInBulk = 1;
        data.spriteId = 0;
        data.type = ItemType.Block;
        this.name = "UNSET BLOCK";
        this.Tile = BlockType.Air;
    }

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        SetVoxel();
    }

    #endregion

    #region IInventoryItem Methods

    public void AddToInventory(InventoryController invRef)
    {
        if (invRef.AddItem(data))
        {
            Destroy(this.gameObject);
        }
    }

    public Sprite GetSprite()
    {
        return Loader.TileSprite((Block.BlockType)data.spriteId, Face.North);
    }

    #endregion

    #region BlockItem Methods
    public void SetBlock(BlockType tile)
    {
        string newName = $"{tile.ToString()} Block";
        this.data.name = newName;
        this.data.spriteId = (int)tile;
        this.name = newName;
        this.Tile = (BlockType)tile;
    }
    void SetVoxel()
    {
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            uvs.AddRange(Loader.TileUVs((BlockType)data.spriteId, Face.North));
        }
        filter.mesh.uv = uvs.ToArray();
    }

    #endregion
}

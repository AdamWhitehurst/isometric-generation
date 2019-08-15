using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Block;
public static class Loader {
    private static List<Sprite> _tsa;
    private static List<Sprite> tileSpriteAtlas {
        get {
            if (_tsa == null) {
                LoadTileSprites();
            }
            return _tsa;
        }
        set {
            _tsa = value;
        }
    }
    private static Dictionary<BlockType, BlockData> _bdd;
    private static Dictionary<BlockType, BlockData> blockDataDict {
        get {
            if (_bdd == null) {
                LoadBlockData();
            }
            return _bdd;
        }
        set {
            _bdd = value;
        }
    }

    private static Texture2D _bcm;

    private static Texture2D biomeColorMap {
        get {
            if (_bcm == null) {
                LoadBlockDataBiomeColorMap();
            }
            return _bcm;
        }
        set {
            _bcm = value;
        }

    }
    static void LoadTileSprites() {
        tileSpriteAtlas = new List<Sprite>();
        tileSpriteAtlas.AddRange(Resources.LoadAll<Sprite>("TileNew"));
    }

    static void LoadItemData() {

    }

    static void LoadBlockDataBiomeColorMap() {
        _bcm = Resources.Load<Texture2D>("biome");
        Debug.Log("Loaded");
    }

    static void LoadBlockData() {
        BlockData[] blocks = Resources.LoadAll<BlockData>("Data/BlockData");
        blockDataDict = new Dictionary<BlockType, BlockData>();

        foreach (BlockData block in blocks) {
            if (blockDataDict.ContainsKey(block.type)) {
                throw new Exception($"Duplicate BlockData type: {block.type} found on {block.UID} ");
            } else
                blockDataDict.Add(block.type, block);
        }

    }

    public static Sprite BasicSprite(int spriteId) {
        Debug.Log("Todo: Implement Basic Sprite loader");
        return TileSprite((BlockType)spriteId, Face.North);
    }

    public static Sprite TileSprite(BlockType tile, Face face) {
        Sprite sprite;
        if (blockDataDict.ContainsKey(tile)) {
            sprite = tileSpriteAtlas[blockDataDict[tile][face]];
        } else {
            Debug.Log($"Invalid Tile Sprite Request: {tile}");
            sprite = tileSpriteAtlas[blockDataDict[0][face]];
        }
        return sprite;
    }

    public static Texture TileTexture(BlockType tile, Face face) {
        if (tileSpriteAtlas == null) LoadTileSprites();
        Texture tex;
        if (blockDataDict.ContainsKey(tile)) {
            tex = tileSpriteAtlas[blockDataDict[tile][face]].texture;
        } else {
            Debug.Log($"Invalid Tile Texture Request: {tile}");
            tex = tileSpriteAtlas[blockDataDict[0][face]].texture;
        }
        return tex;
    }

    public static Vector2[] TileUVs(BlockType tile, Face face) {
        if (tileSpriteAtlas == null) LoadTileSprites();
        if (tile == BlockType.Air) {
            return new Vector2[] {
                new Vector2(0,0),
                new Vector2(0,0),
                new Vector2(0,0),
                new Vector2(0,0),
            };
        }
        Vector2[] uvs;
        if (blockDataDict.ContainsKey(tile)) {
            uvs = tileSpriteAtlas[blockDataDict[tile][face]].uv;
        } else {
            Debug.Log($"Invalid Tile UV Request: {tile}");
            uvs = tileSpriteAtlas[blockDataDict[0][face]].uv;
        }

        Vector2[] sortedUVs = new Vector2[] { uvs[2], uvs[0], uvs[1], uvs[3] };
        return sortedUVs;
    }
}
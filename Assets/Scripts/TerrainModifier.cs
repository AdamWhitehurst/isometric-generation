using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;
using Generation;

public static class TerrainModifier {
    public static readonly int maxRayDistance = 10000;

    /// <summary>
    /// Replaces the block directly in front of the player
    /// </summary>
    /// <param name="range"></param>
    /// <param name="block"></param>
    public static void ReplaceBlockCenter(Planet planet, float range, BlockType block) {
        // Cast ray from Camera position
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("Items"))) {
            // And its within the range limit
            if (hit.distance < range) {
                // Replace the block
                ReplaceBlockAt(planet, hit, block);
            }
        }
    }


    /// <summary>
    /// Adds the block specified directly in front of the player
    /// </summary>
    /// <param name="range"></param>
    /// <param name="block"></param>
    public static void AddBlockCenter(Planet planet, float range, BlockType block) {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("Items"))) {
            // And its within the range limit
            if (hit.distance < range) {
                // Add the block
                AddBlockAt(planet, hit, block);
            }
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance), Color.green, 2);
        }
    }


    /// <summary>
    /// Replaces the block specified at current mouse cursor position
    /// </summary>
    /// <param name="block"></param>
    public static void ReplaceBlockCursor(Planet planet, BlockType block) {
        // Cast ray from mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, ~LayerMask.NameToLayer("World"))) {
            // Replace block and draw debug line
            ReplaceBlockAt(planet, hit, block);
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
                            Color.green, 2);

        }

    }

    /// <summary>
    /// Adds the block specified at current mouse cursor position
    /// </summary>
    /// <param name="block"></param>
    public static void AddBlockCursor(Planet planet, BlockType block) {
        // Cast ray from mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, ~LayerMask.NameToLayer("World"))) {
            // Add block and draw debug line
            AddBlockAt(planet, hit, block);
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
                            Color.green, 2);
        }
    }


    /// <summary>
    /// Removes a block at specified hit point,
    /// you can raycast against the terrain and call this with the hit.point
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="block"></param>
    public static void ReplaceBlockAt(Planet planet, RaycastHit hit, BlockType block) {
        // Get position of hit
        Vector3 position = hit.point + (hit.normal * -0.5f);
        // Move position to center of hit block
        BlockType oldTile = planet.data[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z)];
        // Set that hit block to new block
        SetBlockAt(planet, position, block);
        SpawnBlockAt(planet, position, oldTile);
    }


    /// <summary>
    /// Adds the specified block at specified hit point,
    /// you can raycast against the terrain and call this with the hit.point
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="block"></param>
    public static void AddBlockAt(Planet planet, RaycastHit hit, BlockType block) {
        // Get position of hit
        Vector3 position = hit.point;
        // Move position into center of space where block should go
        position += (hit.normal * 0.5f);
        SetBlockAt(planet, position, block);

    }


    /// <summary>
    /// Sets the specified block at specified coordinates
    /// </summary>
    /// <param name="position"></param>
    /// <param name="block"></param>
    public static void SetBlockAt(Planet planet, Vector3 position, BlockType block) {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        SetBlockAt(planet, x, y, z, block);
    }

    /// <summary>
    /// Adds the specified block at specified coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="block"></param>
    public static void SetBlockAt(Planet planet, int x, int y, int z, BlockType block) {
        //print($"SetBlockAt: {x}, {y}, {z}");

        planet.data[x, y, z] = block;
        planet.UpdateChunkAt(x, y, z);
    }

    public static void SpawnBlockAt(Planet planet, Vector3 position, BlockType tile) {
        GameObject newBlockItem = GameObject.Instantiate(Settings.BlockItemPrefab, position, new Quaternion(0, 0, 0, 0)) as GameObject;
        newBlockItem.GetComponent<BlockItem>().SetBlock(tile);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;

public class TerrainModifier
{
    public static readonly int maxRayDistance = 10000;

    private World world;

    public TerrainModifier(World world)
    {
        this.world = world;
    }

    /// <summary>
    /// Replaces the block directly in front of the player
    /// </summary>
    /// <param name="range"></param>
    /// <param name="block"></param>
    public void ReplaceBlockCenter(float range, BlockType block)
    {
        // Cast ray from Camera position
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("Items")))
        {
            // And its within the range limit
            if (hit.distance < range)
            {
                // Replace the block
                ReplaceBlockAt(hit, block);
            }
        }
    }


    /// <summary>
    /// Adds the block specified directly in front of the player
    /// </summary>
    /// <param name="range"></param>
    /// <param name="block"></param>
    public void AddBlockCenter(float range, BlockType block)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("Items")))
        {
            // And its within the range limit
            if (hit.distance < range)
            {
                // Add the block
                AddBlockAt(hit, block);
            }
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance), Color.green, 2);
        }
    }


    /// <summary>
    /// Replaces the block specified at current mouse cursor position
    /// </summary>
    /// <param name="block"></param>
    public void ReplaceBlockCursor(BlockType block)
    {
        // Cast ray from mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("Items")))
        {
            // Replace block and draw debug line
            ReplaceBlockAt(hit, block);
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
                            Color.green, 2);

        }

    }

    /// <summary>
    /// Adds the block specified at current mouse cursor position
    /// </summary>
    /// <param name="block"></param>
    public void AddBlockCursor(BlockType block)
    {
        // Cast ray from mouse position on screen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // If a hit occurs
        if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("Items")))
        {
            // Add block and draw debug line
            AddBlockAt(hit, block);
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
    public void ReplaceBlockAt(RaycastHit hit, BlockType block)
    {
        // Get position of hit
        Vector3 position = hit.point + (hit.normal * -0.5f);
        // Move position to center of hit block
        BlockType oldTile = world.data[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z)];
        // Set that hit block to new block
        SetBlockAt(position, block);
        SpawnBlockAt(position, oldTile);
    }


    /// <summary>
    /// Adds the specified block at specified hit point,
    /// you can raycast against the terrain and call this with the hit.point
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="block"></param>
    public void AddBlockAt(RaycastHit hit, BlockType block)
    {
        // Get position of hit
        Vector3 position = hit.point;
        // Move position into center of space where block should go
        position += (hit.normal * 0.5f);
        SetBlockAt(position, block);

    }


    /// <summary>
    /// Sets the specified block at specified coordinates
    /// </summary>
    /// <param name="position"></param>
    /// <param name="block"></param>
    public void SetBlockAt(Vector3 position, BlockType block)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        SetBlockAt(x, y, z, block);
    }

    /// <summary>
    /// Adds the specified block at specified coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="block"></param>
    public void SetBlockAt(int x, int y, int z, BlockType block)
    {
        //print($"SetBlockAt: {x}, {y}, {z}");

        world.data[x, y, z] = block;
        UpdateChunkAt(x, y, z);
    }

    /// <summary>
    /// Updates the chunk containing block at specified coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void UpdateChunkAt(int x, int y, int z)
    {
        //TODO: add a way to just flag the chunk for update then it update it in lateupdate
        int updateX = Mathf.FloorToInt(x / world.chunkSize);
        int updateY = Mathf.FloorToInt(y / world.chunkSize);
        int updateZ = Mathf.FloorToInt(z / world.chunkSize);

        //print($"UpdateChunkAt: {updateX}, {updateY}, {updateZ}");
        world.chunks[updateX, updateY, updateZ].update = true;

        // Determine if neighbor chunks need to update as well
        if (x - (world.chunkSize * updateX) == 0 && updateX != 0)
        {
            world.chunks[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (world.chunkSize * updateX) == 15 && updateX != world.chunks.GetLength(0) - 1)
        {
            world.chunks[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (world.chunkSize * updateY) == 0 && updateY != 0)
        {
            world.chunks[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (world.chunkSize * updateY) == 15 && updateY != world.chunks.GetLength(1) - 1)
        {
            world.chunks[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.chunks[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 15 && updateZ != world.chunks.GetLength(2) - 1)
        {
            world.chunks[updateX, updateY, updateZ + 1].update = true;
        }
    }

    public void SpawnBlockAt(Vector3 position, BlockType tile)
    {
        GameObject newBlockItem = GameObject.Instantiate(world.blockItemPrefab, position, new Quaternion(0, 0, 0, 0)) as GameObject;
        newBlockItem.GetComponent<BlockItem>().SetBlock(tile);
    }
}

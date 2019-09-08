using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks;

namespace Worlds {
    public static class Shaper {
        public static readonly int maxRayDistance = 10000;

        /// <summary>
        /// Replaces the block specified at current mouse cursor position
        /// </summary>
        /// <param name="block"></param>
        public static void ReplaceBlockAtCursor(BlockData block) {
            // Cast ray from mouse position on screen
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // If a hit occurs
            if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("World"))) {
                // Get the planet that raycast hit
                var planet = hit.transform.parent.GetComponent<Planet>();
                if (planet != null) {
                    // Replace block and draw debug line
                    ReplaceBlockAtRaycast(planet, hit, block);

                    // Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
                    //                 Color.green, 2);
                } else {
                    Debug.LogWarning("AddBlockAtCursor failed to get planet component after raycast hit");
                }
            }

        }

        /// <summary>
        /// Adds the block specified at current mouse cursor position
        /// </summary>
        /// <param name="block"></param>
        public static void AddBlockAtCursor(BlockData block) {
            // Cast ray from mouse position on screen
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // If a hit occurs
            if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.NameToLayer("World"))) {
                var planet = hit.transform.parent.GetComponent<Planet>();
                // Get the planet that raycast hit
                if (planet != null) {
                    // Add block and draw debug line
                    AddBlockAtRaycast(planet, hit, block);
                    Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
                                    Color.green, 2);
                } else {
                    Debug.LogWarning("AddBlockAtCursor failed to get planet component after raycast hit");
                }
            }
        }


        /// <summary>
        /// Removes a block at specified hit point,
        /// you can raycast against the terrain and call this with the hit.point
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="block"></param>
        public static void ReplaceBlockAtRaycast(Planet planet, RaycastHit hit, BlockData block) {
            // Move position from point of contact to center of hit block
            Vector3 worldPosition = hit.point + (hit.normal * -0.5f);
            // Manually get local block position so we can save old tile
            var localBlockPos = WorldToBlockDataPosition(planet, worldPosition);
            // Get old tile
            BlockData oldTile = planet.BlockAt(Mathf.RoundToInt(localBlockPos.x), Mathf.RoundToInt(localBlockPos.y), Mathf.RoundToInt(localBlockPos.z));

            // Set that hit block to new block
            SetBlockAtLocal(planet, localBlockPos, block);
            // Spawn a block from the old one
            SpawnBlockAtWorld(planet, worldPosition, oldTile);
        }


        /// <summary>
        /// Adds the specified block at specified hit point,
        /// you can raycast against the terrain and call this with the hit.point
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="block"></param>
        public static void AddBlockAtRaycast(Planet planet, RaycastHit hit, BlockData block) {
            //Move vector position from point of contact to center of air 
            var worldPosition = hit.point + (hit.normal * 0.5f);
            SetBlockAtWorld(planet, worldPosition, block);
        }


        /// <summary>
        /// Sets the specified block at specified coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        public static void SetBlockAtWorld(Planet planet, Vector3 worldPosition, BlockData block) {
            var position = WorldToBlockDataPosition(planet, worldPosition);

            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            int z = Mathf.RoundToInt(position.z);

            SetBlockAtLocal(planet, x, y, z, block);
        }

        /// <summary>
        /// Adds the specified block at specified coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="block"></param>
        public static void SetBlockAtLocal(Planet planet, int x, int y, int z, BlockData block) {
            planet.data[x, y, z] = block;
            planet.UpdateChunkAt(x, y, z);
        }

        /// <summary>
        /// Sets the specified block at specified coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        public static void SetBlockAtLocal(Planet planet, Vector3 position, BlockData block) {
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            int z = Mathf.RoundToInt(position.z);

            SetBlockAtLocal(planet, x, y, z, block);
        }

        public static void SpawnBlockAtWorld(Planet planet, Vector3 worldPosition, BlockData tile) {
            Debug.Log("Spawn blockitem");
        }

        public static Vector3 WorldToBlockDataPosition(Planet planet, Vector3 worldPosition) {
            var localPos = WorldToLocalPosition(planet, worldPosition);
            var localBlockPos = localPos + planet.planetBounds.extents;
            return localBlockPos;
        }

        public static Vector3 WorldToLocalPosition(Planet planet, Vector3 worldPosition) {
            return planet.transform.InverseTransformPoint(worldPosition);
        }
    }
}
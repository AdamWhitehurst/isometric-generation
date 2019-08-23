using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Block;
namespace Generation {

    public static class PlanetCrucible {
        public static Planet GeneratePlanet(PlanetPreset preset, string seed = "1337") {

            Planet planet = new GameObject().AddComponent<Planet>();

            var planetData = GeneratePlanetData(preset, seed);

            planet.SetBlockData(planetData);

            return planet;

        }
        /// <summary>
        /// Generates the 3D BlockType array for a planet
        /// </summary>
        /// <param name="preset">PlanetPreset to use for calculating how to fill planet data</param>
        /// <param name="seed">Varies planet data generation</param>
        /// <returns>3D BlockType array</returns>
        public static BlockType[,,] GeneratePlanetData(PlanetPreset preset, string seed = "1337") {
            FastNoise noise = new FastNoise();

            var eMapSouth = GenerateElevationMap(noise, preset, seed + "0");
            var eMapNorth = GenerateElevationMap(noise, preset, seed + "1");
            var eMapBot = GenerateElevationMap(noise, preset, seed + "2");
            var eMapTop = GenerateElevationMap(noise, preset, seed + "3");
            var eMapWest = GenerateElevationMap(noise, preset, seed + "4");
            var eMapEast = GenerateElevationMap(noise, preset, seed + "5");

            BlockType[,,] planetData = GeneratePlanetData(noise, preset);

            planetData = ApplyMapInXY(planetData, eMapSouth, 0);
            planetData = ApplyMapInXY(planetData, eMapNorth, preset.totalSize - 1);
            planetData = ApplyMapInXZ(planetData, eMapBot, 0);
            planetData = ApplyMapInXZ(planetData, eMapTop, preset.totalSize - 1);
            planetData = ApplyMapInYZ(planetData, eMapWest, 0);
            planetData = ApplyMapInYZ(planetData, eMapEast, preset.totalSize - 1);

            return planetData;
        }
        /// <summary>
        /// Generates a 3D BlockType array to use for planet generation
        /// </summary>
        /// <param name="noise">Noise generator to use for calculation</param>
        /// <param name="preset">PlanetPreset to use for calculation</param>
        /// <returns>A filled 3D BlockType array</returns>
        static BlockType[,,] GeneratePlanetData(FastNoise noise, PlanetPreset preset) {
            BlockType[,,] planet = new BlockType[preset.totalSize, preset.totalSize, preset.totalSize];
            int padding = preset.extent;
            for (int i = 0; i < preset.strata.Length; i++) {
                planet = LayerStratumOnPlanet(noise, preset.strata[i], planet, padding);
                if (i != 0) padding -= preset.strata[i].size;
            }
            return planet;
        }
        /// <summary>
        /// Generates a 2D map of elevations given the parameters.
        /// </summary>
        /// <param name="noise">Noise generator to use for calculation</param>
        /// <param name="preset"PlanetPreset to use for calculation></param>
        /// <param name="seed">Seed value to use for calculation</param>
        /// <returns>2D map of floats</returns>
        static float[,] GenerateElevationMap(FastNoise noise, PlanetPreset preset, string seed) {

            float[,] eMap = new float[preset.totalSize, preset.totalSize];

            noise.SetFrequency(1 / preset.mapElevationScale);
            for (int x = 0; x < preset.totalSize; x++) {
                for (int z = 0; z < preset.totalSize; z++) {
                    // Get value of noise in Range(-1f, 1f)
                    noise.SetFractalOctaves(preset.octaves);
                    noise.SetFractalLacunarity(preset.lacunarity);
                    noise.SetFractalType(preset.fractalType);
                    noise.SetNoiseType(preset.noiseType);
                    float noiseVal = noise.GetNoise(x, z);
                    // Convert to Range(0f, 1f)
                    float e = (noiseVal + 1) / 2;
                    eMap[x, z] = Mathf.Pow(e, preset.mapElevationPower);

                }
            }

            return eMap;
        }
        /// <summary>
        /// Returns the passed BlockType array with the passed PlanetStratum layered onto it.
        /// </summary>
        /// <param name="noise">Noise used to calculate stratum blocks</param>
        /// <param name="stratum">PlanetStratum to generate blocks from</param>
        /// <param name="planet">BlockType array to layer onto</param>
        /// <param name="padding">How much padding on outsides to leave</param>
        /// <returns>BlockType Array with PlanetStratum layer added</returns>
        static BlockType[,,] LayerStratumOnPlanet(FastNoise noise, PlanetStratum stratum, BlockType[,,] planet, int padding) {
            BlockType[,,] layeredPlanet = planet;

            int planetSize = planet.GetLength(0);

            int innerBound = padding + stratum.size;
            int outerBound = planetSize - innerBound;

            for (int x = padding; x < planetSize - padding; x++) {
                for (int y = padding; y < planetSize - padding; y++) {
                    for (int z = padding; z < planetSize - padding; z++) {
                        if (planet[x, y, z] == BlockType.Air) {
                            BlockType type = CalculateBlockFromPlanetStratum(noise, stratum, x, y, z);
                            layeredPlanet[x, y, z] = type;
                        }
                    }
                }
            }

            return layeredPlanet;
        }
        /// <summary>
        /// Calculates what BlockType should exist at given coordinate in planet, based on the passed Plenet Stratum
        /// </summary>
        /// <param name="noise">Noise generator to use for calculation</param>
        /// <param name="s">PlanetStratum to use for calculation</param>
        /// <param name="x">Planet coordinate X</param>
        /// <param name="y">Planet coordinate Y</param>
        /// <param name="z">Planet coordinate Z</param>
        /// <returns>BlockType given the planetstratum at the given coordinate</returns>
        private static BlockType CalculateBlockFromPlanetStratum(FastNoise noise, PlanetStratum s, int x, int y, int z) {
            BlockType type = BlockType.Air;
            noise.SetFrequency(1 / s.scale);
            // Get noise in Range(-1f, 1f)
            float noiseVal = noise.GetSimplexFractal(x, y, z);
            // Convert to Range(0f, 1f)
            float e = (noiseVal + 1) / 2;
            // Raise to strata power level
            e = Mathf.Pow(e, s.power);
            // Scale from e Range(0,1) to size of blockTypes array
            int index = Mathf.FloorToInt(e * s.blockDistribution.Length);
            // Set to type at that index in array
            type = s.blockDistribution[index];

            return type;
        }
        /// <summary>
        /// Removes blocks beyond the elevations map's given values from planet data
        /// in the XZ plane from the fixed Z coordinate
        /// </summary>
        /// <param name="planet">Planet Data on which elevation to apply map</param>
        /// <param name="elevationMap">Elevation map to apply</param>
        /// <param name="fixedZ">Coordinate in Z axis at which to apply Map</param>
        /// <returns>Planet data with elevation map applied</returns> 
        private static BlockType[,,] ApplyMapInXZ(BlockType[,,] planet, float[,] elevationMap, int fixedZ) {
            var newPlanet = planet;

            for (int x = 0; x < newPlanet.GetLength(0); x++) {
                for (int y = 0; y < newPlanet.GetLength(1); y++) {
                    // Determine how far to iterate into this plane
                    int limit = Mathf.RoundToInt(elevationMap[x, y] * newPlanet.GetLength(2));
                    // Determine which direction to iterate over,
                    int increment = (fixedZ == newPlanet.GetLength(2) - 1) ? -1 : 1;
                    int count = 0;
                    int offset = 0;
                    // until we reach the height limit defined by elevationMap
                    while (count <= limit) {
                        newPlanet[x, y, fixedZ + offset] = BlockType.Air;
                        offset += increment;
                        count++;
                    }
                }
            }

            return newPlanet;
        }
        /// <summary>
        /// Removes blocks beyond the elevations map's given values from planet data
        /// in the XY plane from the fixed Y coordinate
        /// </summary>
        /// <param name="planet">Planet Data on which elevation to apply map</param>
        /// <param name="elevationMap">Elevation map to apply</param>
        /// <param name="fixedY">Coordinate in Y axis at which to apply Map</param>
        /// <returns>Planet data with elevation map applied</returns> 
        private static BlockType[,,] ApplyMapInXY(BlockType[,,] planet, float[,] elevationMap, int fixedY) {
            var newPlanet = planet;

            for (int x = 0; x < newPlanet.GetLength(0); x++) {
                for (int z = 0; z < newPlanet.GetLength(2); z++) {
                    // Determine how far to iterate into this plane
                    int limit = Mathf.RoundToInt(elevationMap[x, z] * newPlanet.GetLength(1));
                    // Determine which direction to iterate over,
                    int increment = (fixedY == newPlanet.GetLength(1) - 1) ? -1 : 1;
                    int count = 0;
                    int offset = 0;
                    // until we reach the height limit defined by elevationMap
                    while (count <= limit) {
                        newPlanet[x, fixedY + offset, z] = BlockType.Air;
                        offset += increment;
                        count++;
                    }
                }
            }

            return newPlanet;
        }
        /// <summary>
        /// Removes blocks beyond the elevations map's given values from planet data
        /// in the YX plane from the fixed X coordinate
        /// </summary>
        /// <param name="planet">Planet Data on which elevation to apply map</param>
        /// <param name="elevationMap">Elevation map to apply</param>
        /// <param name="fixedX">Coordinate in X axis at which to apply Map</param>
        /// <returns>Planet data with elevation map applied</returns> 
        private static BlockType[,,] ApplyMapInYZ(BlockType[,,] planet, float[,] elevationMap, int fixedX) {
            var newPlanet = planet;

            for (int y = 0; y < newPlanet.GetLength(1); y++) {
                for (int z = 0; z < newPlanet.GetLength(2); z++) {
                    // Determine how far to iterate into this plane
                    int limit = Mathf.RoundToInt(elevationMap[y, z] * newPlanet.GetLength(0));
                    // Determine which direction to iterate over,
                    int increment = (fixedX == planet.GetLength(0) - 1) ? -1 : 1;
                    int count = 0;
                    int offset = 0;
                    // until we reach the height limit defined by elevationMap
                    while (count <= limit) {
                        planet[fixedX + offset, y, z] = BlockType.Air;
                        offset += increment;
                        count++;
                    }
                }
            }

            return newPlanet;
        }

    }
}
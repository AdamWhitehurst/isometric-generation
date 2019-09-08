using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Blocks;
namespace Worlds {

    public static class Crucible {
        /// <summary>
        /// Generates the 3D BlockType array for a planet
        /// </summary>
        /// <param name="preset">Preset to use for calculating how to fill planet data</param>
        /// <returns>3D BlockType array</returns>
        public static BlockData[,,] GenerateElevatedPlanetData(Preset preset) {
            FastNoise noise = new FastNoise();
            BlockData[,,] planetData = GeneratePlanetStrataData(preset);
            float[][,] elevationMaps = GenerateElevationMaps(preset);
            planetData = ApplyElevations(preset, planetData, elevationMaps);

            return planetData;
        }
        /// <summary>
        /// Generates a 3D BlockType array to use for planet generation
        /// </summary>
        /// <param name="preset">Preset to use for calculation</param>
        /// <returns>A filled 3D BlockType array</returns>
        public static BlockData[,,] GeneratePlanetStrataData(Preset preset) {
            FastNoise noise = new FastNoise();
            BlockData[,,] planet = new BlockData[preset.totalSize, preset.totalSize, preset.totalSize];
            int padding = preset.extent;
            for (int i = 0; i < preset.strata.Length; i++) {
                if (i != 0) padding -= preset.strata[i].size;
                planet = LayerStratumOnPlanet(noise, preset.strata[i], planet, padding);
            }
            return planet;
        }

        /// <summary>
        /// Returns the passed BlockType array with the passed PlanetStratum layered onto it.
        /// </summary>
        /// <param name="noise">Noise used to calculate stratum blocks</param>
        /// <param name="stratum">PlanetStratum to generate blocks from</param>
        /// <param name="planet">BlockType array to layer onto</param>
        /// <param name="padding">How much padding on outsides to leave</param>
        /// <returns>BlockType Array with PlanetStratum layer added</returns>
        static BlockData[,,] LayerStratumOnPlanet(FastNoise noise, PlanetStratum stratum, BlockData[,,] planet, int padding) {
            BlockData[,,] layeredPlanet = planet;

            int planetSize = planet.GetLength(0);

            int innerBound = padding + stratum.size;
            int outerBound = planetSize - innerBound;

            for (int x = padding; x < planetSize - padding; x++) {
                for (int y = padding; y < planetSize - padding; y++) {
                    for (int z = padding; z < planetSize - padding; z++) {
                        if (planet[x, y, z] == null) {
                            BlockData data = CalculateBlockFromPlanetStratum(noise, stratum, x, y, z);
                            layeredPlanet[x, y, z] = data;
                        }
                    }
                }
            }

            return layeredPlanet;
        }

        public static BlockData[,,] ApplyElevations(Preset preset, BlockData[,,] planetData, float[][,] elevationMaps) {
            FastNoise noise = new FastNoise();

            planetData = ErodePlaneY(planetData, elevationMaps[0], 1);
            planetData = ErodePlaneY(planetData, elevationMaps[1], -1);
            planetData = ErodePlaneZ(planetData, elevationMaps[2], 1);
            planetData = ErodePlaneZ(planetData, elevationMaps[3], -1);
            planetData = ErodePlaneX(planetData, elevationMaps[4], 1);
            planetData = ErodePlaneX(planetData, elevationMaps[5], -1);

            return planetData;
        }

        public static float[][,] GenerateElevationMaps(Preset preset) {
            FastNoise noise = new FastNoise();

            float[][,] elevationMaps = new float[6][,];

            for (int i = 0; i < 6; i++) {
                elevationMaps[i] = GenerateSingleElevationMap(preset, $"{preset.planetSeed} Side {i}");
            }

            return elevationMaps;
        }

        /// <summary>
        /// Generates a 2D map of elevations given the parameters.
        /// </summary>
        /// <param name="preset">Preset to use for calculation</param>
        /// <param name="seed">Seed to use to vary elevation map</param>
        /// <returns>2D map of floats</returns>
        static float[,] GenerateSingleElevationMap(Preset preset, string seed) {
            FastNoise noise = new FastNoise(seed.GetHashCode());

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
                    eMap[x, z] = Mathf.Pow(e, preset.mapElevationPower) * preset.mapElevationDamping;

                }
            }

            return eMap;
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
        private static BlockData CalculateBlockFromPlanetStratum(FastNoise noise, PlanetStratum s, int x, int y, int z) {
            BlockData data = null;
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
            data = s.blockDistribution[index];

            return data;
        }
        /// <summary>
        /// Removes blocks beyond the elevations map's given values from planet data
        /// in the XZ plane from the fixed Z coordinate
        /// </summary>
        /// <param name="planet">Planet Data on which elevation to apply map</param>
        /// <param name="elevationMap">Elevation map to apply</param>
        /// <param name="fixedZ">Coordinate in Z axis at which to apply Map</param>
        /// <returns>Planet data with elevation map applied</returns> 
        private static BlockData[,,] ErodePlaneZ(BlockData[,,] planet, float[,] elevationMap, int direction) {
            var newPlanet = planet;

            for (int x = 0; x < newPlanet.GetLength(0); x++) {
                for (int y = 0; y < newPlanet.GetLength(1); y++) {
                    // Determine how far to iterate into this plane
                    int limit = Mathf.RoundToInt(elevationMap[x, y] * newPlanet.GetLength(2));
                    // Determine which direction to iterate over,
                    int fixedZ = (direction == 1) ? 0 : newPlanet.GetLength(2) - 1;
                    int count = 0;
                    int offset = 0;
                    // until we reach the height limit defined by elevationMap
                    while (count <= limit) {
                        newPlanet[x, y, fixedZ + offset] = null;
                        offset += direction;
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
        private static BlockData[,,] ErodePlaneY(BlockData[,,] planet, float[,] elevationMap, int direction) {
            var newPlanet = planet;

            for (int x = 0; x < newPlanet.GetLength(0); x++) {
                for (int z = 0; z < newPlanet.GetLength(2); z++) {
                    // Determine how far to iterate into this plane
                    int limit = Mathf.RoundToInt(elevationMap[x, z] * newPlanet.GetLength(1));
                    int fixedY = (direction == 1) ? 0 : newPlanet.GetLength(1) - 1;
                    int count = 0;
                    int offset = 0;
                    // until we reach the height limit defined by elevationMap
                    while (count <= limit) {
                        newPlanet[x, fixedY + offset, z] = null;
                        offset += direction;
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
        private static BlockData[,,] ErodePlaneX(BlockData[,,] planet, float[,] elevationMap, int direction) {
            var newPlanet = planet;

            for (int y = 0; y < newPlanet.GetLength(1); y++) {
                for (int z = 0; z < newPlanet.GetLength(2); z++) {
                    // Determine how far to iterate into this plane
                    int limit = Mathf.RoundToInt(elevationMap[y, z] * newPlanet.GetLength(0));
                    int fixedX = (direction == 1) ? 0 : newPlanet.GetLength(0) - 1;
                    int count = 0;
                    int offset = 0;
                    // until we reach the height limit defined by elevationMap
                    while (count <= limit) {
                        planet[fixedX + offset, y, z] = null;
                        offset += direction;
                        count++;
                    }
                }
            }

            return newPlanet;
        }

    }
}
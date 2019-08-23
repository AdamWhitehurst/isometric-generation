using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Generation;
public class Settings : MonoBehaviour {
    [SerializeField] private float characterSpeed = 25f;
    public static float CharacterSpeed => instance.characterSpeed;

    [SerializeField] private float characterRotationSpeed = 180f;
    public static float CharacterRotationSpeed => instance.characterRotationSpeed;

    [SerializeField] private float characterJumpHeight = 9f;
    public static float CharacterJumpHeight => instance.characterJumpHeight;

    [SerializeField] private float maxVelocity = 2f;
    public static float MaxVelocity => instance.maxVelocity;

    [SerializeField]
    private GameObject chunkPrefab;
    public static GameObject ChunkPrefab => instance.chunkPrefab;

    [SerializeField]
    public GameObject blockItemPrefab;


    public static GameObject BlockItemPrefab => instance.blockItemPrefab;

    [SerializeField]
    public PlanetPreset defaultPlanetPreset;


    public static PlanetPreset DefaultPlanetPreset => instance.defaultPlanetPreset;


    #region Singleton Pattern 
    private static Settings instance { get; set; }

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
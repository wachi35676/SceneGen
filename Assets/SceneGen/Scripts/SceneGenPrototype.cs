using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGenPrototype : MonoBehaviour
{
    [Header("Select the list of Biomes")]
    public Boolean DesertBiome;
    public Boolean ForestBiome;
    public Boolean MountainBiome;
    public Boolean WaterBiome;
    public Boolean CityBiome;
    public Boolean SnowBiome;

    [Header("Tune biome ")]
    [Range(0.0f, 1.0f)]
    public float MinBiomeSize;
    [Range(0.0f, 1.0f)]
    public float MaxBiomeSize;
    [Range(0.0f, 1.0f)]
    public float BiomeTransitionSize;
    
    [Header("Add your player's setting")]
    [Range(0.0f, 1.0f)]
    public float PlayerHeight;
    [Range(0.0f, 1.0f)]
    public float PlayerJumpHeight;
    [Range(0.0f, 1.0f)]
    public float PlayerSpeed;
    
    [Header("Add spirites for each biome")]
    [Header("Desert")]
    public Sprite DesertSprite1;
    public Sprite DesertSprite2;
    
    [Header("Forest")]
    public Sprite ForestSprite1;
    public Sprite ForestSprite2;
    
    [Header("Mountain")]
    public Sprite MountainSprite1;
    public Sprite MountainSprite2;
    
    [Header("Water")]
    public Sprite WaterSprite1;
    public Sprite WaterSprite2;
    
    [Header("City")]
    public Sprite CitySprite1;
    public Sprite CitySprite2;
    
    [Header("Snow")]
    public Sprite SnowSprite1;
    public Sprite SnowSprite2;
    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate()
    {

    }
}

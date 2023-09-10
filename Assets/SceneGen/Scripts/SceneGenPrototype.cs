using System;
using System.Collections;
using System.Collections.Generic;
using SceneGen.Scripts;
using UnityEngine;
using SceneGen.Scripts; // Import the namespace where the NoiseGeneratorFactory is defined


public class SceneGenPrototype : MonoBehaviour
{
    [Header("Select the list of Biomes")]
    public Boolean DesertBiome;
    public Boolean ForestBiome;
    public Boolean MountainBiome;
    public Boolean WaterBiome;
    public Boolean CityBiome;
    public Boolean SnowBiome;
    
    [Header("Select the list of Algorithms")]
    public NoiseType NoiseType;
    

    [Header("Tune biome ")]
    [Range(0.0f, 100.0f)]
    public float MinBiomeSize;
    [Range(0.0f, 100.0f)]
    public float MaxBiomeSize;
    [Range(0.0f, 1.0f)]
    public float BiomeTransitionSize;
    
    [Header("Add your player's setting")]
    [Range(0.0f, 10.0f)]
    public float PlayerHeight;
    [Range(0.0f, 10.0f)]
    public float PlayerJumpHeight;
    [Range(0.0f, 10.0f)]
    public float PlayerSpeed;
    
    [Header("Add spirites for each biome")]
    [Header("Desert")]
    public Sprite DesertSprite1;
    public Sprite DesertSprite2;
    
    [Header("Forest")]
    public GameObject Dirt;
    public GameObject Grass;
    [Range(0.0f, 1.0f)]
    public float Flatness;
    
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

    private INoiseGenerator _noiseGenerator;
    
    public void Generate()
    {
        Clear();
        SceneGeneration();
    }

    void Spawn(GameObject obj, Vector3 position, Quaternion rotation)
    {
        obj = Instantiate(obj, position, rotation);
        obj.transform.parent = this.transform;
    }
    
    public void Clear()
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        for (int i = 0; i < children.Count; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }
    }

    public void SceneGeneration()
    {
        float width = UnityEngine.Random.Range(MinBiomeSize, MaxBiomeSize);

        float noiseScale = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f); // Randomize the noise scale
        float heightScale = 5f + UnityEngine.Random.Range(-1f, 1f); // Randomize the height scale
        float noiseOffset = UnityEngine.Random.Range(-100f, 100f); // Randomize the noise offset

        INoiseGenerator _noiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType);
        
        for (int i = 0; i < width; i++)
        {
            float noiseValue = _noiseGenerator.GenerateNoise(i,noiseOffset,noiseScale);
            int height = Mathf.RoundToInt(noiseValue * heightScale);

            // Apply flatness attribute
            int maxHeight = Mathf.RoundToInt(height * (1f - Flatness));

            for (int j = 0; j < maxHeight; j++)
            {
                Spawn(Dirt, new Vector3(i, j, 0), Quaternion.identity);
            }

            int h = (int)maxHeight;
            Spawn(Grass, new Vector3(i, h, 0), Quaternion.identity);
        }
    }
}

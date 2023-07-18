using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGenPrototype : MonoBehaviour
{
    [Header("Select the list of Biomes")]
    public bool DesertBiome;
    public bool ForestBiome;
    public bool MountainBiome;
    public bool WaterBiome;
    public bool CityBiome;
    public bool SnowBiome;

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

    [Header("Add sprites for each biome")]
    [Header("Desert")]
    public Sprite DesertSprite1;
    public Sprite DesertSprite2;

    [Header("Forest")]
    public GameObject Dirt;
    public GameObject Grass;
    public GameObject SkyPrefab;
    public GameObject TreePrefab;
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
    //
    public float skyGridSize = 0.2f; // Adjust this to control the density of the Worley noise for the sky
    public float skyOffset = 0f;
    public int maxTreeCount = 30;

public void Generate()
{
    Clear();

    float width = UnityEngine.Random.Range(MinBiomeSize, MaxBiomeSize);

    // Generate terrain
    int terrainTop = Mathf.RoundToInt(PlayerJumpHeight);

    for (int x = 0; x < width; x++)
    {
        float terrainNoise = CalculateWorleyNoise(x, skyGridSize, skyOffset);
        int terrainHeight = Mathf.RoundToInt(terrainNoise * PlayerJumpHeight);

        int currentY = 0; // Variable to track the current y position

        for (int y = 0; y < terrainHeight; y++)
        {
            Spawn(Dirt, new Vector3(x, y, 0), Quaternion.identity);
            currentY = y;
        }

        Spawn(Grass, new Vector3(x, terrainHeight, 0), Quaternion.identity);

        // Randomly spawn trees on grass points according to treeCount
        if (UnityEngine.Random.Range(10, 100) < maxTreeCount)
        {
            int treeY = terrainHeight + 1;
            Spawn(TreePrefab, new Vector3(x, treeY, 0), Quaternion.identity);
        }

        // Generate sky below the platform
        for (int y = terrainHeight + 1; y <= terrainTop; y++)
        {
            if (ForestBiome)
            {
                // Spawn sky prefab specific to the Forest biome
                Spawn(SkyPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
            else
            {
                // For other biomes, you can use a general sky prefab or apply a sky color
                Spawn(SkyPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }

    // Fill the sky above the platform
    int screenHeight = Mathf.RoundToInt(PlayerHeight);
    for (int y = terrainTop + 1; y <= screenHeight; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (ForestBiome)
            {
                // Spawn sky prefab specific to the Forest biome
                Spawn(SkyPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
            else
            {
                // For other biomes, you can use a general sky prefab or apply a sky color
                Spawn(SkyPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }
}


  void Spawn(GameObject obj, Vector3 position, Quaternion rotation)
  {
      GameObject spawnedObject = Instantiate(obj, position, rotation, transform);

      // Adjust the sorting layer and order in layer values for the spawned object's renderer
      Renderer renderer = spawnedObject.GetComponent<Renderer>();
      if (renderer != null)
      {
          if (obj == TreePrefab)
          {
              renderer.sortingLayerName = "Foreground"; // Set the desired sorting layer name for trees
              renderer.sortingOrder = 2; // Set a higher order in layer value for trees
          }
          else
          {
              renderer.sortingLayerName = "Background"; // Set the desired sorting layer name for sky
              renderer.sortingOrder = 1; // Set the order in layer value for sky
          }
      }
  }





    float CalculateWorleyNoise(float x, float gridSize, float offset)
    {
        float distance = float.MaxValue;

        int pointCount = 5; // Number of random points to generate
       //seeds are randomly placed on each grid whichhas randomly chosen biome
       for (int i = 0; i < pointCount; i++)
        {
            float point = UnityEngine.Random.Range(x, x + gridSize);
            float currentDistance = Mathf.Abs(point - x);
            //then biome of each grid is based on closest seed
            if (currentDistance < distance)
                distance = currentDistance;
        }

        return distance / gridSize;
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
}
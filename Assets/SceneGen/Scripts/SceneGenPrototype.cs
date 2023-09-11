using System;
using System.Collections;
using System.Collections.Generic;
using SceneGen.Scripts;
using UnityEngine;
using SceneGen.Scripts;
using UnityEngine.Serialization; // Import the namespace where the NoiseGeneratorFactory is defined


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
    public float PlayerJumpHeight;
    [Range(0.0f, 10.0f)]
    public float PlayerSpeed;
    
    [Header("Add spirites for each biome")]
    [Header("Desert")]
    public Sprite DesertSprite1;
    public Sprite DesertSprite2;
    
    [Header("Forest")]
    public GameObject Dirt;
    public GameObject DirtEdge1;
    public GameObject DirtEdge2;
    public GameObject GrassMiddle;
    public GameObject GrassMiddle2;

    public GameObject GrassPlatformEdge;
    public GameObject GrassPlatformLeft;
    public GameObject GrassPlatformRight;
    public GameObject GrassPlatformMiddle;
    
    [Header("(Optional) Add corner grass")]
    public GameObject CornerGrass;
    public GameObject CornerGrassWide;
    
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

    void Spawn(GameObject obj, Vector3 position, Quaternion rotation, int scale = 1)
    {
        obj = Instantiate(obj, position, rotation);

        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;
        var localScale = obj.transform.localScale;

        localScale = new Vector3(localScale.x / sprite.bounds.size.x, localScale.y / sprite.bounds.size.y, 1);
        obj.transform.localScale = localScale * scale;

        obj.transform.parent = transform;
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
        Quaternion rotation;

        float noiseValue = _noiseGenerator.GenerateNoise(0, noiseOffset, noiseScale);
        int height = Mathf.RoundToInt(noiseValue * heightScale);
        int dirtCount = 0;
        int offset = 0;
        
        for (int i = 0; i < width; i++)
        {
            int lastHeight = height;
            
            int h = lastHeight;
            
            noiseValue = _noiseGenerator.GenerateNoise(i, noiseOffset, noiseScale);
            
            height = Mathf.RoundToInt(noiseValue * heightScale);

            if (CornerGrass != null)
            {
                if (height > lastHeight)
                {
                    Spawn(CornerGrass, new Vector3(i + offset, height, 0), Quaternion.Euler(180, 0, 180));
                }
                else if (height < lastHeight)
                {
                    Spawn(CornerGrass, new Vector3(i + offset + 1, height + 1, 0), Quaternion.identity);
                }
            }
            else if (CornerGrassWide != null)
            {
            
                if (height > lastHeight)
                {
                    Spawn(CornerGrassWide, new Vector3((i + offset) + 1f / 2f, (height) - 1f / 2f, 0), Quaternion.Euler(180, 0, 180), 2);
                    dirtCount = 2;
                }
                else if (height < lastHeight)
                {
                    Spawn(CornerGrassWide, new Vector3((i + offset) + 1f / 2f, (height) + 1f / 2f, 0), Quaternion.identity, 2);
                    dirtCount = 2;
                }
                
            }

            for (int j = 0; j < lastHeight; j++)
            {
                // Random rotation for the object only in increments of 90 degrees
                rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90);
                Spawn(Dirt, new Vector3(i + offset, j, 0), rotation);
            }
            
            rotation = Quaternion.identity;
            if (dirtCount == 0)
            {
                int grassRandom = UnityEngine.Random.Range(0, 1);
                switch (grassRandom)
                {
                    case 0:
                        Spawn(GrassMiddle, new Vector3(i + offset, h, 0), rotation);
                        break;
                    case 1:
                        Spawn(GrassMiddle2, new Vector3(i + offset, h, 0), rotation);
                        break;
                }
            }
            else
            {
                dirtCount--;
            }

            if (lastHeight == height)
            {
                offset += GeneratePlatforms(i + offset, h);
            }
        }
    }

    private int GeneratePlatforms(int i, int h)
    {
        int starting = i;
        if (UnityEngine.Random.Range(0, 100) < 10)
        {
            i++;
            int gap = (int) UnityEngine.Random.Range(1, PlayerSpeed);
            int platformWidth = UnityEngine.Random.Range(1, 10);
            int platformHeight = (int) UnityEngine.Random.Range(1, PlayerJumpHeight);
            
            for (int j = 0; j < h; j++)
            {
                int dirtRandom = UnityEngine.Random.Range(0, 1);
                switch (dirtRandom)
                {
                    case 0:
                        Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.identity);
                        break;
                    case 1:
                        Spawn(DirtEdge2, new Vector3(i, j, 0), Quaternion.identity);
                        break;
                }
            }
            Spawn(GrassPlatformEdge, new Vector3(i, h, 0), Quaternion.identity);
            
            
            Spawn(GrassPlatformLeft, new Vector3(i + 1 + (gap / 2), h + platformHeight, 0), Quaternion.identity);
            for (int k = 2; k < platformWidth; k++)
            {
                Spawn(GrassPlatformMiddle, new Vector3(i + k + (gap / 2), h + platformHeight, 0), Quaternion.identity);
            }
            Spawn(GrassPlatformRight, new Vector3(i + platformWidth + (gap / 2), h + platformHeight, 0), Quaternion.identity);
            
            i += gap +  platformWidth + 1;
            
            for (int j = 0; j < h; j++)
            {
                int dirtRandom = UnityEngine.Random.Range(0, 1);
                switch (dirtRandom)
                {
                    case 0:
                        Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                        break;
                    case 1:
                        Spawn(DirtEdge2, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                        break;
                }
            }
            Spawn(GrassPlatformEdge, new Vector3(i, h, 0), Quaternion.Euler(180, 0, 180));
        }
        
        int offset = i - starting ;
        return offset;
    }
}

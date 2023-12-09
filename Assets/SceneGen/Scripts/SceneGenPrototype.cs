using System;
using System.Collections;
using System.Collections.Generic;
using SceneGen.Scripts;
using UnityEngine;
using SceneGen.Scripts;
using Unity.VisualScripting;
using UnityEngine.Serialization; // Import the namespace where the NoiseGeneratorFactory is defined


public class SceneGenPrototype : MonoBehaviour
{
    [Header("Select the list of Algorithms")]
    public NoiseType NoiseType;
    

    [Header("Tune biome ")]
    [Range(0.0f, 100.0f)]
    public float MinBiomeSize;
    [Range(0.0f, 100.0f)]
    public float MaxBiomeSize;
    
    [Header("Add your player's setting")]
    [Range(0.0f, 10.0f)]
    public float PlayerJumpHeight;
    [Range(0.0f, 10.0f)]
    public float PlayerSpeed;
    
    public GameObject Dirt;
    public GameObject DirtEdge1;
    public GameObject DirtEdge2;
    public GameObject GrassMiddle;
    public GameObject GrassMiddle2;

    public GameObject GrassPlatformEdge;
    public GameObject GrassPlatformLeft;
    public GameObject GrassPlatformRight;
    public GameObject GrassPlatformMiddle;
    
    [Range(0.0f, 20.0f)]
    public float Height;
    
    [Header("(Optional) Add corner grass")]
    public GameObject CornerGrass;
    public GameObject CornerGrassWide;
    public GameObject CornerGrassHigh;
    
    [Header("(Optional) Add Bridge")]
    public GameObject Bridge;

    [Header("(Optional) Water")]
    public GameObject Water;
    [Range(0.0f, 20.0f)]
    public float WaterHeight;
    public GameObject WaterBody;

    [Header("(Optional) Cave")]
    public int CaveCount;
    public GameObject Cave;
    public GameObject CaveCorner;
    [Range(0, 20)]
    public float CaveHeight;
    public float CaveScale;
    
    private INoiseGenerator _noiseGenerator;
    
    public void Generate()
    {
        Clear();
        SceneGeneration();
    }

    void Spawn(GameObject obj, Vector3 position, Quaternion rotation, int heightScale = 1, int widthScale = 1, int orderInLayer = 0, bool toMask = false)
    {
        obj = Instantiate(obj, position, rotation);
        obj.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;

        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;

        var localScale = obj.transform.localScale;

        localScale = new Vector3(localScale.x / sprite.bounds.size.x, localScale.y / sprite.bounds.size.y, 1);
        obj.transform.localScale = new Vector3(localScale.x * widthScale, localScale.y * heightScale, 1);

        obj.transform.parent = transform;

        if (Cave != null)
        {
            if (toMask)
            {
                obj.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else
            {
                //make a new game object
                GameObject mask = new GameObject();
                //add a sprite mask component to it
                SpriteMask maskComponent = mask.AddComponent<SpriteMask>();
                //set the mask component to the sprite renderer of the object
                maskComponent.sprite = obj.GetComponent<SpriteRenderer>().sprite;
                mask.transform.position = obj.transform.position;
                mask.transform.localScale = obj.transform.localScale;
                mask.transform.rotation = obj.transform.rotation;

                mask.transform.parent = obj.transform;
            }
        }

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
    // Generate a random width for the biome
    float width = UnityEngine.Random.Range(MinBiomeSize, MaxBiomeSize);

    // Randomize the noise scale
    float noiseScale = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f);
    
    float heightScale = Height;

    // Randomize the noise offset
    float noiseOffset = UnityEngine.Random.Range(-100f, 100f);

    // Initialize a noise generator based on NoiseType
    INoiseGenerator _noiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType);
    
    Quaternion rotation;
    
    INoiseGenerator _caveNoiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType.PerlinNoise);
    _caveNoiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType.PerlinNoise);
    float caveNoiseValue = _noiseGenerator.GenerateNoise(0, noiseOffset, noiseScale);
    float caveNoiseScale = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f);
    float caveHeightScale = CaveHeight;
    float caveNoiseOffset = UnityEngine.Random.Range(-100f, 100f);
    int heightCave = Mathf.RoundToInt(caveNoiseValue * caveHeightScale);
    
    // Calculate the initial height based on noise
    int nextNextHeight = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(2, noiseOffset, noiseScale) * heightScale);
    int nextHeight = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(1, noiseOffset, noiseScale) * heightScale);
    int height = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(0, noiseOffset, noiseScale) * heightScale);
    int prevHeight = height;
    int lastLastHeight = height;
    

    int dirtCount = 0;
    int offset = 0;
    
    for (int i = 0; i < width; i++)
    {
        lastLastHeight = prevHeight;
        prevHeight = height;
        height = nextHeight;
        nextHeight = nextNextHeight;
        
        int h = height;
        
        // Calculate the height based on the current noise value
        nextNextHeight = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(i + 2, noiseOffset, noiseScale) * heightScale);
        
        
        if (CornerGrassWide != null && prevHeight == lastLastHeight && height == prevHeight + 1)
        {
            // Spawn wider corner grass at higher terrain
            Spawn(CornerGrassWide, new Vector3((i + offset) - 1f / 2f, (height) - 1f / 2f, 0), Quaternion.Euler(180, 0, 180), 2, 2, 1);
            dirtCount = 1;
            h--;
        }
        else if (CornerGrass != null && height == prevHeight + 1)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrass, new Vector3(i + offset, height - 1f / 2f, 0), Quaternion.Euler(180, 0, 180), 2);
            dirtCount = 1;
            h--;
            h--;
        }
        else if (CornerGrassHigh != null && height == prevHeight + 2)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrassHigh, new Vector3(i + offset, height - 1, 0), Quaternion.Euler(180, 0, 180), 3);
            dirtCount = 1;
            h--;
            h--;
            h--;
        }
            
        else if (CornerGrassWide != null && nextHeight == nextNextHeight && height == nextHeight + 1)
        {
            // Spawn wider corner grass at higher terrain
            Spawn(CornerGrassWide, new Vector3((i + offset) + 1f / 2f, (height) - 1f / 2f, 0), Quaternion.identity, 2, 2, 1);
            dirtCount = 1;
            h--;
        }
        else if (CornerGrass != null && height == nextHeight + 1)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrass, new Vector3(i + offset , height - 1f / 2f, 0), Quaternion.identity, 2, 1, 1);
            dirtCount = 1;
            h--;
            h--;
            
        }
        else if (CornerGrassHigh != null && height == nextHeight + 2)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrassHigh, new Vector3(i + offset, height - 1, 0), Quaternion.identity, 3, 1, 1);
            dirtCount = 1;
            h--;
            h--;
            h--;
        }
        
        for (int j = 0; j < h; j++)
        {
            Spawn(Dirt, new Vector3(i + offset, j, 0), Quaternion.identity);
        }
       
        
        if (Cave != null)
        {
            caveNoiseValue = _caveNoiseGenerator.GenerateNoise(i, caveNoiseOffset, caveNoiseScale);
            heightCave = Mathf.RoundToInt(caveNoiseValue * caveHeightScale);

            
            for (int j = heightCave; j <= height && j < heightCave + CaveScale; j++)
            {
                if (j <= height)
                {
                    Spawn(Cave, new Vector3(i + offset, j, 0), Quaternion.identity, 1, 1, 2, true);
                }

                if (CaveCorner != null && j == heightCave)
                {
                    int prevHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i - 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    int nextHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i + 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    
                    if (prevHeightCave < heightCave)
                    {
                        Spawn(CaveCorner, new Vector3(i + offset, j - 1, 0), Quaternion.Euler(0, 0, 180), 1, 1, 2, true);
                    }
                    if (nextHeightCave < heightCave)
                    {
                        Spawn(CaveCorner, new Vector3(i + offset, j - 1, 0), Quaternion.Euler(0, 0, 90), 1, 1, 2, true);
                    }
                }

                if (CaveCorner != null && j == heightCave + CaveScale - 1)
                {
                    int prevHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i - 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    int nextHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i + 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    
                    if (prevHeightCave < heightCave)
                    {
                        Spawn(CaveCorner, new Vector3(i + offset - 1, j, 0), Quaternion.Euler(0, 0, 0), 1, 1, 2, true);
                    }
                    if (nextHeightCave < heightCave )
                    {
                        Spawn(CaveCorner, new Vector3(i + offset + 1, j, 0), Quaternion.Euler(0, 0, 270), 1, 1, 2, true);
                    }
                }
                
            }

        }

        
        if (dirtCount == 0)
        {
            int grassRandom = UnityEngine.Random.Range(0, 1);
            switch (grassRandom)
            {
                case 0:
                    // Spawn middle grass
                    Spawn(GrassMiddle, new Vector3(i + offset, h, 0), Quaternion.identity);
                    break;
                case 1:
                    // Spawn alternative middle grass
                    Spawn(GrassMiddle2, new Vector3(i + offset, h, 0), Quaternion.identity);
                    break;
            }
        }
        else
        {
            dirtCount--;
            Spawn(Dirt, new Vector3(i + offset, h, 0), Quaternion.identity);
        }

        if (prevHeight == height && height == nextHeight)
        {
            // Generate platforms if the terrain height remains the same
            offset += GeneratePlatforms(i + offset, h, Bridge != null);
        }
    }

    if (Water != null)
    {
        GenerateWater((int)width + offset, (int)WaterHeight);
    }
}

private void GenerateWater(int width, int height)
{
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            Spawn(WaterBody, new Vector3(i, j, 0), Quaternion.identity, 1 , 1, -1);
        }
        Spawn(Water, new Vector3(i, height, 0), Quaternion.identity, 1 , 1, -1);
    }
}

private int GeneratePlatforms(int i, int h, bool isBridge = false)
{
    int starting = i;

    // Randomly decide whether to create a platform
    if (UnityEngine.Random.Range(0, 100) < 10)
    {
        i++;
        int gap = (int)UnityEngine.Random.Range(1, PlayerSpeed);
        int platformWidth = UnityEngine.Random.Range(1, 10);

        for (int j = 0; j < h; j++)
        {
            int dirtRandom = UnityEngine.Random.Range(0, 1);
            switch (dirtRandom)
            {
                case 0:
                    // Spawn dirt edge type 1
                    Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.identity);
                    break;
                case 1:
                    // Spawn dirt edge type 2
                    Spawn(DirtEdge2, new Vector3(i, j, 0), Quaternion.identity);
                    break;
            }
        }

        // Spawn the edge of the grass platform
        Spawn(GrassPlatformEdge, new Vector3(i, h, 0), Quaternion.identity);
        
        //generate a random number 1 or 0
        int bridgeRandom = UnityEngine.Random.Range(0, 2);

        if (!isBridge || bridgeRandom == 0)
        {
            int platformHeight = (int)UnityEngine.Random.Range(1, PlayerJumpHeight);

            // Spawn the left part of the grass platform
            Spawn(GrassPlatformLeft, new Vector3(i + 1 + (gap / 2), h + platformHeight, 0), Quaternion.identity);

            for (int k = 2; k < platformWidth; k++)
            {
                // Spawn middle parts of the grass platform
                Spawn(GrassPlatformMiddle, new Vector3(i + k + (gap / 2), h + platformHeight, 0), Quaternion.identity);
            }

            // Spawn the right part of the grass platform
            Spawn(GrassPlatformRight, new Vector3(i + platformWidth + (gap / 2), h + platformHeight, 0),
                Quaternion.identity);
        }
        else
        {
            for (int k = 0; k <= gap + platformWidth + 1; k++)
            {
                // Spawn middle parts of the grass platform
                Spawn(Bridge, new Vector3(i+k, h , 0), Quaternion.identity, 1, 1, -1);
            }
        }

        i += gap + platformWidth + 1;

        for (int j = 0; j < h; j++)
        {
            int dirtRandom = UnityEngine.Random.Range(0, 1);
            switch (dirtRandom)
            {
                case 0:
                    // Spawn inverted dirt edge type 1
                    Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                    break;
                case 1:
                    // Spawn inverted dirt edge type 2
                    Spawn(DirtEdge2, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                    break;
            }
        }
        // Spawn the inverted edge of the grass platform
        Spawn(GrassPlatformEdge, new Vector3(i, h, 0), Quaternion.Euler(180, 0, 180));
    }

    int offset = i - starting;
    return offset;
}

}
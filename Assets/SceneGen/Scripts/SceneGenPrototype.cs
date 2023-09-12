using System;
using System.Collections;
using System.Collections.Generic;
using SceneGen.Scripts;
using UnityEngine;
using SceneGen.Scripts;
using Unity.VisualScripting;
using UnityEngine.Serialization; // Import the namespace where the NoiseGeneratorFactory is defined

//Beach Biome, Zoya Mahboob 20i-0524

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
    
    
    [Header("Beach")]
    public GameObject BaseSand;
    public GameObject BaseSand1;
    public GameObject BaseSand2;
    public GameObject TopSand;
    public GameObject TopSand2;

    public GameObject SandPlatformEdge;
    public GameObject SandPlatformLeft;
    public GameObject SandPlatformRight;
    public GameObject SandPlatformMiddle;
    
    [Header("(Optional) Add corner grass")]
    public GameObject CornerSand;
    public GameObject CornerSandWide;
    
    /*[Header("Mountain")]
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
    public Sprite SnowSprite2;*/

    private INoiseGenerator _noiseGenerator;
    
    public void Generate()
    {
        Clear();
        SceneGeneration();
    }
    
    //Scaling the textures
    void Spawn(GameObject obj, Vector3 position, Quaternion rotation, int scale = 1)
    {
        obj = Instantiate(obj, position, rotation);

        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;
        var localScale = obj.transform.localScale;

        localScale = new Vector3(localScale.x / sprite.bounds.size.x, localScale.y / sprite.bounds.size.y, 1);
        obj.transform.localScale = localScale * scale;

        obj.transform.parent = transform;
    }

    //Picking the children and destroying them all
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
    
 //   
public void SceneGeneration()
{
    // Generate a random width for the biome
    float width = UnityEngine.Random.Range(MinBiomeSize, MaxBiomeSize);

    // Randomize the noise scale
    float noiseScale = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f);
    
    // Randomize the height scale
    float heightScale = 3f + UnityEngine.Random.Range(-1f, 1f);
    //float heightScale = 2f + UnityEngine.Random.Range(-1f, 1f);

    // Randomize the noise offset
    float noiseOffset = UnityEngine.Random.Range(-100f, 100f);

    // Initialize a noise generator based on NoiseType
    INoiseGenerator _noiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType);
    
    Quaternion rotation;

    // Generate noise value for the initial position
    float noiseValue = _noiseGenerator.GenerateNoise(0, noiseOffset, noiseScale);
    
    // Calculate the initial height based on noise
    int height = Mathf.RoundToInt(noiseValue * heightScale);
    
    int dirtCount = 0;
    int offset = 0;
    
    for (int i = 0; i < width; i++)
    {
        int lastHeight = height;
        
        int h = lastHeight;
        
        // Generate noise value for the current position
        noiseValue = _noiseGenerator.GenerateNoise(i, noiseOffset, noiseScale);
        
        // Calculate the height based on the current noise value
        height = Mathf.RoundToInt(noiseValue * heightScale);

        if (CornerSand != null)
        {
            
            //Determining the height of the previous platform to generate the next platform
            // Handle corner grass placement
            if (height > lastHeight)
            {
                // Spawn corner grass at higher terrain
                Spawn(CornerSand, new Vector3(i + offset, height, 0), Quaternion.Euler(180, 0, 180));
            }
            else if (height < lastHeight)
            {
                // Spawn corner grass at lower terrain
                Spawn(CornerSand, new Vector3(i + offset + 1, height + 1, 0), Quaternion.identity);
            }
        }
        else if (CornerSandWide != null)
        {
            // Handle wider corner grass placement
            if (height > lastHeight)
            {
                // Spawn wider corner grass at higher terrain
                Spawn(CornerSandWide, new Vector3((i + offset) + 1f / 2f, (height) - 1f / 2f, 0), Quaternion.Euler(180, 0, 180), 2);
                dirtCount = 2;
            }
            else if (height < lastHeight)
            {
                // Spawn wider corner grass at lower terrain
                Spawn(CornerSandWide, new Vector3((i + offset) + 1f / 2f, (height) + 1f / 2f, 0), Quaternion.identity, 2);
                dirtCount = 2;
            }
        }

        for (int j = 0; j < lastHeight; j++)
        {
            // Randomly rotate the dirt objects in 90-degree increments
            rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90);
            Spawn(BaseSand, new Vector3(i + offset, j, 0), rotation);
        }
        
        rotation = Quaternion.identity;
        if (dirtCount == 0)
        {
            int grassRandom = UnityEngine.Random.Range(0, 1);
            switch (grassRandom)
            {
                case 0:
                    // Spawn middle grass
                    Spawn(TopSand, new Vector3(i + offset, h, 0), rotation);
                    break;
                case 1:
                    // Spawn alternative middle grass
                    Spawn(TopSand2, new Vector3(i + offset, h, 0), rotation);
                    break;
            }
        }
        else
        {
            dirtCount--;
        }

        if (lastHeight == height)
        {
            if (GetRandomValue() % 2 == 1)
            {
                offset += GeneratePlatforms(i + offset, h); //generate the platform
                Debug.Log("Platform Generated!");
            }
            else
            {
                Debug.Log("Empty space");
                //Leave an empty space 
                offset += GenerateEmptySpace(i + offset, h);
            }
        }



    }
}

int GetRandomValue() {
    float rand = UnityEngine.Random.value;
    if (rand <= .33f)
        return UnityEngine.Random.Range(0, 6);
    if (rand <= .33f)
        return UnityEngine.Random.Range(6, 9);
    return UnityEngine.Random.Range(9, 10);
}


private int GenerateEmptySpace(int i, int h)
{
    int starting = i;

    // Randomly decide whether to create a platform
    if (UnityEngine.Random.Range(0, 100) < 10)
    {
        i++;
        int gap = (int)UnityEngine.Random.Range(1, PlayerSpeed);
        int platformWidth = UnityEngine.Random.Range(1, 10);
        int platformHeight = (int)UnityEngine.Random.Range(1, PlayerJumpHeight);

        for (int j = 0; j < h; j++)
        {
            int sandRandom = UnityEngine.Random.Range(0, 1);
            switch (sandRandom)
            {
                case 0:
                    // Spawn dirt edge type 1
                    Spawn(BaseSand1, new Vector3(i, j, 0), Quaternion.identity);
                    break;
                case 1:
                    // Spawn dirt edge type 2
                    Spawn(BaseSand2, new Vector3(i, j, 0), Quaternion.identity);
                    break;
            }
        }
        // Spawn the edge of the grass platform
        Spawn(SandPlatformEdge, new Vector3(i, h, 0), Quaternion.identity);

        // Spawn the left part of the grass platform
        /*Spawn(SandPlatformLeft, new Vector3(i + 1 + (gap / 2), h + platformHeight, 0), Quaternion.identity);

        for (int k = 2; k < platformWidth; k++)
        {
            // Spawn middle parts of the grass platform
            Spawn(SandPlatformMiddle, new Vector3(i + k + (gap / 2), h + platformHeight, 0), Quaternion.identity);
        }

        // Spawn the right part of the grass platform
        Spawn(SandPlatformRight, new Vector3(i + platformWidth + (gap / 2), h + platformHeight, 0), Quaternion.identity);
        */

        i += gap + platformWidth + 1;

        for (int j = 0; j < h; j++)
        {
            int sandRandom = UnityEngine.Random.Range(0, 1);
            switch (sandRandom)
            {
                case 0:
                    // Spawn inverted dirt edge type 1
                    Spawn(BaseSand1, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                    break;
                case 1:
                    // Spawn inverted dirt edge type 2
                    Spawn(BaseSand2, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                    break;
            }
        }
        // Spawn the inverted edge of the grass platform
        Spawn(SandPlatformEdge, new Vector3(i, h, 0), Quaternion.Euler(180, 0, 180));
    }

    int offset = i - starting;
    return offset;

}

private int GeneratePlatforms(int i, int h)
{
    int starting = i;

    // Randomly decide whether to create a platform
    if (UnityEngine.Random.Range(0, 100) < 10)
    {
        i++;
        int gap = (int)UnityEngine.Random.Range(1, PlayerSpeed);
        int platformWidth = UnityEngine.Random.Range(1, 10);
        int platformHeight = (int)UnityEngine.Random.Range(1, PlayerJumpHeight);

        for (int j = 0; j < h; j++)
        {
            int sandRandom = UnityEngine.Random.Range(0, 1);
            switch (sandRandom)
            {
                case 0:
                    // Spawn dirt edge type 1
                    Spawn(BaseSand1, new Vector3(i, j, 0), Quaternion.identity);
                    break;
                case 1:
                    // Spawn dirt edge type 2
                    Spawn(BaseSand2, new Vector3(i, j, 0), Quaternion.identity);
                    break;
            }
        }
        // Spawn the edge of the grass platform
        Spawn(SandPlatformEdge, new Vector3(i, h, 0), Quaternion.identity);

        // Spawn the left part of the grass platform
        Spawn(SandPlatformLeft, new Vector3(i + 1 + (gap / 2), h + platformHeight, 0), Quaternion.identity);

        for (int k = 2; k < platformWidth; k++)
        {
            // Spawn middle parts of the grass platform
            Spawn(SandPlatformMiddle, new Vector3(i + k + (gap / 2), h + platformHeight, 0), Quaternion.identity);
        }

        // Spawn the right part of the grass platform
        Spawn(SandPlatformRight, new Vector3(i + platformWidth + (gap / 2), h + platformHeight, 0), Quaternion.identity);

        i += gap + platformWidth + 1;

        for (int j = 0; j < h; j++)
        {
            int sandRandom = UnityEngine.Random.Range(0, 1);
            switch (sandRandom)
            {
                case 0:
                    // Spawn inverted dirt edge type 1
                    Spawn(BaseSand1, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                    break;
                case 1:
                    // Spawn inverted dirt edge type 2
                    Spawn(BaseSand2, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                    break;
            }
        }
        // Spawn the inverted edge of the grass platform
        Spawn(SandPlatformEdge, new Vector3(i, h, 0), Quaternion.Euler(180, 0, 180));
    }

    int offset = i - starting;
    return offset;
}

}

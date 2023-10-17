//Diamond-Square Algorithm


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareAlgorithm : MonoBehaviour
{
    [Header("Select the list of Biomes")]
    public bool DesertBiome;
    public bool ForestBiome;
    public bool MountainBiome;
    public bool WaterBiome;
    public bool CityBiome;
    public bool SnowBiome;

    [Header("Tune biome")]
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

    public void Generate()
    {
        Clear();

        int width = (int)UnityEngine.Random.Range(MinBiomeSize, MaxBiomeSize);
        int height = (int)PlayerJumpHeight;

        float[,] terrain = new float[width + 1, height + 1];

        float roughness = 1.0f;

        // Set the corners of the terrain
        terrain[0, 0] = UnityEngine.Random.Range(0.0f, 1.0f);
        terrain[width, 0] = UnityEngine.Random.Range(0.0f, 1.0f);
        terrain[0, height] = UnityEngine.Random.Range(0.0f, 1.0f);
        terrain[width, height] = UnityEngine.Random.Range(0.0f, 1.0f);

        // Generate the terrain using Diamond-Square algorithm
        DiamondSquare(terrain, 0, 0, width, height, roughness);

        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                for (int j = 0; j < Mathf.RoundToInt(terrain[x, y] * height); j++)
                {
                    Spawn(Dirt, new Vector3(x, j, 0), Quaternion.identity);
                }
                int h = Mathf.RoundToInt(terrain[x, y] * height);
                Spawn(Grass, new Vector3(x, h, 0), Quaternion.identity);
            }
        }
    }

  private void DiamondSquare(float[,] terrain, int xStart, int yStart, int xEnd, int yEnd, float roughness)
{
    int width = xEnd - xStart;
    int height = yEnd - yStart;

    if (width > 1 || height > 1)
    {
        int xMid = (xStart + xEnd) / 2;
        int yMid = (yStart + yEnd) / 2;

        // Diamond step
        float avg = (terrain[xStart, yStart] + terrain[xEnd, yStart] + terrain[xStart, yEnd] + terrain[xEnd, yEnd]) / 4.0f;
        float offset = UnityEngine.Random.Range(-roughness, roughness);
        terrain[xMid, yMid] = avg + offset;

        // Square step
        if (width > 1)
        {
            float left = (terrain[xStart, yStart] + terrain[xStart, yEnd]) / 2.0f;
            float right = (terrain[xEnd, yStart] + terrain[xEnd, yEnd]) / 2.0f;
            offset = UnityEngine.Random.Range(-roughness, roughness);
            terrain[xMid, yStart] = left + offset;
            terrain[xMid, yEnd] = right + offset;
        }
        if (height > 1)
        {
            float top = (terrain[xStart, yStart] + terrain[xEnd, yStart]) / 2.0f;
            float bottom = (terrain[xStart, yEnd] + terrain[xEnd, yEnd]) / 2.0f;
            offset = UnityEngine.Random.Range(-roughness, roughness);
            terrain[xStart, yMid] = top + offset;
            terrain[xEnd, yMid] = bottom + offset;
        }

        // Curvature adjustment
        float curveAmount = (xEnd - xStart) * 0.1f; // Adjust this value to control the curvature
        terrain[xMid, yMid] += (terrain[xStart, yStart] + terrain[xEnd, yStart] + terrain[xStart, yEnd] + terrain[xEnd, yEnd]) * curveAmount;

        // Smooth edges
        if (xStart == 0 || yStart == 0 || xEnd == terrain.GetLength(0) - 1 || yEnd == terrain.GetLength(1) - 1)
        {
            terrain[xMid, yMid] = (terrain[xStart, yStart] + terrain[xEnd, yStart] + terrain[xStart, yEnd] + terrain[xEnd, yEnd]) / 4.0f;
        }

        // Recurse on sub-squares
        DiamondSquare(terrain, xStart, yStart, xMid, yMid, roughness * 0.5f);
        DiamondSquare(terrain, xMid, yStart, xEnd, yMid, roughness * 0.5f);
        DiamondSquare(terrain, xStart, yMid, xMid, yEnd, roughness * 0.5f);
        DiamondSquare(terrain, xMid, yMid, xEnd, yEnd, roughness * 0.5f);
    }
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
}

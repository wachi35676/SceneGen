using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellularNoise : MonoBehaviour
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

        float width = UnityEngine.Random.Range(MinBiomeSize, MaxBiomeSize);

        float cellSize = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f); // Randomize the cell size
        float heightScale = 5f + UnityEngine.Random.Range(-1f, 1f); // Randomize the height scale

        for (int i = 0; i < width; i++)
        {
            float noiseValue = CellularNoiseFunc((i + 0.5f) * cellSize, 0);
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

    public float CellularNoiseFunc(float x, float y) //The input x and y coordinates 
    {
        Vector2Int[] cellPoints = new Vector2Int[3]; //Immediate neighbours of the input coordinates by taking floor and ceiling values
        cellPoints[0] = new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
        cellPoints[1] = new Vector2Int(Mathf.CeilToInt(x), Mathf.FloorToInt(y));
        cellPoints[2] = new Vector2Int(Mathf.FloorToInt(x), Mathf.CeilToInt(y));

        float minDistance = float.MaxValue;
        for (int i = 0; i < cellPoints.Length; i++)
        {
            //For organic random patterns, unity's random circle is used
            Vector2 point = cellPoints[i] + UnityEngine.Random.insideUnitCircle; 
            float distance = Vector2.Distance(new Vector2(x, y), point); //Minimum distance between points and input coordinates
            minDistance = Mathf.Min(minDistance, distance); //Minimum dist is used since cellular noise is based on the closest point
        }

        return minDistance;
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

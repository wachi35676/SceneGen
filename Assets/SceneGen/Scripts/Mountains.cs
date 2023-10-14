using System.Collections.Generic;
using UnityEngine;

public class Mountains : MonoBehaviour
{
    [SerializeField] private int width, height=10;
    [SerializeField] private int minStoneHeight, maxStoneHeight;
    [SerializeField] private GameObject dirt, grass, stone;
    [SerializeField] private int numberOfGaps;
    [SerializeField] private int maxGapWidth;

    private void Start()
    {
        Clear();
        GenerateMountains();
    }

    public void GenerateMountains()
    {
        List<int> gapStartIndices = new List<int>();
        while (gapStartIndices.Count < numberOfGaps)
        {
            int gapStartIndex = Random.Range(1, width - maxGapWidth - 1); // Subtract 1 from width - maxGapWidth
            if (!gapStartIndices.Contains(gapStartIndex))
            {
                gapStartIndices.Add(gapStartIndex);
            }
        }

        for (int x = 0; x < width; x++)
        {
            int minHeight = height - 1;
            int maxHeight = height + 2;
            height = Random.Range(minHeight, maxHeight);
            int minStoneSpawnDistance = height - minStoneHeight;
            int maxStoneSpawnDistance = height - maxStoneHeight;
            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSpawnDistance);

            if (gapStartIndices.Contains(x))
            {
                int gapWidth = Random.Range(1, maxGapWidth + 1);
                if (gapWidth > 5)
                {
                    int platformWidth = Mathf.Min(gapWidth - 2, width - x - 2); // Subtract 2 from gapWidth and width - x
                    int platformStartX = x + 1; // Add 1 to x

                    if (platformWidth > 0)
                    {
                        for (int i = platformStartX; i < platformStartX + platformWidth; i++) // Use < instead of <=
                        {
                            spawnObj(dirt, i, height);
                        }
                    }
                }

                x += gapWidth - 1;
                continue;
            }

            for (int y = 0; y < height; y++)
            {
                if (y < totalStoneSpawnDistance)
                {
                    spawnObj(stone, x, y);
                }
                else
                {
                    spawnObj(dirt, x, y);
                }
            }

            if (totalStoneSpawnDistance == height)
            {
                spawnObj(stone, x, height);
            }
            else
            {
                spawnObj(grass, x, height);
            }
        }
    }

    void spawnObj(GameObject obj, int width, int height)
    {
        obj = Instantiate(obj, new Vector2(width, height), Quaternion.identity);
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
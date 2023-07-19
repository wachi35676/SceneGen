using System.Collections.Generic;
using UnityEngine;


public class Mountains : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private int minStoneheight, maxStoneHeight;
    [SerializeField] private GameObject dirt, grass, stone ;
    private void Start()
    {
        Clear();
        GenerateMountains();
    }

    public void GenerateMountains()
    {
        
        for (int x = 0; x < width; x++)
        {
            int minHeight = height - 1;
            int maxHeight = height + 2;
            height = Random.Range(minHeight, maxHeight);
            int minStoneSpawnDistance = height - minStoneheight;
            int maxStoneSpawnDistance = height - maxStoneHeight;
            int totalStoneSpawnDistance = Random.Range(minStoneSpawnDistance, maxStoneSpawnDistance);
            
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
                spawnObj(grass,x,height);
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
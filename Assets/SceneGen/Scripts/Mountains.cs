using System.Collections.Generic;
using UnityEngine;


public class Mountains : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private GameObject dirt, grass, stone ;
    private void Start()
    {
        GenerateMountains();
    }

    public void GenerateMountains()
    {
        Clear();
        for (int x = 0; x < width; x++)
        {
            int minHeight = height - 1;
            int maxHeight = height + 1;
            height = Random.Range(minHeight, maxHeight);
            int minStoneSpawnDistance = height - 5;
            int maxStoneSpawnDistance = height - 6;
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
            spawnObj(grass,x,height);
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
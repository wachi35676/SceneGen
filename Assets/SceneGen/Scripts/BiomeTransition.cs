using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeTransition : MonoBehaviour
{
    public GameObject[] biomes;

public void Generate()
{
    float currentPosition = 0;

    for (int i = 0; i < biomes.Length; i++)
    {
        GameObject biome = biomes[i];
        float width = biome.GetComponent<SceneGenPrototype>().TotalWidth;
        Vector3 newPosition = new Vector3(currentPosition, biome.transform.position.y, biome.transform.position.z);
        biome.transform.position = newPosition;

        currentPosition += width;
    }
}
}

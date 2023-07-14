using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : INoiseGenerator
{
    public float GenerateNoise(int index, float noiseOffset, float noiseScale)
    {
        float noiseValue = Mathf.PerlinNoise((index + noiseOffset) * noiseScale, 0);
        return noiseValue;
    }

}

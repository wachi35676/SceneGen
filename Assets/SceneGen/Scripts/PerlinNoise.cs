using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : INoiseGenerator
{
    
    public float GenerateNoise(float noiseOffset, float noiseScale)
    {
        float noiseValue = Mathf.PerlinNoise((noiseOffset) * noiseScale, 0);
            return noiseValue;
           
    }

}

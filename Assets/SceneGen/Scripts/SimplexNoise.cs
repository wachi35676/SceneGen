using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

public class SimplexNoise : INoiseGenerator
{
    private OpenSimplexNoise noiseGenerator;

    public float GenerateNoise(int index, float noiseOffset, float noiseScale)
    {
        noiseGenerator = new OpenSimplexNoise();
        
        float noiseValue = SimplexNoiseFunc(index * noiseScale + noiseOffset, 0);
         return noiseValue;
    }

    public float SimplexNoiseFunc(float x, float y)
    {
        double noiseValue = noiseGenerator.Evaluate(x, y);
        return (float) noiseValue;
    }

}

using System;
using UnityEngine;

using NoiseTest;

namespace SceneGen.Scripts
{
    public enum NoiseType
    {
        PerlinNoise,
        CellularNoise,
        SimplexNoise
        // You can add more noise types here if needed
    }

    public interface INoiseGenerator
    {
        float GenerateNoise(int index, float x, float y);
    
    }

    public class PerlinNoise : INoiseGenerator
    {
        public float GenerateNoise(int index, float noiseOffset, float noiseScale)
        {
            float noiseValue = Mathf.PerlinNoise((index + noiseOffset) * noiseScale, 0);
            return noiseValue;
        }
    }

    public class CellularNoise : INoiseGenerator
    {
        public float GenerateNoise(int index, float noiseOffset, float cellSize)
        {
            float noiseValue;
            noiseValue = CellularNoiseFunc((index + 0.5f) * cellSize, 0);
            return noiseValue;
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
    }

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

    public class NoiseGeneratorFactory
    {
        private static INoiseGenerator noiseSingleton;

        // Private constructor prevents direct instantiation
        private NoiseGeneratorFactory()
        {
        }

        public static INoiseGenerator InitializeAndGetNoiseGenerator(NoiseType noiseType)
        {
            switch (noiseType)
            {
                case NoiseType.PerlinNoise:
                    noiseSingleton = new PerlinNoise();
                    break;
                case NoiseType.CellularNoise:
                    noiseSingleton = new CellularNoise();
                    break;
                case NoiseType.SimplexNoise:
                    noiseSingleton = new SimplexNoise();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(noiseType), noiseType, null);
            }
            

            return noiseSingleton;
        }

        public static INoiseGenerator GetNoiseGenerator(NoiseType noiseType)
        {
            if (noiseSingleton == null)
            {
                noiseSingleton = InitializeAndGetNoiseGenerator(noiseType);
            }
            
            return noiseSingleton;
        }
        
    }
}

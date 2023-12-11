using System;
using UnityEngine;

using NoiseTest;

namespace SceneGen.Scripts
{
    public enum NoiseType
    {
        PerlinNoise,
        CellularNoise,
        SimplexNoise,
        PerlinNoiseAndCellularNoise,
        PerlinNoiseAndSimplexNoise,
        DiamondSquareNoise
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

    public class PerlinNoiseAndCellularNoise : INoiseGenerator
    {
        private PerlinNoise perlin = new PerlinNoise();
        private CellularNoise cellular = new CellularNoise();

        public float GenerateNoise(int index, float noiseOffset, float cellSizeOrNoiseScale) //The cell size and noise scale are different names for the same variables
        {
            return perlin.GenerateNoise(index, noiseOffset, cellSizeOrNoiseScale) +
                   cellular.GenerateNoise(index, noiseOffset, cellSizeOrNoiseScale);
        }
    }

    public class PerlinNoiseAndSimplexNoise : INoiseGenerator
    {
        private PerlinNoise perlin = new PerlinNoise();
        private SimplexNoise simplex = new SimplexNoise();

        public float GenerateNoise(int index, float noiseOffset, float noiseScale) //The cell size and noise scale are different names for the same variables
        {
            return perlin.GenerateNoise(index, noiseOffset, noiseScale) +
                   simplex.GenerateNoise(index, noiseOffset, noiseScale);
        }
    }

    
   public class DiamondSquareNoise : INoiseGenerator
{
    private float[,] grid;
    private float roughness;
    private int width;

    public DiamondSquareNoise(int size, float roughness)
    {
        this.width = size;
        this.roughness = roughness;
        this.grid = new float[size, size];
    }

    public void Generate()
    {
        DiamondSquare(0, 0, width - 1, width - 1);
    }

    private void DiamondSquare(int xStart, int yStart, int xEnd, int yEnd)
    {
        int width = xEnd - xStart;
        if (width <= 1)
            return;

        int xMid = (xStart + xEnd) / 2;
        int yMid = (yStart + yEnd) / 2;

        // Diamond step
        grid[xMid, yMid] = AverageOfCorners(xStart, yStart, xEnd, yEnd) + RandomOffset(width);

        // Square step
        grid[xMid, yStart] = AverageOfTopAndBottomCorners(xMid, yStart, yEnd) + RandomOffset(width);
        grid[xStart, yMid] = AverageOfLeftAndRightCorners(xStart, yMid, xEnd) + RandomOffset(width);
        grid[xMid, yEnd] = AverageOfTopAndBottomCorners(xMid, yStart, yEnd) + RandomOffset(width);
        grid[xEnd, yMid] = AverageOfLeftAndRightCorners(xStart, yMid, xEnd) + RandomOffset(width);

        // Recurse on sub-squares
        DiamondSquare(xStart, yStart, xMid, yMid);
        DiamondSquare(xMid, yStart, xEnd, yMid);
        DiamondSquare(xStart, yMid, xMid, yEnd);
        DiamondSquare(xMid, yMid, xEnd, yEnd);
    }

    private float AverageOfCorners(int xStart, int yStart, int xEnd, int yEnd)
    {
        return (grid[xStart, yStart] + grid[xStart, yEnd] + grid[xEnd, yStart] + grid[xEnd, yEnd]) / 4;
    }

    private float AverageOfTopAndBottomCorners(int xMid, int yStart, int yEnd)
    {
        return (grid[xMid, yStart] + grid[xMid, yEnd]) / 2;
    }

    private float AverageOfLeftAndRightCorners(int xStart, int yMid, int xEnd)
    {
        return (grid[xStart, yMid] + grid[xEnd, yMid]) / 2;
    }

    private float RandomOffset(int width)
    {
        return (float)(new System.Random().NextDouble() * 2 - 1) * width * roughness;
    }
    
    public float GenerateNoise(int index, float noiseOffset, float noiseScale)
    {
        Generate();
        return grid[index, 0];
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
                case NoiseType.PerlinNoiseAndCellularNoise:
                    noiseSingleton = new PerlinNoiseAndCellularNoise();
                    break;
                case NoiseType.PerlinNoiseAndSimplexNoise:
                    noiseSingleton = new PerlinNoiseAndSimplexNoise();
                    break;
                case NoiseType.DiamondSquareNoise:
                    noiseSingleton = new DiamondSquareNoise(100, 0.5f);
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


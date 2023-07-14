using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


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

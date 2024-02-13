using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SceneGen.Scripts;
using UnityEngine;
using SceneGen.Scripts;
using Unity.VisualScripting;
using UnityEngine.Serialization; // Import the namespace where the NoiseGeneratorFactory is defined


public class SceneGenPrototype : MonoBehaviour
{
    [Header("Select the list of Algorithms")]
    public NoiseType NoiseType;
    

    [Header("Tune biome ")]
    [Range(0.0f, 100.0f)]
    public float BiomeWidth;
    
    [Header("Add your player's setting")]
    [Range(0.0f, 10.0f)]
    public float PlayerJumpHeight;
    [Range(0.0f, 10.0f)]
    public float PlayerJumpDistance;
    [Range(0.0f, 10.0f)]
    public float PlayerAcceleration;
    [Range(0.0f, 10.0f)]
    public float PlayerMaxSpeed;
    
    [Header("Add your Assets")]
    public GameObject Dirt;
    public GameObject DirtEdge1;
    public GameObject DirtEdge2;
    public GameObject GrassMiddle;
    public GameObject GrassMiddle2;

    public GameObject GrassPlatformEdge;
    public GameObject GrassPlatformLeft;
    public GameObject GrassPlatformRight;
    public GameObject GrassPlatformMiddle;
    
    [Range(0.0f, 20.0f)]
    public float Height;
    
    [Header("(Optional) Add corner grass")]
    public GameObject CornerGrass;
    public GameObject CornerGrassWide;
    public GameObject CornerGrassHigh;
    
    
    [Header("(Optional) Add Tree")]
    
    [Range(0,6)]
    public int iterations = 4;
    public string input = "F";
    public float angle = 30.0f;
    public string rule = "F[-F]F[+F][F]";
    private string output;
    public Dictionary<char, string> rules = new Dictionary<char, string>();
    
    public GameObject cylinder;
    
    [Header("(Optional) Add Bridge")]
    public GameObject Bridge;

    [Header("(Optional) Water")]
    public GameObject Water;
    [Range(0.0f, 20.0f)]
    public float WaterHeight;
    public GameObject WaterBody;

    [Header("(Optional) Cave")]
    public GameObject Cave;
    public GameObject CaveCorner;
    [Range(0, 20)]
    public float CaveHeight;
    public float CaveScale;

    [Header("(Optional) Building")]
    public GameObject Building;

    [Header("(Optional) Collectables")]
    public GameObject Collectable;
    
    [Header("(Optional) Underwater")]
    public GameObject UnderwaterObject1;
    public GameObject UnderwaterObject2;
    public GameObject UnderwaterObject3;
    public GameObject UnderwaterObject4;
    public GameObject CoralReef1;
    public GameObject CoralReef2;
    public GameObject CoralReef3;

    private INoiseGenerator _noiseGenerator;
    
    public void Generate()
    {
        Clear();
        SceneGeneration();
    }

    void Spawn(GameObject obj, Vector3 position, Quaternion rotation, int heightScale = 1, int widthScale = 1, int orderInLayer = 0, bool toMask = false)
    {
        obj = Instantiate(obj, position, rotation);
        obj.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;

        Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;

        var localScale = obj.transform.localScale;

        localScale = new Vector3(localScale.x / sprite.bounds.size.x, localScale.y / sprite.bounds.size.y, 1);
        obj.transform.localScale = new Vector3(localScale.x * widthScale, localScale.y * heightScale, 1);

        obj.transform.parent = transform;

        if (Cave != null)
        {
            if (toMask)
            {
                obj.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else
            {
                //make a new game object
                GameObject mask = new GameObject();
                //add a sprite mask component to it
                SpriteMask maskComponent = mask.AddComponent<SpriteMask>();
                //set the mask component to the sprite renderer of the object
                maskComponent.sprite = obj.GetComponent<SpriteRenderer>().sprite;
                mask.transform.position = obj.transform.position;
                mask.transform.localScale = obj.transform.localScale;
                mask.transform.rotation = obj.transform.rotation;

                mask.transform.parent = obj.transform;
            }
        }

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
    
    
public void SceneGeneration()
{
    
    // Generate a random width for the biome
    float width = BiomeWidth;

    // Randomize the noise scale
    float noiseScale = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f);
    
    float heightScale = Height;

    // Randomize the noise offset
    float noiseOffset = UnityEngine.Random.Range(-100f, 100f);

    // Initialize a noise generator based on NoiseType
    INoiseGenerator _noiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType);
    
    Quaternion rotation;
    
    INoiseGenerator _caveNoiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType.PerlinNoise);
    _caveNoiseGenerator = NoiseGeneratorFactory.InitializeAndGetNoiseGenerator(NoiseType.PerlinNoise);
    float caveNoiseValue = _noiseGenerator.GenerateNoise(0, noiseOffset, noiseScale);
    float caveNoiseScale = 0.1f + UnityEngine.Random.Range(-0.05f, 0.05f);
    float caveHeightScale = CaveHeight;
    float caveNoiseOffset = UnityEngine.Random.Range(-100f, 100f);
    int heightCave = Mathf.RoundToInt(caveNoiseValue * caveHeightScale);
    
    List<Vector3> underwaterObjectPositions = new List<Vector3>(); // List to store the positions of spawned underwater objects
    
    // Calculate the initial height based on noise
    int nextNextHeight = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(2, noiseOffset, noiseScale) * heightScale);
    int nextHeight = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(1, noiseOffset, noiseScale) * heightScale);
    int height = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(0, noiseOffset, noiseScale) * heightScale);
    int prevHeight = height;
    int lastLastHeight = height;
    

    int dirtCount = 0;
    int offset = 0;
    
    for (int i = 0; i < width; i++)
    {
        lastLastHeight = prevHeight;
        prevHeight = height;
        height = nextHeight;
        nextHeight = nextNextHeight;
        
        int h = height;
        
        // Calculate the height based on the current noise value
        nextNextHeight = Mathf.RoundToInt(_noiseGenerator.GenerateNoise(i + 2, noiseOffset, noiseScale) * heightScale);
        
        
        if (CornerGrassWide != null && prevHeight == lastLastHeight && height == prevHeight + 1)
        {
            // Spawn wider corner grass at higher terrain
            Spawn(CornerGrassWide, new Vector3((i + offset) - 1f / 2f, (height) - 1f / 2f, 0), Quaternion.Euler(180, 0, 180), 2, 2, 1);
            dirtCount = 1;
            h--;
        }
        else if (CornerGrass != null && height == prevHeight + 1)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrass, new Vector3(i + offset, height - 1f / 2f, 0), Quaternion.Euler(180, 0, 180), 2);
            dirtCount = 1;
            h--;
            h--;
        }
        else if (CornerGrassHigh != null && height == prevHeight + 2)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrassHigh, new Vector3(i + offset, height - 1, 0), Quaternion.Euler(180, 0, 180), 3);
            dirtCount = 1;
            h--;
            h--;
            h--;
        }
            
        else if (CornerGrassWide != null && nextHeight == nextNextHeight && height == nextHeight + 1)
        {
            // Spawn wider corner grass at higher terrain
            Spawn(CornerGrassWide, new Vector3((i + offset) + 1f / 2f, (height) - 1f / 2f, 0), Quaternion.identity, 2, 2, 1);
            dirtCount = 1;
            h--;
        }
        else if (CornerGrass != null && height == nextHeight + 1)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrass, new Vector3(i + offset , height - 1f / 2f, 0), Quaternion.identity, 2, 1, 1);
            dirtCount = 1;
            h--;
            h--;
            
        }
        else if (CornerGrassHigh != null && height == nextHeight + 2)
        {
            // Spawn corner grass at higher terrain
            Spawn(CornerGrassHigh, new Vector3(i + offset, height - 1, 0), Quaternion.identity, 3, 1, 1);
            dirtCount = 1;
            h--;
            h--;
            h--;
        }
        
        for (int j = 0; j < h; j++)
        {
            Spawn(Dirt, new Vector3(i + offset, j, 0), Quaternion.identity);
        }
        
        if (cylinder != null)
        {
            // Generate tree
            if (UnityEngine.Random.Range(0, 100) < 10)
            {
                GenerateTree(i + offset, height);
            }
        }
       
        
        if (Cave != null)
        {
            caveNoiseValue = _caveNoiseGenerator.GenerateNoise(i, caveNoiseOffset, caveNoiseScale);
            heightCave = Mathf.RoundToInt(caveNoiseValue * caveHeightScale);

            
            for (int j = heightCave; j <= height && j < heightCave + CaveScale; j++)
            {
                if (j <= height)
                {
                    Spawn(Cave, new Vector3(i + offset, j, 0), Quaternion.identity, 1, 1, 2, true);
                }

                if (CaveCorner != null && j == heightCave)
                {
                    int prevHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i - 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    int nextHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i + 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    
                    if (prevHeightCave < heightCave)
                    {
                        Spawn(CaveCorner, new Vector3(i + offset, j - 1, 0), Quaternion.Euler(0, 0, 180), 1, 1, 2, true);
                    }
                    if (nextHeightCave < heightCave)
                    {
                        Spawn(CaveCorner, new Vector3(i + offset, j - 1, 0), Quaternion.Euler(0, 0, 90), 1, 1, 2, true);
                    }
                }

                if (CaveCorner != null && j == heightCave + CaveScale - 1)
                {
                    int prevHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i - 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    int nextHeightCave = Mathf.RoundToInt(_caveNoiseGenerator.GenerateNoise(i + 1, caveNoiseOffset, caveNoiseScale) * caveHeightScale);
                    
                    if (prevHeightCave < heightCave)
                    {
                        Spawn(CaveCorner, new Vector3(i + offset - 1, j, 0), Quaternion.Euler(0, 0, 0), 1, 1, 2, true);
                    }
                    if (nextHeightCave < heightCave )
                    {
                        Spawn(CaveCorner, new Vector3(i + offset + 1, j, 0), Quaternion.Euler(0, 0, 270), 1, 1, 2, true);
                    }
                }
                
            }

        }

        
        if (dirtCount == 0)
        {
            int grassRandom = UnityEngine.Random.Range(0, 1);
            if (GrassMiddle2 != null)
            {
                switch (grassRandom)
                {
                    case 0:
                        // Spawn middle grass
                        Spawn(GrassMiddle, new Vector3(i + offset, h, 0), Quaternion.identity);
                        break;
                    case 1:
                        // Spawn alternative middle grass
                        Spawn(GrassMiddle2, new Vector3(i + offset, h, 0), Quaternion.identity);
                        break;
                }
            }
            else
            {
                // Spawn middle grass
                Spawn(GrassMiddle, new Vector3(i + offset, h, 0), Quaternion.identity);
            }
        }
        else
        {
            dirtCount--;
            Spawn(Dirt, new Vector3(i + offset, h, 0), Quaternion.identity);
        }

        if (prevHeight == height && height == nextHeight)
        {
            // Generate platforms if the terrain height remains the same
            offset += GeneratePlatforms(i + offset, h, Bridge != null);
        }

        if (Collectable != null)
        {
            if (UnityEngine.Random.Range(0, 100) < 10)
            {
                Spawn(Collectable, new Vector3(i + offset, h + 1, 0), Quaternion.identity, 1, 1, 3);
            }
        }
        
        // Check if the current height matches grass middle or grass middle 2
        bool isFlatSurface = (h == prevHeight && h == nextHeight) || (h == prevHeight + 1 && h == nextHeight + 1);
        

        // Spawn underwater objects randomly only on flat surfaces
        if (isFlatSurface)
        {
            // Adjust the probability of spawning an underwater object here
            if (UnityEngine.Random.Range(0, 100) < 20) // Adjust the percentage chance here
            {
                Vector3 spawnPosition = new Vector3(i + offset, h + 1, 0);
                if (!IsTooCloseToExisting(spawnPosition, underwaterObjectPositions, 1.5f)) // Adjust the minimum distance here
                {
                    int underwaterObjectIndex = UnityEngine.Random.Range(1, 5);
                    GameObject underwaterObject = null;
                    switch (underwaterObjectIndex)
                    {
                        case 1:
                            underwaterObject = UnderwaterObject1;
                            break;
                        case 2:
                            underwaterObject = UnderwaterObject2;
                            break;
                        case 3:
                            underwaterObject = UnderwaterObject3;
                            break;
                        case 4:
                            underwaterObject = UnderwaterObject4;
                            break;
                    }
                    if (underwaterObject != null)
                    {
                        Spawn(underwaterObject, spawnPosition, Quaternion.identity, 1, 1, 3);
                        underwaterObjectPositions.Add(spawnPosition);
                    }
                }
            }
        }
        
        bool isFlatSurface2 = (h == prevHeight && h == nextHeight) || (h == prevHeight + 1 && h == nextHeight + 1);

        // Spawn underwater objects randomly only on flat surfaces
        if (isFlatSurface2)
        {
            // Spawn coral reefs
            if (UnityEngine.Random.Range(0, 100) < 20) // Adjust the percentage chance here
            {
                Vector3 spawnPosition = new Vector3(i + offset, h + 1, 0);
                if (!IsTooCloseToExisting(spawnPosition, underwaterObjectPositions, 1f)) // Adjust the minimum distance here
                {
                    int coralReefIndex = UnityEngine.Random.Range(1, 4);
                    GameObject coralReef = null;
                    switch (coralReefIndex)
                    {
                        case 1:
                            coralReef = CoralReef1;
                            break;
                        case 2:
                            coralReef = CoralReef2;
                            break;
                        case 3:
                            coralReef = CoralReef3;
                            break;
                    }
                    if (coralReef != null)
                    {
                        Spawn(coralReef, spawnPosition, Quaternion.identity, 1, 1, 3);
                        underwaterObjectPositions.Add(spawnPosition);
                    }
                }
            }
        }


        
        
    }

    if (Water != null)
    {
        GenerateWater((int)width + offset, (int)WaterHeight);
    }
    
    if (Building != null)
    {
        GenerateBuildings();
    }
}

// Function to check if a new position is too close to existing positions
bool IsTooCloseToExisting(Vector3 newPosition, List<Vector3> existingPositions, float minDistance)
{
    foreach (Vector3 existingPosition in existingPositions)
    {
        if (Vector3.Distance(newPosition, existingPosition) < minDistance)
        {
            return true; // Too close to an existing position
        }
    }
    return false; // Not too close to any existing position
}

private void GenerateBuildings()
{
    for(int i = 0; i < BiomeWidth/4; i++)
    {
        //generate random number between 10 and 30
        int height = UnityEngine.Random.Range(2, 10);
        for (int j = 0; j < height; j++)
        {
            Spawn(Building, new Vector3(i * 4, j * 4, 0), Quaternion.identity, 4 , 4, -1);
        }
    }
}

private void GenerateWater(int width, int height)
{
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            Spawn(WaterBody, new Vector3(i, j, 0), Quaternion.identity, 1 , 1, -1);
        }
        Spawn(Water, new Vector3(i, height, 0), Quaternion.identity, 1 , 1, -1);
    }
}

private int GeneratePlatforms(int i, int h, bool isBridge = false)
{
    int starting = i;

    // Randomly decide whether to create a platform
    if (UnityEngine.Random.Range(0, 100) < 10)
    {
        i++;
        int gap = (int)UnityEngine.Random.Range(1, PlayerJumpDistance);
        int distanceRequiredToReachTopSpeed = (int) ((PlayerMaxSpeed * PlayerMaxSpeed) / (2 * PlayerAcceleration));
        int platformWidth = UnityEngine.Random.Range(distanceRequiredToReachTopSpeed - 5, distanceRequiredToReachTopSpeed + 5);

        for (int j = 0; j < h; j++)
        {
            if (DirtEdge2 != null)
            {
                int dirtRandom = UnityEngine.Random.Range(0, 1);
                switch (dirtRandom)
                {
                    case 0:
                        // Spawn dirt edge type 1
                        Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.identity);
                        break;
                    case 1:
                        // Spawn dirt edge type 2
                        Spawn(DirtEdge2, new Vector3(i, j, 0), Quaternion.identity);
                        break;
                }
            }
            else
            {
                Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.identity);
            }
        }

        // Spawn the edge of the grass platform
        Spawn(GrassPlatformEdge, new Vector3(i, h, 0), Quaternion.identity);
        
        //generate a random number 1 or 0
        int bridgeRandom = UnityEngine.Random.Range(0, 2);

        if (GrassPlatformLeft != null && GrassPlatformMiddle != null && GrassPlatformRight != null)
        {
            if (!isBridge || bridgeRandom == 0)
            {
                int platformHeight = (int)UnityEngine.Random.Range(1, PlayerJumpHeight);

                // Spawn the left part of the grass platform
                Spawn(GrassPlatformLeft, new Vector3(i + 1 + (gap), h + platformHeight, 0), Quaternion.identity);

                for (int k = 2; k < platformWidth; k++)
                {
                    // Spawn middle parts of the grass platform
                    Spawn(GrassPlatformMiddle, new Vector3(i + k + (gap), h + platformHeight, 0),
                        Quaternion.identity);
                }

                // Spawn the right part of the grass platform
                Spawn(GrassPlatformRight, new Vector3(i + platformWidth + (gap), h + platformHeight, 0),
                    Quaternion.identity);
            }
            else
            {
                for (int k = 0; k <= gap + platformWidth + 1; k++)
                {
                    // Spawn middle parts of the grass platform
                    Spawn(Bridge, new Vector3(i + k, h, 0), Quaternion.identity, 1, 1, -1);
                }
            }
        }

        i += 2 * gap + platformWidth + 1;

        for (int j = 0; j < h; j++)
        {
            if (DirtEdge2 != null)
            {
                int dirtRandom = UnityEngine.Random.Range(0, 1);
                switch (dirtRandom)
                {
                    case 0:
                        // Spawn inverted dirt edge type 1
                        Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                        break;
                    case 1:
                        // Spawn inverted dirt edge type 2
                        Spawn(DirtEdge2, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
                        break;
                }
            }
            else
            {
                Spawn(DirtEdge1, new Vector3(i, j, 0), Quaternion.Euler(180, 0, 180));
            }
        }
        // Spawn the inverted edge of the grass platform
        Spawn(GrassPlatformEdge, new Vector3(i, h, 0), Quaternion.Euler(180, 0, 180));
    }

    int offset = i - starting;
    return offset;
}

public void GenerateTree(int x, int y)
    {
        List<point> points = new List<point>();
        List<GameObject> branches = new List<GameObject>();
        
        rules.Clear();

        // Rules
        // Key is replaced by value
        rules.Add('F', rule);

        // Apply rules for i interations
        output = input;
        for (int i = 0; i < iterations; i++)
        {
            output = applyRules(output);
        }
        determinePoints(output, points);
        CreateCylinders(points, branches, x, y);
    }

    string applyRules(string p_input)
    {
        StringBuilder sb = new StringBuilder();
        // Loop through characters in the input string
        foreach (char c in p_input)
        {
            // If character matches key in rules, then replace character with rhs of rule
            if (rules.ContainsKey(c))
            {
                sb.Append(rules[c]);
            }
            // If not, keep the character
            else
            {
                sb.Append(c);
            }
        }
        // Return string with rules applied
        return sb.ToString();
    }

    struct point
    {
        public point(Vector3 rP, Vector3 rA, float rL) { Point = rP; Angle = rA; BranchLength = rL; }
        public Vector3 Point;
        public Vector3 Angle;
        public float BranchLength;
    }

    void determinePoints(string p_input, List<point> points)
    {
        Stack<point> returnValues = new Stack<point>();
        point lastPoint = new point(Vector3.zero, Vector3.zero, 1f);
        returnValues.Push(lastPoint);

        foreach (char c in p_input)
        {
            switch (c)
            {
                case 'F': // Draw line of length lastBranchLength, in direction of lastAngle
                    points.Add(lastPoint);

                    point newPoint = new point(lastPoint.Point + new Vector3(0, lastPoint.BranchLength, 0), lastPoint.Angle, 1f);
                    newPoint.BranchLength = lastPoint.BranchLength - 0.02f;
                    if (newPoint.BranchLength <= 0.0f) newPoint.BranchLength = 0.001f;

                    newPoint.Angle.y = lastPoint.Angle.y + UnityEngine.Random.Range(-30, 30);

                    newPoint.Point = pivot(newPoint.Point, lastPoint.Point, new Vector3(newPoint.Angle.x, 0, 0));
                    newPoint.Point = pivot(newPoint.Point, lastPoint.Point, new Vector3(0, newPoint.Angle.y, 0));

                    points.Add(newPoint);
                    lastPoint = newPoint;
                    break;
                case '+': // Rotate +30
                    lastPoint.Angle.x += angle;
                    break;
                case '[': // Save State
                    returnValues.Push(lastPoint);
                    break;
                case '-': // Rotate -30
                    lastPoint.Angle.x += -1 * angle;
                    break;
                case ']': // Load Saved State
                    lastPoint = returnValues.Pop();
                    break;
            }
        }
    }

    void CreateCylinders(List<point> points, List<GameObject> branches, int x, int y)
    {
        //make a tree game object
        GameObject tree = new GameObject();
        tree.name = "Tree";
        tree.transform.parent = transform;
        for (int i = 0; i < points.Count; i += 2)
        {
            CreateCylinder(points[i], points[i + 1], 0.1f, branches, tree);
        }
        
        tree.transform.position = new Vector3(x, y, 0);
    }

    // Pivot point1 around point2 by angles
    Vector3 pivot(Vector3 point1, Vector3 point2, Vector3 angles)
    {
        Vector3 dir = point1 - point2;
        dir = Quaternion.Euler(angles) * dir;
        point1 = dir + point2;
        return point1;
    }

    void CreateCylinder(point point1, point point2, float radius, List<GameObject> branches, GameObject tree)
    {
        //UnityEngine.Random.Range(0,3);
        GameObject newCylinder = (GameObject)Instantiate(cylinder);
        newCylinder.SetActive(true);
        float length = Vector3.Distance(point2.Point, point1.Point);
        radius = radius * length;

        Vector3 scale = new Vector3(radius, length / 2.0f, radius);
        newCylinder.transform.localScale = scale;

        newCylinder.transform.position = point1.Point;
        newCylinder.transform.Rotate(point2.Angle);

        newCylinder.transform.parent = tree.transform;

        branches.Add(newCylinder);
    }

}
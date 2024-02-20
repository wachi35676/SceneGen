using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BiomeTransition))]
public class BiomeTransistionGenerator:Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector() ;
        BiomeTransition myScript = (BiomeTransition)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}

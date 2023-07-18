using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mountains))]
public class MountainButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector() ;
        Mountains myScript = (Mountains)target;
        
        if (GUILayout.Button("Generate"))
        {
            myScript.GenerateMountains();
        }
        if (GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
    }
}
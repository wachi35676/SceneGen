using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneGenPrototype))]
public class SceneGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector() ;
        SceneGenPrototype myScript = (SceneGenPrototype)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
    }
}

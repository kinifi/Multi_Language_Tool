using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(M10NStringDatabase))]
public class M10NStringDatabaseEditor : Editor 
{
    public override void OnInspectorGUI()
    {

        M10NStringDatabase db = target as M10NStringDatabase;
        
        EditorGUILayout.LabelField("You don't wanna know...");

		DrawDefaultInspector();
    }
}

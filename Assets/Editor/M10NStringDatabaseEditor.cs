using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(M10NStringDatabase))]
public class M10NStringDatabaseEditor : Editor 
{

	private bool showDebugData = false;

    public override void OnInspectorGUI()
    {

        M10NStringDatabase db = target as M10NStringDatabase;
        
        
        
        showDebugData = EditorGUILayout.Toggle("Show Debug Data", showDebugData);

        if(showDebugData == true)
        {
			DrawDefaultInspector();
		}
    }
}

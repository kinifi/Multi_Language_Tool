using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(M10NText), true)]
[CanEditMultipleObjects]
public class M10NTextEditor : UnityEditor.UI.GraphicEditor
{
	SerializedProperty m_M10NDB;
	SerializedProperty m_M10NIndex;
	SerializedProperty m_M10NStringRef;
    SerializedProperty m_FontData;

    protected override void OnEnable()
    {
        base.OnEnable();
		m_M10NStringRef = serializedObject.FindProperty("m_reference");
		m_M10NDB = serializedObject.FindProperty("m_reference.m_db");
		m_M10NIndex = serializedObject.FindProperty("m_reference.m_index");
        m_FontData = serializedObject.FindProperty("m_FontData");
    }

	private void DoM10NStringReferenceGUI() {

		M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;

		string[] keyArray = db.keys.ToArray();

		//select the language you want to display
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Key:");
		GUI.changed = false;
		m_M10NIndex.intValue = EditorGUILayout.Popup(m_M10NIndex.intValue, keyArray);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField("Text:", EditorStyles.boldLabel);
		EditorGUILayout.LabelField(db.GetStringTable().values[m_M10NIndex.intValue].text);
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

		EditorGUILayout.PropertyField(m_M10NDB);

		DoM10NStringReferenceGUI();

        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}

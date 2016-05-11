using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(M10NText), true)]
[CanEditMultipleObjects]
public class M10NTextEditor : UnityEditor.UI.GraphicEditor
{
	SerializedProperty m_M10NDB;
	SerializedProperty m_M10NIndex;
	SerializedProperty m_M10NSelectedKey;
    SerializedProperty m_FontData;

	private bool keyMissing {
		get {
			M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;
			if(db != null) {
				return (db.keys.IndexOf(m_M10NSelectedKey.stringValue) < 0);
			}
			return false;
		}
	}

    protected override void OnEnable()
    {
        base.OnEnable();
		m_M10NDB = serializedObject.FindProperty("m_reference.m_db");
		m_M10NIndex = serializedObject.FindProperty("m_reference.m_index");
		m_M10NSelectedKey = serializedObject.FindProperty("m_reference.m_selectedKey");
        m_FontData = serializedObject.FindProperty("m_FontData");
    }

	private void DoM10NStringReferenceGUI() {

		M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;

		string[] keyArray = null;

		if( db != null ) {
			keyArray = db.keys.ToArray();
		} 

		//select the language you want to display
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Key:");
		if(keyArray != null) {
			GUI.changed = false;
			m_M10NIndex.intValue = EditorGUILayout.Popup(m_M10NIndex.intValue, keyArray);
			if(GUI.changed) {
				m_M10NSelectedKey.stringValue = db.keys[m_M10NIndex.intValue];
				MultiLanguage.SelectItemForKey(m_M10NSelectedKey.stringValue);
				//SceneView.RepaintAll();
			}
		}
		EditorGUILayout.EndHorizontal();
		if(keyMissing) {
			EditorGUILayout.LabelField("Key was:" + m_M10NSelectedKey.stringValue);
			if(GUILayout.Button("Add key to current database")) {
				db.AddTextEntry(m_M10NSelectedKey.stringValue);
				m_M10NIndex.intValue = db.keys.IndexOf(m_M10NSelectedKey.stringValue);
				EditorUtility.SetDirty(db);
			}
		}

		EditorGUILayout.LabelField("Text:", EditorStyles.boldLabel);
		if(db != null && !keyMissing) {
			EditorGUILayout.LabelField(db.GetStringTable().values[m_M10NIndex.intValue].text);
		}
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

		GUI.changed = false;
		EditorGUILayout.PropertyField(m_M10NDB);
		if(GUI.changed) {
			M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;
			if(db != null) {
				m_M10NIndex.intValue = db.keys.IndexOf(m_M10NSelectedKey.stringValue);
			}

			MultiLanguage.RepaintEditor();
		}

		DoM10NStringReferenceGUI();

        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}

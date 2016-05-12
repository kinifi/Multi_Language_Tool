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

	M10NStringTableKeyGUI m_keyGUI;

	private bool keyMissing {
		get {
			M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;
			if(db != null) {
				return !db.ContainsKey(m_M10NSelectedKey.stringValue);
			}
			return false;
		}
	}

    protected override void OnEnable()
    {
        base.OnEnable();

		Init();

		m_M10NDB = serializedObject.FindProperty("m_reference.m_db");
		m_M10NIndex = serializedObject.FindProperty("m_reference.m_index");
		m_M10NSelectedKey = serializedObject.FindProperty("m_reference.m_selectedKey");
        m_FontData = serializedObject.FindProperty("m_FontData");
		m_keyGUI.OnEnable();
    }

	protected override void OnDisable ()
	{
		m_keyGUI.OnDisable();
		base.OnDisable ();
	}

	public void OnLostFocus()
	{
		m_keyGUI.OnLostFocus();
	}

	private void Init() {
		if( m_keyGUI == null ) {
			m_keyGUI = new M10NStringTableKeyGUI();
			m_keyGUI.selectionChangedDelegate = KeyListSelectionChangedCallback;
		}
	}

	private void KeyListSelectionChangedCallback(int index, string key) {
		serializedObject.Update();

		m_M10NIndex.intValue = index;
		m_M10NSelectedKey.stringValue = key;

		serializedObject.ApplyModifiedProperties();

		StringTableEditorWindow.SelectItemForKey(key);
	}

	private void DoM10NStringReferenceGUI() {

		M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;

//		string[] keyArray = null;
//
//		if( db != null ) {
//			keyArray = db.keys.ToArray();
//		} 

		//select the language you want to display
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Key:");
//		if(keyArray != null) {
//			GUI.changed = false;
//			m_M10NIndex.intValue = EditorGUILayout.Popup(m_M10NIndex.intValue, keyArray);
//			if(GUI.changed) {
//				m_M10NSelectedKey.stringValue = db.keys[m_M10NIndex.intValue];
//				StringTableEditorWindow.SelectItemForKey(m_M10NSelectedKey.stringValue);
//				//SceneView.RepaintAll();
//			}
//		}

		m_keyGUI.OnKeyGUI(db, m_M10NSelectedKey.stringValue);

		EditorGUILayout.EndHorizontal();
		if(keyMissing) {
			EditorGUILayout.LabelField("Key was:" + m_M10NSelectedKey.stringValue);
			if(GUILayout.Button("Add key to current database")) {
				db.AddTextEntry(m_M10NSelectedKey.stringValue);
				m_M10NIndex.intValue = db.IndexOfKey(m_M10NSelectedKey.stringValue);
				EditorUtility.SetDirty(db);
			}
		}

		EditorGUILayout.LabelField("Text:", EditorStyles.boldLabel);
		if(db != null && !keyMissing) {
			EditorGUILayout.LabelField(db.current.values[m_M10NIndex.intValue].text);
		}
	}

    public override void OnInspectorGUI()
    {
		Init();

        serializedObject.Update();

		GUI.changed = false;
		EditorGUILayout.PropertyField(m_M10NDB);
		if(GUI.changed) {
			M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;
			if(db != null) {
				m_M10NIndex.intValue = db.IndexOfKey(m_M10NSelectedKey.stringValue);
			}

			StringTableEditorWindow.RepaintEditor();
		}

		DoM10NStringReferenceGUI();

        EditorGUILayout.PropertyField(m_FontData);
        AppearanceControlsGUI();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}

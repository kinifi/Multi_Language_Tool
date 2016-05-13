using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

		StringTableEditorWindow.SelectItemForKey(key, true);
	}
		

	private void DoM10NStringReferenceGUI() {

		M10NStringDatabase db = m_M10NDB.objectReferenceValue as M10NStringDatabase;

		//select the language you want to display
		EditorGUILayout.LabelField("Key", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();

		float vspace = EditorGUIUtility.standardVerticalSpacing;
		float lineH = EditorGUIUtility.singleLineHeight;

		GUILayoutUtility.GetRect(vspace, vspace, lineH, lineH);

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

		GUILayoutUtility.GetRect(vspace, vspace, lineH, lineH);

		EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
		if(db != null && !keyMissing) {
			EditorGUILayout.BeginHorizontal();
			GUILayoutUtility.GetRect(vspace, vspace, lineH, lineH);
			GUILayout.TextArea(db.current.values[m_M10NIndex.intValue].text, GUILayout.Height(60));
			//EditorGUILayout.LabelField(db.current.values[m_M10NIndex.intValue].text);
			EditorGUILayout.EndHorizontal();
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


	[MenuItem("GameObject/UI/Text Reference", false, 2001)]
	static public void AddM10NText(MenuCommand menuCommand)
	{
		GameObject go = new GameObject("Text");
		RectTransform rectTransform = go.AddComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(160f, 30f);

		M10NText lbl = go.AddComponent<M10NText>();
		lbl.color = Color.white;
		lbl.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

		PlaceUIElementRoot(go, menuCommand);
	}

	private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
	{
		GameObject parent = menuCommand.context as GameObject;
		if (parent == null || parent.GetComponentInParent<Canvas>() == null)
		{
			parent = GetOrCreateCanvasGameObject();
		}

		string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
		element.name = uniqueName;
		Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
		Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
		GameObjectUtility.SetParentAndAlign(element, parent);
		if (parent != menuCommand.context) // not a context click, so center in sceneview
			SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

		Selection.activeGameObject = element;
	}

	static GameObject GetOrCreateCanvasGameObject()
	{
		GameObject selectedGo = Selection.activeGameObject;

		// Try to find a gameobject that is the selected GO or one if its parents.
		Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
		if (canvas != null && canvas.gameObject.activeInHierarchy)
			return canvas.gameObject;

		// No canvas in selection or its parents? Then use just any canvas..
		canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
		if (canvas != null && canvas.gameObject.activeInHierarchy)
			return canvas.gameObject;

		// No canvas in the scene at all? Then create a new one.
		return CreateNewUI();
	}

	static private GameObject CreateNewUI()
	{
		// Root for the UI
		var root = new GameObject("Canvas");
		root.layer = LayerMask.NameToLayer("UI");
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<CanvasScaler>();
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

		// if there is no event system add one...
		CreateEventSystem(false,  null);
		return root;
	}

	private static void CreateEventSystem(bool select, GameObject parent)
	{
		var esys = Object.FindObjectOfType<EventSystem>();
		if (esys == null)
		{
			var eventSystem = new GameObject("EventSystem");
			GameObjectUtility.SetParentAndAlign(eventSystem, parent);
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();

			Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
		}

		if (select && esys != null)
		{
			Selection.activeGameObject = esys.gameObject;
		}
	}

	private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
	{
		// Find the best scene view
		SceneView sceneView = SceneView.lastActiveSceneView;
		if (sceneView == null && SceneView.sceneViews.Count > 0)
			sceneView = SceneView.sceneViews[0] as SceneView;

		// Couldn't find a SceneView. Don't set position.
		if (sceneView == null || sceneView.camera == null)
			return;

		// Create world space Plane from canvas position.
		Vector2 localPlanePosition;
		Camera camera = sceneView.camera;
		Vector3 position = Vector3.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
		{
			// Adjust for canvas pivot
			localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
			localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

			localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
			localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

			// Adjust for anchoring
			position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
			position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

			Vector3 minLocalPosition;
			minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
			minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

			Vector3 maxLocalPosition;
			maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
			maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

			position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
			position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
		}

		itemTransform.anchoredPosition = position;
		itemTransform.localRotation = Quaternion.identity;
		itemTransform.localScale = Vector3.one;
	}
}

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class MultiLanguage : EditorWindow {

	//the language database that is selected
	public M10NStringDatabase mLanguages;

	//currently editing Language
	public SystemLanguage mCurrentLanguage;

	//new language key and values to edit
	private string m_NewLanguageKey;
	private string m_NewLanguageValue;

	private SystemLanguage[] loadedLanguages;
	private string[] loadedLanguagesString;

	private Vector2 mScroll;

	private string mOriginTextValue;
	private string mTranslatedTextValue;

	private int keySelected;

	private Splitter m_LeftEditorSplitter;

	[MenuItem("Window/Multi-Language %l")]
	static void ShowEditor() {

		//create the editor window
		MultiLanguage editor = EditorWindow.GetWindow<MultiLanguage>();
		//the editor window must have a min size
		editor.minSize = new Vector2 (400, 300);
		//call the init method after we create our window
		editor.Init();

	}

	// Use this for initialization
	public void Init() 
	{
		ResetEditorStatus();

		m_LeftEditorSplitter = new Splitter(Splitter.SplitDirection.Horizontal, 150, 150);
	}

	public void ResetEditorStatus() {
		m_NewLanguageKey = string.Empty;
		m_NewLanguageValue = string.Empty;
		loadedLanguages = null;
		loadedLanguagesString = null;

		mScroll = Vector2.zero;

		mOriginTextValue = string.Empty;
		mTranslatedTextValue = string.Empty;

		keySelected = 0;
	}

	public void OnEnable ()
	{
		Init ();
		DetectLanguageFileFromSelection ();
	}
		
	void OnGUI()
	{
		//draw the main area 
		GUILayout.BeginArea(new Rect(0,0, position.width, position.height));

		//check if the current language is loaded or not
		if(mLanguages != null)
		{
			//the menu that will always display on top
			DoLanguageMenuBar();

			// do split here
			m_LeftEditorSplitter.DoResizeScrollView(new Rect(0, 0, position.width, position.height));
			int editorWidth = (int)position.width-(int)position.width/4;
			int commentWidth = (int)position.width/4;

			Rect leftPaneRect_listview = new Rect(0, 20, editorWidth, position.height-position.height/2.5f);
			Rect leftPaneRect_editorview = new Rect(0, position.height-position.height/3, editorWidth, position.height);
			Rect rightPaneRect = new Rect(editorWidth, 20, commentWidth, position.height);

			//display keys and values
			DoLanguageKeyValueListView(leftPaneRect_listview);

			//display edit field
			DoLanguageKeyValueEditor(leftPaneRect_editorview);

			//display edit field
			DoRightSideView(rightPaneRect);

			//remove notification if we were displaying one
			RemoveNotification();
		}
		else
		{
			//the language is not loaded
			ShowNotification(new GUIContent(EditorGUILayout.TextField("Select A Language To Edit"))); 
		}

		GUILayout.EndArea();

	}

	private void DoRightSideView(Rect paneRectSize)
	{

        GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);
        
        GUILayout.Label("show comment here");
        
        
        GUILayout.EndArea();

	}

	private void DoLanguageKeyValueListView(Rect paneRectSize)
	{
		Assert.IsNotNull(mLanguages);

        GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);

		M10NStringTable t = mLanguages.GetStringTable(mCurrentLanguage);

		// //create the box that shows the Titles Key & Values			
		// GUILayout.BeginHorizontal("HelpBox");
		// GUILayout.Label("Key");
		// GUILayout.Label("Value");
		// GUILayout.EndHorizontal();
		// //end of box that shows titles

		//start the scroll box here so values and keys can be scrollable
		mScroll = EditorGUILayout.BeginScrollView(mScroll);

		//being the vertical view of the key and values
		GUILayout.BeginVertical();
		float columnWidth = (paneRectSize.width / 2.0f) - 20.0f;
		for(int i=0; i < mLanguages.keys.Count; ++i) 
		{
			GUILayout.BeginHorizontal( ((keySelected == i) ? "SelectionRect" : "HelpBox" ));

			GUILayout.Label(mLanguages.keys[i], GUILayout.Width(columnWidth));
			GUILayout.Label(t.values[i].text, GUILayout.Width(columnWidth));

			GUILayout.EndHorizontal();
			if (Event.current.clickCount == 1 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
			{
				keySelected = i;
			}
		}
		GUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DoLanguageKeyValueEditor(Rect paneRectSize)
	{
		Assert.IsNotNull(mLanguages);

//			GUILayout.BeginHorizontal("HelpBox");
//			m_NewLanguageKey = EditorGUILayout.TextField("Key: ", m_NewLanguageKey);
//			m_NewLanguageValue = EditorGUILayout.TextField("Value: ", m_NewLanguageValue);
//
//			//add the language you have selected
//			if(GUILayout.Button("Add"))
//			{
//				mLanguages.SetTextEntry(mCurrentLanguage, m_NewLanguageKey.ToLower(), m_NewLanguageValue);
//				EditorUtility.SetDirty(mLanguages);
//			}
//
//			GUILayout.EndHorizontal();

		GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);

		GUILayout.Space(20);

		string key = string.Empty;
		bool isValidKeySelected = mLanguages.keys.Count > keySelected;
		if(isValidKeySelected) {
			key = mLanguages.keys[keySelected];
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Key:");
		GUI.changed = false;
		string newKey = EditorGUILayout.TextField(key);
		if(GUI.changed) {
			mLanguages.RenameTextEntryKey(key, newKey);
			EditorUtility.SetDirty(mLanguages);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Label("Source Text", EditorStyles.boldLabel);
		mOriginTextValue = EditorGUILayout.TextArea(mOriginTextValue, GUILayout.Height(80));

		GUILayout.Label("Translated Text", EditorStyles.boldLabel);
		if(isValidKeySelected) {
			mTranslatedTextValue = mLanguages.GetStringTable(mCurrentLanguage).values[keySelected].text;
		}
		GUI.changed = false;
		mTranslatedTextValue = EditorGUILayout.TextArea(mTranslatedTextValue, GUILayout.Height(80));
		if(GUI.changed) {
			mLanguages.SetTextEntry(mCurrentLanguage, key, mTranslatedTextValue);
			EditorUtility.SetDirty(mLanguages);
		}

		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.EndArea();
	}

	//create the top menu bar
	private void DoLanguageMenuBar ()
	{
		//the menu bar 
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));

		//display the origin language selection
		DoLanguageOriginPopup();

		//select the language you want to display
		DoLanguageSelectionPopup();
		//Debug.Log(currentLoadedLanguageListSelection);

		EditorGUILayout.Space();

		//display the add key button in the toolbar
		DoAddKey();

		//export the selected language
		DoExportLanguageFileButton();
		
		//select to get a .po file and import to the language asset
		DoImportLanguageFileButton();

		GUILayout.EndHorizontal();

	}

	public void DoAddKey()
	{
		
		m_NewLanguageKey = EditorGUILayout.TextField("Key Name: ", m_NewLanguageKey, EditorStyles.toolbarTextField, GUILayout.Width(300));

		if(GUILayout.Button("Add New Key", EditorStyles.toolbarButton, GUILayout.Width(80)))
		{
			//start exporting language File here
			mLanguages.AddTextEntry(m_NewLanguageKey.ToLower());
			
			//clear the text box
			m_NewLanguageKey = "";
			//Debug.Log("Added New Key");
		}



	}

	//exports the language translations to a .po file
	public void DoExportLanguageFileButton ()
	{
		if(GUILayout.Button("Export", EditorStyles.toolbarButton, GUILayout.Width(50)))
		{
			//start exporting language File here
		}

	}

	//opens up a file selection for a .po file to import
	public void DoImportLanguageFileButton()
	{
		if(GUILayout.Button("Import", EditorStyles.toolbarButton, GUILayout.Width(50)))
		{
			//start exporting language File here
		}

	}

	public void DoLanguageOriginPopup()
	{

		GUILayout.Label("Origin:", GUILayout.Width(60));

		if(mLanguages == null)
		{
			string[] nodata = {""};
			EditorGUILayout.Popup(0, nodata, EditorStyles.toolbarPopup);
			return;
		}

		if( loadedLanguages == null || loadedLanguages.Length != mLanguages.languageCount ) {
			loadedLanguages = mLanguages.languages;
			loadedLanguagesString = new string[loadedLanguages.Length];
			for(int i = 0; i < loadedLanguages.Length; ++i) {
				loadedLanguagesString[i] = loadedLanguages[i].ToString();
			}
		}

		int selectionIndex = 0;
		for(int i = 0; i < loadedLanguages.Length; ++i) {
			if(loadedLanguages[i] == mCurrentLanguage) {
				selectionIndex = i;
			}
		}

		//select the language you want to display
		selectionIndex = EditorGUILayout.Popup(selectionIndex, loadedLanguagesString, EditorStyles.toolbarPopup, GUILayout.Width(100));
		mCurrentLanguage = loadedLanguages[selectionIndex];
	}

	public void DoLanguageSelectionPopup()
	{

		GUILayout.Label("Translated:", GUILayout.Width(70));

		if(mLanguages == null)
		{
			string[] nodata = {""};
			EditorGUILayout.Popup(0, nodata, EditorStyles.toolbarPopup);
			return;
		}

		if( loadedLanguages == null || loadedLanguages.Length != mLanguages.languageCount ) {
			loadedLanguages = mLanguages.languages;
			loadedLanguagesString = new string[loadedLanguages.Length];
			for(int i = 0; i < loadedLanguages.Length; ++i) {
				loadedLanguagesString[i] = loadedLanguages[i].ToString();
			}
		}

		int selectionIndex = 0;
		for(int i = 0; i < loadedLanguages.Length; ++i) {
			if(loadedLanguages[i] == mCurrentLanguage) {
				selectionIndex = i;
			}
		}

		//select the language you want to display
		selectionIndex = EditorGUILayout.Popup(selectionIndex, loadedLanguagesString, EditorStyles.toolbarPopup, GUILayout.Width(100));
		mCurrentLanguage = loadedLanguages[selectionIndex];
	}

	public void DetectLanguageFileFromSelection ()
	{
		M10NStringDatabase selectedAsset = null;

		if (Selection.activeObject == null)
		{
			mLanguages = null;
		}

		if (Selection.activeObject is M10NStringDatabase && EditorUtility.IsPersistent(Selection.activeObject))
		{
			selectedAsset = Selection.activeObject as M10NStringDatabase;
		}

		if (Selection.activeGameObject)
		{
			M10NText m10ntext = Selection.activeGameObject.GetComponent<M10NText>();
			if (m10ntext)
			{
				selectedAsset = m10ntext.database;
			}
		}
				
		if (selectedAsset != null && selectedAsset != mLanguages)
		{
			ResetEditorStatus();
			mLanguages = selectedAsset;
		}
	}

	public void OnFocus ()
	{
		DetectLanguageFileFromSelection();
	}


	public void OnProjectChange ()
	{
		DetectLanguageFileFromSelection ();
	}

	public void OnSelectionChange ()
	{
		DetectLanguageFileFromSelection();
		Repaint();
	}

	public void OnLostFocus () 
	{

	}

}

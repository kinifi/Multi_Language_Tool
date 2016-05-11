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
	public SystemLanguage mCurrentReferenceLanguage;

	//new language key and values to edit
	private string m_NewLanguageKey;
	private string m_NewLanguageValue;

	private SystemLanguage[] loadedLanguages;
	private string[] loadedLanguagesString;

	private Vector2 mScroll;

	private string mReferenceTextValue;
	private string mTextValue;

	private int keySelected;

	private SplitterState mHorizontalSplitterState;
	private SplitterState mVerticalSplitterState;

	private TreeViewState m_stringTableListViewState;
	private M10NStringTableListView m_StringTableListView;

	private bool m_isInitialized;

	public class Styles {
		public readonly int kToolbarHeight = 20;
		public readonly int kEditorPaneHeight = 400;
	}
	static Styles s_Styles;

	[MenuItem("Window/Multi-Language %l")]
	static void ShowEditor() {

		//create the editor window
		MultiLanguage editor = EditorWindow.GetWindow<MultiLanguage>();
		//the editor window must have a min size
		editor.minSize = new Vector2 (400, 300);
		//call the init method after we create our window
		editor.Init();

	}

	public static void RepaintEditor() {
		MultiLanguage editor = EditorWindow.GetWindow<MultiLanguage>();
		editor.DetectLanguageFileFromSelection ();
		editor.Repaint();
	}

	public static void SelectItemForKey(string key) {
		MultiLanguage editor = EditorWindow.GetWindow<MultiLanguage>();
		editor.DetectLanguageFileFromSelection ();
		editor._SelectItemForKey(key);
		editor.Repaint();
	}

	private void _SelectItemForKey(string key) {
		if (m_StringTableListView != null) {
			m_StringTableListView.SelectItemForKey(key);
		}
	}

	// Use this for initialization
	public void Init() 
	{
		if( m_stringTableListViewState == null || m_StringTableListView == null ) {
			m_isInitialized = false;
		}

		if(m_isInitialized) {
			return;
		}

		if (m_stringTableListViewState == null) {
			m_stringTableListViewState = new TreeViewState();
		}

		if( m_StringTableListView == null ) {
			m_StringTableListView = new M10NStringTableListView(this, m_stringTableListViewState, mLanguages);
		}

		if( mHorizontalSplitterState == null ) {
			mHorizontalSplitterState = new SplitterState(new int[] { 200, 100 }, null, null);
		}

		if( mVerticalSplitterState == null ) {
			mVerticalSplitterState = new SplitterState(new int[] { 200, 100 }, null, null);
		}

		ResetEditorStatus();

		m_isInitialized = true;
	}

	public void ResetEditorStatus() {
		m_NewLanguageKey = string.Empty;
		m_NewLanguageValue = string.Empty;
		loadedLanguages = null;
		loadedLanguagesString = null;

		mScroll = Vector2.zero;

		mReferenceTextValue = string.Empty;
		mTextValue = string.Empty;

		keySelected = -1;
	}

	public void OnEnable ()
	{
		Init ();
		DetectLanguageFileFromSelection ();
	}
		
	void OnGUI()
	{
		Init();

		if(s_Styles == null) {
			s_Styles = new Styles();
		}

		//draw the main area 
		GUILayout.BeginArea(new Rect(0,0, position.width, position.height));

		//check if the current language is loaded or not
		if(mLanguages != null)
		{
			Rect menubarRect   = new Rect(0,0,position.width, s_Styles.kToolbarHeight);

			//the menu that will always display on top
			DoLanguageMenuBar(menubarRect);

			// Do layouting
			SplitterGUILayout.BeginHorizontalSplit(mHorizontalSplitterState, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical ();
			SplitterGUILayout.BeginVerticalSplit(mVerticalSplitterState, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.BeginHorizontal ();
			GUILayout.EndHorizontal ();
			SplitterGUILayout.EndVerticalSplit();
			GUILayout.EndVertical ();
			SplitterGUILayout.EndHorizontalSplit();

			// do split here
//			m_LeftEditorSplitter.DoResizeScrollView(new Rect(0, 0, position.width, position.height));
//			int editorWidth = (int)position.width-(int)position.width/4;
//			int commentWidth = (int)position.width/4;
			int editorWidth = (int)mHorizontalSplitterState.realSizes[0];
			int commentWidth = (int)mHorizontalSplitterState.realSizes[1];

			int listViewHeight = (int)mVerticalSplitterState.realSizes[0];
			int editorHeight = (int)mVerticalSplitterState.realSizes[1];

//			Rect leftPaneRect_listview = new Rect(0, s_Styles.kToolbarHeight, editorWidth, position.height- s_Styles.kEditorPaneHeight - s_Styles.kToolbarHeight);
//			Rect leftPaneRect_editorview = new Rect(0, leftPaneRect_listview.y + leftPaneRect_listview.height, editorWidth, s_Styles.kEditorPaneHeight);
//			Rect rightPaneRect = new Rect(editorWidth, s_Styles.kToolbarHeight, commentWidth, position.height - s_Styles.kToolbarHeight);
			Rect leftPaneRect_listview = new Rect(0, s_Styles.kToolbarHeight, editorWidth, listViewHeight - s_Styles.kToolbarHeight);
			Rect leftPaneRect_editorview = new Rect(0, leftPaneRect_listview.height + s_Styles.kToolbarHeight, editorWidth, editorHeight);

			Rect rightPaneRect = new Rect(editorWidth, s_Styles.kToolbarHeight, commentWidth, position.height - s_Styles.kToolbarHeight);


			//display keys and values
			DoLanguageKeyValueListView(leftPaneRect_listview);

			//EditorGUI.DrawRect(leftPaneRect_listview, Color.yellow);

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
		Assert.IsNotNull(mLanguages);

        GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);
        
        GUILayout.Label("show comment here");
        
        
        GUILayout.EndArea();

	}

	private void DoLanguageKeyValueListView(Rect paneRectSize)
	{
		Assert.IsNotNull(mLanguages);

		if (m_StringTableListView != null) {
			m_StringTableListView.OnGUI(paneRectSize);
		}
	}

	private void DoLanguageKeyValueEditor(Rect paneRectSize)
	{
		Assert.IsNotNull(mLanguages);

		GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);

		GUILayout.Space(20);

		string key = string.Empty;
		bool isValidKeySelected = mLanguages.keys.Count > keySelected && keySelected >= 0;
		if(isValidKeySelected) {
			key = mLanguages.keys[keySelected];
			mReferenceTextValue = mLanguages.GetStringTable(mCurrentReferenceLanguage).values[keySelected].text;
			mTextValue = mLanguages.GetStringTable(mCurrentLanguage).values[keySelected].text;
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Key:");
		GUI.changed = false;
		string newKey = EditorGUILayout.TextField(key);
		if(GUI.changed) {
			mLanguages.RenameTextEntryKey(key, newKey);
			m_StringTableListView.ReloadTree ();
			EditorUtility.SetDirty(mLanguages);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();
		GUILayout.Label("Reference Text", EditorStyles.boldLabel);

		using (new EditorGUI.DisabledScope(true)) {
			mReferenceTextValue = EditorGUILayout.TextArea(mReferenceTextValue, GUILayout.Height(80));
		}

		GUILayout.Label("Text", EditorStyles.boldLabel);

		GUI.changed = false;
		mTextValue = EditorGUILayout.TextArea(mTextValue, GUILayout.Height(80));
		if(GUI.changed) {
			mLanguages.SetTextEntry(mCurrentLanguage, key, mTextValue);
			m_StringTableListView.ReloadTree ();
			EditorUtility.SetDirty(mLanguages);
		}
			
		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.EndArea();
	}

	//create the top menu bar
	private void DoLanguageMenuBar (Rect menubarRect)
	{
		GUILayout.BeginArea(menubarRect, EditorStyles.toolbar);

		//the menu bar 
		GUILayout.BeginHorizontal();

		//display the origin language selection
		DoLanguagePopup();

		//select the language you want to display
		DoReferenceLanguagePopup();
		//Debug.Log(currentLoadedLanguageListSelection);

		EditorGUILayout.Space();

		//display the add key button in the toolbar
		DoAddKey();

		//export the selected language
		DoExportLanguageFileButton();
		
		//select to get a .po file and import to the language asset
		DoImportLanguageFileButton();

		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	public void DoAddKey()
	{
		
		m_NewLanguageKey = EditorGUILayout.TextField("Key Name: ", m_NewLanguageKey, EditorStyles.toolbarTextField, GUILayout.Width(300));

		if(GUILayout.Button("Add New Key", EditorStyles.toolbarButton, GUILayout.Width(80)))
		{
			//start exporting language File here
			mLanguages.AddTextEntry(m_NewLanguageKey.ToLower());
			m_StringTableListView.ReloadTree ();
			EditorUtility.SetDirty(mLanguages);

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

	public void DoLanguagePopup()
	{
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
		m_StringTableListView.OnEditingLanguageChanged(mCurrentLanguage);
	}

	public void DoReferenceLanguagePopup()
	{
		GUILayout.Label("Reference:", GUILayout.Width(70));

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
			if(loadedLanguages[i] == mCurrentReferenceLanguage) {
				selectionIndex = i;
			}
		}

		//select the language you want to display
		selectionIndex = EditorGUILayout.Popup(selectionIndex, loadedLanguagesString, EditorStyles.toolbarPopup, GUILayout.Width(100));
		mCurrentReferenceLanguage = loadedLanguages[selectionIndex];
	}

	public void OnStringTableSelectionChanged(int[] selection) {
		if(selection != null && selection.Length > 0) {
			keySelected = selection[0];
		} else {
			keySelected = -1;
		}	
	}

	public void DetectLanguageFileFromSelection ()
	{
		M10NStringDatabase selectedAsset = null;
		string focusKey = null;

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
				focusKey = m10ntext.stringReference.key;
			}
		}
				
		if (selectedAsset != null && selectedAsset != mLanguages)
		{
			ResetEditorStatus();
			mLanguages = selectedAsset;
			if (m_StringTableListView != null) {
				m_StringTableListView.OnM10NStringDatabaseChanged(selectedAsset);
				//m_StringTableListView.InitSelection (true);
			}
		}

		if( focusKey != null ) {
			if (m_StringTableListView != null) {
				m_StringTableListView.SelectItemForKey(focusKey);
			}
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
		EndRenaming();
	}

	void PlaymodeChanged()
	{
		if (mLanguages != null)
		{
			Repaint();
		}

		EndRenaming();
	}

	void EndRenaming()
	{
		if (m_StringTableListView != null) {
			m_StringTableListView.EndRenaming();
		}
	}

	public void UndoRedoPerformed()
	{
		if (mLanguages == null) {
			return;
		}

		// Undo may have deleted one of the selected groups
//		m_Controller.SanitizeGroupViews ();
//		m_Controller.OnUnitySelectionChanged ();
//		m_Controller.OnSubAssetChanged ();

		if (m_StringTableListView != null) {
			m_StringTableListView.OnUndoRedoPerformed ();
		}

//		AudioMixerUtility.RepaintAudioMixerAndInspectors ();
	}

	void OnProjectChanged ()
	{
		if (m_StringTableListView == null) {
			Init ();
		} else {
			m_StringTableListView.ReloadTree ();
		}
	}
}

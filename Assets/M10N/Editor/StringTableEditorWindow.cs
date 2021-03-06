﻿using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class StringTableEditorWindow : EditorWindow {

	//the language database that is selected
	public M10NStringDatabase mLanguages;

	//currently editing Language
	public SystemLanguage mCurrentLanguage;
	public SystemLanguage mCurrentReferenceLanguage;

	private SystemLanguage[] loadedLanguages;
	private string[] loadedLanguagesString;

	private string mReferenceTextValue;
	private string mTextValue;

	private int keySelected;
	private bool mShowReference;
	private bool mShowComment;

	private int newKeyIncrementer;

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

	[MenuItem("Window/StringTable %l")]
	static void ShowEditor() {

		//create the editor window
		StringTableEditorWindow editor = EditorWindow.GetWindow<StringTableEditorWindow>();
		//the editor window must have a min size
		editor.titleContent = new GUIContent("StringTable");
		editor.minSize = new Vector2 (800, 600);
		//call the init method after we create our window
		editor.Init();

	}

	public static void RepaintEditor() {
		StringTableEditorWindow editor = EditorWindow.GetWindow<StringTableEditorWindow>();
		editor.DetectLanguageFileFromSelection ();
		editor.Repaint();
	}

	public static void SelectItemForKey(string key, bool reload) {
		StringTableEditorWindow editor = EditorWindow.GetWindow<StringTableEditorWindow>();
		editor.DetectLanguageFileFromSelection ();
		editor._SelectItemForKey(key, reload);
		editor.Repaint();
	}

	private void _SelectItemForKey(string key, bool reload) {
		if (m_StringTableListView != null) {
			if(reload) {
				m_StringTableListView.ReloadTree();
			}
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

		mCurrentLanguage = EditorSettings.editorPreviewLanguage;

		loadedLanguages = null;
		loadedLanguagesString = null;

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
			if( mShowComment ) {
				SplitterGUILayout.BeginHorizontalSplit(mHorizontalSplitterState, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				GUILayout.BeginVertical ();
			}
			SplitterGUILayout.BeginVerticalSplit(mVerticalSplitterState, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.BeginHorizontal ();
			GUILayout.EndHorizontal ();
			SplitterGUILayout.EndVerticalSplit();
			if( mShowComment ) {
				GUILayout.EndVertical ();
				SplitterGUILayout.EndHorizontalSplit();
			}

			// do split here
			int editorWidth = (mShowComment) ? (int)mHorizontalSplitterState.realSizes[0] : (int)position.width;
			int commentWidth = (int)mHorizontalSplitterState.realSizes[1];

			int listViewHeight = (int)mVerticalSplitterState.realSizes[0];
			int editorHeight = (int)mVerticalSplitterState.realSizes[1];

			Rect leftPaneRect_listview = new Rect(0, s_Styles.kToolbarHeight, editorWidth, listViewHeight - s_Styles.kToolbarHeight);
			Rect leftPaneRect_editorview = new Rect(0, leftPaneRect_listview.height + s_Styles.kToolbarHeight, editorWidth, editorHeight);

			Rect rightPaneRect = new Rect(editorWidth, s_Styles.kToolbarHeight, commentWidth, position.height - s_Styles.kToolbarHeight);


			//display keys and values
			DoLanguageKeyValueListView(leftPaneRect_listview);

			//EditorGUI.DrawRect(leftPaneRect_listview, Color.yellow);

			//display edit field
			DoLanguageKeyValueEditor(leftPaneRect_editorview);

			//display edit field
			if( mShowComment ) {
				DoRightSideView(rightPaneRect);
			}

			//remove notification if we were displaying one
			RemoveNotification();
		}
		else
		{
			//the language is not loaded
			ShowNotification(new GUIContent("Select A Language To Edit")); 
		}

		GUILayout.EndArea();

	}

	private void DoRightSideView(Rect paneRectSize)
	{
		Assert.IsNotNull(mLanguages);

        GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);
        
        GUILayout.Label("comment displays here", "HelpBox");
        GUILayout.TextArea("write a new comment here. When Saved, this will overwrite the above comment", GUILayout.Height(100));
        
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

		Rect buttonAddRect = paneRectSize;

		buttonAddRect.height = 20;
		paneRectSize.yMin += buttonAddRect.height; 

		//EditorGUI.DrawRect(buttonAddRect, Color.blue);

		GUILayout.BeginArea(buttonAddRect);
		GUILayout.Space(2);
		DoAddKey();
		GUILayout.EndArea();

		buttonAddRect.height = 1.0f;
		EditorGUI.DrawRect(buttonAddRect, Color.gray);

		GUILayout.BeginArea(paneRectSize, EditorStyles.helpBox);

		GUILayout.Space(2);

		string key = string.Empty;
		bool isValidKeySelected = mLanguages.Count > keySelected && keySelected >= 0;
		if(isValidKeySelected) {
			key = mLanguages[keySelected];
			mReferenceTextValue = mLanguages[mCurrentReferenceLanguage].values[keySelected].text;
			mTextValue = mLanguages[mCurrentLanguage].values[keySelected].text;
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Key", EditorStyles.boldLabel);
		GUI.changed = false;
		string newKey = EditorGUILayout.TextField(key);
		if(GUI.changed) {
			mLanguages.RenameTextEntryKey(key, newKey);
			SetDatabaseDirty();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();

		if( mShowReference ) {
			GUILayout.Label("Reference Text", EditorStyles.boldLabel);

			using (new EditorGUI.DisabledScope(true)) {
				mReferenceTextValue = EditorGUILayout.TextArea(mReferenceTextValue, GUILayout.Height(40));
			}
		}

		GUILayout.Label("Text", EditorStyles.boldLabel);

		GUI.changed = false;
		mTextValue = EditorGUILayout.TextArea(mTextValue, GUILayout.Height(60));
		if(GUI.changed) {
			mLanguages.SetTextEntry(mCurrentLanguage, key, mTextValue);
			SetDatabaseDirty();
		}
			
		GUILayout.EndVertical();

		EditorGUILayout.Space();

		GUILayout.EndArea();
	}

	private void SetDatabaseDirty() {

		EditorUtility.SetDirty(mLanguages);

		if(m_StringTableListView != null) {
			m_StringTableListView.ReloadTree ();
		}

		InspectorWindow.RepaintAllInspectors();
		M10NText[] texts = FindObjectsOfType(typeof(M10NText)) as M10NText[];
		foreach(M10NText t in texts) {
			t.SetVerticesDirty();
		}
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

		GUILayout.FlexibleSpace();

		DoShowCommentButton();

		DoContextMenuButton();

		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void DoAddKey()
	{
		GUILayout.BeginHorizontal();

		if(GUILayout.Button(" ", "OL Plus", GUILayout.Width(80)))
		{
			string newKeyName = "new_key_" + (++newKeyIncrementer);

			while(mLanguages.ContainsKey(newKeyName)) {
				newKeyName = "new_key_" + (++newKeyIncrementer);
			}

			//start exporting language File here
			mLanguages.AddTextEntry(newKeyName);
			m_StringTableListView.ReloadTree ();
			m_StringTableListView.SelectItemForKey(newKeyName);
			EditorUtility.SetDirty(mLanguages);
		}

		GUILayout.EndHorizontal();
	}

	private void DoContextMenuButton ()
	{
		if(GUILayout.Button("▼", EditorStyles.toolbarButton, GUILayout.Width(20)))
		{
			GenericMenu pm = new GenericMenu();

			pm.AddItem(new GUIContent("Import from .po ..."), false, PerformImportLanguageFile);

			if( mShowReference ) {
				pm.AddItem(new GUIContent("Export to .po ..."), false, PerformExportLanguageFile);
			} else {
				pm.AddDisabledItem (new GUIContent("Export to .po ..."));
				pm.AddSeparator("/");
				pm.AddDisabledItem (new GUIContent("To export, enable Reference"));
			}


			pm.ShowAsContext();
		}
	}

	private void PerformImportLanguageFile ()
	{
		//start parsing the PO File
		//get the file path
		var newPath = EditorUtility.OpenFilePanel(
			"Select PO file",
			"",
			"po");
		if(newPath.Length == 0) {
			return;
		}

		//start importing the file
		M10NPOCreator.ImportFile(mLanguages, newPath, mCurrentReferenceLanguage);

		SetDatabaseDirty();
	}

	//exports the language translations to a .po file
	private void PerformExportLanguageFile ()
	{
		//check if the mLanguages file is null
		Assert.IsNotNull(mLanguages);

		//prompt user to export the file at a location
		var path = EditorUtility.SaveFilePanel(
				"Save Language as .po",
				"",
				mCurrentLanguage.ToString() + ".po",
				"po");
		//check if the file path is zero 
		if(path.Length == 0) {
			return;
		}

		//get the current language selected
		M10NPOCreator.ExportFile(mLanguages, mCurrentLanguage, mCurrentReferenceLanguage, path);
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

	public void DoShowCommentButton() {
		mShowComment = GUILayout.Toggle(mShowComment, new GUIContent("Comment"), EditorStyles.toolbarButton, GUILayout.Width(60));
	}

	public void DoReferenceLanguagePopup()
	{
		if(mLanguages == null)
		{
			string[] nodata = {""};
			EditorGUILayout.Popup(0, nodata, EditorStyles.toolbarPopup);
			return;
		}

		mShowReference = GUILayout.Toggle(mShowReference, new GUIContent("Reference"), EditorStyles.toolbarButton, GUILayout.Width(60));
		if( mShowReference ) {
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
			if(keySelected >= 0 && mLanguages != null) {
				focusKey = mLanguages[keySelected];
			}
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

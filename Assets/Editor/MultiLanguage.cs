using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MultiLanguage : EditorWindow {

	//the language database that is selected
	public M10NStringDatabase mLanguages;

	//currently editing Language
	public SystemLanguage mCurrentLanguage;

	//new language key and values to edit
	private string m_NewLanguageKey, m_NewLanguageValue;

	private SystemLanguage[] loadedLanguages;
	private string[] loadedLanguagesString;

	private Vector2 mScroll;

	private string mTranslatedTextValue, mOriginTextValue;

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
		DetectLanguageFileFromSelection();
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
			LanguageMenuBar();

			AddLanguageKeyValues ();

			//display keys and values
			LanguageKeyValueDisplay();
		}
		else
		{
			//the language is not loaded
			GUILayout.Label("Select Language Asset to Edit", "CN EntryError");
		}

		GUILayout.EndArea();

	}

	private void LanguageKeyValueDisplay()
	{

		if(mLanguages != null) {
			M10NStringTable t = mLanguages.GetStringTable(mCurrentLanguage);
			
			GUILayout.BeginHorizontal("HelpBox");

			GUILayout.Label("Key");
			GUILayout.Label("Value");

			GUILayout.EndHorizontal();

			mScroll = EditorGUILayout.BeginScrollView(mScroll);

			GUILayout.BeginVertical();
			for(int i=0; i < mLanguages.keys.Count; ++i) 
			{
				
				GUILayout.BeginHorizontal();
				
				if(GUILayout.Button(mLanguages.keys[i] + " | " + t.values[i].text, "OL Title"))
				{
					
					
				}

				GUILayout.EndHorizontal();

				// GUILayout.Label("Key: " + mLanguages.keys[i]
				// 	+ " | Value: " + t.values[i].text
				// , "GroupBox", GUILayout.Width(position.width - 40));

			}
			GUILayout.EndVertical();

			EditorGUILayout.EndScrollView();

			GUILayout.BeginVertical();

			GUILayout.Label("Source Text");
			mOriginTextValue = EditorGUILayout.TextArea(mOriginTextValue, GUILayout.Height(80));

			GUILayout.Label("Translated Text");
			mTranslatedTextValue = EditorGUILayout.TextArea(mTranslatedTextValue, GUILayout.Height(80));

			GUILayout.Label("Comments from the .po files will go here");

			GUILayout.EndVertical();

			EditorGUILayout.Space();
		}

		
		// if(mLanguages != null) {
		// 	M10NStringTable t = mLanguages.GetStringTable(mCurrentLanguage);

		// 	for(int i=0; i < mLanguages.keys.Count; ++i) {
		// 		GUILayout.Label("Key: " + mLanguages.keys[i]
		// 			+ " | Value: " + t.values[i].text
		// 		);
		// 	}
		// }
	}

	private void AddLanguageKeyValues ()
	{

		GUILayout.BeginHorizontal("HelpBox");
		m_NewLanguageKey = EditorGUILayout.TextField("Key: ", m_NewLanguageKey);
		m_NewLanguageValue = EditorGUILayout.TextField("Value: ", m_NewLanguageValue);

		//add the language you have selected
		if(GUILayout.Button("Add"))
		{
			mLanguages.SetTextEntry(mCurrentLanguage, m_NewLanguageKey.ToLower(), m_NewLanguageValue);
			EditorUtility.SetDirty(mLanguages);
		}

		GUILayout.EndHorizontal();
	}

	//create the top menu bar
	private void LanguageMenuBar ()
	{

		//the menu bar 
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));

		//select the language you want to display
		DoLanguageSelectionPopup();
		//Debug.Log(currentLoadedLanguageListSelection);

		EditorGUILayout.Space();

		GUILayout.EndHorizontal();

	}

	public void DoLanguageSelectionPopup()
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
		selectionIndex = EditorGUILayout.Popup(selectionIndex, loadedLanguagesString, EditorStyles.toolbarPopup, GUILayout.Width(250));
		mCurrentLanguage = loadedLanguages[selectionIndex];
	}

	public void DetectLanguageFileFromSelection ()
	{
		if (Selection.activeObject == null)
		{
			mLanguages = null;
		}

		if (Selection.activeObject is M10NStringDatabase && EditorUtility.IsPersistent(Selection.activeObject))
		{
			mLanguages = Selection.activeObject as M10NStringDatabase;
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

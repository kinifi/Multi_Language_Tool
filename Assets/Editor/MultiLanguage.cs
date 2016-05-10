using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MultiLanguage : EditorWindow {

	//the language database that is selected
	public M10NStringDatabase mLanguages;

	public bool isLoaded = false;

	//currently editing Languages
	public SystemLanguage mCurrentLanguage;

	//enum of the avaliable languages
	public SystemLanguage mSystemLanguage;
	public SystemLanguage mNewSystemLanguage;

	//new language key and values to edit
	private string m_NewLanguageKey, m_NewLanguageValue;

	private SystemLanguage[] loadedLanguages;
	private string[] loadedLanguagesString;

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
		if(isLoaded == true)
		{
			//the menu that will always display on top
			LanguageMenuBar();

			if(mLanguages.languageCount > 0)
			{
				//display the buttons and text boxes so you can add key and values
				AddLanguageKeyValues();

				//display keys and values
				LanguageKeyValueDisplay();
			}
			else
			{
				GUILayout.Label("Create A New Language");
			}

		}
		else
		{
			//the language is not loaded
			GUILayout.Label("No Language Asset Selected", "CN EntryError");

		}

		GUILayout.EndArea();

	}

	private void LanguageKeyValueDisplay()
	{
		if(mLanguages != null) {
			M10NStringTable t = mLanguages.GetStringTable(mCurrentLanguage);

			for(int i=0; i < mLanguages.keys.Count; ++i) {
				GUILayout.Label("Key: " + mLanguages.keys[i]
					+ " | Value: " + t.values[i].text
				);
			}
		}
	}

	private void LoadDatabase ()
	{
		if(mLanguages.languageCount > 0)
		{
			mCurrentLanguage = mLanguages.languages[0];
			//Debug.Log("Database is not empty. Assigning mCurrentLanguage");
		}
		else
		{
			//Debug.Log("Database is Empty");
		}
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
		}

		GUILayout.EndHorizontal();
	}

	//create the top menu bar
	private void LanguageMenuBar ()
	{

		//the menu bar 
		GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));


		//select the language you want to add
		mNewSystemLanguage = (SystemLanguage) EditorGUILayout.EnumPopup("Add New Language:", mNewSystemLanguage, EditorStyles.toolbarDropDown, GUILayout.Width(250));

		//add the language you have selected
		if(GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20)))
		{
			mLanguages.AddLanguage(mNewSystemLanguage);
			//Debug.Log("Add:" + mNewSystemLanguage.ToString());
		}

		EditorGUILayout.Space();

		//select the language you want to display
		DoLanguageSelectionPopup();
		//Debug.Log(currentLoadedLanguageListSelection);

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

		int selectionIndex = 0;

		if( loadedLanguages == null || loadedLanguages.Length != mLanguages.languageCount ) {
			loadedLanguages = mLanguages.languages;
			loadedLanguagesString = new string[loadedLanguages.Length];
			for(int i = 0; i < loadedLanguages.Length; ++i) {
				loadedLanguagesString[i] = loadedLanguages[i].ToString();
				if(loadedLanguages[i] == mCurrentLanguage) {
					selectionIndex = i;
				}
			}
		}

		//select the language you want to display
		selectionIndex = EditorGUILayout.Popup(selectionIndex, loadedLanguagesString, EditorStyles.toolbarPopup);
		mCurrentLanguage = loadedLanguages[selectionIndex];
	}

	public void DetectLanguageFileFromSelection ()
	{
	/*
		mLanguages = null;

		if (Selection.activeObject == null && mLanguages == null)
		{
			mLanguages = null;
		}
		*/
		if (Selection.activeObject is M10NStringDatabase && EditorUtility.IsPersistent(Selection.activeObject))
		{
			mLanguages = Selection.activeObject as M10NStringDatabase;
			LoadDatabase();
			isLoaded = true;
			//Debug.Log("Language Asset Selected and Loading");
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
